using UnityEngine;

public class HunterLeaderSkill : MonoBehaviour
{
	private int leaderSkillIdx;

	private HunterLeaderSkillDbData leaderskillDbData;

	public void SetLeaderSkill(int _skillIdx)
	{
		leaderSkillIdx = _skillIdx;
		leaderskillDbData = GameDataManager.GetHunterLeaderSkillData(leaderSkillIdx);
	}

	public HunterInfo CheckLeaderSkillStatSetting(HunterInfo _hunterInfo)
	{
		if (leaderSkillIdx == 0)
		{
			return _hunterInfo;
		}
		if (leaderskillDbData.leaderskillType == 1)
		{
			return SetHunterStat_Color(_hunterInfo);
		}
		if (leaderskillDbData.leaderskillType == 6)
		{
			return SetHunterStat_Attribute(_hunterInfo);
		}
		return _hunterInfo;
	}

	public int CheckLeaderSkillComboSetting(int _combo, int _damage)
	{
		if (leaderSkillIdx == 0)
		{
			return _damage;
		}
		if (leaderskillDbData.leaderskillType != 3)
		{
			return _damage;
		}
		return ChangeComboDamage(_damage, _combo);
	}

	public int CheckLeaderSkillColorSetting(int _color, int _damage)
	{
		if (leaderSkillIdx == 0)
		{
			return _damage;
		}
		if (leaderskillDbData.leaderskillType != 2)
		{
			return _damage;
		}
		return ChangeColorDamage(_damage, _color);
	}

	public void CheckLeaderSkillHealSetting(Hunter[] _hunterList)
	{
		if (leaderSkillIdx != 0 && leaderskillDbData.leaderskillType == 4)
		{
			int num = 0;
			for (int i = 0; i < _hunterList.Length; i++)
			{
				num += (int)GameUtil.GetHunterReinForceHeal(_hunterList[i].HunterInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_hunterList[i].HunterInfo.Hunter.hunterIdx));
			}
			num += (int)((float)num * (float)(leaderskillDbData.leaderSkillIncreaseValue / 100));
			UnityEngine.Debug.Log("**************** heal = " + num);
			InGamePlayManager.Heal(num);
		}
	}

	public void CheckLeaderSkillHP1Setting(float prevHp)
	{
		if (leaderSkillIdx != 0 && leaderskillDbData.leaderskillType == 5)
		{
			float num = 0f;
			num = 100f * (prevHp / (float)InGamePlayManager.GetHunterTotalHP());
			if (num >= (float)leaderskillDbData.leaderskillRequirement)
			{
				InGamePlayManager.Heal(1);
			}
		}
	}

	private HunterInfo SetHunterStat_Color(HunterInfo _hunterInfo)
	{
		HunterInfo hunterInfo = null;
		switch (leaderskillDbData.leaderskillRequirement)
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

	private HunterInfo SetHunterStat_Attribute(HunterInfo _hunterInfo)
	{
		HunterInfo hunterInfo = null;
		switch (leaderskillDbData.leaderskillRequirement)
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
		switch (leaderskillDbData.leaderSkillDecreaseStat)
		{
		case "attack":
			_info.leaderSkillAttack = (int)(-1f * ((float)(int)GameUtil.GetHunterReinForceAttack(_info.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)leaderskillDbData.leaderSkillDecreaseValue / 100f)));
			break;
		case "hp":
			_info.leaderSkillHp = (int)(-1f * ((float)(int)GameUtil.GetHunterReinForceHP(_info.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)leaderskillDbData.leaderSkillDecreaseValue / 100f)));
			break;
		case "recovery":
			_info.leaderSkillRecovery = (int)(-1f * ((float)(int)GameUtil.GetHunterReinForceHeal(_info.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)leaderskillDbData.leaderSkillDecreaseValue / 100f)));
			break;
		}
		switch (leaderskillDbData.leaderSkillIncreaseStat)
		{
		case "attack":
			_info.leaderSkillAttack = (int)((float)(int)GameUtil.GetHunterReinForceAttack(_info.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)leaderskillDbData.leaderSkillIncreaseValue / 100f));
			break;
		case "hp":
			_info.leaderSkillHp = (int)((float)(int)GameUtil.GetHunterReinForceHP(_info.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)leaderskillDbData.leaderSkillIncreaseValue / 100f));
			UnityEngine.Debug.Log("_hunterInfo.originHp = " + GameUtil.GetHunterReinForceHP(_info.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)));
			UnityEngine.Debug.Log("_hunterInfo.leaderSkillHp = " + _info.leaderSkillHp);
			break;
		case "recovery":
			_info.leaderSkillRecovery = (int)((float)(int)GameUtil.GetHunterReinForceHeal(_info.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_info.Hunter.hunterIdx)) * ((float)leaderskillDbData.leaderSkillIncreaseValue / 100f));
			break;
		}
		return _info;
	}

	private int ChangeComboDamage(int _damage, int _combo)
	{
		int result = _damage;
		if (leaderskillDbData.leaderskillRequirement <= _combo)
		{
			result = (int)((float)_damage + (float)_damage * ((float)leaderskillDbData.leaderSkillIncreaseValue / 100f));
		}
		return result;
	}

	private int ChangeColorDamage(int _damage, int _color)
	{
		int result = _damage;
		if (leaderskillDbData.leaderskillRequirement <= _color)
		{
			result = (int)((float)_damage + (float)_damage * ((float)leaderskillDbData.leaderSkillIncreaseValue / 100f));
		}
		return result;
	}
}
