namespace com.F4A.MobileThird
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	#if F4A_FACEBOOK_AD
	using AudienceNetwork;
	#endif
	
	public partial class AdsManager
	{
		//private InterstitialAd interstitialAd;
		//private bool isLoaded;
		//[HideInInspector]
		//public static int inadsStatus;
		//[HideInInspector]
		//public int adsStatus;
		#if F4A_FACEBOOK_AD
		private AdView adView;
		#endif
		//public bool isTest;
		//public bool isFBAdsFirst;
		//[HideInInspector]
		//public bool isShowFBBanner;
		//[HideInInspector]
		//public bool isShowAdmobBanner;

		//string facebookBannerID;
		//string facebookInadsID;

		//void Awake()
		//{
		//	if (isTest == true) {
		//		facebookBannerID = "YOUR_PLACEMENT_ID";
		//		facebookInadsID = "YOUR_PLACEMENT_ID";
		//	} 
		//	else 
		//	{
		//		#if UNITY_ANDROID
		//		facebookBannerID = facebookBannerID_Android;
		//		facebookInadsID = facebookInadsID_Android;
		//		#elif UNITY_IOS
		//		facebookBannerID = facebookBannerID_IOS;
		//		facebookInadsID = facebookInadsID_IOS;
		//		#endif
		//	}
		//}
		
		private bool IsEnableFacebookAd()
		{
			return adConfigData != null && adConfigData.facebookAdConfig != null && adConfigData.facebookAdConfig.enableAd;
		}
		
		private void InitializationFacebookAd(){
			if(!IsRemoveAds()){
				LoadBannerFB();
			}
		}

		private void LoadBannerFB ()
		{
			//// Create a banner's ad view with a unique placement ID (generate your own on the Facebook app settings).
			//// Use different ID for each ad placement in your app.
			//AdView adView = new AdView (facebookBannerID, AdSize.BANNER_HEIGHT_50);
			//	this.adView = adView;
			//	this.adView.Register (this.gameObject);


			//	// Set delegates to get notified on changes or when the user interacts with the ad.
			//	this.adView.AdViewDidLoad = (delegate() {
			//		//Debug.Log ("Ad view loaded.");
			//		adsStatus = 1;
			//	});
			//	this.adView.AdViewDidFailWithError = (delegate(string error) {
			//		//Debug.Log ("Ad view failed to load with error: " + error);
			//		adsStatus = 2;
				
			//	});
			//// Initiate a request to load an ad.
			//this.adView.LoadAd ();
		}

		//public void ShowBannerFB()
		//{
		//	if(isFBAdsFirst == true)
		//	{
		//		if(adsStatus == 1)
		//		{
		//			double height = AudienceNetwork.Utility.AdUtility.convert (Screen.height);
		//			this.adView.Show (height - 50);
		//			isShowFBBanner = true;
		//		}
		//		else 
		//		{
		//            //if (AdsControl.instance.admobControl.isHasBanner() == true) 
		//            //{
		//            //	AdsControl.instance.admobControl.ShowBannerAdmob();
		//            //	isShowAdmobBanner = true;
		//            //}
		//            if (AdsManager.Instance.IsBannerAdsReady())
		//            {
		//                AdsManager.Instance.ShowBannerAds();
		//                isShowAdmobBanner = true;
		//            }
		//        } 
		//	}
		//	else
		//	{
		//        if (AdsManager.Instance.IsBannerAdsReady())
		//        {
		//            AdsManager.Instance.ShowBannerAds();
		//            isShowAdmobBanner = true;
		//        }
		//        else 
		//		{
		//			if(adsStatus == 1)
		//			{
		//				double height = AudienceNetwork.Utility.AdUtility.convert (Screen.height);
		//				this.adView.Show (height - 50);
		//				isShowFBBanner = true;
		//			}
		//		}
		//	}
		//}

		//public void ClearBannerFB () 
		//{
		//	// Dispose of banner ad when the scene is destroyed
		//	if (this.adView) {
		//		//Debug.Log("clearbannerfb");
		//		this.adView.Dispose ();
		//	}
		//}

		// Load button
		private void LoadInterstitial ()
		{
			//// Create the interstitial unit with a placement ID (generate your own on the Facebook app settings).
			//// Use different ID for each ad placement in your app.
			//InterstitialAd interstitialAd = new InterstitialAd (facebookInadsID);
			//this.interstitialAd = interstitialAd;
			//this.interstitialAd.Register (this.gameObject);
		
			//// Set delegates to get notified on changes or when the user interacts with the ad.
			//this.interstitialAd.InterstitialAdDidLoad = (delegate() {
			//	//Debug.Log ("Interstitial ad loaded.");
			//	inadsStatus = 1;
			//	this.isLoaded = true;
			//});
			//this.interstitialAd.InterstitialAdDidClose = (delegate() {
			//	#if UNITY_IOS
			//	SoundController.sound.audioSource.mute = false;
			//	MusicController.music.audioSource.mute = false;
			//	#endif
			//	//Debug.Log ("Interstitial ad loaded.");
			//});
			//interstitialAd.InterstitialAdDidFailWithError = (delegate(string error) {
			//	inadsStatus = 2;
			//	AdsControl.instance.texttt.text = error;
			//	//Debug.Log ("Interstitial ad failed to load with error: " + error);
			//});
			//interstitialAd.InterstitialAdWillLogImpression = (delegate() {
			//	//Debug.Log ("Interstitial ad logged impression.");
			//});
			//interstitialAd.InterstitialAdDidClick = (delegate() {
			//	//Debug.Log ("Interstitial ad clicked.");
			//});
		
			//// Initiate the request to load the ad.
			//this.interstitialAd.LoadAd ();
		}
	
		//// Show button
		//public void ShowInterstitial ()
		//{
		//	if (this.isLoaded == true) {
		//		this.interstitialAd.Show ();
		//		this.isLoaded = false;
		//		#if UNITY_IOS
		//		SoundController.sound.audioSource.mute = true;
		//		MusicController.music.audioSource.mute = true;
		//		#endif
		//	}
		//}

		//public bool isHasInads()
		//{
		//	return this.isLoaded;
		//}

		//public void ClearInadsFacebook ()
		//{
		//	// Dispose of interstitial ad when the scene is destroyed
		//	if (this.interstitialAd != null) {
		//		this.interstitialAd.Dispose ();
		//	}
		//}
	}
}