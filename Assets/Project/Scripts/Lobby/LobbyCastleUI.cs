using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCastleUI : MonoBehaviour
{
	[SerializeField]
	private Button btnLeft;

	[SerializeField]
	private Button btnRight;

	[SerializeField]
	private CastleItem[] arrCastleItem = new CastleItem[0];

	private int activeCastleIdx;

	private int maxCastleCount;

	public void Init()
	{
		maxCastleCount = GameInfo.userData.userFloorState.Length;
		RefreshCastle();
	}

	public void MoveCastle(int index)
	{
		activeCastleIdx = index;
		RefreshCastle();
	}

	public bool HasCastleActive(int index)
	{
		return arrCastleItem[index].CastleType == LobbyCastleType.Active || arrCastleItem[index].CastleType == LobbyCastleType.DeActive;
	}

	public void RefreshNotice()
	{
		RefreshCastle();
		for (int i = 0; i < GameInfo.userData.userFloorState.Length; i++)
		{
			if (CheckCastleNotice(i))
			{
				arrCastleItem[i].ShowNotice();
			}
			else
			{
				arrCastleItem[i].ClearNotice();
			}
		}
	}

	public void Exit()
	{
		CastleItem[] array = arrCastleItem;
		foreach (CastleItem castleItem in array)
		{
			castleItem.ClearNotice();
		}
	}

	private void RefreshCastle()
	{
		Dictionary<int, StageDbData> dicStageDbData = GameDataManager.GetDicStageDbData();
		for (int i = 0; i < arrCastleItem.Length; i++)
		{
			CastleItem castleItem = arrCastleItem[i];
			castleItem.SetIndex(i);
			if (activeCastleIdx == i)
			{
				castleItem.SetType(LobbyCastleType.Active);
			}
			else if (dicStageDbData[i + 1].stageLock)
			{
				castleItem.SetType(LobbyCastleType.Lock);
			}
			else if (i <= GameInfo.userData.userFloorState.Length - 1)
			{
				castleItem.SetType(LobbyCastleType.DeActive);
			}
			else
			{
				castleItem.SetType(LobbyCastleType.Close);
			}
		}
	}

	private bool CheckCastleNotice(int _index)
	{
		if (_index > GameInfo.userData.userFloorState.Length - 1)
		{
			return false;
		}
		UserFloorData[] floorList = GameInfo.userData.userFloorState[_index].floorList;
		UserFloorData[] array = floorList;
		foreach (UserFloorData userFloorData in array)
		{
			if (userFloorData.state == 2)
			{
				continue;
			}
			if (userFloorData.state == 3)
			{
				return true;
			}
			if (userFloorData.state == 1)
			{
				StoreProduceDbData storeProduceData = GameDataManager.GetStoreProduceData(userFloorData.storeIdx, userFloorData.storeTier);
				UnityEngine.Debug.Log("CheckCastleNotice :: " + storeProduceData.snip1N + " / " + GameInfo.userData.GetItemCount(storeProduceData.snip1Type));
				if (storeProduceData.snip1N <= GameInfo.userData.GetItemCount(storeProduceData.snip1Type))
				{
					return true;
				}
			}
			StoreUpgradeDbData storeUpgradeData = GameDataManager.GetStoreUpgradeData(userFloorData.storeIdx, userFloorData.storeTier);
			if (storeUpgradeData != null && GameInfo.userData.userInfo.coin >= storeUpgradeData.needCoin && (storeUpgradeData.sniu1 <= 0 || GameInfo.userData.GetItemCount(storeUpgradeData.sniu1) >= storeUpgradeData.sniu1_N) && (storeUpgradeData.sniu2 <= 0 || GameInfo.userData.GetItemCount(storeUpgradeData.sniu2) >= storeUpgradeData.sniu2_N) && (storeUpgradeData.sniu3 <= 0 || GameInfo.userData.GetItemCount(storeUpgradeData.sniu3) >= storeUpgradeData.sniu3_N) && (storeUpgradeData.sniu4 <= 0 || GameInfo.userData.GetItemCount(storeUpgradeData.sniu4) >= storeUpgradeData.sniu4_N))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckStoreNotice()
	{
		UserFloorStage[] userFloorState = GameInfo.userData.userFloorState;
		UserFloorStage[] array = userFloorState;
		foreach (UserFloorStage userFloorStage in array)
		{
			UserFloorData[] floorList = userFloorStage.floorList;
			foreach (UserFloorData userFloorData in floorList)
			{
				if (userFloorData.state == 3)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void OnLeftCastle()
	{
		if (activeCastleIdx > 0)
		{
			activeCastleIdx--;
			LobbyManager.MoveCastle(activeCastleIdx);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	public void OnRightCastle()
	{
		if (activeCastleIdx < maxCastleCount - 1)
		{
			activeCastleIdx++;
			LobbyManager.MoveCastle(activeCastleIdx);
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}
}
