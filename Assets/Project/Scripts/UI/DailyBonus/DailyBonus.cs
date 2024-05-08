using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyBonus : LobbyPopupBase
{
	public Action GoBackEvent;

	public Action RewardResultComplete;

	private const float DAILY_BONUS_EFFECT_TIME = 1.2f;

	private const float DAILY_BONUS_EFFECT_DELAY_SPAWN_DURATION = 0.4f;

	[SerializeField]
	private DailyBonusItem[] arrDailyItem = new DailyBonusItem[0];

	[SerializeField]
	private GameObject goChestResult;

	private USER_DAILY_BONUS_RESULT dailyBonusResult;

	private USER_DAILY_BONUS_DATA currentDailyData;

	private DailyBonusItem currentDailyItem;

	public override void Open()
	{
		Protocol_Set.Protocol_user_daily_bonus_Req(OnDailyBonusConnectComplete);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	public void RewardResultEnd()
	{
		if (RewardResultComplete != null)
		{
			RewardResultComplete();
		}
	}

	private void Init()
	{
		goChestResult.SetActive(value: false);
		for (int i = 0; i < arrDailyItem.Length; i++)
		{
			arrDailyItem[i].Day = i + 1;
			arrDailyItem[i].Init(dailyBonusResult.dailyBonusList[i]);
			if (dailyBonusResult.dailyBonusList[i].accepted == "today")
			{
				currentDailyData = dailyBonusResult.dailyBonusList[i];
				currentDailyItem = arrDailyItem[i];
			}
		}
	}

	private void ShowRewardResult()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		string type = currentDailyData.type;
		if (type == null)
		{
			return;
		}
		if (!(type == "coin") && !(type == "jewel") && !(type == "badge") && !(type == "token"))
		{
			if (type == "hunter")
			{
				LobbyManager.OpenChestResultDone = (Action)Delegate.Combine(LobbyManager.OpenChestResultDone, new Action(OnChestHunterResult));
				LobbyManager.ShowChestOpen(new List<ChestListDbData>(dailyBonusResult.rewardList), ChestType.Mysterious);
			}
		}
		else
		{
			ShowChestRewardPopup();
		}
	}

	private void ShowChestRewardPopup()
	{
		LobbyManager.ShowRewardResult(dailyBonusResult.rewardList);
	}

	private void OnDailyBonusConnectComplete(USER_DAILY_BONUS_RESULT _result)
	{
		dailyBonusResult = _result;
		if (dailyBonusResult == null || dailyBonusResult.dailyBonusList == null)
		{
			if (GoBackEvent != null)
			{
				GoBackEvent();
			}
			RewardResultEnd();
		}
		else
		{
			base.Open();
			Init();
		}
	}

	private void OnDailyBonusCollect()
	{
		ShowRewardResult();
	}

	private void OnChestHunterResult()
	{
		RewardResultEnd();
		LobbyManager.OpenChestResultDone = (Action)Delegate.Remove(LobbyManager.OpenChestResultDone, new Action(OnChestHunterResult));
	}

	public void OnClickCollect()
	{
		Protocol_Set.Protocol_user_get_daily_bonus_Req(currentDailyData.dayCnt, OnDailyBonusCollect);
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickRewardCollectDone()
	{
		goChestResult.SetActive(value: false);
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}
}
