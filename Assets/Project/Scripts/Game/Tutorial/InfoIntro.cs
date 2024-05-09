using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InfoIntro : MonoBehaviour
{
	[FormerlySerializedAs("goCurtainUp")] [SerializeField]
	private GameObject _curtainUp;

	[FormerlySerializedAs("goCurtainDown")] [SerializeField]
	private GameObject _curtainDown;

	[FormerlySerializedAs("goHunterSkillButton")] [SerializeField]
	private GameObject _hunterDownSkill;

	private bool _isBlockSelected;

	private Transform _handTransform;

	private Transform _blockTitleTransform;

	private Transform _skillHunterTransform;

	public void Open(int _seq)
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
				InfoManager.SetClickDimmed(isClick: false);
				GameInfo.inGamePlayData.stage = 0;
				GameInfo.inGamePlayData.chapter = 0;
				GameInfo.inGamePlayData.level = 0;
				GameInfo.inGamePlayData.levelIdx = 0;
				InfoManager.StartSequence(2);
				LobbyManager.GotoInGame(0);
			}
			break;
		case 3:
			_isBlockSelected = false;
			InfoManager.SetClickDimmed(isClick: false);
			PuzzlePlayManager.OnMatchTimeFlow = FlowEvent;
			PuzzlePlayManager.OnClock = SelectOnBlock;
			PuzzlePlayManager.PuzzleTouchEnd = OnTouchEnd;
			PuzzlePlayManager.ActiveBlocks();
			PuzzlePlayManager.ActiveOnlySelectOneBlock(3, 3);
			BlockHand(3, 3, isBottom: true);
			HighLightSomething();
			break;
		case 5:
			_isBlockSelected = false;
			InfoManager.SetClickDimmed(isClick: false);
			PuzzlePlayManager.OnClock = SelectOnBlock;
			PuzzlePlayManager.PuzzleTouchEnd = OnTouchEnd;
			PuzzlePlayManager.ActiveBlocks();
			PuzzlePlayManager.ActiveOnlySelectOneBlock(3, 1);
			BlockHand(3, 1, isBottom: false);
			ShowTutorialSecond();
			break;
		case 8:
			InfoManager.SetClickDimmed(isClick: false);
			PuzzlePlayManager.OnUseHunterSkill = OnHunterSpeell;
			PuzzlePlayManager.SetHunterFullSkillGauge(1);
			PuzzlePlayManager.LockTouch();
			_skillHunterTransform = PuzzlePlayManager.CheckIsUseHunterSkill();
			_hunterDownSkill.transform.position = _skillHunterTransform.position;
			_hunterDownSkill.SetActive(value: true);
			InfoManager.SortLightSprite(_skillHunterTransform);
			break;
		case 9:
			PuzzlePlayManager.LockTouch();
			InfoManager.CloseTutorial();
			InfoManager.NextStepIndex();
			InfoManager.SaveAllData(1, 0, OnIntroComplete);
			break;
		}
	}

	private void OnIntroComplete()
	{
		StartCoroutine(CurtainClose());
	}

	private IEnumerator DeleyRoutine()
	{
		yield return new WaitForSeconds(0.2f);
		PuzzlePlayManager.LockedBlocks();
	}

	private void SelectOnBlock(int _x, int _y)
	{
		switch (InfoManager.Seq)
		{
		case 3:
			if (_x == 3 && _y == 3 && !_isBlockSelected)
			{
				_isBlockSelected = true;
				UnityEngine.Debug.Log("OnBlockSelectEvent 1");
			}
			else if (_x == 2 && _y == 3 && _isBlockSelected)
			{
				UnityEngine.Debug.Log("OnBlockSelectEvent 2");
				_isBlockSelected = false;
				HandRemove();
				PuzzlePlayManager.ActiveBlocks();
				PuzzlePlayManager.OnClock = null;
				PuzzlePlayManager.PuzzleTouchEnd = null;
				InfoManager.ReturnBackAll();
				StartCoroutine(DeleyRoutine());
			}
			break;
		case 5:
			if (_x == 3 && _y == 1 && !_isBlockSelected)
			{
				_isBlockSelected = true;
			}
			else if (_x == 2 && _y == 2 && _isBlockSelected)
			{
				_isBlockSelected = false;
				HandRemove();
				InfoManager.ReturnBackAll();
				InfoManager.StartSequence(7);
				InfoManager.CloseTutorial();
				PuzzlePlayManager.ActiveBlocks();
				PuzzlePlayManager.OnClock = null;
				PuzzlePlayManager.PuzzleTouchEnd = null;
				StartCoroutine(PauseMatchTime());
			}
			break;
		}
	}

	private IEnumerator PauseMatchTime()
	{
		yield return null;
		PuzzlePlayManager.CancelTimer();
	}

	private void OnTouchEnd(Block first, Block second, bool isMatchBlock)
	{
		int seq = InfoManager.Seq;
		if ((seq == 3 || seq == 5) && !isMatchBlock)
		{
			_isBlockSelected = false;
		}
	}

	private void FlowEvent(float time)
	{
		if (time < 4f)
		{
			InfoManager.ReturnBackAll();
			PuzzlePlayManager.OnMatchTimeFlow = null;
			PuzzlePlayManager.PauseTimer();
			InfoManager.StartSequence(4);
			InfoManager.OpenTutorial();
		}
	}

	private void OnHunterSpeell()
	{
		PuzzlePlayManager.ForceMonsterHP();
		PuzzlePlayManager.OnUseHunterSkill = null;
		InfoManager.CloseTutorial();
		InfoManager.ReturnBackAll();
		InfoManager.StartSequence(9);
	}

	private IEnumerator CurtainClose()
	{
		_curtainUp.SetActive(value: true);
		_curtainDown.SetActive(value: true);
		Vector3 curtainPosition = _curtainUp.transform.localPosition;
		curtainPosition.y = 1100f;
		_curtainUp.transform.localPosition = curtainPosition;
		curtainPosition = _curtainDown.transform.localPosition;
		curtainPosition.y = -1100f;
		_curtainDown.transform.localPosition = curtainPosition;
		LeanTween.moveLocalY(_curtainUp, 400f, 4f).setEaseOutCubic();
		LeanTween.moveLocalY(_curtainDown, -400f, 4f).setEaseOutCubic();
		yield return new WaitForSeconds(4f);
		_curtainUp.SetActive(value: false);
		_curtainDown.SetActive(value: false);
		GameDataManager.ShowScenario(2);
	}

	private void BlockHand(int _x, int _y, bool isBottom)
	{
		HandRemove();
		_handTransform = MasterPoolManager.SpawnObject("Tutorial", "Tutorial_Hand");
		_handTransform.position = PuzzlePlayManager.BlockPositions(_x, _y);
		if (isBottom)
		{
			_handTransform.GetComponent<InfoHand>().HandLeftAnim();
		}
		else
		{
			_handTransform.GetComponent<InfoHand>().DiagonalTopLeftHend();
		}
		_blockTitleTransform = MasterPoolManager.SpawnObject("Tutorial", "Tutorial_Tile");
		_blockTitleTransform.position = PuzzlePlayManager.BlockPositions(_x, _y);
	}

	private void HandRemove()
	{
		if (_handTransform != null)
		{
			MasterPoolManager.ReturnToPool("Tutorial", _handTransform);
			_handTransform = null;
		}
		if (_blockTitleTransform != null)
		{
			MasterPoolManager.ReturnToPool("Tutorial", _blockTitleTransform);
			_blockTitleTransform = null;
		}
	}

	private void HighLightSomething()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(BlocksHighLight(0, 3));
		list.AddRange(BlocksHighLight(1, 3));
		list.AddRange(BlocksHighLight(2, 3));
		list.AddRange(BlocksHighLight(3, 3));
		InfoManager.ShowListOfSprites(list);
	}

	private void ShowTutorialSecond()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(BlocksHighLight(0, 3));
		list.AddRange(BlocksHighLight(1, 3));
		list.AddRange(BlocksHighLight(2, 3));
		list.AddRange(BlocksHighLight(2, 0));
		list.AddRange(BlocksHighLight(2, 1));
		list.AddRange(BlocksHighLight(2, 2));
		list.AddRange(BlocksHighLight(3, 1));
		InfoManager.ShowListOfSprites(list);
	}

	private List<SpriteRenderer> BlocksHighLight(int _x, int _y)
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
		PuzzlePlayManager.TutorialHunterSkill(_skillHunterTransform.GetComponent<Hero>());
		_hunterDownSkill.SetActive(value: false);
	}
}
