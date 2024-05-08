using System.Collections.Generic;
using UnityEngine;

public class Pallete : GameObjectSingleton<Pallete>
{
	public enum LevelBlockType
	{
		LV0,
		LV1,
		LV2,
		LV3,
		LV4,
		LV5,
		LV6
	}

	[SerializeField]
	private List<BlockPattern> listBlockPattern = new List<BlockPattern>();

	private List<int> listBlockRate = new List<int>();

	private int[,] iArrLV0 = new int[5, 7]
	{
		{
			2,
			3,
			0,
			3,
			4,
			1,
			0
		},
		{
			3,
			4,
			0,
			0,
			1,
			1,
			3
		},
		{
			1,
			3,
			2,
			2,
			3,
			2,
			1
		},
		{
			0,
			0,
			1,
			0,
			4,
			4,
			2
		},
		{
			3,
			1,
			1,
			3,
			3,
			1,
			2
		}
	};

	private int[,] iArrLV1 = new int[7, 7]
	{
		{
			5,
			1,
			0,
			5,
			1,
			5,
			3
		},
		{
			1,
			0,
			3,
			3,
			1,
			5,
			3
		},
		{
			3,
			1,
			1,
			0,
			5,
			1,
			0
		},
		{
			5,
			0,
			3,
			1,
			3,
			1,
			5
		},
		{
			3,
			1,
			3,
			5,
			0,
			0,
			3
		},
		{
			3,
			1,
			0,
			5,
			3,
			3,
			0
		},
		{
			5,
			0,
			1,
			3,
			5,
			5,
			3
		}
	};

	private int[,] iArrLV2 = new int[7, 7]
	{
		{
			0,
			5,
			0,
			1,
			1,
			5,
			1
		},
		{
			5,
			3,
			5,
			0,
			0,
			3,
			0
		},
		{
			0,
			3,
			0,
			5,
			0,
			1,
			0
		},
		{
			0,
			1,
			0,
			0,
			5,
			1,
			5
		},
		{
			3,
			5,
			3,
			3,
			0,
			5,
			0
		},
		{
			1,
			1,
			5,
			1,
			5,
			1,
			1
		},
		{
			3,
			3,
			5,
			3,
			5,
			3,
			3
		}
	};

	private int[,] iArrLV3 = new int[7, 7]
	{
		{
			1,
			3,
			3,
			5,
			1,
			5,
			1
		},
		{
			1,
			0,
			3,
			5,
			0,
			5,
			0
		},
		{
			3,
			1,
			5,
			0,
			1,
			3,
			5
		},
		{
			5,
			0,
			0,
			5,
			3,
			5,
			1
		},
		{
			1,
			0,
			3,
			5,
			1,
			5,
			1
		},
		{
			3,
			1,
			5,
			0,
			3,
			1,
			5
		},
		{
			0,
			3,
			1,
			5,
			0,
			3,
			1
		}
	};

	private int[,] iArrLV4 = new int[7, 7]
	{
		{
			1,
			0,
			5,
			1,
			5,
			0,
			1
		},
		{
			0,
			3,
			1,
			0,
			1,
			3,
			0
		},
		{
			3,
			1,
			5,
			3,
			5,
			0,
			3
		},
		{
			3,
			1,
			3,
			0,
			3,
			1,
			3
		},
		{
			5,
			5,
			0,
			1,
			0,
			5,
			5
		},
		{
			0,
			3,
			1,
			5,
			1,
			0,
			3
		},
		{
			3,
			1,
			5,
			1,
			5,
			1,
			0
		}
	};

	private int[,] iArrLV5 = new int[7, 7]
	{
		{
			4,
			1,
			0,
			4,
			3,
			4,
			5
		},
		{
			4,
			1,
			5,
			0,
			4,
			1,
			5
		},
		{
			1,
			4,
			0,
			4,
			5,
			3,
			4
		},
		{
			4,
			1,
			5,
			0,
			3,
			1,
			5
		},
		{
			4,
			1,
			0,
			3,
			4,
			3,
			5
		},
		{
			5,
			0,
			5,
			0,
			5,
			0,
			5
		},
		{
			4,
			4,
			1,
			4,
			4,
			1,
			4
		}
	};

	private int[,] iArrLV6 = new int[7, 7]
	{
		{
			0,
			5,
			3,
			1,
			0,
			4,
			3
		},
		{
			5,
			4,
			0,
			5,
			1,
			0,
			5
		},
		{
			5,
			1,
			3,
			4,
			0,
			1,
			0
		},
		{
			1,
			0,
			4,
			3,
			5,
			5,
			3
		},
		{
			0,
			1,
			4,
			5,
			1,
			3,
			5
		},
		{
			4,
			0,
			5,
			3,
			5,
			5,
			3
		},
		{
			4,
			1,
			1,
			5,
			1,
			3,
			4
		}
	};

	public static void Init()
	{
		foreach (BlockPattern item in GameObjectSingleton<Pallete>.Inst.listBlockPattern)
		{
			if (item.type == BlockType.White)
			{
				item.isActive = true;
			}
			else
			{
				item.isActive = (PuzzlePlayManager.HunterPos(item.type) != null);
			}
		}
		GameObjectSingleton<Pallete>.Inst.SetBlockRate();
	}

	public static void Active(BlockType _type)
	{
		foreach (BlockPattern item in GameObjectSingleton<Pallete>.Inst.listBlockPattern)
		{
			if (item.type == _type)
			{
				item.isActive = true;
			}
		}
	}

