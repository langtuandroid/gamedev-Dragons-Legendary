using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
	private enum BATTLE_STATE
	{
		NONE,
		PUZZLE,
		SETTING,
		ADDDAMAGE,
		ADDCOMBO,
		HUNTERATTACK,
		MONSTERATTACK,
		WAVECHANGE,
		USERSKILL,
		GAMECLEAR,
		GAMEFAIL
	}

	private PuzzlePlayData levelData = new PuzzlePlayData();

	[SerializeField]
	private Transform[] Hunter_Pos_Arr;

	[SerializeField]
	private Hero[] Hunter_Arr;

	[SerializeField]
	private Transform Hunter_Character_Pos_Set;

	[SerializeField]
	private Transform[] Hunter_Character_Pos_Arr;

	[SerializeField]
	private Enemy[] Hunter_Character_Arr;

	[SerializeField]
	private Transform[] Monster_Pos_Arr;

	[SerializeField]
	private Enemy[] Monster_Arr;

	[SerializeField]
	private Transform BG_Tr;

	[SerializeField]
	private Animator BG_Anim;

	[SerializeField]
	private Transform Target_Aim;

	[SerializeField]
	private Transform Target_DragonAim;

	[SerializeField]
	private HeroLeaderSkill hunterLeaderSkill;

	[SerializeField]
	private int monsterAttackCount;

	private BATTLE_STATE Battle_State;

	private int current_Combo;

	private int current_Wave;

	private int current_Turn;

	private int current_Color;

	private bool isColor_Red;

	private bool isColor_Blue;

	private bool isColor_Green;

	private bool isColor_Yellow;

	private bool isColor_Purple;

	private int attack_last_idx;

	private bool isAllClear;

	[SerializeField]
	private List<AttackData> Hunter_Attack_List = new List<AttackData>();

	public int CurrentCombo => current_Combo;

	public HeroLeaderSkill GetHunterLeaderSkill => hunterLeaderSkill;

	public bool HunterLeaderSkillNullCheck()
	{
		if (hunterLeaderSkill == null)
		{
			return false;
		}
		return true;
	}

	public int HunterRecovery()
	{
		int num = 0;
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			num += (int)GameUtil.GetHunterReinForceHeal(Hunter_Arr[i].HeroInfo.Stat.hunterRecovery + Hunter_Arr[i].HeroInfo.leaderSkillRecovery, GameDataManager.HasUserHunterEnchant(Hunter_Arr[i].HeroInfo.Hunter.hunterIdx));
		}
		return num;
	}

	public int HunterHP()
	{
		int num = 0;
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			num += (int)GameUtil.GetHunterReinForceHP(Hunter_Arr[i].HeroInfo.Stat.hunterHp + Hunter_Arr[i].HeroInfo.leaderSkillRecovery, GameDataManager.HasUserHunterEnchant(Hunter_Arr[i].HeroInfo.Hunter.hunterIdx));
		}
		return num;
	}

	public void Set_Battle_Data(PuzzlePlayData _levelData)
	{
		levelData = _levelData;
		current_Combo = 0;
		current_Color = 0;
		isColor_Blue = false;
		isColor_Green = false;
		isColor_Purple = false;
		isColor_Red = false;
		isColor_Yellow = false;
		current_Wave = 0;
		current_Turn = 0;
		Set_BG(levelData.stage);
		Set_State(BATTLE_STATE.SETTING);
		Set_Hunter();
		StartCoroutine(Start_Set_Wave_Delay());
	}

	public void User_Turn_Check(bool isAddTurn)
	{
		if (current_Turn > 0)
		{
			hunterLeaderSkill.CheckLeaderSkillHeal(Hunter_Arr);
			UnityEngine.Debug.Log("*********************** LeaderSkill Heal Check ~~~~");
			for (int i = 0; i < Hunter_Arr.Length; i++)
			{
				Hunter_Arr[i].ChangeState(HeroState.idle);
			}
			for (int j = 0; j < Monster_Arr.Length; j++)
			{
				Monster_Arr[j].ChangeMonsterState(EnemyState.Idle);
			}
		}
		if (isAddTurn)
		{
			current_Turn++;
		}
		PuzzlePlayManager.ThisTurn(current_Turn);
	}

	public void Battle_Start_Match()
	{
		Set_State(BATTLE_STATE.PUZZLE);
		UnityEngine.Debug.Log("JY ------------------------- PUZZLE");
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			Hunter_Arr[i].SkillReadyEffect(_isOn: false);
		}
	}

	public void Battle_AddAttack_Start()
	{
		if (Battle_State != BATTLE_STATE.ADDDAMAGE)
		{
			current_Combo = 0;
			current_Color = 0;
			isColor_Blue = false;
			isColor_Green = false;
			isColor_Purple = false;
			isColor_Red = false;
			isColor_Yellow = false;
			Set_State(BATTLE_STATE.ADDDAMAGE);
			for (int i = 0; i < Hunter_Arr.Length; i++)
			{
				Hunter_Arr[i].ResetDamage();
			}
		}
	}

	public void Battle_AddAttack_Block(BlockType _type, int count)
	{
		current_Combo++;
		switch (_type)
		{
		case BlockType.Red:
			if (!isColor_Red)
			{
				isColor_Red = true;
			}
			Check_HunterType_And_AddDamge(HeroType.RED, count);
			break;
		case BlockType.Green:
			if (!isColor_Green)
			{
				isColor_Green = true;
			}
			Check_HunterType_And_AddDamge(HeroType.GREEN, count);
			break;
		case BlockType.Yellow:
			if (!isColor_Yellow)
			{
				isColor_Yellow = true;
			}
			Check_HunterType_And_AddDamge(HeroType.YELLOW, count);
			break;
		case BlockType.Purple:
			if (!isColor_Purple)
			{
				isColor_Purple = true;
			}
			Check_HunterType_And_AddDamge(HeroType.PURPLE, count);
			break;
		case BlockType.Blue:
			if (!isColor_Blue)
			{
				isColor_Blue = true;
			}
			Check_HunterType_And_AddDamge(HeroType.BLUE, count);
			break;
		}
	}

	public void Battle_AddAttack_Complete()
	{
		Check_Attack_Last_Hunter();
		CheckAttackColorCount();
		Set_State(BATTLE_STATE.ADDCOMBO);
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			Hunter_Arr[i].AddDamageCombo(current_Combo, current_Color, attack_last_idx, End_Combo_Effect);
		}
	}

	public void Battle_Attack_Start()
	{
		Set_State(BATTLE_STATE.HUNTERATTACK);
		if (Hunter_Attack_List.Count > 0)
		{
			Hunter_Attack_List.Clear();
		}
		if (attack_last_idx != -1)
		{
			for (int i = 0; i < Hunter_Arr.Length; i++)
			{
				Hunter_Arr[i].StartAttack(Set_Hunter_Attack_Arr);
			}
		}
		else
		{
			StartCoroutine(End_Hunter_Attack());
		}
	}

	public void Set_Hunter_Attack_Arr(int _damage, int _hunter_arr_idx)
	{
		if (_damage > 0)
		{
			AttackData attackData = new AttackData();
			attackData.damage = _damage;
			attackData.idx = _hunter_arr_idx;
			Hunter_Attack_List.Add(attackData);
		}
		if (attack_last_idx == _hunter_arr_idx)
		{
			StartCoroutine(Hunter_Attack_Start());
		}
	}

	public void Check_Monster_Attack()
	{
		Set_State(BATTLE_STATE.MONSTERATTACK);
		StartCoroutine(Monster_Attack_Coroutine());
	}

	public void Show_Heal_Eff()
	{
		Transform transform = null;
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			transform = MasterPoolManager.SpawnObject("Effect", "FX_Heal", Hunter_Arr[i].transform, 2f);
		}
	}

	public void LastMonsterCoinEffect()
	{
		bool flag = false;
		int num = 0;
		Transform transform = null;
		for (int i = 0; i < Monster_Arr.Length; i++)
		{
			if (Monster_Arr[i].MonsterGaugeHP > 0)
			{
				num++;
				transform = Monster_Arr[i].transform;
			}
		}
		if (num == 1)
		{
			Transform transform2 = null;
			if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
			{
				transform2 = MasterPoolManager.SpawnObject("Effect", "Fx_CoinDrop", null, 2f);
				SoundController.EffectSound_Play(EffectSoundType.GetCoinIngame);
			}
			else
			{
				transform2 = MasterPoolManager.SpawnObject("Effect", "Fx_PointDrop", null, 2f);
				SoundController.EffectSound_Play(EffectSoundType.GetArenaPoint);
			}
			transform2.position = transform.position;
		}
	}

	public void MonsterTargeting(Enemy _monster)
	{
		if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Arena)
		{
			Target_Aim.gameObject.SetActive(value: true);
			Target_Aim.position = _monster.HP_GaugeBar.position;
			Target_Aim.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		}
		else if (levelData.isDragon == 0)
		{
			Target_Aim.gameObject.SetActive(value: true);
			Target_Aim.position = _monster.HP_GaugeBar.position;
		}
		else
		{
			Target_DragonAim.gameObject.SetActive(value: true);
			Target_DragonAim.position = _monster.HP_GaugeBar.position;
		}
	}

	public void MonsterUnTargeting()
	{
		Target_Aim.gameObject.SetActive(value: false);
		Target_DragonAim.gameObject.SetActive(value: false);
	}

	public Vector3[] Hunter_Position(BlockType _type)
	{
		int num = 0;
		int num2 = 0;
		Vector3[] array = null;
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			switch (_type)
			{
			case BlockType.Red:
				if (Hunter_Arr[i].HeroType == HeroType.RED)
				{
					num++;
				}
				break;
			case BlockType.Green:
				if (Hunter_Arr[i].HeroType == HeroType.GREEN)
				{
					num++;
				}
				break;
			case BlockType.Yellow:
				if (Hunter_Arr[i].HeroType == HeroType.YELLOW)
				{
					num++;
				}
				break;
			case BlockType.Purple:
				if (Hunter_Arr[i].HeroType == HeroType.PURPLE)
				{
					num++;
				}
				break;
			case BlockType.Blue:
				if (Hunter_Arr[i].HeroType == HeroType.BLUE)
				{
					num++;
				}
				break;
			}
		}
		if (num > 0)
		{
			array = new Vector3[num];
		}
		for (int j = 0; j < Hunter_Arr.Length; j++)
		{
			switch (_type)
			{
			case BlockType.Red:
				if (Hunter_Arr[j].HeroType == HeroType.RED)
				{
					array[num2] = Hunter_Arr[j].transform.position;
					num2++;
				}
				break;
			case BlockType.Green:
				if (Hunter_Arr[j].HeroType == HeroType.GREEN)
				{
					array[num2] = Hunter_Arr[j].transform.position;
					num2++;
				}
				break;
			case BlockType.Yellow:
				if (Hunter_Arr[j].HeroType == HeroType.YELLOW)
				{
					array[num2] = Hunter_Arr[j].transform.position;
					num2++;
				}
				break;
			case BlockType.Purple:
				if (Hunter_Arr[j].HeroType == HeroType.PURPLE)
				{
					array[num2] = Hunter_Arr[j].transform.position;
					num2++;
				}
				break;
			case BlockType.Blue:
				if (Hunter_Arr[j].HeroType == HeroType.BLUE)
				{
					array[num2] = Hunter_Arr[j].transform.position;
					num2++;
				}
				break;
			}
		}
		return array;
	}

	public Transform GetMonsterTurn()
	{
		return Monster_Arr[0].GetMonsterTurn();
	}

	public Transform IsHunterSkillFull()
	{
		Transform transform = null;
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			if (Hunter_Arr[i].IsHeroSkillAvailable)
			{
				transform = Hunter_Arr[i].transform;
				break;
			}
		}
		if (transform != null)
		{
			return transform;
		}
		return null;
	}

	public void UseHunterSkillForTutorial(Hero hunter)
	{
		if (PuzzlePlayManager.OnHunterSkillEventComplete != null)
		{
			PuzzlePlayManager.OnHunterSkillEventComplete();
		}
		Use_Hunter_Skill(hunter);
	}

	public void ForceWaveClear()
	{
		for (int i = 0; i < Monster_Arr.Length; i++)
		{
			Monster_Arr[i].SetMonsterHealth(1000000);
		}
		attack_last_idx = -1;
		StartCoroutine(End_Hunter_Attack());
	}

	public void ForceIntroMonsterHp()
	{
		for (int i = 0; i < Monster_Arr.Length; i++)
		{
			Monster_Arr[i].ForceMonsterHp(1);
		}
	}

	public void Monster_Attack_End_Check_Public()
	{
		Monster_Attack_End_Check();
	}

	public void SetSpeedBattle()
	{
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			Animator[] componentsInChildren = Hunter_Arr[i].HunterCharacter.transform.GetComponentsInChildren<Animator>(includeInactive: true);
			foreach (Animator animator in componentsInChildren)
			{
				animator.speed = GameInfo.inGameBattleSpeedRate;
			}
		}
		for (int k = 0; k < Monster_Arr.Length; k++)
		{
			Animator[] componentsInChildren2 = Monster_Arr[k].transform.GetComponentsInChildren<Animator>(includeInactive: true);
			foreach (Animator animator2 in componentsInChildren2)
			{
				animator2.speed = GameInfo.inGameBattleSpeedRate;
			}
		}
		if (BG_Anim != null)
		{
			BG_Anim.speed = GameInfo.inGameBattleSpeedRate;
		}
	}

	public void SetFullSkillGague(int _index)
	{
		Hunter_Arr[_index].SetHeroValue();
		Hunter_Arr[_index].SkillReadyEffect(_isOn: true);
	}

	private void Init()
	{
	}

	private void RegisterTouchEvent()
	{
		PuzzlePlayManager.OnTouchBegin = (Action<Vector3, RaycastHit2D>)Delegate.Combine(PuzzlePlayManager.OnTouchBegin, new Action<Vector3, RaycastHit2D>(OnTouchBeginEvent));
	}

	private void Set_BG(int _level)
	{
		Transform transform = null;
		transform = ((GameInfo.inGamePlayData.inGameType != 0) ? MasterPoolManager.SpawnObject("Stage", "BG_Arena") : ((levelData.isDragon != 0) ? MasterPoolManager.SpawnObject("Stage", "BG_Dragon") : MasterPoolManager.SpawnObject("Stage", "BG_" + _level)));
		transform.SetParent(BG_Tr);
		transform.localPosition = Vector3.zero;
		BG_Anim = transform.GetComponent<Animator>();
	}

	private void Set_State(BATTLE_STATE _state)
	{
		switch (_state)
		{
		case BATTLE_STATE.NONE:
			Battle_State = BATTLE_STATE.NONE;
			break;
		case BATTLE_STATE.SETTING:
			Battle_State = BATTLE_STATE.SETTING;
			break;
		case BATTLE_STATE.PUZZLE:
			Battle_State = BATTLE_STATE.PUZZLE;
			break;
		case BATTLE_STATE.ADDDAMAGE:
			Battle_State = BATTLE_STATE.ADDDAMAGE;
			break;
		case BATTLE_STATE.ADDCOMBO:
			Battle_State = BATTLE_STATE.ADDCOMBO;
			break;
		case BATTLE_STATE.HUNTERATTACK:
			Battle_State = BATTLE_STATE.HUNTERATTACK;
			break;
		case BATTLE_STATE.USERSKILL:
			Battle_State = BATTLE_STATE.USERSKILL;
			break;
		case BATTLE_STATE.WAVECHANGE:
			Battle_State = BATTLE_STATE.WAVECHANGE;
			break;
		case BATTLE_STATE.GAMECLEAR:
			Battle_State = BATTLE_STATE.GAMECLEAR;
			break;
		case BATTLE_STATE.GAMEFAIL:
			Battle_State = BATTLE_STATE.GAMEFAIL;
			break;
		case BATTLE_STATE.MONSTERATTACK:
			Battle_State = BATTLE_STATE.MONSTERATTACK;
			break;
		}
	}

	private void Set_Hunter()
	{
		if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
		{
			SetStageHunter();
		}
		else
		{
			SetArenaHunter();
		}
		Set_Hunter_Pos_Count();
		SetHunterSummonEffect();
		SetHunterSkillBooster();
	}

	private void SetStageHunter()
	{
		int num = 0;
		Hunter_Arr = new Hero[GameInfo.userData.huntersUseInfo.Length];
		for (int i = 0; i < GameInfo.userData.huntersUseInfo.Length; i++)
		{
			Transform transform = null;
			Transform transform2 = null;
			HunterInfo hunterInfo = null;
			hunterInfo = GameDataManager.GetHunterInfo(GameInfo.userData.huntersUseInfo[i].hunterIdx, GameInfo.userData.huntersUseInfo[i].hunterLevel, GameInfo.userData.huntersUseInfo[i].hunterTier);
			if (i == 0 && hunterInfo.Stat.hunterLeaderSkill != 0)
			{
				hunterLeaderSkill.SetSkill(hunterInfo.Stat.hunterLeaderSkill);
			}
			transform2 = MasterPoolManager.SpawnObject("Hunter", hunterInfo.Hunter.hunterIdx.ToString());
			if (!transform2.gameObject.activeSelf)
			{
				transform2.gameObject.SetActive(value: true);
			}
			transform2.SetParent(Hunter_Character_Pos_Arr[i]);
			transform2.localPosition = Vector3.zero;
			transform2.localScale = Vector3.one;
			transform = MasterPoolManager.SpawnObject("Hunter", "Hunter_" + hunterInfo.Hunter.hunterIdx);
			transform.SetParent(Hunter_Pos_Arr[i]);
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			Hunter_Arr[i] = transform.GetComponent<Hero>();
			Hunter_Arr[i].Construct(i, transform2.GetComponent<HeroCharacter>(), hunterLeaderSkill, hunterLeaderSkill.CheckLeaderSkillSettings(hunterInfo));
			num += (int)GameUtil.GetHunterReinForceHP(Hunter_Arr[i].HeroInfo.Stat.hunterHp + Hunter_Arr[i].HeroInfo.leaderSkillHp, GameDataManager.HasUserHunterEnchant(Hunter_Arr[i].HeroInfo.Hunter.hunterIdx));
		}
		PuzzlePlayManager.SetMaxHP(num);
	}

	private void SetArenaHunter()
	{
		int num = 0;
		Hunter_Arr = new Hero[GameInfo.userData.huntersArenaUseInfo.Length];
		for (int i = 0; i < GameInfo.userData.huntersArenaUseInfo.Length; i++)
		{
			Transform transform = null;
			Transform transform2 = null;
			HunterInfo hunterInfo = null;
			hunterInfo = GameDataManager.GetHunterInfo(GameInfo.userData.huntersArenaUseInfo[i].hunterIdx, GameInfo.userData.huntersArenaUseInfo[i].hunterLevel, GameInfo.userData.huntersArenaUseInfo[i].hunterTier);
			if (i == 0 && hunterInfo.Stat.hunterLeaderSkill != 0)
			{
				hunterLeaderSkill.SetSkill(hunterInfo.Stat.hunterLeaderSkill);
			}
			transform2 = MasterPoolManager.SpawnObject("Hunter", hunterInfo.Hunter.hunterIdx.ToString());
			if (!transform2.gameObject.activeSelf)
			{
				transform2.gameObject.SetActive(value: true);
			}
			transform2.SetParent(Hunter_Character_Pos_Arr[i]);
			transform2.localPosition = Vector3.zero;
			transform2.localScale = Vector3.one;
			transform = MasterPoolManager.SpawnObject("Hunter", "Hunter_" + hunterInfo.Hunter.hunterIdx);
			transform.SetParent(Hunter_Pos_Arr[i]);
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			Hunter_Arr[i] = transform.GetComponent<Hero>();
			Hunter_Arr[i].Construct(i, transform2.GetComponent<HeroCharacter>(), hunterLeaderSkill, hunterLeaderSkill.CheckLeaderSkillSettings(hunterInfo));
			num += (int)GameUtil.GetHunterReinForceHP(Hunter_Arr[i].HeroInfo.Stat.hunterHp + Hunter_Arr[i].HeroInfo.leaderSkillHp, GameDataManager.HasUserHunterEnchant(Hunter_Arr[i].HeroInfo.Hunter.hunterIdx));
			UnityEngine.Debug.Log("GameInfo.inGamePlayData.inGameType = " + GameInfo.inGamePlayData.inGameType);
			if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Arena)
			{
				Hunter_Arr[i].SetBuff();
			}
		}
		PuzzlePlayManager.SetMaxHP(num);
	}

	private void SetHunterSkillBooster()
	{
		if (GameInfo.inGamePlayData.dicActiveBoostItem.ContainsKey(1))
		{
			int num = 0;
			num = UnityEngine.Random.Range(0, Hunter_Arr.Length);
			Hunter_Arr[num].SetHeroValue();
		}
	}

	private void SetHunterSummonEffect()
	{
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			Hunter_Arr[i].HunterCharacter.SetEffect();
		}
	}

	private void Set_Wave(int _wave, int _monstercount)
	{
		SoundController.EffectSound_Play(EffectSoundType.WaveStart);
		Set_Monster_Pos_Count(levelData.dicWaveDbData[_wave].mFormation);
		if (Monster_Arr != null)
		{
			for (int i = 0; i < Monster_Arr.Length; i++)
			{
				MasterPoolManager.ReturnToPool("Monster", Monster_Arr[i].transform);
			}
			Monster_Arr = null;
		}
		Monster_Arr = new Enemy[_monstercount];
		for (int j = 0; j < _monstercount; j++)
		{
			Transform transform = null;
			Transform transform2 = null;
			transform = MasterPoolManager.SpawnObject("Monster", levelData.dicMonsterStatData[_wave][j].mMonsterIdx.ToString());
			Monster_Arr[j] = transform.GetComponent<Enemy>();
			int monsterSettingTurn = 0;
			int monster_DropItem = 0;
			switch (j)
			{
			case 0:
				monsterSettingTurn = levelData.dicWaveDbData[_wave].atM1;
				monster_DropItem = levelData.dicWaveDbData[_wave].spawnM1;
				break;
			case 1:
				monsterSettingTurn = levelData.dicWaveDbData[_wave].atM2;
				monster_DropItem = levelData.dicWaveDbData[_wave].spawnM2;
				break;
			case 2:
				monsterSettingTurn = levelData.dicWaveDbData[_wave].atM3;
				monster_DropItem = levelData.dicWaveDbData[_wave].spawnM3;
				break;
			case 3:
				monsterSettingTurn = levelData.dicWaveDbData[_wave].atM4;
				monster_DropItem = levelData.dicWaveDbData[_wave].spawnM4;
				break;
			}
			Monster_Arr[j].Construct(monsterSettingTurn, monster_DropItem, levelData.dicMonsterStatData[_wave][j], j);
			transform.SetParent(Monster_Pos_Arr[j]);
			transform.localPosition = Vector3.zero;
			transform2 = MasterPoolManager.SpawnObject("Effect", "FX_Summon01", null, 1f);
			transform2.position = transform.position;
			if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
			{
				transform.localScale = Vector3.one;
			}
			else
			{
				transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			}
		}
		Set_State(BATTLE_STATE.NONE);
		for (int k = 0; k < Hunter_Arr.Length; k++)
		{
			Hunter_Arr[k].SkillReadyEffect(_isOn: true);
			Hunter_Arr[k].RemoveStun();
		}
		PuzzlePlayManager.ThisWave(current_Wave);
	}

	private IEnumerator Set_Wave_Delay()
	{
		yield return new WaitForSeconds(1f);
		if (levelData.isDragon == 0)
		{
			SoundController.EffectSound_Play(EffectSoundType.WaveMove);
			if (levelData.dicWaveDbData[current_Wave + 1].isWarning == 1)
			{
				PuzzlePlayManager.WarningEffect();
			}
			BG_Anim.ResetTrigger("BG_Walk");
			BG_Anim.SetTrigger("BG_Walk");
			HunterCharacterMove();
			yield return new WaitForSeconds(2f / GameInfo.inGameBattleSpeedRate);
			if (levelData.dicWaveDbData[current_Wave + 1].isWarning == 1)
			{
				PuzzlePlayManager.HideEffect();
			}
			HunterCharacterIdle();
		}
		else
		{
			if (levelData.dicWaveDbData[current_Wave + 1].isWarning == 1)
			{
				PuzzlePlayManager.WarningEffect();
			}
			yield return new WaitForSeconds(2f / GameInfo.inGameBattleSpeedRate);
			if (levelData.dicWaveDbData[current_Wave + 1].isWarning == 1)
			{
				PuzzlePlayManager.HideEffect();
			}
		}
		current_Wave++;
		isAllClear = false;
		Set_Wave(current_Wave, levelData.dicMonsterStatData[current_Wave].Count);
		PuzzlePlayManager.StartTurn();
	}

	private IEnumerator Start_Set_Wave_Delay()
	{
		if (levelData.isDragon == 0)
		{
			SoundController.EffectSound_Play(EffectSoundType.WaveMove);
			if (levelData.dicWaveDbData[current_Wave + 1].isWarning == 1)
			{
				PuzzlePlayManager.WarningEffect();
			}
			HunterCharacterMove();
			yield return new WaitForSeconds(2f / GameInfo.inGameBattleSpeedRate);
			if (levelData.dicWaveDbData[current_Wave + 1].isWarning == 1)
			{
				PuzzlePlayManager.HideEffect();
			}
			HunterCharacterIdle();
		}
		else
		{
			if (levelData.dicWaveDbData[current_Wave + 1].isWarning == 1)
			{
				PuzzlePlayManager.WarningEffect();
			}
			yield return new WaitForSeconds(2f / GameInfo.inGameBattleSpeedRate);
			if (levelData.dicWaveDbData[current_Wave + 1].isWarning == 1)
			{
				PuzzlePlayManager.HideEffect();
			}
		}
		current_Wave++;
		isAllClear = false;
		Set_Wave(current_Wave, levelData.dicMonsterStatData[current_Wave].Count);
		PuzzlePlayManager.StartTurn();
	}

	private void HunterCharacterMove()
	{
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			Hunter_Arr[i].HunterCharacter.ChangeAnim(Anim_Type.MOVE);
		}
	}

	private void HunterCharacterIdle()
	{
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			Hunter_Arr[i].HunterCharacter.ChangeAnim(Anim_Type.IDLE);
		}
	}

	private void Set_Monster_Pos_Count(string _type)
	{
		switch (_type)
		{
		case "a":
			Monster_Pos_Arr[0].localPosition = new Vector3(2.5f, 2.9f, 0f);
			break;
		case "b":
			Monster_Pos_Arr[0].localPosition = new Vector3(1.2f, 3.6f, 0f);
			Monster_Pos_Arr[1].localPosition = new Vector3(1.2f, 2.2f, 0f);
			break;
		case "c":
			Monster_Pos_Arr[0].localPosition = new Vector3(2.5f, 4.3f, 0f);
			Monster_Pos_Arr[1].localPosition = new Vector3(2.5f, 2.9f, 0f);
			Monster_Pos_Arr[2].localPosition = new Vector3(2.5f, 1.5f, 0f);
			break;
		case "d":
			Monster_Pos_Arr[0].localPosition = new Vector3(2.5f, 4.3f, 0f);
			Monster_Pos_Arr[1].localPosition = new Vector3(1.2f, 3.6f, 0f);
			Monster_Pos_Arr[2].localPosition = new Vector3(2.5f, 2.9f, 0f);
			Monster_Pos_Arr[3].localPosition = new Vector3(2.5f, 1.5f, 0f);
			break;
		case "e":
			Monster_Pos_Arr[0].localPosition = new Vector3(2.5f, 4.3f, 0f);
			Monster_Pos_Arr[1].localPosition = new Vector3(1.2f, 3.6f, 0f);
			Monster_Pos_Arr[2].localPosition = new Vector3(2.5f, 2.9f, 0f);
			Monster_Pos_Arr[3].localPosition = new Vector3(1.2f, 2.2f, 0f);
			Monster_Pos_Arr[4].localPosition = new Vector3(2.5f, 1.5f, 0f);
			break;
		case "f":
			Monster_Pos_Arr[0].localPosition = new Vector3(1.85f, 2.2f, 0f);
			break;
		case "g":
			Monster_Pos_Arr[0].localPosition = new Vector3(1.85f, 3.45f, 0f);
			Monster_Pos_Arr[1].localPosition = new Vector3(1.85f, 1.2f, 0f);
			break;
		case "h":
			Monster_Pos_Arr[0].localPosition = new Vector3(2.3f, 1.2f, 0f);
			break;
		}
	}

	private void Set_Hunter_Pos_Count()
	{
		switch (Hunter_Arr.Length)
		{
		case 1:
			Hunter_Character_Pos_Arr[0].localPosition = new Vector3(-2.5f, 2.9f, 0f);
			break;
		case 2:
			Hunter_Character_Pos_Arr[0].localPosition = new Vector3(-1.2f, 3.6f, 0f);
			Hunter_Character_Pos_Arr[1].localPosition = new Vector3(-1.2f, 2.2f, 0f);
			break;
		case 3:
			Hunter_Character_Pos_Arr[0].localPosition = new Vector3(-2.5f, 4.3f, 0f);
			Hunter_Character_Pos_Arr[1].localPosition = new Vector3(-2.5f, 2.9f, 0f);
			Hunter_Character_Pos_Arr[2].localPosition = new Vector3(-2.5f, 1.5f, 0f);
			break;
		case 4:
			Hunter_Character_Pos_Arr[0].localPosition = new Vector3(-2.5f, 4.3f, 0f);
			Hunter_Character_Pos_Arr[1].localPosition = new Vector3(-1.2f, 3.6f, 0f);
			Hunter_Character_Pos_Arr[2].localPosition = new Vector3(-2.5f, 2.9f, 0f);
			Hunter_Character_Pos_Arr[3].localPosition = new Vector3(-2.5f, 1.5f, 0f);
			break;
		case 5:
			Hunter_Character_Pos_Arr[0].localPosition = new Vector3(-2.5f, 4.3f, 0f);
			Hunter_Character_Pos_Arr[1].localPosition = new Vector3(-1.2f, 3.6f, 0f);
			Hunter_Character_Pos_Arr[2].localPosition = new Vector3(-2.5f, 2.9f, 0f);
			Hunter_Character_Pos_Arr[3].localPosition = new Vector3(-1.2f, 2.2f, 0f);
			Hunter_Character_Pos_Arr[4].localPosition = new Vector3(-2.5f, 1.5f, 0f);
			break;
		}
		if (levelData.isDragon == 1)
		{
			Hunter_Character_Pos_Set.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			Hunter_Character_Pos_Set.localPosition = new Vector3(-1.5f, 0.5f, 0f);
		}
		else
		{
			Hunter_Character_Pos_Set.localScale = Vector3.one;
			Hunter_Character_Pos_Set.localPosition = Vector3.zero;
		}
	}

	private void Check_HunterType_And_AddDamge(HeroType _type, int _count)
	{
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			if (Hunter_Arr[i].HeroType == _type && !Hunter_Arr[i].IsHunterStun)
			{
				Hunter_Arr[i].AddDamage(_count);
				Hunter_Arr[i].AddGaugeSkill(_count);
			}
		}
	}

	private void End_Combo_Effect(int _idx)
	{
		if (attack_last_idx != -1)
		{
			if (attack_last_idx == _idx)
			{
				PuzzlePlayManager.HunterComboComplete();
				PuzzlePlayManager.StartHunterAttack();
			}
		}
		else if (Hunter_Arr.Length - 1 == _idx)
		{
			PuzzlePlayManager.StartHunterAttack();
			PuzzlePlayManager.HunterComboComplete();
		}
	}

	private Enemy Attack_Monster_Targeting()
	{
		Enemy result = null;
		for (int i = 0; i < Monster_Arr.Length; i++)
		{
			if (Monster_Arr[i].MonsterTarget && Monster_Arr[i].MonsterHP > 0)
			{
				result = Monster_Arr[i];
			}
		}
		return result;
	}

	private Enemy Attack_Monster_NonTargeting()
	{
		Enemy result = null;
		int num = 100;
		int num2 = 100;
		for (int i = 0; i < Monster_Arr.Length; i++)
		{
			if (Monster_Arr[i].MonsterTurn <= num2 && Monster_Arr[i].MonsterHP > 0 && i < num)
			{
				result = Monster_Arr[i];
				num = i;
			}
		}
		return result;
	}

	private Enemy Attack_Monster_Already_Killed()
	{
		return null;
	}

	private IEnumerator End_Hunter_Attack()
	{
		bool isTweening = true;
		yield return null;
		while (isTweening)
		{
			int count = 0;
			for (int i = 0; i < Hunter_Arr.Length; i++)
			{
				if (Hunter_Arr[i].HunterCharacter.IsTweening)
				{
					count++;
				}
			}
			isTweening = ((count != 0) ? true : false);
			yield return new WaitForSeconds(0.1f);
		}
		bool isAllClear = true;
		for (int j = 0; j < Monster_Arr.Length; j++)
		{
			Monster_Arr[j].SetMonsterAttack(1);
		}
		for (int k = 0; k < Monster_Arr.Length; k++)
		{
			if (Monster_Arr[k].MonsterHP > 0)
			{
				isAllClear = false;
				break;
			}
		}
		if (isAllClear)
		{
			PuzzlePlayManager.AttackOver();
			switch (GameInfo.inGamePlayData.inGameType)
			{
			case PuzzleInGameType.Stage:
				PuzzlePlayManager.MoreCoins(levelData.dicWaveDbData[current_Wave].getCoin);
				break;
			case PuzzleInGameType.Arena:
				PuzzlePlayManager.ArenaPoins(levelData.dicWaveDbData[current_Wave].getArenaPoint);
				break;
			}
			if (current_Wave == levelData.dicWaveDbData.Count)
			{
				Set_State(BATTLE_STATE.GAMECLEAR);
				StartCoroutine(Game_Clear());
			}
			else
			{
				Set_State(BATTLE_STATE.WAVECHANGE);
				StartCoroutine(Set_Wave_Delay());
			}
		}
		else
		{
			StartCoroutine(Hunter_Attack_Complete());
		}
	}

	private void End_Hunter_Skill()
	{
		bool flag = true;
		for (int i = 0; i < Monster_Arr.Length; i++)
		{
			if (Monster_Arr[i].MonsterHP > 0)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			PuzzlePlayManager.AttackOver();
			switch (GameInfo.inGamePlayData.inGameType)
			{
			case PuzzleInGameType.Stage:
				PuzzlePlayManager.MoreCoins(levelData.dicWaveDbData[current_Wave].getCoin);
				break;
			case PuzzleInGameType.Arena:
				PuzzlePlayManager.ArenaPoins(levelData.dicWaveDbData[current_Wave].getArenaPoint);
				break;
			}
			if (current_Wave == levelData.dicWaveDbData.Count)
			{
				Set_State(BATTLE_STATE.GAMECLEAR);
				StartCoroutine(Game_Clear());
			}
			else
			{
				Set_State(BATTLE_STATE.WAVECHANGE);
				StartCoroutine(Set_Wave_Delay());
			}
		}
		else
		{
			PuzzlePlayManager.ControllerStart();
			Set_State(BATTLE_STATE.NONE);
		}
	}

	private IEnumerator Game_Clear()
	{
		yield return new WaitForSeconds(1f);
		PuzzlePlayManager.ClearGame();
	}

	private IEnumerator Hunter_Attack_Complete()
	{
		yield return new WaitForSeconds(0.1f);
		Set_State(BATTLE_STATE.MONSTERATTACK);
		PuzzlePlayManager.StartEnemyTurn();
	}

	private void Use_Hunter_Skill(Hero _hunter)
	{
		Set_State(BATTLE_STATE.USERSKILL);
		PuzzlePlayManager.LockController();
		if (PuzzlePlayManager.OnUseHunterSkill != null)
		{
			PuzzlePlayManager.OnUseHunterSkill();
		}
		Enemy[] monster = null;
		int num = 0;
		int num2 = 0;
		switch (_hunter.HeroInfo.Skill.range)
		{
		case 0:
			Hunter_Arr[_hunter.HeroArrIdx].SetCharacterSkill((HunterSkillRange)_hunter.HeroInfo.Skill.range, monster, Use_Hunter_Skill_End);
			break;
		case 1:
			monster = new Enemy[1];
			monster[num] = Attack_Monster_Targeting();
			if (monster[num] == null)
			{
				monster[num] = Attack_Monster_NonTargeting();
			}
			if (monster[num] != null)
			{
				Hunter_Arr[_hunter.HeroArrIdx].SetCharacterSkill((HunterSkillRange)_hunter.HeroInfo.Skill.range, monster, Use_Hunter_Skill_End);
			}
			break;
		case 2:
			for (int i = 0; i < Monster_Arr.Length; i++)
			{
				if (Monster_Arr[i].MonsterHP > 0)
				{
					num2++;
				}
			}
			monster = new Enemy[num2];
			for (int j = 0; j < Monster_Arr.Length; j++)
			{
				if (Monster_Arr[j].MonsterHP > 0)
				{
					monster[num] = Monster_Arr[j];
					num++;
				}
			}
			Hunter_Arr[_hunter.HeroArrIdx].SetCharacterSkill((HunterSkillRange)_hunter.HeroInfo.Skill.range, monster, Use_Hunter_Skill_End);
			break;
		}
	}

	private void Use_Hunter_Skill_End()
	{
		End_Hunter_Skill();
	}

	private IEnumerator Hunter_Attack_Start()
	{
		yield return null;
		for (int i = 0; i < Hunter_Attack_List.Count; i++)
		{
			Enemy _monster2 = Attack_Monster_Targeting();
			if (_monster2 == null)
			{
				_monster2 = Attack_Monster_NonTargeting();
			}
			if (_monster2 != null)
			{
				Hunter_Attack_List[i].damage = GameUtil.Check_Property_Damage(Hunter_Arr[Hunter_Attack_List[i].idx], _monster2, Hunter_Attack_List[i].damage);
				_monster2.SetMonsterHealth(Hunter_Attack_List[i].damage);
				HunterCharacterAttack(_monster2, Hunter_Attack_List[i].idx, Hunter_Attack_List[i].damage);
				PuzzlePlayManager.StartHunterAttack(Hunter_Arr[Hunter_Attack_List[i].idx].HeroInfo.Hunter.hunterIdx);
			}
			else
			{
				_monster2 = Attack_Monster_Already_Killed();
				HunterCharacterAttack(_monster2, Hunter_Attack_List[i].idx, Hunter_Attack_List[i].damage);
				PuzzlePlayManager.StartHunterAttack(Hunter_Arr[Hunter_Attack_List[i].idx].HeroInfo.Hunter.hunterIdx);
			}
			yield return new WaitForSeconds(0.4f / GameInfo.inGameBattleSpeedRate);
		}
	}

	private void HunterCharacterAttack(Enemy _monster, int _idx, int _damage)
	{
		Hunter_Arr[_idx].SetAnimation(Anim_Type.ATTACK_HUNTER, _monster, _damage, HunterCharacterAttackEndCheck);
	}

	private void HunterCharacterAttackEndCheck()
	{
		bool flag = true;
		for (int i = 0; i < Monster_Arr.Length; i++)
		{
			if (Monster_Arr[i].MonsterHP > 0)
			{
				flag = false;
				break;
			}
		}
		if (flag && !isAllClear)
		{
			isAllClear = true;
			attack_last_idx = -1;
			StartCoroutine(End_Hunter_Attack());
		}
		else
		{
			if (flag)
			{
				return;
			}
			for (int j = 0; j < Hunter_Arr.Length; j++)
			{
				if (Hunter_Arr[j].HunterState == HeroState.attack)
				{
					return;
				}
			}
			attack_last_idx = -1;
			StartCoroutine(End_Hunter_Attack());
		}
	}

	private void CheckAttackColorCount()
	{
		if (isColor_Red)
		{
			current_Color++;
		}
		if (isColor_Blue)
		{
			current_Color++;
		}
		if (isColor_Green)
		{
			current_Color++;
		}
		if (isColor_Yellow)
		{
			current_Color++;
		}
		if (isColor_Purple)
		{
			current_Color++;
		}
	}

	private void Check_Attack_Last_Hunter()
	{
		attack_last_idx = -1;
		for (int i = 0; i < Hunter_Arr.Length; i++)
		{
			if (Hunter_Arr[i].HeroTotalDamage > 0)
			{
				attack_last_idx = i;
			}
		}
	}

	private Transform Hunter_Attack_To_Monster(HeroType _type)
	{
		Transform result = null;
		switch (_type)
		{
		case HeroType.RED:
			result = MasterPoolManager.SpawnObject("Effect", "Fx_Puzzle_red_attack", null, 0.5f);
			break;
		case HeroType.GREEN:
			result = MasterPoolManager.SpawnObject("Effect", "Fx_Puzzle_green_attack", null, 0.5f);
			break;
		case HeroType.YELLOW:
			result = MasterPoolManager.SpawnObject("Effect", "Fx_Puzzle_yellow_attack", null, 0.5f);
			break;
		case HeroType.PURPLE:
			result = MasterPoolManager.SpawnObject("Effect", "Fx_Puzzle_purple_attack", null, 0.5f);
			break;
		case HeroType.BLUE:
			result = MasterPoolManager.SpawnObject("Effect", "Fx_Puzzle_blue_attack", null, 0.5f);
			break;
		}
		return result;
	}

	private IEnumerator Monster_Attack_Coroutine()
	{
		monsterAttackCount = 0;
		yield return null;
		for (int j = 0; j < Monster_Arr.Length; j++)
		{
			if (Monster_Arr[j].MonsterTurn <= 0 && Monster_Arr[j].MonsterHP > 0)
			{
				monsterAttackCount++;
			}
		}
		for (int i = 0; i < Monster_Arr.Length; i++)
		{
			if (Monster_Arr[i].MonsterTurn <= 0 && Monster_Arr[i].MonsterHP > 0)
			{
				if (CheckMonsterSkill(Monster_Arr[i]))
				{
					yield return StartCoroutine(MonsterSkillUse(Monster_Arr[i]));
					continue;
				}
				Monster_Arr[i].SetAnimation(SetAttackToHunter(), Monster_Attack_End_Check);
				yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate);
				Monster_Arr[i].SetDamage(Monster_Arr[i].MonsterDamage);
				SoundController.EffectSound_Play(EffectSoundType.HunterHit);
				yield return new WaitForSeconds(0.5f / GameInfo.inGameBattleSpeedRate);
				Monster_Arr[i].RefreshTurnAttack();
			}
		}
		if (monsterAttackCount == 0)
		{
			Monster_Attack_End_Check();
		}
	}

	private void Monster_Attack_End_Check()
	{
		monsterAttackCount--;
		if (monsterAttackCount <= 0)
		{
			for (int i = 0; i < Monster_Arr.Length; i++)
			{
				Monster_Arr[i].AttackTurn();
			}
			Set_State(BATTLE_STATE.NONE);
			PuzzlePlayManager.StartTurn();
			for (int j = 0; j < Hunter_Arr.Length; j++)
			{
				Hunter_Arr[j].SkillReadyEffect(_isOn: true);
				Hunter_Arr[j].RemoveStun();
			}
		}
	}

	private bool CheckMonsterSkill(Enemy _monster)
	{
		bool result = false;
		int num = 0;
		if (_monster.SkillInterval <= 0)
		{
			num = UnityEngine.Random.Range(1, 101);
			if (num < _monster.MonsterSkillDbData.mSkillRatio)
			{
				result = true;
				_monster.SkillIntervalChange();
			}
		}
		else
		{
			_monster.DecreaseSkill();
		}
		return result;
	}

	private IEnumerator MonsterSkillUse(Enemy _monster)
	{
		Transform _eff_skill_cut = MasterPoolManager.SpawnObject("Skill", "FX_monsterskill_" + _monster.MonsterSkillDbData.mSkillType, null, 1f);
		_eff_skill_cut.position = Vector3.zero;
		yield return new WaitForSeconds(1f / GameInfo.inGameBattleSpeedRate);
		_monster.SetAnimation(SetAttackToHunter(), Monster_Attack_End_Check);
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate);
		switch (_monster.MonsterSkillDbData.mSkillType)
		{
		case 1:
		{
			int healHP = (int)((float)_monster.MonsterStat.mHp * 0.2f);
			for (int i = 0; i < Monster_Arr.Length; i++)
			{
				if (Monster_Arr[i].MonsterHP > 0)
				{
					Monster_Arr[i].HealMonster(healHP);
					Transform _eff_skill = MasterPoolManager.SpawnObject("Effect", "FX_Heal", Monster_Arr[i].transform, 2f);
					_eff_skill.position = Monster_Arr[i].transform.position;
				}
			}
			break;
		}
		case 2:
		{
			int num = UnityEngine.Random.Range(0, Hunter_Arr.Length);
			Hunter_Arr[num].StunHunter();
			break;
		}
		case 3:
			PuzzlePlayManager.ExeptionBlock(BlockExceptionType.Obstruction);
			break;
		case 4:
			PuzzlePlayManager.ExeptionBlock(BlockExceptionType.DefaultFix);
			break;
		}
		_monster.SetDamage(_monster.MonsterDamage * (_monster.MonsterSkillDbData.mSkillAttackMagnification / 100));
		SoundController.EffectSound_Play(EffectSoundType.HunterHit);
		yield return new WaitForSeconds(0.5f / GameInfo.inGameBattleSpeedRate);
		_monster.RefreshTurnAttack();
	}

	private Hero SetAttackToHunter()
	{
		int num = 0;
		num = UnityEngine.Random.Range(0, Hunter_Arr.Length);
		return Hunter_Arr[num];
	}

	private void OnTouchBeginEvent(Vector3 touchPosition, RaycastHit2D hit)
	{
		if (hit.collider != null && hit.collider.tag == "Hunter")
		{
			if (Battle_State != 0 || PuzzlePlayManager.OnHunterSkill != null)
			{
				return;
			}
			Hero hunter = null;
			hunter = hit.collider.GetComponent<Hero>();
			if (hunter.IsHeroSkillAvailable && !hunter.IsHunterStun)
			{
				if (GameInfo.inGamePlayData.level < 4)
				{
					InfoManager.HunterSkillClear();
				}
				else if (PuzzlePlayManager.OnHunterSkillEventComplete != null)
				{
					PuzzlePlayManager.OnHunterSkillEventComplete();
				}
				Use_Hunter_Skill(hunter);
			}
		}
		else
		{
			if (!(hit.collider != null) || !(hit.collider.tag == "Monster") || Battle_State != 0)
			{
				return;
			}
			Enemy monster = null;
			monster = hit.collider.GetComponent<Enemy>();
			for (int i = 0; i < Monster_Arr.Length; i++)
			{
				if (monster.gameObject.name == Monster_Arr[i].gameObject.name)
				{
					if (Monster_Arr[i].MonsterTarget)
					{
						Monster_Arr[i].SetMonsterTargeting(_istarget: false);
						PuzzlePlayManager.MonsterUnTargeting();
					}
					else
					{
						Monster_Arr[i].SetMonsterTargeting(_istarget: true);
						PuzzlePlayManager.MonsterTargeting(Monster_Arr[i]);
					}
				}
				else
				{
					Monster_Arr[i].SetMonsterTargeting(_istarget: false);
				}
			}
		}
	}

	private void Start()
	{
		Init();
		RegisterTouchEvent();
	}

	private void OnDisable()
	{
		PuzzlePlayManager.OnTouchBegin = (Action<Vector3, RaycastHit2D>)Delegate.Remove(PuzzlePlayManager.OnTouchBegin, new Action<Vector3, RaycastHit2D>(OnTouchBeginEvent));
	}
}
