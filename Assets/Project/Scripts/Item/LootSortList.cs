using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LootSortList : LobbyPopupBase
{
	public Action OnBackEvent;

	[FormerlySerializedAs("trTitleItemAnchor")] [SerializeField]
	private Transform _titleItemTransform;

	[FormerlySerializedAs("trListAnchor")] [SerializeField]
	private Transform _listAnchor;

	[FormerlySerializedAs("textUserOwnItemCount")] [SerializeField]
	private Text _textOwnItem;

	[FormerlySerializedAs("textTitleItemName")] [SerializeField]
	private Text _textTitle;

	[FormerlySerializedAs("scrollRect")] [SerializeField]
	private ScrollRect _scrollRect;

	private int _itemIdx;

	private bool _waveSort;

	private Transform _itemTitle;

	public void Open(int _itemIdx, bool _isWaveSort = false)
	{
		UnityEngine.Debug.Log("_itemIdx :: " + _itemIdx);
		this._itemIdx = _itemIdx;
		_waveSort = _isWaveSort;
		LobbyManager.HideHunterLobby();
		base.Open();
		Construct();
	}

	public void Reload()
	{
		LevelGameBlock[] componentsInChildren = _listAnchor.GetComponentsInChildren<LevelGameBlock>();
		foreach (LevelGameBlock levelCell in componentsInChildren)
		{
			levelCell.Reset();
		}
		if (_itemIdx > 0)
		{
			if (_itemTitle != null)
			{
				MasterPoolManager.ReturnToPool("Item", _itemTitle);
				_itemTitle = null;
			}
			TitleItemShow();
		}
	}

	private void Construct()
	{
		ClearAllLoot();
		TitleItemShow();
		SortListItem();
	}

	private void TitleItemShow()
	{
		_textOwnItem.text = string.Format("{0} <color=#FCF13E>{1}</color>", MasterLocalize.GetData("common_text_you_have"), GameInfo.userData.GetItemCount(_itemIdx));
		_textTitle.text = MasterLocalize.GetData(GameDataManager.GetItemListData(_itemIdx).itemName);
		_itemTitle = MasterPoolManager.SpawnObject("Item", $"Item_{_itemIdx}", _titleItemTransform);
	}

	private void SortListItem()
	{
		_scrollRect.verticalNormalizedPosition = 0f;
		int num = 0;
		UserStageState[] userStageState = GameInfo.userData.userStageState;
		foreach (UserStageState userStageState2 in userStageState)
		{
			LevelGameStage component = MasterPoolManager.SpawnObject("Lobby", "Cell_stage", _listAnchor).GetComponent<LevelGameStage>();
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
					if (levelIndexDbData.rewardFixItem == _itemIdx)
					{
						LevelGameBlock component2 = MasterPoolManager.SpawnObject("Lobby", "Cell_level", _listAnchor).GetComponent<LevelGameBlock>();
						component2.transform.localScale = Vector3.one;
						component2.SetData(levelIndexDbData);
						component2.PlaceStars(GameDataManager.GetLevelStarCount(levelIndexDbData.stage, levelIndexDbData.chapter, levelIndexDbData.level));
						num++;
					}
					else if (_waveSort)
					{
						foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum in GameDataManager.GetDicWaveDbData(levelIndexDbData.levelIdx))
						{
							if (dicWaveDbDatum.Value.dropM1 == _itemIdx || dicWaveDbDatum.Value.dropM2 == _itemIdx || dicWaveDbDatum.Value.dropM3 == _itemIdx || dicWaveDbDatum.Value.dropM4 == _itemIdx)
							{
								LevelGameBlock component3 = MasterPoolManager.SpawnObject("Lobby", "Cell_level", _listAnchor).GetComponent<LevelGameBlock>();
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
			LevelGameStage[] componentsInChildren = _listAnchor.GetComponentsInChildren<LevelGameStage>();
			foreach (LevelGameStage levelStage in componentsInChildren)
			{
				MasterPoolManager.ReturnToPool("Lobby", levelStage.transform);
			}
			foreach (KeyValuePair<int, StageDbData> dicStageDbDatum in GameDataManager.GetDicStageDbData())
			{
				if (!dicStageDbDatum.Value.stageLock)
				{
					LevelGameStage component4 = MasterPoolManager.SpawnObject("Lobby", "Cell_stage", _listAnchor).GetComponent<LevelGameStage>();
					component4.transform.localScale = Vector3.one;
					component4.ChangeSetData(dicStageDbDatum.Value.stageIdx);
					foreach (KeyValuePair<int, ChapterDbData> dicChapterDbDatum in GameDataManager.GetDicChapterDbData(dicStageDbDatum.Value.stageIdx))
					{
						foreach (LevelGameDbData levelListDbDatum in GameDataManager.GetLevelListDbData(dicChapterDbDatum.Value.stage, dicChapterDbDatum.Value.chapter))
						{
							if (levelListDbDatum.rewardFixItem == _itemIdx)
							{
								LevelGameBlock component5 = MasterPoolManager.SpawnObject("Lobby", "Cell_level", _listAnchor).GetComponent<LevelGameBlock>();
								component5.transform.localScale = Vector3.one;
								component5.SetData(levelListDbDatum);
								component5.PlaceStars(GameDataManager.GetLevelStarCount(levelListDbDatum.stage, levelListDbDatum.chapter, levelListDbDatum.level));
								return;
							}
							if (_waveSort)
							{
								foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum2 in GameDataManager.GetDicWaveDbData(levelListDbDatum.levelIdx))
								{
									if (dicWaveDbDatum2.Value.dropM1 == _itemIdx || dicWaveDbDatum2.Value.dropM2 == _itemIdx || dicWaveDbDatum2.Value.dropM3 == _itemIdx || dicWaveDbDatum2.Value.dropM4 == _itemIdx)
									{
										LevelGameBlock component6 = MasterPoolManager.SpawnObject("Lobby", "Cell_level", _listAnchor).GetComponent<LevelGameBlock>();
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
			ClearAllLoot();
			if (OnBackEvent != null)
			{
				OnBackEvent();
			}
		}
	}

	private void ClearAllLoot()
	{
		LevelGameBlock[] componentsInChildren = _listAnchor.GetComponentsInChildren<LevelGameBlock>();
		foreach (LevelGameBlock levelCell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Lobby", levelCell.transform);
		}
		LevelGameStage[] componentsInChildren2 = _listAnchor.GetComponentsInChildren<LevelGameStage>();
		foreach (LevelGameStage levelStage in componentsInChildren2)
		{
			MasterPoolManager.ReturnToPool("Lobby", levelStage.transform);
		}
		if (_itemTitle != null)
		{
			MasterPoolManager.ReturnToPool("Item", _itemTitle);
			_itemTitle = null;
		}
	}

	public void OnClickGoBack()
	{
		if (OnBackEvent != null)
		{
			OnBackEvent();
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
		ClearAllLoot();
	}
}
