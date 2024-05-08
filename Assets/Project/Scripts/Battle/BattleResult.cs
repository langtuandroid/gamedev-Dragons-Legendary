using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResult : MonoBehaviour
{
	private const float RESULT_STAR_SHOW_DELAY = 0.3f;

	private const float RESULT_STAR_GAP_DELAY = 0.2f;

	[SerializeField]
	private Text textChapterLevel;

	[SerializeField]
	private Text textStageName;

	[SerializeField]
	private Text textClearTurn;

	[SerializeField]
	private Text textStar2;

	[SerializeField]
	private Text textStar3;

	[SerializeField]
	private GameObject[] arrGoStar;

	[SerializeField]
	private Transform trItemResult;

	[SerializeField]
	private ScrollRect scrollLoot;

	private bool isStarProcess;

	private int userClearTurn;

	private int userClearStar;

	private LevelGameDbData levelData;

	private GAME_END_RESULT gameEndData;

	public void Show(int clearTurn)
	{
		userClearTurn = clearTurn;
		int[] array = new int[GameInfo.userPlayData.wave];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = i + 1;
		}
		Protocol_Set.Protocol_game_end_Req(GameInfo.inGamePlayData.levelIdx, GameInfo.userPlayData.gameKey, 1, 0, clearTurn, GameInfo.userPlayData.chestKey, GameInfo.userPlayData.listMonsterClear, array, OnGameEndConnectComplete);
	}

	private void Init()
	{
		base.gameObject.SetActive(value: true);
		textChapterLevel.text = string.Format("{0} {1} - {2} {3}", MWLocalize.GetData("common_text_chapter"), GameInfo.inGamePlayData.chapter, MWLocalize.GetData("common_text_level"), GameInfo.inGamePlayData.level);
		textStageName.text = MWLocalize.GetData(GameDataManager.GetDicStageDbData()[GameInfo.inGamePlayData.stage].stageName);
		textClearTurn.text = string.Format("{0} {1}", userClearTurn, MWLocalize.GetData("common_text_turns"));
		levelData = GetLevelData();
		userClearStar = gameEndData.rewardStar;
		textStar2.text = $"{levelData.star2}";
		textStar3.text = $"{levelData.star3}";
		ShowItem();
		StartCoroutine(ProcessStarShow());
		if (levelData.isDragon == 1)
		{
			GameInfo.isRate = true;
		}
	}

	private LevelGameDbData GetLevelData()
	{
		List<LevelGameDbData> levelListDbData = GameDataManager.GetLevelListDbData(GameInfo.inGamePlayData.stage, GameInfo.inGamePlayData.chapter);
		foreach (LevelGameDbData item in levelListDbData)
		{
			if (item.level == GameInfo.inGamePlayData.level)
			{
				return item;
			}
		}
		return null;
	}

	private void ShowItem()
	{
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
		SoundController.EffectSound_Play(EffectSoundType.LevelClear);
		ResultItemData resultItemData = new ResultItemData();
		resultItemData.itemIdx = 50040;
		resultItemData.itemMultiply = gameEndData.rewardExp;
		resultItemData.itemName = GameDataManager.GetItemListData(resultItemData.itemIdx).itemName;
		resultItemData.itemAmount = GameInfo.userData.GetItemCount(resultItemData.itemIdx);
		PuzzleResultItem component = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
		component.OpenMenu(resultItemData);
		ResultItemData resultItemData2 = new ResultItemData();
		resultItemData2.itemIdx = 50032;
		resultItemData2.itemMultiply = gameEndData.rewardCoin;
		resultItemData2.itemName = GameDataManager.GetItemListData(resultItemData2.itemIdx).itemName;
		resultItemData2.itemAmount = GameInfo.userData.GetItemCount(resultItemData2.itemIdx);
		PuzzleResultItem component2 = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
		component2.OpenMenu(resultItemData2);
		PuzzleResultItem inGameResultItem = null;
		if (GameInfo.userPlayData.chestKey > 0)
		{
			ResultItemData resultItemData3 = new ResultItemData();
			resultItemData3.itemIdx = 50033;
			resultItemData3.itemMultiply = GameInfo.userPlayData.chestKey;
			resultItemData3.itemName = GameDataManager.GetItemListData(resultItemData3.itemIdx).itemName;
			resultItemData3.itemAmount = GameInfo.userData.GetItemCount(resultItemData3.itemIdx);
			inGameResultItem = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
			inGameResultItem.OpenMenu(resultItemData3);
		}
		if (gameEndData.rewardFixItem.Length > 0)
		{
			if (gameEndData.rewardFixItem[0].itemIdx == 50040)
			{
				component.AddCount(gameEndData.rewardFixItem[0].count);
			}
			else if (gameEndData.rewardFixItem[0].itemIdx == 50033 && inGameResultItem != null)
			{
				inGameResultItem.AddCount(gameEndData.rewardFixItem[0].count);
			}
			else if (gameEndData.rewardFixItem[0].itemIdx == 50032)
			{
				component2.AddCount(gameEndData.rewardFixItem[0].count);
			}
			else
			{
				ResultItemData resultItemData4 = new ResultItemData();
				resultItemData4.itemIdx = gameEndData.rewardFixItem[0].itemIdx;
				resultItemData4.itemMultiply = gameEndData.rewardFixItem[0].count;
				resultItemData4.itemName = GameDataManager.GetItemListData(resultItemData4.itemIdx).itemName;
				resultItemData4.itemAmount = GameInfo.userData.GetItemCount(resultItemData4.itemIdx);
				PuzzleResultItem component3 = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
				component3.OpenMenu(resultItemData4);
			}
		}
		REWARDITEM[] rewardMonsterItem = gameEndData.rewardMonsterItem;
		foreach (REWARDITEM rEWARDITEM in rewardMonsterItem)
		{
			if (rEWARDITEM.itemIdx == 50040)
			{
				component.AddCount(gameEndData.rewardFixItem[0].count);
				continue;
			}
			if (rEWARDITEM.itemIdx == 50033 && inGameResultItem != null)
			{
				inGameResultItem.AddCount(gameEndData.rewardFixItem[0].count);
				continue;
			}
			ResultItemData resultItemData5 = new ResultItemData();
			resultItemData5.itemIdx = rEWARDITEM.itemIdx;
			resultItemData5.itemMultiply = rEWARDITEM.count;
			resultItemData5.itemName = GameDataManager.GetItemListData(resultItemData5.itemIdx).itemName;
			resultItemData5.itemAmount = GameInfo.userData.GetItemCount(resultItemData5.itemIdx);
			PuzzleResultItem component4 = MWPoolManager.Spawn("Puzzle", "InGameResultItem", trItemResult).GetComponent<PuzzleResultItem>();
			component4.OpenMenu(resultItemData5);
		}
		scrollLoot.horizontalNormalizedPosition = 0f;
	}

	private IEnumerator ProcessStarShow()
	{
		yield return new WaitForSeconds(0.3f);
		for (int i = 0; i < arrGoStar.Length; i++)
		{
			yield return new WaitForSeconds(0.2f);
			arrGoStar[i].SetActive(i <= userClearStar - 1);
		}
		isStarProcess = true;
	}

	private void OnGameEndConnectComplete(GAME_END_RESULT _data)
	{
		gameEndData = _data;
		Init();
	}

	private FBLog_Type CheckFireBase()
	{
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 1 && GameInfo.inGamePlayData.level == 1)
		{
			return FBLog_Type.cleared_level_lv1;
		}
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 1 && GameInfo.inGamePlayData.level == 2)
		{
			return FBLog_Type.cleared_level_lv2;
		}
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 1 && GameInfo.inGamePlayData.level == 3)
		{
			return FBLog_Type.cleared_level_lv3;
		}
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 1 && GameInfo.inGamePlayData.level == 4)
		{
			return FBLog_Type.cleared_level_lv4;
		}
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 1 && GameInfo.inGamePlayData.level == 5)
		{
			return FBLog_Type.cleared_level_lv5;
		}
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 2 && GameInfo.inGamePlayData.level == 6)
		{
			return FBLog_Type.cleared_level_lv6;
		}
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 2 && GameInfo.inGamePlayData.level == 12)
		{
			return FBLog_Type.cleared_level_lv12;
		}
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 3 && GameInfo.inGamePlayData.level == 13)
		{
			return FBLog_Type.cleared_level_lv13;
		}
		if (GameInfo.inGamePlayData.stage == 1 && GameInfo.inGamePlayData.chapter == 3 && GameInfo.inGamePlayData.level == 19)
		{
			return FBLog_Type.cleared_level_lv19;
		}
		return FBLog_Type.none;
	}

	public void OnClickConfirm()
	{
		if (!isStarProcess)
		{
			StopAllCoroutines();
			for (int i = 0; i < arrGoStar.Length; i++)
			{
				arrGoStar[i].SetActive(i <= userClearStar - 1);
			}
			isStarProcess = true;
		}
		else
		{
			PuzzlePlayManager.WinReward();
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}
}
