using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InfoHunterLevelUp : MonoBehaviour
{
	private Transform _required;

	[FormerlySerializedAs("ItemLock")] [SerializeField]
	private Transform _itemLock;

	public void Open(int _seq)
	{
		switch (_seq)
		{
			case 1:
				InfoManager.SaveAllData();
				InfoManager.CloseTutorial();
				if (GameInfo.currentSceneType != SceneType.InGame)
				{
					GameInfo.isDirectBattleReward = true;
					GameInfo.inGamePlayData.stage = 1;
					GameInfo.inGamePlayData.chapter = 1;
					GameInfo.inGamePlayData.level = 3;
					GameInfo.inGamePlayData.levelIdx = 3;
					LobbyManager.GotoInGame(3);
					UnityEngine.Debug.Log("Tutorial 3:1");
					PuzzlePlayManager.OnBattleRewardTutorial = RewardConfirm;
				}
				else
				{
					UnityEngine.Debug.Log("Tutorial 3:1");
					PuzzlePlayManager.OnBattleRewardTutorial = RewardConfirm;
				}

				break;
			case 2:
				InfoManager.SaveAllData(3, 3);
				InfoManager.SetClickDimmed(isClick: true);
				break;
			case 3:
				InfoManager.SetClickDimmed(isClick: true);
				if (GameInfo.currentSceneType == SceneType.InGame)
				{
					PuzzlePlayManager.OnBattleRewardComplete = RewardComplete;
				}

				break;
			case 4:
				InfoManager.HandAppear(LobbyManager.GetHunterListButton, new Vector3(0.5f, 0.5f, 0f));
				InfoManager.HighLightUI(LobbyManager.GetHunterListButton);
				InfoManager.SetClickDimmed(isClick: false);
				LobbyManager.OpenHunterList = HunterListOpen;
				break;
			case 5:
				InfoManager.ReturnUILight();
				InfoManager.RemoveHand();
				InfoManager.SetClickDimmed(isClick: false);
				LobbyManager.OpenHunterList = HunterListOpen;
				LobbyManager.OpenHunterInfo = InfoOpen;
				LobbyManager.OpenHunterLevel = HunterLevelOpen;
				LobbyManager.OpenHunterLevelUp = HunterLevelUp;
				ItemCountSet(1);
				break;
			case 6:
				InfoManager.SetClickDimmed(isClick: false);
				break;
			case 7:
				InfoManager.SetClickDimmed(isClick: true);
				break;
			case 8:
				InfoManager.SetClickDimmed(isClick: true);
				GetHunterToken();
				break;
			case 9:
				if (!LobbyManager.GetHunterList.gameObject.activeSelf)
				{
					LobbyManager.OpenHunterLevelUp = HunterLevelUp;
					LobbyManager.ShowHunterListForce();
					LobbyManager.ShowHunterView(GameDataManager.GetHunterInfo(20001, 1, 1), _isSpawn: true);
					LobbyManager.ShowHunterLevel(GameDataManager.GetHunterInfo(20001, 1, 1), _isSpawn: true);
				}

				_itemLock.gameObject.SetActive(value: false);
				InfoManager.ReturnUILight();
				InfoManager.HandAppear(LobbyManager.LevelUpPlayBT, Vector3.zero);
				InfoManager.HighLightUI(LobbyManager.LevelUpPlayBT);
				InfoManager.SetClickDimmed(isClick: false);
				InfoManager.SaveAllData();
				break;
		}
	}

	private void RewardConfirm()
	{
		InfoManager.StartSequence(2);
		InfoManager.OpenTutorial();
		if (GameInfo.currentSceneType == SceneType.InGame)
		{
			InfoManager.HighLightUI(PuzzlePlayManager.BattleRewardPickItem);
		}

		PuzzlePlayManager.OnBattleRewardTutorial = null;
	}

	private void RewardComplete()
	{
		InfoManager.ReturnUILight();
		PuzzlePlayManager.ClaimReward();
		PuzzlePlayManager.OnBattleRewardComplete = null;
		InfoManager.CloseTutorial();
	}

	private void HunterListOpen()
	{
		StartCoroutine(OpenListRoutine());
	}

	private IEnumerator OpenListRoutine()
	{
		yield return null;
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(5);
		InfoManager.OpenTutorial();
		Transform trCopyCell = InfoManager.ShowCopyHighLight(LobbyManager.GetHunter);
		trCopyCell.position = LobbyManager.GetHunter.position;
		trCopyCell.localScale = Vector3.one;
		trCopyCell.GetComponent<Image>().SetNativeSize();
		trCopyCell.GetComponent<HeroCard>().HunterInfo = LobbyManager.GetHunter.GetComponent<HeroCard>().HunterInfo;
		InfoManager.HandAppear(trCopyCell, Vector3.zero);
		LobbyManager.OpenHunterList = null;
	}

	private void InfoOpen()
	{
		StartCoroutine(OpenInfoCoroutine());
	}

	private IEnumerator OpenInfoCoroutine()
	{
		yield return null;
		InfoManager.ReturnCopy();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(6);
		InfoManager.OpenTutorial();
		Transform trCopyCell = InfoManager.ShowCopyHighLight(LobbyManager.LevelUpBT);
		trCopyCell.position = LobbyManager.LevelUpBT.position;
		trCopyCell.localScale = Vector3.one;
		LobbyManager.OpenHunterInfo = null;
		InfoManager.HandAppear(trCopyCell, Vector3.zero);
	}

	private void HunterLevelOpen()
	{
		StartCoroutine(RoutineHunter());
	}

	private IEnumerator RoutineHunter()
	{
		yield return null;
		InfoManager.ReturnCopy();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(7);
		InfoManager.OpenTutorial();
		_required = LobbyManager.LevelUpRequiredItem;
		InfoManager.HighLightUI(_required);
		_required.GetChild(1).GetChild(0).GetChild(1)
			.GetComponent<Text>()
			.text = "<color=#ffffff>" + GameInfo.userData.GetItemCount(50038) + "</color>/" + 1;
		LobbyManager.OpenHunterLevel = null;
		_itemLock.gameObject.SetActive(value: true);
		_itemLock.position = _required.position;
		_itemLock.SetAsLastSibling();
	}

	private void GetHunterToken()
	{
		_required.GetChild(1).GetChild(0).GetChild(1)
			.GetComponent<Text>()
			.text = "<color=#ffffff>1</color>/" + 1;
		LobbyManager.GetHunterLevel.GetComponent<HeroLevel>().LevelUpCondition();
	}

	private void HunterLevelUp()
	{
		_required = null;
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(9);
		InfoManager.NextStepIndex();
		InfoManager.SaveAllData(4, 1);
		LobbyManager.OpenHunterLevelUp = null;
		InfoManager.CloseTutorial();
	}

	private void ItemCountSet(int _idx)
	{
		for (int i = 0; i < GameInfo.userData.userItemList.Length; i++)
		{
			if (GameInfo.userData.userItemList[i].itemIdx == 50038)
			{
				GameInfo.userData.userItemList[i].count = _idx;
			}
		}
	}
}
