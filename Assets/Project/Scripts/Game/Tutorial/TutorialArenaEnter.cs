using UnityEngine;
using UnityEngine.UI;

public class TutorialArenaEnter : MonoBehaviour
{
	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 2:
			TutorialManager.ShowHighLightUI(LobbyManager.ArenaOpenTimer);
			TutorialManager.ShowHand(LobbyManager.ArenaOpenTimer, Vector3.zero);
			break;
		case 3:
			TutorialManager.ClearHand();
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ShowHighLightUI(LobbyManager.ArenaLevelContent);
			LobbyManager.ArenaLevelContentDimmed.gameObject.SetActive(value: true);
			break;
		case 4:
			LobbyManager.ArenaLevelContentDimmed.gameObject.SetActive(value: false);
			TutorialManager.ClearHand();
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ShowHighLightUI(LobbyManager.ArenaShopButton);
			TutorialManager.ShowHand(LobbyManager.ArenaShopButton, new Vector3(-0.9f, 0.5f));
			LobbyManager.ArenaShopButton.GetComponent<Button>().enabled = false;
			break;
		case 5:
			LobbyManager.ArenaShopButton.GetComponent<Button>().enabled = true;
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ClearHand();
			break;
		}
	}
}
