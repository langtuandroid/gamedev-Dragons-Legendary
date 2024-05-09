using UnityEngine;

public class InfoLeaderSkill : MonoBehaviour
{
	[SerializeField]
	private Transform hunterBT;

	private Transform _deckEditButton;

	private Transform _deckEditHunterTransform;

	private Transform _editDeckButton;

	private Vector3 _startSize;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 1:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 2:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 3:
			_deckEditButton = LobbyManager.GetDeckEditButton;
			InfoManager.HighLightUI(_deckEditButton);
			InfoManager.HandAppear(_deckEditButton, Vector3.zero);
			InfoManager.SetClickDimmed(isClick: false);
			LobbyManager.OpenDeckEdit = EditDeck;
			break;
		case 4:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 5:
			_deckEditHunterTransform = LobbyManager.GetHunterDeckEdit1;
			hunterBT.gameObject.SetActive(value: true);
			hunterBT.position = _deckEditHunterTransform.position;
			InfoManager.HandAppear(_deckEditHunterTransform, Vector3.zero);
			LobbyManager.OpenDeckEdit = null;
			LobbyManager.OpenDeckEdit = HunterClick;
			InfoManager.SetClickDimmed(isClick: false);
			break;
		case 6:
			_deckEditHunterTransform = LobbyManager.GetHunterDeckEdit2;
			hunterBT.position = _deckEditHunterTransform.position;
			InfoManager.HandAppear(_deckEditHunterTransform, Vector3.zero);
			LobbyManager.OpenDeckEdit = null;
			LobbyManager.OpenDeckEdit = SecondHunterClick;
			InfoManager.SetClickDimmed(isClick: false);
			break;
		case 7:
			_editDeckButton = LobbyManager.GetDeckEditBackButton;
			InfoManager.HighLightUI(_editDeckButton);
			InfoManager.HandAppear(_editDeckButton, Vector3.zero);
			InfoManager.SetClickDimmed(isClick: false);
			LobbyManager.OpenDeckEdit = BackButtonb;
			break;
		case 8:
			InfoManager.SaveAllData();
			InfoManager.SetClickDimmed(isClick: true);
			LobbyManager.OpenDeckEdit = null;
			break;
		}
	}

	private void EditDeck()
	{
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(4);
		InfoManager.OpenTutorial();
	}

	private void HunterClick()
	{
		InfoManager.ReturnCopy();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(6);
		InfoManager.OpenTutorial();
	}

	private void SecondHunterClick()
	{
		InfoManager.ReturnCopy();
		InfoManager.RemoveHand();
		hunterBT.gameObject.SetActive(value: false);
		InfoManager.StartSequence(7);
		InfoManager.OpenTutorial();
	}

	private void BackButtonb()
	{
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(8);
		InfoManager.OpenTutorial();
	}

	public void ClickHunterCard()
	{
		LobbyManager.HunterCardClickForTUtorial(_deckEditHunterTransform.GetComponent<HeroCard>());
	}
}
