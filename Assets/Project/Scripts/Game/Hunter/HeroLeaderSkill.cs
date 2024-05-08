using UnityEngine;

public class HeroLeaderSkill : MonoBehaviour
{
	private int _leaderSkillIdx;

	private HunterLeaderSkillDbData _leaderskillDbData;

	public void SetSkill(int _skillIdx)
	{
		_leaderSkillIdx = _skillIdx;
		_leaderskillDbData = GameDataManager.GetHunterLeaderSkillData(_leaderSkillIdx);
	}

	public HunterInfo CheckLeaderSkillSettings(HunterInfo _hunterInfo)
	{
		if (_leaderSkillIdx == 0)
		{
			return _hunterInfo;
		}
		if (_leaderskillDbData.leaderskillType == 1)
		{
			return SetHeroStatColor(_hunterInfo);
		}
		if (_leaderskillDbData.leaderskillType == 6)
		{
			return SetAtribute(_hunterInfo);
		}
		return _hunterInfo;
	}

	public int CkeckSkillCombo(int _combo, int _damage)
	{
		if (_leaderSkillIdx == 0)
		{
			return _damage;
		}
		if (_leaderskillDbData.leaderskillType != 3)
		{
			return _damage;
		}
		return ChangeDamage(_damage, _combo);
	}

	public int CheckLeaderSkillColor(int _color, int _damage)
	{
		if (_leaderSkillIdx == 0)
		{
			return _damage;
		}
		if (_leaderskillDbData.leaderskillType != 2)
		{
			return _damage;
		}
		return ColorDamage(_damage, _color);
	}

	public void CheckLeaderSkillHeal(Hero[] _hunterList)
	{
		if (_leaderSkillIdx != 0 && _leaderskillDbData.leaderskillType == 4)
		{
			int num = 0;
			for (int i = 0; i < _hunterList.Length; i++)
			{
				num += (int)GameUtil.GetHunterReinForceHeal(_hunterList[i].HeroInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_hunterList[i].HeroInfo.Hunter.hunterIdx));
			}
			num += (int)((float)num * (float)(_leaderskillDbData.leaderSkillIncreaseValue / 100));
			UnityEngine.Debug.Log("**************** heal = " + num);
			PuzzlePlayManager.HealHeroes(num);
		}
	}

	public void CheckLeaderSkillHp1(float prevHp)
	{
		if (_leaderSkillIdx != 0 && _leaderskillDbData.leaderskillType == 5)
		{
			float num = 0f;
			num = 100f * (prevHp / (float)PuzzlePlayManager.GetTotalHp());
			if (num >= (float)_leaderskillDbData.leaderskillRequirement)
			{
				PuzzlePlayManager.HealHeroes(1);
			}
		}
	}

	private HunterInfo SetHeroStatColor(HunterInfo _hunterInfo)
	{
		HunterInfo hunterInfo = null;
		switch (_leaderskillDbData.leaderskillRequirement)
		{
		case 1:
			if (_hunterInfo.Hunter.hunterTribe == 1)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		case 2:
			if (_hunterInfo.Hunter.hunterTribe == 2)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		case 3:
			if (_hunterInfo.Hunter.hunterTribe == 3)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		case 4:
			if (_hunterInfo.Hunter.hunterTribe == 4)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		case 5:
			if (_hunterInfo.Hunter.hunterTribe == 5)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		default:
			return _hunterInfo;
		}
	}

	private HunterInfo SetAtribute(HunterInfo _hunterInfo)
	{
		HunterInfo hunterInfo = null;
		switch (_leaderskillDbData.leaderskillRequirement)
		{
		case 0:
			if (_hunterInfo.Hunter.color == 0)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		case 1:
			if (_hunterInfo.Hunter.color == 1)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		case 2:
			if (_hunterInfo.Hunter.color == 2)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		case 3:
			if (_hunterInfo.Hunter.color == 3)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		case 4:
			if (_hunterInfo.Hunter.color == 4)
			{
				return ChangeHunterInfo(_hunterInfo);
			}
			return _hunterInfo;
		default:
			return _hunterInfo;
		}
	}

	private HunterInfo ChangeHunterInfo(HunterInfo _info)
	{
		switch (_leaderskillDbData.leaderSkillDecreaseStat)
		{
		case "attack":
			_info.leaderSkillAttack = (int)(-1f * ((float)(int)GameUtil.GetHunterReinForceAttack(_info.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)_leaderskillDbData.leaderSkillDecreaseValue / 100f)));
			break;
		case "hp":
			_info.leaderSkillHp = (int)(-1f * ((float)(int)GameUtil.GetHunterReinForceHP(_info.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)_leaderskillDbData.leaderSkillDecreaseValue / 100f)));
			break;
		case "recovery":
			_info.leaderSkillRecovery = (int)(-1f * ((float)(int)GameUtil.GetHunterReinForceHeal(_info.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)_leaderskillDbData.leaderSkillDecreaseValue / 100f)));
			break;
		}
		switch (_leaderskillDbData.leaderSkillIncreaseStat)
		{
		case "attack":
			_info.leaderSkillAttack = (int)((float)(int)GameUtil.GetHunterReinForceAttack(_info.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)_leaderskillDbData.leaderSkillIncreaseValue / 100f));
			break;
		case "hp":
			_info.leaderSkillHp = (int)((float)(int)GameUtil.GetHunterReinForceHP(_info.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)_leaderskillDbData.leaderSkillIncreaseValue / 100f));
			UnityEngine.Debug.Log("_hunterInfo.originHp = " + GameUtil.GetHunterReinForceHP(_info.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)));
			UnityEngine.Debug.Log("_hunterInfo.leaderSkillHp = " + _info.leaderSkillHp);
			break;
		case "recovery":
			_info.leaderSkillRecovery = (int)((float)(int)GameUtil.GetHunterReinForceHeal(_info.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)_leaderskillDbData.leaderSkillIncreaseValue / 100f));
			break;
		}
		return _info;
	}

	private int ChangeDamage(int _damage, int _combo)
	{
		int result = _damage;
		if (_leaderskillDbData.leaderskillRequirement <= _combo)
		{
			result = (int)((float)_damage + (float)_damage * ((float)_leaderskillDbData.leaderSkillIncreaseValue / 100f));
		}
		return result;
	}

	private int ColorDamage(int _damage, int _color)
	{
		int result = _damage;
		if (_leaderskillDbData.leaderskillRequirement <= _color)
		{
			result = (int)((float)_damage + (float)_damage * ((float)_leaderskillDbData.leaderSkillIncreaseValue / 100f));
		}
		return result;
	}
}
