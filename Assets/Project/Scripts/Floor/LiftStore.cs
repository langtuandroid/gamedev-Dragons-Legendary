using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class LiftStore : MonoBehaviour
{
	
	[FormerlySerializedAs("srBlend")] [SerializeField]
	private SpriteRenderer _spriteRendererBlend;

	[FormerlySerializedAs("animatorFloor")] [SerializeField]
	private Animator _floorAnimator;

	[FormerlySerializedAs("goDimmed")] [SerializeField]
	private GameObject _dimmedGameObject;

	private LiftStateType _stateType;

	public void BlendSet(int _stageIdx)
	{
		_spriteRendererBlend.sprite = GameDataManager.GetStageStoreBlendSprite(_stageIdx);
	}

	public void Activate()
	{
		if (_stateType != LiftStateType.Open)
		{
			_stateType = LiftStateType.Open;
			_dimmedGameObject.SetActive(value: false);
			_floorAnimator.enabled = true;
			StartCoroutine(IsCull());
		}
	}

	public void Deactivate()
	{
		if (_stateType != LiftStateType.Close)
		{
			_stateType = LiftStateType.Close;
			_dimmedGameObject.SetActive(value: true);
			_floorAnimator.enabled = true;
			StartCoroutine(DelayAnimationDisable());
		}
	}

	public void UnlockEffectSpawn()
	{
		MasterPoolManager.SpawnObject("Effect", "FX_floor_unlock", base.transform, 8f);
	}

	public void WowShow()
	{
		base.gameObject.SetActive(value: true);
		if (_stateType == LiftStateType.Open)
		{
			_dimmedGameObject.SetActive(value: false);
			StartCoroutine(IsCull());
		}
		else
		{
			_dimmedGameObject.SetActive(value: true);
			StartCoroutine(DelayAnimationDisable());
		}
	}

	public void SkibidiHide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Refresh()
	{
		switch (_stateType)
		{
		case LiftStateType.Open:
			Activate();
			break;
		case LiftStateType.Close:
			Deactivate();
			break;
		}
	}

	private IEnumerator IsCull()
	{
		_floorAnimator.enabled = true;
		yield return null;
		_floorAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		yield return null;
		_floorAnimator.Play("floor_idle");
		yield return null;
	}

	private IEnumerator DelayAnimationDisable()
	{
		_floorAnimator.enabled = true;
		yield return null;
		_floorAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		_floorAnimator.Play("floor_stop");
		yield return null;
		_floorAnimator.enabled = false;
	}

	private void OnEnable()
	{
		if (_floorAnimator != null)
		{
			_floorAnimator.enabled = true;
		}
	}
}
