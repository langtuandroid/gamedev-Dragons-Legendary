using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalTimeCheckManager : MonoBehaviour
{
	public static Action<string, float> OnTimeTick;

	public static Action<string> OnLocalTimeComplete;

	private static LocalTimeCheckManager instance;

	private List<string> listReadyTimeCheck = new List<string>();

	private Dictionary<string, double> dicTimeCheck = new Dictionary<string, double>();

	private Dictionary<string, Coroutine> dicTimeCoroutine = new Dictionary<string, Coroutine>();

	private uint Timestamp => (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Ticks;

	public static TimeCheckState GetTimeState(string type)
	{
		double num = 0.0;
		if (instance.dicTimeCheck.ContainsKey(type))
		{
			num = instance.dicTimeCheck[type];
		}
		if (num < 0.0)
		{
			return TimeCheckState.Nothing;
		}
		if (num == 0.0)
		{
			return TimeCheckState.Complete;
		}
		return TimeCheckState.Progress;
	}

	public static double GetSecond(string type)
	{
		double result = 0.0;
		if (instance.dicTimeCheck.ContainsKey(type))
		{
			result = instance.dicTimeCheck[type];
		}
		return result;
	}

	public static void InitType(string type)
	{
		double @double = ObscuredPrefs.GetDouble(type.ToString(), -1.0);
		if (@double > 0.0)
		{
			double totalMilliseconds = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
			float num = (float)((@double - totalMilliseconds) / 1000.0);
			if (num > 0f)
			{
				AddTimer(type, num);
			}
		}
	}

	public static void AddTimer(string type, float second)
	{
		instance.RefreshTimeData(type, second);
		if (instance.dicTimeCoroutine.ContainsKey(type))
		{
			instance.StopCoroutine(instance.dicTimeCoroutine[type]);
			instance.dicTimeCoroutine.Remove(type);
		}
		Coroutine value = instance.StartCoroutine(instance.StartCheckLocalTimer(type, second));
		instance.dicTimeCoroutine.Add(type, value);
	}

	public static void AddDuration(string type, float second, bool isPlay = true)
	{
		if (instance.dicTimeCoroutine.ContainsKey(type))
		{
			instance.StopCoroutine(instance.dicTimeCoroutine[type]);
			instance.dicTimeCoroutine.Remove(type);
		}
		if (instance.dicTimeCheck.ContainsKey(type))
		{
			second += (float)instance.dicTimeCheck[type];
			if (isPlay)
			{
				Coroutine value = instance.StartCoroutine(instance.StartCheckLocalTimer(type, second));
				instance.dicTimeCoroutine.Add(type, value);
			}
			return;
		}
		double @double = ObscuredPrefs.GetDouble(type.ToString(), -1.0);
		if (@double > 0.0)
		{
			double totalMilliseconds = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
			float num = (float)((@double - totalMilliseconds) / 1000.0);
			if (num > 0f)
			{
				num += second;
			}
		}
	}

	public static void TimeComplete(string type)
	{
		if (type != null)
		{
			if (instance.dicTimeCoroutine.ContainsKey(type) && instance.dicTimeCoroutine[type] != null)
			{
				instance.StopCoroutine(instance.dicTimeCoroutine[type]);
			}
			instance.dicTimeCoroutine.Remove(type);
			if (instance.dicTimeCheck.ContainsKey(type))
			{
				instance.dicTimeCheck[type] = 0.0;
			}
		}
	}

	public static void TimeNothing(string type)
	{
		if (instance.dicTimeCheck.ContainsKey(type))
		{
			instance.dicTimeCheck.Remove(type);
		}
	}

	public static void TimeClear(string type)
	{
		if (!(instance == null))
		{
			TimeComplete(type);
			TimeNothing(type);
		}
	}

	public static void SaveAndExit(string type)
	{
		if (!(instance == null) && type != null && !(type == string.Empty) && instance.dicTimeCheck.ContainsKey(type) && instance.dicTimeCheck[type] > 0.0)
		{
			double totalMilliseconds = (DateTime.UtcNow.AddSeconds(instance.dicTimeCheck[type]) - new DateTime(1970, 1, 1)).TotalMilliseconds;
			if (instance.dicTimeCoroutine.ContainsKey(type))
			{
				instance.StopCoroutine(instance.dicTimeCoroutine[type]);
			}
			instance.dicTimeCheck.Remove(type);
			instance.dicTimeCoroutine.Remove(type);
			ObscuredPrefs.DeleteKey(type);
		}
	}

	public static void Clear()
	{
		foreach (KeyValuePair<string, Coroutine> item in instance.dicTimeCoroutine)
		{
			instance.StopCoroutine(item.Value);
		}
		foreach (KeyValuePair<string, double> item2 in instance.dicTimeCheck)
		{
			ObscuredPrefs.DeleteKey(item2.Key);
		}
		instance.dicTimeCheck.Clear();
		instance.dicTimeCoroutine.Clear();
	}

	private void StartReadyTimeCheck()
	{
		foreach (string item in listReadyTimeCheck)
		{
			InitType(item);
		}
		listReadyTimeCheck.Clear();
	}

	private IEnumerator StartCheckLocalTimer(string type, float second)
	{
		float currentSecond = second;
		float time = Time.time;
		float checkSecond = -1f;
		while (currentSecond > 0f)
		{
			float timeStamp = Time.time;
			yield return null;
			currentSecond -= Time.time - timeStamp;
			RefreshTimeData(type, currentSecond);
			dicTimeCheck[type] = currentSecond;
			if (OnTimeTick != null && checkSecond != Mathf.Floor(currentSecond))
			{
				checkSecond = Mathf.Floor(currentSecond);
				OnTimeTick(type, currentSecond);
			}
		}
		dicTimeCheck[type] = 0.0;
		dicTimeCoroutine.Remove(type);
		SaveAndExit(type);
		if (OnLocalTimeComplete != null)
		{
			OnLocalTimeComplete(type);
		}
	}

	private void RefreshTimeData(string type, float second)
	{
		double totalMilliseconds = (DateTime.UtcNow.AddSeconds(second) - new DateTime(1970, 1, 1)).TotalMilliseconds;
		if (!dicTimeCheck.ContainsKey(type))
		{
			dicTimeCheck.Add(type, second);
		}
		else
		{
			dicTimeCheck[type] = second;
		}
	}

	private void CheckLocalTime(string type, double second)
	{
		double totalMilliseconds = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
		float num = (float)((second - totalMilliseconds) / 1000.0);
		if (num > 0f)
		{
			AddTimer(type, num);
			return;
		}
		dicTimeCheck[type] = 0.0;
		if (OnLocalTimeComplete != null)
		{
			OnLocalTimeComplete(type);
		}
	}

	private void SaveLocalTimeData()
	{
		foreach (KeyValuePair<string, double> item in dicTimeCheck)
		{
			if (item.Value > 0.0)
			{
				double totalMilliseconds = (DateTime.UtcNow.AddSeconds(item.Value) - new DateTime(1970, 1, 1)).TotalMilliseconds;
				ObscuredPrefs.SetDouble(item.Key.ToString(), totalMilliseconds);
			}
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		UnityEngine.Debug.Log("OnApplicationPause - pauseStatus :: " + pauseStatus + ", " + dicTimeCheck.Count);
		if (pauseStatus)
		{
			StopAllCoroutines();
			dicTimeCheck.Clear();
			dicTimeCoroutine.Clear();
			listReadyTimeCheck.Clear();
		}
	}

	private void OnApplicationQuit()
	{
		StopAllCoroutines();
	}
}
