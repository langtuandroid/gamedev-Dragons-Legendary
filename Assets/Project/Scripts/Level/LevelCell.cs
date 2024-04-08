using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelCell : MonoBehaviour
{
	public Action SelectLevelCell;

	[SerializeField]
	private Text textLevelIndex;

	[SerializeField]
	private Text textLevelIndexForLock;

	[SerializeField]
	private Text textEnegyCost;

	[SerializeField]
	private Transform trRewardItemAnchor;

	[SerializeField]
	private GameObject goLock;

	[SerializeField]
	private GameObject goBoss;

	[SerializeField]
	private GameObject goBossForLock;

	[SerializeField]
	private GameObject[] arrGoStar = new GameObject[0];

	private bool isLock = true;

	private int userStarCount;

	private string typeKey;

	private TimeCheckState timeState;

	private Transform trRewardItem;

	private LevelDbData levelDbData;

	private UserLevelState userLevelState;

	public int LevelIdx => levelDbData.levelIdx;

	public int StarCount => userStarCount;

	public void SetData(LevelDbData data)
	{
		levelDbData = data;
		Init();
	}

	public void SetUnLock()
	{
		isLock = false;
	}

	public void SetStarCount(int count)
	{
		userStarCount = count;
		goLock.SetActive(count < 0);
		isLock = (count < 0);
		for (int i = 0; i < arrGoStar.Length; i++)
		{
			arrGoStar[i].SetActive(i + 1 <= count);
		}
	}

	public void Refresh()
	{
		userLevelState = GameInfo.userData.GetUserLevelState(levelDbData.stage - 1, levelDbData.chapter - 1, levelDbData.levelIdx);
		if (userLevelState != null)
		{
			trRewardItemAnchor.gameObject.SetActive(value: true);
			trRewardItem = MWPoolManager.Spawn("Item", $"Item_{levelDbData.rewardFixItem}", trRewardItemAnchor);
		}
		else
		{
			trRewardItemAnchor.gameObject.SetActive(value: false);
		}
		SetStarCount(GameDataManager.GetLevelStarCount(levelDbData.stage, levelDbData.chapter, levelDbData.level));
	}

	private void Init()
	{
		textLevelIndex.text = $"{levelDbData.level}";
		textLevelIndexForLock.text = $"{levelDbData.level}";
		textEnegyCost.text = $"{levelDbData.energyCost}";
		textLevelIndex.gameObject.SetActive(levelDbData.specialMark != 1);
		textLevelIndexForLock.gameObject.SetActive(levelDbData.specialMark != 1);
		goBoss.SetActive(levelDbData.specialMark == 1);
		goBossForLock.SetActive(levelDbData.specialMark == 1);
		userLevelState = GameInfo.userData.GetUserLevelState(levelDbData.stage - 1, levelDbData.chapter - 1, levelDbData.levelIdx);
		typeKey = $"Stage_{GameInfo.inGamePlayData.stage}_Level_{levelDbData.level}";
		Transform[] componentsInChildren = trRewardItemAnchor.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (Transform trObj in componentsInChildren)
		{
			MWPoolManager.DeSpawn("Item", trObj);
		}
		trRewardItem = null;
		if (userLevelState != null)
		{
			trRewardItemAnchor.gameObject.SetActive(value: true);
			trRewardItem = MWPoolManager.Spawn("Item", $"Item_{levelDbData.rewardFixItem}", trRewardItemAnchor);
		}
		else
		{
			trRewardItemAnchor.gameObject.SetActive(value: false);
		}
		LocalTimeCheckManager.SaveAndExit(typeKey);
	}

	public void OnClickLevelSelect()
	{
		if (!isLock)
		{
			GameInfo.inGamePlayData.level = levelDbData.level;
			GameInfo.inGamePlayData.levelIdx = levelDbData.levelIdx;
			if (userStarCount == 3)
			{
				LobbyManager.ShowQuickLoot(levelDbData.levelIdx);
			}
			else
			{
				LobbyManager.ShowLevelPlay(levelDbData.levelIdx);
			}
			if (SelectLevelCell != null)
			{
				SelectLevelCell();
			}
			SoundController.EffectSound_Play(EffectSoundType.ButtonClick);
		}
	}

	private void OnDisable()
	{
		if (trRewardItem != null)
		{
			MWPoolManager.DeSpawn("Item", trRewardItem);
			trRewardItem = null;
		}
	}
}
