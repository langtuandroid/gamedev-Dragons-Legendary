using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleDefeat : MonoBehaviour
{
	[SerializeField]
	private Text textChallengeCost;

	[SerializeField]
	private Transform trItemResult;

	[SerializeField]
	private ScrollRect scrollLoot;

	[SerializeField]
	private Text timer_Text;

	private GAME_END_RESULT resultData;

	private ARENA_GAME_END_RESULT resultArenaData;

	private Coroutine timerCoroutine;

	public void Show()
	{
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case InGameType.Stage:
			base.gameObject.SetActive(value: true);
			ShowItem();
			timerCoroutine = StartCoroutine(ContinueTimer());
			break;
		case InGameType.Arena:
			textChallengeCost.text = $"{GameDataManager.GetGameConfigData(ConfigDataType.Arena_continue_jewel)}";
			Protocol_Set.Protocol_arena_game_end_Req(GameInfo.inGamePlayData.arenaLevelData.levelIdx, GameInfo.userPlayData.gameKey, 0, 1, GameInfo.userPlayData.wave - 1, ShowArenaItem);
			break;
		}
	}

	public void Hide()
	{
		InGameResultItem[] componentsInChildren = trItemResult.GetComponentsInChildren<InGameResultItem>();
		foreach (InGameResultItem inGameResultItem in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Puzzle", inGameResultItem.transform);
		}
		base.gameObject.SetActive(value: false);
	}

	private void ShowItem()
	{
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case InGameType.Stage:
			SoundController.BGM_Stop(MusicSoundType.IngameBGM);
			SoundController.BGM_Stop(MusicSoundType.InGameDragonBgm);
			break;
		case InGameType.Arena:
			SoundController.BGM_Stop(MusicSoundType.ArenaBGM);
			break;
		}
		SoundController.EffectSound_Play(EffectSoundType.LevelFail);
		textChallengeCost.text = $"{10}";
		LevelDbData levelIndexDbData = GameDataManager.GetLevelIndexDbData(GameInfo.inGamePlayData.levelIdx);
		ResultItemData resultItemData = new ResultItemData();
		resultItemData.itemIdx = levelIndexDbData.rewardFixItem;
		resultItemData.itemMultiply = levelIndexDbData.rewardFixCount;
		resultItemData.itemName = GameDataManager.GetItemListData(resultItemData.itemIdx).itemName;
		resultItemData.itemAmount = GameInfo.userData.GetItemCount(resultItemData.itemIdx);
		InGameResultItem component = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<InGameResultItem>();
		component.Show(resultItemData);
		ResultItemData resultItemData2 = new ResultItemData();
		resultItemData2.itemIdx = 50040;
		resultItemData2.itemMultiply = levelIndexDbData.getExp;
		resultItemData2.itemName = GameDataManager.GetItemListData(resultItemData2.itemIdx).itemName;
		resultItemData2.itemAmount = GameInfo.userData.GetItemCount(resultItemData2.itemIdx);
		InGameResultItem component2 = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<InGameResultItem>();
		component2.Show(resultItemData2);
		scrollLoot.horizontalNormalizedPosition = 0f;
	}

	private void ShowArenaItem(ARENA_GAME_END_RESULT _data)
	{
		base.gameObject.SetActive(value: true);
		resultArenaData = _data;
		ResultItemData resultItemData = new ResultItemData();
		resultItemData.itemIdx = 50040;
		resultItemData.itemMultiply = _data.rewardExp;
		resultItemData.itemName = GameDataManager.GetItemListData(resultItemData.itemIdx).itemName;
		resultItemData.itemAmount = GameInfo.userData.GetItemCount(resultItemData.itemIdx);
		InGameResultItem component = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<InGameResultItem>();
		component.Show(resultItemData);
		int itemIdx = 50044;
		if (_data.chestType == 2)
		{
			itemIdx = 50045;
		}
		ResultItemData resultItemData2 = new ResultItemData();
		resultItemData2.itemIdx = itemIdx;
		resultItemData2.itemMultiply = 1;
		resultItemData2.itemName = GameDataManager.GetItemListData(resultItemData2.itemIdx).itemName;
		resultItemData2.itemAmount = GameInfo.userData.GetItemCount(resultItemData2.itemIdx);
		InGameResultItem component2 = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<InGameResultItem>();
		component2.Show(resultItemData2);
		timerCoroutine = StartCoroutine(ContinueTimer());
	}

	private void OnGameContinueConnectComplete()
	{
		InGamePlayManager.GameContinue();
	}

	private void ContinueTimerSet()
	{
		timerCoroutine = StartCoroutine(ContinueTimer());
	}

	private IEnumerator ContinueTimer()
	{
		UnityEngine.Debug.Log("Timer Start");
		int time = 10;
		while (time > 0)
		{
			time--;
			timer_Text.text = time.ToString();
			SoundController.EffectSound_Play(EffectSoundType.TimerTick);
			yield return new WaitForSeconds(1f);
		}
		OnClickGiveUp();
	}

	public void OnChallange()
	{
		StopCoroutine(timerCoroutine);
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case InGameType.Stage:
			if (GameInfo.userData.userInfo.jewel >= 10)
			{
				Protocol_Set.Protocol_game_continue_Req(OnGameContinueConnectComplete);
			}
			else
			{
				InGamePlayManager.NotEnoughJewel(ContinueTimerSet);
			}
			break;
		case InGameType.Arena:
			if (GameInfo.userData.userInfo.jewel >= GameDataManager.GetGameConfigData(ConfigDataType.Arena_continue_jewel))
			{
				Protocol_Set.Protocol_arena_game_continue_Req(OnGameContinueConnectComplete);
			}
			else
			{
				InGamePlayManager.NotEnoughJewel(ContinueTimerSet);
			}
			break;
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickGiveUp()
	{
		StopCoroutine(timerCoroutine);
		Hide();
		switch (GameInfo.inGamePlayData.inGameType)
		{
		case InGameType.Stage:
			InGamePlayManager.ShowLoseResult();
			break;
		case InGameType.Arena:
			InGamePlayManager.ShowArenaLoseResult(resultArenaData);
			break;
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}
}
