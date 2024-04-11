using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialJewelChest : MonoBehaviour
{
	private Transform mysteriousChestBT;

	private Vector3 originalSize;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 1:
			TutorialManager.ShowHighLightUI(LobbyManager.GetChestButton);
			TutorialManager.ShowHand(LobbyManager.GetChestButton, Vector3.zero);
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.OpenChest = OpenChest;
			break;
		case 2:
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ClearHand();
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 3:
			GameInfo.userData.userInfo.jewel = GameInfo.userData.userInfo.jewel + 150;
			mysteriousChestBT = LobbyManager.GetMysteriousChestButton;
			originalSize = mysteriousChestBT.localScale;
			TutorialManager.ShowHighLightUI(mysteriousChestBT, isScaleOne: false);
			TutorialManager.ShowHand(mysteriousChestBT, Vector3.zero);
			mysteriousChestBT.localScale = originalSize;
			StartCoroutine(SetJewelCount());
			TutorialManager.SetDimmedClick(isClick: false);
			if (!GameInfo.isRate && !GamePreferenceManager.GetIsRate())
			{
				GameInfo.isRate = true;
			}
			break;
		}
	}

	private IEnumerator SetJewelCount()
	{
		yield return null;
		mysteriousChestBT.GetChild(2).GetComponent<Text>().text = "<color=#ffffff>" + mysteriousChestBT.GetChild(2).GetComponent<Text>().text + "</color>";
		mysteriousChestBT.GetComponent<Button>().onClick.AddListener(OnSelectMysteriousChest);
		LobbyManager.JewelChestOpen = MysteriousChestOpen;
		LobbyManager.OpenChestResultDone = (Action)Delegate.Combine(LobbyManager.OpenChestResultDone, new Action(OnOpenChestResultDone));
	}

	private void OpenChest()
	{
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(2);
		TutorialManager.ShowTutorial();
		LobbyManager.OpenChest = null;
	}

	private void OnSelectMysteriousChest()
	{
		mysteriousChestBT.GetComponent<Button>().onClick.RemoveListener(OnSelectMysteriousChest);
		TutorialManager.ReturnHighLightUI(originalSize);
		TutorialManager.ClearHand();
		TutorialManager.HideTutorial();
	}

	private void MysteriousChestOpen()
	{
		LobbyManager.JewelChestOpen = null;
		AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.open_chest_2);
		TutorialManager.SaveTutorial();
		TutorialManager.EndEventTutorial();
	}

	private void OnOpenChestResultDone()
	{
		LobbyManager.OpenChestResultDone = (Action)Delegate.Remove(LobbyManager.OpenChestResultDone, new Action(OnOpenChestResultDone));
		if (GameInfo.currentSceneType == SceneType.Lobby)
		{
			LobbyManager.CheckDailyBonusConnect();
		}
	}
}
