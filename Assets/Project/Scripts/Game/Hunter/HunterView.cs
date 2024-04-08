using System;
using UnityEngine;
using UnityEngine.UI;

public class HunterView : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform hunterCharactertr;

	[SerializeField]
	private HunterView_Color hunter_Character;

	[SerializeField]
	private HunterInfo hunterInfo;

	[SerializeField]
	private Text hunter_Title;

	[SerializeField]
	private Text hunter_Level;

	[SerializeField]
	private Text hunter_Name;

	[SerializeField]
	private Text hunter_HP;

	[SerializeField]
	private Text hunter_Attack;

	[SerializeField]
	private Text hunter_Recovery;

	[SerializeField]
	private Text hunter_Skill;

	[SerializeField]
	private Text hunter_Skill_Explain;

	[SerializeField]
	private Text hunter_LeaderSkill;

	[SerializeField]
	private Text hunter_LeaderSkill_Explain;

	[SerializeField]
	private Text hunter_Enchant;

	[SerializeField]
	private Text hunter_HunterGetText;

	[SerializeField]
	private Transform Promotion_BT;

	[SerializeField]
	private Transform Promotion_BT_Notice;

	[SerializeField]
	private Transform Promotion_Only_BT;

	[SerializeField]
	private Transform Promotion_Only_BT_Notice;

	[SerializeField]
	private Transform LevelUp_BT;

	[SerializeField]
	private Transform LevelUp_BT_Notice;

	[SerializeField]
	private Transform LevelUp_Only_BT;

	[SerializeField]
	private Transform LevelUp_Only_BT_Notice;

	[SerializeField]
	private Transform Full_Upgrade;

	[SerializeField]
	private Transform Not_Have;

	[SerializeField]
	private Transform colorRedIcon;

	[SerializeField]
	private Transform colorYellowIcon;

	[SerializeField]
	private Transform colorGreenIcon;

	[SerializeField]
	private Transform colorBlueIcon;

	[SerializeField]
	private Transform colorPurpleIcon;

	[SerializeField]
	private bool isOwn;

	public Transform HunterTransform => hunter_Character.transform;

	public bool HunterCheckNull()
	{
		if (hunter_Character != null)
		{
			return true;
		}
		return false;
	}

	public void Show(HunterInfo _hunterInfo, bool _isOwn)
	{
		base.Show();
		base.gameObject.SetActive(value: true);
		Init(_hunterInfo, _isOwn);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
	}

	public void SetInit(HunterInfo _hunterInfo, bool _isOwn)
	{
		Init(_hunterInfo, _isOwn);
		hunter_Character.gameObject.SetActive(value: true);
	}

	private void Init(HunterInfo _hunterInfo, bool _isOwn)
	{
		hunterInfo = _hunterInfo;
		isOwn = _isOwn;
		if (hunter_Character != null)
		{
			MWPoolManager.DeSpawn("Hunter", hunter_Character.transform);
			hunter_Character = null;
		}
		switch (_hunterInfo.Hunter.color)
		{
		case 0:
			SetHunterColorIcon(0);
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_B", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		case 1:
			SetHunterColorIcon(1);
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_G", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		case 2:
			SetHunterColorIcon(2);
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_P", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		case 3:
			SetHunterColorIcon(3);
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_R", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		case 4:
			SetHunterColorIcon(4);
			hunter_Character = MWPoolManager.Spawn("Hunter", "HunterPhotoBg_Y", hunterCharactertr).GetComponent<HunterView_Color>();
			break;
		}
		hunter_Character.transform.SetAsFirstSibling();
		hunter_Character.transform.localPosition = new Vector3(-155f, 0f, 0f);
		hunter_Character.transform.localScale = Vector3.one;
		hunter_Character.Init(_hunterInfo);
		hunter_Title.text = MWLocalize.GetData(GameDataManager.GetHunterTribeName(_hunterInfo.Hunter.hunterTribe)) + " / " + MWLocalize.GetData(_hunterInfo.Hunter.hunterClass);
		hunter_Level.text = MWLocalize.GetData("common_text_level") + _hunterInfo.Stat.hunterLevel + " / " + _hunterInfo.Stat.hunterTier * 20;
		hunter_Name.text = MWLocalize.GetData(_hunterInfo.Hunter.hunterName);
		if (isOwn)
		{
			UnityEngine.Debug.Log("&&&&&&&&&&&&&&&&&&&&&&& check 11 = " + _hunterInfo.Hunter.hunterIdx);
			UnityEngine.Debug.Log("&&&&&&&&&&&&&&&&&&&&&&& check 22 = " + GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx));
			hunter_HP.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(_hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx)));
			hunter_Attack.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(_hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx)));
			hunter_Recovery.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(_hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx)));
			hunter_Enchant.text = "x" + GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx);
		}
		else
		{
			hunter_HP.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(_hunterInfo.Stat.hunterHp, 1));
			hunter_Attack.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(_hunterInfo.Stat.hunterAttack, 1));
			hunter_Recovery.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(_hunterInfo.Stat.hunterRecovery, 1));
			hunter_Enchant.text = "x1";
		}
		hunter_Skill.text = MWLocalize.GetData(_hunterInfo.Skill.skillName);
		hunter_Skill_Explain.text = string.Format(MWLocalize.GetData("Hunter_skill_text_" + _hunterInfo.Skill.skillIdx), _hunterInfo.Skill.multiple);
		if (_hunterInfo.Stat.hunterLeaderSkill == 0)
		{
			hunter_LeaderSkill_Explain.text = MWLocalize.GetData("Popup_hunter_leaderskill_02");
		}
		else
		{
			hunter_LeaderSkill_Explain.text = MWLocalize.GetData(GameDataManager.GetHunterLeaderSkillData(_hunterInfo.Stat.hunterLeaderSkill).leaderSkillDescription);
		}
		Button_Setting();
		if (GameDataManager.HasUserHunterNew(hunterInfo.Hunter.hunterIdx))
		{
			Protocol_Set.Protocol_hunter_is_not_new_Req(hunterInfo.Hunter.hunterIdx, IsUserHunterViewResponse);
		}
		UnityEngine.Debug.Log("11111111111111111");
		if (LobbyManager.OpenChestOpenEnchant != null)
		{
			LobbyManager.OpenChestOpenEnchant();
		}
	}

	private void IsUserHunterViewResponse()
	{
		GameUtil.SetUseHunterList();
		GameUtil.SetOwnHunterList(HUNTERLIST_TYPE.NORMAL);
	}

	private void SetHunterColorIcon(int _type)
	{
		colorRedIcon.gameObject.SetActive(value: false);
		colorGreenIcon.gameObject.SetActive(value: false);
		colorBlueIcon.gameObject.SetActive(value: false);
		colorYellowIcon.gameObject.SetActive(value: false);
		colorPurpleIcon.gameObject.SetActive(value: false);
		switch (_type)
		{
		case 3:
			colorRedIcon.gameObject.SetActive(value: true);
			break;
		case 1:
			colorGreenIcon.gameObject.SetActive(value: true);
			break;
		case 0:
			colorBlueIcon.gameObject.SetActive(value: true);
			break;
		case 4:
			colorYellowIcon.gameObject.SetActive(value: true);
			break;
		case 2:
			colorPurpleIcon.gameObject.SetActive(value: true);
			break;
		}
	}

	private void Button_Setting()
	{
		if (!isOwn)
		{
			Promotion_BT.gameObject.SetActive(value: false);
			Promotion_Only_BT.gameObject.SetActive(value: false);
			LevelUp_BT.gameObject.SetActive(value: false);
			LevelUp_Only_BT.gameObject.SetActive(value: false);
			Full_Upgrade.gameObject.SetActive(value: false);
			Not_Have.gameObject.SetActive(value: true);
			if (hunterInfo.Hunter.hunterIdx == 20501 || hunterInfo.Hunter.hunterIdx == 20502 || hunterInfo.Hunter.hunterIdx == 20503 || hunterInfo.Hunter.hunterIdx == 20504 || hunterInfo.Hunter.hunterIdx == 20505)
			{
				hunter_HunterGetText.text = string.Format(MWLocalize.GetData("popup_hunter_text_13"));
			}
			else
			{
				hunter_HunterGetText.text = string.Format(MWLocalize.GetData("popup_hunter_text_12"));
			}
			return;
		}
		if (hunterInfo.Stat.hunterLevel >= hunterInfo.Hunter.maxTier * 20 && hunterInfo.Stat.hunterTier >= hunterInfo.Hunter.maxTier)
		{
			Promotion_BT.gameObject.SetActive(value: false);
			Promotion_Only_BT.gameObject.SetActive(value: false);
			LevelUp_BT.gameObject.SetActive(value: false);
			LevelUp_Only_BT.gameObject.SetActive(value: false);
			Full_Upgrade.gameObject.SetActive(value: true);
			Not_Have.gameObject.SetActive(value: false);
			return;
		}
		if (hunterInfo.Stat.hunterTier >= hunterInfo.Hunter.maxTier)
		{
			Promotion_BT.gameObject.SetActive(value: false);
			Promotion_Only_BT.gameObject.SetActive(value: false);
			LevelUp_BT.gameObject.SetActive(value: false);
			LevelUp_Only_BT.gameObject.SetActive(value: true);
			Full_Upgrade.gameObject.SetActive(value: false);
			Not_Have.gameObject.SetActive(value: false);
			if (CheckHunterLevelUp())
			{
				LevelUp_Only_BT_Notice.gameObject.SetActive(value: true);
			}
			else
			{
				LevelUp_Only_BT_Notice.gameObject.SetActive(value: false);
			}
			return;
		}
		if (hunterInfo.Stat.hunterLevel >= hunterInfo.Stat.hunterTier * 20)
		{
			Promotion_BT.gameObject.SetActive(value: false);
			Promotion_Only_BT.gameObject.SetActive(value: true);
			LevelUp_BT.gameObject.SetActive(value: false);
			LevelUp_Only_BT.gameObject.SetActive(value: false);
			Full_Upgrade.gameObject.SetActive(value: false);
			Not_Have.gameObject.SetActive(value: false);
			if (CheckHunterPromotion())
			{
				Promotion_Only_BT_Notice.gameObject.SetActive(value: true);
			}
			else
			{
				Promotion_Only_BT_Notice.gameObject.SetActive(value: false);
			}
			return;
		}
		Promotion_BT.gameObject.SetActive(value: true);
		Promotion_Only_BT.gameObject.SetActive(value: false);
		LevelUp_BT.gameObject.SetActive(value: true);
		LevelUp_Only_BT.gameObject.SetActive(value: false);
		Full_Upgrade.gameObject.SetActive(value: false);
		Not_Have.gameObject.SetActive(value: false);
		if (CheckHunterLevelUp())
		{
			LevelUp_BT_Notice.gameObject.SetActive(value: true);
		}
		else
		{
			LevelUp_BT_Notice.gameObject.SetActive(value: false);
		}
		if (CheckHunterPromotion())
		{
			Promotion_BT_Notice.gameObject.SetActive(value: true);
		}
		else
		{
			Promotion_BT_Notice.gameObject.SetActive(value: false);
		}
	}

	private bool CheckHunterLevelUp()
	{
		bool result = false;
		HunterLevelDbData hunterLevelDbData = null;
		hunterLevelDbData = GameDataManager.GetHunterLevelData(hunterInfo.Stat.hunterIdx, hunterInfo.Stat.hunterLevel, hunterInfo.Stat.hunterTier);
		if (CheckEneoughLevelUpItem(hunterLevelDbData.hnil, hunterLevelDbData.hnil_N) && hunterLevelDbData.needCoin <= GameInfo.userData.userInfo.coin && hunterInfo.Stat.hunterTier * 20 > hunterInfo.Stat.hunterLevel)
		{
			result = true;
		}
		return result;
	}

	private bool CheckHunterPromotion()
	{
		bool result = false;
		HunterPromotionDbData hunterPromotionDbData = null;
		hunterPromotionDbData = GameDataManager.GetHunterPromotionData(hunterInfo.Hunter.color, hunterInfo.Hunter.maxTier, hunterInfo.Stat.hunterTier);
		if (CheckEneoughPromotionItem(hunterPromotionDbData.hnip1, hunterPromotionDbData.hnip1_N) && hunterPromotionDbData.needCoin <= GameInfo.userData.userInfo.coin && hunterInfo.Stat.hunterTier * 20 == hunterInfo.Stat.hunterLevel && CheckEneoughPromotionItem(hunterPromotionDbData.hnip2, hunterPromotionDbData.hnip2_N) && CheckEneoughPromotionItem(hunterPromotionDbData.hnip3, hunterPromotionDbData.hnip3_N) && CheckEneoughPromotionItem(hunterPromotionDbData.hnip4, hunterPromotionDbData.hnip4_N))
		{
			result = true;
		}
		return result;
	}

	private bool CheckEneoughLevelUpItem(int _itemIdx, int _itemCount)
	{
		bool result = false;
		if (_itemCount <= GameInfo.userData.GetItemCount(_itemIdx))
		{
			result = true;
		}
		return result;
	}

	private bool CheckEneoughPromotionItem(int _itemIdx, int _itemCount)
	{
		bool result = false;
		if (_itemIdx == 0)
		{
			result = true;
		}
		else if (_itemCount <= GameInfo.userData.GetItemCount(_itemIdx))
		{
			result = true;
		}
		return result;
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		LobbyManager.ShowHunterList();
		LobbyManager.ShowLevelPlay();
		LobbyManager.ShowArenaLevelPlay();
		LobbyManager.ShowQuickLoot();
	}

	public void ShowHunterLevel()
	{
		hunter_Character.gameObject.SetActive(value: false);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		LobbyManager.ShowHunterLevel(hunterInfo, _isSpawn: true);
	}

	public void ShowHunterPromotion()
	{
		hunter_Character.gameObject.SetActive(value: false);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		LobbyManager.ShowHunterPromotion(hunterInfo, _isSpawn: true);
	}
}
