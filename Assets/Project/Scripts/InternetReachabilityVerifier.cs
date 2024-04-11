#if ENABLE_HTTP_BEST
using BestHTTP;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetReachabilityVerifier : MonoBehaviour
{
	public enum CaptivePortalDetectionMethod
	{
		DefaultByPlatform,
		Google204,
		GoogleBlank,
		MicrosoftNCSI,
		Apple,
		Ubuntu,
		Custom,
		Apple2,
		AppleHTTPS
	}

	public enum Status
	{
		Offline,
		PendingVerification,
		Error,
		Mismatch,
		NetVerified
	}

	public delegate void StatusChangedDelegate(Status newStatus);

	public delegate bool CustomMethodVerifierDelegate(WWW www, string customMethodExpectedData);

	public CaptivePortalDetectionMethod captivePortalDetectionMethod;

	[Tooltip("Self-hosted URL for using CaptivePortalDetectionMethod.Custom. For example: https://example.com/IRV.txt")]
	public string customMethodURL = string.Empty;

	[Tooltip("Data expected from the custom self-hosted URL. By default the data returned by the custom url is expected to start with contents of this string. Alternatively you can set the customMethodVerifierDelegate (see example code), in which case this string will be passed to the delegate.")]
	public string customMethodExpectedData = "OK";

	[Tooltip("Makes the IRV object not be destroyed automatically when loading a new scene.")]
	public bool dontDestroyOnLoad = true;

	[Tooltip("When enabled, custom method URL is appended with a query string containing a random number.\nExample of what such a query string may look like: ?z=13371337")]
	public bool customMethodWithCacheBuster = true;

	[Tooltip("Default time in seconds to wait until trying to verify network connectivity again.\nSuggested minimum: 1 second.")]
	public float defaultCheckPeriod = 4f;

	[Tooltip("Time in seconds to wait before retrying, after last verification attempt resulted in an error.\nSuggested minimum: 3 seconds.")]
	public float errorRetryDelay = 15f;

	[Tooltip("Time in seconds to wait after detecting a captive portal (WiFi login screen).\nSuggested minimum: 2 seconds.")]
	public float mismatchRetryDelay = 7f;

	public CustomMethodVerifierDelegate customMethodVerifierDelegate;

	private float noInternetStartTime;

	[SerializeField]
	private Status _status;

	private string _lastError = string.Empty;

	private static InternetReachabilityVerifier _instance = null;

	private static RuntimePlatform[] methodGoogle204Supported = new RuntimePlatform[6]
	{
		RuntimePlatform.WindowsPlayer,
		RuntimePlatform.WindowsEditor,
		RuntimePlatform.Android,
		RuntimePlatform.LinuxPlayer,
		RuntimePlatform.OSXPlayer,
		RuntimePlatform.OSXEditor
	};

	private const CaptivePortalDetectionMethod fallbackMethodIfNoDefaultByPlatform = CaptivePortalDetectionMethod.MicrosoftNCSI;

	private bool netActivityRunning;

	private string apple2MethodURL = string.Empty;

	private float _yieldWaitStart;

	public Status status
	{
		get
		{
			return _status;
		}
		set
		{
			Status status = _status;
			_status = value;
			if (status == Status.NetVerified && _status != Status.NetVerified)
			{
				noInternetStartTime = Time.realtimeSinceStartup;
			}
			if (this.statusChangedDelegate != null)
			{
				this.statusChangedDelegate(value);
			}
		}
	}

	public string lastError
	{
		get
		{
			return _lastError;
		}
		set
		{
			_lastError = value;
		}
	}

	public static InternetReachabilityVerifier Instance => _instance;

	public event StatusChangedDelegate statusChangedDelegate;

	public float getTimeWithoutInternetConnection()
	{
		if (status == Status.NetVerified)
		{
			return 0f;
		}
		return Time.realtimeSinceStartup - noInternetStartTime;
	}

	public IEnumerator waitForNetVerifiedStatus()
	{
		if (status != Status.NetVerified)
		{
			forceReverification();
		}
		while (status != Status.NetVerified)
		{
			yield return null;
		}
	}

	public void setNetActivityTimes(float defaultCheckPeriodSeconds, float errorRetryDelaySeconds, float mismatchRetryDelaySeconds)
	{
		defaultCheckPeriod = defaultCheckPeriodSeconds;
		errorRetryDelay = errorRetryDelaySeconds;
		mismatchRetryDelay = mismatchRetryDelaySeconds;
	}

	public void forceReverification()
	{
		status = Status.PendingVerification;
	}

	private string getCaptivePortalDetectionURL(CaptivePortalDetectionMethod cpdm)
	{
		switch (cpdm)
		{
			case CaptivePortalDetectionMethod.Custom:
				{
					string text = customMethodURL;
					if (customMethodWithCacheBuster)
					{
						text = text + "?z=" + (UnityEngine.Random.Range(0, int.MaxValue) ^ 0x13377AA7);
					}
					return text;
				}
			case CaptivePortalDetectionMethod.Google204:
				return "http://clients3.google.com/generate_204";
			case CaptivePortalDetectionMethod.MicrosoftNCSI:
				return "http://www.msftncsi.com/ncsi.txt";
			case CaptivePortalDetectionMethod.GoogleBlank:
				return "http://www.google.com/blank.html";
			case CaptivePortalDetectionMethod.Apple:
				return "http://www.apple.com/library/test/success.html";
			case CaptivePortalDetectionMethod.Ubuntu:
				return "http://start.ubuntu.com/connectivity-check";
			case CaptivePortalDetectionMethod.Apple2:
				if (apple2MethodURL.Length == 0)
				{
					apple2MethodURL = "http://captive.apple.com/";
					char[] array = new char[17];
					for (int i = 0; i < 17; i++)
					{
						array[i] = (char)(97 + UnityEngine.Random.Range(0, 26));
					}
					array[8] = '/';
					apple2MethodURL += new string(array);
				}
				return apple2MethodURL;
			case CaptivePortalDetectionMethod.AppleHTTPS:
				return "https://www.apple.com/library/test/success.html";
			default:
				return string.Empty;
		}
	}

	private bool checkCaptivePortalDetectionResult(CaptivePortalDetectionMethod cpdm, WWW www)
	{
		if (www == null)
		{
			return false;
		}
		if (www.error != null && www.error.Length > 0)
		{
			return false;
		}
		switch (cpdm)
		{
			case CaptivePortalDetectionMethod.Custom:
				if (customMethodVerifierDelegate != null)
				{
					return customMethodVerifierDelegate(www, customMethodExpectedData);
				}
				if ((customMethodExpectedData.Length > 0 && www.text != null && www.text.StartsWith(customMethodExpectedData)) || (customMethodExpectedData.Length == 0 && (www.bytes == null || www.bytes.Length == 0)))
				{
					return true;
				}
				break;
			case CaptivePortalDetectionMethod.Google204:
				{
					Dictionary<string, string> responseHeaders = www.responseHeaders;
					if (responseHeaders != null && responseHeaders.Keys != null && responseHeaders.Keys.Count > 0)
					{
						string text2 = string.Empty;
						if (responseHeaders.ContainsKey("STATUS"))
						{
							text2 = responseHeaders["STATUS"];
						}
						else if (responseHeaders.ContainsKey("NULL"))
						{
							text2 = responseHeaders["NULL"];
						}
						if (text2.Length > 0 && text2.IndexOf("204 No Content") >= 0)
						{
							return true;
						}
					}
					else if (www.bytesDownloaded == 0)
					{
						return true;
					}
					break;
				}
			case CaptivePortalDetectionMethod.GoogleBlank:
				if (www.bytesDownloaded == 0)
				{
					return true;
				}
				break;
			case CaptivePortalDetectionMethod.MicrosoftNCSI:
				if (www.text.StartsWith("Microsoft NCSI"))
				{
					return true;
				}
				break;
			case CaptivePortalDetectionMethod.Apple:
			case CaptivePortalDetectionMethod.Apple2:
			case CaptivePortalDetectionMethod.AppleHTTPS:
				{
					string text = www.text.ToLower();
					int num = text.IndexOf("<body>success</body>");
					int num2 = text.IndexOf("<title>success</title>");
					if ((num >= 0 && num < 500) || (num2 >= 0 && num2 < 500))
					{
						return true;
					}
					break;
				}
			case CaptivePortalDetectionMethod.Ubuntu:
				if (www.text.IndexOf("Lorem ipsum dolor sit amet") == 109)
				{
					return true;
				}
				break;
		}
		return false;
	}

	private bool internal_yieldWait(float seconds)
	{
		if (_yieldWaitStart == 0f)
		{
			_yieldWaitStart = Time.realtimeSinceStartup;
		}
		bool flag = Time.realtimeSinceStartup - _yieldWaitStart < seconds;
		if (!flag)
		{
			_yieldWaitStart = 0f;
		}
		return flag;
	}

	private IEnumerator netActivity()
	{
		netActivityRunning = true;
		NetworkReachability prevUnityReachability = Application.internetReachability;
		if (Application.internetReachability != 0)
		{
			status = Status.PendingVerification;
		}
		else
		{
			status = Status.Offline;
		}
		noInternetStartTime = Time.realtimeSinceStartup;
		while (netActivityRunning)
		{
			if (status == Status.Error)
			{
				while (internal_yieldWait(errorRetryDelay) && status != Status.PendingVerification)
				{
					yield return null;
				}
				status = Status.PendingVerification;
			}
			else if (status == Status.Mismatch)
			{
				while (internal_yieldWait(mismatchRetryDelay) && status != Status.PendingVerification)
				{
					yield return null;
				}
				status = Status.PendingVerification;
			}
			NetworkReachability unityReachability = Application.internetReachability;
			if (prevUnityReachability != unityReachability)
			{
				UnityEngine.Debug.Log("IRV unity reachability changed: " + unityReachability, this);
				if (unityReachability != 0)
				{
					status = Status.PendingVerification;
				}
				else if (unityReachability == NetworkReachability.NotReachable)
				{
					status = Status.Offline;
				}
				prevUnityReachability = Application.internetReachability;
			}
			if (status == Status.PendingVerification)
			{
				verifyCaptivePortalDetectionMethod();
				CaptivePortalDetectionMethod cpdm = captivePortalDetectionMethod;
				getCaptivePortalDetectionURL(cpdm);
#if ENABLE_HTTP_BEST
				string url = (Application.platform != RuntimePlatform.Android) ? ("https://bubblecoco_mobile.cookappsgames.com/" + customMethodURL) : ("http://bubblecoco_mobile.cookappsgames.com/" + customMethodURL);
				string www_error = null;
				string rData = null;
				HTTPRequest request = new HTTPRequest(new Uri(url)).Send();
				while (request.State < HTTPRequestStates.Finished)
				{
					yield return new WaitForSeconds(0.1f);
				}
				switch (request.State)
				{
					case HTTPRequestStates.Finished:
						if (request.Response.IsSuccess)
						{
							rData = request.Response.DataAsText;
						}
						else
						{
							www_error = "Error";
						}
						break;
					case HTTPRequestStates.Error:
						www_error = "Error";
						break;
					case HTTPRequestStates.Aborted:
						www_error = "Error";
						break;
					case HTTPRequestStates.ConnectionTimedOut:
						www_error = "Error";
						break;
					case HTTPRequestStates.TimedOut:
						www_error = "Error";
						break;
				}
                if (www_error != null && www_error.Length > 0)
				{
					lastError = www_error;
					status = Status.Error;
					continue;
				}
				bool flag = false;
				if (rData.IndexOf("1") > -1)
				{
					flag = true;
				}
				if (!flag)
				{
					status = Status.Mismatch;
					continue;
				}
#endif
                status = Status.NetVerified;
			}
			while (internal_yieldWait(defaultCheckPeriod) && status != Status.PendingVerification)
			{
				yield return null;
			}
		}
		netActivityRunning = false;
		status = Status.PendingVerification;
	}

	private void Awake()
	{
		if ((bool)_instance)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		_instance = this;
		if (dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void verifyCaptivePortalDetectionMethod()
	{
		if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.DefaultByPlatform)
		{
			captivePortalDetectionMethod = CaptivePortalDetectionMethod.Google204;
			if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.DefaultByPlatform)
			{
				captivePortalDetectionMethod = CaptivePortalDetectionMethod.MicrosoftNCSI;
			}
		}
		if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.Google204 && Array.IndexOf(methodGoogle204Supported, Application.platform) < 0)
		{
			captivePortalDetectionMethod = CaptivePortalDetectionMethod.GoogleBlank;
		}
		if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.Custom && customMethodURL.Length == 0)
		{
			UnityEngine.Debug.LogError("IRV - Custom method is selected but URL is empty, cannot start! (disabling component)", this);
			base.enabled = false;
			if (netActivityRunning)
			{
				Stop();
			}
		}
	}

	private void Start()
	{
		verifyCaptivePortalDetectionMethod();
		if (!netActivityRunning)
		{
			StartCoroutine(netActivity());
		}
	}

	private void OnDisable()
	{
		Stop();
	}

	private void OnEnable()
	{
		Start();
	}

	public void Stop()
	{
		StopCoroutine(netActivity());
		netActivityRunning = false;
	}
}