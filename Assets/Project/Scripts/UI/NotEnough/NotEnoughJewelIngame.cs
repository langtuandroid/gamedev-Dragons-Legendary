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
		PuzzlePlayManager.ShowJewelShop();
		base.gameObject.SetActive(value: false);
	}

	public void OnClickGotoShop()
	{
		if (GameInfo.userData.userDailyItemList == null)
		{
			Protocol_Set.Protocol_shop_list_Req(ShopListResponse);
			return;
		}
		PuzzlePlayManager.ShowJewelShop();
		base.gameObject.SetActive(value: false);
	}

	public void OnClickCancel()
	{
		if (PuzzlePlayManager.OnContinueTimer != null)
		{
			PuzzlePlayManager.OnContinueTimer();
			PuzzlePlayManager.OnContinueTimer = null;
		}
		base.gameObject.SetActive(value: false);
	}
}
