using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialChest : MonoBehaviour
{
	private Transform wornChestBT;

	private Vector3 originalSize;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 4:
		case 5:
			break;
		case 1:
			wornChestBT = LobbyManager.GetMysteriousFreeChestButton;
			TutorialManager.ShowHighLightUI(LobbyManager.GetChestButton);
			TutorialManager.ShowHand(LobbyManager.GetChestButton, new Vector3(0f, 0f, 0f));
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.OpenChest = OpenChest;
			TutorialManager.SaveTutorial(4, 1);
			break;
		case 2:
			originalSize = wornChestBT.localScale;
			wornChestBT.localScale = originalSize;
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 3:
			if (wornChestBT == null)
			{
				wornChestBT = LobbyManager.GetMysteriousFreeChestButton;
			}
			StartCoroutine(SetKeyCount());
			originalSize = wornChestBT.localScale;
			TutorialManager.ShowHighLightUI(wornChestBT, isScaleOne: false);
			TutorialManager.ShowHand(wornChestBT, Vector3.zero);
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.OpenChestOpen = OpenChestOpen;
			LobbyManager.OpenChestOpenResult = OpenChestOpenResult;
			break;
		case 6:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 7:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		}
	}

	private IEnumerator SetKeyCount()
	{
		yield return null;
		wornChestBT.GetComponent<Button>().onClick.AddListener(OnSelectWornChest);
	}

	private void OpenChest()
	{
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(2);
		TutorialManager.ShowTutorial();
		LobbyManager.OpenChest = null;
	}

	private void OpenChestOpen()
	{
		TutorialManager.ReturnHighLightUI(originalSize);
		TutorialManager.ClearHand();
		TutorialManager.SaveTutorial(5, 1);
		TutorialManager.SetSeq(6);
		TutorialManager.ShowTutorial();
		LobbyManager.OpenChestOpen = null;
	}

	private void OpenChestOpenResult()
	{
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		LobbyManager.OpenChestOpenResult = null;
		TutorialManager.HideTutorial();
		LobbyManager.ShowChestOpenResult();
	}

	private void OnSelectWornChest()
	{
		wornChestBT.GetComponent<Button>().onClick.RemoveListener(OnSelectWornChest);
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.HideTutorial();
	}
}
