using UnityEngine;

public class TutorialLeaderSkill : MonoBehaviour
{
	[SerializeField]
	private Transform hunterBT;

	private Transform deckEditButton;

	private Transform deckEditHunter;

	private Transform deckEditBackButton;

	private Vector3 originalSize;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 1:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 2:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 3:
			deckEditButton = LobbyManager.GetDeckEditButton;
			TutorialManager.ShowHighLightUI(deckEditButton);
			TutorialManager.ShowHand(deckEditButton, Vector3.zero);
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.OpenDeckEdit = OpenDeckEdit;
			break;
		case 4:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 5:
			deckEditHunter = LobbyManager.GetHunterDeckEdit1;
			hunterBT.gameObject.SetActive(value: true);
			hunterBT.position = deckEditHunter.position;
			TutorialManager.ShowHand(deckEditHunter, Vector3.zero);
			LobbyManager.OpenDeckEdit = null;
			LobbyManager.OpenDeckEdit = ClickHunter1;
			TutorialManager.SetDimmedClick(isClick: false);
			break;
		case 6:
			deckEditHunter = LobbyManager.GetHunterDeckEdit2;
			hunterBT.position = deckEditHunter.position;
			TutorialManager.ShowHand(deckEditHunter, Vector3.zero);
			LobbyManager.OpenDeckEdit = null;
			LobbyManager.OpenDeckEdit = ClickHunter2;
			TutorialManager.SetDimmedClick(isClick: false);
			break;
		case 7:
			deckEditBackButton = LobbyManager.GetDeckEditBackButton;
			TutorialManager.ShowHighLightUI(deckEditBackButton);
			TutorialManager.ShowHand(deckEditBackButton, Vector3.zero);
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.OpenDeckEdit = ClickGoBack;
			break;
		case 8:
			TutorialManager.SaveTutorial();
			TutorialManager.SetDimmedClick(isClick: true);
			LobbyManager.OpenDeckEdit = null;
			break;
		}
	}

	private void OpenDeckEdit()
	{
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(4);
		TutorialManager.ShowTutorial();
	}

	private void ClickHunter1()
	{
		TutorialManager.ReturnCopyHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(6);
		TutorialManager.ShowTutorial();
	}

	private void ClickHunter2()
	{
		TutorialManager.ReturnCopyHighLightUI();
		TutorialManager.ClearHand();
		hunterBT.gameObject.SetActive(value: false);
		TutorialManager.SetSeq(7);
		TutorialManager.ShowTutorial();
	}

	private void ClickGoBack()
	{
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(8);
		TutorialManager.ShowTutorial();
	}

	public void ClickHunterCard()
	{
		LobbyManager.HunterCardClickForTUtorial(deckEditHunter.GetComponent<HunterCard>());
	}
}
