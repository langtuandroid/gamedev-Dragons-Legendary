using System.Collections;
using UnityEngine;

public class InfoNewStoreOpen : MonoBehaviour
{
	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 1:
			StartCoroutine(DelayItems());
			break;
		case 3:
			InfoManager.SetClickDimmed(isClick: false);
			InfoManager.ReturnCopy();
			InfoManager.ReturnBackAll();
			InfoManager.HighLightUI(LobbyManager.BattleButton);
			InfoManager.HandAppear(LobbyManager.BattleButton, new Vector3(0f, 0.5f, 0f));
			LobbyManager.OpenStageSelect = StageSelect;
			break;
		}
	}

	private IEnumerator DelayItems()
	{
		LobbyManager.MoveStore(0, 1);
		yield return null;
		Transform secondFloorItem = InfoManager.ShowCopyHighLight(LobbyManager.SecondFloorItem.transform);
		secondFloorItem.position = LobbyManager.SecondFloorItem.transform.position;
		secondFloorItem.localScale = Vector3.one;
		InfoManager.SortLightSprite(LobbyManager.SecondFloorItem.Store);
	}

	private void StageSelect()
	{
		LobbyManager.OpenStageSelect = null;
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.CloseTutorial();
		InfoManager.TutorialEnd();
	}
}
