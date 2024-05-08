using System;
using UnityEngine;
using UnityEngine.UI;

public class NotEnouchCoin : LobbyPopupBase
{
	public Action GoBackEvent;

	public static Action<int> CallBuyCoin;

	[SerializeField]
	private Text textLackCoin;

	[SerializeField]
	private Text textConvertJewel;

	private int needJewel;

	private int lackCoin;

	public void Show(int coin)
	{
		lackCoin = coin;
		LobbyManager.HideHunterLobby();
		base.Open();
		Init();
	}

	private void Init()
	{
		needJewel = GameUtil.GetConvertCoinToJewel(lackCoin);
		textConvertJewel.text = $"{needJewel}";
		textLackCoin.text = $"{lackCoin}";
	}

	private void ShopListResponse()
	{
		LobbyManager.ShowValueShop(ValueShopType.Coin);
	}

	public void OnClickChargeCoin()
	{
		if (needJewel > GameInfo.userData.userInfo.jewel)
		{
			Protocol_Set.Protocol_shop_list_Req(ShopListResponse);
		}
		else if (CallBuyCoin != null)
		{
			CallBuyCoin(needJewel);
		}
		if (LobbyManager.HunterLevelHunterInfo != null)
		{
			LobbyManager.ShowHunterLevel(LobbyManager.HunterLevelHunterInfo, _isSpawn: false);
		}
		if (LobbyManager.HunterPromotionHunterInfo != null)
		{
			LobbyManager.ShowHunterPromotion(LobbyManager.HunterPromotionHunterInfo, _isSpawn: false);
		}
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickCancel()
	{
		if (LobbyManager.HunterLevelHunterInfo != null)
		{
			LobbyManager.ShowHunterLevel(LobbyManager.HunterLevelHunterInfo, _isSpawn: false);
		}
		if (LobbyManager.HunterPromotionHunterInfo != null)
		{
			LobbyManager.ShowHunterPromotion(LobbyManager.HunterPromotionHunterInfo, _isSpawn: false);
		}
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
}
