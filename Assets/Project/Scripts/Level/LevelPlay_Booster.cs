using UnityEngine;

public class LevelPlay_Booster : MonoBehaviour
{
	[SerializeField]
	private Transform boosterItemList;

	[SerializeField]
	private Transform boosterLock;

	private int[] itemType;

	private LevelDbData levelData;

	public void Init(int[] _itemType)
	{
		DespawnBoosterItem();
		itemType = _itemType;
		itemType[0] = _itemType[0];
		itemType[1] = _itemType[1];
		itemType[2] = _itemType[2];
		if (GameInfo.userData.userStageState[0].chapterList.Length >= 3)
		{
			SetBoosterItem();
		}
		else
		{
			SetBoosterLock();
		}
	}

	public void AddBoostItem()
	{
		for (int i = 0; i < boosterItemList.childCount; i++)
		{
			LevelPlay_BoosterItem component = boosterItemList.GetChild(i).GetComponent<LevelPlay_BoosterItem>();
			if (component.IsSelect)
			{
				GameInfo.inGamePlayData.dicActiveBoostItem.Add(component.BoostItemType, component.BoosterData);
			}
		}
	}

	public void BoostItemCancel()
	{
		for (int i = 0; i < boosterItemList.childCount; i++)
		{
			LevelPlay_BoosterItem component = boosterItemList.GetChild(i).GetComponent<LevelPlay_BoosterItem>();
			if (component.IsSelect && component.BoosterData.costCount > 0)
			{
				GameDataManager.AddUserJewel(component.BoosterData.costCount);
			}
		}
		GameInfo.inGamePlayData.dicActiveBoostItem.Clear();
	}

	private void DespawnBoosterItem()
	{
		int childCount = boosterItemList.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", boosterItemList.GetChild(0).transform);
		}
	}

	private void SetBoosterItem()
	{
		LevelPlay_BoosterItem levelPlay_BoosterItem = null;
		for (int i = 0; i < 3; i++)
		{
			levelPlay_BoosterItem = MWPoolManager.Spawn("Item", "BoosterFrame", boosterItemList).GetComponent<LevelPlay_BoosterItem>();
			levelPlay_BoosterItem.Init(itemType[i]);
		}
	}

	private void SetBoosterLock()
	{
		boosterLock.gameObject.SetActive(value: true);
	}
}
