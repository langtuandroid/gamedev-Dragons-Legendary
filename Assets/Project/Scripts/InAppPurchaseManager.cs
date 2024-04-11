using com.F4A.MobileThird;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class InAppPurchaseManager : MonoBehaviour
{
	public static Action<int, string, string, System.Object> OnInAppPurchaseProcessComplete;

	public const string INAPP_PURCHASE_JEWEL_1 = "matchhero_jewel_1";

	public const string INAPP_PURCHASE_JEWEL_2 = "matchhero_jewel_2";

	public const string INAPP_PURCHASE_JEWEL_3 = "matchhero_jewel_3";

	public const string INAPP_PURCHASE_JEWEL_4 = "matchhero_jewel_4";

	public const string INAPP_PURCHASE_JEWEL_5 = "matchhero_jewel_5";

	public const string INAPP_PURCHASE_JEWEL_6 = "matchhero_jewel_6";

	public const string INAPP_PURCHASE_STARTER_PACK = "matchhero_starter_pack";

	public const string INAPP_PURCHASE_SPECIAL_OFFER = "matchhero_special_offer";

	public const string INAPP_PURCHASE_ARENA_PACK = "matchhero_arena_pack";

	private static InAppPurchaseManager instance;

	private int purchase_id;

	public static void BuyProductID(string productId, int idx)
	{
		try
		{
			var status = IAPManager.Instance.BuyProductByID(productId);
			instance.purchase_id = idx;
		}
		catch
		{

		}
	}

	public static string GetPrice(string idstring, string origin)
	{
		return instance.GetProductMoneyString(idstring, origin);
	}

	private bool IsInitialized()
	{
		return IAPManager.Instance.IsInitialized();
	}

	private string GetProductMoneyString(string idString, string origin)
	{
		string price = IAPManager.Instance.GetProductPriceStringById(idString);
		if (string.IsNullOrEmpty(price)) price = origin;
		return price;
	}

    #region OLD
	/*
    private void VerificationReceipt_AOS(PurchaseEventArgs args)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)JsonConvert.DeserializeObject(args.purchasedProduct.receipt);
		Dictionary<string, object> dictionary2 = (Dictionary<string, object>)JsonConvert.DeserializeObject((string)dictionary["Payload"]);
		string arg = (string)dictionary2["json"];
		string arg2 = (string)dictionary2["signature"];
		purchase_args = args;
		if (OnInAppPurchaseProcessComplete != null)
		{
			OnInAppPurchaseProcessComplete(purchase_id, arg2, arg, args);
		}
	}

	private void VerificationReceipt_AWS(PurchaseEventArgs args)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)JsonConvert.DeserializeObject(args.purchasedProduct.receipt);
		string arg = (string)dictionary["Payload"];
		string id = args.purchasedProduct.definition.id;
		purchase_args = args;
		if (OnInAppPurchaseProcessComplete != null)
		{
			OnInAppPurchaseProcessComplete(purchase_id, id, arg, args);
		}
	}

	private void VerificationReceipt_IOS(PurchaseEventArgs args)
	{
		string receipt = args.purchasedProduct.receipt;
		c_Iap c_Iap = null;
		string text;
		if (receipt.Contains("Payload"))
		{
			c_Iap = JsonConvert.DeserializeObject<c_Iap>(receipt);
			text = c_Iap.Payload;
		}
		else
		{
			text = receipt;
		}
		if (text.IndexOf("{") > -1 && text.IndexOf("}") > -1)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			text = Convert.ToBase64String(bytes);
		}
		string id = args.purchasedProduct.definition.id;
		string arg = text;
		purchase_args = args;
		if (OnInAppPurchaseProcessComplete != null)
		{
			OnInAppPurchaseProcessComplete(purchase_id, id, arg, args);
		}
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		UnityEngine.Debug.Log($"ProcessPurchase: PASS. Product: '{args.purchasedProduct.definition.id}'");
		if (BuildSet.CurrentPlatformType == PlatformType.aos)
		{
			VerificationReceipt_AOS(args);
		}
		else if (BuildSet.CurrentPlatformType == PlatformType.aws)
		{
			VerificationReceipt_AWS(args);
		}
		else if (BuildSet.CurrentPlatformType == PlatformType.ios)
		{
			VerificationReceipt_IOS(args);
		}
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		UnityEngine.Debug.Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
	}
    */
	#endregion

    private void Awake()
	{
		instance = this;
	}

	private void OnEnable()
	{
		IAPManager.OnBuyPurchaseSuccessed += IAPManager_OnBuyPurchaseSuccessed;
		IAPManager.OnBuyPurchaseFailed += IAPManager_OnBuyPurchaseFailed;
	}

	private void IAPManager_OnBuyPurchaseSuccessed(string id)
	{
		OnInAppPurchaseProcessComplete?.Invoke(purchase_id, string.Empty, string.Empty, new System.Object());
    }

	private void IAPManager_OnBuyPurchaseFailed(string id, string error)
	{
	}

	private void OnDisable()
	{
        IAPManager.OnBuyPurchaseSuccessed -= IAPManager_OnBuyPurchaseSuccessed;
        IAPManager.OnBuyPurchaseFailed -= IAPManager_OnBuyPurchaseFailed;
    }
}
