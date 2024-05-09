using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDragMatch : MonoBehaviour
{
	private bool _blockSelect;

	private Transform _hand;

	private Transform _blockTitle;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 3:
			InfoManager.SetClickDimmed(isClick: false);
			PuzzlePlayManager.OnClock = SelectBlockEvent;
			PuzzlePlayManager.PuzzleTouchEnd = OnTouchOver;
			PuzzlePlayManager.ActiveOnlySelectOneBlock(3, 2);
			PuzzlePlayManager.SelectDeBlocks();
			HandBlock(3, 2);
			ShowDragHighLightTutorialFirst();
			break;
		case 5:
			InfoManager.SetClickDimmed(isClick: false);
			PuzzlePlayManager.OnClock = SelectBlockEvent;
			PuzzlePlayManager.ActiveOnlySelectOneBlock(4, 3);
			PuzzlePlayManager.SelectDeBlocks();
			HandBlock(4, 3, 1);
			ShowDragHighLightTutorialSecond();
			break;
		}
	}

	private void SelectBlockEvent(int _x, int _y)
	{
		switch (InfoManager.Seq)
		{
		case 3:
			if (_x == 3 && _y == 2 && !_blockSelect)
			{
				_blockSelect = true;
			}
			else if (_x == 2 && _y == 2 && _blockSelect)
			{
				_blockSelect = false;
				RemoveAllFromHand();
				PuzzlePlayManager.ActiveBlocks();
				InfoManager.ReturnBackAll();
				PuzzlePlayManager.OnClock = null;
				StartCoroutine(StopTimer());
			}
			break;
		case 5:
			if (_x == 4 && _y == 3 && !_blockSelect)
			{
				_blockSelect = true;
			}
			else if (_x == 3 && _y == 2 && _blockSelect)
			{
				_blockSelect = false;
				RemoveAllFromHand();
				PuzzlePlayManager.ActiveBlocks();
				InfoManager.ReturnBackAll();
				PuzzlePlayManager.OnClock = null;
				StartCoroutine(StopTimer());
			}
			break;
		}
	}

	private IEnumerator StopTimer()
	{
		PuzzlePlayManager.LockTouch();
		yield return new WaitForSeconds(0.3f);
		PuzzlePlayManager.ActivateTouch();
		PuzzlePlayManager.PauseTimer();
		InfoManager.ContinueStep();
	}

	private void OnTouchOver(Block first, Block second, bool isMatchBlock)
	{
	}

	private void HandBlock(int _x, int _y, int type = 0)
	{
		RemoveAllFromHand();
		_hand = MasterPoolManager.SpawnObject("Tutorial", "Tutorial_Hand");
		_hand.position = PuzzlePlayManager.BlockPositions(_x, _y);
		switch (type)
		{
		case 0:
			_hand.GetComponent<InfoHand>().HandLeftAnim();
			break;
		case 1:
			_hand.GetComponent<InfoHand>().DiagonalHand();
			break;
		}
		_blockTitle = MasterPoolManager.SpawnObject("Tutorial", "Tutorial_Tile");
		_blockTitle.position = PuzzlePlayManager.BlockPositions(_x, _y);
	}

	private void RemoveAllFromHand()
	{
		if (_hand != null)
		{
			MasterPoolManager.ReturnToPool("Tutorial", _hand);
			_hand = null;
		}
		if (_blockTitle != null)
		{
			MasterPoolManager.ReturnToPool("Tutorial", _blockTitle);
			_blockTitle = null;
		}
	}

	private void ShowDragHighLightTutorialFirst()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(GetHighLightBlock(2, 0));
		list.AddRange(GetHighLightBlock(2, 1));
		list.AddRange(GetHighLightBlock(2, 2));
		list.AddRange(GetHighLightBlock(2, 3));
		list.AddRange(GetHighLightBlock(2, 4));
		list.AddRange(GetHighLightBlock(3, 2));
		InfoManager.ShowListOfSprites(list);
	}

	private void ShowDragHighLightTutorialSecond()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		list.AddRange(GetHighLightBlock(2, 0));
		list.AddRange(GetHighLightBlock(2, 1));
		list.AddRange(GetHighLightBlock(2, 2));
		list.AddRange(GetHighLightBlock(2, 3));
		list.AddRange(GetHighLightBlock(2, 4));
		list.AddRange(GetHighLightBlock(3, 2));
		list.AddRange(GetHighLightBlock(4, 2));
		list.AddRange(GetHighLightBlock(4, 3));
		InfoManager.ShowListOfSprites(list);
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
}
