using System.Collections;
using UnityEngine;

public class TutorialNewStoreOpen : MonoBehaviour
{
	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 1:
			StartCoroutine(ShowDelaySecondFloorItem());
			break;
		case 3:
			TutorialManager.SetDimmedClick(isClick: false);
			TutorialManager.ReturnCopyHighLightUI();
			TutorialManager.ReturnHighLightSpriteList();
			TutorialManager.ShowHighLightUI(LobbyManager.BattleButton);
			TutorialManager.ShowHand(LobbyManager.BattleButton, new Vector3(0f, 0.5f, 0f));
			LobbyManager.OpenStageSelect = OnOpenStageSelect;
			break;
		}
	}

	private IEnumerator ShowDelaySecondFloorItem()
	{
		LobbyManager.MoveStore(0, 1);
		yield return null;
		Transform secondFloorItem = TutorialManager.ShowCopyHighLightUI(LobbyManager.SecondFloorItem.transform);
		secondFloorItem.position = LobbyManager.SecondFloorItem.transform.position;
		secondFloorItem.localScale = Vector3.one;
		TutorialManager.ShowAndSortHighLightSprite(LobbyManager.SecondFloorItem.TrStore);
	}

	private void OnOpenStageSelect()
	{
		LobbyManager.OpenStageSelect = null;
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.HideTutorial();
		TutorialManager.EndMustTutorial();
	}
}
