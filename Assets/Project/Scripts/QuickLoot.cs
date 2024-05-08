using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickLoot : LobbyPopupBase
{
	public enum QuickLootState
	{
		Play = 1,
		Result
	}

	public Action GoBackEvent;

	[SerializeField]
	private Text textStageName;

	[SerializeField]
	private Text textChapterLevel;

	[SerializeField]
	private Text textPlayCost;

	[SerializeField]
	private Text leaderSkill_Text;

	[SerializeField]
	private Text totalHealth_Text;

	[SerializeField]
	private Text totalAttack_Text;

	[SerializeField]
	private Text totalRecovery_Text;

	[SerializeField]
	private Text textRewardFixCount;

	[SerializeField]
	private Image imageStagePreview;

	[SerializeField]
	private Button btnBack;

	[SerializeField]
	private Button btnPlay;

	[SerializeField]
	private Button btnAds;

	[SerializeField]
	private Button btnDone;

	[SerializeField]
	private ScrollRect scrollLoot;

	[SerializeField]
	private Transform trItemAnchor;

	[SerializeField]
	private Transform trMonsterListParent;

	[SerializeField]
	private Transform trItemListParent;

	[SerializeField]
	private Transform trLootsAnchor;

	[SerializeField]
	private Transform trHunterCardParent;

	[SerializeField]
	private Transform trDeckEditBT;

	[SerializeField]
	private Transform trDeckEditLockBT;

	[SerializeField]
	private Transform trDeckEditToolTip;

	[SerializeField]
	private GameObject goLoots;

	[SerializeField]
	private GameObject goHunterDeck;

	[SerializeField]
	private GameObject goMonsterList;

	[SerializeField]
	private GameObject goRewardItemList;

	[SerializeField]
	private GameObject goItemListCount;

	private int levelIndex;

	private int chestKeyCount;

	private int adsKey;

	private int totalHealth_current;

	private int totalAttack_current;

	private int totalRecovery_current;

	private float currentSecond;

	private string typeKey = string.Empty;

	private Transform trRewardItem;

	private LevelGameDbData levelData;

	private QuickLootState currentState;

	private UserLevelState userLevelState;

	private GAME_QUICK_LOOT quickLootData;

	public void Show(int index)
	{
		base.Open();
		levelIndex = index;
		levelData = GameDataManager.GetLevelIndexDbData(levelIndex);
		textStageName.text = MasterLocalize.GetData(GameDataManager.GetDicStageDbData()[GameInfo.inGamePlayData.stage].stageName);
		textChapterLevel.text = string.Format("{0} {1} - {2} {3}", MasterLocalize.GetData("common_text_chapter"), GameInfo.inGamePlayData.chapter, MasterLocalize.GetData("common_text_level"), GameInfo.inGamePlayData.level);
		textPlayCost.text = $"{GameDataManager.GetLevelIndexDbData(GameInfo.inGamePlayData.levelIdx).energyCost}";
		imageStagePreview.sprite = GameDataManager.GetStagePreviewSprite(GameInfo.inGamePlayData.stage - 1);
		typeKey = $"Stage_{GameInfo.inGamePlayData.stage}_Level_{GameInfo.inGamePlayData.level}";
		userLevelState = GameInfo.userData.GetUserLevelState(levelData.stage - 1, levelData.chapter - 1, levelData.levelIdx);
		SetDeckEditBT();
		ShowQuickLootState();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public void RefreshDeck()
	{
		if (base.gameObject.activeSelf)
		{
			RemoveHunterCard();
			RefreshHunterUseCard();
			SetTotalHunterStat();
		}
	}

	public void ShowMonster()
	{
		trMonsterListParent.gameObject.SetActive(value: true);
	}

	public void HideMonster()
	{
		trMonsterListParent.gameObject.SetActive(value: false);
	}

	private void ShowQuickLootState()
	{
		DespawnAll();
		currentState = QuickLootState.Play;
		goMonsterList.SetActive(value: true);
		goRewardItemList.SetActive(value: false);
		goLoots.SetActive(value: false);
		goHunterDeck.SetActive(value: true);
		btnBack.gameObject.SetActive(value: true);
		btnPlay.gameObject.SetActive(value: true);
		btnAds.gameObject.SetActive(value: true);
		btnDone.gameObject.SetActive(value: false);
		ShowMonsterList();
		ShowRewardItem();
		RefreshDeck();
	}

	private void ShowQuickLootResult()
	{
		DespawnAll();
		Transform transform = MasterPoolManager.SpawnObject("Effect", "FX_Quickroot", null, 3f);
		transform.position = Vector3.zero;
		SoundController.EffectSound_Play(EffectSoundType.OpenChapter);
		userLevelState = GameInfo.userData.GetUserLevelState(levelData.stage - 1, levelData.chapter - 1, levelData.levelIdx);
		currentState = QuickLootState.Result;
		goMonsterList.SetActive(value: false);
		goRewardItemList.SetActive(value: true);
		goLoots.SetActive(value: true);
		goHunterDeck.SetActive(value: false);
		btnBack.gameObject.SetActive(value: false);
		btnPlay.gameObject.SetActive(value: false);
		btnAds.gameObject.SetActive(value: false);
		btnDone.gameObject.SetActive(value: true);
		DespawnAll();
		ShowItemList();
		ShowLootItemList();
		ShowRewardItem();
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

	private void ShowMonsterList()
	{
		ShowMonster();
		ItemInfoUI[] componentsInChildren = trMonsterListParent.GetComponentsInChildren<ItemInfoUI>();
		ItemInfoUI[] array = componentsInChildren;
		foreach (ItemInfoUI itemInfoUI in array)
		{
			itemInfoUI.Clear();
			MasterPoolManager.ReturnToPool("Lobby", itemInfoUI.transform);
		}
		foreach (KeyValuePair<int, int> levelMonster in GameUtil.GetLevelMonsterList(levelIndex))
		{
			UnityEngine.Debug.Log("ShowMonsterList ::: " + levelMonster.Key);
			Transform transform = MasterPoolManager.SpawnObject("Lobby", "QuickLootMonster", trMonsterListParent);
			transform.GetComponent<ItemInfoUI>().Show("Info", $"UI_monster_{levelMonster.Key}", levelMonster.Value);
		}
	}

	private void ShowItemList()
	{
		REWARDITEM[] rewardMonsterItem = quickLootData.result.rewardMonsterItem;
		foreach (REWARDITEM rEWARDITEM in rewardMonsterItem)
		{
			if (!GameUtil.CheckUserInfoItem(rEWARDITEM.itemIdx))
			{
				Transform transform = MasterPoolManager.SpawnObject("Lobby", "QuickLootItem", trItemListParent);
				transform.GetComponent<ItemInfoUI>().Show("Item", $"Item_{rEWARDITEM.itemIdx}", rEWARDITEM.count);
			}
		}
		goItemListCount.SetActive(trItemListParent.childCount == 0);
	}

	private void ShowLootItemList()
	{
		if (chestKeyCount > 0)
		{
			ResultItemData resultItemData = new ResultItemData();
			resultItemData.itemIdx = 50033;
			resultItemData.itemMultiply = chestKeyCount;
			resultItemData.itemName = GameDataManager.GetItemListData(resultItemData.itemIdx).itemName;
			resultItemData.itemAmount = GameInfo.userData.GetItemCount(resultItemData.itemIdx);
			PuzzleResultItem component = MasterPoolManager.SpawnObject("Puzzle", "InGameResultItem", trLootsAnchor).GetComponent<PuzzleResultItem>();
			component.OpenMenu(resultItemData);
		}
		ResultItemData resultItemData2 = new ResultItemData();
		resultItemData2.itemIdx = 50040;
		resultItemData2.itemMultiply = quickLootData.result.rewardExp;
		resultItemData2.itemName = GameDataManager.GetItemListData(resultItemData2.itemIdx).itemName;
		resultItemData2.itemAmount = GameInfo.userData.GetItemCount(resultItemData2.itemIdx);
		PuzzleResultItem component2 = MasterPoolManager.SpawnObject("Puzzle", "InGameResultItem", trLootsAnchor).GetComponent<PuzzleResultItem>();
		component2.OpenMenu(resultItemData2);
		REWARDITEM[] rewardMonsterItem = quickLootData.result.rewardMonsterItem;
		foreach (REWARDITEM rEWARDITEM in rewardMonsterItem)
		{
			if (GameUtil.CheckUserInfoItem(rEWARDITEM.itemIdx))
			{
				ResultItemData resultItemData3 = new ResultItemData();
				resultItemData3.itemIdx = rEWARDITEM.itemIdx;
				resultItemData3.itemMultiply = rEWARDITEM.count;
				resultItemData3.itemName = GameDataManager.GetItemListData(resultItemData3.itemIdx).itemName;
				resultItemData3.itemAmount = GameInfo.userData.GetItemCount(resultItemData3.itemIdx);
				PuzzleResultItem component3 = MasterPoolManager.SpawnObject("Puzzle", "InGameResultItem", trLootsAnchor).GetComponent<PuzzleResultItem>();
				component3.OpenMenu(resultItemData3);
			}
		}
		ResultItemData resultItemData4 = new ResultItemData();
		resultItemData4.itemIdx = quickLootData.result.rewardChest[0].chestItem;
		resultItemData4.itemMultiply = quickLootData.result.rewardChest[0].chestItemN;
		resultItemData4.itemName = GameDataManager.GetItemListData(resultItemData4.itemIdx).itemName;
		resultItemData4.itemAmount = GameInfo.userData.GetItemCount(resultItemData4.itemIdx);
		PuzzleResultItem component4 = MasterPoolManager.SpawnObject("Puzzle", "InGameResultItem", trLootsAnchor).GetComponent<PuzzleResultItem>();
		component4.OpenMenu(resultItemData4);
		scrollLoot.horizontalNormalizedPosition = 0f;
	}

	private void ShowRewardItem()
	{
		if (trRewardItem == null)
		{
			trRewardItem = MasterPoolManager.SpawnObject("Item", $"Item_{GameDataManager.GetLevelIndexDbData(levelIndex).rewardFixItem}", trItemAnchor);
		}
		textRewardFixCount.text = $"x{GameDataManager.GetLevelIndexDbData(levelIndex).rewardFixCount}";
	}

	private void DespawnAll()
	{
		RemoveHunterCard();
		if (trRewardItem != null)
		{
			MasterPoolManager.ReturnToPool("Item", trRewardItem);
			trRewardItem = null;
		}
		ItemInfoUI[] componentsInChildren = trItemListParent.GetComponentsInChildren<ItemInfoUI>();
		ItemInfoUI[] array = componentsInChildren;
		foreach (ItemInfoUI itemInfoUI in array)
		{
			itemInfoUI.Clear();
			MasterPoolManager.ReturnToPool("Lobby", itemInfoUI.transform);
		}
		componentsInChildren = trMonsterListParent.GetComponentsInChildren<ItemInfoUI>();
		ItemInfoUI[] array2 = componentsInChildren;
		foreach (ItemInfoUI itemInfoUI2 in array2)
		{
			itemInfoUI2.Clear();
			MasterPoolManager.ReturnToPool("Lobby", itemInfoUI2.transform);
		}
		PuzzleResultItem[] componentsInChildren2 = trLootsAnchor.GetComponentsInChildren<PuzzleResultItem>();
		PuzzleResultItem[] array3 = componentsInChildren2;
		foreach (PuzzleResultItem inGameResultItem in array3)
		{
			inGameResultItem.ClearItems();
			MasterPoolManager.ReturnToPool("Puzzle", inGameResultItem.transform);
		}
	}

	private void OnQuickLootResultComplete(GAME_QUICK_LOOT data)
	{
		quickLootData = data;
		Protocol_Set.Protocol_user_item_info_Req(OnUserDataChangeComplete);
	}

	private void OnUserDataChangeComplete()
	{
		ShowQuickLootResult();
	}

	private int GetQuickLootChestKey()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < levelData.monsterCount; i++)
		{
			num2 = UnityEngine.Random.Range(1, 101);
			if (num2 <= GameDataManager.GetGameConfigData(ConfigDataType.DropKey))
			{
				num++;
			}
		}
		return num;
	}
	
	private void OnQuickLootAdStartConnect(int _adKey)
	{
		adsKey = _adKey;
		GameInfo.isResumeUserDataConnect = false;
	}

	private void RewardVideoComplete()
	{
		
	}

	private IEnumerator CheckUserQuickLoot(bool _isReward)
	{
		yield return null;
		if (_isReward)
		{
			chestKeyCount = GetQuickLootChestKey();
			Protocol_Set.Protocol_game_quick_loot_Req(levelIndex, chestKeyCount, adsKey, OnQuickLootResultComplete);
		}
		else
		{
			Protocol_Set.Protocol_user_item_info_Req();
		}
		GameInfo.isResumeUserDataConnect = true;
	}

	private void OnGamePlayConncectComplete()
	{
		DespawnAll();
		LobbyManager.GotoInGame(levelIndex);
	}

	private void RemoveHunterCard()
	{
		HeroCard[] componentsInChildren = trHunterCardParent.GetComponentsInChildren<HeroCard>();
		foreach (HeroCard hunterCard in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Hunter", hunterCard.transform);
		}
	}

	private void RefreshHunterUseCard()
	{
		for (int i = 0; i < GameInfo.userData.huntersUseInfo.Length; i++)
		{
			HeroCard component = MasterPoolManager.SpawnObject("Hunter", "HunterCard_" + GameInfo.userData.huntersUseInfo[i].hunterIdx, trHunterCardParent).GetComponent<HeroCard>();
			component.Construct(HerocardType.Levelplay, GameDataManager.GetHunterInfo(GameInfo.userData.huntersUseInfo[i].hunterIdx, GameInfo.userData.huntersUseInfo[i].hunterLevel, GameInfo.userData.huntersUseInfo[i].hunterTier), _isOwn: true, _isArena: false);
			component.HeroIdx = i;
			component.IsUseHero = true;
			component.transform.localPosition = Vector3.zero;
			component.transform.localScale = Vector3.one;
			if (i == 0)
			{
				if (component.HunterInfo.Stat.hunterLeaderSkill == 0)
				{
					leaderSkill_Text.text = string.Format(MasterLocalize.GetData("Popup_hunter_leaderskill_02"));
				}
				else
				{
					leaderSkill_Text.text = string.Format(MasterLocalize.GetData(GameDataManager.GetHunterLeaderSkillData(component.HunterInfo.Stat.hunterLeaderSkill).leaderSkillDescription));
				}
			}
		}
	}

	private void SetTotalHunterStat()
	{
		totalHealth_current = 0;
		totalAttack_current = 0;
		totalRecovery_current = 0;
		for (int i = 0; i < trHunterCardParent.childCount; i++)
		{
			HunterInfo hunterInfo = trHunterCardParent.GetChild(i).GetComponent<HeroCard>().HunterInfo;
			totalHealth_current += (int)GameUtil.GetHunterReinForceHP(hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			totalAttack_current += (int)GameUtil.GetHunterReinForceAttack(hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			totalRecovery_current += (int)GameUtil.GetHunterReinForceHeal(hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
		}
		totalHealth_Text.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_health"), StatTranslate(totalHealth_current)) + "</color>";
		totalAttack_Text.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_attack"), StatTranslate(totalAttack_current)) + "</color>";
		totalRecovery_Text.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_recovery"), StatTranslate(totalRecovery_current)) + "</color>";
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

	public void OnClickAds()
	{
		Protocol_Set.Protocol_shop_ad_start_Req(6, OnQuickLootAdStartConnect);
	}

	public void OnClickLevelPlay()
	{
		//TODO remove coment
		/*
		if (GameInfo.userData.userInfo.energy < levelData.energyCost)
		{
			LobbyManager.ShowUserEnergyInfo();
			AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.stamin_under_4);
			return;
		}
		*/
		
		GameInfo.userPlayData.Clear();
		SoundController.EffectSound_Play(EffectSoundType.LevelPlay);
		GameInfo.inGamePlayData.stage = levelData.stage;
		GameInfo.inGamePlayData.chapter = levelData.chapter;
		GameInfo.inGamePlayData.level = levelData.level;
		GameInfo.inGamePlayData.levelIdx = levelData.levelIdx;
		Protocol_Set.Protocol_game_start_Req(levelIndex, null, OnGamePlayConncectComplete);
	}

	public void OnClickDone()
	{
		ShowQuickLootState();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	public void OnClickLockDeckEdit()
	{
		trDeckEditToolTip.gameObject.SetActive(value: true);
	}

	public void OnClickToolTip()
	{
		trDeckEditToolTip.gameObject.SetActive(value: false);
	}

	private void Start()
	{
	}

	private void OnDisable()
	{
		DespawnAll();
		GameInfo.isResumeUserDataConnect = true;
	}
}
