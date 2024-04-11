using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHunterEnchant : MonoBehaviour
{
	private Transform mysteriousChestBT;

	private Vector3 originalSize;

	private Transform chestResultOkBT;

	private Transform chestResultBackBT;

	private Transform hunterListBT;

	private Transform hunterEnchantCard;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 6:
			break;
		case 1:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 2:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 3:
			TutorialManager.ShowHighLightUI(LobbyManager.GetChestButton);
			TutorialManager.ShowHand(LobbyManager.GetChestButton, Vector3.zero);
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.OpenChest = OpenChest;
			LobbyManager.OpenChestOpenEnchant = OpenChest;
			break;
		case 4:
			GameInfo.userData.userInfo.jewel = GameInfo.userData.userInfo.jewel + 150;
			mysteriousChestBT = LobbyManager.GetMysteriousChestButton;
			originalSize = mysteriousChestBT.localScale;
			mysteriousChestBT.localScale = originalSize;
			StartCoroutine(SetJewelCount());
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 5:
			TutorialManager.ShowHighLightUI(mysteriousChestBT, isScaleOne: false);
			TutorialManager.ShowHand(mysteriousChestBT, Vector3.zero);
			LobbyManager.OpenChestOpenEnchant = null;
			LobbyManager.OpenChestOpenEnchant = OpenChestOpen;
			TutorialManager.SetDimmedClick(isClick: false);
			break;
		case 7:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 8:
			chestResultOkBT = LobbyManager.GetChestResultOkButton;
			TutorialManager.ShowHighLightUI(chestResultOkBT);
			TutorialManager.ShowHand(chestResultOkBT, Vector3.zero);
			TutorialManager.SetDimmedClick(isClick: false);
			break;
		case 9:
			chestResultBackBT = LobbyManager.GetChestBackButton;
			TutorialManager.ShowHighLightUI(chestResultBackBT);
			TutorialManager.ShowHand(chestResultBackBT, new Vector3(0.5f, 0.5f, 0f));
			TutorialManager.SetDimmedClick(isClick: false);
			break;
		case 10:
			hunterListBT = LobbyManager.GetHunterListButton;
			TutorialManager.ShowHighLightUI(hunterListBT);
			TutorialManager.ShowHand(hunterListBT, new Vector3(0.5f, 0.5f, 0f));
			TutorialManager.SetDimmedClick(isClick: false);
			break;
		case 11:
			TutorialManager.SetDimmedClick(isClick: false);
			break;
		case 12:
			hunterEnchantCard = LobbyManager.GetHunterEnchantCard;
			TutorialManager.ShowHighLightUI(hunterEnchantCard);
			TutorialManager.ShowHand(hunterEnchantCard, new Vector3(0.5f, 0f, 0f));
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 13:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 14:
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ClearHand();
			hunterEnchantCard = null;
			TutorialManager.SaveTutorial();
			chestResultOkBT = null;
			chestResultBackBT = null;
			hunterListBT = null;
			break;
		}
	}

	private IEnumerator SetJewelCount()
	{
		yield return null;
		mysteriousChestBT.GetChild(2).GetComponent<Text>().text = "<color=#ffffff>" + mysteriousChestBT.GetChild(2).GetComponent<Text>().text + "</color>";
		mysteriousChestBT.GetComponent<Button>().onClick.AddListener(OnSelectMysteriousChest);
		LobbyManager.JewelChestOpen = MysteriousChestOpen;
	}

	private void OpenChest()
	{
		UnityEngine.Debug.Log("ENCHANT TUTORIAL 11");
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(4);
		TutorialManager.ShowTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
	}

	private void OpenChestOpen()
	{
		TutorialManager.ReturnHighLightUI(originalSize);
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(7);
		TutorialManager.ShowTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
		LobbyManager.OpenChestOpenEnchant = OpenChestOpenResult;
	}

	private void OpenChestOpenResult()
	{
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(9);
		TutorialManager.ShowTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
		LobbyManager.OpenChestOpenEnchant = OpenChestOpenBack;
	}

	private void OpenChestOpenBack()
	{
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(10);
		TutorialManager.ShowTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
		LobbyManager.OpenChestOpenEnchant = OpenHunterList;
	}

	private void OpenHunterList()
	{
		StartCoroutine(OpenHunterListCoroutine());
	}

	private IEnumerator OpenHunterListCoroutine()
	{
		yield return null;
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(11);
		TutorialManager.ShowTutorial();
		Transform trCopyCell = TutorialManager.ShowCopyHighLightUI(LobbyManager.GetHunter);
		trCopyCell.position = LobbyManager.GetHunter.position;
		trCopyCell.localScale = Vector3.one;
		trCopyCell.GetComponent<Image>().SetNativeSize();
		trCopyCell.GetComponent<HunterCard>().HunterInfo = LobbyManager.GetHunter.GetComponent<HunterCard>().HunterInfo;
		TutorialManager.ShowHand(trCopyCell, Vector3.zero);
		LobbyManager.OpenChestOpenEnchant = null;
		LobbyManager.OpenChestOpenEnchant = OpenChestOpenHunterView;
	}

	private void OpenChestOpenHunterView()
	{
		UnityEngine.Debug.Log("2222222222222222222");
		TutorialManager.ReturnCopyHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(12);
		TutorialManager.ShowTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
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
	}
}
