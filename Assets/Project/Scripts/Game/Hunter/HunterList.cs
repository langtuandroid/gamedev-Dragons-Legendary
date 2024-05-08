using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HunterList : LobbyPopupBase
{
	public enum SortType
	{
		SORT = 1,
		ATTACK,
		TIER,
		TRIBE,
		ELEMENT
	}

	public Action GoBackEvent;

	[SerializeField]
	private Transform trHunterContent;

	[SerializeField]
	private Transform trGetHunter;

	[SerializeField]
	private Transform trMyDeckHunterCardParent;

	[SerializeField]
	private Transform trDeckEditToolTip;

	[SerializeField]
	private Text hunterCount_Text;

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
	private Transform trDeckEditBT;

	[SerializeField]
	private Transform trDeckEditLockBT;

	[SerializeField]
	private List<HunterInfo> Hunter_Own_List = new List<HunterInfo>();

	[SerializeField]
	private List<HunterInfo> Hunter_NotOwn_List = new List<HunterInfo>();

	[SerializeField]
	private List<string> dropdownList;

	[SerializeField]
	private SortType sortType = SortType.SORT;

	[SerializeField]
	private Dropdown sortDropdown;

	public Transform GetHunter => trGetHunter;

	public override void Open()
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		Init();
	}

	public void SetInit()
	{
		UnityEngine.Debug.Log("*************** Test 11");
		Init();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
		HeroCard[] componentsInChildren = trHunterContent.GetComponentsInChildren<HeroCard>();
		foreach (HeroCard hunterCard in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Hunter", hunterCard.transform);
		}
	}

	public void HunterListSorting_Type(int _type)
	{
		switch (_type)
		{
		case 0:
			HunterListSorting_HunterIdx();
			break;
		case 1:
			HunterListSorting_Damage();
			break;
		case 2:
			HunterListSorting_Tier();
			break;
		case 3:
			HunterListSorting_Tribe();
			break;
		case 4:
			HunterListSorting_Element();
			break;
		}
		HeroCard[] componentsInChildren = trHunterContent.GetComponentsInChildren<HeroCard>();
		foreach (HeroCard hunterCard in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Hunter", hunterCard.transform);
		}
		HunterCard_Spawn();
	}

	public void HunterListSorting_HunterIdx()
	{
		Hunter_Own_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (x.Stat.hunterIdx > y.Stat.hunterIdx)
			{
				return 1;
			}
			return (x.Stat.hunterIdx < y.Stat.hunterIdx) ? (-1) : 0;
		});
		Hunter_NotOwn_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (x.Stat.hunterIdx > y.Stat.hunterIdx)
			{
				return 1;
			}
			return (x.Stat.hunterIdx < y.Stat.hunterIdx) ? (-1) : 0;
		});
		sortType = SortType.SORT;
	}

	public void HunterListSorting_Damage()
	{
		Hunter_Own_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (GameUtil.GetHunterReinForceAttack(x.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(x.Hunter.hunterIdx)) < GameUtil.GetHunterReinForceAttack(y.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(y.Hunter.hunterIdx)))
			{
				return 1;
			}
			return (GameUtil.GetHunterReinForceAttack(x.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(x.Hunter.hunterIdx)) > GameUtil.GetHunterReinForceAttack(y.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(y.Hunter.hunterIdx))) ? (-1) : HunterListSorting_Sub(x, y);
		});
		Hunter_NotOwn_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (GameUtil.GetHunterReinForceAttack(x.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(x.Hunter.hunterIdx)) < GameUtil.GetHunterReinForceAttack(y.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(y.Hunter.hunterIdx)))
			{
				return 1;
			}
			return (GameUtil.GetHunterReinForceAttack(x.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(x.Hunter.hunterIdx)) > GameUtil.GetHunterReinForceAttack(y.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(y.Hunter.hunterIdx))) ? (-1) : HunterListSorting_Sub(x, y);
		});
		sortType = SortType.ATTACK;
	}

	public void HunterListSorting_Tier()
	{
		Hunter_Own_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (x.Stat.hunterTier < y.Stat.hunterTier)
			{
				return 1;
			}
			return (x.Stat.hunterTier > y.Stat.hunterTier) ? (-1) : HunterListSorting_Sub(x, y);
		});
		Hunter_NotOwn_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (x.Stat.hunterTier < y.Stat.hunterTier)
			{
				return 1;
			}
			return (x.Stat.hunterTier > y.Stat.hunterTier) ? (-1) : HunterListSorting_Sub(x, y);
		});
		sortType = SortType.TIER;
	}

	public void HunterListSorting_Tribe()
	{
		Hunter_Own_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (x.Hunter.hunterTribe < y.Hunter.hunterTribe)
			{
				return 1;
			}
			return (x.Hunter.hunterTribe > y.Hunter.hunterTribe) ? (-1) : HunterListSorting_Sub(x, y);
		});
		Hunter_NotOwn_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (x.Hunter.hunterTribe < y.Hunter.hunterTribe)
			{
				return 1;
			}
			return (x.Hunter.hunterTribe > y.Hunter.hunterTribe) ? (-1) : HunterListSorting_Sub(x, y);
		});
		sortType = SortType.TRIBE;
	}

	public void HunterListSorting_Element()
	{
		Hunter_Own_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (x.Hunter.color < y.Hunter.color)
			{
				return 1;
			}
			return (x.Hunter.color > y.Hunter.color) ? (-1) : HunterListSorting_Sub(x, y);
		});
		Hunter_NotOwn_List.Sort(delegate(HunterInfo x, HunterInfo y)
		{
			if (x.Hunter.color < y.Hunter.color)
			{
				return 1;
			}
			return (x.Hunter.color > y.Hunter.color) ? (-1) : HunterListSorting_Sub(x, y);
		});
		sortType = SortType.ELEMENT;
	}

	public int HunterListSorting_Sub(HunterInfo x, HunterInfo y)
	{
		if (x.Hunter.hunterIdx > y.Hunter.hunterIdx)
		{
			return 1;
		}
		if (x.Hunter.hunterIdx < y.Hunter.hunterIdx)
		{
			return -1;
		}
		return 0;
	}

	public void RefreshDeck()
	{
		RefreshHunterUseCard();
		SetTotalHunterStat();
	}

	private void Init()
	{
		GameUtil.SetUseHunterList();
		GameUtil.SetOwnHunterList(HanterListType.Normal);
		if (Hunter_Own_List != null)
		{
			Hunter_Own_List.Clear();
		}
		if (Hunter_NotOwn_List != null)
		{
			Hunter_NotOwn_List.Clear();
		}
		HunterListSetting();
		SetDropDownList();
		SetSorting();
		RefreshHunterUseCard();
		SetTotalHunterStat();
		SetDeckEditBT();
		SetHunterCount();
		if (LobbyManager.OpenChestOpenEnchant != null)
		{
			LobbyManager.OpenChestOpenEnchant();
		}
	}

	private void RefreshHunterUseCard()
	{
		HeroCard[] componentsInChildren = trMyDeckHunterCardParent.GetComponentsInChildren<HeroCard>();
		foreach (HeroCard hunterCard in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Hunter", hunterCard.transform);
		}
		for (int j = 0; j < GameInfo.userData.huntersUseInfo.Length; j++)
		{
			HeroCard component = MasterPoolManager.SpawnObject("Hunter", "HunterCard_" + GameInfo.userData.huntersUseInfo[j].hunterIdx, trMyDeckHunterCardParent).GetComponent<HeroCard>();
			component.Construct(HerocardType.Levelplay, GameDataManager.GetHunterInfo(GameInfo.userData.huntersUseInfo[j].hunterIdx, GameInfo.userData.huntersUseInfo[j].hunterLevel, GameInfo.userData.huntersUseInfo[j].hunterTier), _isOwn: true, _isArena: false);
			component.HeroIdx = j;
			component.IsUseHero = true;
			component.transform.localPosition = Vector3.zero;
			component.transform.localScale = Vector3.one;
			if (j == 0)
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
		for (int i = 0; i < trMyDeckHunterCardParent.childCount; i++)
		{
			HunterInfo hunterInfo = trMyDeckHunterCardParent.GetChild(i).GetComponent<HeroCard>().HunterInfo;
			totalHealth_current += (int)GameUtil.GetHunterReinForceHP(hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			totalAttack_current += (int)GameUtil.GetHunterReinForceAttack(hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
			totalRecovery_current += (int)GameUtil.GetHunterReinForceHeal(hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo.Hunter.hunterIdx));
		}
		totalHealth_Text.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_health"), StatTranslate(totalHealth_current)) + "</color>";
		totalAttack_Text.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_attack"), StatTranslate(totalAttack_current)) + "</color>";
		totalRecovery_Text.text = "<color=#ffffff>" + string.Format(MasterLocalize.GetData("popup_ingame_level_text_recovery"), StatTranslate(totalRecovery_current)) + "</color>";
	}

	private void SetHunterCount()
	{
		hunterCount_Text.text = string.Format(MasterLocalize.GetData("popup_hunter_text_10")) + " " + Hunter_Own_List.Count + " / " + (Hunter_Own_List.Count + Hunter_NotOwn_List.Count);
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

	private void SetDropDownList()
	{
		if (dropdownList != null)
		{
			dropdownList = null;
		}
		dropdownList = new List<string>();
		sortDropdown.ClearOptions();
		dropdownList.Add(MasterLocalize.GetData("popup_hunter_text_2"));
		dropdownList.Add(MasterLocalize.GetData("popup_hunter_text_5"));
		dropdownList.Add(MasterLocalize.GetData("popup_hunter_text_3"));
		dropdownList.Add(MasterLocalize.GetData("popup_hunter_text_class"));
		dropdownList.Add(MasterLocalize.GetData("popup_hunter_text_element"));
		sortDropdown.AddOptions(dropdownList);
	}

	private void HunterListSetting()
	{
		Dictionary<int, HunterDbData> hunterList = GameDataManager.GetHunterList();
		int childCount = trHunterContent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MasterPoolManager.ReturnToPool("Hunter", trHunterContent.GetChild(0).transform);
		}
		for (int j = 0; j < GameInfo.userData.huntersUseInfo.Length; j++)
		{
			HunterInfo hunterInfo = new HunterInfo();
			hunterInfo = GameDataManager.GetHunterInfo(GameInfo.userData.huntersUseInfo[j].hunterIdx, GameInfo.userData.huntersUseInfo[j].hunterLevel, GameInfo.userData.huntersUseInfo[j].hunterTier);
			Hunter_Own_List.Add(hunterInfo);
		}
		for (int k = 0; k < GameInfo.userData.huntersOwnInfo.Length; k++)
		{
			HunterInfo hunterInfo2 = new HunterInfo();
			hunterInfo2 = GameDataManager.GetHunterInfo(GameInfo.userData.huntersOwnInfo[k].hunterIdx, GameInfo.userData.huntersOwnInfo[k].hunterLevel, GameInfo.userData.huntersOwnInfo[k].hunterTier);
			Hunter_Own_List.Add(hunterInfo2);
		}
		foreach (KeyValuePair<int, HunterDbData> item in hunterList)
		{
			bool flag = false;
			for (int l = 0; l < Hunter_Own_List.Count; l++)
			{
				if (item.Key == Hunter_Own_List[l].Hunter.hunterIdx)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				HunterInfo hunterInfo3 = new HunterInfo();
				UnityEngine.Debug.Log("idx.Key = " + item.Key);
				hunterInfo3 = GameDataManager.GetHunterInfo(item.Key, 1, 1);
				Hunter_NotOwn_List.Add(hunterInfo3);
			}
		}
		HunterCard_Spawn();
		UnityEngine.Debug.Log("Fuck 1111");
	}

	private void SetSorting()
	{
		UnityEngine.Debug.Log("this.sortType = " + sortType);
		switch (sortType)
		{
		case SortType.SORT:
			UnityEngine.Debug.Log("3333");
			HunterListSorting_Type(0);
			break;
		case SortType.ATTACK:
			UnityEngine.Debug.Log("4444");
			HunterListSorting_Type(1);
			break;
		case SortType.TIER:
			HunterListSorting_Type(2);
			break;
		case SortType.TRIBE:
			HunterListSorting_Type(3);
			break;
		case SortType.ELEMENT:
			HunterListSorting_Type(4);
			break;
		}
	}

	private void HunterCard_Spawn()
	{
		for (int i = 0; i < Hunter_Own_List.Count; i++)
		{
			HeroCard component = MasterPoolManager.SpawnObject("Hunter", "HunterCard_" + Hunter_Own_List[i].Stat.hunterIdx, trHunterContent).GetComponent<HeroCard>();
			component.Construct(HerocardType.Hunterlist, GameDataManager.GetHunterInfo(Hunter_Own_List[i].Stat.hunterIdx, Hunter_Own_List[i].Stat.hunterLevel, Hunter_Own_List[i].Stat.hunterTier), _isOwn: true, _isArena: false);
			component.HeroIdx = i;
			component.transform.localPosition = Vector3.zero;
			component.transform.localScale = Vector3.one;
			if (Hunter_Own_List[i].Stat.hunterIdx == 20001)
			{
				trGetHunter = component.transform;
			}
		}
		for (int j = 0; j < Hunter_NotOwn_List.Count; j++)
		{
			HeroCard component2 = MasterPoolManager.SpawnObject("Hunter", "HunterCard_" + Hunter_NotOwn_List[j].Stat.hunterIdx, trHunterContent).GetComponent<HeroCard>();
			component2.Construct(HerocardType.Hunterlist, GameDataManager.GetHunterInfo(Hunter_NotOwn_List[j].Stat.hunterIdx, Hunter_NotOwn_List[j].Hunter.maxTier * 20, Hunter_NotOwn_List[j].Hunter.maxTier), _isOwn: false, _isArena: false);
			component2.HeroIdx = j;
			component2.transform.localPosition = Vector3.zero;
			component2.transform.localScale = Vector3.one;
		}
		UnityEngine.Debug.Log("Fuck 2222");
	}

	public void OnClickLockDeckEdit()
	{
		trDeckEditToolTip.gameObject.SetActive(value: true);
	}

	public void OnClickToolTip()
	{
		trDeckEditToolTip.gameObject.SetActive(value: false);
	}

	public void OnClickGoBack()
	{
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}
}
