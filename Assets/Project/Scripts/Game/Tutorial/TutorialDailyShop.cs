using UnityEngine;

public class TutorialDailyShop : MonoBehaviour
{
	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 1:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 2:
			TutorialManager.SetDimmedClick(isClick: true);
			LobbyManager.OpenValueShop = OpenValueShop;
			break;
		}
	}

	private void OpenValueShop()
	{
		TutorialManager.SaveTutorial();
		TutorialManager.HideTutorial();
		TutorialManager.EndEventTutorial();
	}
}
