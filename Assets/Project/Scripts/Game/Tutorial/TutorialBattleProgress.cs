using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBattleProgress : MonoBehaviour
{
	private const float BLOCK_MATCH_LOCK_DELAY_DURATION = 0.2f;

	private bool isFirstBlockSelect;

	private Transform trHand;

	private Transform trBlockTile;

	public void Show(int _seq)
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
				TutorialManager.SetSeq(2);
				Protocol_Set.Protocol_game_start_Req(1, null, OnGamePlayConncectComplete);
			}
			break;
		case 4:
			isFirstBlockSelect = false;
			TutorialManager.SetDimmedClick(isClick: false);
			PuzzlePlayManager.OnClock = OnBlockSelectEvent;
			PuzzlePlayManager.PuzzleTouchEnd = OnPuzzleTouchEnd;
			PuzzlePlayManager.ActiveBlocks();
			PuzzlePlayManager.ActiveOnlySelectOneBlock(3, 3);
			ShowHandBlock(3, 3, isBottom: true);
			ShowMatchHighLightTutorialFirst();
			break;
		case 6:
			isFirstBlockSelect = false;
			TutorialManager.SetDimmedClick(isClick: false);
			PuzzlePlayManager.OnClock = OnBlockSelectEvent;
			PuzzlePlayManager.PuzzleTouchEnd = OnPuzzleTouchEnd;
			PuzzlePlayManager.ActiveBlocks();
			PuzzlePlayManager.ActiveOnlySelectOneBlock(5, 3);
			ShowHandBlock(5, 3, isBottom: false);
			ShowMatchHighLightTutorialSecond();
			break;
		case 8:
			TutorialManager.HideTutorial();
			if (GameInfo.currentSceneType != SceneType.InGame)
			{
				GameInfo.isForceRandomBlockPattern = true;
				GameInfo.inGamePlayData.stage = 1;
				GameInfo.inGamePlayData.chapter = 1;
				GameInfo.inGamePlayData.level = 1;
				GameInfo.inGamePlayData.levelIdx = 1;
				Protocol_Set.Protocol_game_start_Req(1, null, OnGamePlayConncectComplete);
			}
			else
			{
				PuzzlePlayManager.OnShowBattleClearResult = OnShowBattleResult;
				PuzzlePlayManager.ShowBattleReward = OnShowBattleReward;
			}
			break;
		case 9:
			TutorialManager.SaveTutorial();
			TutorialManager.HideTutorial();
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
				PuzzlePlayManager.OnBattleRewardOpen = OnBattleRewardOpenEvent;
			}
			break;
		}
	}

	private IEnumerator ProcessDelayLock()
	{
		yield return new WaitForSeconds(0.2f);
		PuzzlePlayManager.LockedBlocks();
	}

	private void OnBlockSelectEvent(int _x, int _y)
	{
		switch (TutorialManager.Seq)
		{
		case 4:
			if (_x == 3 && _y == 3 && !isFirstBlockSelect)
			{
				isFirstBlockSelect = true;
				UnityEngine.Debug.Log("OnBlockSelectEvent 1");
			}
			else if (_x == 3 && _y == 2 && isFirstBlockSelect)
			{
				UnityEngine.Debug.Log("OnBlockSelectEvent 2");
				isFirstBlockSelect = false;
				ClearHand();
				PuzzlePlayManager.ActiveBlocks();
				PuzzlePlayManager.OnMatchTimeFlow = OnMatchTimeFlowEvent;
				PuzzlePlayManager.OnClock = null;
				PuzzlePlayManager.PuzzleTouchEnd = null;
				TutorialManager.ReturnHighLightSpriteList();
				StartCoroutine(ProcessDelayLock());
			}
			break;
		case 6:
			if (_x == 5 && _y == 3 && !isFirstBlockSelect)
			{
				isFirstBlockSelect = true;
			}
			else if (_x == 4 && _y == 2 && isFirstBlockSelect)
			{
				isFirstBlockSelect = false;
				ClearHand();
				PuzzlePlayManager.ActiveBlocks();
				PuzzlePlayManager.OnClock = null;
				PuzzlePlayManager.PuzzleTouchEnd = null;
				TutorialManager.ReturnHighLightSpriteList();
				PuzzlePlayManager.ContinueTimer();
				TutorialManager.SetSeq(8);
				TutorialManager.ShowTutorial();
				PuzzlePlayManager.ActiveBlocks();
			}
			break;
		}
	}

	private void OnPuzzleTouchEnd(Block first, Block second, bool isMatchBlock)
	{
		int seq = TutorialManager.Seq;
		if ((seq == 4 || seq == 6) && !isMatchBlock)
		{
			isFirstBlockSelect = false;
		}
	}

	private IEnumerator ProcessNextTutorialNext()
	{
		PuzzlePlayManager.LockTouch();
		yield return new WaitForSeconds(0.5f);
		PuzzlePlayManager.ActivateTouch();
		TutorialManager.NextSep();
	}

	private void OnMatchTimeFlowEvent(float time)
	{
		if (time < 4f)
		{
			TutorialManager.ReturnHighLightSpriteList();
			PuzzlePlayManager.OnMatchTimeFlow = null;
			PuzzlePlayManager.PauseTimer();
			TutorialManager.SetSeq(5);
			TutorialManager.ShowTutorial();
		}
	}

	private void OnGamePlayConncectComplete()
	{
		LobbyManager.GotoInGame(1);
	}

	private void OnPuzzleSwitchEvent()
	{
		int seq = TutorialManager.Seq;
		if (seq == 14)
		{
			PuzzlePlayManager.ActiveBlocks();
			TutorialManager.ShowTutorial();
			ClearHand();
		}
	}

	private void ShowHandBlock(int _x, int _y, bool isBottom)
	{
		ClearHand();
		trHand = MWPoolManager.Spawn("Tutorial", "Tutorial_Hand");
		trHand.position = PuzzlePlayManager.BlockPositions(_x, _y);
		if (isBottom)
		{
			trHand.GetComponent<TutorialHand>().ShowHandBottomAnim();
		}
		else
		{
			trHand.GetComponent<TutorialHand>().ShowHandDiagonalAnim();
		}
		trBlockTile = MWPoolManager.Spawn("Tutorial", "Tutorial_Tile");
		trBlockTile.position = PuzzlePlayManager.BlockPositions(_x, _y);
	}

	private void ClearHand()
	{
		if (trHand != null)
		{
			MWPoolManager.DeSpawn("Tutorial", trHand);
			trHand = null;
		}
		if (trBlockTile != null)
		{
			MWPoolManager.DeSpawn("Tutorial", trBlockTile);
			trBlockTile = null;
		}
	}

	private void ShowMatchHighLightTutorialFirst()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(GetHighLightBlock(1, 2));
		list.AddRange(GetHighLightBlock(2, 2));
		list.AddRange(GetHighLightBlock(3, 3));
		TutorialManager.ShowHighLightSpriteList(list);
	}

	private void ShowMatchHighLightTutorialSecond()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(GetHighLightBlock(1, 2));
		list.AddRange(GetHighLightBlock(2, 2));
		list.AddRange(GetHighLightBlock(3, 2));
		list.AddRange(GetHighLightBlock(4, 0));
		list.AddRange(GetHighLightBlock(4, 1));
		list.AddRange(GetHighLightBlock(5, 3));
		TutorialManager.ShowHighLightSpriteList(list);
	}

	private List<SpriteRenderer> GetHighLightBlock(int _x, int _y)
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

	private void OnBattleRewardOpenEvent()
	{
		UnityEngine.Debug.Log("OnBattleRewardOpenEvent");
		TutorialManager.NextTutorialindex();
		TutorialManager.SaveTutorial(2, 1);
		PuzzlePlayManager.OnBattleRewardOpen = null;
	}

	private void OnShowBattleResult()
	{
		UnityEngine.Debug.Log("OnShowBattleResult");
		PuzzlePlayManager.OnShowBattleClearResult = null;
		PuzzlePlayManager.ShowBattleReward = null;
		TutorialManager.SetSeq(9);
		TutorialManager.ShowTutorial();
		TutorialManager.HideTutorial();
	}

	private void OnShowBattleReward()
	{
		UnityEngine.Debug.Log("OnShowBattleReward");
		PuzzlePlayManager.OnShowBattleClearResult = null;
		PuzzlePlayManager.ShowBattleReward = null;
		TutorialManager.SetSeq(9);
		TutorialManager.ShowTutorial();
		TutorialManager.HideTutorial();
	}
}
