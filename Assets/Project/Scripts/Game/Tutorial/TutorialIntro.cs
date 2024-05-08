using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIntro : MonoBehaviour
{
	private const float BLOCK_MATCH_LOCK_DELAY_DURATION = 0.2f;

	[SerializeField]
	private GameObject goCurtainUp;

	[SerializeField]
	private GameObject goCurtainDown;

	[SerializeField]
	private GameObject goHunterSkillButton;

	private bool isFirstBlockSelect;

	private Transform trHand;

	private Transform trBlockTile;

	private Transform trUserSkillHunter;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 2:
		case 4:
		case 6:
		case 7:
			break;
		case 1:
			if (GameInfo.currentSceneType != SceneType.InGame)
			{
				TutorialManager.SetDimmedClick(isClick: false);
				GameInfo.inGamePlayData.stage = 0;
				GameInfo.inGamePlayData.chapter = 0;
				GameInfo.inGamePlayData.level = 0;
				GameInfo.inGamePlayData.levelIdx = 0;
				TutorialManager.SetSeq(2);
				LobbyManager.GotoInGame(0);
			}
			break;
		case 3:
			isFirstBlockSelect = false;
			TutorialManager.SetDimmedClick(isClick: false);
			PuzzlePlayManager.OnMatchTimeFlow = OnMatchTimeFlowEvent;
			PuzzlePlayManager.OnClock = OnBlockSelectEvent;
			PuzzlePlayManager.PuzzleTouchEnd = OnPuzzleTouchEnd;
			PuzzlePlayManager.ActiveBlocks();
			PuzzlePlayManager.ActiveOnlySelectOneBlock(3, 3);
			ShowHandBlock(3, 3, isBottom: true);
			ShowMatchHighLightTutorialFirst();
			break;
		case 5:
			isFirstBlockSelect = false;
			TutorialManager.SetDimmedClick(isClick: false);
			PuzzlePlayManager.OnClock = OnBlockSelectEvent;
			PuzzlePlayManager.PuzzleTouchEnd = OnPuzzleTouchEnd;
			PuzzlePlayManager.ActiveBlocks();
			PuzzlePlayManager.ActiveOnlySelectOneBlock(3, 1);
			ShowHandBlock(3, 1, isBottom: false);
			ShowMatchHighLightTutorialSecond();
			break;
		case 8:
			TutorialManager.SetDimmedClick(isClick: false);
			PuzzlePlayManager.OnUseHunterSkill = OnUseHunterSkillEvent;
			PuzzlePlayManager.SetHunterFullSkillGauge(1);
			PuzzlePlayManager.LockTouch();
			trUserSkillHunter = PuzzlePlayManager.CheckIsUseHunterSkill();
			goHunterSkillButton.transform.position = trUserSkillHunter.position;
			goHunterSkillButton.SetActive(value: true);
			TutorialManager.ShowAndSortHighLightSprite(trUserSkillHunter);
			break;
		case 9:
			PuzzlePlayManager.LockTouch();
			TutorialManager.HideTutorial();
			TutorialManager.NextTutorialindex();
			TutorialManager.SaveTutorial(1, 0, OnTutorialIntroComplete);
			break;
		}
	}

	private void OnTutorialIntroComplete()
	{
		StartCoroutine(ProcessCurtainClose());
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
		case 3:
			if (_x == 3 && _y == 3 && !isFirstBlockSelect)
			{
				isFirstBlockSelect = true;
				UnityEngine.Debug.Log("OnBlockSelectEvent 1");
			}
			else if (_x == 2 && _y == 3 && isFirstBlockSelect)
			{
				UnityEngine.Debug.Log("OnBlockSelectEvent 2");
				isFirstBlockSelect = false;
				ClearHand();
				PuzzlePlayManager.ActiveBlocks();
				PuzzlePlayManager.OnClock = null;
				PuzzlePlayManager.PuzzleTouchEnd = null;
				TutorialManager.ReturnHighLightSpriteList();
				StartCoroutine(ProcessDelayLock());
			}
			break;
		case 5:
			if (_x == 3 && _y == 1 && !isFirstBlockSelect)
			{
				isFirstBlockSelect = true;
			}
			else if (_x == 2 && _y == 2 && isFirstBlockSelect)
			{
				isFirstBlockSelect = false;
				ClearHand();
				TutorialManager.ReturnHighLightSpriteList();
				TutorialManager.SetSeq(7);
				TutorialManager.HideTutorial();
				PuzzlePlayManager.ActiveBlocks();
				PuzzlePlayManager.OnClock = null;
				PuzzlePlayManager.PuzzleTouchEnd = null;
				StartCoroutine(ProcessCancelMatchTime());
			}
			break;
		}
	}

	private IEnumerator ProcessCancelMatchTime()
	{
		yield return null;
		PuzzlePlayManager.CancelTimer();
	}

	private void OnPuzzleTouchEnd(Block first, Block second, bool isMatchBlock)
	{
		int seq = TutorialManager.Seq;
		if ((seq == 3 || seq == 5) && !isMatchBlock)
		{
			isFirstBlockSelect = false;
		}
	}

	private void OnMatchTimeFlowEvent(float time)
	{
		if (time < 4f)
		{
			TutorialManager.ReturnHighLightSpriteList();
			PuzzlePlayManager.OnMatchTimeFlow = null;
			PuzzlePlayManager.PauseTimer();
			TutorialManager.SetSeq(4);
			TutorialManager.ShowTutorial();
		}
	}

	private void OnUseHunterSkillEvent()
	{
		PuzzlePlayManager.ForceMonsterHP();
		PuzzlePlayManager.OnUseHunterSkill = null;
		TutorialManager.HideTutorial();
		TutorialManager.ReturnHighLightSpriteList();
		TutorialManager.SetSeq(9);
	}

	private IEnumerator ProcessCurtainClose()
	{
		goCurtainUp.SetActive(value: true);
		goCurtainDown.SetActive(value: true);
		Vector3 curtainPosition = goCurtainUp.transform.localPosition;
		curtainPosition.y = 1100f;
		goCurtainUp.transform.localPosition = curtainPosition;
		curtainPosition = goCurtainDown.transform.localPosition;
		curtainPosition.y = -1100f;
		goCurtainDown.transform.localPosition = curtainPosition;
		LeanTween.moveLocalY(goCurtainUp, 400f, 4f).setEaseOutCubic();
		LeanTween.moveLocalY(goCurtainDown, -400f, 4f).setEaseOutCubic();
		yield return new WaitForSeconds(4f);
		goCurtainUp.SetActive(value: false);
		goCurtainDown.SetActive(value: false);
		GameDataManager.ShowScenario(2);
	}

	private void ShowHandBlock(int _x, int _y, bool isBottom)
	{
		ClearHand();
		trHand = MWPoolManager.Spawn("Tutorial", "Tutorial_Hand");
		trHand.position = PuzzlePlayManager.BlockPositions(_x, _y);
		if (isBottom)
		{
			trHand.GetComponent<TutorialHand>().ShowHandLeftAnim();
		}
		else
		{
			trHand.GetComponent<TutorialHand>().ShowHandDiagonalAnimTopLeft();
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
		list.AddRange(GetHighLightBlock(0, 3));
		list.AddRange(GetHighLightBlock(1, 3));
		list.AddRange(GetHighLightBlock(2, 3));
		list.AddRange(GetHighLightBlock(3, 3));
		TutorialManager.ShowHighLightSpriteList(list);
	}

	private void ShowMatchHighLightTutorialSecond()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(GetHighLightBlock(0, 3));
		list.AddRange(GetHighLightBlock(1, 3));
		list.AddRange(GetHighLightBlock(2, 3));
		list.AddRange(GetHighLightBlock(2, 0));
		list.AddRange(GetHighLightBlock(2, 1));
		list.AddRange(GetHighLightBlock(2, 2));
		list.AddRange(GetHighLightBlock(3, 1));
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

	public void OnClickHunterSkill()
	{
		PuzzlePlayManager.TutorialHunterSkill(trUserSkillHunter.GetComponent<Hero>());
		goHunterSkillButton.SetActive(value: false);
	}
}
