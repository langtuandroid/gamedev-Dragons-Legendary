using UnityEngine;
using UnityEngine.UI;

public class ArenaLobbyTopUI : MonoBehaviour
{
	private const string TIME_CHECK_ENERGY_KEY = "UserEnergyTimer";

	private const float ONE_ENERGY_DURATION = 5f;

	private const float EXP_MAX_GAUGE_VALUE = 134f;

	[SerializeField]
	private Text textUserLevel;

	[SerializeField]
	private Text textUserExp;

	[SerializeField]
	private Text textUserJewel;

	[SerializeField]
	private Text textUserArenaTicket;

	[SerializeField]
	private Text textUserArenaPoint;

	[SerializeField]
	private RectTransform rtExpGauge;

	[SerializeField]
	private Transform trUserLevel;

	[SerializeField]
	private Transform trUserJewel;

	[SerializeField]
	private Transform trUserTicket;

	private float userExp;

	private float userJewel;

	private float userArenaPoint;

	private float userArenaTicket;

	private Vector2 expGaugeOffsetMax;

	public Vector3 UserTicketPosition => trUserTicket.position;

	public void RefreshData(bool isCount = false)
	{
		UserLevelDbData userLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level);
		textUserLevel.text = $"{userLevelData.level}";
		if (isCount && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(GameUtil.ProcessCountNumber(textUserExp, userExp, GameInfo.userData.userInfo.exp, $"/{userLevelData.exp}"));
			StartCoroutine(GameUtil.ProcessCountNumber(textUserJewel, float.Parse(textUserJewel.text), GameInfo.userData.userInfo.jewel, string.Empty));
			StartCoroutine(GameUtil.ProcessCountNumber(textUserArenaPoint, float.Parse(textUserArenaPoint.text), GameInfo.userData.userInfo.arenaPoint, string.Empty));
			textUserArenaTicket.text = $"{GameInfo.userData.userInfo.arenaTicket}/<color=#FFBA00>{GameDataManager.GetGameConfigData(ConfigDataType.User_arena_ticket_reset)}</color>";
		}
		else
		{
			textUserExp.text = $"{GameInfo.userData.userInfo.exp}/{userLevelData.exp}";
			textUserJewel.text = $"{GameInfo.userData.userInfo.jewel}";
			textUserArenaPoint.text = $"{GameInfo.userData.userInfo.arenaPoint}";
			textUserArenaTicket.text = $"{GameInfo.userData.userInfo.arenaTicket}/<color=#FFBA00>{GameDataManager.GetGameConfigData(ConfigDataType.User_arena_ticket_reset)}</color>";
		}
		userJewel = GameInfo.userData.userInfo.jewel;
		userExp = GameInfo.userData.userInfo.exp;
		userArenaPoint = GameInfo.userData.userInfo.arenaPoint;
		userArenaTicket = GameInfo.userData.userInfo.arenaTicket;
		expGaugeOffsetMax = rtExpGauge.offsetMax;
		expGaugeOffsetMax.x = (float)GameInfo.userData.userInfo.exp / (float)userLevelData.exp * 134f;
		rtExpGauge.offsetMax = expGaugeOffsetMax;
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
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
}
