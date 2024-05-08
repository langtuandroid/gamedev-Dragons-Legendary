using UnityEngine;
using UnityEngine.UI;

public class JewelShop : MonoBehaviour
{
	[SerializeField]
	private Text textAmount_1;

	[SerializeField]
	private Text textAmount_2;

	[SerializeField]
	private Text textAmount_3;

	[SerializeField]
	private Text textAmount_4;

	[SerializeField]
	private Text textAmount_5;

	[SerializeField]
	private Text textAmount_6;

	[SerializeField]
	private Text textPrice_1;

	[SerializeField]
	private Text textPrice_2;

	[SerializeField]
	private Text textPrice_3;

	[SerializeField]
	private Text textPrice_4;

	[SerializeField]
	private Text textPrice_5;

	[SerializeField]
	private Text textPrice_6;

	public void Init()
	{
		base.gameObject.SetActive(value: true);
		SetJewelPrice();
	}

	private void SetJewelPrice()
	{
		textAmount_1.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopJewelList[0].jewel);
		textAmount_2.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopJewelList[1].jewel);
		textAmount_3.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopJewelList[2].jewel);
		textAmount_4.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopJewelList[3].jewel);
		textAmount_5.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopJewelList[4].jewel);
		textAmount_6.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopJewelList[5].jewel);
		textPrice_1.text = InAppPurchaseManager.GetPrice("matchhero_jewel_1", "USD 2.99");
		textPrice_2.text = InAppPurchaseManager.GetPrice("matchhero_jewel_2", "USD 4.99");
		textPrice_3.text = InAppPurchaseManager.GetPrice("matchhero_jewel_3", "USD 9.99");
		textPrice_4.text = InAppPurchaseManager.GetPrice("matchhero_jewel_4", "USD 29.99");
		textPrice_5.text = InAppPurchaseManager.GetPrice("matchhero_jewel_5", "USD 59.99");
		textPrice_6.text = InAppPurchaseManager.GetPrice("matchhero_jewel_6", "USD 99.99");
	}

	public void ShowJewelShopBuyPopup(int _type)
	{
		PuzzlePlayManager.BuyJewelShop(_type);
	}

	public void ClosePopup()
	{
		if (PuzzlePlayManager.OnContinueTimer != null)
		{
			PuzzlePlayManager.OnContinueTimer();
			PuzzlePlayManager.OnContinueTimer = null;
		}
		base.gameObject.SetActive(value: false);
	}
}
