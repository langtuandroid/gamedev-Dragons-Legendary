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
			InGamePlayManager.BlockSelect = OnBlockSelectEvent;
			InGamePlayManager.PuzzleTouchEnd = OnPuzzleTouchEnd;
			InGamePlayManager.AllActiveBlock();
			InGamePlayManager.ActiveOnlySelectOneBlock(3, 3);
			ShowHandBlock(3, 3, isBottom: true);
			ShowMatchHighLightTutorialFirst();
			break;
		case 6:
			isFirstBlockSelect = false;
			TutorialManager.SetDimmedClick(isClick: false);
			InGamePlayManager.BlockSelect = OnBlockSelectEvent;
			InGamePlayManager.PuzzleTouchEnd = OnPuzzleTouchEnd;
			InGamePlayManager.AllActiveBlock();
			InGamePlayManager.ActiveOnlySelectOneBlock(5, 3);
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
				InGamePlayManager.ShowBattleClearResult = OnShowBattleResult;
				InGamePlayManager.ShowBattleReward = OnShowBattleReward;
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
				InGamePlayManager.BattleRewardOpen = OnBattleRewardOpenEvent;
			}
			break;
		}
	}

	private IEnumerator ProcessDelayLock()
	{
		yield return new WaitForSeconds(0.2f);
		InGamePlayManager.AllLockBlock();
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
				InGamePlayManager.AllActiveBlock();
				InGamePlayManager.MatchTimeFlow = OnMatchTimeFlowEvent;
				InGamePlayManager.BlockSelect = null;
				InGamePlayManager.PuzzleTouchEnd = null;
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
				InGamePlayManager.AllActiveBlock();
				InGamePlayManager.BlockSelect = null;
				InGamePlayManager.PuzzleTouchEnd = null;
				TutorialManager.ReturnHighLightSpriteList();
				InGamePlayManager.ResumeMatchTimer();
				TutorialManager.SetSeq(8);
				TutorialManager.ShowTutorial();
				InGamePlayManager.AllActiveBlock();
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
		InGamePlayManager.TouchLock();
		yield return new WaitForSeconds(0.5f);
		InGamePlayManager.TouchActive();
		TutorialManager.NextSep();
	}

	private void OnMatchTimeFlowEvent(float time)
	{
		if (time < 4f)
		{
			TutorialManager.ReturnHighLightSpriteList();
			InGamePlayManager.MatchTimeFlow = null;
			InGamePlayManager.StopMatchTimer();
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
			InGamePlayManager.AllActiveBlock();
			TutorialManager.ShowTutorial();
			ClearHand();
		}
	}

	private void ShowHandBlock(int _x, int _y, bool isBottom)
	{
		ClearHand();
		trHand = MWPoolManager.Spawn("Tutorial", "Tutorial_Hand");
		trHand.position = InGamePlayManager.GetBlockPosition(_x, _y);
		if (isBottom)
		{
			trHand.GetComponent<TutorialHand>().ShowHandBottomAnim();
		}
		else
		{
			trHand.GetComponent<TutorialHand>().ShowHandDiagonalAnim();
		}
		trBlockTile = MWPoolManager.Spawn("Tutorial", "Tutorial_Tile");
		trBlockTile.position = InGamePlayManager.GetBlockPosition(_x, _y);
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
		Transform block = InGamePlayManager.GetBlock(_x, _y);
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
		InGamePlayManager.BattleRewardOpen = null;
	}

	private void OnShowBattleResult()
	{
		UnityEngine.Debug.Log("OnShowBattleResult");
		InGamePlayManager.ShowBattleClearResult = null;
		InGamePlayManager.ShowBattleReward = null;
		TutorialManager.SetSeq(9);
		TutorialManager.ShowTutorial();
		TutorialManager.HideTutorial();
	}

	private void OnShowBattleReward()
	{
		UnityEngine.Debug.Log("OnShowBattleReward");
		InGamePlayManager.ShowBattleClearResult = null;
		InGamePlayManager.ShowBattleReward = null;
		TutorialManager.SetSeq(9);
		TutorialManager.ShowTutorial();
		TutorialManager.HideTutorial();
	}
}
