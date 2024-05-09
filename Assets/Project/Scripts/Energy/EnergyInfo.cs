using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnergyInfo : LobbyPopupBase
{
	public Action GoBackEvent;

	private const float STORE_ENERGY_GET_EFFECT_TIME = 1.2f;

	private const float STORE_ENERGY_GET_EFFECT_DELAY_SPAWN_DURATION = 0.4f;

	[SerializeField]
	private Text textChargeEnergyForJewel;

	[SerializeField]
	private Text textChargeEnergyForAds;

	[SerializeField]
	private Text textChargePurchaseJewel;

	[SerializeField]
	private GameObject goAdCover;

	[SerializeField]
	private GameObject goAdText;

	[SerializeField]
	private Transform trEnergyChargeForJewel;

	public override void Open()
	{
		base.Open();
		Init();
		LobbyManager.HideHunterLobby();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Complete()
	{
	}

	private void Init()
	{
		textChargeEnergyForJewel.text = $"{GameDataManager.GetGameConfigData(ConfigDataType.EnergyPackEnergyNumber)}";
		textChargePurchaseJewel.text = $"{GameDataManager.GetGameConfigData(ConfigDataType.EnergyPackPriceJewel)}";
		textChargeEnergyForAds.text = $"{GameInfo.chargeEnergyAdsValue}";
		goAdCover.SetActive(GameInfo.userData.userInfo.ad_energy_limit <= 0 || GameInfo.userData.userInfo.energy >= GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).maxEnergy);
		goAdText.SetActive(GameInfo.userData.userInfo.ad_energy_limit <= 0);
	}

	private void OnEnergyChargeJewelComplete()
	{
		StartCoroutine(ShowEnergyEffect(trEnergyChargeForJewel));
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}

	private void RewardVideoComplete()
	{
		UnityEngine.Debug.Log("Reward Video Complete !!");
		if (true)
		{
			Protocol_Set.Protocol_shop_ad_energy_Req(GameInfo.chargeEnergyAdsValue, GetAdEnergyResponse);
		}
	}

	private void GetAdEnergyResponse()
	{
		UnityEngine.Debug.Log("GetEnergy Complete !!");
		StartCoroutine(ShowEnergyEffect(goAdCover.transform));
		goAdCover.SetActive(GameInfo.userData.userInfo.ad_energy_limit <= 0 || GameInfo.userData.userInfo.energy >= GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).maxEnergy);
		goAdText.SetActive(GameInfo.userData.userInfo.ad_energy_limit <= 0);
	}

	private IEnumerator ShowEnergyEffect(Transform trStart)
	{
		int energyCount = UnityEngine.Random.Range(4, 8);
		for (int i = 0; i < energyCount; i++)
		{
			SoundController.EffectSound_Play(EffectSoundType.GetEnergy);
			float num = UnityEngine.Random.Range(-0.2f, -0.1f);
			Transform transform = MasterPoolManager.SpawnObject("Effect", "FX_Energy_get", null, 1.2f + num + 0.4f);
			transform.position = trStart.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
			GameObject gameObject = transform.gameObject;
			Vector3 userEnergyPosition = LobbyManager.UserEnergyPosition;
			LeanTween.moveX(gameObject, userEnergyPosition.x, 1.2f + num).setEaseInCubic();
			GameObject gameObject2 = transform.gameObject;
			Vector3 userEnergyPosition2 = LobbyManager.UserEnergyPosition;
			LeanTween.moveY(gameObject2, userEnergyPosition2.y, 1.2f + num);
		}
		yield return new WaitForSeconds(1.2f);
		GameDataManager.UpdateUserData();
	}

	public void OnClickChargeEnergyForJewel()
	{
		if (GameInfo.userData.userInfo.jewel >= GameDataManager.GetGameConfigData(ConfigDataType.EnergyPackPriceJewel))
		{
			Protocol_Set.Protocol_shop_buy_energy_Req(OnEnergyChargeJewelComplete);
		}
		else
		{
			LobbyManager.ShowNotEnoughJewel(GameDataManager.GetGameConfigData(ConfigDataType.EnergyPackPriceJewel) - GameInfo.userData.userInfo.jewel);
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickChargeEnergyForAds()
	{
		Protocol_Set.Protocol_shop_ad_energy_start_Req();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
}
