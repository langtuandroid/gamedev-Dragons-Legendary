using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StaminaInfo : LobbyPopupBase
{
	public Action OnStaminaClose;

	[FormerlySerializedAs("textChargeEnergyForJewel")] [SerializeField]
	private Text _textChargeEnergy;

	[FormerlySerializedAs("textChargeEnergyForAds")] [SerializeField]
	private Text _textAdsEnergy;

	[FormerlySerializedAs("textChargePurchaseJewel")] [SerializeField]
	private Text _purchaseJevelText;

	[FormerlySerializedAs("goAdCover")] [SerializeField]
	private GameObject _coverAd;

	[FormerlySerializedAs("goAdText")] [SerializeField]
	private GameObject _adTextObject;

	[FormerlySerializedAs("trEnergyChargeForJewel")] [SerializeField]
	private Transform _energyChargeJevel;

	public override void Open()
	{
		base.Open();
		Construct();
		LobbyManager.HideHunterLobby();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Complete()
	{
	}

	private void Construct()
	{
		_textChargeEnergy.text = $"{GameDataManager.GetGameConfigData(ConfigDataType.EnergyPackEnergyNumber)}";
		_purchaseJevelText.text = $"{GameDataManager.GetGameConfigData(ConfigDataType.EnergyPackPriceJewel)}";
		_textAdsEnergy.text = $"{GameInfo.chargeEnergyAdsValue}";
		_coverAd.SetActive(GameInfo.userData.userInfo.ad_energy_limit <= 0 || GameInfo.userData.userInfo.energy >= GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).maxEnergy);
		_adTextObject.SetActive(GameInfo.userData.userInfo.ad_energy_limit <= 0);
	}

	private void OnChargeComplete()
	{
		StartCoroutine(_energyEffect(_energyChargeJevel));
		if (OnStaminaClose != null)
		{
			OnStaminaClose();
		}
	}

	private void RevardedDone()
	{
		UnityEngine.Debug.Log("Reward Video Complete !!");
		if (true)
		{
			Protocol_Set.Protocol_shop_ad_energy_Req(GameInfo.chargeEnergyAdsValue, EnergyResponce);
		}
	}

	private void EnergyResponce()
	{
		UnityEngine.Debug.Log("GetEnergy Complete !!");
		StartCoroutine(_energyEffect(_coverAd.transform));
		_coverAd.SetActive(GameInfo.userData.userInfo.ad_energy_limit <= 0 || GameInfo.userData.userInfo.energy >= GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).maxEnergy);
		_adTextObject.SetActive(GameInfo.userData.userInfo.ad_energy_limit <= 0);
	}

	private IEnumerator _energyEffect(Transform trStart)
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
			Protocol_Set.Protocol_shop_buy_energy_Req(OnChargeComplete);
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
		if (OnStaminaClose != null)
		{
			OnStaminaClose();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
}
