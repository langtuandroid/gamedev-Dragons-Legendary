using System;
using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
	public Action<Block> BlockMoveComplete;

	public Action<int, int> BlockSelect;

	private const string BLOCK_SELECT_PREFAB_NAME = "Block_select_";

	private const string BLOCK_MATCH_PREFAB_NAME = "Block_chain_";

	private const string ANIM_KEY_NAME_DEFAULT_FIX_IN = "obstruction_chain_In";

	private const string ANIM_KEY_NAME_DEFAULT_FIX_OUT = "obstruction_chain_Out";

	private const string ANIM_KEY_NAME_OBSTRUCTION_IN = "obstruction_stone_In";

	private const string ANIM_KEY_NAME_OBSTRUCTION_OUT = "obstruction_stone_Out";

	private const float BLOCK_MATCH_COMPLETE_DURATION = 0.3f;

	[SerializeField]
	private Transform trSelectAnchor;

	[SerializeField]
	private Transform trMatchAnchor;

	[SerializeField]
	private Transform trDragHint;

	[SerializeField]
	private Transform trDragHintBoost;

	[SerializeField]
	private Animator aniObstruction;

	[SerializeField]
	private Animator aniDefaultFix;

	[SerializeField]
	private bool isSelect = true;

	[SerializeField]
	private bool isDeselect = true;

	private Vector3 specialBlockPosition = Vector3.zero;

	private Vector3 specialBlockPingPongScale = new Vector3(1.2f, 1.2f, 1.2f);

	private Transform trBlock;

	private Transform trSelect;

	private Transform trMatch;

	private Transform trSpecialStanby;

	private BlockHintType hintType;

	[SerializeField]
	private BlockState blockState;

	[SerializeField]
	private BlockType blockType;

	[SerializeField]
	private BlockPattern pattern;

	private SpriteRenderer srBlock;

	[SerializeField]
	private BlockData data;

	[SerializeField]
	private SpecialBlockData specialBlockData = new SpecialBlockData();

	private Coroutine scaleBlockCoroutine;

	[SerializeField]
	private BlockExceptionType exceptionType;

	public BlockType Type => blockType;

	public BlockState State => blockState;

	public BlockData Data => data;

	public SpecialBlockData SpeciaData => specialBlockData;

	public BlockExceptionType ExceptionType => exceptionType;

	public bool Active => pattern.isActive;

	public int X => data.x;

	public int Y => data.y;

	public float PositionX => data.xPos;

	public float PositionY => data.yPos;

	public bool HasSpecialPosition => specialBlockPosition != Vector3.zero;

	public Vector3 SpecialBlockPositon => specialBlockPosition;

	public bool HasMatch => exceptionType != BlockExceptionType.Obstruction && exceptionType != BlockExceptionType.SpecialFix;

	public void Init()
	{
		blockState = BlockState.Empty;
		specialBlockData.type = SpecialBlockType.None;
		specialBlockData.state = SpecialBlockState.None;
		trSelectAnchor.gameObject.SetActive(value: false);
		trMatchAnchor.gameObject.SetActive(value: false);
		aniObstruction.gameObject.SetActive(value: false);
	}

	public void SetData()
	{
		BlockData blockData = data;
		Vector3 localPosition = trBlock.localPosition;
		blockData.x = (int)localPosition.x;
		BlockData blockData2 = data;
		Vector3 localPosition2 = trBlock.localPosition;
		blockData2.y = (int)localPosition2.y;
	}

	public void SetData(BlockData _data)
	{
		data = _data;
	}

	public void RefreshPattern()
	{
		pattern = Pallete.GetPattern(blockType);
		if (Pallete.GetPattern(blockType).isActive && specialBlockData.state != 0)
		{
			srBlock.sprite = pattern.GetSpecialSprite((int)(specialBlockData.type - 1));
		}
		else
		{
			srBlock.sprite = pattern.GetSprite();
		}
	}

	public void SetPattern(BlockType type)
	{
		if (type != BlockType.None)
		{
			RemoveSelectAndMatch();
			blockType = type;
			pattern = Pallete.GetPattern(blockType);
			srBlock.sprite = pattern.GetSprite();
			trSelect = MasterPoolManager.SpawnObject("Puzzle", string.Format("{0}{1}", "Block_select_", type.ToString()), trSelectAnchor);
			trMatch = MasterPoolManager.SpawnObject("Puzzle", string.Format("{0}{1}", "Block_chain_", type.ToString()), trMatchAnchor);
		}
	}

	public void SetRandomPattern()
	{
		RemoveSelectAndMatch();
		pattern = Pallete.GetRandomPattern();
		blockType = pattern.type;
		srBlock.sprite = pattern.GetSprite();
		trSelect = MasterPoolManager.SpawnObject("Puzzle", string.Format("{0}{1}", "Block_select_", blockType.ToString()), trSelectAnchor);
		trMatch = MasterPoolManager.SpawnObject("Puzzle", string.Format("{0}{1}", "Block_chain_", blockType.ToString()), trMatchAnchor);
	}

	public void On()
	{
		StopAllCoroutines();
		blockState = BlockState.Idle;
		specialBlockData.type = SpecialBlockType.None;
		specialBlockData.state = SpecialBlockState.None;
		exceptionType = BlockExceptionType.None;
		trSelectAnchor.gameObject.SetActive(value: false);
		trMatchAnchor.gameObject.SetActive(value: false);
		specialBlockPosition = Vector3.zero;
		trBlock.localScale = Vector3.one;
		CancelHint();
		DesapwnSpecial();
		ClearException();
	}

	public void SetSpecial(int chainCount)
	{
		StopAllCoroutines();
		trBlock.localScale = Vector3.one;
		chainCount -= 6;
		chainCount++;
		if (chainCount >= 1)
		{
			if (chainCount > 5)
			{
				chainCount = 5;
			}
			specialBlockData.type = (SpecialBlockType)chainCount;
			specialBlockData.state = SpecialBlockState.Wait;
			srBlock.sprite = pattern.GetSpecialSprite(chainCount - 1);
			LeanTween.scale(base.gameObject, specialBlockPingPongScale, 142f / (339f * (float)Math.PI)).setLoopPingPong(1).setEase(LeanTweenType.linear);
			trSpecialStanby = MasterPoolManager.SpawnObject("Puzzle", Pallete.GetSpecialStanbyName(blockType), base.transform);
		}
	}

	public void SetSpecialPosition(Vector3 _position)
	{
		specialBlockPosition = _position;
	}

	public void Move(float x, float y)
	{
		if (blockState != BlockState.Move)
		{
			trBlock.localScale = Vector3.one;
			StartCoroutine(moveCoroutine(x, y, blockState));
		}
	}

	public void SetSelect(bool _isTouch)
	{
		isSelect = _isTouch;
	}

	public void SetDeselect(bool _isTouch)
	{
		isDeselect = _isTouch;
	}

	public void TouchActive()
	{
		isSelect = true;
		isDeselect = true;
	}

	public void TouchLock()
	{
		isSelect = false;
		isDeselect = false;
	}

	public void Select(out Block block)
	{
		UnityEngine.Debug.Log("Select :: " + isSelect);
		if (BlockSelect != null)
		{
			BlockSelect(data.x, data.y);
		}
		if (!isSelect)
		{
			block = null;
			return;
		}
		if (exceptionType != 0 || blockState == BlockState.Select)
		{
			block = null;
			return;
		}
		block = this;
		trSelectAnchor.gameObject.SetActive(value: true);
		if (base.gameObject.layer == LayerMask.NameToLayer("Default"))
		{
			srBlock.sortingOrder = -8;
		}
		if (blockState != BlockState.MatchWait)
		{
			blockState = BlockState.Select;
		}
		StartScaleChange(Vector2.one * 0.9f, 0.1f);
	}

	public void DeSelect(out Block block, bool isCheckMove = true)
	{
		UnityEngine.Debug.Log("DeSelect :: " + isDeselect);
		if (!isDeselect)
		{
			block = this;
			return;
		}
		if (exceptionType != 0)
		{
			block = null;
			return;
		}
		base.gameObject.SetActive(value: true);
		block = null;
		trSelectAnchor.gameObject.SetActive(value: false);
		if (base.gameObject.layer == LayerMask.NameToLayer("Default"))
		{
			srBlock.sortingOrder = -9;
		}
		if (blockState != BlockState.MatchWait)
		{
			blockState = BlockState.Idle;
		}
		UnityEngine.Debug.Log("Deslect");
		StartScaleChange(Vector2.one, 0.1f);
		StartCoroutine(moveCoroutine(data.xPos, data.yPos, blockState, isCheckMove));
	}

	public void WaitMatch(bool isForceMatch = false)
	{
		trSelectAnchor.gameObject.SetActive(value: false);
		trMatchAnchor.gameObject.SetActive(value: true);
		StartScaleChange(Vector2.one, 0.1f);
		blockState = BlockState.MatchWait;
		if (isForceMatch)
		{
			StartMatch();
		}
		if (aniDefaultFix.gameObject.activeSelf)
		{
			SoundController.EffectSound_Play(EffectSoundType.LockBlockDestroy);
			aniDefaultFix.Play("obstruction_chain_Out");
		}
	}

	public void SetException(BlockExceptionType _type)
	{
		exceptionType = _type;
		switch (exceptionType)
		{
		case BlockExceptionType.Obstruction:
			RemoveSelectAndMatch();
			blockType = BlockType.None;
			aniObstruction.gameObject.SetActive(value: true);
			aniObstruction.Play("obstruction_stone_In");
			srBlock.sprite = null;
			SoundController.EffectSound_Play(EffectSoundType.ObstacleBlockAppear);
			break;
		case BlockExceptionType.DefaultFix:
			aniDefaultFix.gameObject.SetActive(value: true);
			aniDefaultFix.Play("obstruction_chain_In");
			SoundController.EffectSound_Play(EffectSoundType.LockBlockAppear);
			break;
		}
	}

	public void ReadyMatch()
	{
		blockState = BlockState.MatchReady;
		if (specialBlockData.state == SpecialBlockState.Wait)
		{
			specialBlockData.state = SpecialBlockState.Ready;
		}
	}

	public void StartMatch()
	{
		switch (specialBlockData.state)
		{
		case SpecialBlockState.None:
			if (blockState == BlockState.MatchReady && base.gameObject.activeSelf)
			{
				StartCoroutine(matchCoroutine());
			}
			break;
		case SpecialBlockState.Wait:
			trSelectAnchor.gameObject.SetActive(value: false);
			trMatchAnchor.gameObject.SetActive(value: false);
			blockState = BlockState.Idle;
			break;
		case SpecialBlockState.Ready:
			blockState = BlockState.Idle;
			break;
		case SpecialBlockState.Bomb:
			StartCoroutine(matchCoroutine());
			break;
		}
	}

	public void SetHintType(BlockHintType _hintType)
	{
		HideHint();
		hintType = _hintType;
	}

	public void ShowHint()
	{
		if (hintType != BlockHintType.Switch)
		{
			trDragHint.gameObject.SetActive(value: true);
			Vector3 eulerAngles = trDragHint.localRotation.eulerAngles;
			eulerAngles.z = (float)hintType * -45f;
			trDragHint.localRotation = Quaternion.Euler(eulerAngles);
		}
		else
		{
			LeanTween.scale(base.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f).setLoopPingPong().setEase(LeanTweenType.linear);
		}
	}

	public void ShowHintBoost()
	{
		trDragHintBoost.gameObject.SetActive(value: true);
		Vector3 eulerAngles = trDragHintBoost.localRotation.eulerAngles;
		eulerAngles.z = (float)hintType * -45f;
		trDragHintBoost.localRotation = Quaternion.Euler(eulerAngles);
	}

	public void HideHint()
	{
		trDragHint.gameObject.SetActive(value: false);
		trDragHintBoost.gameObject.SetActive(value: false);
		LeanTween.cancel(base.gameObject);
		trBlock.localScale = Vector3.one;
	}

	public void CancelHint()
	{
		HideHint();
		hintType = BlockHintType.None;
	}

	public void ReadySpecial()
	{
		blockState = BlockState.Idle;
		specialBlockData.state = SpecialBlockState.Ready;
		trMatchAnchor.gameObject.SetActive(value: true);
		LeanTween.scale(base.gameObject, specialBlockPingPongScale, 142f / (339f * (float)Math.PI)).setLoopPingPong(1).setEase(LeanTweenType.linear);
	}

	public void StartSpecialMatch(BlockType type, bool isSelf)
	{
		UnityEngine.Debug.Log("StartSpecialMatch!!!");
		blockState = BlockState.MatchWait;
		if (!isSelf)
		{
			if (SpeciaData.state == SpecialBlockState.Wait || SpeciaData.state == SpecialBlockState.Ready)
			{
				ReadySpecial();
			}
			else
			{
				StartCoroutine(ChangePattern(type));
			}
		}
		else
		{
			specialBlockData.state = SpecialBlockState.Bomb;
		}
	}

	public void CancelObstruction(BlockType _type)
	{
		StartCoroutine(matchCoroutine());
	}

	public void ClearException()
	{
		exceptionType = BlockExceptionType.None;
		aniObstruction.gameObject.SetActive(value: false);
		aniDefaultFix.gameObject.SetActive(value: false);
	}

	private void StartScaleChange(Vector2 end, float duration = 0.3f)
	{
		if (scaleBlockCoroutine != null)
		{
			StopCoroutine(scaleBlockCoroutine);
			scaleBlockCoroutine = null;
		}
		scaleBlockCoroutine = StartCoroutine(scaleCoroutine(end, duration));
	}

	private IEnumerator ChangePattern(BlockType type)
	{
		ClearException();
		blockType = type;
		SetPattern(type);
		trMatchAnchor.gameObject.SetActive(value: true);
		LeanTween.scale(base.gameObject, specialBlockPingPongScale, 142f / (339f * (float)Math.PI)).setLoopPingPong(1).setEase(LeanTweenType.linear);
		yield return new WaitForSeconds(0.4f);
	}

	private IEnumerator moveCoroutine(float x, float y, BlockState pre, bool isCheckMove = true)
	{
		Vector2 vector = (Vector2)trBlock.localPosition;
		Vector2 endPos = new Vector2(x, y);
		float time = Time.time;
		float duration = 0.35f;
		blockState = BlockState.Move;
		LeanTween.moveLocal(base.gameObject, endPos, duration).setEaseOutBack();
		yield return new WaitForSeconds(duration);
		blockState = pre;
		trBlock.localPosition = endPos;
		if (isCheckMove && BlockMoveComplete != null)
		{
			BlockMoveComplete(this);
		}
	}

	private IEnumerator matchCoroutine()
	{
		if (scaleBlockCoroutine != null)
		{
			StopCoroutine(scaleBlockCoroutine);
			scaleBlockCoroutine = null;
		}
		if (exceptionType == BlockExceptionType.Obstruction)
		{
			aniObstruction.Play("obstruction_stone_Out");
			SoundController.EffectSound_Play(EffectSoundType.ObstacleBlockDestroy);
			yield return new WaitForSeconds(0.3f);
		}
		else
		{
			yield return StartCoroutine(scaleCoroutine(Vector2.zero));
		}
		blockState = BlockState.MatchEnd;
		specialBlockData.type = SpecialBlockType.None;
		specialBlockData.state = SpecialBlockState.None;
		RemoveSelectAndMatch();
	}

	private IEnumerator scaleCoroutine(Vector2 endVec)
	{
		float time = Time.time;
		Vector2 vector = (Vector2)trBlock.localScale;
		LeanTween.scale(base.gameObject, endVec, 0.3f).setEaseOutCubic();
		yield return new WaitForSeconds(0.3f);
		trBlock.localScale = endVec;
	}

	private IEnumerator scaleCoroutine(Vector2 endVec, float duration)
	{
		float time = Time.time;
		Vector2 vector = (Vector2)trBlock.localScale;
		LeanTween.scale(base.gameObject, endVec, duration).setEaseOutCubic();
		yield return new WaitForSeconds(duration);
		trBlock.localScale = endVec;
	}

	private void RemoveSelectAndMatch()
	{
		if ((bool)trSelect)
		{
			trSelect.SetParent(null);
			MasterPoolManager.ReturnToPool("Puzzle", trSelect);
			trSelect = null;
		}
		if ((bool)trMatch)
		{
			trMatch.SetParent(null);
			MasterPoolManager.ReturnToPool("Puzzle", trMatch);
			trMatch = null;
		}
	}

	private void DesapwnSpecial()
	{
		if (trSpecialStanby != null)
		{
			MasterPoolManager.ReturnToPool("Puzzle", trSpecialStanby);
			trSpecialStanby = null;
		}
	}

	private void Awake()
	{
		trBlock = base.gameObject.transform;
		srBlock = base.gameObject.GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		base.gameObject.name = "block_" + data.x + "/" + data.y;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		DesapwnSpecial();
	}
}
