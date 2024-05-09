using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StaminaTimeChecker : MonoBehaviour
{
	[FormerlySerializedAs("goEnergyInfo")] [SerializeField]
	private GameObject _energyInfo;

	[FormerlySerializedAs("goEnergyTimer")] [SerializeField]
	private GameObject _energyTimer;

	[FormerlySerializedAs("textEnergyTimer")] [SerializeField]
	private Text _textEnergyTime;

	public void Construct()
	{
		LocalTimeCheckManager.InitType("EnergyTimerKey");
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Combine(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Combine(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnTimeComplete));
		CheckEnergyDa();
	}

	public void Reload()
	{
		CheckEnergyDa();
	}

	public void Remove()
	{
		StopAllCoroutines();
		LocalTimeCheckManager.OnTimeTick = (Action<string, float>)Delegate.Remove(LocalTimeCheckManager.OnTimeTick, new Action<string, float>(OnTimeEvent));
		LocalTimeCheckManager.OnLocalTimeComplete = (Action<string>)Delegate.Remove(LocalTimeCheckManager.OnLocalTimeComplete, new Action<string>(OnTimeComplete));
		LocalTimeCheckManager.TimeClear("EnergyTimerKey");
		LocalTimeCheckManager.SaveAndExit("EnergyTimerKey");
	}

	private void CheckEnergyDa()
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
				_textEnergyTime.text = "max";
			}
		}
	}

	private void OnTimeEvent(string type, float second)
	{
		if (type == "EnergyTimerKey")
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(second);
			string text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			_textEnergyTime.text = text;
		}
	}

	private void OnTimeComplete(string type)
	{
		LocalTimeCheckManager.TimeClear("EnergyTimerKey");
		Protocol_Set.Protocol_user_default_info_Req(null, isLoading: false);
	}

	private void OnDisable()
	{
		Remove();
	}
}
