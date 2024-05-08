using System.Collections;
using UnityEngine;

public class FloorStore : MonoBehaviour
{
	private const string FLOOR_TEMPLE_IDLE = "floor_idle";

	private const string FLOOR_TEMPLE_STOP = "floor_stop";

	[SerializeField]
	private SpriteRenderer srBlend;

	[SerializeField]
	private Animator animatorFloor;

	[SerializeField]
	private GameObject goDimmed;

	private FloorStateType stateType;

	public void SetBlend(int _stageIdx)
	{
		srBlend.sprite = GameDataManager.GetStageStoreBlendSprite(_stageIdx);
	}

	public void Open()
	{
		if (stateType != FloorStateType.Open)
		{
			stateType = FloorStateType.Open;
			goDimmed.SetActive(value: false);
			animatorFloor.enabled = true;
			StartCoroutine(CheckCullMode());
		}
	}

	public void Close()
	{
		if (stateType != FloorStateType.Close)
		{
			stateType = FloorStateType.Close;
			goDimmed.SetActive(value: true);
			animatorFloor.enabled = true;
			StartCoroutine(CheckDelayAnimOff());
		}
	}

	public void ShowUnLockEffect()
	{
		MasterPoolManager.SpawnObject("Effect", "FX_floor_unlock", base.transform, 8f);
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		if (stateType == FloorStateType.Open)
		{
			goDimmed.SetActive(value: false);
			StartCoroutine(CheckCullMode());
		}
		else
		{
			goDimmed.SetActive(value: true);
			StartCoroutine(CheckDelayAnimOff());
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Refresh()
	{
		switch (stateType)
		{
		case FloorStateType.Open:
			Open();
			break;
		case FloorStateType.Close:
			Close();
			break;
		}
	}

	private IEnumerator CheckCullMode()
	{
		animatorFloor.enabled = true;
		yield return null;
		animatorFloor.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		yield return null;
		animatorFloor.Play("floor_idle");
		yield return null;
	}

	private IEnumerator CheckDelayAnimOff()
	{
		animatorFloor.enabled = true;
		yield return null;
		animatorFloor.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		animatorFloor.Play("floor_stop");
		yield return null;
		animatorFloor.enabled = false;
	}

	private void OnEnable()
	{
		if (animatorFloor != null)
		{
			animatorFloor.enabled = true;
		}
	}
}
