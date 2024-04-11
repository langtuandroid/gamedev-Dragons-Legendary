using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : LobbyPopupBase
{
	public Action GoBackEvent;

	private const string USER_ATTACK_BOOST_INFO = "+{0}% ATTACK BOUNS";

	private const float USER_EXP_MAX_GAUGE_VALUE = 444f;

	[SerializeField]
	private Text textUserLevel;

	[SerializeField]
	private Text textUserExp;

	[SerializeField]
	private Text textUserAttackBoost;

	[SerializeField]
	private Text textMaxLevelUp;

	[SerializeField]
	private RectTransform rtUserExpGauge;

	[SerializeField]
	private Transform trItemListAnchor;

	private Vector2 userExpGaugeOffsetMax;

	private Vector3 levelInfoItemSize = new Vector3(0.6f, 0.6f, 0.6f);

	private UserLevelDbData currentLevelData;

	private UserLevelDbData nextLevelData;

	public override void Show()
	{
		base.Show();
		Init();
		LobbyManager.HideHunterLobby();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void HideProcessComplete()
	{
	}

	private void Init()
	{
		currentLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level);
		nextLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level + 1);
		textUserLevel.text = $"{currentLevelData.level}";
		textUserExp.text = $"{GameInfo.userData.userInfo.exp}/{currentLevelData.exp}";
		textUserAttackBoost.text = string.Format(MWLocalize.GetData("popup_level_text_2"), currentLevelData.attackBonusAll * 100f);
		if (nextLevelData == null)
		{
			userExpGaugeOffsetMax = rtUserExpGauge.offsetMax;
			userExpGaugeOffsetMax.x = (float)(currentLevelData.exp / currentLevelData.exp) * 444f;
			rtUserExpGauge.offsetMax = userExpGaugeOffsetMax;
			textUserExp.text = "max";
			textMaxLevelUp.gameObject.SetActive(value: true);
			textMaxLevelUp.text = string.Format("{0}", MWLocalize.GetData("popup_level_up_text_5"));
		}
		else
		{
			userExpGaugeOffsetMax = rtUserExpGauge.offsetMax;
			userExpGaugeOffsetMax.x = (float)GameInfo.userData.userInfo.exp / (float)currentLevelData.exp * 444f;
			rtUserExpGauge.offsetMax = userExpGaugeOffsetMax;
			textMaxLevelUp.gameObject.SetActive(value: false);
		}
		ShowNextLevelReward();
	}

	private void ShowNextLevelReward()
	{
		nextLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level + 1);
		if (nextLevelData != null)
		{
			RequiredItem_Cell component = MWPoolManager.Spawn("Grow", "cell_token", trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component.SetItemImg(50034, levelInfoItemSize);
			component.SetCostText(MWLocalize.GetData("common_text_2max"));
			component.SetClickType(ItemClickType.None);
			RequiredItem_Cell component2 = MWPoolManager.Spawn("Grow", "cell_token", trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component2.SetItemImg(50034, levelInfoItemSize);
			component2.SetCostText(MWLocalize.GetData("common_text_refill"));
			component2.SetClickType(ItemClickType.None);
			RequiredItem_Cell component3 = MWPoolManager.Spawn("Grow", "cell_token", trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component3.SetItemImg(50031, levelInfoItemSize);
			component3.SetCostText(MWLocalize.GetData("common_text_20jewel"));
			component3.SetClickType(ItemClickType.None);
			RequiredItem_Cell component4 = MWPoolManager.Spawn("Grow", "cell_token", trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component4.SetItemImg(50033, levelInfoItemSize);
			component4.SetCostText(MWLocalize.GetData("common_text_1key"));
			component4.SetClickType(ItemClickType.None);
			RequiredItem_Cell component5 = MWPoolManager.Spawn("Grow", "cell_token", trItemListAnchor).GetComponent<RequiredItem_Cell>();
			component5.SetItemImg(50042, levelInfoItemSize);
			component5.SetCostText(MWLocalize.GetData("common_text_5percent"));
			component5.SetClickType(ItemClickType.None);
		}
	}

	public void OnClickGoBack()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}

	private void OnDisable()
	{
		RequiredItem_Cell[] componentsInChildren = trItemListAnchor.GetComponentsInChildren<RequiredItem_Cell>();
		foreach (RequiredItem_Cell requiredItem_Cell in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Grow", requiredItem_Cell.transform);
		}
	}
}
