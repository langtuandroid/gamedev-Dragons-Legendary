using System;
using UnityEngine;

public class GameQuitPopup : LobbyPopupBase
{
	public Action GoBackEvent;

	public void OnClickQuit()
	{
		Application.Quit();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickContinue()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}
}
