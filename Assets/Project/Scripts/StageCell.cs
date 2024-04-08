using System;
using UnityEngine;
using UnityEngine.UI;

public class StageCell : MonoBehaviour
{
	public Action<int> SelectStageEvent;

	[SerializeField]
	private Image imageStage;

	[SerializeField]
	private Text textStageName;

	[SerializeField]
	private Text textStageClearRate;

	[SerializeField]
	private GameObject goLock;

	[SerializeField]
	private GameObject goCommingSoon;

	[SerializeField]
	private GameObject goSelectButton;

	[SerializeField]
	private GameObject goLockButton;

	private bool isTouch;

	private int stageId;

	public Transform SelectButton => goSelectButton.transform;

	public void SetStageId(int id)
	{
		stageId = id;
		goLock.SetActive(value: false);
		goCommingSoon.SetActive(value: false);
		isTouch = true;
		goSelectButton.SetActive(value: true);
		goLockButton.SetActive(value: false);
	}

	public void SetStageImage(Sprite sprite)
	{
		imageStage.sprite = sprite;
	}

	public void SetStageName(string stageName)
	{
		UnityEngine.Debug.Log("SetStageName - " + stageName);
		textStageName.text = MWLocalize.GetData(stageName);
	}

	public void SetStageClearRate(int rate)
	{
		textStageClearRate.text = string.Format(MWLocalize.GetData("stage_clear_completeion_rate_text"), rate);
	}

	public void Lock()
	{
		goLock.SetActive(value: true);
		isTouch = false;
		goSelectButton.SetActive(value: false);
		goLockButton.SetActive(value: true);
	}

	public void CommingSoon()
	{
		imageStage.sprite = null;
		goCommingSoon.SetActive(value: true);
		isTouch = false;
		goSelectButton.SetActive(value: false);
		goLockButton.SetActive(value: false);
	}

	public void SetForceTouch()
	{
		isTouch = true;
	}

	public void OnClickStageSelect()
	{
		UnityEngine.Debug.Log("OnClickStageSelect");
		if (SelectStageEvent != null && isTouch)
		{
			SelectStageEvent(stageId);
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
	}
}
