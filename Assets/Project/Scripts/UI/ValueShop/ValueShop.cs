using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ValueShop : LobbyPopupBase
{
	public Action GoBackEvent;

	public ValueShopType valueShopState = ValueShopType.Daily;

	private const string FLOW_DURATION = "shopDailyDuration";

	private const string SHOP_DAILY_TIMER_KEY = "shopDailyTimerKey";

	[SerializeField]
	private Transform dailyList_Tr;

	[SerializeField]
	private Text textShopDailyTimer;

	[SerializeField]
	private Transform shopScrollPos;

	[SerializeField]
	private Transform dailyShopLock;

	[SerializeField]
	private Text textCoinAmount_1;

	[SerializeField]
	private Text textCoinAmount_2;

	[SerializeField]
	private Text textCoinAmount_3;

	[SerializeField]
	private Text textCoinAmount_4;

	[SerializeField]
	private Text textCoinAmount_5;

	[SerializeField]
	private Text textCoinAmount_6;

	[SerializeField]
	private Text textCoinPrice_1;

	[SerializeField]
	private Text textCoinPrice_2;

	[SerializeField]
	private Text textCoinPrice_3;

	[SerializeField]
	private Text textCoinPrice_4;

	[SerializeField]
	private Text textCoinPrice_5;

	[SerializeField]
	private Text textCoinPrice_6;

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

	[SerializeField]
	private BannerStartPack bannerStartPack;

	[SerializeField]
	private BannerSpecialOffer bannerSpecialOffer;

	private float flowDuration;

	private float shopDailyRefreshDuration;

	public void Show(ValueShopType _type = ValueShopType.Daily)
	{
		base.Show();
		base.gameObject.SetActive(value: true);
		Init(_type);
		LobbyManager.HideHunterLobby();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
	}

	public void SetInit()
	{
		RefreshValueShop_Buy();
	}

	private void Init(ValueShopType _type)
	{
		GameInfo.userData.userInfo.dailyShopNewYn = "n";
		LobbyManager.RefreshBottomNotice();
		shopDailyRefreshDuration = GameInfo.userData.userDailyItemList.dailyShopInfo.remainTime;
		bannerStartPack.StartPackPuchaseComplete = OnStartPackPurchaseComplete;
		CheckShopDailyTimer(shopDailyRefreshDuration);
		SetDailyShopItem();
		PackageListShow();
		SetCoinPrice();
		SetJewelPrice();
		switch (_type)
		{
		case ValueShopType.Daily:
		{
			Transform transform3 = shopScrollPos.transform;
			Vector3 localPosition5 = shopScrollPos.transform.localPosition;
			float x3 = localPosition5.x;
			Vector3 localPosition6 = shopScrollPos.transform.localPosition;
			transform3.localPosition = new Vector3(x3, 0f, localPosition6.z);
			break;
		}
		case ValueShopType.Coin:
		{
			Transform transform2 = shopScrollPos.transform;
			Vector3 localPosition3 = shopScrollPos.transform.localPosition;
			float x2 = localPosition3.x;
			Vector3 localPosition4 = shopScrollPos.transform.localPosition;
			transform2.localPosition = new Vector3(x2, 500f, localPosition4.z);
			break;
		}
		case ValueShopType.Jewel:
		{
			Transform transform = shopScrollPos.transform;
			Vector3 localPosition = shopScrollPos.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = shopScrollPos.transform.localPosition;
			transform.localPosition = new Vector3(x, 1300f, localPosition2.z);
			break;
		}
		}
		if (GameInfo.userData.userInfo.level >= 3)
		{
			dailyShopLock.gameObject.SetActive(value: false);
			if (LobbyManager.DailyShopOpen != null)
			{
				LobbyManager.DailyShopOpen();
			}
		}
	}

	private void SetCoinPrice()
	{
		textCoinAmount_1.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[0].coinNumber);
		textCoinAmount_2.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[1].coinNumber);
		textCoinAmount_3.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[2].coinNumber);
		textCoinAmount_4.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[3].coinNumber);
		textCoinAmount_5.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[4].coinNumber);
		textCoinAmount_6.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[5].coinNumber);
		textCoinPrice_1.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[0].jewelPrice);
		textCoinPrice_2.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[1].jewelPrice);
		textCoinPrice_3.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[2].jewelPrice);
		textCoinPrice_4.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[3].jewelPrice);
		textCoinPrice_5.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[4].jewelPrice);
		textCoinPrice_6.text = GameUtil.InsertCommaInt(GameInfo.userData.userDailyItemList.shopCoinList[5].jewelPrice);
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

	private void RefreshValueShop()
	{
		shopDailyRefreshDuration = GameInfo.userData.userDailyItemList.dailyShopInfo.remainTime;
		CheckShopDailyTimer(shopDailyRefreshDuration);
		SetDailyShopItem();
	}

	private void RefreshValueShop_Buy()
	{
		CheckShopDailyTimer(shopDailyRefreshDuration);
		SetDailyShopItem();
	}

	private void SetDailyShopItem()
	{
		int childCount = dailyList_Tr.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", dailyList_Tr.GetChild(0));
		}
		if (GameInfo.userData.userDailyItemList.dailyShopInfo.dailyShopList.Length > 0)
		{
			for (int j = 0; j < GameInfo.userData.userDailyItemList.dailyShopInfo.dailyShopList.Length; j++)
			{
				DailyItem dailyItem = null;
				dailyItem = MWPoolManager.Spawn("Item", "DailyItem", dailyList_Tr).GetComponent<DailyItem>();
				dailyItem.transform.localPosition = Vector3.zero;
				dailyItem.transform.localScale = Vector3.one;
				dailyItem.Init(GameInfo.userData.userDailyItemList.dailyShopInfo.dailyShopList[j], this);
			}
		}
	}

	private void PackageListShow()
	{
		SHOP_PACKAGE_LIST_RESULT packageList = LobbyManager.PackageList;
		if (packageList == null || packageList.packageList == null || packageList.packageList.Length == 0)
		{
			bannerStartPack.Hide();
			bannerSpecialOffer.Hide();
		}
		else if (packageList.starterPackYn == "n")
		{
			bannerStartPack.Show(packageList.packageList[0]);
			bannerSpecialOffer.Hide();
		}
		else if (packageList.specialOfferYn == "n")
		{
			bannerStartPack.Hide();
			bannerSpecialOffer.Show(packageList.packageList[1]);
		}
	}

	private void OnStartPackPurchaseComplete()
	{
		PackageListShow();
	}

	private void Exit()
	{
		StopAllCoroutines();
		SaveShopDailyState();
	}

	private void CheckShopDailyTimer(float startTime = 0f)
	{
		StopAllCoroutines();
		StartCoroutine(StartCheckTimer(startTime));
	}

	private IEnumerator StartCheckTimer(float startTime)
	{
		float currentSecond = startTime;
		float time = Time.time;
		while (currentSecond > 0f)
		{
			float timeStamp = Time.time;
			yield return null;
			currentSecond -= Time.time - timeStamp;
			TimeSpan ts = TimeSpan.FromSeconds(currentSecond);
			string minSec = $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
			textShopDailyTimer.text = string.Format(MWLocalize.GetData("shop_daily_text_1"), minSec);
			flowDuration = shopDailyRefreshDuration - currentSecond;
		}
		StopAllCoroutines();
		Protocol_Set.Protocol_shop_list_Req(RefreshValueShop);
	}

	private void LoadShopDailyState()
	{
		string @string = ObscuredPrefs.GetString("shopDailyTimerKey", string.Empty);
		if (@string != string.Empty)
		{
			DateTime d = DateTime.Parse(@string);
			flowDuration = (float)ObscuredPrefs.GetDouble("shopDailyDuration", 0.0);
			double num = (DateTime.UtcNow - d).TotalMilliseconds / 1000.0 + (double)flowDuration;
			int num2 = (int)(num / (double)shopDailyRefreshDuration);
			int num3 = (int)(num % (double)shopDailyRefreshDuration);
			if (num2 > 0)
			{
				Protocol_Set.Protocol_shop_list_Req(RefreshValueShop);
			}
			else
			{
				CheckShopDailyTimer(shopDailyRefreshDuration - (float)num3);
			}
		}
	}

	private void SaveShopDailyState()
	{
		ObscuredPrefs.SetDouble("shopDailyDuration", flowDuration);
		ObscuredPrefs.SetString("shopDailyTimerKey", $"{DateTime.UtcNow}");
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	public void ShowValueShopBuyPopup_Wealth_Jewel(int _type)
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		valueShopState = ValueShopType.Coin;
		LobbyManager.ShowValueShopBuy(_type, "jewel");
	}

	public void ShowValueShopBuyPopup_Wealth_Cash(int _type)
	{
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		valueShopState = ValueShopType.Jewel;
		LobbyManager.ShowValueShopBuy(_type, "cash");
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			UnityEngine.Debug.Log("PAUSE");
			Exit();
		}
		else
		{
			UnityEngine.Debug.Log("RESUME");
			LoadShopDailyState();
		}
	}

	private void OnApplicationQuit()
	{
		Exit();
	}

	private void OnDisable()
	{
		Exit();
	}
}
