using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LiftAnimControl : MonoBehaviour
{
	public Action openBadgeGauge;

	public Action begingBadgeAnimation;

	public Action OnReward;

	public Action OnEffectsReady;

	[FormerlySerializedAs("textCoin")] [SerializeField]
	private Text _coinText;

	[FormerlySerializedAs("textRatioCount")] [SerializeField]
	private Text _ratioCountText;

	[FormerlySerializedAs("textOwnItemCount")] [SerializeField]
	private Text _itemCountText;

	[FormerlySerializedAs("textUserGetBadgeCount")] [SerializeField]
	private Text _getBadgeCount;

	[FormerlySerializedAs("textUserGetBadgeCountResult")] [SerializeField]
	private Text _budgeTextResult;

	[FormerlySerializedAs("goUserOwnItem")] [SerializeField]
	private GameObject _userItemOwn;

	[FormerlySerializedAs("goTouchCollect")] [SerializeField]
	private GameObject _touchCollect;

	[SerializeField]
	private GameObject[] arrBadge = new GameObject[0];

	private int _itemIndex;

	private Animation _animator;

	public void Construct()
	{
		_animator = base.gameObject.GetComponent<Animation>();
		_touchCollect.SetActive(value: false);
	}

	public void StartPlay()
	{
		if (_animator == null)
		{
			_animator = base.gameObject.GetComponent<Animation>();
		}
		_animator["FloorScrollView_Calculations"].speed = 1f;
		_userItemOwn.SetActive(value: false);
		_animator.Rewind();
		_animator.Play();
	}

	public void RemoveAll()
	{
		StopAllCoroutines();
		if (_animator == null)
		{
			_animator = base.gameObject.GetComponent<Animation>();
		}
		_animator["FloorScrollView_Calculations"].speed = 1f;
	}

	public void Next()
	{
		if (_animator == null)
		{
			_animator = base.gameObject.GetComponent<Animation>();
		}
		_animator["FloorScrollView_Calculations"].speed = 1f;
	}

	public void PlaceCoin(int _coin)
	{
		_coinText.text = $"{_coin}";
	}

	public void PlaceBudges(int _count)
	{
		_getBadgeCount.text = $"x {_count}";
		_budgeTextResult.text = $"x {_count}";
	}

	public void CountRaio(int _count)
	{
		if (_count == 0)
		{
			_count = 5;
		}
		_ratioCountText.text = $"{_count}/5";
	}

	public void BadgeIndex(int _idx)
	{
		_itemIndex = _idx;
	}

	public void OnShowBadgeGauge()
	{
		if (openBadgeGauge != null)
		{
			openBadgeGauge();
		}
	}

	public void OnStartBadgeGaugeAnimation()
	{
		if (_animator == null)
		{
			_animator = base.gameObject.GetComponent<Animation>();
		}
		_animator["FloorScrollView_Calculations"].speed = 0f;
		if (begingBadgeAnimation != null)
		{
			begingBadgeAnimation();
		}
	}

	public void OnStoreProduceRewardComplete()
	{
		if (_animator == null)
		{
			_animator = base.gameObject.GetComponent<Animation>();
		}
		_animator["FloorScrollView_Calculations"].speed = 0f;
		if (OnReward != null)
		{
			OnReward();
		}
	}

	public void OnClickResult()
	{
		StartCoroutine(ShowOwnItem());
		_touchCollect.SetActive(value: false);
		UnityEngine.Debug.Log("OnClickResult :: " + GameInfo.userData.GetItemCount(_itemIndex));
		_itemCountText.text = string.Format(MasterLocalize.GetData("common_text_owned"), GameInfo.userData.GetItemCount(_itemIndex));
		LobbyManager.StoreBadgeCollect();
		SoundController.EffectSound_Play(EffectSoundType.GetMedal);
	}

	private IEnumerator ShowOwnItem()
	{
		_userItemOwn.SetActive(value: true);
		yield return new WaitForSeconds(2f);
		_userItemOwn.SetActive(value: false);
		if (OnEffectsReady != null)
		{
			OnEffectsReady();
		}
	}

	private void Awake()
	{
		_animator = base.gameObject.GetComponent<Animation>();
		_animator.Play();
		_touchCollect.SetActive(value: false);
	}
}
