using UnityEngine;

public class InfoDailyShop : MonoBehaviour
{
	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 1:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 2:
			InfoManager.SetClickDimmed(isClick: true);
			LobbyManager.OpenValueShop = ValueShopOpen;
			break;
		}
	}

	private void ValueShopOpen()
	{
		InfoManager.SaveAllData();
		InfoManager.CloseTutorial();
		InfoManager.EventTutorialEbd();
	}
}
