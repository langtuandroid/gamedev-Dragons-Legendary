using System.Collections.Generic;

public class UserPlayData
{
	public int gameKey;

	public int coin;

	public int exp;

	public int wave;

	public int turn;

	public int chestKey;

	public int arenaPoint;

	public List<UserItemData> listUserItem = new List<UserItemData>();

	public List<List<int>> listMonsterClear = new List<List<int>>();

	public void Clear()
	{
		gameKey = 0;
		coin = 0;
		exp = 0;
		wave = 0;
		turn = 0;
		chestKey = 0;
		arenaPoint = 0;
		listUserItem.Clear();
		listMonsterClear.Clear();
	}

	public void AddCoin(int addCoin)
	{
		coin += addCoin;
	}

	public void AddArenaPoint(int _addPoint)
	{
		arenaPoint += _addPoint;
	}

	public void AddChestKey()
	{
		chestKey++;
	}

	public void AddItem(int itemIdx, int count)
	{
		foreach (UserItemData item in listUserItem)
		{
			if (item.itemIdx == itemIdx)
			{
				item.count += count;
				return;
			}
		}
		UserItemData userItemData = new UserItemData();
		userItemData.itemIdx = itemIdx;
		userItemData.count = count;
		listUserItem.Add(userItemData);
	}

	public void AddMonster(int monsterIdx)
	{
		UnityEngine.Debug.Log("AddMonster :: " + monsterIdx);
		if (wave > listMonsterClear.Count)
		{
			listMonsterClear.Add(new List<int>());
		}
		listMonsterClear[wave - 1].Add(monsterIdx);
	}

	public void AddExp(int addExp)
	{
		exp += addExp;
	}
}
