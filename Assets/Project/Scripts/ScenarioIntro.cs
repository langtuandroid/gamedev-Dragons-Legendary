using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioIntro : MonoBehaviour
{
	[SerializeField]
	private Text textIntroMessage;

	[SerializeField]
	private GameObject goNextArrow;

	private bool isMessageFlow;

	private ScenarioDbData scenarioDbData;

	private Coroutine coroutineMessage;

	public bool MessageFlow => isMessageFlow;

	public void Show(ScenarioDbData _data)
	{
		base.gameObject.SetActive(value: true);
		scenarioDbData = _data;
		StopMessageCoroutine();
		coroutineMessage = StartCoroutine(ProcessShowMessage());
	}

	public void Hide()
	{
		StopMessageCoroutine();
		base.gameObject.SetActive(value: false);
	}

	public void MessageComplete()
	{
		SoundController.EffectSound_Stop(EffectSoundType.DialogueText);
		StopMessageCoroutine();
		isMessageFlow = false;
		goNextArrow.SetActive(value: true);
		textIntroMessage.text = MWLocalize.GetData(scenarioDbData.text);
	}

	private IEnumerator ProcessShowMessage()
	{
		textIntroMessage.text = string.Empty;
		isMessageFlow = true;
		goNextArrow.SetActive(value: false);
		int activeCount = 1;
		string strMessage = MWLocalize.GetData(scenarioDbData.text);
		SoundController.EffectSound_Play(EffectSoundType.DialogueText);
		while (activeCount < strMessage.Length)
		{
			textIntroMessage.text = strMessage.Substring(0, activeCount);
			activeCount++;
			yield return new WaitForSeconds(0.1f);
		}
		MessageComplete();
	}

	private void StopMessageCoroutine()
	{
		if (coroutineMessage != null)
		{
			StopCoroutine(coroutineMessage);
			coroutineMessage = null;
		}
	}
}
