using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStoreManagement : MonoBehaviour
{
	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 3:
		case 4:
		case 10:
			break;
		case 2:
			StartCoroutine(ShowFocusFirstFloorItem());
			break;
		case 5:
			TutorialManager.SetDimmedClick(isClick: false);
			TutorialManager.ShowHighLightUI(LobbyManager.BattleButton);
			TutorialManager.ShowHand(LobbyManager.BattleButton, new Vector3(0f, 0.5f, 0f));
			LobbyManager.OpenStageSelect = OnOpenStageSelect;
			break;
		case 6:
			TutorialManager.SetDimmedClick(isClick: false);
			StartCoroutine(CheckDelayStageSelect());
			break;
		case 7:
			TutorialManager.SetDimmedClick(isClick: false);
			StartCoroutine(CheckDelayShowLevelCell());
			break;
		case 8:
			GameInfo.inGamePlayData.stage = 1;
			GameInfo.inGamePlayData.chapter = 1;
			GameInfo.inGamePlayData.level = 2;
			GameInfo.inGamePlayData.levelIdx = 2;
			TutorialManager.SetDimmedClick(isClick: false);
			TutorialManager.ShowHighLightUI(LobbyManager.LevelPlayButton);
			TutorialManager.ShowHand(LobbyManager.LevelPlayButton, Vector3.zero);
			LobbyManager.LevelPlayButton.GetComponent<Button>().onClick.AddListener(OnSelectLevelPlay);
			break;
		case 9:
			TutorialManager.HideTutorial();
			InGamePlayManager.ShowBattleReward = OnShowBattleReward;
			InGamePlayManager.GameLose = onGameLose;
			break;
		case 11:
			TutorialManager.SaveTutorial();
			TutorialManager.HideTutorial();
			if (GameInfo.currentSceneType != SceneType.InGame)
			{
				GameInfo.isDirectBattleReward = true;
				GameInfo.inGamePlayData.stage = 1;
				GameInfo.inGamePlayData.chapter = 1;
				GameInfo.inGamePlayData.level = 2;
				GameInfo.inGamePlayData.levelIdx = 2;
				LobbyManager.GotoInGame(2);
			}
			else
			{
				InGamePlayManager.BattleRewardOpen = OnBattleRewardOpenEvent;
			}
			break;
		case 12:
			TutorialManager.SetDimmedClick(isClick: false);
			TutorialManager.SaveTutorial();
			StartCoroutine(CheckDelayShowCollectShow());
			break;
		case 13:
			TutorialManager.SetDimmedClick(isClick: false);
			TutorialManager.SaveTutorial();
			TutorialManager.ShowHighLightUI(LobbyManager.BattleButton);
			TutorialManager.ShowHand(LobbyManager.BattleButton, new Vector3(0f, 0.5f, 0f));
			LobbyManager.OpenStageSelect = OnOpenStageSelect;
			break;
		}
	}

	private IEnumerator ShowFocusFirstFloorItem()
	{
		LobbyManager.MoveStore(0, 0);
		yield return null;
		TutorialManager.ShowHighLightUI(LobbyManager.FirstFloorOpenButton);
		TutorialManager.ShowHand(LobbyManager.FirstFloorOpenButton, new Vector3(-1f, 0f, 0f));
		TutorialManager.SetDimmedClick(isClick: false);
		LobbyManager.StartStoreOpen = OnStartStoreOpen;
	}

	private IEnumerator CheckDelayStageSelect()
	{
		yield return null;
		Transform trCopyCell = TutorialManager.ShowCopyHighLightUI(LobbyManager.FirstStageCell);
		trCopyCell.GetComponent<StageCell>().SetForceTouch();
		trCopyCell.GetComponent<StageCell>().SelectStageEvent = OnSelectStageCell;
		trCopyCell.position = LobbyManager.FirstStageCell.position;
		trCopyCell.localScale = Vector3.one;
		TutorialManager.ShowHand(trCopyCell.GetComponent<StageCell>().SelectButton, Vector3.zero);
	}

	private IEnumerator CheckDelayShowLevelCell()
	{
		yield return null;
		LevelCell secondCell = LobbyManager.SecondLevelCell;
		LevelDbData data = GameDataManager.GetLevelIndexDbData(2);
		LevelCell copySecondLevelCell = TutorialManager.ShowCopyHighLightUI(secondCell.transform).GetComponent<LevelCell>();
		copySecondLevelCell.transform.position = secondCell.transform.position;
		copySecondLevelCell.transform.localScale = Vector3.one;
		copySecondLevelCell.SetData(data);
		copySecondLevelCell.SetUnLock();
		copySecondLevelCell.SelectLevelCell = OnSelectLevelCell;
		TutorialManager.ShowHand(copySecondLevelCell.transform, Vector3.zero);
	}

	private IEnumerator CheckDelayShowCollectShow()
	{
		LobbyManager.MoveStore(0, 0);
		yield return null;
		TutorialManager.ShowHighLightUI(LobbyManager.FirstFloorCollectButton);
		TutorialManager.ShowHand(LobbyManager.FirstFloorCollectButton, new Vector3(-1f, 0f, 0f));
		LobbyManager.StoreCollectComplete = OnStoreCollectComplete;
	}

	private void OnStartStoreOpen()
	{
		TutorialManager.SetDimmedClick(isClick: true);
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(3);
		TutorialManager.ShowTutorial();
		TutorialManager.SaveTutorial(2, 5);
		LobbyManager.StartStoreOpen = null;
	}

	private void OnOpenStageSelect()
	{
		switch (TutorialManager.Seq)
		{
		case 5:
			TutorialManager.ReturnHighLightUI();
			TutorialManager.ClearHand();
			TutorialManager.SetSeq(6);
			TutorialManager.ShowTutorial();
			break;
		case 13:
			TutorialManager.HideTutorial();
			TutorialManager.ClearHand();
			TutorialManager.ReturnHighLightUI();
			TutorialManager.NextTutorialindex();
			break;
		}
		LobbyManager.OpenStageSelect = null;
	}

	private void OnSelectStageCell(int index)
	{
		TutorialManager.ReturnCopyHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(7);
		TutorialManager.ShowTutorial();
		LobbyManager.ShowChapterList(1);
	}

	private void OnSelectLevelCell()
	{
		TutorialManager.ReturnCopyHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(8);
		TutorialManager.ShowTutorial();
	}

	private void OnSelectLevelPlay()
	{
		LobbyManager.LevelPlayButton.GetComponent<Button>().onClick.RemoveListener(OnSelectLevelPlay);
		TutorialManager.ReturnHighLightUI();
		TutorialManager.ClearHand();
		TutorialManager.SetSeq(9);
		TutorialManager.HideTutorial();
	}

	private void OnStoreCollectComplete()
	{
		LobbyManager.StoreCollectComplete = null;
		TutorialManager.ReturnHighLightUI();
		TutorialManager.NextSep();
		TutorialManager.ClearHand();
	}

	private void OnBattleRewardOpenEvent()
	{
		InGamePlayManager.BattleRewardOpen = null;
		TutorialManager.SetSeq(12);
		TutorialManager.SaveTutorial(3, 1);
	}

	private void OnShowBattleReward()
	{
		UnityEngine.Debug.Log("OnShowBattleReward");
		InGamePlayManager.ShowBattleReward = null;
		InGamePlayManager.GameLose = null;
		TutorialManager.SetSeq(11);
		TutorialManager.ShowTutorial();
		TutorialManager.HideTutorial();
	}

	public void onGameLose()
	{
		InGamePlayManager.ShowBattleReward = null;
		InGamePlayManager.GameLose = null;
		TutorialManager.SetSeq(5);
	}
}
