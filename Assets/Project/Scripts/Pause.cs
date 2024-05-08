using UnityEngine;

public class Pause : MonoBehaviour
{
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
	private Transform Really_Quit;

	public void Init()
	{
		base.gameObject.SetActive(value: true);
		PuzzlePlayManager.LockTouch();
		SettingOption();
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
	}

	private void OnGameEndConnectComplete(GAME_END_RESULT _result)
	{
		PuzzlePlayManager.LoseGameEvent();
		GameDataManager.MoveScene(SceneType.Lobby);
	}

	private void OnArenaGameEndComplete(ARENA_GAME_END_RESULT _data)
	{
		GameDataManager.MoveScene(SceneType.Lobby);
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
			switch (GameInfo.inGamePlayData.inGameType)
			{
			case PuzzleInGameType.Stage:
				SoundController.BGM_Stop(MusicSoundType.IngameBGM);
				SoundController.BGM_Stop(MusicSoundType.InGameDragonBgm);
				break;
			case PuzzleInGameType.Arena:
				SoundController.BGM_Stop(MusicSoundType.ArenaBGM);
				break;
			}
			return;
		}
		musicBT_On.gameObject.SetActive(value: true);
		musicBT_Off.gameObject.SetActive(value: false);
		GamePreferenceManager.SetIsMusicSound(isOnOff: true);
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case PuzzleInGameType.Stage:
			if (GameInfo.inGamePlayData.isDragon == 0)
			{
				SoundController.BGM_Play(MusicSoundType.IngameBGM);
			}
			else
			{
				SoundController.BGM_Play(MusicSoundType.InGameDragonBgm);
			}
			break;
		case PuzzleInGameType.Arena:
			SoundController.BGM_Play(MusicSoundType.ArenaBGM);
			break;
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

	public void Click_Continue()
	{
		PuzzlePlayManager.ActivateTouch();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		base.gameObject.SetActive(value: false);
	}

	public void Click_Quit()
	{
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		Really_Quit.gameObject.SetActive(value: true);
	}

	public void Click_Really_Quit_Continue()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		Really_Quit.gameObject.SetActive(value: false);
	}

	public void Click_Really_Quit_Quit()
	{
		PuzzlePlayManager.GameQuit();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case PuzzleInGameType.Stage:
		{
			int[] array = new int[GameInfo.userPlayData.wave - 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i + 1;
			}
			Protocol_Set.Protocol_game_end_Req(GameInfo.inGamePlayData.levelIdx, GameInfo.userPlayData.gameKey, 0, 2, GameInfo.userPlayData.turn, GameInfo.userPlayData.chestKey, GameInfo.userPlayData.listMonsterClear, array, OnGameEndConnectComplete);
			break;
		}
		case PuzzleInGameType.Arena:
			Protocol_Set.Protocol_arena_game_end_Req(GameInfo.inGamePlayData.arenaLevelData.levelIdx, GameInfo.userPlayData.gameKey, 0, 2, GameInfo.userPlayData.wave, OnArenaGameEndComplete);
			break;
		}
	}
}
