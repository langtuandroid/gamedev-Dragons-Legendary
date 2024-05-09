using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroLevel : LobbyPopupBase
{
	public Action OnGoBack;

	[FormerlySerializedAs("hunterCharactertr")] [SerializeField]
	private Transform _heroCharactertr;
	
	[FormerlySerializedAs("hunterRequiredItemListtr")] [SerializeField]
	private Transform _heroRequiredItemListtr;

	[FormerlySerializedAs("hunter_Level_Origin")] [SerializeField]
	private Text _heroLevelOrigin;

	[FormerlySerializedAs("hunter_Level_After")] [SerializeField]
	private Text _heroLevelAfter;

	[FormerlySerializedAs("hunter_HP_Origin")] [SerializeField]
	private Text _heroHpOrigin;

	[FormerlySerializedAs("hunter_HP_After")] [SerializeField]
	private Text _hero_HP_After;

	[FormerlySerializedAs("hunter_Attack_Origin")] [SerializeField]
	private Text _heroAttackOrigin;

	[FormerlySerializedAs("hunter_Attack_After")] [SerializeField]
	private Text _hunterAttackAfter;

	[FormerlySerializedAs("hunter_Recovery_Origin")] [SerializeField]
	private Text _heroRecoveryOrigin;

	[FormerlySerializedAs("hunter_Recovery_After")] [SerializeField]
	private Text _heroRecoveryAfter;

	[FormerlySerializedAs("levelUp_Btn_Text")] [SerializeField]
	private Text _levelUpBtnText;
	
	[FormerlySerializedAs("levelUp_Btn_Lock")] [SerializeField]
	private Image _levelUpBtnLock;
	
	private HeroColor _hunterCharacter;
	private int _currentLevel;
	private int _targetLevel;
	private int _maxLevel;
	private int _currentHp;
	private int _targetHp;
	private int _currentAttack;
	private int _targetAttack;
	private int _currentRecovery;
	private int _targetRecovery;
	private bool _isLevelUpCondition1;
	private bool _isLevelUpCondition2;
	private bool _isLevelUpConditionTotal;
	private int _totalToken;
	private int _totalCoin;
	private HunterInfo _hunterInfoOrigin;
	private HunterInfo _hunterInfoAfter;
	private RequiredItem_Cell _requiredItemCell;
	private RequiredItem_Cell _requiredCoinCell;
	private HunterLevelDbData _hunterLevelDbData;
	
	public HunterInfo HunterInfo => _hunterInfoOrigin;

	public Transform HunterTransform => _hunterCharacter.transform;

	public bool CheckNull()
	{
		if (_hunterCharacter != null)
		{
			return true;
		}
		return false;
	}

	public void ShowHnter(HunterInfo _hunterInfo)
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		Construct(_hunterInfo);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	public void SetConfigure(HunterInfo _hunterInfo)
	{
		Construct(_hunterInfo);
	}

	public void TargetLevel_Up()
	{
		if (_targetLevel + 1 <= _hunterInfoOrigin.Stat.hunterTier * 20)
		{
			_targetLevel++;
			SetData();
			_requiredItemCell.SetCostText(SetCostTextToken());
			_requiredCoinCell.SetCostText(SetCostTextCoin());
			if (GameInfo.userData.userInfo.coin < _totalCoin)
			{
				_requiredCoinCell.SetClickType(LootClickType.Coin, _totalCoin - GameInfo.userData.userInfo.coin);
			}
			else
			{
				_requiredCoinCell.SetClickType(LootClickType.None);
			}
		}
	}

	public void TargetLevel_Down()
	{
		if (_currentLevel + 1 < _targetLevel)
		{
			_targetLevel--;
			SetData();
			_requiredItemCell.SetCostText(SetCostTextToken());
			_requiredCoinCell.SetCostText(SetCostTextCoin());
			if (GameInfo.userData.userInfo.coin < _totalCoin)
			{
				_requiredCoinCell.SetClickType(LootClickType.Coin, _totalCoin - GameInfo.userData.userInfo.coin);
			}
			else
			{
				_requiredCoinCell.SetClickType(LootClickType.None);
			}
		}
	}

	public void Click_Level_Up()
	{
		if (!_isLevelUpConditionTotal)
		{
			UnityEngine.Debug.Log("Level Up Condition Fail!");
			return;
		}
		UnityEngine.Debug.Log("target_level :: " + _targetLevel);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		if (LobbyManager.OpenHunterLevelUp != null)
		{
			Protocol_Set.Protocol_hunter_level_up_Req(_hunterInfoOrigin.Hunter.hunterIdx, _targetLevel, LevelUpResponse, isTutorial: true);
		}
		else
		{
			Protocol_Set.Protocol_hunter_level_up_Req(_hunterInfoOrigin.Hunter.hunterIdx, _targetLevel, LevelUpResponse);
		}
	}

	public void LevelUpCondition()
	{
		_isLevelUpCondition1 = true;
		_isLevelUpCondition2 = true;
		_isLevelUpConditionTotal = true;
		_levelUpBtnLock.gameObject.SetActive(value: false);
	}

	private void Construct(HunterInfo _hunterInfo)
	{
		RequiredItem_Cell[] componentsInChildren = _heroRequiredItemListtr.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}
		_isLevelUpCondition1 = true;
		_isLevelUpCondition2 = true;
		_isLevelUpConditionTotal = true;
		_totalToken = 0;
		_totalCoin = 0;
		_hunterInfoOrigin = _hunterInfo;
		_hunterLevelDbData = GameDataManager.GetHunterLevelData(_hunterInfoOrigin.Hunter.hunterIdx, _hunterInfoOrigin.Stat.hunterLevel, _hunterInfoOrigin.Stat.hunterTier);
		if (_hunterCharacter != null)
		{
			MasterPoolManager.ReturnToPool("Hunter", _hunterCharacter.transform);
			_hunterCharacter = null;
		}
		switch (_hunterInfo.Hunter.color)
		{
		case 0:
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_B", _heroCharactertr).GetComponent<HeroColor>();
			break;
		case 1:
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_G", _heroCharactertr).GetComponent<HeroColor>();
			break;
		case 2:
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_P", _heroCharactertr).GetComponent<HeroColor>();
			break;
		case 3:
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_R", _heroCharactertr).GetComponent<HeroColor>();
			break;
		case 4:
			_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_Y", _heroCharactertr).GetComponent<HeroColor>();
			break;
		}
		_hunterCharacter.transform.SetAsFirstSibling();
		_hunterCharacter.transform.localPosition = new Vector3(0f, 130f, 0f);
		_hunterCharacter.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		_hunterCharacter.Construct(_hunterInfo);
		_currentLevel = _hunterInfoOrigin.Stat.hunterLevel;
		if (CheckLevel())
		{
			_targetLevel = _currentLevel;
		}
		else
		{
			_targetLevel = _currentLevel + 1;
		}
		SetData();
		SetRequiredItem();
		NotEnouchCoin.CallBuyCoin = OnCallBuyCoinEvent;
	}

	private bool CheckLevel()
	{
		bool result = false;
		if (_hunterInfoOrigin.Hunter.maxTier * 20 == _hunterInfoOrigin.Stat.hunterLevel)
		{
			result = true;
		}
		return result;
	}

	private void SetData()
	{
		_hunterInfoAfter = GetHunterInfo();
		_heroLevelOrigin.text = MasterLocalize.GetData("common_text_level") + _currentLevel;
		if (CheckLevel())
		{
			_heroLevelAfter.text = _targetLevel.ToString() + MasterLocalize.GetData("common_text_max");
		}
		else
		{
			_heroLevelAfter.text = _targetLevel.ToString();
		}
		_currentHp = (int)GameUtil.GetHunterReinForceHP(_hunterInfoOrigin.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_hunterInfoOrigin.Hunter.hunterIdx));
		_targetHp = (int)GameUtil.GetHunterReinForceHP(_hunterInfoAfter.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_hunterInfoAfter.Hunter.hunterIdx));
		_heroHpOrigin.text = GameUtil.InsertCommaInt(_currentHp);
		_hero_HP_After.text = GameUtil.InsertCommaInt(_targetHp);
		_currentAttack = (int)GameUtil.GetHunterReinForceAttack(_hunterInfoOrigin.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_hunterInfoOrigin.Hunter.hunterIdx));
		_targetAttack = (int)GameUtil.GetHunterReinForceAttack(_hunterInfoAfter.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_hunterInfoAfter.Hunter.hunterIdx));
		_heroAttackOrigin.text = GameUtil.InsertCommaInt(_currentAttack);
		_hunterAttackAfter.text = GameUtil.InsertCommaInt(_targetAttack);
		_currentRecovery = (int)GameUtil.GetHunterReinForceHeal(_hunterInfoOrigin.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_hunterInfoOrigin.Hunter.hunterIdx));
		_targetRecovery = (int)GameUtil.GetHunterReinForceHeal(_hunterInfoAfter.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_hunterInfoAfter.Hunter.hunterIdx));
		_heroRecoveryOrigin.text = GameUtil.InsertCommaInt(_currentRecovery);
		_heroRecoveryAfter.text = GameUtil.InsertCommaInt(_targetRecovery);
		_levelUpBtnText.text = MasterLocalize.GetData("common_text_level") + " x" + (_targetLevel - _currentLevel).ToString();
	}

	private void LevelUpResponse()
	{
		_hunterCharacter.gameObject.SetActive(value: false);
		LobbyManager.ShowHunterLevelUp(_hunterInfoOrigin, _hunterInfoAfter, _isSpawn: true);
		UnityEngine.Debug.Log("Before Level = " + _hunterInfoOrigin.Stat.hunterLevel);
		UnityEngine.Debug.Log("After Level = " + _hunterInfoAfter.Stat.hunterLevel);
	}

	private void CheckLevelUp()
	{
		if (_isLevelUpCondition1 && _isLevelUpCondition2)
		{
			_isLevelUpConditionTotal = true;
			_levelUpBtnLock.gameObject.SetActive(value: false);
		}
		else
		{
			_isLevelUpConditionTotal = false;
			_levelUpBtnLock.gameObject.SetActive(value: true);
		}
	}

	private HunterInfo GetHunterInfo()
	{
		UnityEngine.Debug.Log("this.target_level = " + _targetLevel);
		return GameDataManager.GetHunterInfo(_hunterInfoOrigin.Hunter.hunterIdx, _targetLevel, _hunterInfoOrigin.Stat.hunterTier);
	}

	private void SetRequiredItem()
	{
		_requiredItemCell = MasterPoolManager.SpawnObject("Grow", "cell_token", _heroRequiredItemListtr).GetComponent<RequiredItem_Cell>();
		_requiredItemCell.transform.localScale = Vector3.one;
		_requiredItemCell.transform.SetAsFirstSibling();
		_requiredItemCell.SetItemImg(_hunterLevelDbData.hnil);
		_requiredItemCell.SetCostText(SetCostTextToken());
		_requiredCoinCell = MasterPoolManager.SpawnObject("Grow", "cell_coin", _heroRequiredItemListtr).GetComponent<RequiredItem_Cell>();
		_requiredCoinCell.transform.localScale = Vector3.one;
		_requiredCoinCell.transform.SetAsLastSibling();
		_requiredCoinCell.SetCostText(SetCostTextCoin());
		if (GameInfo.userData.userInfo.coin < _hunterLevelDbData.needCoin)
		{
			_requiredCoinCell.SetClickType(LootClickType.Coin, _hunterLevelDbData.needCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			_requiredCoinCell.SetClickType(LootClickType.None);
		}
	}

	private string SetCostTextToken()
	{
		string result = string.Empty;
		int num = 0;
		bool flag = true;
		for (int i = _currentLevel; i < _targetLevel; i++)
		{
			num += GameDataManager.GetHunterLevelData(_hunterInfoOrigin.Hunter.hunterIdx, i, _hunterInfoOrigin.Stat.hunterTier).hnil_N;
		}
		_totalToken = num;
		for (int j = 0; j < GameInfo.userData.userItemList.Length; j++)
		{
			if (GameInfo.userData.userItemList[j].itemIdx == _hunterLevelDbData.hnil)
			{
				flag = false;
				if (GameInfo.userData.userItemList[j].count >= _totalToken)
				{
					_isLevelUpCondition1 = true;
					result = "<color=#ffffff>" + GameInfo.userData.userItemList[j].count + "</color>/" + num;
				}
				else
				{
					_isLevelUpCondition1 = false;
					result = "<color=#ff0000>" + GameInfo.userData.userItemList[j].count + "</color>/" + num;
				}
			}
		}
		if (flag)
		{
			_isLevelUpCondition1 = false;
			result = "<color=#ff0000>0</color>/" + num;
		}
		CheckLevelUp();
		return result;
	}

	private string SetCostTextCoin()
	{
		string empty = string.Empty;
		int num = 0;
		for (int i = _currentLevel; i < _targetLevel; i++)
		{
			num += GameDataManager.GetHunterLevelData(_hunterInfoOrigin.Hunter.hunterIdx, i, _hunterInfoOrigin.Stat.hunterTier).needCoin;
		}
		_totalCoin = num;
		if (GameInfo.userData.userInfo.coin >= _totalCoin)
		{
			_isLevelUpCondition2 = true;
			empty = "<color=#ffffff>" + GameInfo.userData.userInfo.coin + "</color>/" + num;
		}
		else
		{
			_isLevelUpCondition2 = false;
			empty = "<color=#ff0000>" + GameInfo.userData.userInfo.coin + "</color>/" + num;
		}
		CheckLevelUp();
		return empty;
	}

	private void OnCallBuyCoinEvent(int _needJewel)
	{
		Protocol_Set.Protocol_shop_popup_hunter_buy_coin_Req(_hunterInfoOrigin.Hunter.hunterIdx, _targetLevel, _needJewel, OnBuyCoinComplete);
	}

	private void OnBuyCoinComplete()
	{
		_requiredCoinCell.SetCostText(SetCostTextCoin());
		if (GameInfo.userData.userInfo.coin < _totalCoin)
		{
			_requiredCoinCell.SetClickType(LootClickType.Coin, _totalCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			_requiredCoinCell.SetClickType(LootClickType.None);
		}
	}

	public void OnClickGoBack()
	{
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		LobbyManager.ShowHunterView(_hunterInfoOrigin, _isSpawn: false);
		if (OnGoBack != null)
		{
			OnGoBack();
		}
	}

	private void OnDisable()
	{
		RequiredItem_Cell[] componentsInChildren = _heroRequiredItemListtr.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}
		NotEnouchCoin.CallBuyCoin = null;
	}
}
