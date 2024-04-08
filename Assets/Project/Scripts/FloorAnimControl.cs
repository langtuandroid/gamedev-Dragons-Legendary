using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloorAnimControl : MonoBehaviour
{
	public Action ShowBadgeGauge;

	public Action StartBadgeAnimation;

	public Action ProduceRewardComplete;

	public Action AllEffectComplete;

	private const float USER_OWN_ITME_SHOW_TIME = 2f;

	[SerializeField]
	private Text textCoin;

	[SerializeField]
	private Text textRatioCount;

	[SerializeField]
	private Text textOwnItemCount;

	[SerializeField]
	private Text textUserGetBadgeCount;

	[SerializeField]
	private Text textUserGetBadgeCountResult;

	[SerializeField]
	private GameObject goUserOwnItem;

	[SerializeField]
	private GameObject goTouchCollect;

	[SerializeField]
	private GameObject[] arrBadge = new GameObject[0];

	private int itemIdx;

	private Animation aniFloor;

	public void Init()
	{
		aniFloor = base.gameObject.GetComponent<Animation>();
		goTouchCollect.SetActive(value: false);
	}

	public void Play()
	{
		if (aniFloor == null)
		{
			aniFloor = base.gameObject.GetComponent<Animation>();
		}
		aniFloor["FloorScrollView_Calculations"].speed = 1f;
		goUserOwnItem.SetActive(value: false);
		aniFloor.Rewind();
		aniFloor.Play();
	}

	public void Clear()
	{
		StopAllCoroutines();
		if (aniFloor == null)
		{
			aniFloor = base.gameObject.GetComponent<Animation>();
		}
		aniFloor["FloorScrollView_Calculations"].speed = 1f;
	}

	public void Continue()
	{
		if (aniFloor == null)
		{
			aniFloor = base.gameObject.GetComponent<Animation>();
		}
		aniFloor["FloorScrollView_Calculations"].speed = 1f;
	}

	public void SetCoin(int _coin)
	{
		textCoin.text = $"{_coin}";
	}

	public void SetBadgeCount(int _count)
	{
		textUserGetBadgeCount.text = $"x {_count}";
		textUserGetBadgeCountResult.text = $"x {_count}";
	}

	public void SetRatioCount(int _count)
	{
		if (_count == 0)
		{
			_count = 5;
		}
		textRatioCount.text = $"{_count}/5";
	}

	public void SetBadgeIdx(int _idx)
	{
		itemIdx = _idx;
	}

	public void OnShowBadgeGauge()
	{
		if (ShowBadgeGauge != null)
		{
			ShowBadgeGauge();
		}
	}

	public void OnStartBadgeGaugeAnimation()
	{
		if (aniFloor == null)
		{
			aniFloor = base.gameObject.GetComponent<Animation>();
		}
		aniFloor["FloorScrollView_Calculations"].speed = 0f;
		if (StartBadgeAnimation != null)
		{
			StartBadgeAnimation();
		}
	}

	public void OnStoreProduceRewardComplete()
	{
		if (aniFloor == null)
		{
			aniFloor = base.gameObject.GetComponent<Animation>();
		}
		aniFloor["FloorScrollView_Calculations"].speed = 0f;
		if (ProduceRewardComplete != null)
		{
			ProduceRewardComplete();
		}
	}

	public void OnClickResult()
	{
		StartCoroutine(ProcessShowUserOwnItem());
		goTouchCollect.SetActive(value: false);
		UnityEngine.Debug.Log("OnClickResult :: " + GameInfo.userData.GetItemCount(itemIdx));
		textOwnItemCount.text = string.Format(MWLocalize.GetData("common_text_owned"), GameInfo.userData.GetItemCount(itemIdx));
		LobbyManager.StoreBadgeCollect();
		SoundController.EffectSound_Play(EffectSoundType.GetMedal);
	}

	private IEnumerator ProcessShowUserOwnItem()
	{
		goUserOwnItem.SetActive(value: true);
		yield return new WaitForSeconds(2f);
		goUserOwnItem.SetActive(value: false);
		if (AllEffectComplete != null)
		{
			AllEffectComplete();
		}
	}

	private void Awake()
	{
		aniFloor = base.gameObject.GetComponent<Animation>();
		aniFloor.Play();
		goTouchCollect.SetActive(value: false);
	}
}
