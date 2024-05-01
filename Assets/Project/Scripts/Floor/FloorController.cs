using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorController : MonoBehaviour
{
	public Action StartStoreOpen;

	public Action CollectComplete;

	private const float FLOOR_MOVE_START_POS_Y = 283f;

	private const float FLOOR_MOVE_GAP = 500f;

	[SerializeField]
	private Text textFloorControlName;

	[SerializeField]
	private FloorOffice officeFloor;

	[SerializeField]
	private List<FloorItem> listFloorItem = new List<FloorItem>();

	[SerializeField]
	private RectTransform rtTail;

	private ScrollRect scrollRect;

	private int stageId;

	public List<FloorItem> ListFloorItem => listFloorItem;

	public void Init(int id)
	{
		stageId = id;
		officeFloor.SetOffice($"Floor_office{stageId + 1}");
		textFloorControlName.text = MWLocalize.GetData(GameDataManager.GetDicStageDbData()[stageId + 1].castleName);
		SortFloorList();
		List<StoreDbData> storeListForStage = GameDataManager.GetStoreListForStage(stageId + 1);
		for (int i = 0; i < listFloorItem.Count; i++)
		{
			FloorItem floorItem = listFloorItem[i];
			UnityEngine.Debug.Log("FloorController Lenth :: " + GameInfo.userData.userFloorState[id].floorList.Length);
			if (i < GameInfo.userData.userFloorState[id].floorList.Length)
			{
				floorItem.Init(GameInfo.userData.userFloorState[id].floorList[i]);
				floorItem.SetStageId(stageId);
				floorItem.SetFloorId(i);
				floorItem.SetBlend(stageId != 0);
				FloorItem floorItem2 = floorItem;
				floorItem2.StartStoreOpen = (Action)Delegate.Combine(floorItem2.StartStoreOpen, new Action(OnStartStoreOpen));
				FloorItem floorItem3 = floorItem;
				floorItem3.CollectComplete = (Action)Delegate.Combine(floorItem3.CollectComplete, new Action(OnCollectComplete));
			}
			else
			{
				floorItem.SetDefaultProductData(storeListForStage[i].storeIdx);
				floorItem.SetStageId(stageId);
				floorItem.SetFloorId(i);
				floorItem.Lock();
			}
		}
	}

	public void Refresh()
	{
		for (int i = 0; i < listFloorItem.Count; i++)
		{
			FloorItem floorItem = listFloorItem[i];
			if (GameInfo.userData.userFloorState.Length > stageId && GameInfo.userData.userFloorState[stageId] != null)
			{
				if (i < GameInfo.userData.userFloorState[stageId].floorList.Length)
				{
					floorItem.Refresh(GameInfo.userData.userFloorState[stageId].floorList[i]);
				}
			}
		}
	}

	public void FloorForceCollect(int id)
	{
		listFloorItem[id].ForceCollectEffect();
	}

	public void MoveToFloor(int id)
	{
		scrollRect.content.anchoredPosition = new Vector2(0f, (float)id * 500f + 283f);
	}

	public void ShowStore()
	{
		for (int i = 0; i < listFloorItem.Count; i++)
		{
			listFloorItem[i].ShowStore();
		}
	}

	public void HideStore()
	{
		for (int i = 0; i < listFloorItem.Count; i++)
		{
			listFloorItem[i].HideStore();
		}
	}

	private void SortFloorList()
	{
		UnityEngine.Debug.Log("SortFloorList - stageId :: " + stageId);
		List<StoreDbData> storeListForStage = GameDataManager.GetStoreListForStage(stageId + 1);
		for (int i = 0; i < listFloorItem.Count; i++)
		{
			int floorTypeIndex = GetFloorTypeIndex((BlockType)storeListForStage[i].storeColor);
			if (floorTypeIndex > -1 && i != floorTypeIndex)
			{
				ChangeFloorList(i, floorTypeIndex);
			}
		}
		for (int j = 0; j < listFloorItem.Count; j++)
		{
			listFloorItem[j].ChangeIndex(2 + j);
		}
		rtTail.SetSiblingIndex(2 + listFloorItem.Count);
	}

	private void ChangeFloorList(int firstIndex, int secondIdex)
	{
		FloorItem value = listFloorItem[firstIndex];
		FloorItem value2 = listFloorItem[secondIdex];
		listFloorItem[secondIdex] = value;
		listFloorItem[firstIndex] = value2;
	}

	private int GetFloorTypeIndex(BlockType type)
	{
		for (int i = 0; i < listFloorItem.Count; i++)
		{
			FloorItem floorItem = listFloorItem[i];
			if (floorItem.FloorType == type)
			{
				return i;
			}
		}
		return -1;
	}

	private void OnStartStoreOpen()
	{
		if (StartStoreOpen != null)
		{
			StartStoreOpen();
		}
	}

	private void OnCollectComplete()
	{
		if (CollectComplete != null)
		{
			CollectComplete();
		}
	}

	private void Awake()
	{
		scrollRect = base.gameObject.GetComponent<ScrollRect>();
		scrollRect.content.anchoredPosition = Vector3.zero;
	}

	private void LateUpdate()
	{
		if (officeFloor != null)
		{
			officeFloor.SyncOffice();
		}
		for (int i = 0; i < listFloorItem.Count; i++)
		{
			listFloorItem[i].SyncStore();
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < listFloorItem.Count; i++)
		{
			FloorItem floorItem = listFloorItem[i];
			FloorItem floorItem2 = floorItem;
			floorItem2.StartStoreOpen = (Action)Delegate.Remove(floorItem2.StartStoreOpen, new Action(OnStartStoreOpen));
			FloorItem floorItem3 = floorItem;
			floorItem3.CollectComplete = (Action)Delegate.Remove(floorItem3.CollectComplete, new Action(OnCollectComplete));
		}
		scrollRect.content.anchoredPosition = Vector3.zero;
	}
}
