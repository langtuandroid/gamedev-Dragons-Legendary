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
		base.Open();
		Init();
	}

	public void Refresh()
	{
		LevelGameBlock[] componentsInChildren = trListAnchor.GetComponentsInChildren<LevelGameBlock>();
		foreach (LevelGameBlock levelCell in componentsInChildren)
		{
			levelCell.Reset();
		}
		if (itemIdx > 0)
		{
			if (trTitleItem != null)
			{
				MasterPoolManager.ReturnToPool("Item", trTitleItem);
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
		textUserOwnItemCount.text = string.Format("{0} <color=#FCF13E>{1}</color>", MasterLocalize.GetData("common_text_you_have"), GameInfo.userData.GetItemCount(itemIdx));
		textTitleItemName.text = MasterLocalize.GetData(GameDataManager.GetItemListData(itemIdx).itemName);
		trTitleItem = MasterPoolManager.SpawnObject("Item", $"Item_{itemIdx}", trTitleItemAnchor);
	}

	private void ShowSortItemLevelList()
	{
		scrollRect.verticalNormalizedPosition = 0f;
		int num = 0;
		UserStageState[] userStageState = GameInfo.userData.userStageState;
		foreach (UserStageState userStageState2 in userStageState)
		{
			LevelGameStage component = MasterPoolManager.SpawnObject("Lobby", "Cell_stage", trListAnchor).GetComponent<LevelGameStage>();
			component.transform.localScale = Vector3.one;
			component.ChangeSetData(userStageState2.stage);
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
					LevelGameDbData levelIndexDbData = GameDataManager.GetLevelIndexDbData(userLevelState.levelIdx);
					if (levelIndexDbData.rewardFixItem == itemIdx)
					{
						LevelGameBlock component2 = MasterPoolManager.SpawnObject("Lobby", "Cell_level", trListAnchor).GetComponent<LevelGameBlock>();
						component2.transform.localScale = Vector3.one;
						component2.SetData(levelIndexDbData);
						component2.PlaceStars(GameDataManager.GetLevelStarCount(levelIndexDbData.stage, levelIndexDbData.chapter, levelIndexDbData.level));
						num++;
					}
					else if (isWaveItemSort)
					{
						foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum in GameDataManager.GetDicWaveDbData(levelIndexDbData.levelIdx))
						{
							if (dicWaveDbDatum.Value.dropM1 == itemIdx || dicWaveDbDatum.Value.dropM2 == itemIdx || dicWaveDbDatum.Value.dropM3 == itemIdx || dicWaveDbDatum.Value.dropM4 == itemIdx)
							{
								LevelGameBlock component3 = MasterPoolManager.SpawnObject("Lobby", "Cell_level", trListAnchor).GetComponent<LevelGameBlock>();
								component3.transform.localScale = Vector3.one;
								component3.SetData(levelIndexDbData);
								component3.PlaceStars(GameDataManager.GetLevelStarCount(levelIndexDbData.stage, levelIndexDbData.chapter, levelIndexDbData.level));
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
			LevelGameStage[] componentsInChildren = trListAnchor.GetComponentsInChildren<LevelGameStage>();
			foreach (LevelGameStage levelStage in componentsInChildren)
			{
				MasterPoolManager.ReturnToPool("Lobby", levelStage.transform);
			}
			foreach (KeyValuePair<int, StageDbData> dicStageDbDatum in GameDataManager.GetDicStageDbData())
			{
				if (!dicStageDbDatum.Value.stageLock)
				{
					LevelGameStage component4 = MasterPoolManager.SpawnObject("Lobby", "Cell_stage", trListAnchor).GetComponent<LevelGameStage>();
					component4.transform.localScale = Vector3.one;
					component4.ChangeSetData(dicStageDbDatum.Value.stageIdx);
					foreach (KeyValuePair<int, ChapterDbData> dicChapterDbDatum in GameDataManager.GetDicChapterDbData(dicStageDbDatum.Value.stageIdx))
					{
						foreach (LevelGameDbData levelListDbDatum in GameDataManager.GetLevelListDbData(dicChapterDbDatum.Value.stage, dicChapterDbDatum.Value.chapter))
						{
							if (levelListDbDatum.rewardFixItem == itemIdx)
							{
								LevelGameBlock component5 = MasterPoolManager.SpawnObject("Lobby", "Cell_level", trListAnchor).GetComponent<LevelGameBlock>();
								component5.transform.localScale = Vector3.one;
								component5.SetData(levelListDbDatum);
								component5.PlaceStars(GameDataManager.GetLevelStarCount(levelListDbDatum.stage, levelListDbDatum.chapter, levelListDbDatum.level));
								return;
							}
							if (isWaveItemSort)
							{
								foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum2 in GameDataManager.GetDicWaveDbData(levelListDbDatum.levelIdx))
								{
									if (dicWaveDbDatum2.Value.dropM1 == itemIdx || dicWaveDbDatum2.Value.dropM2 == itemIdx || dicWaveDbDatum2.Value.dropM3 == itemIdx || dicWaveDbDatum2.Value.dropM4 == itemIdx)
									{
										LevelGameBlock component6 = MasterPoolManager.SpawnObject("Lobby", "Cell_level", trListAnchor).GetComponent<LevelGameBlock>();
										component6.transform.localScale = Vector3.one;
										component6.SetData(levelListDbDatum);
										component6.PlaceStars(GameDataManager.GetLevelStarCount(levelListDbDatum.stage, levelListDbDatum.chapter, levelListDbDatum.level));
										num++;
										return;
									}
								}
							}
						}
					}
					MasterPoolManager.ReturnToPool("Lobby", component4.transform);
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
		LevelGameBlock[] componentsInChildren = trListAnchor.GetComponentsInChildren<LevelGameBlock>();
		foreach (LevelGameBlock levelCell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Lobby", levelCell.transform);
		}
		LevelGameStage[] componentsInChildren2 = trListAnchor.GetComponentsInChildren<LevelGameStage>();
		foreach (LevelGameStage levelStage in componentsInChildren2)
		{
			MasterPoolManager.ReturnToPool("Lobby", levelStage.transform);
		}
		if (trTitleItem != null)
		{
			MasterPoolManager.ReturnToPool("Item", trTitleItem);
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
