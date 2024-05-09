using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroView : LobbyPopupBase
{
	public Action OnBackEvent;

	[FormerlySerializedAs("hunterCharactertr")] [SerializeField]
	private Transform _hunterCharactertr;
	
	private HeroColor _hunterCharacter;
	private HunterInfo _hunterInfo;

	[FormerlySerializedAs("hunter_Title")] [SerializeField]
	private Text _hunterTitle;

	[FormerlySerializedAs("hunter_Level")] [SerializeField]
	private Text _hunterLevel;

	[FormerlySerializedAs("hunter_Name")] [SerializeField]
	private Text _hunterName;

	[FormerlySerializedAs("hunter_HP")] [SerializeField]
	private Text _hunterHp;

	[FormerlySerializedAs("hunter_Attack")] [SerializeField]
	private Text _heroAttack;

	[FormerlySerializedAs("hunter_Recovery")] [SerializeField]
	private Text _heroRecovery;

	[FormerlySerializedAs("hunter_Skill")] [SerializeField]
	private Text _heroHunterSkill;

	[FormerlySerializedAs("hunter_Skill_Explain")] [SerializeField]
	private Text _heroHunterSkillExplain;

	[FormerlySerializedAs("hunter_LeaderSkill")] [SerializeField]
	private Text _heroHunterLeaderSkill;

	[FormerlySerializedAs("hunter_LeaderSkill_Explain")] [SerializeField]
	private Text _heroHunterLeaderSkillExplain;

	[FormerlySerializedAs("hunter_Enchant")] [SerializeField]
	private Text _heroEnchant;

	[FormerlySerializedAs("hunter_HunterGetText")] [SerializeField]
	private Text _heroHunterGetText;

	[FormerlySerializedAs("Promotion_BT")] [SerializeField]
	private Transform _promotionBt;

	[FormerlySerializedAs("Promotion_BT_Notice")] [SerializeField]
	private Transform _promotionBtNotice;

	[FormerlySerializedAs("Promotion_Only_BT")] [SerializeField]
	private Transform _promotionOnlyBt;

	[FormerlySerializedAs("Promotion_Only_BT_Notice")] [SerializeField]
	private Transform _promotionOnlyBtNotice;

	[FormerlySerializedAs("LevelUp_BT")] [SerializeField]
	private Transform _levelUpBt;

	[FormerlySerializedAs("LevelUp_BT_Notice")] [SerializeField]
	private Transform _levelUpBtNotice;

	[FormerlySerializedAs("LevelUp_Only_BT")] [SerializeField]
	private Transform _levelUpOnlyBt;

	[FormerlySerializedAs("LevelUp_Only_BT_Notice")] [SerializeField]
	private Transform _levelUpOnlyBtNotice;

	[FormerlySerializedAs("Full_Upgrade")] [SerializeField]
	private Transform _fullUpgrade;

	[FormerlySerializedAs("Not_Have")] [SerializeField]
	private Transform _notHave;

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
	
	private bool _isOwn;

	public Transform HunterTransform => _hunterCharacter.transform;

	public bool IsHunterNull()
	{
		if (_hunterCharacter != null)
		{
			return true;
		}
		return false;
	}

	public void ShowView(HunterInfo _hunterInfo, bool _isOwn)
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		Configure(_hunterInfo, _isOwn);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Complete()
	{
	}

	public void SetConfigure(HunterInfo _hunterInfo, bool _isOwn)
	{
		Configure(_hunterInfo, _isOwn);
		_hunterCharacter.gameObject.SetActive(value: true);
	}

	private void Configure(HunterInfo _hunterInfo, bool _isOwn)
	{
		this._hunterInfo = _hunterInfo;
		this._isOwn = _isOwn;
		if (_hunterCharacter != null)
		{
			MasterPoolManager.ReturnToPool("Hunter", _hunterCharacter.transform);
			_hunterCharacter = null;
		}
		switch (_hunterInfo.Hunter.color)
		{
		case 0:
			SetHunterColorIcon(0);
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_B", _hunterCharactertr).GetComponent<HeroColor>();
			break;
		case 1:
			SetHunterColorIcon(1);
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_G", _hunterCharactertr).GetComponent<HeroColor>();
			break;
		case 2:
			SetHunterColorIcon(2);
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_P", _hunterCharactertr).GetComponent<HeroColor>();
			break;
		case 3:
			SetHunterColorIcon(3);
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_R", _hunterCharactertr).GetComponent<HeroColor>();
			break;
		case 4:
			SetHunterColorIcon(4);
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_Y", _hunterCharactertr).GetComponent<HeroColor>();
			break;
		}
		_hunterCharacter.transform.SetAsFirstSibling();
		_hunterCharacter.transform.localPosition = new Vector3(-155f, 0f, 0f);
		_hunterCharacter.transform.localScale = Vector3.one;
		_hunterCharacter.Construct(_hunterInfo);
		_hunterTitle.text = MasterLocalize.GetData(GameDataManager.GetHunterTribeName(_hunterInfo.Hunter.hunterTribe)) + " / " + MasterLocalize.GetData(_hunterInfo.Hunter.hunterClass);
		_hunterLevel.text = MasterLocalize.GetData("common_text_level") + _hunterInfo.Stat.hunterLevel + " / " + _hunterInfo.Stat.hunterTier * 20;
		_hunterName.text = MasterLocalize.GetData(_hunterInfo.Hunter.hunterName);
		if (this._isOwn)
		{
			UnityEngine.Debug.Log("&&&&&&&&&&&&&&&&&&&&&&& check 11 = " + _hunterInfo.Hunter.hunterIdx);
			UnityEngine.Debug.Log("&&&&&&&&&&&&&&&&&&&&&&& check 22 = " + GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx));
			_hunterHp.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(_hunterInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx)));
			_heroAttack.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(_hunterInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx)));
			_heroRecovery.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(_hunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx)));
			_heroEnchant.text = "x" + GameDataManager.HasUserHunterEnchant(_hunterInfo.Hunter.hunterIdx);
		}
		else
		{
			_hunterHp.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHP(_hunterInfo.Stat.hunterHp, 1));
			_heroAttack.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceAttack(_hunterInfo.Stat.hunterAttack, 1));
			_heroRecovery.text = GameUtil.InsertCommaInt((int)GameUtil.GetHunterReinForceHeal(_hunterInfo.Stat.hunterRecovery, 1));
			_heroEnchant.text = "x1";
		}
		_heroHunterSkill.text = MasterLocalize.GetData(_hunterInfo.Skill.skillName);
		_heroHunterSkillExplain.text = string.Format(MasterLocalize.GetData("Hunter_skill_text_" + _hunterInfo.Skill.skillIdx), _hunterInfo.Skill.multiple);
		if (_hunterInfo.Stat.hunterLeaderSkill == 0)
		{
			_heroHunterLeaderSkillExplain.text = MasterLocalize.GetData("Popup_hunter_leaderskill_02");
		}
		else
		{
			_heroHunterLeaderSkillExplain.text = MasterLocalize.GetData(GameDataManager.GetHunterLeaderSkillData(_hunterInfo.Stat.hunterLeaderSkill).leaderSkillDescription);
		}
		SettingsButton();
		if (GameDataManager.HasUserHunterNew(this._hunterInfo.Hunter.hunterIdx))
		{
			Protocol_Set.Protocol_hunter_is_not_new_Req(this._hunterInfo.Hunter.hunterIdx, IsUserHunterViewResponse);
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
		GameUtil.SetOwnHunterList(HanterListType.Normal);
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

	private void SettingsButton()
	{
		if (!_isOwn)
		{
			_promotionBt.gameObject.SetActive(value: false);
			_promotionOnlyBt.gameObject.SetActive(value: false);
			_levelUpBt.gameObject.SetActive(value: false);
			_levelUpOnlyBt.gameObject.SetActive(value: false);
			_fullUpgrade.gameObject.SetActive(value: false);
			_notHave.gameObject.SetActive(value: true);
			if (_hunterInfo.Hunter.hunterIdx == 20501 || _hunterInfo.Hunter.hunterIdx == 20502 || _hunterInfo.Hunter.hunterIdx == 20503 || _hunterInfo.Hunter.hunterIdx == 20504 || _hunterInfo.Hunter.hunterIdx == 20505)
			{
				_heroHunterGetText.text = string.Format(MasterLocalize.GetData("popup_hunter_text_13"));
			}
			else
			{
				_heroHunterGetText.text = string.Format(MasterLocalize.GetData("popup_hunter_text_12"));
			}
			return;
		}
		if (_hunterInfo.Stat.hunterLevel >= _hunterInfo.Hunter.maxTier * 20 && _hunterInfo.Stat.hunterTier >= _hunterInfo.Hunter.maxTier)
		{
			_promotionBt.gameObject.SetActive(value: false);
			_promotionOnlyBt.gameObject.SetActive(value: false);
			_levelUpBt.gameObject.SetActive(value: false);
			_levelUpOnlyBt.gameObject.SetActive(value: false);
			_fullUpgrade.gameObject.SetActive(value: true);
			_notHave.gameObject.SetActive(value: false);
			return;
		}
		if (_hunterInfo.Stat.hunterTier >= _hunterInfo.Hunter.maxTier)
		{
			_promotionBt.gameObject.SetActive(value: false);
			_promotionOnlyBt.gameObject.SetActive(value: false);
			_levelUpBt.gameObject.SetActive(value: false);
			_levelUpOnlyBt.gameObject.SetActive(value: true);
			_fullUpgrade.gameObject.SetActive(value: false);
			_notHave.gameObject.SetActive(value: false);
			if (CheckIsLevelUp())
			{
				_levelUpOnlyBtNotice.gameObject.SetActive(value: true);
			}
			else
			{
				_levelUpOnlyBtNotice.gameObject.SetActive(value: false);
			}
			return;
		}
		if (_hunterInfo.Stat.hunterLevel >= _hunterInfo.Stat.hunterTier * 20)
		{
			_promotionBt.gameObject.SetActive(value: false);
			_promotionOnlyBt.gameObject.SetActive(value: true);
			_levelUpBt.gameObject.SetActive(value: false);
			_levelUpOnlyBt.gameObject.SetActive(value: false);
			_fullUpgrade.gameObject.SetActive(value: false);
			_notHave.gameObject.SetActive(value: false);
			if (CheckPromotion())
			{
				_promotionOnlyBtNotice.gameObject.SetActive(value: true);
			}
			else
			{
				_promotionOnlyBtNotice.gameObject.SetActive(value: false);
			}
			return;
		}
		_promotionBt.gameObject.SetActive(value: true);
		_promotionOnlyBt.gameObject.SetActive(value: false);
		_levelUpBt.gameObject.SetActive(value: true);
		_levelUpOnlyBt.gameObject.SetActive(value: false);
		_fullUpgrade.gameObject.SetActive(value: false);
		_notHave.gameObject.SetActive(value: false);
		if (CheckIsLevelUp())
		{
			_levelUpBtNotice.gameObject.SetActive(value: true);
		}
		else
		{
			_levelUpBtNotice.gameObject.SetActive(value: false);
		}
		if (CheckPromotion())
		{
			_promotionBtNotice.gameObject.SetActive(value: true);
		}
		else
		{
			_promotionBtNotice.gameObject.SetActive(value: false);
		}
	}

	private bool CheckIsLevelUp()
	{
		bool result = false;
		HunterLevelDbData hunterLevelDbData = null;
		hunterLevelDbData = GameDataManager.GetHunterLevelData(_hunterInfo.Stat.hunterIdx, _hunterInfo.Stat.hunterLevel, _hunterInfo.Stat.hunterTier);
		if (CheckEneoughLevelUpItem(hunterLevelDbData.hnil, hunterLevelDbData.hnil_N) && hunterLevelDbData.needCoin <= GameInfo.userData.userInfo.coin && _hunterInfo.Stat.hunterTier * 20 > _hunterInfo.Stat.hunterLevel)
		{
			result = true;
		}
		return result;
	}

	private bool CheckPromotion()
	{
		bool result = false;
		HunterPromotionDbData hunterPromotionDbData = null;
		hunterPromotionDbData = GameDataManager.GetHunterPromotionData(_hunterInfo.Hunter.color, _hunterInfo.Hunter.maxTier, _hunterInfo.Stat.hunterTier);
		if (CheckEneoughPromotionItem(hunterPromotionDbData.hnip1, hunterPromotionDbData.hnip1_N) && hunterPromotionDbData.needCoin <= GameInfo.userData.userInfo.coin && _hunterInfo.Stat.hunterTier * 20 == _hunterInfo.Stat.hunterLevel && CheckEneoughPromotionItem(hunterPromotionDbData.hnip2, hunterPromotionDbData.hnip2_N) && CheckEneoughPromotionItem(hunterPromotionDbData.hnip3, hunterPromotionDbData.hnip3_N) && CheckEneoughPromotionItem(hunterPromotionDbData.hnip4, hunterPromotionDbData.hnip4_N))
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
		if (OnBackEvent != null)
		{
			OnBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		LobbyManager.ShowHunterList();
		LobbyManager.ShowLevelPlay();
		LobbyManager.ShowArenaLevelPlay();
		LobbyManager.ShowQuickLoot();
	}

	public void ShowHunterLevel()
	{
		_hunterCharacter.gameObject.SetActive(value: false);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		LobbyManager.ShowHunterLevel(_hunterInfo, _isSpawn: true);
	}

	public void ShowHunterPromotion()
	{
		_hunterCharacter.gameObject.SetActive(value: false);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		LobbyManager.ShowHunterPromotion(_hunterInfo, _isSpawn: true);
	}
}
