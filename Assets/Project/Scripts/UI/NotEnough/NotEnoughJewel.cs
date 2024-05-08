using System;
using UnityEngine;
using UnityEngine.UI;

public class NotEnoughJewel : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Text textNeedJewel;

	private int lackJewel;

	public void Show(int _jewel)
	{
		lackJewel = _jewel;
		base.Open();
		Init();
	}

	private void Init()
	{
		textNeedJewel.text = $"{lackJewel}";
	}

	private void ShopListResponse()
	{
		LobbyManager.ShowValueShop(ValueShopType.Jewel);
	}

	public void OnClickGotoShop()
	{
		if (GameInfo.userData.userDailyItemList == null)
		{
			Protocol_Set.Protocol_shop_list_Req(ShopListResponse);
		}
		else
		{
			ShopListResponse();
		}
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickCancel()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
}
