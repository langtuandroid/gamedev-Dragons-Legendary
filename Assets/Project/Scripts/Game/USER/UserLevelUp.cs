using System;
using UnityEngine;
using UnityEngine.UI;

public class UserLevelUp : GameObjectSingleton<UserLevelUp>
{
	public Action GoBackEvent;

	private const float LEVEL_GAUGE_MAX_VALUE = 444f;

	[SerializeField]
	private Text textUserLevel;

	[SerializeField]
	private Text textExp;

	[SerializeField]
	private Text textAttackBonus;

	[SerializeField]
	private Text textStaminaMax;

	[SerializeField]
	private Text textJewel;

	[SerializeField]
	private Text textKey;

	[SerializeField]
	private RectTransform rtExpGauge;

	private int level;

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		Init();
		SoundController.EffectSound_Play(EffectSoundType.UserLevelUp);
	}

	private void Init()
	{
		if (GameInfo.userData.userInfo.level == 1 && GoBackEvent != null)
		{
			GoBackEvent();
		}
		UserLevelDbData userLevelData = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level);
		UserLevelDbData userLevelData2 = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level - 1);
		level = userLevelData.level;
		textUserLevel.text = $"{userLevelData.level}";
		textExp.text = $"<color=#FCF13E>{GameInfo.userData.userInfo.exp}</color>/{userLevelData.exp}";
		textAttackBonus.text = string.Format(MWLocalize.GetData("popup_level_up_attack_bonus"), (int)Mathf.Round((userLevelData.attackBonusAll - userLevelData2.attackBonusAll) * 100f));
		textStaminaMax.text = string.Format(MWLocalize.GetData("popup_level_up_stamina_max"), userLevelData.maxEnergy - userLevelData2.maxEnergy);
		textJewel.text = string.Format(MWLocalize.GetData("popup_level_up_jewel"), GameDataManager.GetGameConfigData(ConfigDataType.User_levelup_get_jewel));
		textKey.text = string.Format(MWLocalize.GetData("popup_level_up_key"), GameDataManager.GetGameConfigData(ConfigDataType.User_levelup_get_key));
		Vector2 sizeDelta = rtExpGauge.sizeDelta;
		sizeDelta.x = (float)(GameInfo.userData.userInfo.exp / userLevelData.exp) * 444f;
		rtExpGauge.sizeDelta = sizeDelta;
	}

	public void OnClickGoBack()
	{
		if (level == 2)
		{
			AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.level_up_user);
		}
		base.gameObject.SetActive(value: false);
	}
}
