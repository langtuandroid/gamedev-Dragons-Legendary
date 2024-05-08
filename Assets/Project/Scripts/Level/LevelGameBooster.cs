using UnityEngine;
using UnityEngine.Serialization;

public class LevelGameBooster : MonoBehaviour
{
	[FormerlySerializedAs("boosterItemList")] [SerializeField]
	private Transform _boosterItemList;

	[FormerlySerializedAs("boosterLock")] [SerializeField]
	private Transform _boosterLock;

	private int[] _itemType;

	private LevelGameDbData _levelData;

	public void Construct(int[] _itemType)
	{
		RemoveBooster();
		this._itemType = _itemType;
		this._itemType[0] = _itemType[0];
		this._itemType[1] = _itemType[1];
		this._itemType[2] = _itemType[2];
		if (GameInfo.userData.userStageState[0].chapterList.Length >= 3)
		{
			ConfigureBooster();
		}
		else
		{
			LockBooster();
		}
	}

	public void AddBuster()
	{
		for (int i = 0; i < _boosterItemList.childCount; i++)
		{
			LevelGamePlayBoosterItem component = _boosterItemList.GetChild(i).GetComponent<LevelGamePlayBoosterItem>();
			if (component.IsSelect)
			{
				GameInfo.inGamePlayData.dicActiveBoostItem.Add(component.BoostItemType, component.BoosterData);
			}
		}
	}

	public void BoostCancel()
	{
		for (int i = 0; i < _boosterItemList.childCount; i++)
		{
			LevelGamePlayBoosterItem component = _boosterItemList.GetChild(i).GetComponent<LevelGamePlayBoosterItem>();
			if (component.IsSelect && component.BoosterData.costCount > 0)
			{
				GameDataManager.AddUserJewel(component.BoosterData.costCount);
			}
		}
		GameInfo.inGamePlayData.dicActiveBoostItem.Clear();
	}

	private void RemoveBooster()
	{
		int childCount = _boosterItemList.childCount;
		for (int i = 0; i < childCount; i++)
		{
			MWPoolManager.DeSpawn("Item", _boosterItemList.GetChild(0).transform);
		}
	}

	private void ConfigureBooster()
	{
		LevelGamePlayBoosterItem levelPlay_BoosterItem = null;
		for (int i = 0; i < 3; i++)
		{
			levelPlay_BoosterItem = MWPoolManager.Spawn("Item", "BoosterFrame", _boosterItemList).GetComponent<LevelGamePlayBoosterItem>();
			levelPlay_BoosterItem.Construct(_itemType[i]);
		}
	}

	private void LockBooster()
	{
		_boosterLock.gameObject.SetActive(value: true);
	}
}
