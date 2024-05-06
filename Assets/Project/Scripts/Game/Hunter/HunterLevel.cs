using System;
using UnityEngine;
using UnityEngine.UI;

public class HunterLevel : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform hunterCharactertr;

	[SerializeField]
	private HunterView_Color hunter_Character;

	[SerializeField]
	private Transform hunterRequiredItemListtr;

	[SerializeField]
	private Text hunter_Level_Origin;

	[SerializeField]
	private Text hunter_Level_After;

	[SerializeField]
	private Text hunter_HP_Origin;

	[SerializeField]
	private Text hunter_HP_After;

	[SerializeField]
	private Text hunter_Attack_Origin;

	[SerializeField]
	private Text hunter_Attack_After;

	[SerializeField]
	private Text hunter_Recovery_Origin;

	[SerializeField]
	private Text hunter_Recovery_After;

	[SerializeField]
	private Text levelUp_Btn_Text;

	[SerializeField]
	private HunterInfo hunterInfo_Origin;

	[SerializeField]
	private HunterInfo hunterInfo_After;

	[SerializeField]
	private RequiredItem_Cell requiredItem_Cell;

	[SerializeField]
	private RequiredItem_Cell requiredCoin_Cell;

	[SerializeField]
	private HunterLevelDbData hunterLevelDbData;

	[SerializeField]
	private Image levelUp_Btn_Lock;

	[SerializeField]
	private int current_level;

	[SerializeField]
	private int target_level;

	[SerializeField]
	private int max_level;

	[SerializeField]
	private int current_HP;

	[SerializeField]
	private int target_HP;

	[SerializeField]
	private int current_Attack;

	[SerializeField]
	private int target_Attack;

	[SerializeField]
	private int current_Recovery;

	[SerializeField]
	private int target_Recovery;

	[SerializeField]
	private bool isLevelUp_Condition1;

	[SerializeField]
	private bool isLevelUp_Condition2;

	[SerializeField]
	private bool isLevelUp_Condition_Total;

	[SerializeField]
	private int total_token;

	[SerializeField]
	private int total_coin;

	public HunterInfo HunterInfo => hunterInfo_Origin;

	public Transform HunterTransform => hunter_Character.transform;

	public bool HunterCheckNull()
	{
		if (hunter_Character != null)
		{
			return true;
		}
		return false;
	}

	public void Show(HunterInfo _hunterInfo)
	{
		base.Show();
		base.gameObject.SetActive(value: true);
		Init(_hunterInfo);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
	}

	public void SetInit(HunterInfo _hunterInfo)
	{
		Init(_hunterInfo);
	}

	public void TargetLevel_Up()
	{
		if (target_level + 1 <= hunterInfo_Origin.Stat.hunterTier * 20)
		{
			target_level++;
			SetHunterData();
			requiredItem_Cell.SetCostText(SetCostText_Token());
			requiredCoin_Cell.SetCostText(SetCostText_Coin());
			if (GameInfo.userData.userInfo.coin < total_coin)
			{
				requiredCoin_Cell.SetClickType(ItemClickType.Coin, total_coin - GameInfo.userData.userInfo.coin);
			}
			else
			{
				requiredCoin_Cell.SetClickType(ItemClickType.None);
			}
		}
	}

	public void TargetLevel_Down()
	{
		if (current_level + 1 < target_level)
		{
			target_level--;
			SetHunterData();
			requiredItem_Cell.SetCostText(SetCostText_Token());
			requiredCoin_Cell.SetCostText(SetCostText_Coin());
			if (GameInfo.userData.userInfo.coin < total_coin)
			{
				requiredCoin_Cell.SetClickType(ItemClickType.Coin, total_coin - GameInfo.userData.userInfo.coin);
			}
			else
			{
				requiredCoin_Cell.SetClickType(ItemClickType.None);
			}
		}
	}

	public void Click_Level_Up()
	{
		if (!isLevelUp_Condition_Total)
		{
			UnityEngine.Debug.Log("Level Up Condition Fail!");
			return;
		}
		UnityEngine.Debug.Log("target_level :: " + target_level);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (LobbyManager.OpenHunterLevelUp != null)
		{
			Protocol_Set.Protocol_hunter_level_up_Req(hunterInfo_Origin.Hunter.hunterIdx, target_level, LevelUpResponse, isTutorial: true);
		}
		else
		{
			Protocol_Set.Protocol_hunter_level_up_Req(hunterInfo_Origin.Hunter.hunterIdx, target_level, LevelUpResponse);
		}
	}

	public void SetForceLevelupCondition()
	{
		isLevelUp_Condition1 = true;
		isLevelUp_Condition2 = true;
		isLevelUp_Condition_Total = true;
		levelUp_Btn_Lock.gameObject.SetActive(value: false);
	}

	private void Init(HunterInfo _hunterInfo)
	{
		RequiredItem_Cell[] componentsInChildren = hunterRequiredItemListtr.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Grow", requiredItem_Cell.transform);
		}
		isLevelUp_Condition1 = true;
		isLevelUp_Condition2 = true;
		isLevelUp_Condition_Total = true;
		total_token = 0;
		total_coin = 0;
		hunterInfo_Origin = _hunterInfo;
		hunterLevelDbData = GameDataManager.GetHunterLevelData(hunterInfo_Origin.Hunter.hunterIdx, hunterInfo_Origin.Stat.hunterLevel, hunterInfo_Origin.Stat.hunterTier);
		if (hunter_Character != null)
		{
			MWPoolManager.DeSpawn("Hunter", hunter_Character.transform);
			hunter_Character = null;
		}
		switch (_hunterInfo.Hunter.color)
		{
		case 0:
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_B", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		case 1:
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_G", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		case 2:
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_P", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		case 3:
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_R", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		case 4:
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_Y", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		}
		hunter_Character.transform.SetAsFirstSibling();
		hunter_Character.transform.localPosition = new Vector3(0f, 130f, 0f);
		hunter_Character.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		hunter_Character.Init(_hunterInfo);
		current_level = hunterInfo_Origin.Stat.hunterLevel;
		if (Check_MaxLevel())
		{
			target_level = current_level;
		}
		else
		{
			target_level = current_level + 1;
		}
		SetHunterData();
		SetRequiredItem();
		NotEnouchCoin.CallBuyCoin = OnCallBuyCoinEvent;
	}

	private bool Check_MaxLevel()
	{
		bool result = false;
		if (hunterInfo_Origin.Hunter.maxTier * 20 == hunterInfo_Origin.Stat.hunterLevel)
		{
			result = true;
		}
		return result;
	}

	private void SetHunterData()
	{
		hunterInfo_After = GetHunterInfo();
		hunter_Level_Origin.text = MWLocalize.GetData("common_text_level") + current_level;
		if (Check_MaxLevel())
		{
			hunter_Level_After.text = target_level.ToString() + MWLocalize.GetData("common_text_max");
		}
		else
		{
			hunter_Level_After.text = target_level.ToString();
		}
		current_HP = (int)GameUtil.GetHunterReinForceHP(hunterInfo_Origin.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo_Origin.Hunter.hunterIdx));
		target_HP = (int)GameUtil.GetHunterReinForceHP(hunterInfo_After.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunterInfo_After.Hunter.hunterIdx));
		hunter_HP_Origin.text = GameUtil.InsertCommaInt(current_HP);
		hunter_HP_After.text = GameUtil.InsertCommaInt(target_HP);
		current_Attack = (int)GameUtil.GetHunterReinForceAttack(hunterInfo_Origin.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo_Origin.Hunter.hunterIdx));
		target_Attack = (int)GameUtil.GetHunterReinForceAttack(hunterInfo_After.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunterInfo_After.Hunter.hunterIdx));
		hunter_Attack_Origin.text = GameUtil.InsertCommaInt(current_Attack);
		hunter_Attack_After.text = GameUtil.InsertCommaInt(target_Attack);
		current_Recovery = (int)GameUtil.GetHunterReinForceHeal(hunterInfo_Origin.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo_Origin.Hunter.hunterIdx));
		target_Recovery = (int)GameUtil.GetHunterReinForceHeal(hunterInfo_After.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunterInfo_After.Hunter.hunterIdx));
		hunter_Recovery_Origin.text = GameUtil.InsertCommaInt(current_Recovery);
		hunter_Recovery_After.text = GameUtil.InsertCommaInt(target_Recovery);
		levelUp_Btn_Text.text = MWLocalize.GetData("common_text_level") + " x" + (target_level - current_level).ToString();
	}

	private void LevelUpResponse()
	{
		hunter_Character.gameObject.SetActive(value: false);
		LobbyManager.ShowHunterLevelUp(hunterInfo_Origin, hunterInfo_After, _isSpawn: true);
		UnityEngine.Debug.Log("Before Level = " + hunterInfo_Origin.Stat.hunterLevel);
		UnityEngine.Debug.Log("After Level = " + hunterInfo_After.Stat.hunterLevel);
	}

	private void Check_LevelUp()
	{
		if (isLevelUp_Condition1 && isLevelUp_Condition2)
		{
			isLevelUp_Condition_Total = true;
			levelUp_Btn_Lock.gameObject.SetActive(value: false);
		}
		else
		{
			isLevelUp_Condition_Total = false;
			levelUp_Btn_Lock.gameObject.SetActive(value: true);
		}
	}

	private HunterInfo GetHunterInfo()
	{
		UnityEngine.Debug.Log("this.target_level = " + target_level);
		return GameDataManager.GetHunterInfo(hunterInfo_Origin.Hunter.hunterIdx, target_level, hunterInfo_Origin.Stat.hunterTier);
	}

	private void SetRequiredItem()
	{
		requiredItem_Cell = MWPoolManager.Spawn("Grow", "cell_token", hunterRequiredItemListtr).GetComponent<RequiredItem_Cell>();
		requiredItem_Cell.transform.localScale = Vector3.one;
		requiredItem_Cell.transform.SetAsFirstSibling();
		requiredItem_Cell.SetItemImg(hunterLevelDbData.hnil);
		requiredItem_Cell.SetCostText(SetCostText_Token());
		requiredCoin_Cell = MWPoolManager.Spawn("Grow", "cell_coin", hunterRequiredItemListtr).GetComponent<RequiredItem_Cell>();
		requiredCoin_Cell.transform.localScale = Vector3.one;
		requiredCoin_Cell.transform.SetAsLastSibling();
		requiredCoin_Cell.SetCostText(SetCostText_Coin());
		if (GameInfo.userData.userInfo.coin < hunterLevelDbData.needCoin)
		{
			requiredCoin_Cell.SetClickType(ItemClickType.Coin, hunterLevelDbData.needCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			requiredCoin_Cell.SetClickType(ItemClickType.None);
		}
	}

	private string SetCostText_Token()
	{
		string result = string.Empty;
		int num = 0;
		bool flag = true;
		for (int i = current_level; i < target_level; i++)
		{
			num += GameDataManager.GetHunterLevelData(hunterInfo_Origin.Hunter.hunterIdx, i, hunterInfo_Origin.Stat.hunterTier).hnil_N;
		}
		total_token = num;
		for (int j = 0; j < GameInfo.userData.userItemList.Length; j++)
		{
			if (GameInfo.userData.userItemList[j].itemIdx == hunterLevelDbData.hnil)
			{
				flag = false;
				if (GameInfo.userData.userItemList[j].count >= total_token)
				{
					isLevelUp_Condition1 = true;
					result = "<color=#ffffff>" + GameInfo.userData.userItemList[j].count + "</color>/" + num;
				}
				else
				{
					isLevelUp_Condition1 = false;
					result = "<color=#ff0000>" + GameInfo.userData.userItemList[j].count + "</color>/" + num;
				}
			}
		}
		if (flag)
		{
			isLevelUp_Condition1 = false;
			result = "<color=#ff0000>0</color>/" + num;
		}
		Check_LevelUp();
		return result;
	}

	private string SetCostText_Coin()
	{
		string empty = string.Empty;
		int num = 0;
		for (int i = current_level; i < target_level; i++)
		{
			num += GameDataManager.GetHunterLevelData(hunterInfo_Origin.Hunter.hunterIdx, i, hunterInfo_Origin.Stat.hunterTier).needCoin;
		}
		total_coin = num;
		if (GameInfo.userData.userInfo.coin >= total_coin)
		{
			isLevelUp_Condition2 = true;
			empty = "<color=#ffffff>" + GameInfo.userData.userInfo.coin + "</color>/" + num;
		}
		else
		{
			isLevelUp_Condition2 = false;
			empty = "<color=#ff0000>" + GameInfo.userData.userInfo.coin + "</color>/" + num;
		}
		Check_LevelUp();
		return empty;
	}

	private void OnCallBuyCoinEvent(int _needJewel)
	{
		Protocol_Set.Protocol_shop_popup_hunter_buy_coin_Req(hunterInfo_Origin.Hunter.hunterIdx, target_level, _needJewel, OnBuyCoinComplete);
	}

	private void OnBuyCoinComplete()
	{
		requiredCoin_Cell.SetCostText(SetCostText_Coin());
		if (GameInfo.userData.userInfo.coin < total_coin)
		{
			requiredCoin_Cell.SetClickType(ItemClickType.Coin, total_coin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			requiredCoin_Cell.SetClickType(ItemClickType.None);
		}
	}

	public void OnClickGoBack()
	{
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		LobbyManager.ShowHunterView(hunterInfo_Origin, _isSpawn: false);
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}

	private void OnDisable()
	{
		RequiredItem_Cell[] componentsInChildren = hunterRequiredItemListtr.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Grow", requiredItem_Cell.transform);
		}
		NotEnouchCoin.CallBuyCoin = null;
	}
}
