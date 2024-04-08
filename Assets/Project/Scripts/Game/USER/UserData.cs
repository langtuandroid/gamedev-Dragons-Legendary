using System;

[Serializable]
public class UserData
{
	public UserInfoData userInfo = new UserInfoData();

	public UserHunterData[] userHunterList = new UserHunterData[0];

	public UserHunterData[] huntersUseInfo;

	public UserHunterData[] huntersArenaUseInfo;

	public UserHunterData[] huntersOwnInfo;

	public UserItemData[] userItemList;

	public UserStageState[] userStageState;

	public UserFloorStage[] userFloorState = new UserFloorStage[0];

	public SHOP_LIST_RESULT userDailyItemList = new SHOP_LIST_RESULT();

	public string forceCollectYn = "n";

	public int GetItemCount(int itemIdx)
	{
		int result = 0;
		switch (itemIdx)
		{
		case 50032:
			return userInfo.coin;
		case 50034:
			return userInfo.energy;
		case 50040:
			return userInfo.exp;
		case 50031:
			return userInfo.jewel;
		case 50033:
			return userInfo.chestKey;
		default:
		{
			UserItemData[] array = userItemList;
			foreach (UserItemData userItemData in array)
			{
				if (userItemData.itemIdx == itemIdx)
				{
					result = userItemData.count;
				}
			}
			return result;
		}
		}
	}

	public UserLevelState GetUserLevelState(int stage, int chapter, int levelIdx)
	{
		UnityEngine.Debug.Log("GetUserLevelState :: " + stage + " / " + chapter + " / " + levelIdx);
		if (stage > userStageState.Length - 1)
		{
			return null;
		}
		if (chapter > userStageState[stage].chapterList.Length - 1)
		{
			return null;
		}
		UserLevelState[] levelList = userStageState[stage].chapterList[chapter].levelList;
		UserLevelState[] array = levelList;
		foreach (UserLevelState userLevelState in array)
		{
			if (userLevelState.levelIdx == levelIdx)
			{
				return userLevelState;
			}
		}
		return null;
	}
}
