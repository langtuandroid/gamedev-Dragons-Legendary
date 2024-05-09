using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoJewelChest : MonoBehaviour
{
	private Transform _mysteryChestOpen;

	private Vector3 _origPos;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 1:
			InfoManager.HighLightUI(LobbyManager.GetChestButton);
			InfoManager.HandAppear(LobbyManager.GetChestButton, Vector3.zero);
			InfoManager.SetClickDimmed(isClick: false);
			LobbyManager.OpenChest = ChestOpen;
			break;
		case 2:
			InfoManager.ReturnUILight();
			InfoManager.RemoveHand();
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 3:
			GameInfo.userData.userInfo.jewel = GameInfo.userData.userInfo.jewel + 150;
			_mysteryChestOpen = LobbyManager.GetMysteriousChestButton;
			_origPos = _mysteryChestOpen.localScale;
			InfoManager.HighLightUI(_mysteryChestOpen, isScaleOne: false);
			InfoManager.HandAppear(_mysteryChestOpen, Vector3.zero);
			_mysteryChestOpen.localScale = _origPos;
			StartCoroutine(JewelCount());
			InfoManager.SetClickDimmed(isClick: false);
			if (!GameInfo.isRate && !GamePreferenceManager.GetIsRate())
			{
				GameInfo.isRate = true;
			}
			break;
		}
	}

	private IEnumerator JewelCount()
	{
		yield return null;
		_mysteryChestOpen.GetChild(2).GetComponent<Text>().text = "<color=#ffffff>" + _mysteryChestOpen.GetChild(2).GetComponent<Text>().text + "</color>";
		_mysteryChestOpen.GetComponent<Button>().onClick.AddListener(MysteryChestSelect);
		LobbyManager.JewelChestOpen = ChestOpenMysterious;
		LobbyManager.OpenChestResultDone = (Action)Delegate.Combine(LobbyManager.OpenChestResultDone, new Action(OnChestOpen));
	}

	private void ChestOpen()
	{
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(2);
		InfoManager.OpenTutorial();
		LobbyManager.OpenChest = null;
	}

	private void MysteryChestSelect()
	{
		_mysteryChestOpen.GetComponent<Button>().onClick.RemoveListener(MysteryChestSelect);
		InfoManager.ReturnUILight(_origPos);
		InfoManager.RemoveHand();
		InfoManager.CloseTutorial();
	}

	private void ChestOpenMysterious()
	{
		LobbyManager.JewelChestOpen = null;
		InfoManager.SaveAllData();
		InfoManager.EventTutorialEbd();
	}

	private void OnChestOpen()
	{
		LobbyManager.OpenChestResultDone = (Action)Delegate.Remove(LobbyManager.OpenChestResultDone, new Action(OnChestOpen));
		if (GameInfo.currentSceneType == SceneType.Lobby)
		{
			LobbyManager.CheckDailyBonusConnect();
		}
	}
}
