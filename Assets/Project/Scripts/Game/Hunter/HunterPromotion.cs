using System;
using UnityEngine;
using UnityEngine.UI;

public class HunterPromotion : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform hunterCharactertr;

	[SerializeField]
	private HunterView_Color hunter_Character;

	[SerializeField]
	private Transform hunterTiertr;

	[SerializeField]
	private Transform hunterRequiredItemListtr;

	[SerializeField]
	private HunterInfo hunterInfo;

	[SerializeField]
	private Text hunter_Level_Origin;

	[SerializeField]
	private Text hunter_Level_After;

	[SerializeField]
	private RequiredItem_Cell requiredLevel_Cell;

	[SerializeField]
	private RequiredItem_Cell requiredItem_Cell_1;

	[SerializeField]
	private RequiredItem_Cell requiredItem_Cell_2;

	[SerializeField]
	private RequiredItem_Cell requiredItem_Cell_3;

	[SerializeField]
	private RequiredItem_Cell requiredItem_Cell_4;

	[SerializeField]
	private RequiredItem_Cell requiredCoin_Cell;

	[SerializeField]
	private Image promotion_Btn_Lock;

	[SerializeField]
	private HunterPromotionDbData hunterPromotionDbData;

	[SerializeField]
	private bool isPromotion_Condition1;

	[SerializeField]
	private bool isPromotion_Condition2;

	[SerializeField]
	private bool isPromotion_Condition3;

	[SerializeField]
	private bool isPromotion_Condition_Total;

	public HunterInfo HunterInfo => hunterInfo;

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

	public void Click_Promotion()
	{
		if (!isPromotion_Condition_Total)
		{
			UnityEngine.Debug.Log("Promotion Condition Fail!");
			return;
		}
		UnityEngine.Debug.Log("Promotion Condition Success ! = " + (hunterInfo.Stat.hunterTier + 1));
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		hunterInfo = GameDataManager.GetHunterInfo(hunterInfo.Hunter.hunterIdx, hunterInfo.Stat.hunterLevel, hunterInfo.Stat.hunterTier + 1);
		Protocol_Set.Protocol_hunter_promotion_Req(hunterInfo.Hunter.hunterIdx, PromotionResponse);
	}

	private void Init(HunterInfo _hunterInfo)
	{
		RequiredItem_Cell[] componentsInChildren = hunterRequiredItemListtr.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Grow", requiredItem_Cell.transform);
		}
		isPromotion_Condition1 = true;
		isPromotion_Condition2 = true;
		isPromotion_Condition3 = true;
		isPromotion_Condition_Total = true;
		hunterInfo = _hunterInfo;
		UnityEngine.Debug.Log("************** HunterPromotion Data  = " + hunterInfo.Hunter.skillIdx);
		hunterPromotionDbData = GameDataManager.GetHunterPromotionData(hunterInfo.Hunter.color, hunterInfo.Hunter.maxTier, hunterInfo.Stat.hunterTier);
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
		hunter_Character.transform.localPosition = new Vector3(0f, 70f, 0f);
		hunter_Character.transform.localScale = new Vector3(1f, 1f, 1f);
		hunter_Character.Init(_hunterInfo);
		hunter_Level_Origin.text = string.Format(MWLocalize.GetData("common_text_max_level"), (hunterInfo.Stat.hunterTier * 20).ToString());
		hunter_Level_After.text = ((hunterInfo.Stat.hunterTier + 1) * 20).ToString();
		SetTierStar();
		SetRequiredItem();
		if (hunterInfo.Stat.hunterLevel >= hunterInfo.Stat.hunterTier * 20)
		{
			isPromotion_Condition3 = true;
		}
		else
		{
			isPromotion_Condition3 = false;
		}
		NotEnouchCoin.CallBuyCoin = OnCallBuyCoinEvent;
	}

	private void PromotionResponse()
	{
		hunter_Character.gameObject.SetActive(value: false);
		LobbyManager.ShowHunterPromotionUp(hunterInfo, _isSpawn: true);
	}

	private void SetTierStar()
	{
		for (int i = 0; i < hunterTiertr.childCount; i++)
		{
			hunterTiertr.GetChild(i).GetChild(0).GetChild(0)
				.gameObject.SetActive(value: false);
				hunterTiertr.GetChild(i).GetChild(0).gameObject.SetActive(value: false);
				hunterTiertr.GetChild(i).gameObject.SetActive(value: false);
			}
			for (int j = 0; j < hunterInfo.Hunter.maxTier; j++)
			{
				hunterTiertr.GetChild(j).gameObject.SetActive(value: true);
				if (hunterInfo.Stat.hunterTier >= j + 1)
				{
					hunterTiertr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
				}
				else if (hunterInfo.Stat.hunterTier + 1 >= j + 1)
				{
					hunterTiertr.GetChild(j).GetChild(0).gameObject.SetActive(value: true);
					hunterTiertr.GetChild(j).GetChild(0).GetChild(0)
						.gameObject.SetActive(value: true);
					}
				}
			}

			private void SetRequiredItem()
			{
				if (hunterPromotionDbData.hnip1 != 0)
				{
					requiredItem_Cell_1 = SetRequiredItemSet(1);
				}
				if (hunterPromotionDbData.hnip2 != 0)
				{
					requiredItem_Cell_2 = SetRequiredItemSet(2);
				}
				if (hunterPromotionDbData.hnip3 != 0)
				{
					requiredItem_Cell_3 = SetRequiredItemSet(3);
				}
				if (hunterPromotionDbData.hnip4 != 0)
				{
					requiredItem_Cell_4 = SetRequiredItemSet(4);
				}
				requiredCoin_Cell = MWPoolManager.Spawn("Grow", "cell_coin", hunterRequiredItemListtr).GetComponent<RequiredItem_Cell>();
				requiredCoin_Cell.transform.localScale = Vector3.one;
				requiredCoin_Cell.transform.SetAsLastSibling();
				requiredCoin_Cell.SetCostText(SetCostText_Coin());
				if (GameInfo.userData.userInfo.coin < hunterPromotionDbData.needCoin)
				{
					requiredCoin_Cell.SetClickType(ItemClickType.Coin, hunterPromotionDbData.needCoin - GameInfo.userData.userInfo.coin);
				}
				else
				{
					requiredCoin_Cell.SetClickType(ItemClickType.None);
				}
				requiredLevel_Cell = MWPoolManager.Spawn("Grow", "cell_badge", hunterRequiredItemListtr).GetComponent<RequiredItem_Cell>();
				requiredLevel_Cell.transform.localScale = Vector3.one;
				requiredLevel_Cell.transform.SetAsFirstSibling();
				requiredLevel_Cell.SetItemImg(50040);
				requiredLevel_Cell.SetCostText(SetCostText_Level());
			}

			private RequiredItem_Cell SetRequiredItemSet(int _idx)
			{
				RequiredItem_Cell requiredItem_Cell = null;
				requiredItem_Cell = MWPoolManager.Spawn("Grow", "cell_badge", hunterRequiredItemListtr).GetComponent<RequiredItem_Cell>();
				requiredItem_Cell.transform.localScale = Vector3.one;
				requiredItem_Cell.transform.SetAsLastSibling();
				switch (_idx)
				{
				case 1:
					requiredItem_Cell.SetItemImg(hunterPromotionDbData.hnip1);
					requiredItem_Cell.SetCostText(SetCostText_Badge(1));
					break;
				case 2:
					requiredItem_Cell.SetItemImg(hunterPromotionDbData.hnip2);
					requiredItem_Cell.SetCostText(SetCostText_Badge(2));
					break;
				case 3:
					requiredItem_Cell.SetItemImg(hunterPromotionDbData.hnip3);
					requiredItem_Cell.SetCostText(SetCostText_Badge(3));
					break;
				case 4:
					requiredItem_Cell.SetItemImg(hunterPromotionDbData.hnip4);
					requiredItem_Cell.SetCostText(SetCostText_Badge(4));
					break;
				}
				return requiredItem_Cell;
			}

			private string SetCostText_Badge(int _idx)
			{
				string result = string.Empty;
				bool flag = true;
				int num = 0;
				int num2 = 0;
				switch (_idx)
				{
				case 1:
					num = hunterPromotionDbData.hnip1;
					num2 = hunterPromotionDbData.hnip1_N;
					break;
				case 2:
					num = hunterPromotionDbData.hnip2;
					num2 = hunterPromotionDbData.hnip2_N;
					break;
				case 3:
					num = hunterPromotionDbData.hnip3;
					num2 = hunterPromotionDbData.hnip3_N;
					break;
				case 4:
					num = hunterPromotionDbData.hnip4;
					num2 = hunterPromotionDbData.hnip4_N;
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
						isPromotion_Condition1 = false;
						result = "<color=#ff0000>" + GameInfo.userData.userItemList[i].count + "</color>/" + num2;
					}
				}
				if (flag)
				{
					isPromotion_Condition1 = false;
					result = "<color=#ff0000>0</color>/" + num2;
				}
				Check_Promotion();
				return result;
			}

			private string SetCostText_Coin()
			{
				string empty = string.Empty;
				if (GameInfo.userData.userInfo.coin >= hunterPromotionDbData.needCoin)
				{
					empty = "<color=#ffffff>" + GameInfo.userData.userInfo.coin + "</color>/" + hunterPromotionDbData.needCoin;
				}
				else
				{
					isPromotion_Condition2 = false;
					empty = "<color=#ff0000>" + GameInfo.userData.userInfo.coin + "</color>/" + hunterPromotionDbData.needCoin;
				}
				Check_Promotion();
				return empty;
			}

			private string SetCostText_Level()
			{
				string empty = string.Empty;
				if (hunterInfo.Stat.hunterLevel >= hunterInfo.Stat.hunterTier * 20)
				{
					empty = "LV.<color=#ffffff>" + hunterInfo.Stat.hunterLevel + "</color>/" + hunterInfo.Stat.hunterTier * 20;
				}
				else
				{
					isPromotion_Condition3 = false;
					empty = "LV.<color=#ff0000>" + hunterInfo.Stat.hunterLevel + "</color>/" + hunterInfo.Stat.hunterTier * 20;
				}
				Check_Promotion();
				return empty;
			}

			private void Check_Promotion()
			{
				if (isPromotion_Condition1 && isPromotion_Condition2 && isPromotion_Condition3)
				{
					isPromotion_Condition_Total = true;
					promotion_Btn_Lock.gameObject.SetActive(value: false);
				}
				else
				{
					isPromotion_Condition_Total = false;
					promotion_Btn_Lock.gameObject.SetActive(value: true);
				}
			}

			private void OnCallBuyCoinEvent(int _needJewel)
			{
				Protocol_Set.Protocol_shop_popup_hunter_promotion_buy_coin_Req(hunterInfo.Hunter.hunterIdx, _needJewel, OnBuyCoinComplete);
			}

			private void OnBuyCoinComplete()
			{
				requiredCoin_Cell.SetCostText(SetCostText_Coin());
				if (GameInfo.userData.userInfo.coin < hunterPromotionDbData.needCoin)
				{
					requiredCoin_Cell.SetClickType(ItemClickType.Coin, hunterPromotionDbData.needCoin - GameInfo.userData.userInfo.coin);
				}
				else
				{
					requiredCoin_Cell.SetClickType(ItemClickType.None);
				}
				if (GameInfo.userData.userInfo.coin > hunterPromotionDbData.needCoin)
				{
					isPromotion_Condition2 = true;
					Check_Promotion();
				}
			}

			public void OnClickGoBack()
			{
				SoundController.EffectSound_Play(EffectSoundType.Cancel);
				LobbyManager.ShowHunterView(hunterInfo, _isSpawn: false);
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
			}
		}
