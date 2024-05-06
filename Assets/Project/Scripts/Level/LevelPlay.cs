using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPlay : LobbyPopupBase
{
	public Action GoBackEvent;

	public Action SelectLevelPlay;

	[SerializeField]
	private Text textStageName;

	[SerializeField]
	private Text textChapterLevel;

	[SerializeField]
	private Text textPlayCost;

	[SerializeField]
	private Text textRewardFixCount;

	[SerializeField]
	private Image imageStagePreview;

	[SerializeField]
	private Transform trPlayButton;

	[SerializeField]
	private Transform trHunterCardParent;

	[SerializeField]
	private Transform trRewardItemAnchor;

	[SerializeField]
	private Transform trMonsterListAnchor;

	[SerializeField]
	private Transform trDeckEditBT;

	[SerializeField]
	private Transform trDeckEditLockBT;

	[SerializeField]
	private Transform trDeckEditToolTip;

	[SerializeField]
	private Text totalHealth_Text;

	[SerializeField]
	private Text totalAttack_Text;

	[SerializeField]
	private Text totalRecovery_Text;

	[SerializeField]
	private Text leaderSkill_Text;

	[SerializeField]
	private int totalHealth_current;

	[SerializeField]
	private int totalAttack_current;

	[SerializeField]
	private int totalRecovery_current;

	[SerializeField]
	private LevelPlay_BoosterDescription boostDescription;

	[SerializeField]
	private List<GameObject> listClearStar = new List<GameObject>();

	[SerializeField]
	private LevelPlay_Booster boosterItem;

	private int levelIndex;

	private Transform trRewardItem;

	private LevelDbData levelData;

	private UserLevelState userLevelState;

	public Transform PlayButton => trPlayButton;

	public void Show(int index)
	{
		base.Show();
		GameUtil.SetUseHunterList();
		GameUtil.SetOwnHunterList(HUNTERLIST_TYPE.NORMAL);
		levelIndex = index;
		textStageName.text = MWLocalize.GetData(GameDataManager.GetDicStageDbData()[GameInfo.inGamePlayData.stage].stageName);
		textChapterLevel.text = string.Format("{0} {1} - {2} {3}", MWLocalize.GetData("common_text_chapter"), GameInfo.inGamePlayData.chapter, MWLocalize.GetData("common_text_level"), GameInfo.inGamePlayData.level);
		textPlayCost.text = $"{GameDataManager.GetLevelIndexDbData(GameInfo.inGamePlayData.levelIdx).energyCost}";
		imageStagePreview.sprite = GameDataManager.GetStagePreviewSprite(GameInfo.inGamePlayData.stage - 1);
		trRewardItem = MWPoolManager.Spawn("Item", $"Item_{GameDataManager.GetLevelIndexDbData(levelIndex).rewardFixItem}", trRewardItemAnchor);
		textRewardFixCount.text = $"x{GameDataManager.GetLevelIndexDbData(levelIndex).rewardFixCount}";
		levelData = GameDataManager.GetLevelIndexDbData(levelIndex);
		userLevelState = GameInfo.userData.GetUserLevelState(levelData.stage - 1, levelData.chapter - 1, levelData.levelIdx);
		BoostInit();
		RefreshHunterUseCard();
		ShowClearStar();
		ShowMonsterList();
		SetTotalHunterStat();
		SetDeckEditBT();
		UnityEngine.Debug.Log("Tutorial LeaderSkill = " + levelIndex);
		TutorialManager.CheckHunterDeckTutorial();
		
		if (LobbyManager.OpenDeckEdit != null && levelIndex == 13)
		{
			LobbyManager.OpenDeckEdit();
		}
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
		RemoveHunterCard();
	}

	public void RefreshDeck()
	{
		RemoveHunterCard();
		RefreshHunterUseCard();
		SetTotalHunterStat();
	}

	public void ShowMonster()
	{
		trMonsterListAnchor.gameObject.SetActive(value: true);
	}

	public void HideMonster()
	{
		trMonsterListAnchor.gameObject.SetActive(value: false);
	}

	private void RefreshHunterUseCard()
	{
		RemoveHunterCard();
		for (int i = 0; i < GameInfo.userData.huntersUseInfo.Length; i++)
		{
			Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
			HunterCard component = MWPoolManager.Spawn("Hunter", "HunterCard_" + GameInfo.userData.huntersUseInfo[i].hunterIdx, trHunterCardParent).GetComponent<HunterCard>();
			component.Init(HUNTERCARD_TYPE.LEVELPLAY, GameDataManager.GetHunterInfo(GameInfo.userData.huntersUseInfo[i].hunterIdx, GameInfo.userData.huntersUseInfo[i].hunterLevel, GameInfo.userData.huntersUseInfo[i].hunterTier), _isOwn: true, _isArena: false);
			component.HunterIdx = i;
			component.IsUseHunter = true;
			component.transform.localPosition = Vector3.zero;
			component.transform.localScale = Vector3.one;
			if (i == 0)
			{
				if (component.HunterInfo.Stat.hunterLeaderSkill == 0)
				{
					leaderSkill_Text.text = string.Format(MWLocalize.GetData("Popup_hunter_leaderskill_02"));
				}
				else
				{
					leaderSkill_Text.text = string.Format(MWLocalize.GetData(GameDataManager.GetHunterLeaderSkillData(component.HunterInfo.Stat.hunterLeaderSkill).leaderSkillDescription));
				}
			}
		}
	}

	private void RemoveHunterCard()
	{
		HunterCard[] componentsInChildren = trHunterCardParent.GetComponentsInChildren<HunterCard>();
		foreach (HunterCard hunterCard in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Hunter", hunterCard.transform);
		}
	}

	private void ShowClearStar()
	{
		UserLevelState[] levelList = GameInfo.userData.userStageState[GameInfo.inGamePlayData.stage - 1].chapterList[GameInfo.inGamePlayData.chapter - 1].levelList;
		UserLevelState[] array = levelList;
		foreach (UserLevelState userLevelState in array)
		{
			if (userLevelState.level == GameInfo.inGamePlayData.level)
			{
				for (int j = 0; j < listClearStar.Count; j++)
				{
					listClearStar[j].SetActive(j + 1 <= userLevelState.starCount);
				}
			}
		}
	}

	private void ShowMonsterList()
	{
		ShowMonster();
		foreach (KeyValuePair<int, int> levelMonster in GameUtil.GetLevelMonsterList(levelIndex))
		{
			Transform transform = MWPoolManager.Spawn("Lobby", "QuickLootMonster", trMonsterListAnchor);
			transform.GetComponent<ItemInfoUI>().Show("Info", $"UI_monster_{levelMonster.Key}", levelMonster.Value);
		}
	}

	private void OnGamePlayConncectComplete()
	{
		if (trRewardItem != null)
		{
			MWPoolManager.DeSpawn("Item", trRewardItem);
			trRewardItem = null;
		}
		ItemInfoUI[] componentsInChildren = trMonsterListAnchor.GetComponentsInChildren<ItemInfoUI>();
		ItemInfoUI[] array = componentsInChildren;
		foreach (ItemInfoUI itemInfoUI in array)
		{
			itemInfoUI.Clear();
			MWPoolManager.DeSpawn("Lobby", itemInfoUI.transform);
		}
		LobbyManager.GotoInGame(levelIndex);
	}

	private void SetTotalHunterStat()
	{
		totalHealth_current = 0;
		totalAttack_current = 0;
		totalRecovery_current = 0;
		for (int i = 0; i < trHunterCardParent.childCount; i++)
		{
			HunterInfo hunterInfo = trHunterCardParent.GetChild(i).GetComponent<HunterCard>().HunterInfo;
			totalHealth_current += (int)GameUtil.GetHunterReinForceHP(hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			totalAttack_current += (int)GameUtil.GetHunterReinForceAttack(hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			totalRecovery_current += (int)GameUtil.GetHunterReinForceHeal(hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
		}
		totalHealth_Text.text = "<color=#ffffff>" + string.Format(MWLocalize.GetData("popup_ingame_level_text_health"), StatTranslate(totalHealth_current)) + "</color>";
		totalAttack_Text.text = "<color=#ffffff>" + string.Format(MWLocalize.GetData("popup_ingame_level_text_attack"), StatTranslate(totalAttack_current)) + "</color>";
		totalRecovery_Text.text = "<color=#ffffff>" + string.Format(MWLocalize.GetData("popup_ingame_level_text_recovery"), StatTranslate(totalRecovery_current)) + "</color>";
	}

	private void BoostInit()
	{
		int[] itemType = new int[3]
		{
			levelData.boosterItem_1,
			levelData.boosterItem_2,
			levelData.boosterItem_3
		};
		boosterItem.Init(itemType);
	}

	private string StatTranslate(float _stat)
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

	private void SetDeckEditBT()
	{
		if (GameInfo.userData.huntersUseInfo.Length + GameInfo.userData.huntersOwnInfo.Length >= 5 && GameInfo.userData.userStageState[0].chapterList.Length >= 3)
		{
			trDeckEditBT.gameObject.SetActive(value: true);
			trDeckEditLockBT.gameObject.SetActive(value: false);
		}
		else
		{
			trDeckEditBT.gameObject.SetActive(value: false);
			trDeckEditLockBT.gameObject.SetActive(value: true);
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
		if (SelectLevelPlay != null)
		{
			SelectLevelPlay();
		}
		SoundController.EffectSound_Play(EffectSoundType.LevelPlay);
		GameInfo.inGamePlayData.stage = levelData.stage;
		GameInfo.inGamePlayData.chapter = levelData.chapter;
		GameInfo.inGamePlayData.level = levelData.level;
		GameInfo.inGamePlayData.levelIdx = levelData.levelIdx;
		List<int> list = new List<int>();
		boosterItem.AddBoostItem();
		if (GameInfo.inGamePlayData.dicActiveBoostItem.Count > 0)
		{
			foreach (KeyValuePair<int, BoostItemDbData> item in GameInfo.inGamePlayData.dicActiveBoostItem)
			{
				list.Add(item.Key);
			}
		}
		Protocol_Set.Protocol_game_start_Req(levelIndex, list, OnGamePlayConncectComplete);
	}

	public void OnClickLockDeckEdit()
	{
		trDeckEditToolTip.gameObject.SetActive(value: true);
	}

	public void OnClickBoostInfo()
	{
		int[] itemType = new int[3]
		{
			levelData.boosterItem_1,
			levelData.boosterItem_2,
			levelData.boosterItem_3
		};
		boostDescription.Init(itemType);
	}

	public void OnClickToolTip()
	{
		trDeckEditToolTip.gameObject.SetActive(value: false);
	}

	public void OnClickGoBack()
	{
		boosterItem.BoostItemCancel();
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	private void Start()
	{
	}

	private void OnDisable()
	{
		if (trRewardItem != null)
		{
			MWPoolManager.DeSpawn("Item", trRewardItem);
			trRewardItem = null;
		}
		ItemInfoUI[] componentsInChildren = trMonsterListAnchor.GetComponentsInChildren<ItemInfoUI>();
		ItemInfoUI[] array = componentsInChildren;
		foreach (ItemInfoUI itemInfoUI in array)
		{
			itemInfoUI.Clear();
			MWPoolManager.DeSpawn("Lobby", itemInfoUI.transform);
		}
		RemoveHunterCard();
	}
}
