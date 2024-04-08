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
	}

	public void ShowJewelShopBuyPopup(int _type)
	{
		InGamePlayManager.JewelShopBuy(_type);
	}

	public void ClosePopup()
	{
		if (InGamePlayManager.continueTimer != null)
		{
			InGamePlayManager.continueTimer();
			InGamePlayManager.continueTimer = null;
		}
		base.gameObject.SetActive(value: false);
	}
}
