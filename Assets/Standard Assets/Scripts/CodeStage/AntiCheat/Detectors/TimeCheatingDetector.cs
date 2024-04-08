using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Time Cheating Detector")]
	[HelpURL("http://codestage.net/uas_files/actk/api/class_code_stage_1_1_anti_cheat_1_1_detectors_1_1_time_cheating_detector.html")]
	public class TimeCheatingDetector : ActDetectorBase
	{
		private struct AsyncCallbackData
		{
			public Action<double> callback;

			public double data;
		}

		internal const string COMPONENT_NAME = "Time Cheating Detector";

		private const string LOG_PREFIX = "[ACTk] Time Cheating Detector: ";

		private const string TIME_SERVER = "pool.ntp.org";

		private const int NTP_DATA_BUFFER_LENGTH = 48;

		private static int instancesInScene;

		private readonly DateTime date1900 = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private readonly Queue<AsyncCallbackData> asyncResultQueue = new Queue<AsyncCallbackData>(1);

		private readonly List<AsyncCallbackData> resultList = new List<AsyncCallbackData>(1);

		private readonly object lockObject = new object();

		private float timeElapsed;

		protected UnityAction errorAction;

		[Tooltip("Time (in minutes) between detector checks.")]
		[Range(0.1f, 60f)]
		public int interval = 1;

		[Tooltip("Maximum allowed difference between online and offline time, in minutes.")]
		public int threshold = 65;

		private Socket asyncSocket;

		private Action<double> getOnlineTimeCallback;

		private string targetHost;

		private byte[] targetIP;

		private IPEndPoint targetEndpoint;

		private byte[] ntpData = new byte[48];

		private SocketAsyncEventArgs connectArgs;

		private SocketAsyncEventArgs sendArgs;

		private SocketAsyncEventArgs receiveArgs;

		private bool checkingForCheat;

		public bool IsCheckingForCheat => checkingForCheat;

		public static TimeCheatingDetector Instance
		{
			get;
			private set;
		}

		private static TimeCheatingDetector GetOrCreateInstance
		{
			get
			{
				if (Instance != null)
				{
					return Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				Instance = ActDetectorBase.detectorsContainer.AddComponent<TimeCheatingDetector>();
				return Instance;
			}
		}

		private TimeCheatingDetector()
		{
		}

		public static void StartDetection()
		{
			if (Instance != null)
			{
				Instance.StartDetectionInternal(null, null, Instance.interval);
			}
			else
			{
				UnityEngine.Debug.LogError("[ACTk] Time Cheating Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		public static void StartDetection(UnityAction detectionCallback, UnityAction errorCallback = null)
		{
			StartDetection(detectionCallback, errorCallback, GetOrCreateInstance.interval);
		}

		public static void StartDetection(UnityAction detectionCallback, int interval)
		{
			StartDetection(detectionCallback, null, interval);
		}

		public static void StartDetection(UnityAction detectionCallback, UnityAction errorCallback, int interval)
		{
			GetOrCreateInstance.StartDetectionInternal(detectionCallback, errorCallback, interval);
		}

		public static void StopDetection()
		{
			if (Instance != null)
			{
				Instance.StopDetectionInternal();
			}
		}

		public static void SetErrorCallback(UnityAction errorCallback)
		{
			if (Instance != null)
			{
				Instance.errorAction = errorCallback;
			}
			else
			{
				UnityEngine.Debug.LogError("[ACTk] Time Cheating Detector: Can't set error callback since detector is not created or initialized yet.");
			}
		}

		public static void Dispose()
		{
			if (Instance != null)
			{
				Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			instancesInScene++;
			if (Init(Instance, "Time Cheating Detector"))
			{
				Instance = this;
			}
			SceneManager.sceneLoaded += OnLevelWasLoadedNew;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			instancesInScene--;
		}

		private void OnLevelWasLoadedNew(Scene scene, LoadSceneMode mode)
		{
			OnLevelLoadedCallback();
		}

		private void OnLevelLoadedCallback()
		{
			if (instancesInScene < 2)
			{
				if (!keepAlive)
				{
					DisposeInternal();
				}
			}
			else if (!keepAlive && Instance != this)
			{
				DisposeInternal();
			}
		}

		private void Update()
		{
			if (!started || !isRunning)
			{
				return;
			}
			timeElapsed += Time.unscaledDeltaTime;
			if (timeElapsed >= (float)(interval * 60))
			{
				timeElapsed = 0f;
				CheckForCheat();
			}
			else if (asyncResultQueue != null && asyncResultQueue.Count != 0)
			{
				lock (lockObject)
				{
					while (asyncResultQueue.Count > 0)
					{
						AsyncCallbackData item = asyncResultQueue.Dequeue();
						resultList.Add(item);
					}
				}
				if (resultList.Count > 0)
				{
					foreach (AsyncCallbackData result in resultList)
					{
						AsyncCallbackData current = result;
						current.callback(current.data);
					}
					resultList.Clear();
				}
			}
		}

		public void ForceCheck()
		{
			if (!started || !isRunning)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Time Cheating Detector: Detector should be started to use ForceCheck().");
				return;
			}
			if (IsCheckingForCheat)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Time Cheating Detector: Can't force cheating check since another check is already in progress.");
				return;
			}
			timeElapsed = 0f;
			CheckForCheat();
		}

		private void StartDetectionInternal(UnityAction detectionCallback, UnityAction errorCallback, int checkInterval)
		{
			if (isRunning)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Time Cheating Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Time Cheating Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (detectionCallback != null && detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Time Cheating Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (detectionCallback == null && !detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Time Cheating Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			timeElapsed = 0f;
			detectionAction = detectionCallback;
			errorAction = errorCallback;
			interval = checkInterval;
			started = true;
			isRunning = true;
		}

		protected override void StartDetectionAutomatically()
		{
			StartDetectionInternal(null, null, interval);
		}

		protected override void PauseDetector()
		{
			isRunning = false;
		}

		protected override void ResumeDetector()
		{
			if (detectionAction != null || detectionEventHasListener)
			{
				isRunning = true;
			}
		}

		protected override void StopDetectionInternal()
		{
			if (started)
			{
				detectionAction = null;
				started = false;
				isRunning = false;
			}
		}

		protected override void DisposeInternal()
		{
			if (Instance == this)
			{
				Instance = null;
			}
			if (asyncSocket != null && asyncSocket.Connected)
			{
				asyncSocket.Close();
			}
			base.DisposeInternal();
		}

		private void CheckForCheat()
		{
			if (isRunning)
			{
				checkingForCheat = true;
				GetOnlineTimeAsync("pool.ntp.org", OnTimeGot);
			}
		}

		private void OnTimeGot(double onlineTime)
		{
			checkingForCheat = false;
			if (!started || !isRunning)
			{
				return;
			}
			if (onlineTime <= 0.0)
			{
				if (errorAction != null)
				{
					errorAction();
				}
				return;
			}
			double localTime = GetLocalTime();
			TimeSpan timeSpan = new TimeSpan((long)onlineTime * 10000);
			TimeSpan timeSpan2 = new TimeSpan((long)localTime * 10000);
			double value = timeSpan.TotalMinutes - timeSpan2.TotalMinutes;
			if (Math.Abs(value) > (double)threshold)
			{
				OnCheatingDetected();
			}
		}

		public static double GetOnlineTime(string server)
		{
			try
			{
				byte[] array = new byte[48];
				array[0] = 27;
				IPAddress[] addressList = Dns.GetHostEntry(server).AddressList;
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				socket.Connect(new IPEndPoint(addressList[0], 123));
				socket.ReceiveTimeout = 3000;
				socket.Send(array);
				socket.Receive(array);
				socket.Close();
				ulong num = ((ulong)array[40] << 24) | ((ulong)array[41] << 16) | ((ulong)array[42] << 8) | array[43];
				ulong num2 = ((ulong)array[44] << 24) | ((ulong)array[45] << 16) | ((ulong)array[46] << 8) | array[47];
				return (double)num * 1000.0 + (double)num2 * 1000.0 / 4294967296.0;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("[ACTk] Time Cheating Detector: Could not get NTP time from " + server + " =/\n" + ex);
				return -1.0;
			}
		}

		public void GetOnlineTimeAsync(string server, Action<double> callback)
		{
			try
			{
				IPAddress[] addressList = Dns.GetHostEntry(server).AddressList;
				if (addressList.Length == 0)
				{
					UnityEngine.Debug.Log("[ACTk] Time Cheating Detector: Could not resolve IP from the host " + server + " =/");
					callback(-1.0);
				}
				else
				{
					if (asyncSocket == null)
					{
						asyncSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					}
					targetHost = server;
					IPAddress iPAddress = addressList[0];
					byte[] addressBytes = iPAddress.GetAddressBytes();
					if (addressBytes != targetIP)
					{
						targetEndpoint = new IPEndPoint(iPAddress, 123);
						targetIP = addressBytes;
					}
					if (connectArgs == null)
					{
						connectArgs = new SocketAsyncEventArgs();
						connectArgs.Completed += OnSocketConnected;
					}
					connectArgs.RemoteEndPoint = targetEndpoint;
					asyncSocket.ReceiveTimeout = 3000;
					getOnlineTimeCallback = callback;
					asyncSocket.ConnectAsync(connectArgs);
				}
			}
			catch
			{
				callback(-1.0);
			}
		}

		private void OnSocketConnected(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError != 0)
			{
				UnityEngine.Debug.Log("[ACTk] Time Cheating Detector: Could not get NTP time from " + targetHost + " =/\n" + e);
				SocketAsyncResult(-1.0);
			}
			else if (started && isRunning)
			{
				ntpData[0] = 27;
				if (sendArgs == null)
				{
					sendArgs = new SocketAsyncEventArgs();
					sendArgs.Completed += OnSocketSend;
					sendArgs.UserToken = asyncSocket;
					sendArgs.SetBuffer(ntpData, 0, 48);
				}
				sendArgs.RemoteEndPoint = targetEndpoint;
				asyncSocket.SendAsync(sendArgs);
			}
		}

		private void OnSocketSend(object sender, SocketAsyncEventArgs e)
		{
			if (!started || !isRunning)
			{
				return;
			}
			if (e.SocketError == SocketError.Success)
			{
				if (e.LastOperation == SocketAsyncOperation.Send)
				{
					if (receiveArgs == null)
					{
						receiveArgs = new SocketAsyncEventArgs();
						receiveArgs.Completed += OnSocketReceive;
						receiveArgs.UserToken = asyncSocket;
						receiveArgs.SetBuffer(ntpData, 0, 48);
					}
					receiveArgs.RemoteEndPoint = targetEndpoint;
					asyncSocket.ReceiveAsync(receiveArgs);
				}
			}
			else
			{
				UnityEngine.Debug.Log("[ACTk] Time Cheating Detector: Could not get NTP time from " + targetHost + " =/\n" + e);
				SocketAsyncResult(-1.0);
			}
		}

		private void OnSocketReceive(object sender, SocketAsyncEventArgs e)
		{
			if (started && isRunning)
			{
				ntpData = e.Buffer;
				ulong num = ((ulong)ntpData[40] << 24) | ((ulong)ntpData[41] << 16) | ((ulong)ntpData[42] << 8) | ntpData[43];
				ulong num2 = ((ulong)ntpData[44] << 24) | ((ulong)ntpData[45] << 16) | ((ulong)ntpData[46] << 8) | ntpData[47];
				double time = (double)num * 1000.0 + (double)num2 * 1000.0 / 4294967296.0;
				SocketAsyncResult(time);
			}
		}

		private void SocketAsyncResult(double time)
		{
			if (getOnlineTimeCallback != null)
			{
				lock (lockObject)
				{
					AsyncCallbackData item = default(AsyncCallbackData);
					item.callback = getOnlineTimeCallback;
					item.data = time;
					asyncResultQueue.Enqueue(item);
				}
			}
		}

		private double GetLocalTime()
		{
			return DateTime.UtcNow.Subtract(date1900).TotalMilliseconds;
		}
	}
}
