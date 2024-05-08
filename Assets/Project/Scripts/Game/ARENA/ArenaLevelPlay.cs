using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaLevelPlay : LobbyPopupBase
{
	public Action GoBackEvent;

	public Action SelectLevelPlay;

	[SerializeField]
	private Text textChapterLevel;

	[SerializeField]
	private Text textTicketCost;

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
	private Text textColorBuff;

	[SerializeField]
	private Text textTribeBuff;

	[SerializeField]
	private int totalHealth_current;

	[SerializeField]
	private int totalAttack_current;

	[SerializeField]
	private int totalRecovery_current;

	[SerializeField]
	private GameObject[] arrGoColorBuff = new GameObject[0];

	[SerializeField]
	private GameObject[] arrGoTribeBuff = new GameObject[0];

	[SerializeField]
	private LevelPlay_Booster boosterItem;

	[SerializeField]
	private LevelPlay_BoosterDescription boostDescription;

	private Transform trRewardItem;

	private ARENA_INFO_DATA_RESULT arenaInfoData;

	public Transform PlayButton => trPlayButton;

	public void Show(ARENA_INFO_DATA_RESULT _data)
	{
		base.Show();
		arenaInfoData = _data;
		GameInfo.inGamePlayData.arenaInfo = arenaInfoData.arenaInfo;
		GameInfo.inGamePlayData.arenaLevelData = arenaInfoData.activeLevelData;
		BoostInit();
		GameUtil.SetUseArenaHunterList();
		GameUtil.SetOwnHunterList(HanterListType.Arena);
		textChapterLevel.text = string.Format(MWLocalize.GetData("arena_lobby_text_03"), _data.activeLevelData.levelIdx);
		textTicketCost.text = $"{_data.activeLevelData.costTicket}";
		string data = MWLocalize.GetData(GameDataManager.GetHunterColorName(arenaInfoData.arenaInfo.color).colorOccupation);
		textColorBuff.text = $"{data} X{arenaInfoData.arenaInfo.color_buff}";
		string data2 = MWLocalize.GetData(GameDataManager.GetHunterTribeName(arenaInfoData.arenaInfo.tribe));
		textTribeBuff.text = $"{data2} X{arenaInfoData.arenaInfo.tribe_buff}";
		for (int i = 0; i < arrGoColorBuff.Length; i++)
		{
			arrGoColorBuff[i].SetActive(i != arenaInfoData.arenaInfo.color);
		}
		for (int j = 0; j < arrGoTribeBuff.Length; j++)
		{
			arrGoTribeBuff[j].SetActive(j + 1 != arenaInfoData.arenaInfo.tribe);
		}
		trRewardItem = MWPoolManager.Spawn("Item", $"Item_{50044 + _data.activeLevelData.chestType - 1}", trRewardItemAnchor);
		RefreshHunterUseCard();
		ShowMonsterList();
		SetTotalHunterStat();
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

	public void ForceGoArena()
	{
		if (GameInfo.userData.userInfo.arenaTicket >= arenaInfoData.activeLevelData.costTicket)
		{
			List<int> list = new List<int>();
			boosterItem.AddBoostItem();
			if (GameInfo.inGamePlayData.dicActiveBoostItem.Count > 0)
			{
				foreach (KeyValuePair<int, BoostItemDbData> item in GameInfo.inGamePlayData.dicActiveBoostItem)
				{
					list.Add(item.Key);
				}
			}
			Protocol_Set.Protocol_arena_game_start_Req(arenaInfoData.activeLevelData.levelIdx, list, OnArenaGameStart);
		}
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
		UnityEngine.Debug.Log("********************** Set Arena Hunter 11 = " + GameInfo.userData.huntersArenaUseInfo.Length);
		for (int i = 0; i < GameInfo.userData.huntersArenaUseInfo.Length; i++)
		{
			UnityEngine.Debug.Log("********************** Set Arena Hunter 22");
			HeroCard component = MWPoolManager.Spawn("Hunter", "HunterCard_" + GameInfo.userData.huntersArenaUseInfo[i].hunterIdx, trHunterCardParent).GetComponent<HeroCard>();
			component.Construct(HerocardType.Levelplay, GameDataManager.GetHunterInfo(GameInfo.userData.huntersArenaUseInfo[i].hunterIdx, GameInfo.userData.huntersArenaUseInfo[i].hunterLevel, GameInfo.userData.huntersArenaUseInfo[i].hunterTier), _isOwn: true, _isArena: true);
			component.HeroIdx = i;
			component.IsUseHero = true;
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
		HeroCard[] componentsInChildren = trHunterCardParent.GetComponentsInChildren<HeroCard>();
		foreach (HeroCard hunterCard in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Hunter", hunterCard.transform);
		}
	}

	private void ShowMonsterList()
	{
		ShowMonster();
		ARENA_MONSTER_DATA[] monsterList = arenaInfoData.monsterList;
		foreach (ARENA_MONSTER_DATA aRENA_MONSTER_DATA in monsterList)
		{
			Transform transform = MWPoolManager.Spawn("Lobby", "QuickLootMonster", trMonsterListAnchor);
			transform.GetComponent<ItemInfoUI>().Show("Info", $"UI_monster_{aRENA_MONSTER_DATA.uiImage}", aRENA_MONSTER_DATA.count);
		}
	}

	private void SetTotalHunterStat()
	{
		UnityEngine.Debug.Log("ArenaLevelPlay SetTotalHunterStat");
		totalHealth_current = 0;
		totalAttack_current = 0;
		totalRecovery_current = 0;
		for (int i = 0; i < trHunterCardParent.childCount; i++)
		{
			HunterInfo hunterInfo = trHunterCardParent.GetChild(i).GetComponent<HeroCard>().HunterInfo;
			totalHealth_current += (int)GameUtil.GetHunterReinForceHP(hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			totalAttack_current += CheckArenaBuff(hunterInfo) * (int)GameUtil.GetHunterReinForceAttack(hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			totalRecovery_current += (int)GameUtil.GetHunterReinForceHeal(hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
		}
		totalHealth_Text.text = "<color=#ffffff>" + string.Format(MWLocalize.GetData("popup_ingame_level_text_health"), StatTranslate(totalHealth_current)) + "</color>";
		totalAttack_Text.text = "<color=#ffffff>" + string.Format(MWLocalize.GetData("popup_ingame_level_text_attack"), StatTranslate(totalAttack_current)) + "</color>";
		totalRecovery_Text.text = "<color=#ffffff>" + string.Format(MWLocalize.GetData("popup_ingame_level_text_recovery"), StatTranslate(totalRecovery_current)) + "</color>";
	}

	private int CheckArenaBuff(HunterInfo _hunterinfo)
	{
		int num = 1;
		if (GameInfo.inGamePlayData.arenaInfo == null)
		{
			return num;
		}
		if (GameInfo.inGamePlayData.arenaInfo.color == _hunterinfo.Hunter.color)
		{
			num *= GameInfo.inGamePlayData.arenaInfo.color_buff;
		}
		if (GameInfo.inGamePlayData.arenaInfo.tribe == _hunterinfo.Hunter.hunterTribe)
		{
			num *= GameInfo.inGamePlayData.arenaInfo.tribe_buff;
		}
		return num;
	}

	private void BoostInit()
	{
		int[] itemType = new int[3]
		{
			GameInfo.inGamePlayData.arenaLevelData.booster1,
			GameInfo.inGamePlayData.arenaLevelData.booster2,
			GameInfo.inGamePlayData.arenaLevelData.booster3
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

	private void OnArenaGameStart(ARENA_GAME_START_RESULT _result)
	{
		UnityEngine.Debug.Log("OnArenaGameStart");
		LobbyManager.GotoArena(_result);
	}

	public void OnClickPlay()
	{
		if (GameInfo.userData.userInfo.arenaTicket < arenaInfoData.activeLevelData.costTicket)
		{
			LobbyManager.ShowArenaTicketNone();
		}
		else
		{
			List<int> list = new List<int>();
			boosterItem.AddBoostItem();
			if (GameInfo.inGamePlayData.dicActiveBoostItem.Count > 0)
			{
				foreach (KeyValuePair<int, BoostItemDbData> item in GameInfo.inGamePlayData.dicActiveBoostItem)
				{
					list.Add(item.Key);
				}
			}
			Protocol_Set.Protocol_arena_game_start_Req(arenaInfoData.activeLevelData.levelIdx, list, OnArenaGameStart);
		}
		SoundController.EffectSound_Play(EffectSoundType.LevelPlay);
	}

	public void OnClickBoostInfo()
	{
		int[] itemType = new int[3]
		{
			GameInfo.inGamePlayData.arenaLevelData.booster1,
			GameInfo.inGamePlayData.arenaLevelData.booster2,
			GameInfo.inGamePlayData.arenaLevelData.booster3
		};
		boostDescription.Init(itemType);
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
