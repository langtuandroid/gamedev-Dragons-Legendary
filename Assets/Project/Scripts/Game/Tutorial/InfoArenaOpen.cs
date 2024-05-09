using UnityEngine;

public class InfoArenaOpen : MonoBehaviour
{
	public void Open(int _seq)
	{
		if (_seq == 2)
		{
			InfoManager.SetClickDimmed(isClick: false);
			InfoManager.HighLightUI(LobbyManager.ArenaMenuButton);
			InfoManager.HandAppear(LobbyManager.ArenaMenuButton, new Vector3(-0.8f, 0.3f));
			LobbyManager.ArenaMenuEnter = MenuEnterEvent;
		}
	}

	private void MenuEnterEvent()
	{
		LobbyManager.ArenaMenuEnter = null;
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.CloseTutorial();
		InfoManager.EventTutorialEbd();
	}
}
