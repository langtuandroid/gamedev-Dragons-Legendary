using UnityEngine;
using UnityEngine.UI;

public class InfoArenaEnter : MonoBehaviour
{
	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 2:
			InfoManager.HighLightUI(LobbyManager.ArenaOpenTimer);
			InfoManager.HandAppear(LobbyManager.ArenaOpenTimer, Vector3.zero);
			break;
		case 3:
			InfoManager.RemoveHand();
			InfoManager.ReturnUILight();
			InfoManager.HighLightUI(LobbyManager.ArenaLevelContent);
			LobbyManager.ArenaLevelContentDimmed.gameObject.SetActive(value: true);
			break;
		case 4:
			LobbyManager.ArenaLevelContentDimmed.gameObject.SetActive(value: false);
			InfoManager.RemoveHand();
			InfoManager.ReturnUILight();
			InfoManager.HighLightUI(LobbyManager.ArenaShopButton);
			InfoManager.HandAppear(LobbyManager.ArenaShopButton, new Vector3(-0.9f, 0.5f));
			LobbyManager.ArenaShopButton.GetComponent<Button>().enabled = false;
			break;
		case 5:
			LobbyManager.ArenaShopButton.GetComponent<Button>().enabled = true;
			InfoManager.ReturnUILight();
			InfoManager.RemoveHand();
			break;
		}
	}
}
