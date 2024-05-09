using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LiftController : MonoBehaviour
{
	 public Action OnStoreOpen;

	public Action CollectComplete;

	[FormerlySerializedAs("textFloorControlName")] [SerializeField]
	private Text _floorNameText;

	[FormerlySerializedAs("officeFloor")] [SerializeField]
	private LiftOffice _liftOffice;

	[FormerlySerializedAs("listFloorItem")] [SerializeField]
	private List<LiftItem> _floorItem = new List<LiftItem>();

	[FormerlySerializedAs("rtTail")] [SerializeField]
	private RectTransform _tailRectTransform;

	private ScrollRect _scroll;

	private int _stageNumber;

	public List<LiftItem> FloorItem => _floorItem;

	public void Construct(int id)
	{
		_stageNumber = id;
		_liftOffice.PlaceOffice($"Floor_office{_stageNumber + 1}");
		_floorNameText.text = MasterLocalize.GetData(GameDataManager.GetDicStageDbData()[_stageNumber + 1].castleName);
		SortLiftList();
		List<StoreDbData> storeListForStage = GameDataManager.GetStoreListForStage(_stageNumber + 1);
		for (int i = 0; i < _floorItem.Count; i++)
		{
			LiftItem floorItem = _floorItem[i];
			UnityEngine.Debug.Log("FloorController Lenth :: " + GameInfo.userData.userFloorState[id].floorList.Length);
			if (i < GameInfo.userData.userFloorState[id].floorList.Length)
			{
				floorItem.Construct(GameInfo.userData.userFloorState[id].floorList[i]);
				floorItem.StafeIdSet(_stageNumber);
				floorItem.FloorSet(i);
				floorItem.ChangeBlend(_stageNumber != 0);
				LiftItem floorItem2 = floorItem;
				floorItem2.OnStoreOpen = (Action)Delegate.Combine(floorItem2.OnStoreOpen, new Action(StoreOpenEvent));
				LiftItem floorItem3 = floorItem;
				floorItem3.OnCompleteCollect = (Action)Delegate.Combine(floorItem3.OnCompleteCollect, new Action(OnCollectEvent));
			}
			else
			{
				floorItem.ResetProdictData(storeListForStage[i].storeIdx);
				floorItem.StafeIdSet(_stageNumber);
				floorItem.FloorSet(i);
				floorItem.LockLift();
			}
		}
	}

	public void ResetEverything()
	{
		for (int i = 0; i < _floorItem.Count; i++)
		{
			LiftItem floorItem = _floorItem[i];
			if (GameInfo.userData.userFloorState.Length > _stageNumber && GameInfo.userData.userFloorState[_stageNumber] != null)
			{
				if (i < GameInfo.userData.userFloorState[_stageNumber].floorList.Length)
				{
					floorItem.RefreshLift(GameInfo.userData.userFloorState[_stageNumber].floorList[i]);
				}
			}
		}
	}

	public void CollectLift(int id)
	{
		_floorItem[id].RunCollection();
	}

	public void MoveToLift(int id)
	{
		_scroll.content.anchoredPosition = new Vector2(0f, (float)id * 500f + 283f);
	}

	public void OpenStore()
	{
		for (int i = 0; i < _floorItem.Count; i++)
		{
			_floorItem[i].StoreOpen();
		}
	}

	public void CloseStore()
	{
		for (int i = 0; i < _floorItem.Count; i++)
		{
			_floorItem[i].CloseStore();
		}
	}

	private void SortLiftList()
	{
		UnityEngine.Debug.Log("SortFloorList - stageId :: " + _stageNumber);
		List<StoreDbData> storeListForStage = GameDataManager.GetStoreListForStage(_stageNumber + 1);
		for (int i = 0; i < _floorItem.Count; i++)
		{
			int floorTypeIndex = GetListTypeIndex((BlockType)storeListForStage[i].storeColor);
			if (floorTypeIndex > -1 && i != floorTypeIndex)
			{
				ChangeLiftList(i, floorTypeIndex);
			}
		}
		for (int j = 0; j < _floorItem.Count; j++)
		{
			_floorItem[j].ChangeIndex(2 + j);
		}
		_tailRectTransform.SetSiblingIndex(2 + _floorItem.Count);
	}

	private void ChangeLiftList(int firstIndex, int secondIdex)
	{
		LiftItem value = _floorItem[firstIndex];
		LiftItem value2 = _floorItem[secondIdex];
		_floorItem[secondIdex] = value;
		_floorItem[firstIndex] = value2;
	}

	private int GetListTypeIndex(BlockType type)
	{
		for (int i = 0; i < _floorItem.Count; i++)
		{
			LiftItem floorItem = _floorItem[i];
			if (floorItem.FloorBlockType == type)
			{
				return i;
			}
		}
		return -1;
	}

	private void StoreOpenEvent()
	{
		if (OnStoreOpen != null)
		{
			OnStoreOpen();
		}
	}

	private void OnCollectEvent()
	{
		if (CollectComplete != null)
		{
			CollectComplete();
		}
	}

	private void Awake()
	{
		_scroll = base.gameObject.GetComponent<ScrollRect>();
		_scroll.content.anchoredPosition = Vector3.zero;
	}

	private void LateUpdate()
	{
		if (_liftOffice != null)
		{
			_liftOffice.SaveOffice();
		}
		for (int i = 0; i < _floorItem.Count; i++)
		{
			_floorItem[i].SyncronizeStore();
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < _floorItem.Count; i++)
		{
			LiftItem floorItem = _floorItem[i];
			LiftItem floorItem2 = floorItem;
			floorItem2.OnStoreOpen = (Action)Delegate.Remove(floorItem2.OnStoreOpen, new Action(StoreOpenEvent));
			LiftItem floorItem3 = floorItem;
			floorItem3.OnCompleteCollect = (Action)Delegate.Remove(floorItem3.OnCompleteCollect, new Action(OnCollectEvent));
		}
		_scroll.content.anchoredPosition = Vector3.zero;
	}
}
