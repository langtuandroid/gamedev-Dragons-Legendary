using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSortList : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform trTitleItemAnchor;

	[SerializeField]
	private Transform trListAnchor;

	[SerializeField]
	private Text textUserOwnItemCount;

	[SerializeField]
	private Text textTitleItemName;

	[SerializeField]
	private ScrollRect scrollRect;

	private int itemIdx;

	private bool isWaveItemSort;

	private Transform trTitleItem;

	public void Show(int _itemIdx, bool _isWaveSort = false)
	{
		UnityEngine.Debug.Log("_itemIdx :: " + _itemIdx);
		itemIdx = _itemIdx;
		isWaveItemSort = _isWaveSort;
		LobbyManager.HideHunterLobby();
		base.Show();
		Init();
	}

	public void Refresh()
	{
		LevelCell[] componentsInChildren = trListAnchor.GetComponentsInChildren<LevelCell>();
		foreach (LevelCell levelCell in componentsInChildren)
		{
			levelCell.Refresh();
		}
		if (itemIdx > 0)
		{
			if (trTitleItem != null)
			{
				MWPoolManager.DeSpawn("Item", trTitleItem);
				trTitleItem = null;
			}
			ShowTitleItemInfo();
		}
	}

	private void Init()
	{
		DespawnAll();
		ShowTitleItemInfo();
		ShowSortItemLevelList();
	}

	private void ShowTitleItemInfo()
	{
		textUserOwnItemCount.text = string.Format("{0} <color=#FCF13E>{1}</color>", MWLocalize.GetData("common_text_you_have"), GameInfo.userData.GetItemCount(itemIdx));
		textTitleItemName.text = MWLocalize.GetData(GameDataManager.GetItemListData(itemIdx).itemName);
		trTitleItem = MWPoolManager.Spawn("Item", $"Item_{itemIdx}", trTitleItemAnchor);
	}

	private void ShowSortItemLevelList()
	{
		scrollRect.verticalNormalizedPosition = 0f;
		int num = 0;
		UserStageState[] userStageState = GameInfo.userData.userStageState;
		foreach (UserStageState userStageState2 in userStageState)
		{
			LevelStage component = MWPoolManager.Spawn("Lobby", "Cell_stage", trListAnchor).GetComponent<LevelStage>();
			component.transform.localScale = Vector3.one;
			component.SetData(userStageState2.stage);
			UserChapterState[] chapterList = userStageState2.chapterList;
			foreach (UserChapterState userChapterState in chapterList)
			{
				if (!userChapterState.isOpen)
				{
					continue;
				}
				UserLevelState[] levelList = userChapterState.levelList;
				foreach (UserLevelState userLevelState in levelList)
				{
					LevelDbData levelIndexDbData = GameDataManager.GetLevelIndexDbData(userLevelState.levelIdx);
					if (levelIndexDbData.rewardFixItem == itemIdx)
					{
						LevelCell component2 = MWPoolManager.Spawn("Lobby", "Cell_level", trListAnchor).GetComponent<LevelCell>();
						component2.transform.localScale = Vector3.one;
						component2.SetData(levelIndexDbData);
						component2.SetStarCount(GameDataManager.GetLevelStarCount(levelIndexDbData.stage, levelIndexDbData.chapter, levelIndexDbData.level));
						num++;
					}
					else if (isWaveItemSort)
					{
						foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum in GameDataManager.GetDicWaveDbData(levelIndexDbData.levelIdx))
						{
							if (dicWaveDbDatum.Value.dropM1 == itemIdx || dicWaveDbDatum.Value.dropM2 == itemIdx || dicWaveDbDatum.Value.dropM3 == itemIdx || dicWaveDbDatum.Value.dropM4 == itemIdx)
							{
								LevelCell component3 = MWPoolManager.Spawn("Lobby", "Cell_level", trListAnchor).GetComponent<LevelCell>();
								component3.transform.localScale = Vector3.one;
								component3.SetData(levelIndexDbData);
								component3.SetStarCount(GameDataManager.GetLevelStarCount(levelIndexDbData.stage, levelIndexDbData.chapter, levelIndexDbData.level));
								num++;
								break;
							}
						}
					}
				}
			}
		}
		if (num == 0)
		{
			LevelStage[] componentsInChildren = trListAnchor.GetComponentsInChildren<LevelStage>();
			foreach (LevelStage levelStage in componentsInChildren)
			{
				MWPoolManager.DeSpawn("Lobby", levelStage.transform);
			}
			foreach (KeyValuePair<int, StageDbData> dicStageDbDatum in GameDataManager.GetDicStageDbData())
			{
				if (!dicStageDbDatum.Value.stageLock)
				{
					LevelStage component4 = MWPoolManager.Spawn("Lobby", "Cell_stage", trListAnchor).GetComponent<LevelStage>();
					component4.transform.localScale = Vector3.one;
					component4.SetData(dicStageDbDatum.Value.stageIdx);
					foreach (KeyValuePair<int, ChapterDbData> dicChapterDbDatum in GameDataManager.GetDicChapterDbData(dicStageDbDatum.Value.stageIdx))
					{
						foreach (LevelDbData levelListDbDatum in GameDataManager.GetLevelListDbData(dicChapterDbDatum.Value.stage, dicChapterDbDatum.Value.chapter))
						{
							if (levelListDbDatum.rewardFixItem == itemIdx)
							{
								LevelCell component5 = MWPoolManager.Spawn("Lobby", "Cell_level", trListAnchor).GetComponent<LevelCell>();
								component5.transform.localScale = Vector3.one;
								component5.SetData(levelListDbDatum);
								component5.SetStarCount(GameDataManager.GetLevelStarCount(levelListDbDatum.stage, levelListDbDatum.chapter, levelListDbDatum.level));
								return;
							}
							if (isWaveItemSort)
							{
								foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum2 in GameDataManager.GetDicWaveDbData(levelListDbDatum.levelIdx))
								{
									if (dicWaveDbDatum2.Value.dropM1 == itemIdx || dicWaveDbDatum2.Value.dropM2 == itemIdx || dicWaveDbDatum2.Value.dropM3 == itemIdx || dicWaveDbDatum2.Value.dropM4 == itemIdx)
									{
										LevelCell component6 = MWPoolManager.Spawn("Lobby", "Cell_level", trListAnchor).GetComponent<LevelCell>();
										component6.transform.localScale = Vector3.one;
										component6.SetData(levelListDbDatum);
										component6.SetStarCount(GameDataManager.GetLevelStarCount(levelListDbDatum.stage, levelListDbDatum.chapter, levelListDbDatum.level));
										num++;
										return;
									}
								}
							}
						}
					}
					MWPoolManager.DeSpawn("Lobby", component4.transform);
				}
			}
			DespawnAll();
			if (GoBackEvent != null)
			{
				GoBackEvent();
			}
		}
	}

	private void DespawnAll()
	{
		LevelCell[] componentsInChildren = trListAnchor.GetComponentsInChildren<LevelCell>();
		foreach (LevelCell levelCell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Lobby", levelCell.transform);
		}
		LevelStage[] componentsInChildren2 = trListAnchor.GetComponentsInChildren<LevelStage>();
		foreach (LevelStage levelStage in componentsInChildren2)
		{
			MWPoolManager.DeSpawn("Lobby", levelStage.transform);
		}
		if (trTitleItem != null)
		{
			MWPoolManager.DeSpawn("Item", trTitleItem);
			trTitleItem = null;
		}
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		if (LobbyManager.HunterLevelHunterInfo != null)
		{
			LobbyManager.ShowHunterLevel(LobbyManager.HunterLevelHunterInfo, _isSpawn: false);
		}
		if (LobbyManager.HunterPromotionHunterInfo != null)
		{
			LobbyManager.ShowHunterPromotion(LobbyManager.HunterPromotionHunterInfo, _isSpawn: false);
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	private void OnDisable()
	{
		DespawnAll();
	}
}
