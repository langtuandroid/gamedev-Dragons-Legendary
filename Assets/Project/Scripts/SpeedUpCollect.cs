using System;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUpCollect : LobbyPopupBase
{
	public Action GoBackEvent;

	private Action<bool> SpeedUpResult;

	[SerializeField]
	private Text textRemainDuration;

	[SerializeField]
	private Text textNeedJewel;

	[SerializeField]
	private Text textGetCoin;

	private float second;

	private int needJewel;

	private int getCoin;

	public void Show(float _second, int _needJewel, int _getCoin, Action<bool> callBack)
	{
		base.Open();
		second = _second;
		needJewel = _needJewel;
		getCoin = _getCoin;
		SpeedUpResult = callBack;
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
		float num = second / 60f;
		if (num < 0f)
		{
			num = 1f;
		}
		textRemainDuration.text = string.Format(MWLocalize.GetData("popup_lack_of_time_title"), Mathf.Floor(num));
		textGetCoin.text = $"{getCoin:#,###}";
		textNeedJewel.text = $"{needJewel}";
	}

	public void OnClickSpeedUp()
	{
		if (SpeedUpResult != null)
		{
			SpeedUpResult(obj: true);
		}
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}

	public void OnClickCancel()
	{
		if (SpeedUpResult != null)
		{
			SpeedUpResult(obj: false);
		}
		if (GoBackEvent != null)
		{
			GoBackEvent();
		}
		SoundController.EffectSound_Play(EffectSoundType.Cancel);
	}
}
