using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SaleStartPack : LobbyPopupBase
{
	public Action GoBackEvent;

	private const float PACKAGE_EFFECT_TIME = 1.2f;

	private const float PACKAGE_EFFECT_DELAY_SPAWN_DURATION = 0.4f;

	private const float PACKAGE_HUNTER_BOX_SHOW_DELAY = 0.5f;

	[SerializeField]
	private Text textPackPrice;

	[SerializeField]
	private Text textCoin;

	[SerializeField]
	private Text textJewel;

	[SerializeField]
	private Text textDisCount;

	[SerializeField]
	private Button btnPurchase;

	private SHOP_PACKAGE_LIST_DATA packageData;

	private System.Object purchase_args;

	public void Show(SHOP_PACKAGE_LIST_DATA _data)
	{
		packageData = _data;
		if (!CheckPackShow())
		{
			if (GoBackEvent != null)
			{
				GoBackEvent();
			}
		}
		else
		{
			base.Show();
			Init();
		}
	}

	public override void Hide()
	{
		base.Hide();
		InAppPurchaseManager.OnInAppPurchaseProcessComplete = null;
	}

	public override void HideProcessComplete()
	{
	}

	private void Init()
	{
		btnPurchase.enabled = true;
		textPackPrice.text = InAppPurchaseManager.GetPrice("matchhero_starter_pack", "USD 4.99");
		textCoin.text = $"{packageData.coin:#,###}";
		textJewel.text = $"{packageData.jewel:#,###}";
		textDisCount.text = $"{packageData.discount}%";
		InAppPurchaseManager.OnInAppPurchaseProcessComplete = OnInAppPurchaseComplete;
	}

	private bool CheckPackShow()
	{
		bool result = GameInfo.saleStartPackCheck == 0;
		GameInfo.saleStartPackCheck = Mathf.Min(GameInfo.saleStartPackCheck + 1, packageData.cntOffer);
		if (GameInfo.saleStartPackCheck == packageData.cntOffer)
		{
			GameInfo.saleStartPackCheck = 0;
		}
		return result;
	}

	private void OnInAppPurchaseComplete(int _purchase_id, string _signature, string _receipt, System.Object _args)
	{
		purchase_args = _args;
		Protocol_Set.Protocol_shop_buy_package_Req(_purchase_id, _signature, _receipt, OnBuyProductComplete);
	}

	private void OnBuyProductComplete(ChestListDbData[] _arrRewardList)
	{
		List<ChestListDbData> list = new List<ChestListDbData>();
		foreach (ChestListDbData chestListDbData in _arrRewardList)
		{
			if (chestListDbData.chestHunter > 0)
			{
				list.Add(chestListDbData);
			}
		}
		StartCoroutine(ProcessUserDataEffect(list));
	}

	private IEnumerator ProcessUserDataEffect(List<ChestListDbData> _listHunter)
	{
		int jewelCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < jewelCount; i++)
		{
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MWPoolManager.Spawn("Effect", "FX_jewel_get", null, 1.2f + num + 0.4f);
			transform.localScale = Vector3.one;
			transform.position = btnPurchase.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject = transform.gameObject;
			Vector3 userJewelPosition = LobbyManager.UserJewelPosition;
			LeanTween.moveX(gameObject, userJewelPosition.x, 1.2f + num);
			GameObject gameObject2 = transform.gameObject;
			Vector3 userJewelPosition2 = LobbyManager.UserJewelPosition;
			LeanTween.moveY(gameObject2, userJewelPosition2.y, 1.2f + num).setEaseInCubic();
		}
		int coinCount = UnityEngine.Random.Range(4, 8);
		for (int j = 0; j < coinCount; j++)
		{
			float num2 = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform2 = MWPoolManager.Spawn("Effect", "FX_Coin_get", null, 1.2f + num2 + 0.4f);
			transform2.localScale = new Vector2(0.12f, 0.12f);
			transform2.position = btnPurchase.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
			GameObject gameObject3 = transform2.gameObject;
			Vector3 userCoinPosition = LobbyManager.UserCoinPosition;
			LeanTween.moveX(gameObject3, userCoinPosition.x, 1.2f + num2).setEaseInCubic();
			GameObject gameObject4 = transform2.gameObject;
			Vector3 userCoinPosition2 = LobbyManager.UserCoinPosition;
			LeanTween.moveY(gameObject4, userCoinPosition2.y, 1.2f + num2);
		}
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
		SoundController.EffectSound_Play(EffectSoundType.GetJewel);
		SoundController.EffectSound_Play(EffectSoundType.GetCoin);
		yield return new WaitForSeconds(0.5f);
		LobbyManager.ShowChestOpen(_listHunter, ChestType.Mysterious);
		LobbyManager.PurchaseStartPackComplete();
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}

	public void OnClickPurchase()
	{
		Debug.Log(1);
		InAppPurchaseManager.BuyProductID("matchhero_starter_pack", 1);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
}
