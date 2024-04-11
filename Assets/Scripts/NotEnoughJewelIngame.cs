using UnityEngine;
using UnityEngine.UI;

public class NotEnoughJewelIngame : MonoBehaviour
{
	[SerializeField]
	private Text textNeedJewel;

	private int lackJewel;

	public void Init()
	{
		base.gameObject.SetActive(value: true);
		lackJewel = 10 - GameInfo.userData.userInfo.jewel;
		textNeedJewel.text = $"{lackJewel}";
	}

	private void ShopListResponse()
	{
		InGamePlayManager.JewelShop();
		base.gameObject.SetActive(value: false);
	}

	public void OnClickGotoShop()
	{
		if (GameInfo.userData.userDailyItemList == null)
		{
			Protocol_Set.Protocol_shop_list_Req(ShopListResponse);
			return;
		}
		InGamePlayManager.JewelShop();
		base.gameObject.SetActive(value: false);
	}

	public void OnClickCancel()
	{
		if (InGamePlayManager.continueTimer != null)
		{
			InGamePlayManager.continueTimer();
			InGamePlayManager.continueTimer = null;
		}
		base.gameObject.SetActive(value: false);
	}
}
