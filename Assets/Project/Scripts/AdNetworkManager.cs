using com.F4A.MobileThird;
using System;
using System.Collections;
using UnityEngine;
using F4A_Ads = com.F4A.MobileThird.AdsManager;

public class AdNetworkManager : MonoSingleton<AdNetworkManager>
{
	public Action OnRewardShowComplete;

	public bool isReward;

	public bool isForcePlay;

	private static AdNetworkManager instance;

	private void OnEnable()
	{
		F4A_Ads.OnRewardedAdLoaded += F4A_Ads_OnRewardedAdLoaded;
		F4A_Ads.OnRewardedAdCompleted += F4A_Ads_OnRewardedAdCompleted;
		F4A_Ads.OnRewardedAdSkiped += F4A_Ads_OnRewardedAdSkiped;
	}

	private void F4A_Ads_OnRewardedAdLoaded(ERewardedAdNetwork adNetwork)
	{
        if (instance.isForcePlay)
        {
            isForcePlay = false;
            PlayReward();
        }
    }

	private void F4A_Ads_OnRewardedAdSkiped(com.F4A.MobileThird.ERewardedAdNetwork adNetwork)
	{
	}

	private void F4A_Ads_OnRewardedAdCompleted(com.F4A.MobileThird.ERewardedAdNetwork adNetwork, string adName, double value)
	{
		OnRewardShowComplete?.Invoke();
		isReward = true;
    }

    private void OnDisable()
	{
		F4A_Ads.OnRewardedAdLoaded -= F4A_Ads_OnRewardedAdLoaded;
        F4A_Ads.OnRewardedAdCompleted -= F4A_Ads_OnRewardedAdCompleted;
        F4A_Ads.OnRewardedAdSkiped -= F4A_Ads_OnRewardedAdSkiped;
    }

    public static void PlayReward(Action _onCallback = null)
	{
		if (GameInfo.currentSceneType == SceneType.Lobby)
		{
			AnalyticsManager.RewardRequestAppEnvent("get_energy");
		}
		else
		{
			AnalyticsManager.RewardRequestAppEnvent("battle_reward_pick_more");
		}
        instance.OnRewardShowComplete = _onCallback;
        instance.isReward = false;
		if (instance.IsRwdAdLoaded())
		{
			instance.ShowRewardAd();
			return;
		}
		instance.isForcePlay = true;
	}

	public static void PlayInterstitial()
	{
	}

	public static void RequestRewardVideo()
	{
//		instance.RequestRewardBasedVideo();
	}

	private IEnumerator CheckRequestReward()
	{
		yield return new WaitForSeconds(0.5f);
		//RequestRewardBasedVideo();
	}

	public bool IsRwdAdLoaded()
	{
		return F4A_Ads.Instance.IsRewardAdsReady();
	}

	public void ShowRewardAd()
	{
		if (IsRwdAdLoaded())
		{
			F4A_Ads.Instance.ShowRewardAds();
		}
	}

	private new void Awake()
	{
		instance = this;
	}
	private void OnApplicationPause(bool pauseStatus)
	{
		UnityEngine.Debug.Log("pauseStatus = " + pauseStatus);
		if (!pauseStatus)
		{
			StartCoroutine(CheckRequestReward());
		}
	}
}
