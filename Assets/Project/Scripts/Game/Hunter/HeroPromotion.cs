using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroPromotion : LobbyPopupBase
{
	public Action OnBackEvent;

	[SerializeField] private Image promotion_Btn_Lock;

	[SerializeField] private Transform hunterCharactertr;

	[SerializeField] private HeroColor hunter_Character;

	[SerializeField] private Transform hunterTiertr;

	[SerializeField] private Transform hunterRequiredItemListtr;

	[SerializeField] private Text hunter_Level_Origin;

	[SerializeField] private Text hunter_Level_After;

	private RequiredItem_Cell _levelCell;
	private RequiredItem_Cell _requiredItemCell1;
	private RequiredItem_Cell _requiredItemCell2;
	private RequiredItem_Cell _requiredItemCell3;
	private RequiredItem_Cell _requiredItemCell4;
	private RequiredItem_Cell _requiredCoinCell;
	private HunterInfo _hunterInfo;
	private HunterPromotionDbData _hunterPromotionDb;
	private bool _isPromotionCondition1;
	private bool _isPromotionCondition2;
	private bool _isPromotionCondition3;
	private bool _isPromotionConditionTotal;

	public HunterInfo HunterInfo => _hunterInfo;

	public Transform HunterTransform => hunter_Character.transform;

	public bool HunterCheckNull()
	{
		if (hunter_Character != null)
		{
			return true;
		}

		return false;
	}

	public void ShowPanel(HunterInfo _hunterInfo)
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

	public void Click_Promotion()
	{
		if (!_isPromotionConditionTotal)
		{
			UnityEngine.Debug.Log("Promotion Condition Fail!");
			return;
		}

		UnityEngine.Debug.Log("Promotion Condition Success ! = " + (_hunterInfo.Stat.hunterTier + 1));
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		_hunterInfo = GameDataManager.GetHunterInfo(_hunterInfo.Hunter.hunterIdx, _hunterInfo.Stat.hunterLevel,
			_hunterInfo.Stat.hunterTier + 1);
		Protocol_Set.Protocol_hunter_promotion_Req(_hunterInfo.Hunter.hunterIdx, OnResponce);
	}

	private void Construct(HunterInfo _hunterInfo)
	{
		RequiredItem_Cell[] componentsInChildren =
			hunterRequiredItemListtr.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}

		_isPromotionCondition1 = true;
		_isPromotionCondition2 = true;
		_isPromotionCondition3 = true;
		_isPromotionConditionTotal = true;
		this._hunterInfo = _hunterInfo;
		UnityEngine.Debug.Log("************** HunterPromotion Data  = " + this._hunterInfo.Hunter.skillIdx);
		_hunterPromotionDb = GameDataManager.GetHunterPromotionData(this._hunterInfo.Hunter.color,
			this._hunterInfo.Hunter.maxTier, this._hunterInfo.Stat.hunterTier);
		if (hunter_Character != null)
		{
			MasterPoolManager.ReturnToPool("Hunter", hunter_Character.transform);
			hunter_Character = null;
		}

		switch (_hunterInfo.Hunter.color)
		{
			case 0:
				hunter_Character = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_B", hunterCharactertr)
					.GetComponent<HeroColor>();
				break;
			case 1:
				hunter_Character = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_G", hunterCharactertr)
					.GetComponent<HeroColor>();
				break;
			case 2:
				hunter_Character = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_P", hunterCharactertr)
					.GetComponent<HeroColor>();
				break;
			case 3:
				hunter_Character = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_R", hunterCharactertr)
					.GetComponent<HeroColor>();
				break;
			case 4:
				hunter_Character = MasterPoolManager.SpawnObject("Hunter", "HunterPhotoBg_Y", hunterCharactertr)
					.GetComponent<HeroColor>();
				break;
		}

		hunter_Character.transform.SetAsFirstSibling();
		hunter_Character.transform.localPosition = new Vector3(0f, 70f, 0f);
		hunter_Character.transform.localScale = new Vector3(1f, 1f, 1f);
		hunter_Character.Construct(_hunterInfo);
		hunter_Level_Origin.text = string.Format(MasterLocalize.GetData("common_text_max_level"),
			(this._hunterInfo.Stat.hunterTier * 20).ToString());
		hunter_Level_After.text = ((this._hunterInfo.Stat.hunterTier + 1) * 20).ToString();
		SetStars();
		SetNeededItems();
		if (this._hunterInfo.Stat.hunterLevel >= this._hunterInfo.Stat.hunterTier * 20)
		{
			_isPromotionCondition3 = true;
		}
		else
		{
			_isPromotionCondition3 = false;
		}

		NotEnouchCoin.CallBuyCoin = CoinBuyEvent;
	}

	private void OnResponce()
	{
		hunter_Character.gameObject.SetActive(value: false);
		LobbyManager.ShowHunterPromotionUp(_hunterInfo, _isSpawn: true);
	}

	private void SetStars()
	{
		for (int i = 0; i < hunterTiertr.childCount; i++)
		{
			hunterTiertr.GetChild(i).GetChild(0).GetChild(0)
				.gameObject.SetActive(value: false);
			hunterTiertr.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
			hunterTiertr.GetChild(i).gameObject.SetActive(value: false);
		}

		for (int j = 0; j < _hunterInfo.Hunter.maxTier; j++)
		{
			hunterTiertr.GetChild(j).gameObject.SetActive(value: true);
			if (_hunterInfo.Stat.hunterTier >= j + 1)
			{
				hunterTiertr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
			}
			else if (_hunterInfo.Stat.hunterTier + 1 >= j + 1)
			{
				hunterTiertr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
				hunterTiertr.GetChild(j).GetChild(0).GetChild(0)
					.gameObject.SetActive(value: true);
			}
		}
	}

	private void SetNeededItems()
	{
		if (_hunterPromotionDb.hnip1 != 0)
		{
			_requiredItemCell1 = SetRequiredItemSet(1);
		}

		if (_hunterPromotionDb.hnip2 != 0)
		{
			_requiredItemCell2 = SetRequiredItemSet(2);
		}

		if (_hunterPromotionDb.hnip3 != 0)
		{
			_requiredItemCell3 = SetRequiredItemSet(3);
		}

		if (_hunterPromotionDb.hnip4 != 0)
		{
			_requiredItemCell4 = SetRequiredItemSet(4);
		}

		_requiredCoinCell = MasterPoolManager.SpawnObject("Grow", "cell_coin", hunterRequiredItemListtr)
			.GetComponent<RequiredItem_Cell>();
		_requiredCoinCell.transform.localScale = Vector3.one;
		_requiredCoinCell.transform.SetAsLastSibling();
		_requiredCoinCell.SetCostText(SetCoinsText());
		if (GameInfo.userData.userInfo.coin < _hunterPromotionDb.needCoin)
		{
			_requiredCoinCell.SetClickType(LootClickType.Coin,
				_hunterPromotionDb.needCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			_requiredCoinCell.SetClickType(LootClickType.None);
		}

		_levelCell = MasterPoolManager.SpawnObject("Grow", "cell_badge", hunterRequiredItemListtr)
			.GetComponent<RequiredItem_Cell>();
		_levelCell.transform.localScale = Vector3.one;
		_levelCell.transform.SetAsFirstSibling();
		_levelCell.SetItemImg(50040);
		_levelCell.SetCostText(SetCostText());
	}

	private RequiredItem_Cell SetRequiredItemSet(int _idx)
	{
		RequiredItem_Cell requiredItem_Cell = null;
		requiredItem_Cell = MasterPoolManager.SpawnObject("Grow", "cell_badge", hunterRequiredItemListtr)
			.GetComponent<RequiredItem_Cell>();
		requiredItem_Cell.transform.localScale = Vector3.one;
		requiredItem_Cell.transform.SetAsLastSibling();
		switch (_idx)
		{
			case 1:
				requiredItem_Cell.SetItemImg(_hunterPromotionDb.hnip1);
				requiredItem_Cell.SetCostText(SetCostTextBadge(1));
				break;
			case 2:
				requiredItem_Cell.SetItemImg(_hunterPromotionDb.hnip2);
				requiredItem_Cell.SetCostText(SetCostTextBadge(2));
				break;
			case 3:
				requiredItem_Cell.SetItemImg(_hunterPromotionDb.hnip3);
				requiredItem_Cell.SetCostText(SetCostTextBadge(3));
				break;
			case 4:
				requiredItem_Cell.SetItemImg(_hunterPromotionDb.hnip4);
				requiredItem_Cell.SetCostText(SetCostTextBadge(4));
				break;
		}

		return requiredItem_Cell;
	}

	private string SetCostTextBadge(int _idx)
	{
		string result = string.Empty;
		bool flag = true;
		int num = 0;
		int num2 = 0;
		switch (_idx)
		{
			case 1:
				num = _hunterPromotionDb.hnip1;
				num2 = _hunterPromotionDb.hnip1_N;
				break;
			case 2:
				num = _hunterPromotionDb.hnip2;
				num2 = _hunterPromotionDb.hnip2_N;
				break;
			case 3:
				num = _hunterPromotionDb.hnip3;
				num2 = _hunterPromotionDb.hnip3_N;
				break;
			case 4:
				num = _hunterPromotionDb.hnip4;
				num2 = _hunterPromotionDb.hnip4_N;
				break;
		}

		for (int i = 0; i < GameInfo.userData.userItemList.Length; i++)
		{
			if (GameInfo.userData.userItemList[i].itemIdx == num)
			{
				flag = false;
				if (GameInfo.userData.userItemList[i].count >= num2)
				{
					result = "<color=#ffffff>" + GameInfo.userData.userItemList[i].count + "</color>/" + num2;
					continue;
				}

				_isPromotionCondition1 = false;
				result = "<color=#ff0000>" + GameInfo.userData.userItemList[i].count + "</color>/" + num2;
			}
		}

		if (flag)
		{
			_isPromotionCondition1 = false;
			result = "<color=#ff0000>0</color>/" + num2;
		}

		CheckPromotion();
		return result;
	}

	private string SetCoinsText()
	{
		string empty = string.Empty;
		if (GameInfo.userData.userInfo.coin >= _hunterPromotionDb.needCoin)
		{
			empty = "<color=#ffffff>" + GameInfo.userData.userInfo.coin + "</color>/" + _hunterPromotionDb.needCoin;
		}
		else
		{
			_isPromotionCondition2 = false;
			empty = "<color=#ff0000>" + GameInfo.userData.userInfo.coin + "</color>/" + _hunterPromotionDb.needCoin;
		}

		CheckPromotion();
		return empty;
	}

	private string SetCostText()
	{
		string empty = string.Empty;
		if (_hunterInfo.Stat.hunterLevel >= _hunterInfo.Stat.hunterTier * 20)
		{
			empty = "LV.<color=#ffffff>" + _hunterInfo.Stat.hunterLevel + "</color>/" +
			        _hunterInfo.Stat.hunterTier * 20;
		}
		else
		{
			_isPromotionCondition3 = false;
			empty = "LV.<color=#ff0000>" + _hunterInfo.Stat.hunterLevel + "</color>/" +
			        _hunterInfo.Stat.hunterTier * 20;
		}

		CheckPromotion();
		return empty;
	}

	private void CheckPromotion()
	{
		if (_isPromotionCondition1 && _isPromotionCondition2 && _isPromotionCondition3)
		{
			_isPromotionConditionTotal = true;
			promotion_Btn_Lock.gameObject.SetActive(value: false);
		}
		else
		{
			_isPromotionConditionTotal = false;
			promotion_Btn_Lock.gameObject.SetActive(value: true);
		}
	}

	private void CoinBuyEvent(int _needJewel)
	{
		Protocol_Set.Protocol_shop_popup_hunter_promotion_buy_coin_Req(_hunterInfo.Hunter.hunterIdx, _needJewel,
			OnBuyCoinComplete);
	}

	private void OnBuyCoinComplete()
	{
		_requiredCoinCell.SetCostText(SetCoinsText());
		if (GameInfo.userData.userInfo.coin < _hunterPromotionDb.needCoin)
		{
			_requiredCoinCell.SetClickType(LootClickType.Coin,
				_hunterPromotionDb.needCoin - GameInfo.userData.userInfo.coin);
		}
		else
		{
			_requiredCoinCell.SetClickType(LootClickType.None);
		}

		if (GameInfo.userData.userInfo.coin > _hunterPromotionDb.needCoin)
		{
			_isPromotionCondition2 = true;
			CheckPromotion();
		}
	}

	public void OnClickGoBack()
	{
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
		LobbyManager.ShowHunterView(_hunterInfo, _isSpawn: false);
		if (OnBackEvent != null)
		{
			OnBackEvent();
		}
	}

	private void OnDisable()
	{
		RequiredItem_Cell[] componentsInChildren =
			hunterRequiredItemListtr.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MasterPoolManager.ReturnToPool("Grow", requiredItem_Cell.transform);
		}
	}
}
