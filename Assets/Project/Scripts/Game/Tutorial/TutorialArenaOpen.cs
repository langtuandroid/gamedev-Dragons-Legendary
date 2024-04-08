using UnityEngine;

public class TutorialArenaOpen : MonoBehaviour
{
	public void Show(int _seq)
	{
		if (_seq == 2)
		{
			TutorialManager.SetDimmedClick(isClick: false);
			TutorialManager.ShowHighLightUI(LobbyManager.ArenaMenuButton);
			TutorialManager.ShowHand(LobbyManager.ArenaMenuButton, new Vector3(-0.8f, 0.3f));
			LobbyManager.ArenaMenuEnter = OnArenaMenuEnterEvent;
		}
	}

	private void OnArenaMenuEnterEvent()
	{
		LobbyManager.ArenaMenuEnter = null;
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.HideTutorial();
		TutorialManager.EndEventTutorial();
	}
}
