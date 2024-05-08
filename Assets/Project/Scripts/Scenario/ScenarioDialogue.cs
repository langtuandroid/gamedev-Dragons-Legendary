using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioDialogue : MonoBehaviour
{
	private const float ACTIVE_DIALOGUE_CHARACTER_POS_X = 0.25f;

	private const float ACTIVE_DIALOGUE_CHARACTER_SCALE_SIZE = 1.8f;

	private const float CHAR_SCALE_CHANGE_SPEED = 0.2f;

	[SerializeField]
	private Text textLeftName;

	[SerializeField]
	private Text textRightName;

	[SerializeField]
	private Text textMessage;

	[SerializeField]
	private Transform trLeftCharacterAnchor;

	[SerializeField]
	private Transform trRightCharacterAnchor;

	[SerializeField]
	private GameObject goLeftName;

	[SerializeField]
	private GameObject goRightName;

	[SerializeField]
	private GameObject goBackFlipBg;

	[SerializeField]
	private GameObject goFrontFlipBg;

	[SerializeField]
	private GameObject goNextArrow;

	private bool isMessageFlow;

	private string activeCharacterName = string.Empty;

	private Vector3 flipScale;

	private Coroutine coroutineMessage;

	private Transform trLeftChar;

	private Transform trRightChar;

	private ScenarioDbData scenarioDbData;

	public bool MessageFlow => isMessageFlow;

	public void Show(ScenarioDbData _data)
	{
		base.gameObject.SetActive(value: true);
		scenarioDbData = _data;
		if (scenarioDbData.characterLocation == 1)
		{
			ShowLeft();
			StopMessageCoroutine();
			coroutineMessage = StartCoroutine(ProcessShowMessage());
		}
		else if (scenarioDbData.characterLocation == 2)
		{
			ShowRight();
			StopMessageCoroutine();
			coroutineMessage = StartCoroutine(ProcessShowMessage());
		}
		else
		{
			Hide();
		}
	}

	public void Hide()
	{
		Clear();
		StopMessageCoroutine();
		base.gameObject.SetActive(value: false);
	}

	public void MessageComplete()
	{
		SoundController.EffectSound_Stop(EffectSoundType.DialogueText);
		goNextArrow.SetActive(value: true);
		StopMessageCoroutine();
		isMessageFlow = false;
		textMessage.text = MasterLocalize.GetData(scenarioDbData.text);
	}

	public void Clear()
	{
		if (trLeftChar != null)
		{
			MasterPoolManager.ReturnToPool("Scenario", trLeftChar);
			trLeftChar = null;
		}
		if (trRightChar != null)
		{
			MasterPoolManager.ReturnToPool("Scenario", trRightChar);
			trRightChar = null;
		}
		activeCharacterName = string.Empty;
		SoundController.EffectSound_Stop(EffectSoundType.DialogueText);
	}

	private void ShowLeft()
	{
		if (!(MasterLocalize.GetData(scenarioDbData.characterName) == activeCharacterName))
		{
			if (trLeftChar != null)
			{
				LeanTween.cancel(trLeftChar.gameObject);
			}
			if (trRightChar != null)
			{
				LeanTween.cancel(trRightChar.gameObject);
			}
			goLeftName.SetActive(value: true);
			goRightName.SetActive(value: false);
			activeCharacterName = MasterLocalize.GetData(scenarioDbData.characterName);
			textLeftName.text = activeCharacterName;
			flipScale = goFrontFlipBg.transform.localScale;
			flipScale.x = -1f;
			goFrontFlipBg.transform.localScale = flipScale;
			if (trLeftChar != null)
			{
				MasterPoolManager.ReturnToPool("Scenario", trLeftChar);
				trLeftChar = null;
			}
			if (trRightChar != null)
			{
				LeanTween.moveLocalX(trRightChar.gameObject, 0f, 0.2f).setEaseOutCubic();
				LeanTween.scale(trRightChar.gameObject, Vector3.one, 0.2f).setEaseOutCubic();
				trRightChar.GetComponent<ScenarioCharacter>().Dimmed();
			}
			string characterName = GetCharacterName(scenarioDbData.characterName);
			if (characterName != string.Empty)
			{
				trLeftChar = MasterPoolManager.SpawnObject("Scenario", GetCharacterName(scenarioDbData.characterName), trLeftCharacterAnchor);
				trLeftChar.GetComponent<ScenarioCharacter>().Talk();
				LeanTween.moveLocalX(trLeftChar.gameObject, 0.25f, 0.2f).setEaseOutCubic();
				LeanTween.scale(trLeftChar.gameObject, new Vector3(1.8f, 1.8f), 0.2f).setEaseOutCubic();
			}
		}
	}

	private void ShowRight()
	{
		if (!(MasterLocalize.GetData(scenarioDbData.characterName) == activeCharacterName))
		{
			if (trLeftChar != null)
			{
				LeanTween.cancel(trLeftChar.gameObject);
			}
			if (trRightChar != null)
			{
				LeanTween.cancel(trRightChar.gameObject);
			}
			goLeftName.SetActive(value: false);
			goRightName.SetActive(value: true);
			activeCharacterName = MasterLocalize.GetData(scenarioDbData.characterName);
			textRightName.text = activeCharacterName;
			flipScale = goFrontFlipBg.transform.localScale;
			flipScale.x = 1f;
			goFrontFlipBg.transform.localScale = flipScale;
			if (trRightChar != null)
			{
				MasterPoolManager.ReturnToPool("Scenario", trRightChar);
				trRightChar = null;
			}
			if (trLeftChar != null)
			{
				LeanTween.moveLocalX(trLeftChar.gameObject, 0f, 0.2f).setEaseOutCubic();
				LeanTween.scale(trLeftChar.gameObject, Vector3.one, 0.2f).setEaseOutCubic();
				trLeftChar.GetComponent<ScenarioCharacter>().Dimmed();
			}
			string characterName = GetCharacterName(scenarioDbData.characterName);
			if (characterName != string.Empty)
			{
				trRightChar = MasterPoolManager.SpawnObject("Scenario", GetCharacterName(scenarioDbData.characterName), trRightCharacterAnchor);
				trRightChar.GetComponent<ScenarioCharacter>().Talk();
				LeanTween.moveLocalX(trRightChar.gameObject, 0.25f, 0.2f).setEaseOutCubic();
				LeanTween.scale(trRightChar.gameObject, new Vector3(1.8f, 1.8f), 0.2f).setEaseOutCubic();
			}
		}
	}

	private IEnumerator ProcessShowMessage()
	{
		textMessage.text = string.Empty;
		isMessageFlow = true;
		goNextArrow.SetActive(value: false);
		int activeCount = 1;
		string strMessage = MasterLocalize.GetData(scenarioDbData.text);
		SoundController.EffectSound_Play(EffectSoundType.DialogueText);
		while (activeCount < strMessage.Length)
		{
			textMessage.text = strMessage.Substring(0, activeCount);
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

	private string GetCharacterName(string _name)
	{
		switch (_name)
		{
		case "character_name_merlin":
			return "Storymode_Merlin";
		case "character_name_1st_prince":
			return "Storymode_BigBrother";
		case "character_name_2nd_prince":
			return "Storymode_Brother";
		case "character_name_royal_knight(w)":
			return "Storymode_Paladin";
		case "character_name_royal_knight(m)":
			return "Storymode_Knight";
		case "character_name_alice":
			return "Storymode_Archer";
		case "character_name_peter":
			return "Storymode_Guard";
		case "character_name_musketeer":
			return "Storymode_Musketeer";
		case "character_name_robin":
			return "Storymode_Thief";
		case "character_name_elizabeth":
			return "Storymode_Friest";
		default:
			return string.Empty;
		}
	}

	private void OnDisable()
	{
		activeCharacterName = string.Empty;
	}
}
