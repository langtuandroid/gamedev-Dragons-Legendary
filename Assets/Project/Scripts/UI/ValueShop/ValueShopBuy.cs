using System;
using UnityEngine;

using UnityEngine.UI;

public class ValueShopBuy : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Transform buyItem;

	[SerializeField]
	private Transform item_Img;

	[SerializeField]
	private Transform item_Img_Tr;

	[SerializeField]
	private Text item_Name;

	[SerializeField]
	private Text item_Owned;

	[SerializeField]
	private Text item_BuyCount;

	[SerializeField]
	private Text item_Left;

	[SerializeField]
	private Text item_Price;

	[SerializeField]
	private Transform buyWealth;

	[SerializeField]
	private Transform jewel_BT;

	[SerializeField]
	private Text jewel_Price;

	[SerializeField]
	private Text jewel_Amount;

	[SerializeField]
	private Transform jewel_Tr;

	[SerializeField]
	private Transform cash_BT;

	[SerializeField]
	private Text cash_Price;

	[SerializeField]
	private Text cash_Amount;

	[SerializeField]
	private Transform cash_Tr;

	[SerializeField]
	private Transform cell_Jewel;

	[SerializeField]
	private Transform cell_Cash;

	private int buy_Count;

	private int total_Price;

	private bool isbuyCondition;

	[SerializeField]
	private int key;

	[SerializeField]
	private string type = string.Empty;

	[SerializeField]
	private ShopDailyDbData shopDailyDbData;

	[SerializeField]
	private ShopCoinDbData shopCoinDbData;

	[SerializeField]
	private ShopJewelDbData shopJewelDbData;

	private System.Object purchase_args;

	public void Show(int _key, string _type)
	{
		base.Open();
		base.gameObject.SetActive(value: true);
		Init(_key, _type);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
		InAppPurchaseManager.OnInAppPurchaseProcessComplete = null;
	}

	public void Init(int _key, string _type)
	{
		key = _key;
		type = _type;
		buy_Count = 1;
		if (_type == null)
		{
			return;
		}
		if (!(_type == "item"))
		{
			if (!(_type == "jewel"))
			{
				if (_type == "cash")
				{
					InAppPurchaseManager.OnInAppPurchaseProcessComplete = OnInAppPurchaseComplete;
					SetJewelForm();
				}
			}
			else
			{
				SetCoinForm();
			}
		}
		else
		{
			SetItemForm();
		}
	}

	private void SetItemForm()
	{
		for (int i = 0; i < GameInfo.userData.userDailyItemList.dailyShopInfo.dailyShopList.Length; i++)
		{
			if (GameInfo.userData.userDailyItemList.dailyShopInfo.dailyShopList[i].productIdx == key)
			{
				shopDailyDbData = GameInfo.userData.userDailyItemList.dailyShopInfo.dailyShopList[i];
			}
		}
		buyItem.gameObject.SetActive(value: true);
		buyWealth.gameObject.SetActive(value: false);
		if (item_Img != null)
		{
			MasterPoolManager.ReturnToPool("Item", item_Img);
			item_Img = null;
		}
		item_Img = MasterPoolManager.SpawnObject("Item", "Item_" + shopDailyDbData.itemIdx, item_Img_Tr);
		item_Name.text = MasterLocalize.GetData(shopDailyDbData.itemName);
		item_Owned.text = string.Format(MasterLocalize.GetData("common_text_owned"), GameInfo.userData.GetItemCount(shopDailyDbData.itemIdx).ToString());
		item_BuyCount.text = "x" + buy_Count;
		item_Left.text = string.Format(shopDailyDbData.left.ToString(), MasterLocalize.GetData("popup_level_text_2"));
		total_Price = shopDailyDbData.resultPrice;
		Check_Coin_Enough();
	}

	private void SetCoinForm()
	{
		for (int i = 0; i < GameInfo.userData.userDailyItemList.shopCoinList.Length; i++)
		{
			if (GameInfo.userData.userDailyItemList.shopCoinList[i].productIdx == key)
			{
				shopCoinDbData = GameInfo.userData.userDailyItemList.shopCoinList[i];
			}
		}
		buyItem.gameObject.SetActive(value: false);
		buyWealth.gameObject.SetActive(value: true);
		cell_Jewel.gameObject.SetActive(value: true);
		cell_Cash.gameObject.SetActive(value: false);
		jewel_Tr.gameObject.SetActive(value: true);
		cash_Tr.gameObject.SetActive(value: false);
		jewel_BT.gameObject.SetActive(value: true);
		cash_BT.gameObject.SetActive(value: false);
		for (int j = 0; j < jewel_Tr.childCount; j++)
		{
			jewel_Tr.GetChild(j).transform.gameObject.SetActive(value: false);
		}
		jewel_Tr.GetChild(shopCoinDbData.productIdx - 1).transform.gameObject.SetActive(value: true);
		total_Price = shopCoinDbData.jewelPrice;
		jewel_Price.text = total_Price.ToString();
		jewel_Amount.text = GameUtil.InsertCommaInt(shopCoinDbData.coinNumber);
		Check_Jewel_Enough();
	}

	private void SetJewelForm()
	{
		for (int i = 0; i < GameInfo.userData.userDailyItemList.shopJewelList.Length; i++)
		{
			if (GameInfo.userData.userDailyItemList.shopJewelList[i].productIdx == key)
			{
				shopJewelDbData = GameInfo.userData.userDailyItemList.shopJewelList[i];
			}
		}
		buyItem.gameObject.SetActive(value: false);
		buyWealth.gameObject.SetActive(value: true);
		cell_Jewel.gameObject.SetActive(value: false);
		cell_Cash.gameObject.SetActive(value: true);
		jewel_Tr.gameObject.SetActive(value: false);
		cash_Tr.gameObject.SetActive(value: true);
		jewel_BT.gameObject.SetActive(value: false);
		cash_BT.gameObject.SetActive(value: true);
		for (int j = 0; j < cash_Tr.childCount; j++)
		{
			cash_Tr.GetChild(j).transform.gameObject.SetActive(value: false);
		}
		UnityEngine.Debug.Log("this.key - 1 = " + (key - 1));
		cash_Tr.GetChild(shopJewelDbData.productIdx - 1).transform.gameObject.SetActive(value: true);
		switch (key)
		{
		case 1:
			cash_Price.text = InAppPurchaseManager.GetPrice("matchhero_jewel_1", "USD 2.99");
			break;
		case 2:
			cash_Price.text = InAppPurchaseManager.GetPrice("matchhero_jewel_2", "USD 4.99");
			break;
		case 3:
			cash_Price.text = InAppPurchaseManager.GetPrice("matchhero_jewel_3", "USD 9.99");
			break;
		case 4:
			cash_Price.text = InAppPurchaseManager.GetPrice("matchhero_jewel_4", "USD 29.99");
			break;
		case 5:
			cash_Price.text = InAppPurchaseManager.GetPrice("matchhero_jewel_5", "USD 59.99");
			break;
		case 6:
			cash_Price.text = InAppPurchaseManager.GetPrice("matchhero_jewel_6", "USD 99.99");
			break;
		}
		cash_Amount.text = GameUtil.InsertCommaInt(shopJewelDbData.jewel);
	}

	private int GetTotalPrice(int _count)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < _count; i++)
		{
			num += shopDailyDbData.coinPrice + (shopDailyDbData.buyCount + num2) * shopDailyDbData.priceIncrease;
			num2++;
		}
		return num;
	}

	private void Check_Coin_Enough()
	{
		if (total_Price <= GameInfo.userData.userInfo.coin)
		{
			isbuyCondition = true;
			item_Price.text = "<color=#ffffff>" + total_Price.ToString() + "</color>";
		}
		else
		{
			isbuyCondition = false;
			item_Price.text = "<color=#ff0000>" + total_Price.ToString() + "</color>";
		}
	}

	private void Check_Jewel_Enough()
	{
		if (total_Price <= GameInfo.userData.userInfo.jewel)
		{
			Debug.Log(GameInfo.userData.userInfo.jewel);
			isbuyCondition = true;
			item_Price.text = "<color=#ffffff>" + total_Price.ToString() + "</color>";
		}
		else
		{
			isbuyCondition = false;
			item_Price.text = "<color=#ff0000>" + total_Price.ToString() + "</color>";
		}
	}

	private void OnInAppPurchaseComplete(int _purchase_id, string _signature, string _receipt, System.Object _args)
	{
		purchase_args = _args;
		Protocol_Set.Protocol_shop_buy_product_Req(_purchase_id, _signature, _receipt, OnBuyProductComplete);
	}

	private void OnBuyProductComplete()
	{
		AnalyticsManager.InAppPurchaseAppEnvent(purchase_args);
		LobbyManager.ShowValueShop_Refresh();
		OnClickGoBack();
		SoundController.EffectSound_Play(EffectSoundType.GetJewel);
	}

	private void BuyDailyResponse()
	{
		LobbyManager.ShowValueShop_Refresh();
		OnClickGoBack();
		SoundController.EffectSound_Play(EffectSoundType.UseCoin);
	}

	private void BuyCoinResponse()
	{
		LobbyManager.ShowValueShop_Refresh();
		OnClickGoBack();
		SoundController.EffectSound_Play(EffectSoundType.GetCoin);
	}

	public void Click_Buy_Count_Up()
	{
		if (buy_Count < shopDailyDbData.left)
		{
			buy_Count++;
			item_BuyCount.text = "x" + buy_Count;
			item_Left.text = shopDailyDbData.left.ToString() + " left";
			total_Price = GetTotalPrice(buy_Count);
			Check_Coin_Enough();
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	public void Click_Buy_Count_Down()
	{
		if (buy_Count >= 2)
		{
			buy_Count--;
			item_BuyCount.text = "x" + buy_Count;
			item_Left.text = shopDailyDbData.left.ToString() + " left";
			total_Price = GetTotalPrice(buy_Count);
			Check_Coin_Enough();
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	public void Click_Buy_Daily()
	{
		if (isbuyCondition)
		{
			Protocol_Set.Protocol_shop_buy_daily_Req(shopDailyDbData.productIdx, buy_Count, BuyDailyResponse);
		}
	}

	public void Click_Buy_Coin()
	{
		if (isbuyCondition)
		{
			Protocol_Set.Protocol_shop_buy_coin_Req(shopCoinDbData.productIdx, BuyCoinResponse);
			return;
		}
		Debug.Log("NOT ENOUGH");
		LobbyManager.ShowNotEnoughJewel(total_Price - GameInfo.userData.userInfo.jewel);
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}

	public void Click_Buy_Jewel()
	{
		switch (shopJewelDbData.productIdx)
		{
		case 1:
			InAppPurchaseManager.BuyProductID("matchhero_jewel_1", shopJewelDbData.productIdx);
			break;
		case 2:
			InAppPurchaseManager.BuyProductID("matchhero_jewel_2", shopJewelDbData.productIdx);
			break;
		case 3:
			InAppPurchaseManager.BuyProductID("matchhero_jewel_3", shopJewelDbData.productIdx);
			break;
		case 4:
			InAppPurchaseManager.BuyProductID("matchhero_jewel_4", shopJewelDbData.productIdx);
			break;
		case 5:
			InAppPurchaseManager.BuyProductID("matchhero_jewel_5", shopJewelDbData.productIdx);
			break;
		case 6:
			InAppPurchaseManager.BuyProductID("matchhero_jewel_6", shopJewelDbData.productIdx);
			break;
		}
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
