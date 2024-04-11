using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHunterLevelUp : MonoBehaviour
{
	private Transform RequiredItem;

	[SerializeField]
	private Transform ItemLock;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 1:
			TutorialManager.SaveTutorial();
			TutorialManager.HideTutorial();
			if (GameInfo.currentSceneType != SceneType.InGame)
			{
				GameInfo.isDirectBattleReward = true;
				GameInfo.inGamePlayData.stage = 1;
				GameInfo.inGamePlayData.chapter = 1;
				GameInfo.inGamePlayData.level = 3;
				GameInfo.inGamePlayData.levelIdx = 3;
				LobbyManager.GotoInGame(3);
				UnityEngine.Debug.Log("Tutorial 3:1");
				InGamePlayManager.BattleRewardTutorial = OpenReward;
			}
			else
			{
				UnityEngine.Debug.Log("Tutorial 3:1");
				InGamePlayManager.BattleRewardTutorial = OpenReward;
			}
			break;
		case 2:
			TutorialManager.SaveTutorial(3, 3);
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 3:
			TutorialManager.SetDimmedClick(isClick: true);
			if (GameInfo.currentSceneType == SceneType.InGame)
			{
				InGamePlayManager.BattleRewardComplete = OpenRewardComplete;
			}
			break;
		case 4:
			TutorialManager.ShowHand(LobbyManager.GetHunterListButton, new Vector3(0.5f, 0.5f, 0f));
			TutorialManager.ShowHighLightUI(LobbyManager.GetHunterListButton);
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.OpenHunterList = OpenHunterList;
			break;
		case 5:
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ClearHand();
			TutorialManager.SetDimmedClick(isClick: false);
			LobbyManager.OpenHunterList = OpenHunterList;
			LobbyManager.OpenHunterInfo = OpenHunterInfo;
			LobbyManager.OpenHunterLevel = OpenHunterLevel;
			LobbyManager.OpenHunterLevelUp = OpenHunterLevelUp;
			SetItemCount(1);
			break;
		case 6:
			TutorialManager.SetDimmedClick(isClick: false);
			break;
		case 7:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 8:
			TutorialManager.SetDimmedClick(isClick: true);
			OpenHunterLevelGetToken();
			break;
		case 9:
			if (!LobbyManager.GetHunterList.gameObject.activeSelf)
			{
				LobbyManager.OpenHunterLevelUp = OpenHunterLevelUp;
				LobbyManager.ShowHunterListForce();
				LobbyManager.ShowHunterView(GameDataManager.GetHunterInfo(20001, 1, 1), _isSpawn: true);
				LobbyManager.ShowHunterLevel(GameDataManager.GetHunterInfo(20001, 1, 1), _isSpawn: true);
			}
			ItemLock.gameObject.SetActive(value: false);
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ShowHand(LobbyManager.LevelUpPlayBT, Vector3.zero);
			TutorialManager.ShowHighLightUI(LobbyManager.LevelUpPlayBT);
			TutorialManager.SetDimmedClick(isClick: false);
			TutorialManager.SaveTutorial();
			break;
		}
	}

	private void OpenReward()
	{
		TutorialManager.SetSeq(2);
		TutorialManager.ShowTutorial();
		if (GameInfo.currentSceneType == SceneType.InGame)
		{
			TutorialManager.ShowHighLightUI(InGamePlayManager.BattleRewardPickItem);
		}
		InGamePlayManager.BattleRewardTutorial = null;
	}

	private void OpenRewardComplete()
	{
		TutorialManager.ReturnHighLightUI();
		InGamePlayManager.BattleRewardClaim();
		InGamePlayManager.BattleRewardComplete = null;
		TutorialManager.HideTutorial();
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
		TutorialManager.SetSeq(5);
		TutorialManager.ShowTutorial();
		Transform trCopyCell = TutorialManager.ShowCopyHighLightUI(LobbyManager.GetHunter);
		trCopyCell.position = LobbyManager.GetHunter.position;
		trCopyCell.localScale = Vector3.one;
		trCopyCell.GetComponent<Image>().SetNativeSize();
		trCopyCell.GetComponent<HunterCard>().HunterInfo = LobbyManager.GetHunter.GetComponent<HunterCard>().HunterInfo;
		TutorialManager.ShowHand(trCopyCell, Vector3.zero);
		LobbyManager.OpenHunterList = null;
	}

	private void OpenHunterInfo()
	{
		StartCoroutine(OpenHunterInfoCoroutine());
	}

	private IEnumerator OpenHunterInfoCoroutine()
	{
		yield return null;
		TutorialManager.ReturnCopyHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(6);
		TutorialManager.ShowTutorial();
		Transform trCopyCell = TutorialManager.ShowCopyHighLightUI(LobbyManager.LevelUpBT);
		trCopyCell.position = LobbyManager.LevelUpBT.position;
		trCopyCell.localScale = Vector3.one;
		LobbyManager.OpenHunterInfo = null;
		TutorialManager.ShowHand(trCopyCell, Vector3.zero);
	}

	private void OpenHunterLevel()
	{
		StartCoroutine(OpenHunterLevelCoroutine());
	}

	private IEnumerator OpenHunterLevelCoroutine()
	{
		yield return null;
		TutorialManager.ReturnCopyHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(7);
		TutorialManager.ShowTutorial();
		RequiredItem = LobbyManager.LevelUpRequiredItem;
		TutorialManager.ShowHighLightUI(RequiredItem);
		RequiredItem.GetChild(1).GetChild(0).GetChild(1)
			.GetComponent<Text>()
			.text = "<color=#ffffff>" + GameInfo.userData.GetItemCount(50038) + "</color>/" + 1;
			LobbyManager.OpenHunterLevel = null;
			ItemLock.gameObject.SetActive(value: true);
			ItemLock.position = RequiredItem.position;
			ItemLock.SetAsLastSibling();
		}

		private void OpenHunterLevelGetToken()
		{
			RequiredItem.GetChild(1).GetChild(0).GetChild(1)
				.GetComponent<Text>()
				.text = "<color=#ffffff>1</color>/" + 1;
				LobbyManager.GetHunterLevel.GetComponent<HunterLevel>().SetForceLevelupCondition();
			}

			private void OpenHunterLevelUp()
			{
				AnalyticsManager.FirebaseAnalyticsLogEvent(FBLog_Type.hunter_level_up);
				RequiredItem = null;
				TutorialManager.ReturnHighLightUI();
				TutorialManager.ClearHand();
				TutorialManager.SetSeq(9);
				TutorialManager.NextTutorialindex();
				TutorialManager.SaveTutorial(4, 1);
				LobbyManager.OpenHunterLevelUp = null;
				TutorialManager.HideTutorial();
			}

			private void SetItemCount(int _idx)
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
