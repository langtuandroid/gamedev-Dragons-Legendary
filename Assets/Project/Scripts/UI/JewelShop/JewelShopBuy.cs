using UnityEngine;

using UnityEngine.UI;

public class JewelShopBuy : MonoBehaviour
{
	[SerializeField]
	private Text cash_Price;

	[SerializeField]
	private Text cash_Amount;

	[SerializeField]
	private Transform cell_Cash;

	[SerializeField]
	private int key;

	[SerializeField]
	private ShopJewelDbData shopJewelDbData;

	private System.Object purchase_args;

	public void Init(int _key)
	{
		base.gameObject.SetActive(value: true);
		key = _key;
		InAppPurchaseManager.OnInAppPurchaseProcessComplete = OnInAppPurchaseComplete;
		SetJewelForm();
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
		for (int j = 0; j < cell_Cash.childCount; j++)
		{
			cell_Cash.GetChild(j).gameObject.SetActive(value: false);
		}
		cell_Cash.GetChild(key - 1).gameObject.SetActive(value: true);
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

	private void OnInAppPurchaseComplete(int _purchase_id, string _signature, string _receipt, System.Object _args)
	{
		InAppPurchaseManager.OnInAppPurchaseProcessComplete = null;
		purchase_args = _args;
		Protocol_Set.Protocol_shop_buy_product_Req(_purchase_id, _signature, _receipt, OnBuyProductComplete);
	}

	private void OnBuyProductComplete()
	{
		AnalyticsManager.InAppPurchaseAppEnvent(purchase_args);
		PuzzlePlayManager.CloseJewelShop();
		base.gameObject.SetActive(value: false);
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

	public void ClosePopup()
	{
		base.gameObject.SetActive(value: false);
	}
}
