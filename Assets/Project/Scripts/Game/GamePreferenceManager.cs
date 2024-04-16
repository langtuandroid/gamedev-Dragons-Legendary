using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class GamePreferenceManager : MonoBehaviour
{
	public static void SetIsEffectSound(bool isOnOff)
	{
		GameInfo.IS_EFFECT_SOUND = isOnOff;
		if (isOnOff)
		{
			ObscuredPrefs.SetBool("IsEffectSound", value: true);
		}
		else
		{
			ObscuredPrefs.SetBool("IsEffectSound", value: false);
		}
	}

	public static bool GetIsEffectSound()
	{
		bool flag = false;
		return GameInfo.IS_EFFECT_SOUND = ObscuredPrefs.GetBool("IsEffectSound", defaultValue: true);
	}

	public static void SetIsMusicSound(bool isOnOff)
	{
		GameInfo.IS_MUSIC_SOUND = isOnOff;
		if (isOnOff)
		{
			ObscuredPrefs.SetBool("IsMusicSound", value: true);
		}
		else
		{
			ObscuredPrefs.SetBool("IsMusicSound", value: false);
		}
	}

	public static bool GetIsMusicSound()
	{
		bool flag = false;
		return GameInfo.IS_MUSIC_SOUND = ObscuredPrefs.GetBool("IsMusicSound", defaultValue: true);
	}

	public static void SetIsVibration(bool isOnOff)
	{
		GameInfo.IS_VIBRATION = isOnOff;
		if (isOnOff)
		{
			ObscuredPrefs.SetBool("IsVibration", value: true);
		}
		else
		{
			ObscuredPrefs.SetBool("IsVibration", value: false);
		}
	}

	public static bool GetIsVibration()
	{
		bool flag = false;
		return GameInfo.IS_VIBRATION = ObscuredPrefs.GetBool("IsVibration", defaultValue: true);
	}

	public static void SetIsNotification(bool isOnOff)
	{
		GameInfo.IS_NOTIFICATIONS = isOnOff;
		if (isOnOff)
		{
			ObscuredPrefs.SetBool("IsNotification", value: true);
		}
		else
		{
			ObscuredPrefs.SetBool("IsNotification", value: false);
		}
	}

	public static bool GetIsNotification()
	{
		bool flag = false;
		return GameInfo.IS_NOTIFICATIONS = ObscuredPrefs.GetBool("IsNotification", defaultValue: true);
	}

	public static void SetLanguage(Language_Type languageType)
	{
		GameInfo.CURRENTLANGUAGE = languageType;
		ObscuredPrefs.SetInt("Language", (int)languageType);
	}

	public static Language_Type GetLanguage()
	{
		Language_Type language_Type = Language_Type.English;
		return GameInfo.CURRENTLANGUAGE = (Language_Type)ObscuredPrefs.GetInt("Language", 0);
	}

	public static void SetIsAnalytics(string _type)
	{
		ObscuredPrefs.SetBool(_type, value: true);
	}

	public static bool GetIsAnalytics(string _type)
	{
		bool flag = false;
		return ObscuredPrefs.GetBool(_type, defaultValue: false);
	}

	public static void SetIsGetHunter20025(bool isGet)
	{
		ObscuredPrefs.SetBool("IsGetHunter20025", isGet);
	}

	public static bool GetIsGetHunter20025()
	{
		bool flag = false;
		return ObscuredPrefs.GetBool("IsGetHunter20025", defaultValue: false);
	}

	public static void SetIsRate()
	{
		ObscuredPrefs.SetBool("IsRate", value: true);
	}

	public static bool GetIsRate()
	{
		bool flag = false;
		return ObscuredPrefs.GetBool("IsRate", defaultValue: false);
	}

	public static void SetIsBoostRewardVideo(bool isBoostRewardVideo)
	{
		ObscuredPrefs.SetBool("IsBoostRewardVideo", isBoostRewardVideo);
	}

	public static bool GetIsBoostRewardVideo()
	{
		bool flag = false;
		return ObscuredPrefs.GetBool("IsBoostRewardVideo", defaultValue: false);
	}
}
