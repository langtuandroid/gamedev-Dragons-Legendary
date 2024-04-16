using System;
using UnityEngine;
using UnityEngine.UI;

public class EnergyTimeChecker : MonoBehaviour
{
	private const string USER_ENERGY_TIMER_KEY = "EnergyTimerKey";

	[SerializeField]
	private GameObject goEnergyInfo;

	[SerializeField]
	private GameObject goEnergyTimer;

	[SerializeField]
	private Text textEnergyTimer;

	public void Init()
	{
		LocalTimeCheckManager.InitType("EnergyTimerKey");
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Combine(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Combine(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
		CheckUserEnergy();
	}

	public void Refresh()
	{
		CheckUserEnergy();
	}

	public void Exit()
	{
		StopAllCoroutines();
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Remove(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeTickEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Remove(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnLocalTimeComplete));
		LocalTimeCheckManager.TimeClear("EnergyTimerKey");
		LocalTimeCheckManager.SaveAndExit("EnergyTimerKey");
	}

	private void CheckUserEnergy()
	{
		if (!(LocalTimeCheckManager.GetSecond("EnergyTimerKey") > 0.0))
		{
			StopAllCoroutines();
			if (GameInfo.userData.userInfo.energy < GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).maxEnergy && GameInfo.userData.userInfo.energyRemainTime > 0)
			{
				LocalTimeCheckManager.TimeClear("EnergyTimerKey");
				LocalTimeCheckManager.AddTimer("EnergyTimerKey", GameInfo.userData.userInfo.energyRemainTime);
			}
			else
			{
				textEnergyTimer.text = "max";
			}
		}
	}

	private void OnTimeTickEvent(string type, float second)
	{
		if (type == "EnergyTimerKey")
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(second);
			string text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			textEnergyTimer.text = text;
		}
	}

	private void OnLocalTimeComplete(string type)
	{
		LocalTimeCheckManager.TimeClear("EnergyTimerKey");
		Protocol_Set.Protocol_user_default_info_Req(null, isLoading: false);
	}

	private void OnDisable()
	{
		Exit();
	}
}
