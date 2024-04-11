using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameUtil
{
	public static T[] AddItemToArray<T>(this T[] original, T itemToAdd)
	{
		T[] array = new T[original.Length + 1];
		for (int i = 0; i < original.Length; i++)
		{
			array[i] = original[i];
		}
		array[array.Length - 1] = itemToAdd;
		return array;
	}

	public static string InsertCommaInt(int _num)
	{
		string empty = string.Empty;
		empty = $"{_num:#,###}";
		if (_num == 0)
		{
			empty = "0";
		}
		return empty;
	}

	public static Dictionary<int, int> GetLevelMonsterList(int levelIndex)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		UnityEngine.Debug.Log("GetLevelMonsterList - level index :: " + levelIndex);
		foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum in GameDataManager.GetDicWaveDbData(levelIndex))
		{
			UnityEngine.Debug.Log("GetLevelMonsterList - spawnM1:: " + dicWaveDbDatum.Value.spawnM1);
			UnityEngine.Debug.Log("GetLevelMonsterList - spawnM2:: " + dicWaveDbDatum.Value.spawnM2);
			UnityEngine.Debug.Log("GetLevelMonsterList - spawnM3:: " + dicWaveDbDatum.Value.spawnM3);
			UnityEngine.Debug.Log("GetLevelMonsterList - spawnM4:: " + dicWaveDbDatum.Value.spawnM4);
			int num = 0;
			int num2 = 0;
			if (dicWaveDbDatum.Value.spawnM1 > 0)
			{
				num = GameDataManager.GetMonsterStatData(dicWaveDbDatum.Value.spawnM1).mIdx;
				num2 = GameDataManager.GetMonsterData(num).uiImage;
				if (num > 0)
				{
					if (dictionary.ContainsKey(num2))
					{
						Dictionary<int, int> dictionary2;
						int key;
						(dictionary2 = dictionary)[key = num2] = dictionary2[key] + 1;
					}
					else
					{
						dictionary.Add(num2, 1);
					}
				}
			}
			if (dicWaveDbDatum.Value.spawnM2 > 0)
			{
				num = GameDataManager.GetMonsterStatData(dicWaveDbDatum.Value.spawnM2).mIdx;
				num2 = GameDataManager.GetMonsterData(num).uiImage;
				if (num > 0)
				{
					if (dictionary.ContainsKey(num2))
					{
						Dictionary<int, int> dictionary2;
						int key2;
						(dictionary2 = dictionary)[key2 = num2] = dictionary2[key2] + 1;
					}
					else
					{
						dictionary.Add(num2, 1);
					}
				}
			}
			if (dicWaveDbDatum.Value.spawnM3 > 0)
			{
				num = GameDataManager.GetMonsterStatData(dicWaveDbDatum.Value.spawnM3).mIdx;
				num2 = GameDataManager.GetMonsterData(num).uiImage;
				if (num > 0)
				{
					if (dictionary.ContainsKey(num2))
					{
						Dictionary<int, int> dictionary2;
						int key3;
						(dictionary2 = dictionary)[key3 = num2] = dictionary2[key3] + 1;
					}
					else
					{
						dictionary.Add(num2, 1);
					}
				}
			}
			if (dicWaveDbDatum.Value.spawnM4 > 0)
			{
				num = GameDataManager.GetMonsterStatData(dicWaveDbDatum.Value.spawnM4).mIdx;
				num2 = GameDataManager.GetMonsterData(num).uiImage;
				if (num > 0)
				{
					if (dictionary.ContainsKey(num2))
					{
						Dictionary<int, int> dictionary2;
						int key4;
						(dictionary2 = dictionary)[key4 = num2] = dictionary2[key4] + 1;
					}
					else
					{
						dictionary.Add(num2, 1);
					}
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<int, int> GetLevelItemList(int levelIndex)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (KeyValuePair<int, WaveDbData> dicWaveDbDatum in GameDataManager.GetDicWaveDbData(levelIndex))
		{
			if (dicWaveDbDatum.Value.dropM1 > 0)
			{
				if (dictionary.ContainsKey(dicWaveDbDatum.Value.dropM1))
				{
					Dictionary<int, int> dictionary2;
					int dropM;
					(dictionary2 = dictionary)[dropM = dicWaveDbDatum.Value.dropM1] = dictionary2[dropM] + 1;
				}
				else
				{
					dictionary.Add(dicWaveDbDatum.Value.dropM1, 1);
				}
			}
			if (dicWaveDbDatum.Value.dropM2 > 0)
			{
				if (dictionary.ContainsKey(dicWaveDbDatum.Value.dropM2))
				{
					Dictionary<int, int> dictionary2;
					int dropM2;
					(dictionary2 = dictionary)[dropM2 = dicWaveDbDatum.Value.dropM2] = dictionary2[dropM2] + 1;
				}
				else
				{
					dictionary.Add(dicWaveDbDatum.Value.dropM2, 1);
				}
			}
			if (dicWaveDbDatum.Value.dropM3 > 0)
			{
				if (dictionary.ContainsKey(dicWaveDbDatum.Value.dropM3))
				{
					Dictionary<int, int> dictionary2;
					int dropM3;
					(dictionary2 = dictionary)[dropM3 = dicWaveDbDatum.Value.dropM3] = dictionary2[dropM3] + 1;
				}
				else
				{
					dictionary.Add(dicWaveDbDatum.Value.dropM3, 1);
				}
			}
			if (dicWaveDbDatum.Value.dropM4 > 0)
			{
				if (dictionary.ContainsKey(dicWaveDbDatum.Value.dropM4))
				{
					Dictionary<int, int> dictionary2;
					int dropM4;
					(dictionary2 = dictionary)[dropM4 = dicWaveDbDatum.Value.dropM4] = dictionary2[dropM4] + 1;
				}
				else
				{
					dictionary.Add(dicWaveDbDatum.Value.dropM4, 1);
				}
			}
		}
		dictionary.Add(50032, GameDataManager.GetLevelIndexDbData(levelIndex).getCoinSum);
		return dictionary;
	}

	public static IEnumerator ProcessCountNumber(Text textObj, float start, float end, string add = "", float speed = 13f)
	{
		float gap = Mathf.Abs(start - end);
		float section = gap / speed;
		float current2 = start;
		if (start > end)
		{
			while (current2 - section > end)
			{
				current2 -= section;
				textObj.text = $"{(int)current2}{add}";
				yield return null;
			}
			current2 = end;
		}
		else
		{
			while (current2 + section < end)
			{
				current2 += section;
				textObj.text = $"{(int)current2}{add}";
				yield return null;
			}
			current2 = end;
		}
		textObj.text = $"{(int)current2}{add}";
	}

	public static bool CheckUserInfoItem(int itemIdx)
	{
		switch (itemIdx)
		{
		case 50031:
		case 50032:
		case 50033:
		case 50034:
		case 50040:
			return true;
		default:
			return false;
		}
	}

	public static ChestListDbData_Dummy GetRandomChestListData(ChestType _type)
	{
		ChestListDbData_Dummy chestListDbData_Dummy = null;
		int num = 0;
		int num2 = 0;
		num2 = GameDataManager.GetChestListData((int)_type)[GameDataManager.GetChestListData((int)_type).Count - 1].probability;
		num = Random.Range(0, num2 + 1);
		for (int i = 0; i < GameDataManager.GetChestListData((int)_type).Count; i++)
		{
			if (num < GameDataManager.GetChestListData((int)_type)[i].probability)
			{
				num = ((i <= 0) ? i : (i - 1));
				break;
			}
		}
		return GameDataManager.GetChestListData((int)_type)[num];
	}

	public static int GetConvertCoinToJewel(int coin)
	{
		float f = Mathf.Sqrt(coin) / (float)GameDataManager.GetGameConfigData(ConfigDataType.CoinExchangeFactor) * 10f;
		return (int)Mathf.Ceil(f);
	}

	public static int GetConvertTimeToJewel(float time)
	{
		float f = Mathf.Sqrt(time) / (float)GameDataManager.GetGameConfigData(ConfigDataType.TimeExchangeFactor) * 10f;
		return (int)Mathf.Ceil(f);
	}

	public static int Check_Property_Damage(Hunter _hunter, Monster _monster, int _damage)
	{
		float num = 1.3f;
		float num2 = 0.7f;
		float num3 = 1f;
		int result = 0;
		switch (_hunter.HunterInfo.Hunter.color)
		{
		case 3:
			result = ((_monster.MonsterInfo.mColor != 1) ? ((_monster.MonsterInfo.mColor != 0) ? _damage : ((int)((float)_damage * num2))) : ((int)((float)_damage * num)));
			break;
		case 1:
			result = ((_monster.MonsterInfo.mColor != 0) ? ((_monster.MonsterInfo.mColor != 3) ? _damage : ((int)((float)_damage * num2))) : ((int)((float)_damage * num)));
			break;
		case 0:
			result = ((_monster.MonsterInfo.mColor != 3) ? ((_monster.MonsterInfo.mColor != 1) ? _damage : ((int)((float)_damage * num2))) : ((int)((float)_damage * num)));
			break;
		case 2:
			result = ((_monster.MonsterInfo.mColor != 4) ? _damage : ((int)((float)_damage * num)));
			break;
		case 4:
			result = ((_monster.MonsterInfo.mColor != 2) ? _damage : ((int)((float)_damage * num)));
			break;
		}
		return result;
	}

	public static void Check_Property_Damage_UI(Hunter _hunter, Monster _monster, int _damage)
	{
		switch (_hunter.HunterInfo.Hunter.color)
		{
		case 3:
			if (_monster.MonsterInfo.mColor == 1)
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Critical, _damage, _monster.transform.position);
			}
			else if (_monster.MonsterInfo.mColor == 0)
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Weak, _damage, _monster.transform.position);
			}
			else
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Normal, _damage, _monster.transform.position);
			}
			break;
		case 1:
			if (_monster.MonsterInfo.mColor == 0)
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Critical, _damage, _monster.transform.position);
			}
			else if (_monster.MonsterInfo.mColor == 3)
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Weak, _damage, _monster.transform.position);
			}
			else
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Normal, _damage, _monster.transform.position);
			}
			break;
		case 0:
			if (_monster.MonsterInfo.mColor == 3)
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Critical, _damage, _monster.transform.position);
			}
			else if (_monster.MonsterInfo.mColor == 1)
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Weak, _damage, _monster.transform.position);
			}
			else
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Normal, _damage, _monster.transform.position);
			}
			break;
		case 2:
			if (_monster.MonsterInfo.mColor == 4)
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Critical, _damage, _monster.transform.position);
			}
			else
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Normal, _damage, _monster.transform.position);
			}
			break;
		case 4:
			if (_monster.MonsterInfo.mColor == 2)
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Critical, _damage, _monster.transform.position);
			}
			else
			{
				InGamePlayManager.ShowMonsterDamageUI(MonsterDamageUI.MonsterDamageType.Normal, _damage, _monster.transform.position);
			}
			break;
		}
	}

	public static float GetHunterReinForceHP(int _hp, int _reinForce)
	{
		float num = _hp;
		if (_reinForce == 1)
		{
			return num;
		}
		float num2 = num;
		for (int i = 2; i <= _reinForce; i++)
		{
			num2 = Mathf.Round(num2 + num * 0.3f / (float)(i - 1));
		}
		return num2;
	}

	public static float GetHunterReinForceAttack(int _attack, int _reinForce)
	{
		float num = _attack;
		if (_reinForce == 1)
		{
			return num;
		}
		float num2 = num;
		for (int i = 2; i <= _reinForce; i++)
		{
			num2 = Mathf.Round(num2 + num * 0.2f / (float)(i - 1));
		}
		return num2;
	}

	public static float GetHunterReinForceHeal(int _heal, int _reinForce)
	{
		float num = _heal;
		if (_reinForce == 1)
		{
			return num;
		}
		float num2 = num;
		for (int i = 2; i <= _reinForce; i++)
		{
			num2 = Mathf.Round(num2 + num * 0.1f / (float)(i - 1));
		}
		return num2;
	}

	public static bool GetHunterExist(int hunterIdx)
	{
		bool flag = false;
		for (int i = 0; i < GameInfo.userData.huntersUseInfo.Length; i++)
		{
			if (GameInfo.userData.huntersUseInfo[i].hunterIdx == hunterIdx)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			for (int j = 0; j < GameInfo.userData.huntersOwnInfo.Length; j++)
			{
				if (GameInfo.userData.huntersOwnInfo[j].hunterIdx == hunterIdx)
				{
					flag = true;
					break;
				}
			}
		}
		return flag;
	}

	public static void SetUseHunterList()
	{
		int num = 0;
		int num2 = 0;
		GameInfo.userData.huntersUseInfo = null;
		for (int i = 0; i < GameInfo.userData.userHunterList.Length; i++)
		{
			if (GameInfo.userData.userHunterList[i].useYn.Equals("y"))
			{
				num++;
			}
		}
		GameInfo.userData.huntersUseInfo = new UserHunterData[num];
		for (int j = 0; j < GameInfo.userData.userHunterList.Length; j++)
		{
			if (GameInfo.userData.userHunterList[j].useYn.Equals("y") && GameInfo.userData.userHunterList[j].seq != 0)
			{
				GameInfo.userData.huntersUseInfo[GameInfo.userData.userHunterList[j].seq - 1] = GameInfo.userData.userHunterList[j];
			}
			else if (GameInfo.userData.userHunterList[j].useYn.Equals("y") && GameInfo.userData.userHunterList[j].seq == 0)
			{
				GameInfo.userData.huntersUseInfo[num2] = GameInfo.userData.userHunterList[j];
				num2++;
			}
		}
	}

	public static void SetUseArenaHunterList()
	{
		int num = 0;
		int num2 = 0;
		GameInfo.userData.huntersArenaUseInfo = null;
		for (int i = 0; i < GameInfo.userData.userHunterList.Length; i++)
		{
			if (GameInfo.userData.userHunterList[i].arenaUseYn.Equals("y"))
			{
				num++;
			}
		}
		GameInfo.userData.huntersArenaUseInfo = new UserHunterData[num];
		for (int j = 0; j < GameInfo.userData.userHunterList.Length; j++)
		{
			if (GameInfo.userData.userHunterList[j].arenaUseYn.Equals("y") && GameInfo.userData.userHunterList[j].arenaSeq != 0)
			{
				UnityEngine.Debug.Log("GameInfo.userData.userHunterList[i] 11 = " + GameInfo.userData.userHunterList[j].hunterIdx);
				UnityEngine.Debug.Log("GameInfo.userData.userHunterList[i] 22 = " + GameInfo.userData.userHunterList[j].hunterEnchant);
				GameInfo.userData.huntersArenaUseInfo[GameInfo.userData.userHunterList[j].arenaSeq - 1] = GameInfo.userData.userHunterList[j];
			}
			else if (GameInfo.userData.userHunterList[j].arenaUseYn.Equals("y") && GameInfo.userData.userHunterList[j].arenaSeq == 0)
			{
				UnityEngine.Debug.Log("GameInfo.userData.userHunterList[i] 11 = " + GameInfo.userData.userHunterList[j].hunterIdx);
				UnityEngine.Debug.Log("GameInfo.userData.userHunterList[i] 22 = " + GameInfo.userData.userHunterList[j].hunterEnchant);
				GameInfo.userData.huntersArenaUseInfo[num2] = GameInfo.userData.userHunterList[j];
				num2++;
			}
		}
	}

	public static void SetOwnHunterList(HUNTERLIST_TYPE _type)
	{
		int num = 0;
		int num2 = 0;
		GameInfo.userData.huntersOwnInfo = null;
		switch (_type)
		{
		case HUNTERLIST_TYPE.NORMAL:
			for (int k = 0; k < GameInfo.userData.userHunterList.Length; k++)
			{
				if (GameInfo.userData.userHunterList[k].useYn.Equals("n"))
				{
					num++;
				}
			}
			GameInfo.userData.huntersOwnInfo = new UserHunterData[num];
			for (int l = 0; l < GameInfo.userData.userHunterList.Length; l++)
			{
				if (GameInfo.userData.userHunterList[l].useYn.Equals("n"))
				{
					GameInfo.userData.huntersOwnInfo[num2] = GameInfo.userData.userHunterList[l];
					num2++;
				}
			}
			break;
		case HUNTERLIST_TYPE.ARENA:
			for (int i = 0; i < GameInfo.userData.userHunterList.Length; i++)
			{
				if (GameInfo.userData.userHunterList[i].arenaUseYn.Equals("n"))
				{
					num++;
				}
			}
			GameInfo.userData.huntersOwnInfo = new UserHunterData[num];
			for (int j = 0; j < GameInfo.userData.userHunterList.Length; j++)
			{
				if (GameInfo.userData.userHunterList[j].arenaUseYn.Equals("n"))
				{
					GameInfo.userData.huntersOwnInfo[num2] = GameInfo.userData.userHunterList[j];
					num2++;
				}
			}
			break;
		}
		if (GameInfo.userData.huntersOwnInfo == null)
		{
			GameInfo.userData.huntersOwnInfo = new UserHunterData[0];
		}
	}
}
