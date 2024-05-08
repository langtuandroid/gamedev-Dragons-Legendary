using System.Collections.Generic;

public class PuzzlePlayData
{
	public PuzzleInGameType inGameType;

	public bool isShowScenario;

	public int stage = 1;

	public int chapter = 1;

	public int level = 1;

	public int levelIdx;

	public int star2ClearTurn;

	public int star3ClearTurn;

	public int matchTime = 5;

	public int matchTimeBonus = 1;

	public int matchTimeRatio;

	public int puzzleR;

	public int puzzleY;

	public int puzzleG;

	public int puzzleB;

	public int puzzleP;

	public int puzzleH;

	public int isDragon;

	public Dictionary<int, WaveDbData> dicWaveDbData = new Dictionary<int, WaveDbData>();

	public Dictionary<int, List<MonsterStatDbData>> dicMonsterStatData = new Dictionary<int, List<MonsterStatDbData>>();

	public Dictionary<int, HunterInfo> dicHunterInfo = new Dictionary<int, HunterInfo>();

	public Dictionary<int, BoostItemDbData> dicActiveBoostItem = new Dictionary<int, BoostItemDbData>();

	public ARENA_INFO arenaInfo;

	public ARENA_LEVEL_DATA arenaLevelData;
}
