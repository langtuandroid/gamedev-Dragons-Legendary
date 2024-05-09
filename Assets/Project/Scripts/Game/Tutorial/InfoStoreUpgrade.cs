using System.Collections;
using UnityEngine;

public class InfoStoreUpgrade : MonoBehaviour
{
	private LiftItem _firstFloorCopy;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 1:
			StartCoroutine(TuroeialDelayFirst());
			break;
		case 2:
			LobbyManager.StoreDetailEnter = null;
			InfoManager.ReturnCopy();
			InfoManager.RemoveHand();
			InfoManager.SetClickDimmed(isClick: false);
			LobbyManager.FirstFloorItem.OpenDetails();
			InfoManager.HighLightUI(LobbyManager.FloorDetailUpgradeButton);
			InfoManager.HandAppear(LobbyManager.FloorDetailUpgradeButton, Vector3.zero);
			LobbyManager.StoreUpgradeEnter = UpgradeEnterEvent;
			break;
		case 3:
			InfoManager.HighLightUI(LobbyManager.FloorUpgradeAbility);
			break;
		case 4:
			InfoManager.ReturnUILight();
			InfoManager.RemoveHand();
			LobbyManager.ShowFloorUpgradeItemDimmed();
			InfoManager.HighLightUI(LobbyManager.FloorUpgradeRequireItemAnchor);
			break;
		case 5:
			InfoManager.SetClickDimmed(isClick: false);
			InfoManager.ReturnUILight();
			InfoManager.RemoveHand();
			InfoManager.HighLightUI(LobbyManager.FloorUpgradeConfimButton);
			InfoManager.HandAppear(LobbyManager.FloorUpgradeConfimButton, Vector3.zero);
			LobbyManager.StoreUpgradeComplete = UpgradeCompleted;
			break;
		}
	}

	private IEnumerator TuroeialDelayFirst()
	{
		LobbyManager.MoveStore(0, 0);
		yield return null;
		Transform trFirstFloorCopy = InfoManager.ShowCopyHighLight(LobbyManager.FirstFloorItem.transform);
		trFirstFloorCopy.position = LobbyManager.FirstFloorItem.transform.position;
		trFirstFloorCopy.localScale = Vector3.one;
		_firstFloorCopy = trFirstFloorCopy.GetComponent<LiftItem>();
		_firstFloorCopy.LockAllUI();
		InfoManager.HandAppear(trFirstFloorCopy, Vector3.zero);
		LobbyManager.StoreDetailEnter = StoreEventEnter;
	}

	private void StoreEventEnter()
	{
		LobbyManager.StoreDetailEnter = null;
		InfoManager.ReturnCopy();
		InfoManager.RemoveHand();
		InfoManager.ContinueStep();
	}

	private void UpgradeEnterEvent()
	{
		LobbyManager.StoreUpgradeEnter = null;
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.ContinueStep();
	}

	private void UpgradeCompleted()
	{
		LobbyManager.StoreUpgradeComplete = null;
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.CloseTutorial();
		InfoManager.EventTutorialEbd();
	}
}
