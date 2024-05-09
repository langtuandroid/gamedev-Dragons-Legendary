using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoStoreManagement : MonoBehaviour
{
	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 3:
		case 4:
		case 10:
			break;
		case 2:
			StartCoroutine(FocusFloorItem());
			break;
		case 5:
			InfoManager.SetClickDimmed(isClick: false);
			InfoManager.HighLightUI(LobbyManager.BattleButton);
			InfoManager.HandAppear(LobbyManager.BattleButton, new Vector3(0f, 0.5f, 0f));
			LobbyManager.OpenStageSelect = StageSelected;
			break;
		case 6:
			InfoManager.SetClickDimmed(isClick: false);
			StartCoroutine(CheckDelayStage());
			break;
		case 7:
			InfoManager.SetClickDimmed(isClick: false);
			StartCoroutine(ShowLevelCell());
			break;
		case 8:
			GameInfo.inGamePlayData.stage = 1;
			GameInfo.inGamePlayData.chapter = 1;
			GameInfo.inGamePlayData.level = 2;
			GameInfo.inGamePlayData.levelIdx = 2;
			InfoManager.SetClickDimmed(isClick: false);
			InfoManager.HighLightUI(LobbyManager.LevelPlayButton);
			InfoManager.HandAppear(LobbyManager.LevelPlayButton, Vector3.zero);
			LobbyManager.LevelPlayButton.GetComponent<Button>().onClick.AddListener(LevelPlaySelec);
			break;
		case 9:
			InfoManager.CloseTutorial();
			PuzzlePlayManager.ShowBattleReward = RewardCollect;
			PuzzlePlayManager.OnGameLose = WhenGameLose;
			break;
		case 11:
			InfoManager.SaveAllData();
			InfoManager.CloseTutorial();
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
				PuzzlePlayManager.OnBattleRewardOpen = BattleOverCompleted;
			}
			break;
		case 12:
			InfoManager.SetClickDimmed(isClick: false);
			InfoManager.SaveAllData();
			StartCoroutine(CollectShow());
			break;
		case 13:
			InfoManager.SetClickDimmed(isClick: false);
			InfoManager.SaveAllData();
			InfoManager.HighLightUI(LobbyManager.BattleButton);
			InfoManager.HandAppear(LobbyManager.BattleButton, new Vector3(0f, 0.5f, 0f));
			LobbyManager.OpenStageSelect = StageSelected;
			break;
		}
	}

	private IEnumerator FocusFloorItem()
	{
		LobbyManager.MoveStore(0, 0);
		yield return null;
		InfoManager.HighLightUI(LobbyManager.FirstFloorOpenButton);
		InfoManager.HandAppear(LobbyManager.FirstFloorOpenButton, new Vector3(-1f, 0f, 0f));
		InfoManager.SetClickDimmed(isClick: false);
		LobbyManager.StartStoreOpen = OpenStore;
	}

	private IEnumerator CheckDelayStage()
	{
		yield return null;
		Transform trCopyCell = InfoManager.ShowCopyHighLight(LobbyManager.FirstStageCell);
		trCopyCell.GetComponent<StageCell>().SetForceTouch();
		trCopyCell.GetComponent<StageCell>().SelectStageEvent = CellStageSelected;
		trCopyCell.position = LobbyManager.FirstStageCell.position;
		trCopyCell.localScale = Vector3.one;
		InfoManager.HandAppear(trCopyCell.GetComponent<StageCell>().SelectButton, Vector3.zero);
	}

	private IEnumerator ShowLevelCell()
	{
		yield return null;
		LevelGameBlock secondCell = LobbyManager.SecondLevelCell;
		LevelGameDbData data = GameDataManager.GetLevelIndexDbData(2);
		LevelGameBlock copySecondLevelCell = InfoManager.ShowCopyHighLight(secondCell.transform).GetComponent<LevelGameBlock>();
		copySecondLevelCell.transform.position = secondCell.transform.position;
		copySecondLevelCell.transform.localScale = Vector3.one;
		copySecondLevelCell.SetData(data);
		copySecondLevelCell.SetUnLock();
		copySecondLevelCell.OnSelectLevelCell = SelectedLevelCell;
		InfoManager.HandAppear(copySecondLevelCell.transform, Vector3.zero);
	}

	private IEnumerator CollectShow()
	{
		LobbyManager.MoveStore(0, 0);
		yield return null;
		InfoManager.HighLightUI(LobbyManager.FirstFloorCollectButton);
		InfoManager.HandAppear(LobbyManager.FirstFloorCollectButton, new Vector3(-1f, 0f, 0f));
		LobbyManager.StoreCollectComplete = StoreComplete;
	}

	private void OpenStore()
	{
		InfoManager.SetClickDimmed(isClick: true);
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(3);
		InfoManager.OpenTutorial();
		InfoManager.SaveAllData(2, 5);
		LobbyManager.StartStoreOpen = null;
	}

	private void StageSelected()
	{
		switch (InfoManager.Seq)
		{
		case 5:
			InfoManager.ReturnUILight();
			InfoManager.RemoveHand();
			InfoManager.StartSequence(6);
			InfoManager.OpenTutorial();
			break;
		case 13:
			InfoManager.CloseTutorial();
			InfoManager.RemoveHand();
			InfoManager.ReturnUILight();
			InfoManager.NextStepIndex();
			break;
		}
		LobbyManager.OpenStageSelect = null;
	}

	private void CellStageSelected(int index)
	{
		InfoManager.ReturnCopy();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(7);
		InfoManager.OpenTutorial();
		LobbyManager.ShowChapterList(1);
	}

	private void SelectedLevelCell()
	{
		InfoManager.ReturnCopy();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(8);
		InfoManager.OpenTutorial();
	}

	private void LevelPlaySelec()
	{
		LobbyManager.LevelPlayButton.GetComponent<Button>().onClick.RemoveListener(LevelPlaySelec);
		InfoManager.ReturnUILight();
		InfoManager.RemoveHand();
		InfoManager.StartSequence(9);
		InfoManager.CloseTutorial();
	}

	private void StoreComplete()
	{
		LobbyManager.StoreCollectComplete = null;
		InfoManager.ReturnUILight();
		InfoManager.ContinueStep();
		InfoManager.RemoveHand();
	}

	private void BattleOverCompleted()
	{
		PuzzlePlayManager.OnBattleRewardOpen = null;
		InfoManager.StartSequence(12);
		InfoManager.SaveAllData(3, 1);
	}

	private void RewardCollect()
	{
		UnityEngine.Debug.Log("OnShowBattleReward");
		PuzzlePlayManager.ShowBattleReward = null;
		PuzzlePlayManager.OnGameLose = null;
		InfoManager.StartSequence(11);
		InfoManager.OpenTutorial();
		InfoManager.CloseTutorial();
	}

	public void WhenGameLose()
	{
		PuzzlePlayManager.ShowBattleReward = null;
		PuzzlePlayManager.OnGameLose = null;
		InfoManager.StartSequence(5);
	}
}
