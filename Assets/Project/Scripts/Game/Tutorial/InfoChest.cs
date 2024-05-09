using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoChest : MonoBehaviour
{
	private Transform _chestWorn;

	private Vector3 _startSize;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 4:
		case 5:
			break;
		case 1:
			_chestWorn = LobbyManager.GetMysteriousFreeChestButton;
			InfoManager.HighLightUI(LobbyManager.GetChestButton);
			InfoManager.HandAppear(LobbyManager.GetChestButton, new Vector3(0f, 0f, 0f));
			InfoManager.SetClickDimmed(isClick: false);
			LobbyManager.OpenChest = ChestOpen;
			InfoManager.SaveAllData(4, 1);
			break;
		case 2:
			_startSize = _chestWorn.localScale;
			_chestWorn.localScale = _startSize;
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 3:
			if (_chestWorn == null)
			{
				_chestWorn = LobbyManager.GetMysteriousFreeChestButton;
			}
			StartCoroutine(KeysAmount());
			_startSize = _chestWorn.localScale;
			InfoManager.HighLightUI(_chestWorn, isScaleOne: false);
			InfoManager.HandAppear(_chestWorn, Vector3.zero);
			InfoManager.SetClickDimmed(isClick: false);
			LobbyManager.OpenChestOpen = DoubleChestOpen;
			LobbyManager.OpenChestOpenResult = ChestOpenResult;
			break;
		case 6:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 7:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		}
	}

	private IEnumerator KeysAmount()
	{
		yield return null;
		_chestWorn.GetComponent<Button>().onClick.AddListener(ChestWorn);
	}

	private void ChestOpen()
	{
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(2);
		InfoManager.OpenTutorial();
		LobbyManager.OpenChest = null;
	}

	private void DoubleChestOpen()
	{
		InfoManager.ReturnUILight(_startSize);
		InfoManager.RemoveHand();
		InfoManager.SaveAllData(5, 1);
		InfoManager.StartSequence(6);
		InfoManager.OpenTutorial();
		LobbyManager.OpenChestOpen = null;
	}

	private void ChestOpenResult()
	{
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		LobbyManager.OpenChestOpenResult = null;
		InfoManager.CloseTutorial();
		LobbyManager.ShowChestOpenResult();
	}

	private void ChestWorn()
	{
		_chestWorn.GetComponent<Button>().onClick.RemoveListener(ChestWorn);
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.CloseTutorial();
	}
}
