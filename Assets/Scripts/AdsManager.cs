using System;

public class AdsManager : GameObjectSingleton<AdsManager>
{
	private Action onCallBack;

	private void ShowRewardVideo(Action _onCallBack = null)
	{
		GameObjectSingleton<AdsManager>.Inst.onCallBack = _onCallBack;
		AdNetworkManager.PlayReward(_onCallBack);
	}

	private void ShowInterstitial()
	{
		AdNetworkManager.PlayInterstitial();
	}

	private void ShowRequestRewardVideo()
	{
	}

	private void ShowRequestInterstitial()
	{
	}

	private void ShowBanner()
	{
	}

	private void HideBanner()
	{
	}

	private void RequestRewardVideo(Action _onCallBack = null)
	{
	}

	private void RequestInterstitial(Action _onCallBack = null)
	{
	}

	private void SetForceRewardVideoCallBack(Action _onCallBack)
	{
		GameObjectSingleton<AdsManager>.Inst.onCallBack = _onCallBack;
	}

	private void SetForceInterstitialCallBack()
	{
	}

	public static void RewardVideo_Show(Action _onCallBack)
	{
		GameObjectSingleton<AdsManager>.Inst.ShowRewardVideo(_onCallBack);
	}

	public static void Interstitial_Show()
	{
		GameObjectSingleton<AdsManager>.Inst.ShowInterstitial();
	}

	public static void RequestRewardVideo()
	{
		AdNetworkManager.RequestRewardVideo();
	}
}
