using UnityEngine;
using UnityEngine.UI;

public class BattleLose : MonoBehaviour
{
	[SerializeField]
	private Text textStageName;

	[SerializeField]
	private Text textChapterLevel;

	[SerializeField]
	private Text textArenaLevel;

	[SerializeField]
	private Transform trItemResult;

	[SerializeField]
	private ScrollRect scrollLoot;

	[SerializeField]
	private GameObject goNormalTitle;

	private GAME_END_RESULT resultData;

	public void Show()
	{
		int[] array = new int[GameInfo.userPlayData.wave - 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = i + 1;
		}
		goNormalTitle.SetActive(value: true);
		textArenaLevel.gameObject.SetActive(value: false);
		Protocol_Set.Protocol_game_end_Req(GameInfo.inGamePlayData.levelIdx, GameInfo.userPlayData.gameKey, 0, 1, GameInfo.userPlayData.turn, GameInfo.userPlayData.chestKey, GameInfo.userPlayData.listMonsterClear, array, OnGameEndConnectComplete);
	}

	public void ShowArena(ARENA_GAME_END_RESULT _data)
	{
		base.gameObject.SetActive(value: true);
		goNormalTitle.SetActive(value: false);
		textArenaLevel.gameObject.SetActive(value: true);
		textArenaLevel.text = string.Format(MasterLocalize.GetData("arena_lobby_text_03"), GameInfo.inGamePlayData.arenaLevelData.levelIdx);
		if (_data.rewardArenaPoint != 0)
		{
			ResultItemData resultItemData = new ResultItemData();
			resultItemData.itemIdx = 50043;
			resultItemData.itemMultiply = _data.rewardArenaPoint;
			resultItemData.itemName = GameDataManager.GetItemListData(resultItemData.itemIdx).itemName;
			resultItemData.itemAmount = GameInfo.userData.GetItemCount(resultItemData.itemIdx);
			PuzzleResultItem component = MasterPoolManager.SpawnObject("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
			component.OpenMenu(resultItemData);
		}
	}

	private void Init()
	{
		textChapterLevel.text = string.Format("{0} {1} - {2} {3}", MasterLocalize.GetData("common_text_chapter"), GameInfo.inGamePlayData.chapter, MasterLocalize.GetData("common_text_level"), GameInfo.inGamePlayData.level);
		textStageName.text = MasterLocalize.GetData(GameDataManager.GetDicStageDbData()[GameInfo.inGamePlayData.stage].stageName);
	}

	private void ShowItem()
	{
		if (GameInfo.userPlayData.chestKey > 0)
		{
			ResultItemData resultItemData = new ResultItemData();
			resultItemData.itemIdx = 50033;
			resultItemData.itemMultiply = GameInfo.userPlayData.chestKey;
			resultItemData.itemName = GameDataManager.GetItemListData(resultItemData.itemIdx).itemName;
			resultItemData.itemAmount = GameInfo.userData.GetItemCount(resultItemData.itemIdx);
			PuzzleResultItem component = MasterPoolManager.SpawnObject("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
			component.OpenMenu(resultItemData);
		}
		REWARDITEM[] rewardMonsterItem = resultData.rewardMonsterItem;
		foreach (REWARDITEM rEWARDITEM in rewardMonsterItem)
		{
			ResultItemData resultItemData2 = new ResultItemData();
			resultItemData2.itemIdx = rEWARDITEM.itemIdx;
			resultItemData2.itemMultiply = rEWARDITEM.count;
			resultItemData2.itemName = GameDataManager.GetItemListData(resultItemData2.itemIdx).itemName;
			resultItemData2.itemAmount = GameInfo.userData.GetItemCount(resultItemData2.itemIdx);
			PuzzleResultItem component2 = MasterPoolManager.SpawnObject("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
			component2.OpenMenu(resultItemData2);
		}
		ResultItemData resultItemData3 = new ResultItemData();
		resultItemData3.itemIdx = 50032;
		resultItemData3.itemMultiply = resultData.rewardCoin;
		resultItemData3.itemName = GameDataManager.GetItemListData(resultItemData3.itemIdx).itemName;
		resultItemData3.itemAmount = GameInfo.userData.GetItemCount(resultItemData3.itemIdx);
		PuzzleResultItem component3 = MasterPoolManager.SpawnObject("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
		component3.OpenMenu(resultItemData3);
		scrollLoot.horizontalNormalizedPosition = 0f;
	}

	private void OnGameEndConnectComplete(GAME_END_RESULT _data)
	{
		resultData = _data;
		base.gameObject.SetActive(value: true);
		Init();
		ShowItem();
	}

	public void OnClickConfirm()
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		PuzzlePlayManager.LoseGameEvent();
		GameDataManager.MoveScene(SceneType.Lobby);
	}
}
