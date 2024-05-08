using UnityEngine;
using UnityEngine.UI;

public class TutorialDialogue : MonoBehaviour
{
	public enum DialoguePosType
	{
		Top = 1,
		Middle,
		Bottom
	}

	private const float DIALOGUE_POS_TOP = 400f;

	private const float DIALOGUE_POS_MIDDLE = 0f;

	private const float DIALOGUE_POS_BOTTOM = -400f;

	[SerializeField]
	private Text textDialogue;

	[SerializeField]
	private GameObject goMerlin;

	[SerializeField]
	private GameObject goKnight;

	private DialoguePosType posType;

	private RectTransform rtDialogue;

	private Vector2 dialoguePosition;

	public void ShowDialogue(string message, DialoguePosType type, bool isMerlin = true)
	{
		base.gameObject.SetActive(value: true);
		dialoguePosition = rtDialogue.anchoredPosition;
		switch (type)
		{
		case DialoguePosType.Top:
			dialoguePosition.y = 400f;
			break;
		case DialoguePosType.Middle:
			dialoguePosition.y = 0f;
			break;
		case DialoguePosType.Bottom:
			dialoguePosition.y = -400f;
			break;
		}
		rtDialogue.anchoredPosition = dialoguePosition;
		textDialogue.text = MasterLocalize.GetData(message);
		goMerlin.SetActive(isMerlin);
		goKnight.SetActive(!isMerlin);
	}

	public void HideDialogue()
	{
		base.gameObject.SetActive(value: false);
	}

	public void SetNextClickPossible(bool isPossible)
	{
	}

	private void Awake()
	{
		rtDialogue = base.gameObject.GetComponent<RectTransform>();
	}
}
