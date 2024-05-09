using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoHunterEnchant : MonoBehaviour
{
	private Transform _chestMysteryBT;

	private Vector3 _startSize;

	private Transform _chestResultBT;

	private Transform _chestResultBack;

	private Transform _hunterList;

	private Transform _enchantCard;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 6:
			break;
		case 1:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 2:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 3:
			InfoManager.HighLightUI(LobbyManager.GetChestButton);
			InfoManager.HandAppear(LobbyManager.GetChestButton, Vector3.zero);
			InfoManager.SetClickDimmed(isClick: false);
			LobbyManager.OpenChest = ChestOpenCheck;
			LobbyManager.OpenChestOpenEnchant = ChestOpenCheck;
			break;
		case 4:
			GameInfo.userData.userInfo.jewel = GameInfo.userData.userInfo.jewel + 150;
			_chestMysteryBT = LobbyManager.GetMysteriousChestButton;
			_startSize = _chestMysteryBT.localScale;
			_chestMysteryBT.localScale = _startSize;
			StartCoroutine(JewelCount());
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 5:
			InfoManager.HighLightUI(_chestMysteryBT, isScaleOne: false);
			InfoManager.HandAppear(_chestMysteryBT, Vector3.zero);
			LobbyManager.OpenChestOpenEnchant = null;
			LobbyManager.OpenChestOpenEnchant = DoubleChestOpen;
			InfoManager.SetClickDimmed(isClick: false);
			break;
		case 7:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 8:
			_chestResultBT = LobbyManager.GetChestResultOkButton;
			InfoManager.HighLightUI(_chestResultBT);
			InfoManager.HandAppear(_chestResultBT, Vector3.zero);
			InfoManager.SetClickDimmed(isClick: false);
			break;
		case 9:
			_chestResultBack = LobbyManager.GetChestBackButton;
			InfoManager.HighLightUI(_chestResultBack);
			InfoManager.HandAppear(_chestResultBack, new Vector3(0.5f, 0.5f, 0f));
			InfoManager.SetClickDimmed(isClick: false);
			break;
		case 10:
			_hunterList = LobbyManager.GetHunterListButton;
			InfoManager.HighLightUI(_hunterList);
			InfoManager.HandAppear(_hunterList, new Vector3(0.5f, 0.5f, 0f));
			InfoManager.SetClickDimmed(isClick: false);
			break;
		case 11:
			InfoManager.SetClickDimmed(isClick: false);
			break;
		case 12:
			_enchantCard = LobbyManager.GetHunterEnchantCard;
			InfoManager.HighLightUI(_enchantCard);
			InfoManager.HandAppear(_enchantCard, new Vector3(0.5f, 0f, 0f));
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 13:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 14:
			InfoManager.ReturnUILight();
			InfoManager.RemoveHand();
			_enchantCard = null;
			InfoManager.SaveAllData();
			_chestResultBT = null;
			_chestResultBack = null;
			_hunterList = null;
			break;
		}
	}

	private IEnumerator JewelCount()
	{
		yield return null;
		_chestMysteryBT.GetChild(2).GetComponent<Text>().text = "<color=#ffffff>" + _chestMysteryBT.GetChild(2).GetComponent<Text>().text + "</color>";
		_chestMysteryBT.GetComponent<Button>().onClick.AddListener(MysteriousChest);
		LobbyManager.JewelChestOpen = MysteriousChestOpen;
	}

	private void ChestOpenCheck()
	{
		UnityEngine.Debug.Log("ENCHANT TUTORIAL 11");
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(4);
		InfoManager.OpenTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
	}

	private void DoubleChestOpen()
	{
		InfoManager.ReturnUILight(_startSize);
		InfoManager.RemoveHand();
		InfoManager.StartSequence(7);
		InfoManager.OpenTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
		LobbyManager.OpenChestOpenEnchant = ResultOfOpenChest;
	}

	private void ResultOfOpenChest()
	{
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(9);
		InfoManager.OpenTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
		LobbyManager.OpenChestOpenEnchant = BackChestOpen;
	}

	private void BackChestOpen()
	{
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(10);
		InfoManager.OpenTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
		LobbyManager.OpenChestOpenEnchant = OpenHunterList;
	}

	private void OpenHunterList()
	{
		StartCoroutine(ListCoroutine());
	}

	private IEnumerator ListCoroutine()
	{
		yield return null;
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(11);
		InfoManager.OpenTutorial();
		Transform trCopyCell = InfoManager.ShowCopyHighLight(LobbyManager.GetHunter);
		trCopyCell.position = LobbyManager.GetHunter.position;
		trCopyCell.localScale = Vector3.one;
		trCopyCell.GetComponent<Image>().SetNativeSize();
		trCopyCell.GetComponent<HeroCard>().HunterInfo = LobbyManager.GetHunter.GetComponent<HeroCard>().HunterInfo;
		InfoManager.HandAppear(trCopyCell, Vector3.zero);
		LobbyManager.OpenChestOpenEnchant = null;
		LobbyManager.OpenChestOpenEnchant = HunterView;
	}

	private void HunterView()
	{
		UnityEngine.Debug.Log("2222222222222222222");
		InfoManager.ReturnCopy();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(12);
		InfoManager.OpenTutorial();
		LobbyManager.OpenChestOpenEnchant = null;
	}

	private void MysteriousChest()
	{
		_chestMysteryBT.GetComponent<Button>().onClick.RemoveListener(MysteriousChest);
		InfoManager.ReturnUILight(_startSize);
		InfoManager.RemoveHand();
		InfoManager.CloseTutorial();
	}

	private void MysteriousChestOpen()
	{
		LobbyManager.JewelChestOpen = null;
	}
}