	public static void DeActive(BlockType _type)
	{
		foreach (BlockPattern item in GameObjectSingleton<Pallete>.Inst.listBlockPattern)
		{
			if (item.type == _type)
			{
				item.isActive = false;
			}
		}
	}

	public static int[,] GetLevelBlock(LevelBlockType type)
	{
		switch (type)
		{
		case LevelBlockType.LV0:
			return GameObjectSingleton<Pallete>.Inst.iArrLV0;
		case LevelBlockType.LV1:
			return GameObjectSingleton<Pallete>.Inst.iArrLV1;
		case LevelBlockType.LV2:
			return GameObjectSingleton<Pallete>.Inst.iArrLV2;
		case LevelBlockType.LV3:
			return GameObjectSingleton<Pallete>.Inst.iArrLV3;
		case LevelBlockType.LV4:
			return GameObjectSingleton<Pallete>.Inst.iArrLV4;
		case LevelBlockType.LV5:
			return GameObjectSingleton<Pallete>.Inst.iArrLV5;
		case LevelBlockType.LV6:
			return GameObjectSingleton<Pallete>.Inst.iArrLV6;
		default:
			return null;
		}
	}

	public static BlockPattern GetPattern(BlockType type)
	{
		foreach (BlockPattern item in GameObjectSingleton<Pallete>.Inst.listBlockPattern)
		{
			if (item.type == type)
			{
				return item;
			}
		}
		return null;
	}

	public static BlockPattern GetRandomPattern()
	{
		int index = GameObjectSingleton<Pallete>.Inst.GetRandomBlock();
		bool flag = true;
		flag = GameObjectSingleton<Pallete>.Inst.CheckEnablePattern(GameObjectSingleton<Pallete>.Inst.listBlockPattern[index].type);
		while (!flag)
		{
			index = Random.Range(0, GameObjectSingleton<Pallete>.Inst.listBlockPattern.Count);
			flag = GameObjectSingleton<Pallete>.Inst.CheckEnablePattern(GameObjectSingleton<Pallete>.Inst.listBlockPattern[index].type);
		}
		return GameObjectSingleton<Pallete>.Inst.listBlockPattern[index];
	}

	private bool CheckEnablePattern(BlockType _type)
	{
		if (_type == BlockType.White)
		{
			return true;
		}
		if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
		{
			if (GameInfo.userData.huntersUseInfo.Length >= 5)
			{
				return true;
			}
			UserHunterData[] huntersUseInfo = GameInfo.userData.huntersUseInfo;
			foreach (UserHunterData userHunterData in huntersUseInfo)
			{
				if (_type == (BlockType)GameDataManager.GetHunterList()[userHunterData.hunterIdx].color)
				{
					return true;
				}
			}
		}
		else
		{
			if (GameInfo.userData.huntersArenaUseInfo.Length >= 5)
			{
				return true;
			}
			UserHunterData[] huntersArenaUseInfo = GameInfo.userData.huntersArenaUseInfo;
			foreach (UserHunterData userHunterData2 in huntersArenaUseInfo)
			{
				if (_type == (BlockType)GameDataManager.GetHunterList()[userHunterData2.hunterIdx].color)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static string ConvertBlockEffect(BlockType type)
	{
		switch (type)
		{
		case BlockType.Blue:
			return "Fx_Puzzle_blue_attack";
		case BlockType.Green:
			return "Fx_Puzzle_green_attack";
		case BlockType.Purple:
			return "Fx_Puzzle_purple_attack";
		case BlockType.Red:
			return "Fx_Puzzle_red_attack";
		case BlockType.Yellow:
			return "Fx_Puzzle_yellow_attack";
		default:
			return string.Empty;
		}
	}

	public static string GetSpecialStanbyName(BlockType type)
	{
		switch (type)
		{
		case BlockType.Blue:
			return "FX_special_block_blue";
		case BlockType.Green:
			return "FX_special_block_green";
		case BlockType.Purple:
			return "FX_special_block_purple";
		case BlockType.Red:
			return "FX_special_block_red";
		case BlockType.Yellow:
			return "FX_special_block_yellow";
		default:
			return string.Empty;
		}
	}

	private void SetBlockRate()
	{
		listBlockRate.Clear();
		listBlockRate.Add(GameInfo.inGamePlayData.puzzleB);
		listBlockRate.Add(GameInfo.inGamePlayData.puzzleB + GameInfo.inGamePlayData.puzzleG);
		listBlockRate.Add(GameInfo.inGamePlayData.puzzleB + GameInfo.inGamePlayData.puzzleG + GameInfo.inGamePlayData.puzzleP);
		listBlockRate.Add(GameInfo.inGamePlayData.puzzleB + GameInfo.inGamePlayData.puzzleG + GameInfo.inGamePlayData.puzzleP + GameInfo.inGamePlayData.puzzleR);
		listBlockRate.Add(GameInfo.inGamePlayData.puzzleB + GameInfo.inGamePlayData.puzzleG + GameInfo.inGamePlayData.puzzleP + GameInfo.inGamePlayData.puzzleR + GameInfo.inGamePlayData.puzzleY);
		listBlockRate.Add(GameInfo.inGamePlayData.puzzleB + GameInfo.inGamePlayData.puzzleG + GameInfo.inGamePlayData.puzzleP + GameInfo.inGamePlayData.puzzleR + GameInfo.inGamePlayData.puzzleY + GameInfo.inGamePlayData.puzzleH);
	}

	private int GetRandomBlock()
	{
		int num = Random.Range(0, 100);
		for (int i = 0; i < listBlockRate.Count; i++)
		{
			if (num < listBlockRate[i])
			{
				return i;
			}
		}
		return 0;
	}
}
