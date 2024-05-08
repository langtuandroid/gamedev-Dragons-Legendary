using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
	public Action<int, int> BlockSelect;

	public Action PuzzleSwitch;

	public Action<Block, Block, bool> PuzzleTouchEnd;

	private const float BLOCK_SCALE = 1.02f;

	private const float BLOCK_GAP = 0f;

	private const float BLOCK_MATCH_DELAY_DURATION = 0.3f;

	private const float BLOCK_ATTACK_EFFECT_DURATION = 0.2f;

	private const float SPECIAL_BLOCK_POSITION_EFFECT_DURATION = 0.2f;

	private const float HINT_BLOCK_SHOW_DURATION = 2f;

	private const float HINT_BLOCK_SHOW_DELAY = 3f;

	private const int BLOCK_MATCH_HINT_CHECK_START_X = 3;

	private const int BLOCK_MATCH_HINT_CHECK_START_Y = 2;

	private const float CHANGE_SPECIAL_BLOCK_DURATION = 0.3f;

	[SerializeField]
	private int width = 11;

	[SerializeField]
	private int height = 7;

	private bool isTouchControl = true;

	private bool isSwapProcess;

	private bool isMatchEndProcess;

	private bool isDropBlockMathReadyProcess;

	private bool isControlUp;

	private bool isDragLock;

	private float touchMoveLimitDistance = 0.4f;

	private float prevMatchTime;

	private float hintDelayTimd = 3f;

	private float swapTime = 0.1f;

	private Transform trController;

	private Coroutine coroutineMathDown;

	[SerializeField]
	private Block firstBlock;

	[SerializeField]
	private Block secondBlock;

	private Block hintBlock;

	private Block hintSwitchBlock;

	private Vector2 prevMousePos;

	private Block[,] arrBlock;

	private Vector2[,] arrBlockPosition;

	private Coroutine coroutineBlockMatchEnd;

	private Coroutine coroutineHintBlock;

	private List<MatchData> matchList = new List<MatchData>();

	[SerializeField]
	private List<Block> listDropBlock = new List<Block>();

	private List<List<Block>> listMatchReadyBlock = new List<List<Block>>();

	private List<Block> listHintCheckBlock = new List<Block>();

	public Vector2 DamagePosition => arrBlock[3, 2].transform.position;

	public void Init()
	{
		InitializeBlock();
	}

	public void ControlStart()
	{
		isTouchControl = true;
		ShowHintBlock();
	}

	public void ActiveBlockType(BlockType _type)
	{
		Pallete.Active(_type);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				if (block.Type == _type)
				{
					block.RefreshPattern();
				}
			}
		}
	}

	public void DeActiveBlockType(BlockType _type)
	{
		Pallete.DeActive(_type);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				if (block.Type == _type)
				{
					block.RefreshPattern();
				}
			}
		}
	}

	public void SetExceptionBlock(BlockExceptionType _type)
	{
		bool flag = true;
		while (flag)
		{
			int num = UnityEngine.Random.Range(0, width - 1);
			int num2 = UnityEngine.Random.Range(0, height - 1);
			if (arrBlock[num, num2].ExceptionType == BlockExceptionType.None)
			{
				arrBlock[num, num2].SetException(_type);
				flag = false;
			}
		}
	}

	public void ControlLock()
	{
		if ((bool)firstBlock)
		{
			firstBlock.DeSelect(out firstBlock);
		}
		if ((bool)secondBlock)
		{
			secondBlock.DeSelect(out secondBlock);
		}
		isTouchControl = false;
		CancelHintBlock();
	}

	public void ControlEndAndMatch()
	{
		if ((bool)firstBlock)
		{
			firstBlock.DeSelect(out firstBlock);
		}
		if ((bool)secondBlock)
		{
			secondBlock.DeSelect(out secondBlock);
		}
		isTouchControl = false;
		PuzzlePlayManager.BlockAttacl();
		StartMatch();
		CancelHintBlock();
	}

	public void Lock()
	{
		if ((bool)firstBlock)
		{
			firstBlock.DeSelect(out firstBlock);
		}
		if ((bool)secondBlock)
		{
			secondBlock.DeSelect(out secondBlock);
		}
		isTouchControl = false;
		CancelHintBlock();
	}

	public void GameOver()
	{
		PuzzlePlayManager.OnTouchBegin = (Action<Vector3, RaycastHit2D>)Delegate.Remove(PuzzlePlayManager.OnTouchBegin, new Action<Vector3, RaycastHit2D>(OnTouchBeginEvent));
		PuzzlePlayManager.OnTouchMove = (Action<Vector3>)Delegate.Remove(PuzzlePlayManager.OnTouchMove, new Action<Vector3>(OnTouchMoveEvent));
		PuzzlePlayManager.OnTouchEnd = (Action<Vector3>)Delegate.Remove(PuzzlePlayManager.OnTouchEnd, new Action<Vector3>(OnTouchEndEvent));
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				arrBlock[i, j].BlockSelect = null;
			}
		}
	}

	public void GameContinue()
	{
		isTouchControl = true;
		PuzzlePlayManager.OnTouchBegin = (Action<Vector3, RaycastHit2D>)Delegate.Combine(PuzzlePlayManager.OnTouchBegin, new Action<Vector3, RaycastHit2D>(OnTouchBeginEvent));
		PuzzlePlayManager.OnTouchMove = (Action<Vector3>)Delegate.Combine(PuzzlePlayManager.OnTouchMove, new Action<Vector3>(OnTouchMoveEvent));
		PuzzlePlayManager.OnTouchEnd = (Action<Vector3>)Delegate.Combine(PuzzlePlayManager.OnTouchEnd, new Action<Vector3>(OnTouchEndEvent));
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				arrBlock[i, j].BlockSelect = OnBlockSelectEvent;
			}
		}
		ShowHintBlock();
	}

	public void ActiveOnlySelectOneBlock(int _x, int _y)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				block.SetSelect(i == _x && j == _y);
			}
		}
	}

	public void AddActiveSelectBlock(int _x, int _y)
	{
		arrBlock[_x, _y].SetSelect(_isTouch: true);
	}

	public void ActiveOnlyDeselectOneBlock(int _x, int _y)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				block.SetDeselect(i == _x && j == _y);
			}
		}
	}

	public void StopDeSelectAllBlock()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				block.SetDeselect(_isTouch: false);
			}
		}
	}

	public void AllDeSelectBlock()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				block.DeSelect(out block, isCheckMove: false);
			}
		}
		firstBlock = null;
		secondBlock = null;
	}

	public void AllActiveBlock()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				arrBlock[i, j].TouchActive();
			}
		}
	}

	public void AllLockBlock()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				arrBlock[i, j].TouchLock();
			}
		}
	}

	public void SetDragLock()
	{
		isDragLock = true;
	}

	public Vector3 GetBlockPosition(int _x, int _y)
	{
		return arrBlock[_x, _y].transform.position;
	}

	public Transform GetBlock(int _x, int _y)
	{
		return arrBlock[_x, _y].transform;
	}

	public void ChangeBlockType(BlockType _from, BlockType _to, int _skillIdx)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				if (block.Type == _from)
				{
					block.SetPattern(_to);
					Transform transform = MasterPoolManager.SpawnObject("Effect", "Skill_" + _skillIdx, null, 1.5f);
					transform.position = block.transform.position;
				}
			}
		}
	}

	public void ChangeSpecialBlock()
	{
		bool flag = true;
		while (flag)
		{
			int num = UnityEngine.Random.Range(0, width - 1);
			int num2 = UnityEngine.Random.Range(0, height - 1);
			if (arrBlock[num, num2].Type != BlockType.White && arrBlock[num, num2].SpeciaData.type == SpecialBlockType.None && arrBlock[num, num2].ExceptionType == BlockExceptionType.None)
			{
				SpecialBlockType specialBlockType = SpecialBlockType.None;
				int num3 = UnityEngine.Random.Range(0, 100);
				specialBlockType = ((num3 < 50) ? SpecialBlockType.One : ((num3 >= 80) ? SpecialBlockType.Three : SpecialBlockType.Two));
				StartCoroutine(ProcessChangeSpecialBlock(arrBlock[num, num2], (int)(6 + specialBlockType)));
				flag = false;
			}
		}
	}

	private void InitializeBlock()
	{
		trController.localPosition = new Vector2(-3.06f, -5.66f);
		arrBlock = new Block[width, height];
		arrBlockPosition = new Vector2[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Transform transform = MasterPoolManager.SpawnObject("Puzzle", "Block");
				transform.parent = trController;
				transform.gameObject.name = "Block";
				transform.localPosition = new Vector2((float)i * 1.02f, (float)j * 1.02f);
				Block component = transform.GetComponent<Block>();
				component.Init();
				component.On();
				Block block = component;
				int x = i;
				int y = j;
				Vector3 localPosition = transform.localPosition;
				float x2 = localPosition.x;
				Vector3 localPosition2 = transform.localPosition;
				block.SetData(new BlockData(x, y, x2, localPosition2.y));
				component.BlockMoveComplete = OnBlockMoveComplete;
				Block block2 = component;
				block2.BlockSelect = (Action<int, int>)Delegate.Combine(block2.BlockSelect, new Action<int, int>(OnBlockSelectEvent));
				arrBlock[i, j] = component;
				ref Vector2 reference = ref arrBlockPosition[i, j];
				Vector3 localPosition3 = transform.localPosition;
				float x3 = localPosition3.x;
				Vector3 localPosition4 = transform.localPosition;
				reference = new Vector2(x3, localPosition4.y);
			}
		}
		Pallete.Init();
		RegisterTouchEvent();
		CheckBlockType();
	}

	private void CheckBlockType()
	{
		int[,] levelBlock = Pallete.GetLevelBlock((Pallete.LevelBlockType)GameInfo.inGamePlayData.levelIdx);
		if (levelBlock != null && !GameInfo.isForceRandomBlockPattern && GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
		{
			ShowLevelBlockType(levelBlock);
		}
		else
		{
			AllBlockRandomType();
		}
	}

	private void ShowLevelBlockType(int[,] iArrBlockType)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				block.SetPattern((BlockType)iArrBlockType[j, i]);
			}
		}
	}

	private void RegisterTouchEvent()
	{
		PuzzlePlayManager.OnTouchBegin = (Action<Vector3, RaycastHit2D>)Delegate.Combine(PuzzlePlayManager.OnTouchBegin, new Action<Vector3, RaycastHit2D>(OnTouchBeginEvent));
		PuzzlePlayManager.OnTouchMove = (Action<Vector3>)Delegate.Combine(PuzzlePlayManager.OnTouchMove, new Action<Vector3>(OnTouchMoveEvent));
		PuzzlePlayManager.OnTouchEnd = (Action<Vector3>)Delegate.Combine(PuzzlePlayManager.OnTouchEnd, new Action<Vector3>(OnTouchEndEvent));
	}

	private void AllBlockRandomType(bool isExceptionPass = false)
	{
		Block[,] array = arrBlock;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				Block block = array[i, j];
				if ((block.ExceptionType == BlockExceptionType.None || !isExceptionPass) && (block.SpeciaData.type == SpecialBlockType.None || !isExceptionPass))
				{
					block.SetRandomPattern();
					while (CheckMatch(block))
					{
						block.SetRandomPattern();
					}
				}
			}
		}
	}

	private bool CheckClickAble(int x)
	{
		for (int i = 0; i < height; i++)
		{
			if (arrBlock[x, i].State == BlockState.MatchWait || arrBlock[x, i].State == BlockState.Move)
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckClickMatchWait(int x, int y)
	{
		UnityEngine.Debug.Log("CheckClickMatchWait :: " + arrBlock[x, y].State);
		if (arrBlock[x, y].State == BlockState.MatchWait)
		{
			return false;
		}
		return true;
	}

	private void ClickedEvent(int x, int y, Vector3 touchPosition)
	{
		if (arrBlock[x, y].State == BlockState.Move || arrBlock[x, y].State == BlockState.MatchEnd)
		{
			return;
		}
		if (!firstBlock)
		{
			prevMousePos = Camera.main.ScreenToWorldPoint(touchPosition);
			arrBlock[x, y].Select(out firstBlock);
			SoundController.EffectSound_Play(EffectSoundType.BlockClick);
			return;
		}
		if ((bool)firstBlock)
		{
			firstBlock.DeSelect(out firstBlock);
		}
		if ((bool)secondBlock)
		{
			secondBlock.DeSelect(out secondBlock);
		}
	}

	private void CompleteSelect()
	{
		CancelHintBlock();
		StartCoroutine(SwapCoroutine());
		firstBlock = null;
		secondBlock = null;
	}

	private void Swap(Block a, Block b)
	{
		arrBlock[a.X, a.Y] = b;
		arrBlock[b.X, b.Y] = a;
		BlockData data = a.Data;
		a.SetData(b.Data);
		b.SetData(data);
	}

	private void Swap(int ax, int ay, int bx, int by)
	{
		BlockData data = arrBlock[ax, ay].Data;
		arrBlock[ax, ay].SetData(arrBlock[bx, by].Data);
		arrBlock[bx, by].SetData(data);
		Block block = arrBlock[ax, ay];
		arrBlock[ax, ay] = arrBlock[bx, by];
		arrBlock[bx, by] = block;
	}

	private bool FindMatchAtBlock(Block block)
	{
		if (block.State == BlockState.MatchEnd)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = CompareColorInHorizontal(block);
		bool flag3 = CompareColorInVertical(block);
		bool flag4 = CompareColorInPattern(MatchData.Direction.LEFT, block);
		bool flag5 = CompareColorInPattern(MatchData.Direction.TOP, block);
		bool flag6 = CompareColorInPattern(MatchData.Direction.RIGHT, block);
		bool flag7 = CompareColorInPattern(MatchData.Direction.BOTTOM, block);
		flag = (flag2 || flag3 || flag4 || flag5 || flag6 || flag7);
		if (flag)
		{
			if (flag2)
			{
				if (flag4 && flag6)
				{
					matchList.Add(new MatchData(MatchData.Direction.RIGHT, 5, block.X - 2, block.Y));
				}
				else if (flag4)
				{
					matchList.Add(new MatchData(MatchData.Direction.RIGHT, 4, block.X - 2, block.Y));
				}
				else if (flag6)
				{
					matchList.Add(new MatchData(MatchData.Direction.RIGHT, 4, block.X - 1, block.Y));
				}
				else
				{
					matchList.Add(new MatchData(MatchData.Direction.RIGHT, 3, block.X - 1, block.Y));
				}
			}
			else
			{
				if (flag4)
				{
					matchList.Add(new MatchData(MatchData.Direction.LEFT, 3, block.X, block.Y));
				}
				if (flag6)
				{
					matchList.Add(new MatchData(MatchData.Direction.RIGHT, 3, block.X, block.Y));
				}
			}
			if (flag3)
			{
				if (flag5 && flag7)
				{
					matchList.Add(new MatchData(MatchData.Direction.TOP, 5, block.X, block.Y - 2));
				}
				else if (flag5)
				{
					matchList.Add(new MatchData(MatchData.Direction.TOP, 4, block.X, block.Y - 1));
				}
				else if (flag7)
				{
					matchList.Add(new MatchData(MatchData.Direction.TOP, 4, block.X, block.Y - 2));
				}
				else
				{
					matchList.Add(new MatchData(MatchData.Direction.TOP, 3, block.X, block.Y - 1));
				}
			}
			else
			{
				if (flag5)
				{
					matchList.Add(new MatchData(MatchData.Direction.TOP, 3, block.X, block.Y));
				}
				if (flag7)
				{
					matchList.Add(new MatchData(MatchData.Direction.BOTTOM, 3, block.X, block.Y));
				}
			}
		}
		return flag;
	}

	private IEnumerator SwapCoroutine()
	{
		UnityEngine.Debug.Log("swapCoroutine");
		isSwapProcess = true;
		Block a = firstBlock;
		Block b = secondBlock;
		firstBlock = null;
		secondBlock = null;
		if (!CheckClickMatchWait(a.X, a.Y) || !CheckClickMatchWait(b.X, b.Y))
		{
			a.DeSelect(out a);
			b.DeSelect(out b);
			isSwapProcess = false;
			if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(3))
			{
				ShowBoostHint();
			}
			yield break;
		}
		Vector2 firPos = new Vector2(a.Data.xPos, a.Data.yPos);
		Vector2 secPos = new Vector2(b.Data.xPos, b.Data.yPos);
		float time = Time.time;
		SoundController.EffectSound_Play(EffectSoundType.BlockMove);
		LeanTween.cancel(a.gameObject);
		LeanTween.cancel(b.gameObject);
		LeanTween.moveLocal(a.gameObject, secPos, swapTime);
		LeanTween.moveLocal(b.gameObject, firPos, swapTime);
		yield return new WaitForSeconds(swapTime);
		a.transform.localPosition = secPos;
		b.transform.localPosition = firPos;
		Swap(a, b);
		bool matchedA = FindMatchAtBlock(a);
		bool matchedB = FindMatchAtBlock(b);
		if (!matchedA && !matchedB)
		{
			if (PuzzlePlayManager.MatchTimerState || PuzzlePlayManager.MatchTimeEndState)
			{
				CancelHintBlock();
				PuzzlePlayManager.CancelTimer();
			}
			else
			{
				LeanTween.cancel(a.gameObject);
				LeanTween.cancel(b.gameObject);
				LeanTween.moveLocal(a.gameObject, firPos, swapTime);
				LeanTween.moveLocal(b.gameObject, secPos, swapTime);
				yield return new WaitForSeconds(swapTime);
				a.transform.localPosition = firPos;
				b.transform.localPosition = secPos;
				Swap(a, b);
				if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(3))
				{
					ShowBoostHint();
				}
			}
		}
		else
		{
			if (matchList.Count != 0)
			{
				MatchProcess();
				if (!PuzzlePlayManager.MatchTimerState && !PuzzlePlayManager.MatchActive)
				{
					PuzzlePlayManager.StartTimer();
				}
				else
				{
					PuzzlePlayManager.AddTime();
				}
			}
			if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(3))
			{
				ShowBoostHint();
			}
		}
		if (!matchedA)
		{
			a.DeSelect(out a);
		}
		if (!matchedB)
		{
			b.DeSelect(out b);
		}
		yield return null;
		isSwapProcess = false;
	}

	private void MatchProcess(bool isForceMatch = false)
	{
		prevMatchTime = Time.time;
		foreach (MatchData match in matchList)
		{
			int x = match.x;
			int y = match.y;
			int length = match.length;
			if (arrBlock[x, y].State != BlockState.MatchEnd)
			{
				switch (match.direction)
				{
				case MatchData.Direction.LEFT:
					for (int j = 0; j < length; j++)
					{
						if (arrBlock[x - j, y].ExceptionType != BlockExceptionType.Obstruction)
						{
							arrBlock[x - j, y].WaitMatch(isForceMatch);
						}
					}
					if (arrBlock[x, y].Type == BlockType.White)
					{
						SoundController.EffectSound_Play(EffectSoundType.HealMatching);
					}
					else
					{
						SoundController.EffectSound_Play(EffectSoundType.BlockMatching);
					}
					break;
				case MatchData.Direction.RIGHT:
					for (int l = 0; l < length; l++)
					{
						if (arrBlock[x + l, y].ExceptionType != BlockExceptionType.Obstruction)
						{
							arrBlock[x + l, y].WaitMatch(isForceMatch);
						}
					}
					if (arrBlock[x, y].Type == BlockType.White)
					{
						SoundController.EffectSound_Play(EffectSoundType.HealMatching);
					}
					else
					{
						SoundController.EffectSound_Play(EffectSoundType.BlockMatching);
					}
					break;
				case MatchData.Direction.TOP:
					for (int k = 0; k < length; k++)
					{
						if (arrBlock[x, y + k].ExceptionType != BlockExceptionType.Obstruction)
						{
							arrBlock[x, y + k].WaitMatch(isForceMatch);
						}
					}
					if (arrBlock[x, y].Type == BlockType.White)
					{
						SoundController.EffectSound_Play(EffectSoundType.HealMatching);
					}
					else
					{
						SoundController.EffectSound_Play(EffectSoundType.BlockMatching);
					}
					break;
				case MatchData.Direction.BOTTOM:
					for (int i = 0; i < length; i++)
					{
						if (arrBlock[x, y - i].ExceptionType != BlockExceptionType.Obstruction)
						{
							arrBlock[x, y - i].WaitMatch(isForceMatch);
						}
					}
					if (arrBlock[x, y].Type == BlockType.White)
					{
						SoundController.EffectSound_Play(EffectSoundType.HealMatching);
					}
					else
					{
						SoundController.EffectSound_Play(EffectSoundType.BlockMatching);
					}
					break;
				}
			}
		}
		matchList.Clear();
	}

	private void Shuffle()
	{
		CancelHintBlock();
		StartCoroutine(ProcessShuffle());
	}

	private IEnumerator ProcessShuffle()
	{
		PuzzlePlayManager.LockController();
		PuzzlePlayManager.ShowShuffleUI();
		Vector3 shufflePosition = arrBlock[3, 2].transform.position;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				if (block.ExceptionType == BlockExceptionType.None && block.SpeciaData.type == SpecialBlockType.None)
				{
					LeanTween.move(block.gameObject, shufflePosition, 0.3f).setEaseOutCubic();
				}
			}
		}
		yield return new WaitForSeconds(0.3f);
		AllBlockRandomType(isExceptionPass: true);
		for (int k = 0; k < width; k++)
		{
			for (int l = 0; l < height; l++)
			{
				Block block2 = arrBlock[k, l];
				LeanTween.moveLocalX(block2.gameObject, block2.Data.xPos, 0.3f).setEaseOutCubic();
				LeanTween.moveLocalY(block2.gameObject, block2.Data.yPos, 0.3f).setEaseOutCubic();
			}
		}
		yield return new WaitForSeconds(0.3f);
		PuzzlePlayManager.ControllerStart();
		PuzzlePlayManager.HideShuffleUI();
	}

	private void ShowHintBlock()
	{
		HideHintBlock();
		if (GameInfo.inGamePlayData.levelIdx != 0)
		{
			if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(3))
			{
				ShowBoostHint();
			}
			else
			{
				ShowDefaultHint();
			}
		}
	}

	private void ShowDefaultHint()
	{
		CheckHint();
		if (hintBlock == null)
		{
			Shuffle();
		}
		else
		{
			coroutineHintBlock = StartCoroutine(ProcessShowHint());
		}
	}

	private void ShowBoostHint()
	{
		if (!isTouchControl)
		{
			return;
		}
		CheckHint();
		if (hintBlock == null)
		{
			if (!PuzzlePlayManager.MatchTimerState && !PuzzlePlayManager.MatchActive)
			{
				Shuffle();
			}
		}
		else
		{
			hintBlock.ShowHintBoost();
		}
	}

	private void HideHintBlock()
	{
		if (coroutineHintBlock != null)
		{
			StopCoroutine(coroutineHintBlock);
			coroutineHintBlock = null;
		}
	}

	private void CancelHintBlock()
	{
		HideHintBlock();
		if ((bool)hintBlock)
		{
			hintBlock.CancelHint();
			hintBlock = null;
		}
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				arrBlock[i, j].CancelHint();
			}
		}
	}

	private IEnumerator ProcessShowHint()
	{
		UnityEngine.Debug.Log("ProcessShowHint :: " + hintBlock);
		while ((bool)hintBlock)
		{
			yield return new WaitForSeconds(3f);
			hintBlock.ShowHint();
			if ((bool)hintSwitchBlock)
			{
				hintSwitchBlock.ShowHint();
			}
			yield return new WaitForSeconds(2f);
			hintBlock.HideHint();
			if ((bool)hintSwitchBlock)
			{
				hintSwitchBlock.HideHint();
			}
		}
	}

	private void CheckHint()
	{
		CancelHintBlock();
		BlockHintType blockHintType = BlockHintType.None;
		listHintCheckBlock.Clear();
		listHintCheckBlock.Add(arrBlock[3, 2]);
		listHintCheckBlock.AddRange(GetCheckHintBlockRange(1));
		listHintCheckBlock.AddRange(GetCheckHintBlockRange(2));
		listHintCheckBlock.AddRange(GetCheckHintBlockRange(3));
		foreach (Block item in listHintCheckBlock)
		{
			if (item.ExceptionType == BlockExceptionType.None && item.State != BlockState.MatchWait)
			{
				blockHintType = CheckDragMatchAtBlock(item);
				if (blockHintType != BlockHintType.None)
				{
					hintBlock = item;
					hintBlock.SetHintType(blockHintType);
					return;
				}
			}
		}
		foreach (Block item2 in listHintCheckBlock)
		{
			if (item2.ExceptionType == BlockExceptionType.None && item2.State != BlockState.MatchWait)
			{
				blockHintType = CheckDragDiagonalMatchAtBlock(item2);
				if (blockHintType != BlockHintType.None)
				{
					hintBlock = item2;
					hintBlock.SetHintType(blockHintType);
					break;
				}
			}
		}
	}

	private List<Block> GetCheckHintBlockRange(int _checkAdd)
	{
		List<Block> list = new List<Block>();
		int num = 3 - _checkAdd;
		int num2 = 3 + _checkAdd;
		int num3 = 2 - _checkAdd;
		int num4 = 2 + _checkAdd;
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				if (i >= 0 && i <= width - 1 && j >= 0 && j <= height - 1 && !listHintCheckBlock.Contains(arrBlock[i, j]))
				{
					list.Add(arrBlock[i, j]);
				}
			}
		}
		return list;
	}

	private void GetCheckHintBlock(int _x, int _y, List<Block> _list)
	{
		if (_x >= 0 && _x <= width - 1 && _y >= 0 && _y <= height - 1)
		{
			_list.Add(arrBlock[_x, _y]);
		}
	}

	private BlockHintType CheckDragMatchAtBlock(Block block)
	{
		int x = block.Data.x;
		int y = block.Data.y;
		if (x > 0 && arrBlock[x - 1, y].ExceptionType == BlockExceptionType.None && arrBlock[x - 1, y].State != BlockState.MatchWait)
		{
			if (CompareColor(block, -1, 1, -1, 2))
			{
				return BlockHintType.Left;
			}
			if (CompareColor(block, -1, -1, -1, -2))
			{
				return BlockHintType.Left;
			}
			if (CompareColor(block, -2, 0, -3, 0))
			{
				return BlockHintType.Left;
			}
			if (CompareColor(block, -1, 1, -1, -1))
			{
				return BlockHintType.Left;
			}
		}
		if (x < width - 1 && arrBlock[x + 1, y].ExceptionType == BlockExceptionType.None && arrBlock[x + 1, y].State != BlockState.MatchWait)
		{
			if (CompareColor(block, 1, 1, 1, 2))
			{
				return BlockHintType.Right;
			}
			if (CompareColor(block, 1, -1, 1, -2))
			{
				return BlockHintType.Right;
			}
			if (CompareColor(block, 2, 0, 3, 0))
			{
				return BlockHintType.Right;
			}
			if (CompareColor(block, 2, 0, 3, 0))
			{
				return BlockHintType.Right;
			}
			if (CompareColor(block, 1, 1, 1, -1))
			{
				return BlockHintType.Right;
			}
		}
		if (y < height - 1 && arrBlock[x, y + 1].ExceptionType == BlockExceptionType.None && arrBlock[x, y + 1].State != BlockState.MatchWait)
		{
			if (CompareColor(block, 1, 1, 2, 1))
			{
				return BlockHintType.Top;
			}
			if (CompareColor(block, -1, 1, -2, 1))
			{
				return BlockHintType.Top;
			}
			if (CompareColor(block, 0, 2, 0, 3))
			{
				return BlockHintType.Top;
			}
			if (CompareColor(block, -1, 1, 1, 1))
			{
				return BlockHintType.Top;
			}
		}
		if (y > 0 && arrBlock[x, y - 1].ExceptionType == BlockExceptionType.None && arrBlock[x, y - 1].State != BlockState.MatchWait)
		{
			if (CompareColor(block, 1, -1, 2, -1))
			{
				return BlockHintType.Bottom;
			}
			if (CompareColor(block, -1, -1, -2, -1))
			{
				return BlockHintType.Bottom;
			}
			if (CompareColor(block, 0, -2, 0, -3))
			{
				return BlockHintType.Bottom;
			}
			if (CompareColor(block, -1, -1, 1, -1))
			{
				return BlockHintType.Bottom;
			}
		}
		return BlockHintType.None;
	}

	private BlockHintType CheckDragDiagonalMatchAtBlock(Block block)
	{
		int x = block.Data.x;
		int y = block.Data.y;
		if (x > 0 && y < height - 1 && arrBlock[x - 1, y + 1].ExceptionType == BlockExceptionType.None && arrBlock[x - 1, y + 1].State != BlockState.MatchWait)
		{
			if (CompareColor(block, -1, 2, -1, 3))
			{
				return BlockHintType.Top_Left;
			}
			if (CompareColor(block, 0, 1, 1, 1))
			{
				return BlockHintType.Top_Left;
			}
			if (CompareColor(block, -1, 0, -1, -1))
			{
				return BlockHintType.Top_Left;
			}
			if (CompareColor(block, -2, 1, -3, 1))
			{
				return BlockHintType.Top_Left;
			}
			if (CompareColor(block, -2, 1, 0, 1))
			{
				return BlockHintType.Top_Left;
			}
			if (CompareColor(block, -1, 0, -1, 2))
			{
				return BlockHintType.Top_Left;
			}
		}
		if (x < width - 1 && y < height - 1 && arrBlock[x + 1, y + 1].ExceptionType == BlockExceptionType.None && arrBlock[x + 1, y + 1].State != BlockState.MatchWait)
		{
			if (CompareColor(block, 0, 1, -1, 1))
			{
				return BlockHintType.Top_Right;
			}
			if (CompareColor(block, 1, 2, 1, 3))
			{
				return BlockHintType.Top_Right;
			}
			if (CompareColor(block, 1, 0, 1, -1))
			{
				return BlockHintType.Top_Right;
			}
			if (CompareColor(block, 2, 1, 3, 1))
			{
				return BlockHintType.Top_Right;
			}
			if (CompareColor(block, 0, 1, 2, 1))
			{
				return BlockHintType.Top_Right;
			}
			if (CompareColor(block, 1, 0, 1, 2))
			{
				return BlockHintType.Top_Right;
			}
		}
		if (x < width - 1 && y > 0 && arrBlock[x + 1, y - 1].ExceptionType == BlockExceptionType.None && arrBlock[x + 1, y - 1].State != BlockState.MatchWait)
		{
			if (CompareColor(block, 1, 0, 1, 1))
			{
				return BlockHintType.Bottom_Right;
			}
			if (CompareColor(block, 2, -1, 3, -1))
			{
				return BlockHintType.Bottom_Right;
			}
			if (CompareColor(block, 1, -2, 1, -3))
			{
				return BlockHintType.Bottom_Right;
			}
			if (CompareColor(block, 0, -1, -1, -1))
			{
				return BlockHintType.Bottom_Right;
			}
			if (CompareColor(block, 0, -1, 2, -1))
			{
				return BlockHintType.Bottom_Right;
			}
			if (CompareColor(block, 1, 0, 1, -2))
			{
				return BlockHintType.Bottom_Right;
			}
		}
		if (x > 0 && y > 0 && arrBlock[x - 1, y - 1].ExceptionType == BlockExceptionType.None && arrBlock[x - 1, y - 1].State != BlockState.MatchWait)
		{
			if (CompareColor(block, -1, 0, -1, 1))
			{
				return BlockHintType.Bottom_Left;
			}
			if (CompareColor(block, -2, -1, -3, -1))
			{
				return BlockHintType.Bottom_Left;
			}
			if (CompareColor(block, -1, -2, -1, -3))
			{
				return BlockHintType.Bottom_Left;
			}
			if (CompareColor(block, 0, -1, 1, -1))
			{
				return BlockHintType.Bottom_Left;
			}
			if (CompareColor(block, 0, -1, -2, -1))
			{
				return BlockHintType.Bottom_Left;
			}
			if (CompareColor(block, -1, 0, -1, -2))
			{
				return BlockHintType.Bottom_Left;
			}
		}
		return BlockHintType.None;
	}

	private BlockHintType CheckSwitchMatchAtBlock(Block _targetBlock)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				if (_targetBlock != block && CompareColorForSwitch(block, _targetBlock))
				{
					hintSwitchBlock = block;
					hintSwitchBlock.SetHintType(BlockHintType.Switch);
					return BlockHintType.Switch;
				}
			}
		}
		return BlockHintType.None;
	}

	private bool CompareColorInHorizontal(Block block)
	{
		int x = block.X;
		int y = block.Y;
		BlockType type = block.Type;
		return CompareColor(type, x - 1, y) && CompareColor(type, x + 1, y);
	}

	private bool CompareColorInVertical(Block block)
	{
		int x = block.X;
		int y = block.Y;
		BlockType type = block.Type;
		return CompareColor(type, x, y - 1) && CompareColor(type, x, y + 1);
	}

	private bool CompareColorInVertical(int x, int y, BlockType type)
	{
		return CompareColor(type, x, y - 1) && CompareColor(type, x, y + 1);
	}

	private bool CompareColorInPattern(MatchData.Direction dir, Block block)
	{
		int x = block.X;
		int y = block.Y;
		BlockType type = block.Type;
		switch (dir)
		{
		case MatchData.Direction.LEFT:
			return CompareColor(type, x - 2, y) && CompareColor(type, x - 1, y);
		case MatchData.Direction.RIGHT:
			return CompareColor(type, x + 2, y) && CompareColor(type, x + 1, y);
		case MatchData.Direction.TOP:
			return CompareColor(type, x, y + 2) && CompareColor(type, x, y + 1);
		case MatchData.Direction.BOTTOM:
			return CompareColor(type, x, y - 2) && CompareColor(type, x, y - 1);
		default:
			return false;
		}
	}

	private bool CompareColorInPattern(MatchData.Direction dir, int x, int y, BlockType type)
	{
		switch (dir)
		{
		case MatchData.Direction.LEFT:
			return CompareColor(type, x - 2, y) && CompareColor(type, x - 1, y);
		case MatchData.Direction.RIGHT:
			return CompareColor(type, x + 2, y) && CompareColor(type, x + 1, y);
		case MatchData.Direction.TOP:
			return CompareColor(type, x, y + 2) && CompareColor(type, x, y + 1);
		case MatchData.Direction.BOTTOM:
			return CompareColor(type, x, y - 2) && CompareColor(type, x, y - 1);
		default:
			return false;
		}
	}

	private bool CompareColor(BlockType type, int x, int y)
	{
		if (x < 0 || y < 0 || x >= width || y >= height)
		{
			return false;
		}
		return type == arrBlock[x, y].Type;
	}

	private bool CompareColor(Block block, int fx, int fy, int sx, int sy)
	{
		if (block.ExceptionType != 0)
		{
			return false;
		}
		BlockType type = block.Type;
		int x = block.Data.x;
		int y = block.Data.y;
		int num = x + fx;
		int num2 = y + fy;
		int num3 = x + sx;
		int num4 = y + sy;
		if (num < 0 || num > width - 1 || num3 < 0 || num3 > width - 1 || num2 < 0 || num2 > height - 1 || num4 < 0 || num4 > height - 1)
		{
			return false;
		}
		if (arrBlock[num, num2].ExceptionType == BlockExceptionType.Obstruction || arrBlock[num3, num4].ExceptionType == BlockExceptionType.Obstruction)
		{
			return false;
		}
		return type == arrBlock[num, num2].Type && type == arrBlock[num3, num4].Type;
	}

	private bool CompareColor(Block checkBlock, Block targetBlock, int fx, int fy, int sx, int sy)
	{
		if (checkBlock.ExceptionType != 0)
		{
			return false;
		}
		int x = checkBlock.Data.x;
		int y = checkBlock.Data.y;
		int num = x + fx;
		int num2 = y + fy;
		int num3 = x + sx;
		int num4 = y + sy;
		if (num < 0 || num > width - 1 || num3 < 0 || num3 > width - 1 || num2 < 0 || num2 > height - 1 || num4 < 0 || num4 > height - 1)
		{
			return false;
		}
		if (arrBlock[num, num2] == targetBlock || arrBlock[num3, num4] == targetBlock)
		{
			return false;
		}
		if (arrBlock[num, num2].ExceptionType == BlockExceptionType.Obstruction || arrBlock[num3, num4].ExceptionType == BlockExceptionType.Obstruction)
		{
			return false;
		}
		return targetBlock.Type == arrBlock[num, num2].Type && targetBlock.Type == arrBlock[num3, num4].Type;
	}

	private bool CompareColorForSwitch(Block checkBlock, Block targetBlock)
	{
		if (CompareColor(checkBlock, targetBlock, 0, -1, 0, -2))
		{
			return true;
		}
		if (CompareColor(checkBlock, targetBlock, 0, 1, 0, 2))
		{
			return true;
		}
		if (CompareColor(checkBlock, targetBlock, -1, 0, -2, 0))
		{
			return true;
		}
		if (CompareColor(checkBlock, targetBlock, 1, 0, 2, 0))
		{
			return true;
		}
		if (CompareColor(checkBlock, targetBlock, 0, -1, 0, 1))
		{
			return true;
		}
		if (CompareColor(checkBlock, targetBlock, -1, 0, 1, 0))
		{
			return true;
		}
		return false;
	}

	private void StartMatch()
	{
		UnityEngine.Debug.Log("## StartMatch");
		StartCoroutine(StartCheckMatch());
		StartMatchDownProcess();
	}

	private void SortMatchWaitBlock()
	{
		listMatchReadyBlock.Clear();
		for (int i = 0; i < width; i++)
		{
			for (int num = height - 1; num >= 0; num--)
			{
				Block block = arrBlock[i, num];
				if (block.State == BlockState.MatchWait)
				{
					List<Block> list = new List<Block>();
					CheckAddMatchReadyBlock(i, num, list);
					CreateSpecialBlock(list);
					listMatchReadyBlock.Add(list);
				}
			}
		}
	}

	private void CheckAddMatchReadyBlock(int x, int y, List<Block> listBlock)
	{
		Block block = arrBlock[x, y];
		block.ReadyMatch();
		listBlock.Add(block);
		if (x > 0 && block.Type == arrBlock[x - 1, y].Type && arrBlock[x - 1, y].State == BlockState.MatchWait)
		{
			CheckAddMatchReadyBlock(x - 1, y, listBlock);
		}
		if (x < width - 1 && block.Type == arrBlock[x + 1, y].Type && arrBlock[x + 1, y].State == BlockState.MatchWait)
		{
			CheckAddMatchReadyBlock(x + 1, y, listBlock);
		}
		if (y > 0 && block.Type == arrBlock[x, y - 1].Type && arrBlock[x, y - 1].State == BlockState.MatchWait)
		{
			CheckAddMatchReadyBlock(x, y - 1, listBlock);
		}
		if (y < height - 1 && block.Type == arrBlock[x, y + 1].Type && arrBlock[x, y + 1].State == BlockState.MatchWait)
		{
			CheckAddMatchReadyBlock(x, y + 1, listBlock);
		}
	}

	private void CreateSpecialBlock(List<Block> listBlock)
	{
		if (listBlock.Count >= 6 && listBlock.Count >= 1)
		{
			foreach (Block item in listBlock)
			{
				if (item.Type == BlockType.White || item.SpeciaData.state == SpecialBlockState.Bomb || !item.Active)
				{
					return;
				}
			}
			Block block = CheckCurrentSpecialBlock(listBlock);
			if (!(block == null))
			{
				block.SetSpecial(listBlock.Count);
				foreach (Block item2 in listBlock)
				{
					item2.SetSpecialPosition(block.transform.position);
				}
			}
		}
	}

	private Block CheckCurrentSpecialBlock(List<Block> listBlock)
	{
		bool flag = false;
		using (List<Block>.Enumerator enumerator = listBlock.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Block current = enumerator.Current;
				if (current.SpeciaData.type == SpecialBlockType.None)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return null;
		}
		flag = true;
		Block block = null;
		while (flag)
		{
			block = listBlock[UnityEngine.Random.Range(0, listBlock.Count - 1)];
			if (block.SpeciaData.state == SpecialBlockState.None)
			{
				flag = false;
				return block;
			}
		}
		return block;
	}

	private IEnumerator StartCheckMatch()
	{
		while (isSwapProcess || CheckBlockMove())
		{
			yield return null;
		}
		if (!isMatchEndProcess)
		{
			isMatchEndProcess = true;
			yield return null;
			SortMatchWaitBlock();
			foreach (List<Block> listBlock in listMatchReadyBlock)
			{
				StartCoroutine(ShowBlockToHeroEffect(listBlock));
				yield return new WaitForSeconds(0.3f);
			}
			yield return null;
			listMatchReadyBlock.Clear();
			isMatchEndProcess = false;
			DropBlock();
			StartCheckMatchEnd();
		}
	}

	private bool CheckBlockMove()
	{
		for (int i = 0; i < width; i++)
		{
			for (int num = height - 1; num >= 0; num--)
			{
				Block block = arrBlock[i, num];
				if (block.State == BlockState.Move)
				{
					UnityEngine.Debug.Log("CheckBlockMove :: " + block.gameObject.name);
					return true;
				}
			}
		}
		return false;
	}

	private IEnumerator ShowBlockToHeroEffect(List<Block> listBlock)
	{
		foreach (Block item in listBlock)
		{
			item.StartMatch();
			CheckObstructionBlock(item.Data.x, item.Data.y);
			if (item.Type == BlockType.White)
			{
				SoundController.EffectSound_Play(EffectSoundType.HealDestroy);
			}
			else if (PuzzlePlayManager.HunterPos(item.Type) != null)
			{
				Vector3[] hunterPosition = PuzzlePlayManager.HunterPos(item.Type);
				for (int i = 0; i < hunterPosition.Length; i++)
				{
					Vector2 vector = hunterPosition[i];
					Transform transform = MasterPoolManager.SpawnObject("Effect", Pallete.ConvertBlockEffect(item.Type), trController, 0.7f);
					transform.position = item.transform.position;
					LeanTween.moveX(transform.gameObject, vector.x, 0.2f).setEaseInCirc();
					LeanTween.moveY(transform.gameObject, vector.y, 0.2f);
				}
				if (item.HasSpecialPosition)
				{
					Transform transform2 = MasterPoolManager.SpawnObject("Effect", Pallete.ConvertBlockEffect(item.Type), trController, 0.2f);
					transform2.position = item.transform.position;
					LeanTween.move(transform2.gameObject, item.SpecialBlockPositon, 0.2f).setEaseOutCubic();
					item.SetSpecialPosition(Vector3.zero);
				}
			}
		}
		if (listBlock.Count > 0)
		{
			PuzzlePlayManager.BlockAttackEffect();
			SoundController.EffectSound_Play(EffectSoundType.BlockDestroy);
		}
		yield return new WaitForSeconds(0.2f);
		if (listBlock.Count > 0)
		{
			PuzzlePlayManager.AddBlock(listBlock[0].Type, listBlock.Count);
		}
	}

	private void CheckObstructionBlock(int x, int y)
	{
		Block block = arrBlock[x, y];
		if (x > 0 && arrBlock[x - 1, y].ExceptionType == BlockExceptionType.Obstruction)
		{
			arrBlock[x - 1, y].CancelObstruction(block.Type);
		}
		if (x < width - 1 && arrBlock[x + 1, y].ExceptionType == BlockExceptionType.Obstruction)
		{
			arrBlock[x + 1, y].CancelObstruction(block.Type);
		}
		if (y > 0 && arrBlock[x, y - 1].ExceptionType == BlockExceptionType.Obstruction)
		{
			arrBlock[x, y - 1].CancelObstruction(block.Type);
		}
		if (y < height - 1 && arrBlock[x, y + 1].ExceptionType == BlockExceptionType.Obstruction)
		{
			arrBlock[x, y + 1].CancelObstruction(block.Type);
		}
	}

	private IEnumerator CheckContinueProcessMatchReady()
	{
		if (isDropBlockMathReadyProcess)
		{
			yield break;
		}
		isDropBlockMathReadyProcess = true;
		while (CheckBlockMove())
		{
			yield return null;
		}
		bool isContinueMathReady = false;
		while (!isContinueMathReady)
		{
			int checkNunm = 0;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Block block = arrBlock[x, y];
					if (block.State == BlockState.Move)
					{
						checkNunm++;
						yield return null;
					}
				}
			}
			if (checkNunm == 0)
			{
				isContinueMathReady = true;
			}
		}
		isDropBlockMathReadyProcess = false;
		StartMatch();
	}

	private void StartCheckMatchEnd()
	{
		if (coroutineBlockMatchEnd != null)
		{
			StopCoroutine(coroutineBlockMatchEnd);
			coroutineBlockMatchEnd = null;
		}
		coroutineBlockMatchEnd = StartCoroutine(CheckMatchEnd());
	}

	private IEnumerator CheckMatchEnd()
	{
		isTouchControl = false;
		bool checkMatch = false;
		while (!checkMatch)
		{
			int checkNunm = 0;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Block block = arrBlock[x, y];
					if (block.State != BlockState.Idle)
					{
						checkNunm++;
						yield return null;
					}
				}
			}
			if (checkNunm == 0)
			{
				checkMatch = true;
			}
		}
		if (!CheckSpecialBlock())
		{
			StopMatchDownProcess();
			PuzzlePlayManager.CompleteBlock();
		}
	}

	private bool CheckSpecialBlock()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				if (block.Type == BlockType.White || block.SpeciaData.state != SpecialBlockState.Ready)
				{
					continue;
				}
				block.StartSpecialMatch(block.Type, isSelf: true);
				int num = Mathf.Clamp((int)(i - block.SpeciaData.type), 0, 6);
				int num2 = Mathf.Clamp((int)(i + block.SpeciaData.type), 0, 6);
				UnityEngine.Debug.Log("minX :: " + num + ", maxX :: " + num2 + ", type ::: " + (int)block.SpeciaData.type);
				for (int k = num; k <= num2; k++)
				{
					if (arrBlock[k, j].State != BlockState.MatchReady)
					{
						arrBlock[k, j].StartSpecialMatch(block.Type, isSelf: false);
					}
				}
				int num3 = Mathf.Clamp((int)(j - block.SpeciaData.type), 0, 4);
				int num4 = Mathf.Clamp((int)(j + block.SpeciaData.type), 0, 4);
				UnityEngine.Debug.Log("minY :: " + num3 + ", maxY :: " + num4 + ", type ::: " + (int)block.SpeciaData.type + ", y:: " + j);
				for (int l = num3; l <= num4; l++)
				{
					if (arrBlock[i, l].State != BlockState.MatchReady)
					{
						arrBlock[i, l].StartSpecialMatch(block.Type, isSelf: false);
					}
				}
				Invoke("StartMatch", 0.4f);
				return true;
			}
		}
		return false;
	}

	private void StartMatchDownProcess()
	{
		StopMatchDownProcess();
		coroutineMathDown = StartCoroutine(matchDownCoroutine());
	}

	private void StopMatchDownProcess()
	{
		if (coroutineMathDown != null)
		{
			StopCoroutine(coroutineMathDown);
			coroutineMathDown = null;
		}
	}

	private IEnumerator matchDownCoroutine()
	{
		while (isSwapProcess || isDropBlockMathReadyProcess || !isMatchEndProcess)
		{
			yield return null;
		}
		List<Block> listCheckNextMatchDown = new List<Block>();
		for (int i = 0; i < width; i++)
		{
			List<Block> list = new List<Block>();
			for (int j = 0; j < height; j++)
			{
				Block block = arrBlock[i, j];
				if (block.State == BlockState.MatchEnd)
				{
					if (!list.Contains(block))
					{
						list.Add(block);
						block.transform.localPosition = new Vector2(i, height);
					}
				}
				else if (list.Count != 0)
				{
					Swap(i, j, i, j - list.Count);
					Vector2 vector = arrBlockPosition[i, j - list.Count];
					block.SetData(new BlockData(i, j - list.Count, vector.x, vector.y));
					listDropBlock.Add(block);
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				Block block2 = arrBlock[i, height - k - 1];
				Vector2 vector2 = arrBlockPosition[i, height - k - 1];
				block2.transform.localPosition = new Vector2(i, height + list.Count - k);
				block2.On();
				block2.SetData(new BlockData(i, height - k - 1, vector2.x, vector2.y));
				block2.SetRandomPattern();
				listDropBlock.Add(block2);
			}
			listCheckNextMatchDown.AddRange(list);
		}
		yield return null;
		StartMatchDownProcess();
	}

	private void DropBlock()
	{
		foreach (Block item in listDropBlock)
		{
			item.Move(item.Data.xPos, item.Data.yPos);
		}
		listDropBlock.Clear();
	}

	private bool CheckMatch(Block block)
	{
		BlockType type = block.Type;
		int x = block.X;
		int y = block.Y;
		if (x > 1 && type == arrBlock[x - 1, y].Type && type == arrBlock[x - 2, y].Type)
		{
			return true;
		}
		if (x < width - 2 && type == arrBlock[x + 1, y].Type && type == arrBlock[x + 2, y].Type)
		{
			return true;
		}
		if (y < height - 2 && type == arrBlock[x, y + 1].Type && type == arrBlock[x, y + 2].Type)
		{
			return true;
		}
		if (y > 1 && type == arrBlock[x, y - 1].Type && type == arrBlock[x, y - 2].Type)
		{
			return true;
		}
		return false;
	}

	private void CheckRaySecondBlock(Vector3 touchPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(touchPosition);
		RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(ray, float.PositiveInfinity);
		if (rayIntersection.collider != null)
		{
			Block component = rayIntersection.transform.GetComponent<Block>();
			if (component.State != BlockState.MatchWait)
			{
				firstBlock.gameObject.SetActive(value: true);
				component.Select(out secondBlock);
			}
		}
	}

	private void OnBlockMoveComplete(Block block)
	{
		if (FindMatchAtBlock(block))
		{
			UnityEngine.Debug.Log("OnBlockMoveComplete :: " + isMatchEndProcess);
			MatchProcess();
			if (!isMatchEndProcess && PuzzlePlayManager.MatchActive)
			{
				StartCoroutine(CheckContinueProcessMatchReady());
			}
			if (!PuzzlePlayManager.MatchActive)
			{
				PuzzlePlayManager.StartTimer();
			}
		}
	}

	private float GetCheckMoveDistance(float moveDistance, float defaultPos)
	{
		if (Mathf.Abs(moveDistance) >= touchMoveLimitDistance)
		{
			moveDistance = ((!(moveDistance < 0f)) ? 1.02f : (-1.02f));
		}
		if (Mathf.Abs(moveDistance) >= 0.25f)
		{
			moveDistance = ((!(moveDistance < 0f)) ? (moveDistance + 0.5f) : (moveDistance - 0.5f));
		}
		return moveDistance;
	}

	private void OnBlockSelectEvent(int _x, int _y)
	{
		if (BlockSelect != null)
		{
			BlockSelect(_x, _y);
		}
	}

	private void OnTouchBeginEvent(Vector3 touchPosition, RaycastHit2D hit)
	{
		UnityEngine.Debug.Log("OnTouchBeginEvent :: " + isTouchControl);
		if (!isTouchControl)
		{
			return;
		}
		if (hit.collider != null && hit.collider.tag == "Block")
		{
			UnityEngine.Debug.Log("OnTouchBeginEvent :: " + hit.collider.name);
			Block component = hit.transform.GetComponent<Block>();
			if (component.State != BlockState.MatchWait)
			{
				ClickedEvent(component.X, component.Y, touchPosition);
				return;
			}
		}
		if (firstBlock != null)
		{
			firstBlock.gameObject.SetActive(value: true);
			firstBlock.DeSelect(out firstBlock);
		}
	}

	private void OnTouchMoveEvent(Vector3 touchPosition)
	{
		if (!isTouchControl || isDragLock || !firstBlock || (bool)secondBlock)
		{
			return;
		}
		Vector2 vector = Camera.main.ScreenToWorldPoint(touchPosition);
		if (Mathf.Abs(prevMousePos.x - vector.x) < 0.1f && Mathf.Abs(prevMousePos.y - vector.y) < 0.1f)
		{
			return;
		}
		isControlUp = true;
		if (Mathf.Abs(prevMousePos.x - vector.x) > touchMoveLimitDistance || Mathf.Abs(prevMousePos.y - vector.y) > touchMoveLimitDistance)
		{
			Vector2 v = default(Vector2);
			Vector3 position = firstBlock.transform.position;
			v.x = position.x - GetCheckMoveDistance(Mathf.Clamp(prevMousePos.x - vector.x, -1f, 1f), firstBlock.Data.xPos);
			Vector3 position2 = firstBlock.transform.position;
			v.y = position2.y - GetCheckMoveDistance(Mathf.Clamp(prevMousePos.y - vector.y, -1f, 1f), firstBlock.Data.yPos);
			firstBlock.gameObject.SetActive(value: false);
			CheckRaySecondBlock(Camera.main.WorldToScreenPoint(v));
			firstBlock.gameObject.SetActive(value: true);
			if ((bool)firstBlock && (bool)secondBlock)
			{
				CompleteSelect();
			}
		}
	}

	private void OnTouchEndEvent(Vector3 touchPosition)
	{
		if (!isTouchControl)
		{
			return;
		}
		bool arg = false;
		Vector2 vector = Camera.main.ScreenToWorldPoint(touchPosition);
		if (Mathf.Abs(prevMousePos.x - vector.x) < touchMoveLimitDistance)
		{
			isControlUp = false;
		}
		if (!firstBlock && !secondBlock)
		{
			return;
		}
		if ((bool)firstBlock && !secondBlock)
		{
			if (Mathf.Abs(prevMousePos.x - vector.x) > touchMoveLimitDistance || Mathf.Abs(prevMousePos.y - vector.y) > touchMoveLimitDistance)
			{
				Vector2 v = default(Vector2);
				Vector3 position = firstBlock.transform.position;
				v.x = position.x - GetCheckMoveDistance(Mathf.Clamp(prevMousePos.x - vector.x, -1f, 1f), firstBlock.Data.xPos);
				Vector3 position2 = firstBlock.transform.position;
				v.y = position2.y - GetCheckMoveDistance(Mathf.Clamp(prevMousePos.y - vector.y, -1f, 1f), firstBlock.Data.yPos);
				firstBlock.gameObject.SetActive(value: false);
				CheckRaySecondBlock(Camera.main.WorldToScreenPoint(v));
				firstBlock.gameObject.SetActive(value: true);
			}
			if (secondBlock == null && (bool)firstBlock)
			{
				firstBlock.gameObject.SetActive(value: true);
				firstBlock.DeSelect(out firstBlock);
			}
			else
			{
				arg = true;
				CompleteSelect();
			}
		}
		else
		{
			firstBlock.gameObject.SetActive(value: true);
			firstBlock.DeSelect(out firstBlock);
		}
		if (PuzzleTouchEnd != null)
		{
			PuzzleTouchEnd(firstBlock, secondBlock, arg);
		}
		isControlUp = false;
	}

	private IEnumerator ProcessChangeSpecialBlock(Block _block, int _count)
	{
		isTouchControl = false;
		Transform trChangeEffect = MasterPoolManager.SpawnObject("Effect", "FX_Special_block", null, 0.3f, isSpeedProcess: false);
		trChangeEffect.position = _block.transform.position;
		SoundController.HunterSkillSound(25024);
		yield return new WaitForSeconds(0.3f);
		_block.SetSpecial(_count);
		isTouchControl = true;
	}

	private void Awake()
	{
		trController = base.gameObject.transform;
	}
}
