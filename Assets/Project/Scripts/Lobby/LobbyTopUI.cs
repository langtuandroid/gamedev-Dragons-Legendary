using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTopUI : MonoBehaviour
{
	private const string TIME_CHECK_ENERGY_KEY = "UserEnergyTimer";

	private const float ONE_ENERGY_DURATION = 5f;

	private const float EXP_MAX_GAUGE_VALUE = 134f;

	[SerializeField]
	private Text textUserLevel;

	[SerializeField]
	private Text textUserExp;

	[SerializeField]
	private Text textUserEnergy;

	[SerializeField]
	private Text textUserCoin;

	[SerializeField]
	private Text textUserJewel;

	[SerializeField]
	private RectTransform rtExpGauge;

	[SerializeField]
	private Transform trUserLevel;

	[SerializeField]
	private Transform trUserCoin;

	[SerializeField]
	private Transform trUserEnergy;

	[SerializeField]
	private Transform trUserJewel;

	private float userCoin;

	private float userExp;

	private float userEnergy;

	private float userJewel;

	private Vector2 expGaugeOffsetMax;

	private EnergyTimeChecker timeChecker;

	public Vector3 UserLevelPosition => trUserLevel.position;

	public Vector3 UserCoinPosition => trUserCoin.position;

	public Vector3 UserEnergyPosition => trUserEnergy.position;

	public Vector3 UserJewelPosition => trUserJewel.position;

	public void Init()
	{
		timeChecker.Init();
	}

	public void RefreshData(bool isCount = false)
	{
		UserLevelDbData userLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level);
		if (isCount && base.gameObject.activeSelf)
		{
			StartCoroutine(GameUtil.ProcessCountNumber(textUserExp, userExp, GameInfo.userData.userInfo.exp, $"/{userLevelData.exp}"));
			StartCoroutine(GameUtil.ProcessCountNumber(textUserEnergy, userEnergy, GameInfo.userData.userInfo.energy, $"/{userLevelData.maxEnergy}"));
			StartCoroutine(GameUtil.ProcessCountNumber(textUserCoin, float.Parse(textUserCoin.text), GameInfo.userData.userInfo.coin, string.Empty));
			StartCoroutine(GameUtil.ProcessCountNumber(textUserJewel, float.Parse(textUserJewel.text), GameInfo.userData.userInfo.jewel, string.Empty));
		}
		else
		{
			textUserExp.text = $"{GameInfo.userData.userInfo.exp}/{userLevelData.exp}";
			textUserEnergy.text = $"{GameInfo.userData.userInfo.energy}/{userLevelData.maxEnergy}";
			textUserCoin.text = $"{GameInfo.userData.userInfo.coin}";
			textUserJewel.text = $"{GameInfo.userData.userInfo.jewel}";
		}
		textUserLevel.text = $"{userLevelData.level}";
		userCoin = GameInfo.userData.userInfo.coin;
		userJewel = GameInfo.userData.userInfo.jewel;
		userExp = GameInfo.userData.userInfo.exp;
		userEnergy = GameInfo.userData.userInfo.energy;
		UserLevelDbData userLevelData2 = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level + 1);
		if (userLevelData2 == null)
		{
			expGaugeOffsetMax = rtExpGauge.offsetMax;
			expGaugeOffsetMax.x = (float)(userLevelData.exp / userLevelData.exp) * 134f;
			rtExpGauge.offsetMax = expGaugeOffsetMax;
			textUserExp.text = "max";
		}
		else
		{
			expGaugeOffsetMax = rtExpGauge.offsetMax;
			expGaugeOffsetMax.x = (float)GameInfo.userData.userInfo.exp / (float)userLevelData.exp * 134f;
			rtExpGauge.offsetMax = expGaugeOffsetMax;
		}
		timeChecker.Refresh();
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Exit()
	{
		timeChecker.Exit();
	}

	private void OnTimeTickEvent(string type, float second)
	{
		if (type == "UserEnergyTimer")
		{
			float num = second % 5f;
			UnityEngine.Debug.Log("currentTime :: " + num + " / " + second + " / " + Mathf.Round(second / 5f));
		}
	}

	private void OnLocalTimeComplete(string type)
	{
	}

	private void ShopListResponse_Coin()
	{
		LobbyManager.ShowValueShop(ValueShopType.Coin);
	}

	private void ShopListResponse_Jewel()
	{
		LobbyManager.ShowValueShop(ValueShopType.Jewel);
	}

	public void OnClickEnergy()
	{
		LobbyManager.ShowUserEnergyInfo();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickCoin()
	{
		Protocol_Set.Protocol_shop_list_Req(ShopListResponse_Coin);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickJewel()
	{
		Protocol_Set.Protocol_shop_list_Req(ShopListResponse_Jewel);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickUserDataClear()
	{
		LobbyManager.ShowUserLevelInfo();
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	private void Awake()
	{
		timeChecker = base.gameObject.GetComponent<EnergyTimeChecker>();
	}

	private void OnDestroy()
	{
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Remove(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Remove(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
		LocalTimeCheckManager.SaveAndExit("UserEnergyTimer");
	}
}
