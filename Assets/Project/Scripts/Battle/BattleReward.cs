using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour
{
	public enum RewardState
	{
		None = 1,
		Open,
		Result,
		Ads
	}

	[SerializeField]
	private Transform pick_Tr;

	[SerializeField]
	private Transform[] pickItem_Arr;

	[SerializeField]
	private Transform result_Tr;

	[SerializeField]
	private Transform[] resultItem_Arr;

	[SerializeField]
	private List<int> open_chestlist_idx;

	[SerializeField]
	private Transform alreadyFound_Tr;

	[SerializeField]
	private Transform[] alreadyFound_List;

	[SerializeField]
	private GameObject claim_BT;

	[SerializeField]
	private GameObject pickMore_BT;

	[SerializeField]
	private GameObject adReturn;

	[SerializeField]
	private bool[] isOpenItem_Arr;

	[SerializeField]
	private List<ChestListDbData> chestListDbDataList;

	[SerializeField]
	private int[] chestListDbData_Idx;

	[SerializeField]
	private Transform getItem_Tr;

	[SerializeField]
	private RewardState rewardState;

	[SerializeField]
	private int pickCount;

	[SerializeField]
	private List<int> idxList = new List<int>();

	[SerializeField]
	private bool isPickMore;

	[SerializeField]
	private Transform bonus_text;

	private int openIdx;

	public Transform PickItem => getItem_Tr;

	public void Init()
	{
		base.gameObject.SetActive(value: true);
		pickCount = 0;
		GetItemList();
		for (int i = 0; i < alreadyFound_List.Length; i++)
		{
			if (alreadyFound_List[i].childCount > 0)
			{
				MWPoolManager.DeSpawn("Item", alreadyFound_List[i].GetChild(0));
			}
		}
		for (int j = 0; j < isOpenItem_Arr.Length; j++)
		{
			isOpenItem_Arr[j] = false;
		}
		open_chestlist_idx = new List<int>();
	}

	public void End_BattleRewardOpen_Anim()
	{
		SetResultState();
	}

	public void PickMore_Ads()
	{
		
	}

	
	public void Callback_Ads()
	{
		rewardState = RewardState.None;
		RewardRefresh();
	}

	private void RewardRefresh()
	{
		chestListDbData_Idx = getShuffleArray(chestListDbDataList.Count, 0, chestListDbDataList.Count);
		pick_Tr.gameObject.SetActive(value: true);
		result_Tr.gameObject.SetActive(value: false);
		SetPickState();
		rewardState = RewardState.None;
	}

	private void GetItemList()
	{
		if (InGamePlayManager.BattleRewardTutorial == null)
		{
			UnityEngine.Debug.Log("Tutorial After 11");
			Protocol_Set.Protocol_chest_collect_Req(4, GetItemListResponse);
		}
		else
		{
			UnityEngine.Debug.Log("Tutorial After 22");
			Protocol_Set.Protocol_chest_collect_Req(4, GetItemListResponse, "n", isTutorial: true, 3);
		}
	}

	private void GetItemListResponse(ChestListDbData[] _itemList)
	{
		chestListDbDataList = new List<ChestListDbData>();
		for (int i = 0; i < _itemList.Length; i++)
		{
			chestListDbDataList.Add(_itemList[i]);
		}
		RewardRefresh();
	}

	private void SetPickState()
	{
		for (int i = 0; i < isOpenItem_Arr.Length; i++)
		{
			if (isOpenItem_Arr[i])
			{
				pickItem_Arr[i].gameObject.SetActive(value: false);
				continue;
			}
			pickItem_Arr[i].gameObject.SetActive(value: true);
			SetAnimation(pickItem_Arr[i], "idle");
		}
	}

	private void SetResultState()
	{
		TutorialManager.CheckBattleReward();
		pick_Tr.gameObject.SetActive(value: false);
		result_Tr.gameObject.SetActive(value: true);
		SoundController.EffectSound_Play(EffectSoundType.ChestBoxIdle);
		if (pickCount >= 3)
		{
			claim_BT.SetActive(value: true);
			pickMore_BT.SetActive(value: false);
			bonus_text.gameObject.SetActive(value: false);
		}
		else if (GameInfo.userData.userInfo.ad_chest_limit > 0)
		{
			claim_BT.SetActive(value: true);
			pickMore_BT.SetActive(value: true);
			bonus_text.gameObject.SetActive(value: true);
			adReturn.SetActive(value: false);
		}
		else if (GameInfo.userData.userInfo.ad_chest_limit <= 0)
		{
			claim_BT.SetActive(value: true);
			pickMore_BT.SetActive(value: true);
			bonus_text.gameObject.SetActive(value: false);
			adReturn.SetActive(value: true);
		}
		else
		{
			claim_BT.SetActive(value: true);
			pickMore_BT.SetActive(value: true);
			adReturn.SetActive(value: false);
		}
		if (GameInfo.inGamePlayData.levelIdx <= 3)
		{
			pickMore_BT.SetActive(value: false);
			adReturn.SetActive(value: false);
			bonus_text.gameObject.SetActive(value: false);
		}
		rewardState = RewardState.None;
		for (int i = 0; i < isOpenItem_Arr.Length; i++)
		{
			if (isOpenItem_Arr[i] && openIdx == i)
			{
				if (resultItem_Arr[i].childCount > 1)
				{
					resultItem_Arr[i].GetChild(0).GetComponent<Image>().color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					MWPoolManager.DeSpawn("Item", resultItem_Arr[i].GetChild(0));
				}
				if (InGamePlayManager.BattleRewardTutorial != null)
				{
					Tutorial_Change_Item(chestListDbData_Idx[i]);
				}
				Transform transform = null;
				transform = MWPoolManager.Spawn("Effect", "FX_RewardBox", resultItem_Arr[i], 2f);
				transform.localPosition = Vector3.zero;
				transform.localScale = Vector3.one;
				resultItem_Arr[i].gameObject.SetActive(value: true);
				Transform transform2 = null;
				transform2 = MWPoolManager.Spawn("Item", "Item_" + chestListDbDataList[chestListDbData_Idx[i]].chestItem, resultItem_Arr[i]);
				transform2.SetAsFirstSibling();
				resultItem_Arr[i].GetChild(1).GetComponent<Text>().text = "x" + chestListDbDataList[chestListDbData_Idx[i]].chestItemN;
				transform2.localPosition = Vector3.zero;
				transform2.localScale = Vector3.one;
				transform2.GetComponent<Image>().color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				getItem_Tr = transform2;
				Protocol_Set.Protocol_chest_req_reward_Req(chestListDbDataList[chestListDbData_Idx[i]].probIdx, isPickMore);
				isPickMore = false;
				open_chestlist_idx.Add(chestListDbData_Idx[i]);
				InGamePlayManager.BattleRewardOpenEvent();
				if (InGamePlayManager.BattleRewardTutorial != null)
				{
					InGamePlayManager.BattleRewardTutorialEvent();
				}
			}
			else if (!isOpenItem_Arr[i])
			{
				if (resultItem_Arr[i].childCount > 1)
				{
					resultItem_Arr[i].GetChild(0).GetComponent<Image>().color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					MWPoolManager.DeSpawn("Item", resultItem_Arr[i].GetChild(0));
				}
				resultItem_Arr[i].gameObject.SetActive(value: true);
				Transform transform3 = null;
				transform3 = MWPoolManager.Spawn("Item", "Item_" + chestListDbDataList[chestListDbData_Idx[i]].chestItem, resultItem_Arr[i]);
				transform3.SetAsFirstSibling();
				resultItem_Arr[i].GetChild(1).GetComponent<Text>().text = "x" + chestListDbDataList[chestListDbData_Idx[i]].chestItemN;
				transform3.localPosition = Vector3.zero;
				transform3.localScale = Vector3.one;
				transform3.GetComponent<Image>().color = new Color32(100, 100, 100, byte.MaxValue);
			}
			else
			{
				if (resultItem_Arr[i].childCount > 1)
				{
					MWPoolManager.DeSpawn("Item", resultItem_Arr[i].GetChild(0));
				}
				resultItem_Arr[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void Tutorial_Change_Item(int idx)
	{
		int num = 100;
		for (int i = 0; i < chestListDbDataList.Count; i++)
		{
			if (chestListDbDataList[i].chestItem == 50038)
			{
				num = i;
			}
		}
		if (num != 100)
		{
			ChestListDbData chestListDbData = new ChestListDbData();
			chestListDbData = chestListDbDataList[idx];
			chestListDbDataList[idx] = chestListDbDataList[num];
			chestListDbDataList[num] = chestListDbData;
		}
		else
		{
			ChestListDbData chestListDbData2 = new ChestListDbData();
			chestListDbData2 = chestListDbDataList[idx];
			chestListDbDataList[idx] = chestListDbDataList[2];
			chestListDbDataList[2] = chestListDbData2;
		}
	}

	private void SetAnimation(Transform _tr, string _type)
	{
		if (_type == null)
		{
			return;
		}
		if (!(_type == "idle"))
		{
			if (_type == "open")
			{
				_tr.GetComponent<Animator>().ResetTrigger("open");
				_tr.GetComponent<Animator>().SetTrigger("open");
			}
		}
		else
		{
			_tr.GetComponent<Animator>().ResetTrigger("idle");
			_tr.GetComponent<Animator>().SetTrigger("idle");
		}
	}

	private int[] getShuffleArray(int length, int min, int max)
	{
		int[] array = new int[length];
		int num = 0;
		idxList.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 100;
		}
		for (int j = 0; j < chestListDbData_Idx.Length; j++)
		{
			idxList.Add(chestListDbData_Idx[j]);
		}
		for (int k = 0; k < open_chestlist_idx.Count; k++)
		{
			for (int l = 0; l < chestListDbData_Idx.Length; l++)
			{
				if (chestListDbData_Idx[l] == open_chestlist_idx[k])
				{
					array[l] = open_chestlist_idx[k];
					idxList.Remove(open_chestlist_idx[k]);
				}
			}
		}
		for (int m = 0; m < length; m++)
		{
			if (array[m] == 100)
			{
				num = UnityEngine.Random.Range(0, idxList.Count);
				array[m] = idxList[num];
				idxList.Remove(idxList[num]);
			}
		}
		return array;
	}

	private void CollectItemResponse()
	{
		InGamePlayManager.BattleRewardOpenEvent();
		if (InGamePlayManager.BattleRewardTutorial != null)
		{
			InGamePlayManager.BattleRewardTutorialEvent();
		}
	}

	public void Click_Item_Pick(int _idx)
	{
		if (rewardState == RewardState.None)
		{
			pickCount++;
			rewardState = RewardState.Open;
			isOpenItem_Arr[_idx] = true;
			openIdx = _idx;
			SetAnimation(pickItem_Arr[_idx], "open");
		}
	}

	public void Click_Claim()
	{
		if (rewardState != RewardState.None)
		{
			return;
		}
		SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		for (int i = 0; i < isOpenItem_Arr.Length; i++)
		{
			if (!isOpenItem_Arr[i] && resultItem_Arr[i].childCount > 0)
			{
				resultItem_Arr[i].GetChild(0).GetComponent<Image>().color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				MWPoolManager.DeSpawn("Item", resultItem_Arr[i].GetChild(0));
			}
		}
		for (int j = 0; j < resultItem_Arr.Length; j++)
		{
			for (int k = 0; k < resultItem_Arr[j].childCount; k++)
			{
				resultItem_Arr[j].GetChild(k).gameObject.SetActive(value: false);
			}
		}
		MWPoolManager.DeSpawnPoolAll("Puzzle");
		MWPoolManager.DeSpawnPoolAll("Hunter");
		MWPoolManager.DeSpawnPoolAll("Monster");
		MWPoolManager.DeSpawnPoolAll("Effect");
		MWPoolManager.DeSpawnPoolAll("Stage");
		MWPoolManager.DeSpawnPoolAll("Item");
		GameDataManager.MoveScene(SceneType.Lobby);
	}

	public void Click_PickMore()
	{
		if (rewardState == RewardState.None)
		{
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
			rewardState = RewardState.Ads;
			PickMore_Ads();
		}
	}
}
