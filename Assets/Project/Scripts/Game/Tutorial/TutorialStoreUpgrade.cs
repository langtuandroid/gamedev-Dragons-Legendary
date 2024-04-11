using System.Collections;
using UnityEngine;

public class TutorialStoreUpgrade : MonoBehaviour
{
	private FloorItem firstFloorCopyItem;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 1:
			StartCoroutine(ShowDelayFirstTutorial());
			break;
		case 2:
			LobbyManager.StoreDetailEnter = null;
			TutorialManager.ReturnCopyHighLightUI();
			TutorialManager.ClearHand();
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.FirstFloorItem.ShowDetail();
			TutorialManager.ShowHighLightUI(LobbyManager.FloorDetailUpgradeButton);
			TutorialManager.ShowHand(LobbyManager.FloorDetailUpgradeButton, Vector3.zero);
			LobbyManager.StoreUpgradeEnter = OnStoreUpgradeEnterEvent;
			break;
		case 3:
			TutorialManager.ShowHighLightUI(LobbyManager.FloorUpgradeAbility);
			break;
		case 4:
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ClearHand();
			LobbyManager.ShowFloorUpgradeItemDimmed();
			TutorialManager.ShowHighLightUI(LobbyManager.FloorUpgradeRequireItemAnchor);
			break;
		case 5:
			TutorialManager.SetDimmedClick(isClick: false);
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ClearHand();
			TutorialManager.ShowHighLightUI(LobbyManager.FloorUpgradeConfimButton);
			TutorialManager.ShowHand(LobbyManager.FloorUpgradeConfimButton, Vector3.zero);
			LobbyManager.StoreUpgradeComplete = OnStoreUpgradeComplete;
			break;
		}
	}

	private IEnumerator ShowDelayFirstTutorial()
	{
		LobbyManager.MoveStore(0, 0);
		yield return null;
		Transform trFirstFloorCopy = TutorialManager.ShowCopyHighLightUI(LobbyManager.FirstFloorItem.transform);
		trFirstFloorCopy.position = LobbyManager.FirstFloorItem.transform.position;
		trFirstFloorCopy.localScale = Vector3.one;
		firstFloorCopyItem = trFirstFloorCopy.GetComponent<FloorItem>();
		firstFloorCopyItem.AllButtonLock();
		TutorialManager.ShowHand(trFirstFloorCopy, Vector3.zero);
		LobbyManager.StoreDetailEnter = OnStoreDetailEnterEvent;
	}

	private void OnStoreDetailEnterEvent()
	{
		LobbyManager.StoreDetailEnter = null;
		TutorialManager.ReturnCopyHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.NextSep();
	}

	private void OnStoreUpgradeEnterEvent()
	{
		LobbyManager.StoreUpgradeEnter = null;
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.NextSep();
	}

	private void OnStoreUpgradeComplete()
	{
		AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.upgrade_store);
		LobbyManager.StoreUpgradeComplete = null;
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.HideTutorial();
		TutorialManager.EndEventTutorial();
	}
}
