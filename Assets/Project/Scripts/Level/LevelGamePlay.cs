using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelGamePlay : LobbyPopupBase
{
	public Action OnGoBackEvent;

	public Action OnSelectLevelPlay;

	[FormerlySerializedAs("textStageName")] [SerializeField]
	private Text _textStageName;

	[FormerlySerializedAs("textChapterLevel")] [SerializeField]
	private Text _textChapterLevel;

	[FormerlySerializedAs("textPlayCost")] [SerializeField]
	private Text _textPlayCost;

	[FormerlySerializedAs("textRewardFixCount")] [SerializeField]
	private Text _textRewardFixCount;

	[FormerlySerializedAs("imageStagePreview")] [SerializeField]
	private Image _imageStagePreview;

	[FormerlySerializedAs("trPlayButton")] [SerializeField]
	private Transform _trPlayButton;

	[FormerlySerializedAs("trHunterCardParent")] [SerializeField]
	private Transform _trHunterCardParent;

	[FormerlySerializedAs("trRewardItemAnchor")] [SerializeField]
	private Transform _trRewardItemAnchor;

	[FormerlySerializedAs("trMonsterListAnchor")] [SerializeField]
	private Transform _trMonsterListAnchor;

	[FormerlySerializedAs("trDeckEditBT")] [SerializeField]
	private Transform _trDeckEditBT;

	[FormerlySerializedAs("trDeckEditLockBT")] [SerializeField]
	private Transform _trDeckEditLockBT;

	[FormerlySerializedAs("trDeckEditToolTip")] [SerializeField]
	private Transform _trDeckEditToolTip;

	[FormerlySerializedAs("totalHealth_Text")] [SerializeField]
	private Text _totalHealthText;

	[FormerlySerializedAs("totalAttack_Text")] [SerializeField]
	private Text _totalAttackText;

	[FormerlySerializedAs("totalRecovery_Text")] [SerializeField]
	private Text _totalRecoveryText;

	[FormerlySerializedAs("leaderSkill_Text")] [SerializeField]
	private Text _leaderSkillText;
	
	private int _totalHealthCurrent;
	private int _totalAttackCurrent;
	private int _totalRecoveryCurrent;

	[FormerlySerializedAs("boostDescription")] [SerializeField]
	private LevelGamePlayBoosterDescription _levelPlayBoosterDescription;

	[FormerlySerializedAs("listClearStar")] [SerializeField]
	private List<GameObject> _listClearStar = new List<GameObject>();

	[FormerlySerializedAs("boosterItem")] [SerializeField]
	private LevelGameBooster _boosterItem;

	private int _levelIndex;

	private Transform _trRewardItem;

	private LevelGameDbData _levelData;

	public Transform PlayButton => _trPlayButton;

	public void Open(int index)
	{
		base.Open();
		GameUtil.SetUseHunterList();
		GameUtil.SetOwnHunterList(HanterListType.Normal);
		_levelIndex = index;
		_textStageName.text = MasterLocalize.GetData(GameDataManager.GetDicStageDbData()[GameInfo.inGamePlayData.stage].stageName);
		_textChapterLevel.text = string.Format("{0} {1} - {2} {3}", MasterLocalize.GetData("common_text_chapter"), GameInfo.inGamePlayData.chapter, MasterLocalize.GetData("common_text_level"), GameInfo.inGamePlayData.level);
		_textPlayCost.text = $"{GameDataManager.GetLevelIndexDbData(GameInfo.inGamePlayData.levelIdx).energyCost}";
		_imageStagePreview.sprite = GameDataManager.GetStagePreviewSprite(GameInfo.inGamePlayData.stage - 1);
		_trRewardItem = MasterPoolManager.SpawnObject("Item", $"Item_{GameDataManager.GetLevelIndexDbData(_levelIndex).rewardFixItem}", _trRewardItemAnchor);
		_textRewardFixCount.text = $"x{GameDataManager.GetLevelIndexDbData(_levelIndex).rewardFixCount}";
		_levelData = GameDataManager.GetLevelIndexDbData(_levelIndex);
		GameInfo.userData.GetUserLevelState(_levelData.stage - 1, _levelData.chapter - 1, _levelData.levelIdx);
		ActivateBoosters();
		ResetHunterCards();
		ClearStars();
		OpenMonsterList();
		SetAllHunters();
		SaveEditDeck();
		UnityEngine.Debug.Log("Tutorial LeaderSkill = " + _levelIndex);
		InfoManager.DeckTutorial();
		
		if (LobbyManager.OpenDeckEdit != null && _levelIndex == 13)
		{
			LobbyManager.OpenDeckEdit();
		}
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
		DeleteCard();
	}

	public void DeckReset()
	{
		DeleteCard();
		ResetHunterCards();
		SetAllHunters();
	}

	public void SpawnMonster()
	{
		_trMonsterListAnchor.gameObject.SetActive(value: true);
	}

	public void CloseMonster()
	{
		_trMonsterListAnchor.gameObject.SetActive(value: false);
	}

	private void ResetHunterCards()
	{
		DeleteCard();
		for (int i = 0; i < GameInfo.userData.huntersUseInfo.Length; i++)
		{
			Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
			HeroCard component = MasterPoolManager.SpawnObject("Hunter", "HunterCard_" + GameInfo.userData.huntersUseInfo[i].hunterIdx, _trHunterCardParent).GetComponent<HeroCard>();
			component.Construct(HerocardType.Levelplay, GameDataManager.GetHunterInfo(GameInfo.userData.huntersUseInfo[i].hunterIdx, GameInfo.userData.huntersUseInfo[i].hunterLevel, GameInfo.userData.huntersUseInfo[i].hunterTier), _isOwn: true, _isArena: false);
			component.HeroIdx = i;
			component.IsUseHero = true;
			component.transform.localPosition = Vector3.zero;
			component.transform.localScale = Vector3.one;
			if (i == 0)
			{
				if (component.HunterInfo.Stat.hunterLeaderSkill == 0)
				{
					_leaderSkillText.text = string.Format(MasterLocalize.GetData("Popup_hunter_leaderskill_02"));
				}
				else
				{
					_leaderSkillText.text = string.Format(MasterLocalize.GetData(GameDataManager.GetHunterLeaderSkillData(component.HunterInfo.Stat.hunterLeaderSkill).leaderSkillDescription));
				}
			}
		}
	}

	private void DeleteCard()
	{
		HeroCard[] componentsInChildren = _trHunterCardParent.GetComponentsInChildren<HeroCard>();
		foreach (HeroCard hunterCard in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Hunter", hunterCard.transform);
		}
	}

	private void ClearStars()
	{
		UserLevelState[] levelList = GameInfo.userData.userStageState[GameInfo.inGamePlayData.stage - 1].chapterList[GameInfo.inGamePlayData.chapter - 1].levelList;
		UserLevelState[] array = levelList;
		foreach (UserLevelState userLevelState in array)
		{
			if (userLevelState.level == GameInfo.inGamePlayData.level)
			{
				for (int j = 0; j < _listClearStar.Count; j++)
				{
					_listClearStar[j].SetActive(j + 1 <= userLevelState.starCount);
				}
			}
		}
	}

	private void OpenMonsterList()
	{
		SpawnMonster();
		foreach (KeyValuePair<int, int> levelMonster in GameUtil.GetLevelMonsterList(_levelIndex))
		{
			Transform transform = MasterPoolManager.SpawnObject("Lobby", "QuickLootMonster", _trMonsterListAnchor);
			transform.GetComponent<ItemInfoUI>().Show("Info", $"UI_monster_{levelMonster.Key}", levelMonster.Value);
		}
	}

	private void OnGamePlayConncectComplete()
	{
		if (_trRewardItem != null)
		{
			MasterPoolManager.ReturnToPool("Item", _trRewardItem);
			_trRewardItem = null;
		}
		ItemInfoUI[] componentsInChildren = _trMonsterListAnchor.GetComponentsInChildren<ItemInfoUI>();
		ItemInfoUI[] array = componentsInChildren;
		foreach (ItemInfoUI itemInfoUI in array)
		{
			itemInfoUI.Clear();
			MasterPoolManager.ReturnToPool("Lobby", itemInfoUI.transform);
		}
		LobbyManager.GotoInGame(_levelIndex);
	}

	private void SetAllHunters()
	{
		_totalHealthCurrent = 0;
		_totalAttackCurrent = 0;
		_totalRecoveryCurrent = 0;
		for (int i = 0; i < _trHunterCardParent.childCount; i++)
		{
			HunterInfo hunterInfo = _trHunterCardParent.GetChild(i).GetComponent<HeroCard>().HunterInfo;
			_totalHealthCurrent += (int)GameUtil.GetHunterReinForceHP(hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			_totalAttackCurrent += (int)GameUtil.GetHunterReinForceAttack(hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			_totalRecoveryCurrent += (int)GameUtil.GetHunterReinForceHeal(hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
		}
		_totalHealthText.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_health"), Translate(_totalHealthCurrent)) + "</color>";
		_totalAttackText.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_attack"), Translate(_totalAttackCurrent)) + "</color>";
		_totalRecoveryText.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_recovery"), Translate(_totalRecoveryCurrent)) + "</color>";
	}

	private void ActivateBoosters()
	{
		int[] itemType = new int[3]
		{
			_levelData.boosterItem_1,
			_levelData.boosterItem_2,
			_levelData.boosterItem_3
		};
		_boosterItem.Construct(itemType);
	}

	private string Translate(float _stat)
	{
		string empty = string.Empty;
		float num = 0f;
		if (_stat >= 1000f)
		{
			num = _stat / 1000f;
			return (Math.Truncate(num * 100f) / 100.0).ToString() + "K";
		}
		return _stat.ToString();
	}

	private void SaveEditDeck()
	{
		if (GameInfo.userData.huntersUseInfo.Length + GameInfo.userData.huntersOwnInfo.Length >= 5 && GameInfo.userData.userStageState[0].chapterList.Length >= 3)
		{
			_trDeckEditBT.gameObject.SetActive(value: true);
			_trDeckEditLockBT.gameObject.SetActive(value: false);
		}
		else
		{
			_trDeckEditBT.gameObject.SetActive(value: false);
			_trDeckEditLockBT.gameObject.SetActive(value: true);
		}
	}
	
	public void OnClickLevelPlay()
	{
		// TODO uncoment
		/*
		if (GameInfo.userData.userInfo.energy < levelData.energyCost) 
		{
			LobbyManager.ShowUserEnergyInfo();
			AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.stamin_under_4);
			return;
		}
		*/
		
		GameInfo.userPlayData.Clear();
		if (OnSelectLevelPlay != null)
		{
			OnSelectLevelPlay();
		}
		SoundController.EffectSound_Play(EffectSoundType.LevelPlay);
		GameInfo.inGamePlayData.stage = _levelData.stage;
		GameInfo.inGamePlayData.chapter = _levelData.chapter;
		GameInfo.inGamePlayData.level = _levelData.level;
		GameInfo.inGamePlayData.levelIdx = _levelData.levelIdx;
		List<int> list = new List<int>();
		_boosterItem.AddBuster();
		if (GameInfo.inGamePlayData.dicActiveBoostItem.Count > 0)
		{
			foreach (KeyValuePair<int, BoostItemDbData> item in GameInfo.inGamePlayData.dicActiveBoostItem)
			{
				list.Add(item.Key);
			}
		}
		Protocol_Set.Protocol_game_start_Req(_levelIndex, list, OnGamePlayConncectComplete);
	}

	public void OnClickLockDeckEdit()
	{
		_trDeckEditToolTip.gameObject.SetActive(value: true);
	}

	public void OnClickBoostInfo()
	{
		int[] itemType = new int[3]
		{
			_levelData.boosterItem_1,
			_levelData.boosterItem_2,
			_levelData.boosterItem_3
		};
		_levelPlayBoosterDescription.Construct(itemType);
	}

	public void OnClickToolTip()
	{
		_trDeckEditToolTip.gameObject.SetActive(value: false);
	}

	public void OnClickGoBack()
	{
		_boosterItem.BoostCancel();
		if (OnGoBackEvent != null)
		{
			OnGoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
	
	private void OnDisable()
	{
		if (_trRewardItem != null)
		{
			MasterPoolManager.ReturnToPool("Item", _trRewardItem);
			_trRewardItem = null;
		}
		ItemInfoUI[] componentsInChildren = _trMonsterListAnchor.GetComponentsInChildren<ItemInfoUI>();
		ItemInfoUI[] array = componentsInChildren;
		foreach (ItemInfoUI itemInfoUI in array)
		{
			itemInfoUI.Clear();
			MasterPoolManager.ReturnToPool("Lobby", itemInfoUI.transform);
		}
		DeleteCard();
	}
}
