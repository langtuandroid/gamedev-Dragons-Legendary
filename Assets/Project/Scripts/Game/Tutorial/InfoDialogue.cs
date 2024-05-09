using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InfoDialogue : MonoBehaviour
{
	public enum DialogPos
	{
		Top = 1,
		Middle,
		Bottom
	}
	
	[FormerlySerializedAs("textDialogue")] [SerializeField]
	private Text _dialogeText;

	[FormerlySerializedAs("goMerlin")] [SerializeField]
	private GameObject _goMerlin;

	[FormerlySerializedAs("goKnight")] [SerializeField]
	private GameObject _goKnight;

	private DialogPos DialoPos;

	private RectTransform _dialogeRP;

	private Vector2 _dialogue;

	public void OpenPhrase(string message, DialogPos type, bool isMerlin = true)
	{
		base.gameObject.SetActive(value: true);
		_dialogue = _dialogeRP.anchoredPosition;
		switch (type)
		{
		case DialogPos.Top:
			_dialogue.y = 400f;
			break;
		case DialogPos.Middle:
			_dialogue.y = 0f;
			break;
		case DialogPos.Bottom:
			_dialogue.y = -400f;
			break;
		}
		_dialogeRP.anchoredPosition = _dialogue;
		_dialogeText.text = MasterLocalize.GetData(message);
		_goMerlin.SetActive(isMerlin);
		_goKnight.SetActive(!isMerlin);
	}

	public void CloseDialogue()
	{
		base.gameObject.SetActive(value: false);
	}

	public void ClickNext(bool isPossible)
	{
	}

	private void Awake()
	{
		_dialogeRP = base.gameObject.GetComponent<RectTransform>();
	}
}
