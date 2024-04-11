using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LocalDB : GameObjectSingleton<LocalDB>
{
	private const string LOCAL_GAME_DATA_PATH = "Data.db";

	private IDbCommand dbcmd;

	private Dictionary<ConfigDataType, int> dicGameConfigDbData = new Dictionary<ConfigDataType, int>();

	private Dictionary<int, UserLevelDbData> dicUserLevelDbData = new Dictionary<int, UserLevelDbData>();

	private Dictionary<int, StageDbData> dicStageDbData = new Dictionary<int, StageDbData>();

	private Dictionary<int, Dictionary<int, ChapterDbData>> dicChapterDbData = new Dictionary<int, Dictionary<int, ChapterDbData>>();

	private Dictionary<int, Dictionary<int, List<LevelDbData>>> dicLevelDbData = new Dictionary<int, Dictionary<int, List<LevelDbData>>>();

	private Dictionary<int, LevelDbData> dicLevelIndexDbData = new Dictionary<int, LevelDbData>();

	private Dictionary<int, Dictionary<int, WaveDbData>> dicWaveDbData = new Dictionary<int, Dictionary<int, WaveDbData>>();

	private Dictionary<int, HunterDbData> dicHunterDbData = new Dictionary<int, HunterDbData>();

	private Dictionary<int, HunterColorDbData> dicHunterColorDbData = new Dictionary<int, HunterColorDbData>();

	private Dictionary<int, string> dicHunterTribeDbData = new Dictionary<int, string>();

	private Dictionary<int, HunterSkillDbData> dicHunterSkillDbData = new Dictionary<int, HunterSkillDbData>();

	private Dictionary<int, HunterLeaderSkillDbData> dicHunterLeaderSkillDbData = new Dictionary<int, HunterLeaderSkillDbData>();

	private Dictionary<string, Dictionary<int, HunterLevelDbData>> dicHunterLevelDbData = new Dictionary<string, Dictionary<int, HunterLevelDbData>>();

	private List<HunterPromotionDbData> listHunterPromotionDbData = new List<HunterPromotionDbData>();

	private Dictionary<int, MonsterDbData> dicMonsterDbData = new Dictionary<int, MonsterDbData>();

	private Dictionary<int, MonsterStatDbData> dicMonsterStatDbData = new Dictionary<int, MonsterStatDbData>();

	private Dictionary<int, MonsterSkillDbData> dicMonsterSkillDbData = new Dictionary<int, MonsterSkillDbData>();

	private Dictionary<int, StoreDbData> dicStoreDbData = new Dictionary<int, StoreDbData>();

	private Dictionary<int, Dictionary<int, StoreProduceDbData>> dicStoreProduceDbData = new Dictionary<int, Dictionary<int, StoreProduceDbData>>();

	private Dictionary<int, Dictionary<int, StoreUpgradeDbData>> dicStoreUpgradeDbData = new Dictionary<int, Dictionary<int, StoreUpgradeDbData>>();

	private Dictionary<int, ItemListDbData> dicItemListDbData = new Dictionary<int, ItemListDbData>();

	private Dictionary<int, ItemTypeDbData> dicItemTypeDbData = new Dictionary<int, ItemTypeDbData>();

	private Dictionary<int, ChestDbData> dicChestDbData = new Dictionary<int, ChestDbData>();

	private Dictionary<int, List<ChestListDbData_Dummy>> dicChestListDbData = new Dictionary<int, List<ChestListDbData_Dummy>>();

	private Dictionary<int, ShopDailyDbData> dicShopDailyDbData = new Dictionary<int, ShopDailyDbData>();

	private Dictionary<int, ShopCoinDbData> dicShopCoinDbData = new Dictionary<int, ShopCoinDbData>();

	private Dictionary<int, ShopJewelDbData> dicShopJewelDbData = new Dictionary<int, ShopJewelDbData>();

	private Dictionary<int, BoostItemDbData> dicBoostItemDbData = new Dictionary<int, BoostItemDbData>();

	private Dictionary<int, Dictionary<int, TutorialDbData>> dicTutorialDbData = new Dictionary<int, Dictionary<int, TutorialDbData>>();

	private Dictionary<int, Dictionary<int, ScenarioDbData>> dicScenarioDbData = new Dictionary<int, Dictionary<int, ScenarioDbData>>();

	private Dictionary<int, Dictionary<int, ScenarioDbData>> dicScenarioInGameDbData = new Dictionary<int, Dictionary<int, ScenarioDbData>>();

	public Dictionary<int, StageDbData> DicStageDbData => dicStageDbData;

	public void CallLocalDbData(System.Action onCompleted)
	{
		ReadGameLocalDb(onCompleted);
	}

	public int GetGameConfigData(ConfigDataType type)
	{
		return dicGameConfigDbData[type];
	}

	public Dictionary<int, ChapterDbData> GetChapterDataList(int stageId)
	{
		return dicChapterDbData[stageId];
	}

	public List<LevelDbData> GetLevelDbList(int stageId, int chapterId)
	{
		return dicLevelDbData[stageId][chapterId];
	}

	public LevelDbData GetLevelIndexDbData(int levelIdx)
	{
		return dicLevelIndexDbData[levelIdx];
	}

	public Dictionary<int, LevelDbData> GetDicLevelIndexDbData()
	{
		return dicLevelIndexDbData;
	}

	public Dictionary<int, WaveDbData> GetWaveDbData(int levelIndex)
	{
		return dicWaveDbData[levelIndex];
	}

	public MonsterDbData GetMonsterData(int mIdx)
	{
		return dicMonsterDbData[mIdx];
	}

	public MonsterStatDbData GetMonsterStatData(int levelIndex)
	{
		if (!dicMonsterStatDbData.ContainsKey(levelIndex))
		{
			UnityEngine.Debug.LogError("GetMonsterStatData - Key :: " + levelIndex);
		}
		return dicMonsterStatDbData[levelIndex];
	}

	public MonsterSkillDbData GetMonsterSkillData(int skillIdx)
	{
		return dicMonsterSkillDbData[skillIdx];
	}

	public HunterDbData GetHunterData(int hunterIdx)
	{
		return dicHunterDbData[hunterIdx];
	}

	public HunterLevelDbData GetHunterLevelData(int hunterIdx, int hunterLevel, int hunterTier)
	{
		return dicHunterLevelDbData[$"{hunterIdx}_{hunterTier}"][hunterLevel];
	}

	public HunterSkillDbData GetHunterSkillData(int skillIndex)
	{
		return dicHunterSkillDbData[skillIndex];
	}

	public HunterLeaderSkillDbData GetHunterLeaderSkillData(int leaderskillIndex)
	{
		UnityEngine.Debug.Log("leaderskillIndex = " + leaderskillIndex);
		return dicHunterLeaderSkillDbData[leaderskillIndex];
	}

	public Dictionary<int, HunterDbData> GetHunterList()
	{
		return dicHunterDbData;
	}

	public HunterPromotionDbData GetHunterPromotionData(int hunterColor, int hunterMaxTier, int hunterTier)
	{
		foreach (HunterPromotionDbData listHunterPromotionDbDatum in listHunterPromotionDbData)
		{
			if (hunterColor == listHunterPromotionDbDatum.hunterColor && hunterMaxTier == listHunterPromotionDbDatum.hunterMaxTier && hunterTier == listHunterPromotionDbDatum.hunterTier)
			{
				return listHunterPromotionDbDatum;
			}
		}
		return null;
	}

	public string GetHunterTribeData(int hunterAlly)
	{
		return dicHunterTribeDbData[hunterAlly];
	}

	public HunterColorDbData GetHunterColorData(int hunterColorIdx)
	{
		return dicHunterColorDbData[hunterColorIdx];
	}

	public UserLevelDbData GetUserLevelData(int userLevel)
	{
		if (dicUserLevelDbData.ContainsKey(userLevel))
		{
			return dicUserLevelDbData[userLevel];
		}
		return null;
	}

	public List<StoreDbData> GetStoreListForStage(int stage)
	{
		List<StoreDbData> list = new List<StoreDbData>();
		foreach (KeyValuePair<int, StoreDbData> dicStoreDbDatum in dicStoreDbData)
		{
			if (dicStoreDbDatum.Value.storeCaslte == stage)
			{
				list.Add(dicStoreDbDatum.Value);
			}
		}
		return list;
	}

	public StoreDbData GetStoreData(int storeIdx)
	{
		return dicStoreDbData[storeIdx];
	}

	public StoreProduceDbData GetStoreProduceData(int storeIdx, int storeTier)
	{
		if (dicStoreProduceDbData.ContainsKey(storeIdx) && dicStoreProduceDbData[storeIdx].ContainsKey(storeTier))
		{
			return dicStoreProduceDbData[storeIdx][storeTier];
		}
		return null;
	}

	public StoreUpgradeDbData GetStoreUpgradeData(int storeIdx, int storeTier)
	{
		if (!dicStoreUpgradeDbData[storeIdx].ContainsKey(storeTier))
		{
			return null;
		}
		return dicStoreUpgradeDbData[storeIdx][storeTier];
	}

	public ItemListDbData GetItemListData(int itemIdx)
	{
		return dicItemListDbData[itemIdx];
	}

	public int GetChapterCount(int stageIdx)
	{
		return dicLevelDbData[stageIdx].Count;
	}

	public int GetLevelCount(int stageIdx, int chapterIdx)
	{
		return dicLevelDbData[stageIdx][chapterIdx].Count;
	}

	public Dictionary<int, ChestDbData> GetChestData()
	{
		return dicChestDbData;
	}

	public List<ChestListDbData_Dummy> GetChestListData(int chestIdx)
	{
		return dicChestListDbData[chestIdx];
	}

	public Dictionary<int, ShopDailyDbData> GetShopDailyData()
	{
		return dicShopDailyDbData;
	}

	public Dictionary<int, ShopCoinDbData> GetShopCoinData()
	{
		return dicShopCoinDbData;
	}

	public Dictionary<int, ShopJewelDbData> GetShopJewelData()
	{
		return dicShopJewelDbData;
	}

	public Dictionary<int, Dictionary<int, TutorialDbData>> GetDicTutorialData()
	{
		return dicTutorialDbData;
	}

	public Dictionary<int, Dictionary<int, ScenarioDbData>> GetDicScenarioDbData()
	{
		return dicScenarioDbData;
	}

	public Dictionary<int, Dictionary<int, ScenarioDbData>> GetDicScenarioInGameDbData()
	{
		return dicScenarioInGameDbData;
	}

	public BoostItemDbData GetBoostItemData(int key)
	{
		return dicBoostItemDbData[key];
	}

	public int GetStageCount()
	{
		int num = 0;
		foreach (KeyValuePair<int, StageDbData> dicStageDbDatum in dicStageDbData)
		{
			if (!dicStageDbDatum.Value.stageLock)
			{
				num++;
			}
		}
		return num;
	}

	private void ReadGameLocalDb(System.Action onCompleted)
	{
		StartCoroutine(IEReadGameLocalDb(onCompleted));
	}

	IEnumerator IEReadGameLocalDb(System.Action onCompleted)
	{
		string DatabaseName = "Data.db";
		string platformPath = GetPlatformPath(DatabaseName);
        Debug.Log("@LOG ReadGameLocalDb platformPath:" + platformPath);
		if(!File.Exists(platformPath))
		{
#if UNITY_EDITOR
			UnityWebRequest wWW = UnityWebRequest.Get("file://" + Application.streamingAssetsPath + "/" + DatabaseName);
			wWW.SendWebRequest();
#elif UNITY_ANDROID
            UnityWebRequest wWW = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);
            wWW.SendWebRequest();
#elif UNITY_IOS
			UnityWebRequest wWW = UnityWebRequest.Get("file://" + Application.dataPath + "/Raw/" + DatabaseName);
            wWW.SendWebRequest();
#else
			UnityWebRequest wWW = UnityWebRequest.Get("file://" + Application.streamingAssetsPath + "/" + DatabaseName);
			wWW.SendWebRequest();
#endif
			yield return wWW;

			File.WriteAllBytes(platformPath, wWW.downloadHandler.data);
			yield return new WaitForSeconds(0.25f);
		}
        IDbConnection dbConnection = new SqliteConnection("URI=file:" + platformPath);
        dbConnection.Open();
        dbcmd = dbConnection.CreateCommand();
        ReadGameConfigData();
        ReadUserLevelData();
        ReadStageData();
        ReadChapterData();
        ReadLevelData();
        ReadWaveData();
        ReadHunterData();
        ReadHunterColorData();
        ReadHunterTribeData();
        ReadHunterSkillData();
        ReadHunterLeaderSkillData();
        ReadHunterStatData();
        ReadHunterEvolveData();
        ReadMonsterData();
        ReadMonsterStatData();
        ReadMonsterSkillData();
        ReadStoreData();
        ReadStoreProduceData();
        ReadStoreUpgradeData();
        ReadItemListData();
        ReadChestData();
        ReadChestListData();
        ReadShopDailyData();
        ReadShopCoinData();
        ReadShopJewelData();
        ReadBoostItemDbData();
        ReadSceanrioDbData();
        ReadSceanrioInGameDbData();
        ReadTutorialData();
        ReadLocalizeDBdata();
        dbcmd.Dispose();
        dbcmd = null;
        dbConnection.Close();
        dbConnection = null;
        yield return null;
		onCompleted?.Invoke();

	}

    private void ReadGameConfigData()
	{
		string commandText = string.Format("SELECT * From {0}", "Config");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicGameConfigDbData.Clear();
		int num = 0;
		while (dataReader.Read())
		{
			dicGameConfigDbData.Add((ConfigDataType)num, dataReader.GetInt32(1));
			num++;
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - listUserLevelData :: " + dicGameConfigDbData.Count);
	}

	private void ReadUserLevelData()
	{
		string commandText = string.Format("SELECT * From {0}", "User");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicUserLevelDbData.Clear();
		while (dataReader.Read())
		{
			UserLevelDbData userLevelDbData = new UserLevelDbData();
			userLevelDbData.level = dataReader.GetInt32(0);
			userLevelDbData.exp = dataReader.GetInt32(1);
			userLevelDbData.maxEnergy = dataReader.GetInt32(2);
			userLevelDbData.getEnergy = dataReader.GetBoolean(3);
			userLevelDbData.attackBonusAll = dataReader.GetFloat(4);
			dicUserLevelDbData.Add(userLevelDbData.level, userLevelDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - listUserLevelData :: " + dicUserLevelDbData.Count);
	}

	private void ReadStageData()
	{
		string commandText = string.Format("SELECT * From {0}", "D_Stage");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicStageDbData.Clear();
		while (dataReader.Read())
		{
			StageDbData stageDbData = new StageDbData();
			stageDbData.stageIdx = dataReader.GetInt32(0);
			stageDbData.stageName = dataReader.GetString(1);
			stageDbData.stageLock = dataReader.GetBoolean(2);
			stageDbData.castleName = dataReader.GetString(3);
			dicStageDbData.Add(stageDbData.stageIdx, stageDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - listStageDbData :: " + dicStageDbData.Count);
	}

	private void ReadChapterData()
	{
		string commandText = string.Format("SELECT * From {0}", "D_Chapter");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicChapterDbData.Clear();
		while (dataReader.Read())
		{
			ChapterDbData chapterDbData = new ChapterDbData();
			chapterDbData.stage = dataReader.GetInt32(0);
			chapterDbData.chapter = dataReader.GetInt32(1);
			chapterDbData.chapterName = dataReader.GetString(2);
			chapterDbData.ocStar = dataReader.GetInt32(3);
			chapterDbData.rewardType = dataReader.GetInt32(4);
			chapterDbData.rewardCount = dataReader.GetInt32(5);
			chapterDbData.storeIdx = dataReader.GetInt32(6);
			if (!dicChapterDbData.ContainsKey(chapterDbData.stage))
			{
				dicChapterDbData.Add(chapterDbData.stage, new Dictionary<int, ChapterDbData>());
			}
			dicChapterDbData[chapterDbData.stage].Add(chapterDbData.chapter, chapterDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - listChapterDbData :: " + dicChapterDbData.Count);
	}

	private void ReadLevelData()
	{
		string commandText = string.Format("SELECT * From {0}", "D_Level");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicLevelDbData.Clear();
		while (dataReader.Read())
		{
			LevelDbData levelDbData = new LevelDbData();
			levelDbData.levelIdx = dataReader.GetInt32(0);
			levelDbData.stage = dataReader.GetInt32(1);
			levelDbData.chapter = dataReader.GetInt32(2);
			levelDbData.level = dataReader.GetInt32(3);
			levelDbData.specialMark = dataReader.GetInt32(4);
			levelDbData.time = dataReader.GetInt32(5);
			levelDbData.waveN = dataReader.GetInt32(6);
			levelDbData.energyCost = dataReader.GetInt32(7);
			levelDbData.levelCoolTime = dataReader.GetInt32(8);
			levelDbData.getExp = dataReader.GetInt32(9);
			levelDbData.getCoinSum = dataReader.GetInt32(10);
			levelDbData.getChest = dataReader.GetInt32(11);
			levelDbData.star2 = dataReader.GetInt32(12);
			levelDbData.star3 = dataReader.GetInt32(13);
			levelDbData.rewardFixItem = dataReader.GetInt32(14);
			levelDbData.rewardFixCount = dataReader.GetInt32(15);
			levelDbData.monsterCount = dataReader.GetInt32(16);
			levelDbData.puzzleR = dataReader.GetInt32(17);
			levelDbData.puzzleY = dataReader.GetInt32(18);
			levelDbData.puzzleG = dataReader.GetInt32(19);
			levelDbData.puzzleB = dataReader.GetInt32(20);
			levelDbData.puzzleP = dataReader.GetInt32(21);
			levelDbData.puzzleH = dataReader.GetInt32(22);
			levelDbData.isDragon = dataReader.GetInt32(23);
			levelDbData.boosterItem_1 = dataReader.GetInt32(24);
			levelDbData.boosterItem_2 = dataReader.GetInt32(25);
			levelDbData.boosterItem_3 = dataReader.GetInt32(26);
			if (!dicLevelDbData.ContainsKey(levelDbData.stage))
			{
				dicLevelDbData.Add(levelDbData.stage, new Dictionary<int, List<LevelDbData>>());
			}
			if (!dicLevelDbData[levelDbData.stage].ContainsKey(levelDbData.chapter))
			{
				dicLevelDbData[levelDbData.stage].Add(levelDbData.chapter, new List<LevelDbData>());
			}
			dicLevelDbData[levelDbData.stage][levelDbData.chapter].Add(levelDbData);
			dicLevelIndexDbData.Add(levelDbData.levelIdx, levelDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - listLevelDbData :: " + dicLevelDbData.Count);
	}

	private void ReadWaveData()
	{
		string commandText = string.Format("SELECT * From {0}", "D_Wave");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicWaveDbData.Clear();
		while (dataReader.Read())
		{
			WaveDbData waveDbData = new WaveDbData();
			waveDbData.waveIdx = dataReader.GetInt32(0);
			waveDbData.levelIndex = dataReader.GetInt32(1);
			waveDbData.wave = dataReader.GetInt32(2);
			waveDbData.mFormation = dataReader.GetString(3);
			waveDbData.spawnM1 = dataReader.GetInt32(4);
			waveDbData.spawnM2 = dataReader.GetInt32(5);
			waveDbData.spawnM3 = dataReader.GetInt32(6);
			waveDbData.spawnM4 = dataReader.GetInt32(7);
			waveDbData.atM1 = dataReader.GetInt32(8);
			waveDbData.atM2 = dataReader.GetInt32(9);
			waveDbData.atM3 = dataReader.GetInt32(10);
			waveDbData.atM4 = dataReader.GetInt32(11);
			waveDbData.dropM1 = dataReader.GetInt32(12);
			waveDbData.dropM1Min = dataReader.GetInt32(13);
			waveDbData.dropM1Max = dataReader.GetInt32(14);
			waveDbData.dropM2 = dataReader.GetInt32(15);
			waveDbData.dropM2Min = dataReader.GetInt32(16);
			waveDbData.dropM2Max = dataReader.GetInt32(17);
			waveDbData.dropM3 = dataReader.GetInt32(18);
			waveDbData.dropM3Min = dataReader.GetInt32(19);
			waveDbData.dropM3Max = dataReader.GetInt32(20);
			waveDbData.dropM4 = dataReader.GetInt32(21);
			waveDbData.dropM4Min = dataReader.GetInt32(22);
			waveDbData.dropM4Max = dataReader.GetInt32(23);
			waveDbData.getCoin = dataReader.GetInt32(24);
			waveDbData.isWarning = dataReader.GetInt32(25);
			if (!dicWaveDbData.ContainsKey(waveDbData.levelIndex))
			{
				dicWaveDbData.Add(waveDbData.levelIndex, new Dictionary<int, WaveDbData>());
			}
			dicWaveDbData[waveDbData.levelIndex].Add(waveDbData.wave, waveDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - listWaveDbData :: " + dicWaveDbData.Count);
	}

	private void ReadHunterData()
	{
		string commandText = string.Format("SELECT * From {0}", "Hunter");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicHunterDbData.Clear();
		while (dataReader.Read())
		{
			HunterDbData hunterDbData = new HunterDbData();
			hunterDbData.hunterIdx = dataReader.GetInt32(0);
			hunterDbData.hunterName = dataReader.GetString(1);
			hunterDbData.color = dataReader.GetInt32(2);
			hunterDbData.hunterTribe = dataReader.GetInt32(3);
			hunterDbData.maxTier = dataReader.GetInt32(4);
			hunterDbData.skillIdx = dataReader.GetInt32(5);
			hunterDbData.openSpec = dataReader.GetBoolean(6);
			hunterDbData.hunterClass = dataReader.GetString(7);
			hunterDbData.hunterImg1 = dataReader.GetString(8);
			hunterDbData.hunterImg2 = dataReader.GetString(9);
			hunterDbData.hunterImg3 = dataReader.GetString(10);
			hunterDbData.hunterImg4 = dataReader.GetString(11);
			hunterDbData.hunterImg5 = dataReader.GetString(12);
			hunterDbData.hunterSize = dataReader.GetInt32(13);
			dicHunterDbData.Add(hunterDbData.hunterIdx, hunterDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicHunterDbData :: " + dicHunterDbData.Count);
	}

	private void ReadHunterColorData()
	{
		string commandText = string.Format("SELECT * From {0}", "Hunter_color");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicHunterColorDbData.Clear();
		while (dataReader.Read())
		{
			HunterColorDbData hunterColorDbData = new HunterColorDbData();
			hunterColorDbData.color = dataReader.GetInt32(0);
			hunterColorDbData.colorName = dataReader.GetString(1);
			hunterColorDbData.colorOccupation = dataReader.GetString(2);
			dicHunterColorDbData.Add(hunterColorDbData.color, hunterColorDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicHunterColorDbData :: " + dicHunterColorDbData.Count);
	}

	private void ReadHunterTribeData()
	{
		string commandText = string.Format("SELECT * From {0}", "Hunter_tribe");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicHunterTribeDbData.Clear();
		while (dataReader.Read())
		{
			dicHunterTribeDbData.Add(dataReader.GetInt32(0), dataReader.GetString(1));
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicHunterAllyDbData :: " + dicHunterTribeDbData.Count);
	}

	private void ReadHunterSkillData()
	{
		string commandText = string.Format("SELECT * From {0}", "Hunter_skill");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicHunterSkillDbData.Clear();
		while (dataReader.Read())
		{
			HunterSkillDbData hunterSkillDbData = new HunterSkillDbData();
			hunterSkillDbData.skillIdx = dataReader.GetInt32(0);
			hunterSkillDbData.skillName = dataReader.GetString(1);
			hunterSkillDbData.skillDescription = dataReader.GetString(2);
			hunterSkillDbData.skillType = dataReader.GetInt32(3);
			hunterSkillDbData.range = dataReader.GetInt32(4);
			hunterSkillDbData.statType = dataReader.GetInt32(5);
			hunterSkillDbData.multiple = dataReader.GetFloat(6);
			hunterSkillDbData.times = dataReader.GetInt32(7);
			hunterSkillDbData.recPowers = dataReader.GetFloat(8);
			hunterSkillDbData.skillGauge = dataReader.GetInt32(9);
			hunterSkillDbData.beforeBlock = dataReader.GetInt32(10);
			hunterSkillDbData.afterBlock = dataReader.GetInt32(11);
			hunterSkillDbData.motionType = dataReader.GetInt32(12);
			dicHunterSkillDbData.Add(hunterSkillDbData.skillIdx, hunterSkillDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicHunterSkillDbData :: " + dicHunterSkillDbData.Count);
	}

	private void ReadHunterLeaderSkillData()
	{
		string commandText = string.Format("SELECT * From {0}", "Hunter_leaderskill");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicHunterLeaderSkillDbData.Clear();
		while (dataReader.Read())
		{
			HunterLeaderSkillDbData hunterLeaderSkillDbData = new HunterLeaderSkillDbData();
			hunterLeaderSkillDbData.leaderSkillIdx = dataReader.GetInt32(0);
			hunterLeaderSkillDbData.leaderSkillName = dataReader.GetString(1);
			hunterLeaderSkillDbData.leaderskillType = dataReader.GetInt32(2);
			hunterLeaderSkillDbData.leaderskillRequirement = dataReader.GetInt32(3);
			hunterLeaderSkillDbData.leaderSkillDecreaseStat = dataReader.GetString(4);
			hunterLeaderSkillDbData.leaderSkillDecreaseValue = dataReader.GetInt32(5);
			hunterLeaderSkillDbData.leaderSkillIncreaseStat = dataReader.GetString(6);
			hunterLeaderSkillDbData.leaderSkillIncreaseValue = dataReader.GetInt32(7);
			hunterLeaderSkillDbData.leaderSkillDescription = dataReader.GetString(8);
			dicHunterLeaderSkillDbData.Add(hunterLeaderSkillDbData.leaderSkillIdx, hunterLeaderSkillDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicHunterLeaderSkillDbData :: " + dicHunterLeaderSkillDbData.Count);
	}

	private void ReadHunterStatData()
	{
		string commandText = string.Format("SELECT * From {0}", "Hunter_level");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicHunterLevelDbData.Clear();
		while (dataReader.Read())
		{
			HunterLevelDbData hunterLevelDbData = new HunterLevelDbData();
			hunterLevelDbData.hunterIdx = dataReader.GetInt32(0);
			hunterLevelDbData.hunterTier = dataReader.GetInt32(1);
			hunterLevelDbData.hunterLevel = dataReader.GetInt32(2);
			hunterLevelDbData.hnil = dataReader.GetInt32(3);
			hunterLevelDbData.hnil_N = dataReader.GetInt32(4);
			hunterLevelDbData.needCoin = dataReader.GetInt32(5);
			hunterLevelDbData.hunterLeaderSkill = dataReader.GetInt32(6);
			hunterLevelDbData.hunterIncHp = dataReader.GetInt32(7);
			hunterLevelDbData.hunterHp = dataReader.GetInt32(8);
			hunterLevelDbData.hunterIncAttack = dataReader.GetInt32(9);
			hunterLevelDbData.hunterAttack = dataReader.GetInt32(10);
			hunterLevelDbData.hunterIncRecovery = dataReader.GetInt32(11);
			hunterLevelDbData.hunterRecovery = dataReader.GetInt32(12);
			if (!dicHunterLevelDbData.ContainsKey($"{hunterLevelDbData.hunterIdx}_{hunterLevelDbData.hunterTier}"))
			{
				dicHunterLevelDbData.Add($"{hunterLevelDbData.hunterIdx}_{hunterLevelDbData.hunterTier}", new Dictionary<int, HunterLevelDbData>());
			}
			dicHunterLevelDbData[$"{hunterLevelDbData.hunterIdx}_{hunterLevelDbData.hunterTier}"].Add(hunterLevelDbData.hunterLevel, hunterLevelDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicHunterStatDbData :: " + dicHunterLevelDbData.Count);
	}

	private void ReadHunterEvolveData()
	{
		string commandText = string.Format("SELECT * From {0}", "Hunter_promotion");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		listHunterPromotionDbData.Clear();
		while (dataReader.Read())
		{
			HunterPromotionDbData hunterPromotionDbData = new HunterPromotionDbData();
			hunterPromotionDbData.hunterColor = dataReader.GetInt32(0);
			hunterPromotionDbData.hunterMaxTier = dataReader.GetInt32(1);
			hunterPromotionDbData.hunterTier = dataReader.GetInt32(2);
			hunterPromotionDbData.hnip1 = dataReader.GetInt32(3);
			hunterPromotionDbData.hnip1_N = dataReader.GetInt32(4);
			hunterPromotionDbData.hnip2 = dataReader.GetInt32(5);
			hunterPromotionDbData.hnip2_N = dataReader.GetInt32(6);
			hunterPromotionDbData.hnip3 = dataReader.GetInt32(7);
			hunterPromotionDbData.hnip3_N = dataReader.GetInt32(8);
			hunterPromotionDbData.hnip4 = dataReader.GetInt32(9);
			hunterPromotionDbData.hnip4_N = dataReader.GetInt32(10);
			hunterPromotionDbData.needCoin = dataReader.GetInt32(11);
			hunterPromotionDbData.getExp = dataReader.GetInt32(12);
			listHunterPromotionDbData.Add(hunterPromotionDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - listHunterEvolveDbData :: " + listHunterPromotionDbData.Count);
	}

	private void ReadMonsterData()
	{
		string commandText = string.Format("SELECT * From {0}", "Monster");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicMonsterDbData.Clear();
		while (dataReader.Read())
		{
			MonsterDbData monsterDbData = new MonsterDbData();
			monsterDbData.mIdx = dataReader.GetInt32(0);
			monsterDbData.mName = dataReader.GetString(1);
			monsterDbData.mColor = dataReader.GetInt32(2);
			monsterDbData.itemIdx = dataReader.GetInt32(3);
			monsterDbData.uiImage = dataReader.GetInt32(4);
			monsterDbData.motionType = dataReader.GetInt32(5);
			dicMonsterDbData.Add(monsterDbData.mIdx, monsterDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicMonsterDbData :: " + dicMonsterDbData.Count);
	}

	private void ReadMonsterStatData()
	{
		string commandText = string.Format("SELECT * From {0}", "Monster_stat");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicMonsterStatDbData.Clear();
		while (dataReader.Read())
		{
			MonsterStatDbData monsterStatDbData = new MonsterStatDbData();
			monsterStatDbData.mLevelIdx = dataReader.GetInt32(0);
			monsterStatDbData.mIdx = dataReader.GetInt32(1);
			monsterStatDbData.mLevel = dataReader.GetInt32(2);
			monsterStatDbData.mHp = dataReader.GetInt32(3);
			monsterStatDbData.mTurnsAttack = dataReader.GetInt32(4);
			monsterStatDbData.mDamageAttack = dataReader.GetInt32(5);
			monsterStatDbData.mSkillIdx = dataReader.GetInt32(6);
			monsterStatDbData.mMonsterIdx = dataReader.GetInt32(7);
			monsterStatDbData.mMonsterImg = dataReader.GetString(8);
			UnityEngine.Debug.Log("************** 11 :: " + monsterStatDbData.mMonsterIdx);
			UnityEngine.Debug.Log("************** 22 :: " + monsterStatDbData.mMonsterImg);
			dicMonsterStatDbData.Add(monsterStatDbData.mLevelIdx, monsterStatDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicMonsterStatDbData :: " + dicMonsterStatDbData.Count);
	}

	private void ReadMonsterSkillData()
	{
		string commandText = string.Format("SELECT * From {0}", "Monster_skill");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicMonsterSkillDbData.Clear();
		while (dataReader.Read())
		{
			MonsterSkillDbData monsterSkillDbData = new MonsterSkillDbData();
			monsterSkillDbData.mSkillIdx = dataReader.GetInt32(0);
			monsterSkillDbData.mSkillAttackMagnification = dataReader.GetInt32(1);
			monsterSkillDbData.mSkillType = dataReader.GetInt32(2);
			monsterSkillDbData.mSkillRatio = dataReader.GetInt32(3);
			monsterSkillDbData.mSkillInterval = dataReader.GetInt32(4);
			monsterSkillDbData.mSkillName = dataReader.GetString(5);
			dicMonsterSkillDbData.Add(monsterSkillDbData.mSkillIdx, monsterSkillDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicMonsterSkillDbData :: " + dicMonsterSkillDbData.Count);
	}

	private void ReadStoreData()
	{
		string commandText = string.Format("SELECT * From {0}", "Store");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicStoreDbData.Clear();
		while (dataReader.Read())
		{
			StoreDbData storeDbData = new StoreDbData();
			storeDbData.storeIdx = dataReader.GetInt32(0);
			storeDbData.storeName = dataReader.GetString(1);
			storeDbData.storeColor = dataReader.GetInt32(2);
			storeDbData.spi = dataReader.GetInt32(3);
			storeDbData.ocStage = dataReader.GetInt32(4);
			storeDbData.ocChapter = dataReader.GetInt32(5);
			storeDbData.storeCaslte = dataReader.GetInt32(6);
			storeDbData.storeOrder = dataReader.GetInt32(7);
			dicStoreDbData.Add(storeDbData.storeIdx, storeDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicStoreDbData :: " + dicStoreDbData.Count);
	}

	private void ReadStoreProduceData()
	{
		string commandText = string.Format("SELECT * From {0}", "Store_produce");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicStoreProduceDbData.Clear();
		while (dataReader.Read())
		{
			StoreProduceDbData storeProduceDbData = new StoreProduceDbData();
			storeProduceDbData.storeIdx = dataReader.GetInt32(0);
			storeProduceDbData.storeTier = dataReader.GetInt32(1);
			storeProduceDbData.snip1Type = dataReader.GetInt32(2);
			storeProduceDbData.snip1N = dataReader.GetInt32(3);
			storeProduceDbData.produceTime = dataReader.GetInt32(4);
			storeProduceDbData.getExp = dataReader.GetInt32(5);
			storeProduceDbData.getCoin = dataReader.GetInt32(6);
			storeProduceDbData.spi = dataReader.GetInt32(7);
			storeProduceDbData.spiN = dataReader.GetInt32(8);
			if (!dicStoreProduceDbData.ContainsKey(storeProduceDbData.storeIdx))
			{
				dicStoreProduceDbData.Add(storeProduceDbData.storeIdx, new Dictionary<int, StoreProduceDbData>());
			}
			dicStoreProduceDbData[storeProduceDbData.storeIdx].Add(storeProduceDbData.storeTier, storeProduceDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicStoreProduceDbData :: " + dicStoreProduceDbData.Count);
	}

	private void ReadStoreUpgradeData()
	{
		string commandText = string.Format("SELECT * From {0}", "Store_upgrade");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicStoreUpgradeDbData.Clear();
		while (dataReader.Read())
		{
			StoreUpgradeDbData storeUpgradeDbData = new StoreUpgradeDbData();
			storeUpgradeDbData.storeIdx = dataReader.GetInt32(0);
			storeUpgradeDbData.storeTier = dataReader.GetInt32(1);
			storeUpgradeDbData.sniu1 = dataReader.GetInt32(2);
			storeUpgradeDbData.sniu2 = dataReader.GetInt32(3);
			storeUpgradeDbData.sniu3 = dataReader.GetInt32(4);
			storeUpgradeDbData.sniu4 = dataReader.GetInt32(5);
			storeUpgradeDbData.sniu1_N = dataReader.GetInt32(6);
			storeUpgradeDbData.sniu2_N = dataReader.GetInt32(7);
			storeUpgradeDbData.sniu3_N = dataReader.GetInt32(8);
			storeUpgradeDbData.sniu4_N = dataReader.GetInt32(9);
			storeUpgradeDbData.needCoin = dataReader.GetInt32(10);
			storeUpgradeDbData.getExp = dataReader.GetInt32(11);
			storeUpgradeDbData.getItem = dataReader.GetInt32(12);
			storeUpgradeDbData.getItem_N = dataReader.GetInt32(13);
			if (!dicStoreUpgradeDbData.ContainsKey(storeUpgradeDbData.storeIdx))
			{
				dicStoreUpgradeDbData.Add(storeUpgradeDbData.storeIdx, new Dictionary<int, StoreUpgradeDbData>());
			}
			dicStoreUpgradeDbData[storeUpgradeDbData.storeIdx].Add(storeUpgradeDbData.storeTier, storeUpgradeDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicStoreEvolveDbData :: " + dicStoreUpgradeDbData.Count);
	}

	private void ReadItemListData()
	{
		string commandText = string.Format("SELECT * From {0}", "Item");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicItemListDbData.Clear();
		while (dataReader.Read())
		{
			ItemListDbData itemListDbData = new ItemListDbData();
			itemListDbData.itemIdx = dataReader.GetInt32(0);
			itemListDbData.itemName = dataReader.GetString(1);
			itemListDbData.itemRarity = dataReader.GetInt32(2);
			itemListDbData.itemType = dataReader.GetString(3);
			dicItemListDbData.Add(itemListDbData.itemIdx, itemListDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicItemListDbData :: " + dicItemListDbData.Count);
	}

	private void ReadChestData()
	{
		string commandText = string.Format("SELECT * From {0}", "Chest");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicChestDbData.Clear();
		while (dataReader.Read())
		{
			ChestDbData chestDbData = new ChestDbData();
			chestDbData.chestIdx = dataReader.GetInt32(0);
			chestDbData.chestName = dataReader.GetString(1);
			chestDbData.needItem = dataReader.GetInt32(2);
			chestDbData.needItemNx1 = dataReader.GetInt32(3);
			chestDbData.needItemNx5 = dataReader.GetInt32(4);
			chestDbData.pickTimes = dataReader.GetInt32(5);
			dicChestDbData.Add(chestDbData.chestIdx, chestDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicChestDbData :: " + dicChestDbData.Count);
	}

	private void ReadChestListData()
	{
		string commandText = string.Format("SELECT * From {0}", "Chest_list");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicChestListDbData.Clear();
		while (dataReader.Read())
		{
			ChestListDbData_Dummy chestListDbData_Dummy = new ChestListDbData_Dummy();
			chestListDbData_Dummy.chestIdx = dataReader.GetInt32(0);
			chestListDbData_Dummy.chestHunter = dataReader.GetInt32(1);
			chestListDbData_Dummy.hunterTier = dataReader.GetInt32(2);
			chestListDbData_Dummy.hunterLevel = dataReader.GetInt32(3);
			chestListDbData_Dummy.chestItem = dataReader.GetInt32(4);
			chestListDbData_Dummy.chestItemN = dataReader.GetInt32(5);
			chestListDbData_Dummy.probability = dataReader.GetInt32(6);
			chestListDbData_Dummy.itemRarity = dataReader.GetInt32(7);
			if (!dicChestListDbData.ContainsKey(chestListDbData_Dummy.chestIdx))
			{
				dicChestListDbData.Add(chestListDbData_Dummy.chestIdx, new List<ChestListDbData_Dummy>());
			}
			dicChestListDbData[chestListDbData_Dummy.chestIdx].Add(chestListDbData_Dummy);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - chestListDbData :: " + dicChestListDbData.Count);
	}

	private void ReadShopDailyData()
	{
		string commandText = string.Format("SELECT * From {0}", "Shop_daily");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicShopDailyDbData.Clear();
		while (dataReader.Read())
		{
			ShopDailyDbData shopDailyDbData = new ShopDailyDbData();
			shopDailyDbData.productIdx = dataReader.GetInt32(0);
			shopDailyDbData.itemIdx = dataReader.GetInt32(1);
			shopDailyDbData.itemName = dataReader.GetString(2);
			shopDailyDbData.coinPrice = dataReader.GetInt32(4);
			shopDailyDbData.priceIncrease = dataReader.GetInt32(5);
			dicShopDailyDbData.Add(shopDailyDbData.productIdx, shopDailyDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicShopDailyDbData :: " + dicShopDailyDbData.Count);
	}

	private void ReadShopCoinData()
	{
		string commandText = string.Format("SELECT * From {0}", "Shop_coin");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicShopCoinDbData.Clear();
		while (dataReader.Read())
		{
			ShopCoinDbData shopCoinDbData = new ShopCoinDbData();
			shopCoinDbData.productIdx = dataReader.GetInt32(0);
			shopCoinDbData.itemIdx = dataReader.GetInt32(1);
			shopCoinDbData.coinNumber = dataReader.GetInt32(2);
			shopCoinDbData.jewelPrice = dataReader.GetInt32(3);
			dicShopCoinDbData.Add(shopCoinDbData.productIdx, shopCoinDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicShopCoinDbData :: " + dicShopCoinDbData.Count);
	}

	private void ReadShopJewelData()
	{
		string commandText = string.Format("SELECT * From {0}", "Shop_jewel");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicShopJewelDbData.Clear();
		while (dataReader.Read())
		{
			ShopJewelDbData shopJewelDbData = new ShopJewelDbData();
			shopJewelDbData.productIdx = dataReader.GetInt32(0);
			shopJewelDbData.name = dataReader.GetString(1);
			shopJewelDbData.price = dataReader.GetFloat(2);
			shopJewelDbData.priceIdx = dataReader.GetInt32(3);
			shopJewelDbData.itemIdx = dataReader.GetInt32(4);
			shopJewelDbData.jewel = dataReader.GetInt32(5);
			shopJewelDbData.desc = dataReader.GetString(6);
			dicShopJewelDbData.Add(shopJewelDbData.productIdx, shopJewelDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicShopJewelDbData :: " + dicShopJewelDbData.Count);
	}

	private void ReadBoostItemDbData()
	{
		string commandText = string.Format("SELECT * From {0}", "booster");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicShopJewelDbData.Clear();
		while (dataReader.Read())
		{
			BoostItemDbData boostItemDbData = new BoostItemDbData();
			boostItemDbData.boosterIdx = dataReader.GetInt32(0);
			boostItemDbData.boosterName = dataReader.GetString(1);
			boostItemDbData.boosterType = dataReader.GetInt32(2);
			boostItemDbData.costType = dataReader.GetInt32(3);
			boostItemDbData.costCount = dataReader.GetInt32(4);
			boostItemDbData.boosterExplain = dataReader.GetString(5);
			dicBoostItemDbData.Add(boostItemDbData.boosterIdx, boostItemDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicBoostItemDbData :: " + dicBoostItemDbData.Count);
	}

	private void ReadSceanrioDbData()
	{
		string commandText = string.Format("SELECT * From {0}", "scenario");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicScenarioDbData.Clear();
		while (dataReader.Read())
		{
			ScenarioDbData scenarioDbData = new ScenarioDbData();
			scenarioDbData.scenarioIdx = dataReader.GetInt32(0);
			scenarioDbData.scenario = dataReader.GetInt32(1);
			scenarioDbData.seq = dataReader.GetInt32(2);
			scenarioDbData.type = dataReader.GetInt32(3);
			scenarioDbData.characterName = dataReader.GetString(4);
			scenarioDbData.characterLocation = dataReader.GetInt32(5);
			scenarioDbData.text = dataReader.GetString(6);
			scenarioDbData.bg = dataReader.GetString(7);
			scenarioDbData.messageLocation = dataReader.GetInt32(8);
			scenarioDbData.feeling = dataReader.GetInt32(9);
			scenarioDbData.effect = dataReader.GetInt32(10);
			if (!dicScenarioDbData.ContainsKey(scenarioDbData.scenario))
			{
				dicScenarioDbData.Add(scenarioDbData.scenario, new Dictionary<int, ScenarioDbData>());
			}
			dicScenarioDbData[scenarioDbData.scenario].Add(scenarioDbData.seq, scenarioDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicScenarioDbData :: " + dicScenarioDbData.Count);
	}

	private void ReadSceanrioInGameDbData()
	{
		string commandText = string.Format("SELECT * From {0}", "scenario_ingame");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicScenarioInGameDbData.Clear();
		while (dataReader.Read())
		{
			ScenarioDbData scenarioDbData = new ScenarioDbData();
			scenarioDbData.scenarioIdx = dataReader.GetInt32(0);
			scenarioDbData.scenario = dataReader.GetInt32(1);
			scenarioDbData.seq = dataReader.GetInt32(2);
			scenarioDbData.type = dataReader.GetInt32(3);
			scenarioDbData.characterName = dataReader.GetString(4);
			scenarioDbData.characterLocation = dataReader.GetInt32(5);
			scenarioDbData.text = dataReader.GetString(6);
			scenarioDbData.bg = dataReader.GetString(7);
			scenarioDbData.messageLocation = dataReader.GetInt32(8);
			scenarioDbData.feeling = dataReader.GetInt32(9);
			scenarioDbData.effect = dataReader.GetInt32(10);
			if (!dicScenarioInGameDbData.ContainsKey(scenarioDbData.scenario))
			{
				dicScenarioInGameDbData.Add(scenarioDbData.scenario, new Dictionary<int, ScenarioDbData>());
			}
			dicScenarioInGameDbData[scenarioDbData.scenario].Add(scenarioDbData.seq, scenarioDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicScenarioDbData :: " + dicScenarioDbData.Count);
	}

	private void ReadTutorialData()
	{
		string commandText = string.Format("SELECT * From {0}", "Tutorial");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		dicTutorialDbData.Clear();
		while (dataReader.Read())
		{
			TutorialDbData tutorialDbData = new TutorialDbData();
			tutorialDbData.sidex = dataReader.GetInt32(0);
			tutorialDbData.seq = dataReader.GetInt32(1);
			tutorialDbData.type = dataReader.GetString(2);
			tutorialDbData.scene = dataReader.GetString(3);
			tutorialDbData.background = dataReader.GetInt32(4);
			tutorialDbData.descPosition = dataReader.GetInt32(5);
			tutorialDbData.characterImg = dataReader.GetString(6);
			tutorialDbData.TutoMessage = dataReader.GetString(7);
			if (!dicTutorialDbData.ContainsKey(tutorialDbData.sidex))
			{
				dicTutorialDbData.Add(tutorialDbData.sidex, new Dictionary<int, TutorialDbData>());
			}
			dicTutorialDbData[tutorialDbData.sidex].Add(tutorialDbData.seq, tutorialDbData);
		}
		dataReader.Close();
		dataReader = null;
		UnityEngine.Debug.Log("LocalDb Read - dicTutorialDbData :: " + dicTutorialDbData.Count);
	}

	private void ReadLocalizeDBdata()
	{
		string commandText = string.Format("SELECT * From {0}", "Localize");
		dbcmd.CommandText = commandText;
		IDataReader dataReader = dbcmd.ExecuteReader();
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		while (dataReader.Read())
		{
			List<string> list = new List<string>();
			list.Add(dataReader.GetString(1));
			list.Add(dataReader.GetString(2));
			list.Add(dataReader.GetString(3));
			UnityEngine.Debug.Log("ReadLocalizeDBdata - key :: " + dataReader.GetString(0));
			dictionary.Add(dataReader.GetString(0), list);
		}
		dataReader.Close();
		dataReader = null;
		MWLocalize.SetDicData(dictionary);
		UnityEngine.Debug.Log("LocalDb Read - dicLocalizeData :: " + dictionary.Count);
	}

	private string GetPlatformPath(string DatabaseName)
	{
		string text = $"{Application.persistentDataPath}/{DatabaseName}";
		return text;
	}
}
