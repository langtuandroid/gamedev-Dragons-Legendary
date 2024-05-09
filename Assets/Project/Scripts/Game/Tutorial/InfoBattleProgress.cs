using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBattleProgress : MonoBehaviour
{
	private bool _isBlockSelect;

	private Transform _trHand;

	private Transform _trBlock;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 2:
		case 3:
		case 5:
		case 7:
			break;
		case 1:
			if (GameInfo.currentSceneType != SceneType.InGame)
			{
				GameInfo.inGamePlayData.stage = 1;
				GameInfo.inGamePlayData.chapter = 1;
				GameInfo.inGamePlayData.level = 1;
				GameInfo.inGamePlayData.levelIdx = 1;
				InfoManager.StartSequence(2);
				Protocol_Set.Protocol_game_start_Req(1, null, GamePlayComplete);
			}
			break;
		case 4:
			_isBlockSelect = false;
			InfoManager.SetClickDimmed(isClick: false);
			PuzzlePlayManager.OnClock = EventBlock;
			PuzzlePlayManager.PuzzleTouchEnd = TouchEndPuzzle;
			PuzzlePlayManager.ActiveBlocks();
			PuzzlePlayManager.ActiveOnlySelectOneBlock(3, 3);
			HandBlock(3, 3, isBottom: true);
			OpenMatchHighLight();
			break;
		case 6:
			_isBlockSelect = false;
			InfoManager.SetClickDimmed(isClick: false);
			PuzzlePlayManager.OnClock = EventBlock;
			PuzzlePlayManager.PuzzleTouchEnd = TouchEndPuzzle;
			PuzzlePlayManager.ActiveBlocks();
			PuzzlePlayManager.ActiveOnlySelectOneBlock(5, 3);
			HandBlock(5, 3, isBottom: false);
			OpenMatchHighLightSecondary();
			break;
		case 8:
			InfoManager.CloseTutorial();
			if (GameInfo.currentSceneType != SceneType.InGame)
			{
				GameInfo.isForceRandomBlockPattern = true;
				GameInfo.inGamePlayData.stage = 1;
				GameInfo.inGamePlayData.chapter = 1;
				GameInfo.inGamePlayData.level = 1;
				GameInfo.inGamePlayData.levelIdx = 1;
				Protocol_Set.Protocol_game_start_Req(1, null, GamePlayComplete);
			}
			else
			{
				PuzzlePlayManager.OnShowBattleClearResult = ResultButtle;
				PuzzlePlayManager.ShowBattleReward = OnBattleRewardShown;
			}
			break;
		case 9:
			InfoManager.SaveAllData();
			InfoManager.CloseTutorial();
			if (GameInfo.currentSceneType != SceneType.InGame)
			{
				GameInfo.isDirectBattleReward = true;
				GameInfo.inGamePlayData.stage = 1;
				GameInfo.inGamePlayData.chapter = 1;
				GameInfo.inGamePlayData.level = 1;
				GameInfo.inGamePlayData.levelIdx = 1;
				LobbyManager.GotoInGame(1);
			}
			else
			{
				PuzzlePlayManager.OnBattleRewardOpen = OpenEventBattcle;
			}
			break;
		}
	}

	private IEnumerator DelayLockProccess()
	{
		yield return new WaitForSeconds(0.2f);
		PuzzlePlayManager.LockedBlocks();
	}

	private void EventBlock(int _x, int _y)
	{
		switch (InfoManager.Seq)
		{
		case 4:
			if (_x == 3 && _y == 3 && !_isBlockSelect)
			{
				_isBlockSelect = true;
				UnityEngine.Debug.Log("OnBlockSelectEvent 1");
			}
			else if (_x == 3 && _y == 2 && _isBlockSelect)
			{
				UnityEngine.Debug.Log("OnBlockSelectEvent 2");
				_isBlockSelect = false;
				RemoveHand();
				PuzzlePlayManager.ActiveBlocks();
				PuzzlePlayManager.OnMatchTimeFlow = TimeFlowEvent;
				PuzzlePlayManager.OnClock = null;
				PuzzlePlayManager.PuzzleTouchEnd = null;
				InfoManager.ReturnBackAll();
				StartCoroutine(DelayLockProccess());
			}
			break;
		case 6:
			if (_x == 5 && _y == 3 && !_isBlockSelect)
			{
				_isBlockSelect = true;
			}
			else if (_x == 4 && _y == 2 && _isBlockSelect)
			{
				_isBlockSelect = false;
				RemoveHand();
				PuzzlePlayManager.ActiveBlocks();
				PuzzlePlayManager.OnClock = null;
				PuzzlePlayManager.PuzzleTouchEnd = null;
				InfoManager.ReturnBackAll();
				PuzzlePlayManager.ContinueTimer();
				InfoManager.StartSequence(8);
				InfoManager.OpenTutorial();
				PuzzlePlayManager.ActiveBlocks();
			}
			break;
		}
	}

	private void TouchEndPuzzle(Block first, Block second, bool isMatchBlock)
	{
		int seq = InfoManager.Seq;
		if ((seq == 4 || seq == 6) && !isMatchBlock)
		{
			_isBlockSelect = false;
		}
	}

	private IEnumerator TutorialNext()
	{
		PuzzlePlayManager.LockTouch();
		yield return new WaitForSeconds(0.5f);
		PuzzlePlayManager.ActivateTouch();
		InfoManager.ContinueStep();
	}

	private void TimeFlowEvent(float time)
	{
		if (time < 4f)
		{
			InfoManager.ReturnBackAll();
			PuzzlePlayManager.OnMatchTimeFlow = null;
			PuzzlePlayManager.PauseTimer();
			InfoManager.StartSequence(5);
			InfoManager.OpenTutorial();
		}
	}

	private void GamePlayComplete()
	{
		LobbyManager.GotoInGame(1);
	}

	private void OnPuzzleSwitchEvent()
	{
		int seq = InfoManager.Seq;
		if (seq == 14)
		{
			PuzzlePlayManager.ActiveBlocks();
			InfoManager.OpenTutorial();
			RemoveHand();
		}
	}

	private void HandBlock(int _x, int _y, bool isBottom)
	{
		RemoveHand();
		_trHand = MasterPoolManager.SpawnObject("Tutorial", "Tutorial_Hand");
		_trHand.position = PuzzlePlayManager.BlockPositions(_x, _y);
		if (isBottom)
		{
			_trHand.GetComponent<InfoHand>().BottomAnimation();
		}
		else
		{
			_trHand.GetComponent<InfoHand>().DiagonalHand();
		}
		_trBlock = MasterPoolManager.SpawnObject("Tutorial", "Tutorial_Tile");
		_trBlock.position = PuzzlePlayManager.BlockPositions(_x, _y);
	}

	private void RemoveHand()
	{
		if (_trHand != null)
		{
			MasterPoolManager.ReturnToPool("Tutorial", _trHand);
			_trHand = null;
		}
		if (_trBlock != null)
		{
			MasterPoolManager.ReturnToPool("Tutorial", _trBlock);
			_trBlock = null;
		}
	}

	private void OpenMatchHighLight()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(BlockHighLight(1, 2));
		list.AddRange(BlockHighLight(2, 2));
		list.AddRange(BlockHighLight(3, 3));
		InfoManager.ShowListOfSprites(list);
	}

	private void OpenMatchHighLightSecondary()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(BlockHighLight(1, 2));
		list.AddRange(BlockHighLight(2, 2));
		list.AddRange(BlockHighLight(3, 2));
		list.AddRange(BlockHighLight(4, 0));
		list.AddRange(BlockHighLight(4, 1));
		list.AddRange(BlockHighLight(5, 3));
		InfoManager.ShowListOfSprites(list);
	}

	private List<SpriteRenderer> BlockHighLight(int _x, int _y)
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		Transform block = PuzzlePlayManager.GetBlocks(_x, _y);
		SpriteRenderer[] componentsInChildren = block.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		foreach (SpriteRenderer item in componentsInChildren)
		{
			list.Add(item);
		}
		return list;
	}

	private void OpenEventBattcle()
	{
		UnityEngine.Debug.Log("OnBattleRewardOpenEvent");
		InfoManager.NextStepIndex();
		InfoManager.SaveAllData(2, 1);
		PuzzlePlayManager.OnBattleRewardOpen = null;
	}

	private void ResultButtle()
	{
		UnityEngine.Debug.Log("OnShowBattleResult");
		PuzzlePlayManager.OnShowBattleClearResult = null;
		PuzzlePlayManager.ShowBattleReward = null;
		InfoManager.StartSequence(9);
		InfoManager.OpenTutorial();
		InfoManager.CloseTutorial();
	}

	private void OnBattleRewardShown()
	{
		UnityEngine.Debug.Log("OnShowBattleReward");
		PuzzlePlayManager.OnShowBattleClearResult = null;
		PuzzlePlayManager.ShowBattleReward = null;
		InfoManager.StartSequence(9);
		InfoManager.OpenTutorial();
		InfoManager.CloseTutorial();
	}
}
