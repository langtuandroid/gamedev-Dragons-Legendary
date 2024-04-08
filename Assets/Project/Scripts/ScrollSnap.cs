using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(ScrollRect))]
public class ScrollSnap : UIBehaviour, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public class OnLerpCompleteEvent : UnityEvent
	{
	}

	public class OnReleaseEvent : UnityEvent<int>
	{
	}

	public Action<int> ScrollPageSnapEvent;

	[SerializeField]
	public int startingIndex;

	[SerializeField]
	public bool wrapAround;

	[SerializeField]
	public float lerpTimeMilliSeconds = 200f;

	[SerializeField]
	public float triggerPercent = 5f;

	[Range(0f, 10f)]
	public float triggerAcceleration = 1f;

	public OnLerpCompleteEvent onLerpComplete;

	public OnReleaseEvent onRelease;

	private int actualIndex;

	private int cellIndex;

	private ScrollRect scrollRect;

	private CanvasGroup canvasGroup;

	private RectTransform content;

	private Vector2 cellSize;

	private Vector2 spacingSize;

	private bool indexChangeTriggered;

	private bool isLerping;

	private DateTime lerpStartedAt;

	private Vector2 releasedPosition;

	private Vector2 targetPosition;

	public int CurrentIndex
	{
		get
		{
			int num = LayoutElementCount();
			int num2 = actualIndex % num;
			return (num2 < 0) ? (num + num2) : num2;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		actualIndex = startingIndex;
		cellIndex = startingIndex;
		onLerpComplete = new OnLerpCompleteEvent();
		onRelease = new OnReleaseEvent();
		scrollRect = GetComponent<ScrollRect>();
		canvasGroup = GetComponent<CanvasGroup>();
		content = scrollRect.content;
		cellSize = new Vector2(720f, 0f);
		spacingSize = Vector2.zero;
		RectTransform rectTransform = content;
		float x = (0f - cellSize.x) * (float)cellIndex;
		Vector2 anchoredPosition = content.anchoredPosition;
		rectTransform.anchoredPosition = new Vector2(x, anchoredPosition.y);
		int num = LayoutElementCount();
		SetContentSize(num);
		if (startingIndex < num)
		{
			MoveToIndex(startingIndex);
		}
	}

	private void LateUpdate()
	{
		if (!isLerping)
		{
			return;
		}
		LerpToElement();
		if (ShouldStopLerping())
		{
			isLerping = false;
			canvasGroup.blocksRaycasts = true;
			onLerpComplete.Invoke();
			onLerpComplete.RemoveListener(WrapElementAround);
			if (ScrollPageSnapEvent != null)
			{
				ScrollPageSnapEvent(cellIndex);
			}
		}
	}

	public void PushLayoutElement(LayoutElement element)
	{
		element.transform.SetParent(content.transform, worldPositionStays: false);
		SetContentSize(LayoutElementCount());
	}

	public void PopLayoutElement()
	{
		LayoutElement[] componentsInChildren = content.GetComponentsInChildren<LayoutElement>();
		UnityEngine.Object.Destroy(componentsInChildren[componentsInChildren.Length - 1].gameObject);
		SetContentSize(LayoutElementCount() - 1);
		if (cellIndex == CalculateMaxIndex())
		{
			cellIndex--;
		}
	}

	public void UnshiftLayoutElement(LayoutElement element)
	{
		cellIndex++;
		element.transform.SetParent(content.transform, worldPositionStays: false);
		element.transform.SetAsFirstSibling();
		SetContentSize(LayoutElementCount());
		RectTransform rectTransform = content;
		Vector2 anchoredPosition = content.anchoredPosition;
		float x = anchoredPosition.x - cellSize.x;
		Vector2 anchoredPosition2 = content.anchoredPosition;
		rectTransform.anchoredPosition = new Vector2(x, anchoredPosition2.y);
	}

	public void ShiftLayoutElement()
	{
		UnityEngine.Object.Destroy(GetComponentInChildren<LayoutElement>().gameObject);
		SetContentSize(LayoutElementCount() - 1);
		cellIndex--;
		RectTransform rectTransform = content;
		Vector2 anchoredPosition = content.anchoredPosition;
		float x = anchoredPosition.x + cellSize.x;
		Vector2 anchoredPosition2 = content.anchoredPosition;
		rectTransform.anchoredPosition = new Vector2(x, anchoredPosition2.y);
	}

	public int LayoutElementCount()
	{
		return content.GetComponentsInChildren<LayoutElement>(includeInactive: false).Count((LayoutElement e) => e.transform.parent == content);
	}

	public void OnDrag(PointerEventData data)
	{
		Vector2 delta = data.delta;
		float x = delta.x;
		float num = Time.deltaTime * 1000f;
		float num2 = Mathf.Abs(x / num);
		if (num2 > triggerAcceleration && num2 != float.PositiveInfinity)
		{
			indexChangeTriggered = true;
		}
	}

	public void OnEndDrag(PointerEventData data)
	{
		if (IndexShouldChangeFromDrag(data))
		{
			Vector2 pressPosition = data.pressPosition;
			float x = pressPosition.x;
			Vector2 position = data.position;
			int num = (x - position.x > 0f) ? 1 : (-1);
			SnapToIndex(cellIndex + num);
		}
		else
		{
			StartLerping();
		}
	}

	public void SnapToNext()
	{
		SnapToIndex(cellIndex + 1);
	}

	public void SnapToPrev()
	{
		SnapToIndex(cellIndex - 1);
	}

	public void SnapToIndex(int newCellIndex)
	{
		int num = CalculateMaxIndex();
		if (wrapAround && num > 0)
		{
			actualIndex += newCellIndex - cellIndex;
			cellIndex = newCellIndex;
			onLerpComplete.AddListener(WrapElementAround);
		}
		else if (newCellIndex >= 0 && newCellIndex <= num)
		{
			actualIndex += newCellIndex - cellIndex;
			cellIndex = newCellIndex;
		}
		onRelease.Invoke(cellIndex);
		StartLerping();
	}

	public void MoveToIndex(int newCellIndex)
	{
		int num = CalculateMaxIndex();
		if (newCellIndex >= 0 && newCellIndex <= num)
		{
			actualIndex += newCellIndex - cellIndex;
			cellIndex = newCellIndex;
		}
		onRelease.Invoke(cellIndex);
		content.anchoredPosition = CalculateTargetPoisition(cellIndex);
	}

	private void StartLerping()
	{
		releasedPosition = content.anchoredPosition;
		targetPosition = CalculateTargetPoisition(cellIndex);
		lerpStartedAt = DateTime.Now;
		canvasGroup.blocksRaycasts = false;
		isLerping = true;
	}

	private int CalculateMaxIndex()
	{
		Vector2 size = scrollRect.GetComponent<RectTransform>().rect.size;
		int num = Mathf.FloorToInt(size.x / cellSize.x);
		return LayoutElementCount() - num;
	}

	private bool IndexShouldChangeFromDrag(PointerEventData data)
	{
		if (indexChangeTriggered)
		{
			indexChangeTriggered = false;
			return true;
		}
		Vector2 anchoredPosition = scrollRect.content.anchoredPosition;
		float num = anchoredPosition.x + (float)cellIndex * cellSize.x;
		float num2 = Mathf.Abs(num / cellSize.x);
		return num2 * 100f > triggerPercent;
	}

	private void LerpToElement()
	{
		float t = (float)((DateTime.Now - lerpStartedAt).TotalMilliseconds / (double)lerpTimeMilliSeconds);
		float num = Mathf.Lerp(releasedPosition.x, targetPosition.x, t);
		RectTransform rectTransform = content;
		float x = num;
		Vector2 anchoredPosition = content.anchoredPosition;
		rectTransform.anchoredPosition = new Vector2(x, anchoredPosition.y);
	}

	private void WrapElementAround()
	{
		if (cellIndex <= 0)
		{
			LayoutElement[] componentsInChildren = content.GetComponentsInChildren<LayoutElement>();
			componentsInChildren[componentsInChildren.Length - 1].transform.SetAsFirstSibling();
			cellIndex++;
			RectTransform rectTransform = content;
			Vector2 anchoredPosition = content.anchoredPosition;
			float x = anchoredPosition.x - cellSize.x;
			Vector2 anchoredPosition2 = content.anchoredPosition;
			rectTransform.anchoredPosition = new Vector2(x, anchoredPosition2.y);
		}
		else if (cellIndex >= CalculateMaxIndex())
		{
			LayoutElement componentInChildren = content.GetComponentInChildren<LayoutElement>();
			componentInChildren.transform.SetAsLastSibling();
			cellIndex--;
			RectTransform rectTransform2 = content;
			Vector2 anchoredPosition3 = content.anchoredPosition;
			float x2 = anchoredPosition3.x + cellSize.x;
			Vector2 anchoredPosition4 = content.anchoredPosition;
			rectTransform2.anchoredPosition = new Vector2(x2, anchoredPosition4.y);
		}
	}

	private void SetContentSize(int elementCount)
	{
	}

	private Vector2 CalculateTargetPoisition(int index)
	{
		float x = (0f - (cellSize.x + spacingSize.x)) * (float)index;
		Vector2 anchoredPosition = content.anchoredPosition;
		return new Vector2(x, anchoredPosition.y);
	}

	private bool ShouldStopLerping()
	{
		Vector2 anchoredPosition = content.anchoredPosition;
		return (double)Mathf.Abs(anchoredPosition.x - targetPosition.x) < 0.001;
	}
}
