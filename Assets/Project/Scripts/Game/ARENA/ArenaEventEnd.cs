using System;
using UnityEngine;
using UnityEngine.UI;

public class ArenaEventEnd : LobbyPopupBase
{
	public Action GoBackEvent;

	[SerializeField]
	private Text textLevel;

	[SerializeField]
	private Text textGetPoint;

	[SerializeField]
	private Text textNextPoint;

	private ARENA_REWARD_INFO rewardInfoData;

	public void Show(ARENA_REWARD_INFO _data)
	{
		rewardInfoData = _data;
		Init();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void CloseProcessComplete()
	{
	}

	private void Init()
	{
		if (rewardInfoData.rewardYn == "y")
		{
			base.Open();
			textLevel.text = string.Format(MasterLocalize.GetData("arena_lobby_text_03"), "<color=#FCF13E>" + rewardInfoData.arenaLevel + "</color>/10");
			textGetPoint.text = $"+{rewardInfoData.arenaPoint}";
			textNextPoint.text = $"{rewardInfoData.nextArenaPoint}";
		}
	}

	public void OnClickCollect()
	{
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
	}
}
