using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform soundBT_On;

	[SerializeField]
	private Transform soundBT_Off;

	[SerializeField]
	private Transform musicBT_On;

	[SerializeField]
	private Transform musicBT_Off;

	[SerializeField]
	private Transform vibrationBT_On;

	[SerializeField]
	private Transform vibrationBT_Off;

	[SerializeField]
	private Transform notificationBT_On;

	[SerializeField]
	private Transform notificationBT_Off;

	[SerializeField]
	private List<string> dropdownList;

	[SerializeField]
	private Dropdown language;

	[SerializeField]
	private Transform android;

	[SerializeField]
	private Transform apple;

	[SerializeField]
	private Transform connected;

	[SerializeField]
	private Text TestcaUid_Text;

	[SerializeField]
	private Text Testversion_Text;

	public new void Show()
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		Init();
		LobbyManager.HideHunterLobby();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	public void OsLoginSuccess()
	{
		apple.gameObject.SetActive(value: false);
		android.gameObject.SetActive(value: false);
		connected.gameObject.SetActive(value: true);
	}

	private void Init()
	{
		SettingOption();
		SettingOsLogin();
		SetLanguageDropdown();
		TestcaUid_Text.text = "UID : " + GameInfo.caUid;
		Testversion_Text.text = "VERSION : " + Application.version;
	}

	private void SettingOption()
	{
		if (GameInfo.IS_MUSIC_SOUND)
		{
			musicBT_On.gameObject.SetActive(value: true);
			musicBT_Off.gameObject.SetActive(value: false);
		}
		else
		{
			musicBT_On.gameObject.SetActive(value: false);
			musicBT_Off.gameObject.SetActive(value: true);
		}
		if (GameInfo.IS_EFFECT_SOUND)
		{
			soundBT_On.gameObject.SetActive(value: true);
			soundBT_Off.gameObject.SetActive(value: false);
		}
		else
		{
			soundBT_On.gameObject.SetActive(value: false);
			soundBT_Off.gameObject.SetActive(value: true);
		}
		if (GameInfo.IS_VIBRATION)
		{
			vibrationBT_On.gameObject.SetActive(value: true);
			vibrationBT_Off.gameObject.SetActive(value: false);
		}
		else
		{
			vibrationBT_On.gameObject.SetActive(value: false);
			vibrationBT_Off.gameObject.SetActive(value: true);
		}
		if (GameInfo.IS_NOTIFICATIONS)
		{
			notificationBT_On.gameObject.SetActive(value: true);
			notificationBT_Off.gameObject.SetActive(value: false);
		}
		else
		{
			notificationBT_On.gameObject.SetActive(value: false);
			notificationBT_Off.gameObject.SetActive(value: true);
		}
		language.value = (int)GameInfo.CURRENTLANGUAGE;
	}

	private void SettingOsLogin()
	{
		if (MasterPlatformService.LoginState)
		{
			apple.gameObject.SetActive(value: false);
			android.gameObject.SetActive(value: false);
			connected.gameObject.SetActive(value: true);
			return;
		}
		switch (BuildSet.CurrentPlatformType)
		{
		case PlatformType.aos:
			apple.gameObject.SetActive(value: false);
			android.gameObject.SetActive(value: true);
			connected.gameObject.SetActive(value: false);
			break;
		case PlatformType.ios:
			apple.gameObject.SetActive(value: true);
			android.gameObject.SetActive(value: false);
			connected.gameObject.SetActive(value: false);
			break;
		}
	}

	private void SetLanguageDropdown()
	{
		if (dropdownList != null)
		{
			dropdownList = null;
		}
		dropdownList = new List<string>();
		language.ClearOptions();
		if (MasterLocalize.GetLanguage == 0)
		{
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_korean"));
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_english"));
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_russian"));
		}
		else if (MasterLocalize.GetLanguage == 1)
		{
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_korean"));
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_english"));
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_russian"));
		}
		else
		{
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_korean"));
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_english"));
			dropdownList.Add(MasterLocalize.GetData("popup_setting_text_russian"));
		}
		language.AddOptions(dropdownList);
	}

	public void Click_SoundBT()
	{
		if (GameInfo.IS_EFFECT_SOUND)
		{
			soundBT_On.gameObject.SetActive(value: false);
			soundBT_Off.gameObject.SetActive(value: true);
			GamePreferenceManager.SetIsEffectSound(isOnOff: false);
		}
		else
		{
			soundBT_On.gameObject.SetActive(value: true);
			soundBT_Off.gameObject.SetActive(value: false);
			GamePreferenceManager.SetIsEffectSound(isOnOff: true);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	public void Click_MusicBT()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (GameInfo.IS_MUSIC_SOUND)
		{
			musicBT_On.gameObject.SetActive(value: false);
			musicBT_Off.gameObject.SetActive(value: true);
			GamePreferenceManager.SetIsMusicSound(isOnOff: false);
			SoundController.BGM_Stop(MusicSoundType.LobbyBGM);
		}
		else
		{
			musicBT_On.gameObject.SetActive(value: true);
			musicBT_Off.gameObject.SetActive(value: false);
			GamePreferenceManager.SetIsMusicSound(isOnOff: true);
			SoundController.BGM_Play(MusicSoundType.LobbyBGM);
		}
	}

	public void Click_VibrationBT()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (GameInfo.IS_VIBRATION)
		{
			vibrationBT_On.gameObject.SetActive(value: false);
			vibrationBT_Off.gameObject.SetActive(value: true);
			GamePreferenceManager.SetIsVibration(isOnOff: false);
		}
		else
		{
			vibrationBT_On.gameObject.SetActive(value: true);
			vibrationBT_Off.gameObject.SetActive(value: false);
			GamePreferenceManager.SetIsVibration(isOnOff: true);
		}
	}

	public void Click_NotificationBT()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (GameInfo.IS_NOTIFICATIONS)
		{
			notificationBT_On.gameObject.SetActive(value: false);
			notificationBT_Off.gameObject.SetActive(value: true);
			GamePreferenceManager.SetIsNotification(isOnOff: false);
			LocalPushManager.CancelAllNotification();
		}
		else
		{
			notificationBT_On.gameObject.SetActive(value: true);
			notificationBT_Off.gameObject.SetActive(value: false);
			GamePreferenceManager.SetIsNotification(isOnOff: true);
		}
	}

	public void Click_LanguageBT(int _idx)
	{
		switch (_idx)
		{
		case 1:
			MasterLocalize.ChangeLanguage(1);
			GameInfo.CURRENTLANGUAGE = Language_Type.English;
			GamePreferenceManager.SetLanguage(Language_Type.English);
			break;
		case 0:
			MasterLocalize.ChangeLanguage(0);
			GameInfo.CURRENTLANGUAGE = Language_Type.Korean;
			GamePreferenceManager.SetLanguage(Language_Type.Korean);
			break;
		case 2:
			MasterLocalize.ChangeLanguage(2);
			GameInfo.CURRENTLANGUAGE = Language_Type.Russian;
			GamePreferenceManager.SetLanguage(Language_Type.Russian);
			break;
		}
	}

	public void Click_Google_Login()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		Protocol_Set.CallSocialLoginConnect();
	}

	public void Click_Apple_Login()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		Protocol_Set.CallSocialLoginConnect();
	}

	public void Click_PrivacyPolicy()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		Application.OpenURL("http://matchhero.cookappsgames.com/mw_match/policy/privacy.html");
	}

	public void Click_Support()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		Application.OpenURL("https://cookapps.zendesk.com/hc/en-us/requests/new");
	}

	public void OnClickGoBack()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}
}
