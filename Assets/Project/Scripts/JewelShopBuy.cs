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
			break;
		case 2:
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			break;
		case 6:
			break;
		}
		cash_Amount.text = GameUtil.InsertCommaInt(shopJewelDbData.jewel);
	}


	public void Click_Buy_Jewel()
	{
		switch (shopJewelDbData.productIdx)
		{
		case 1:
			break;
		case 2:
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			break;
		case 6:
			break;
		}
	}

	public void ClosePopup()
	{
		base.gameObject.SetActive(value: false);
	}
}
