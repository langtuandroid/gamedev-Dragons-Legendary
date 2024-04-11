using System;
using System.Collections;
using UnityEngine;

public class Hunter : Hunter_Base
{
	private Action<int, int> OnCallback_Hunter_Attack;

	private Action<int> OnCallback_Combo_Eff;

	private Action OnCallback_Skill_End;

	private Action OnCallback_CharacterAttack_End;

	[SerializeField]
	private Transform hunter_Tier_tr;

	[SerializeField]
	private HunterCharacter hunterCharacter;

	[SerializeField]
	private HunterInfo hunter_Info;

	[SerializeField]
	private HUNTER_TYPE hunter_type;

	[SerializeField]
	private HUNTER_TRIBE hunter_tribe;

	[SerializeField]
	private int hunter_Arr_Idx;

	[SerializeField]
	private int hunter_Block_Count;

	[SerializeField]
	private int hunter_Total_Damage;

	[SerializeField]
	private Transform hunter_Skill_Gauge_Sprite;

	[SerializeField]
	private int hunter_Skill_Gauge_Value;

	[SerializeField]
	private bool isHunter_Skill_Available;

	[SerializeField]
	private bool isHunterStun;

	[SerializeField]
	private int isHunterStunClearCount;

	[SerializeField]
	private int hunter_Skill_Gauge_Full_Value;

	[SerializeField]
	private GameObject hunter_Skill_Ready_Eff;

	[SerializeField]
	private float user_Bonus_Attack;

	[SerializeField]
	private int Damage_Dummy_constX = 1;

	[SerializeField]
	private float Damage_Dummy_Combo = 0.1f;

	[SerializeField]
	private Transform stunEff;

	[SerializeField]
	private HunterLeaderSkill hunterLeaderSkill;

	[SerializeField]
	private Transform arenaBuffTextAnchor;

	[SerializeField]
	private Transform hunterFace;

	private HUNTER_STATE hunterState;

	private Monster attackMonster;

	private int attackDamage;

	public HunterInfo HunterInfo => hunter_Info;

	public HUNTER_STATE HunterState => hunterState;

	public HUNTER_TYPE HunterType => hunter_type;

	public HUNTER_TRIBE HunterTribe => hunter_tribe;

	public int Hunter_Arr_Idx => hunter_Arr_Idx;

	public int Hunter_Total_Damage => hunter_Total_Damage;

	public int Hunter_Block_Count => hunter_Block_Count;

	public bool IsHunter_Skill_Available => isHunter_Skill_Available;

	public bool IsHunterStun => isHunterStun;

	public HunterCharacter HunterCharacter => hunterCharacter;

	public void Init(int _hunter_arr_idx, HunterCharacter _hunterChacter, HunterLeaderSkill _hunterLeaderSkill, HunterInfo _hunter_Info = null)
	{
		if (hunter_Info != null)
		{
			hunter_Info = null;
			hunter_Info = new HunterInfo();
		}
		for (int i = 0; i < hunter_Tier_tr.childCount; i++)
		{
			hunter_Tier_tr.GetChild(i).gameObject.SetActive(value: false);
		}
		if (stunEff != null)
		{
			UnityEngine.Debug.Log("Return Stun !");
			MWPoolManager.DeSpawn("Effect", stunEff);
			stunEff = null;
		}
		OnCallback_Hunter_Attack = null;
		OnCallback_Combo_Eff = null;
		OnCallback_Skill_End = null;
		hunter_Block_Count = 0;
		hunter_Total_Damage = 0;
		hunter_Skill_Gauge_Sprite.localScale = SetVector3Scale("x", hunter_Skill_Gauge_Sprite, 0f);
		hunter_Skill_Gauge_Value = 0;
		isHunterStun = false;
		isHunterStunClearCount = 0;
		isHunter_Skill_Available = false;
		hunter_Arr_Idx = _hunter_arr_idx;
		hunter_Skill_Ready_Eff.SetActive(value: false);
		user_Bonus_Attack = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).attackBonusAll + 1f;
		if (_hunter_Info != null)
		{
			hunter_Info = _hunter_Info;
			hunter_type = (HUNTER_TYPE)_hunter_Info.Hunter.color;
			hunter_tribe = (HUNTER_TRIBE)_hunter_Info.Hunter.hunterTribe;
			hunter_Skill_Gauge_Full_Value = hunter_Info.Skill.skillGauge;
		}
		hunterCharacter = _hunterChacter;
		hunterLeaderSkill = _hunterLeaderSkill;
		hunterCharacter.Init(this);
		SetHunterFace();
		hunter_Tier_tr.GetChild(hunter_Info.Stat.hunterTier - 1).gameObject.SetActive(value: true);
	}

	public void SetHunterState(HUNTER_STATE _state)
	{
		switch (_state)
		{
		case HUNTER_STATE.ATTACK:
			hunterState = HUNTER_STATE.ATTACK;
			break;
		case HUNTER_STATE.IDLE:
			hunterState = HUNTER_STATE.IDLE;
			break;
		}
	}

	public void SetCharacterAttackAnim(Anim_Type _type, Monster _monster, int _damage, Action _onCallBack)
	{
		if (!(_monster == null))
		{
			UnityEngine.Debug.Log("** 11");
			SetHunterState(HUNTER_STATE.ATTACK);
			hunterCharacter.SetTweenMonster(_monster);
			attackMonster = _monster;
			attackDamage = _damage;
			OnCallback_CharacterAttack_End = _onCallBack;
		}
	}

	public void SetCharacterSkillAnim(HunterSkillRange _range, Monster[] _monster, Action _OnCallback)
	{
		if (_monster != null)
		{
			hunterCharacter.SetTweenSkill(_monster[UnityEngine.Random.Range(0, _monster.Length)]);
		}
		StartCoroutine(Use_Hunter_Skill(_range, _monster, _OnCallback));
	}

	public void SetMonsterHP_Gauge()
	{
		Transform transform = null;
		transform = MWPoolManager.Spawn("Effect", "Fx_hit01", null, 1.5f);
		transform.position = attackMonster.HitAnchor;
		attackMonster.SetMonsterHP_Gauge(attackDamage);
		GameUtil.Check_Property_Damage_UI(this, attackMonster, attackDamage);
	}

	public void EndCharacterAttack()
	{
		UnityEngine.Debug.Log("** 22 = " + hunterState);
		OnCallback_CharacterAttack_End();
	}

	public void SetArenaBuff()
	{
		UnityEngine.Debug.Log("SetArenaBuff !!");
		InGamePlayManager.ShowBuffUI(arenaBuffTextAnchor.position, (BlockType)hunter_Info.Hunter.color, CheckArenaBuff(hunter_Info));
	}

	public void SetHunterInfo(HunterInfo _info)
	{
		hunter_Info = _info;
	}

	public void Add_Attack_Start()
	{
		hunter_Block_Count = 0;
		hunter_Total_Damage = 0;
	}

	public void Add_Attack_Damage(int _blockCount)
	{
		hunter_Block_Count += _blockCount - 2;
		hunter_Total_Damage = (int)((GameUtil.GetHunterReinForceAttack(hunter_Info.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunter_Info.Hunter.hunterIdx)) * (float)CheckArenaBuff(hunter_Info) + (float)hunter_Info.leaderSkillAttack) * user_Bonus_Attack * (float)hunter_Block_Count * (float)Damage_Dummy_constX);
		InGamePlayManager.AddHunterAttack(hunter_Total_Damage, hunter_Info.Hunter.color, base.transform.position, hunter_Info.Hunter.hunterIdx);
	}

	public void Add_Attack_Combo(int _combo, int _color, int _lastAttackIdx, Action<int> _OnCallBack)
	{
		StartCoroutine(Add_Combo_Effect(_combo, _color, _lastAttackIdx, _OnCallBack));
	}

	public void Add_Skill_Gauge(int _count)
	{
		if (hunter_Skill_Gauge_Full_Value <= hunter_Skill_Gauge_Value)
		{
			hunter_Skill_Gauge_Value = hunter_Skill_Gauge_Full_Value;
			if (!isHunter_Skill_Available)
			{
				isHunter_Skill_Available = true;
			}
			return;
		}
		hunter_Skill_Gauge_Value += _count;
		if (hunter_Skill_Gauge_Value >= hunter_Skill_Gauge_Full_Value)
		{
			hunter_Skill_Gauge_Value = hunter_Skill_Gauge_Full_Value;
			if (!isHunter_Skill_Available)
			{
				isHunter_Skill_Available = true;
			}
		}
		hunter_Skill_Gauge_Sprite.localScale = SetVector3Scale("x", hunter_Skill_Gauge_Sprite, (float)hunter_Skill_Gauge_Value * (1f / (float)hunter_Skill_Gauge_Full_Value));
	}

	public void SetHunterSkillFullValue()
	{
		Add_Skill_Gauge(hunter_Skill_Gauge_Full_Value);
	}

	public void Add_Attack_End()
	{
	}

	public void Attack_Start(Action<int, int> _OnCallback)
	{
		OnCallback_Hunter_Attack = _OnCallback;
		OnCallback_Hunter_Attack(hunter_Total_Damage, hunter_Arr_Idx);
		OnCallback_Hunter_Attack = null;
	}

	public IEnumerator Use_Hunter_Skill(HunterSkillRange _range, Monster[] _monster, Action _OnCallback)
	{
		if (hunter_Info.Skill.motionType == 1)
		{
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate);
		}
		Transform skillEff = null;
		switch (hunter_Info.Hunter.color)
		{
		case 0:
			skillEff = MWPoolManager.Spawn("Skill", "FX_Blue_skill", null, 1f);
			break;
		case 1:
			skillEff = MWPoolManager.Spawn("Skill", "FX_Green_skill", null, 1f);
			break;
		case 3:
			skillEff = MWPoolManager.Spawn("Skill", "FX_Red_skill", null, 1f);
			break;
		case 4:
			skillEff = MWPoolManager.Spawn("Skill", "FX_Yellow_skill", null, 1f);
			break;
		case 2:
			skillEff = MWPoolManager.Spawn("Skill", "FX_Purple_skill", null, 1f);
			break;
		}
		SoundController.HunterSkillCutPlay(hunter_Info.Hunter.hunterIdx);
		skillEff.localScale = Vector3.one;
		skillEff.position = Vector3.zero;
		skillEff.GetComponent<HunterSkillEffect>().Init(this);
		yield return new WaitForSeconds(1f);
		OnCallback_Skill_End = _OnCallback;
		hunter_Skill_Gauge_Value = 0;
		hunter_Skill_Gauge_Sprite.localScale = SetVector3Scale("x", hunter_Skill_Gauge_Sprite, (float)hunter_Skill_Gauge_Value * (1f / (float)hunter_Skill_Gauge_Full_Value));
		isHunter_Skill_Available = false;
		hunter_Skill_Ready_Eff.SetActive(value: false);
		SoundController.HunterSkillSound(HunterInfo.Skill.skillIdx);
		UnityEngine.Debug.Log("************ this.hunter_Info.Skill.skillType = " + hunter_Info.Skill.skillType);
		switch (hunter_Info.Skill.skillType)
		{
		case 1:
			StartCoroutine(Use_Hunter_Skill_Setting(_range, _monster));
			break;
		case 2:
			StartCoroutine(Use_Hunter_Skill_Setting(_range, _monster));
			InGamePlayManager.Heal((int)((float)InGamePlayManager.GetHunterTotalHP() * hunter_Info.Skill.recPowers));
			break;
		case 3:
			UnityEngine.Debug.Log("***********STUN 11");
			StartCoroutine(Use_Hunter_Skill_Setting(_range, _monster));
			if (_range == HunterSkillRange.SINGLE)
			{
				_monster[0].SetStun();
				break;
			}
			for (int i = 0; i < _monster.Length; i++)
			{
				_monster[i].SetStun();
			}
			break;
		case 4:
			InGamePlayManager.ChangeBlockType((BlockType)hunter_Info.Skill.beforeBlock, (BlockType)hunter_Info.Skill.afterBlock, hunter_Info.Skill.skillIdx);
			StartCoroutine(SkillEndDelay());
			break;
		}
	}

	public IEnumerator Use_Hunter_Skill_Setting(HunterSkillRange _range, Monster[] _monster)
	{
		int _damage = 0;
		Transform[] _eff = new Transform[hunter_Info.Skill.times];
		float _eff_delay = 0.2f;
		for (int attackcount = 0; attackcount < hunter_Info.Skill.times; attackcount++)
		{
			switch (hunter_Info.Skill.statType)
			{
			case 1:
				_damage = (int)((GameUtil.GetHunterReinForceHP(hunter_Info.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(hunter_Info.Hunter.hunterIdx)) + (float)hunter_Info.leaderSkillHp) * hunter_Info.Skill.multiple);
				break;
			case 2:
				_damage = (int)((GameUtil.GetHunterReinForceAttack(hunter_Info.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(hunter_Info.Hunter.hunterIdx)) * (float)CheckArenaBuff(hunter_Info) + (float)hunter_Info.leaderSkillAttack) * hunter_Info.Skill.multiple);
				break;
			case 3:
				_damage = (int)((GameUtil.GetHunterReinForceHeal(hunter_Info.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(hunter_Info.Hunter.hunterIdx)) + (float)hunter_Info.leaderSkillRecovery) * hunter_Info.Skill.multiple);
				break;
			}
			for (int i = 0; i < _monster.Length; i++)
			{
				GameUtil.Check_Property_Damage(this, _monster[i], _damage);
				_monster[i].SetMonsterHP(_damage);
			}
			_eff[attackcount] = MWPoolManager.Spawn("Effect", "Skill_" + hunter_Info.Skill.skillIdx, null, 1.5f);
			if (_range == HunterSkillRange.SINGLE)
			{
				_eff[attackcount].position = _monster[0].SkillHitAnchor;
			}
			else
			{
				_eff[attackcount].position = Vector3.zero;
			}
			_eff[attackcount].GetChild(0).GetComponent<SkillEffect_Anim>().SetMonster(this, _monster, _damage);
			if (hunter_Info.Skill.times > 1)
			{
				yield return new WaitForSeconds(_eff_delay);
			}
			else
			{
				yield return null;
			}
		}
		yield return new WaitForSeconds(1f);
		OnCallback_Skill_End();
		OnCallback_Skill_End = null;
	}

	public void Hunter_Skill_Ready_Effect_Setting(bool _isOn)
	{
		if (_isOn)
		{
			if (isHunter_Skill_Available)
			{
				hunter_Skill_Ready_Eff.SetActive(value: true);
				SoundController.EffectSound_Play(EffectSoundType.HunterSkillReady);
			}
			else
			{
				hunter_Skill_Ready_Eff.SetActive(value: false);
			}
		}
		else
		{
			hunter_Skill_Ready_Eff.SetActive(value: false);
		}
	}

	public void SetHunterStun()
	{
		if (stunEff != null)
		{
			UnityEngine.Debug.Log("Return Stun !");
			MWPoolManager.DeSpawn("Effect", stunEff);
			stunEff = null;
		}
		stunEff = MWPoolManager.Spawn("Effect", "FX_stun_hunter", base.transform);
		stunEff.position = base.transform.position;
		isHunterStun = true;
		isHunterStunClearCount = 1;
		hunterCharacter.SetAnim(Anim_Type.STUN);
		InGamePlayManager.DeActiveBlock((BlockType)hunter_Info.Hunter.color);
	}

	public void ClearHunterStun()
	{
		if (isHunterStunClearCount > 0)
		{
			isHunterStunClearCount--;
			return;
		}
		isHunterStun = false;
		hunterCharacter.SetAnim(Anim_Type.IDLE);
		InGamePlayManager.ActiveBlock((BlockType)hunter_Info.Hunter.color);
		if (stunEff != null)
		{
			UnityEngine.Debug.Log("Return Stun !");
			MWPoolManager.DeSpawn("Effect", stunEff);
			stunEff = null;
		}
	}

	private int CheckArenaBuff(HunterInfo _hunterinfo)
	{
		int num = 1;
		if (GameInfo.inGamePlayData.inGameType != 0)
		{
			if (GameInfo.inGamePlayData.arenaInfo == null)
			{
				return num;
			}
			if (GameInfo.inGamePlayData.arenaInfo.color == _hunterinfo.Hunter.color)
			{
				num *= GameInfo.inGamePlayData.arenaInfo.color_buff;
			}
			if (GameInfo.inGamePlayData.arenaInfo.tribe == _hunterinfo.Hunter.hunterTribe)
			{
				num *= GameInfo.inGamePlayData.arenaInfo.tribe_buff;
			}
		}
		return num;
	}

	private IEnumerator Add_Combo_Effect(int _combo, int _color, int _lastAttackIdx, Action<int> _OnCallBack)
	{
		OnCallback_Combo_Eff = _OnCallBack;
		yield return null;
		if (_combo > 0 && hunter_Total_Damage > 0)
		{
			float comboDuration = 0.24f;
			for (int i = 0; i < _combo; i++)
			{
				base.transform.localScale = Vector3.one;
				InGamePlayManager.AddHunterCombo(i + 1, hunter_Info.Hunter.color, base.transform.position, hunter_Info.Hunter.hunterIdx);
				if (i > 0)
				{
					hunter_Total_Damage += (int)((float)hunter_Total_Damage * Damage_Dummy_Combo);
				}
				LeanTween.cancel(base.transform.gameObject);
				LeanTween.scale(base.transform.gameObject, new Vector3(1.2f, 1.2f, 1.2f), comboDuration / 2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
				InGamePlayManager.AddHunterAttack(hunter_Total_Damage, hunter_Info.Hunter.color, base.transform.position, hunter_Info.Hunter.hunterIdx);
				if (hunter_Arr_Idx == _lastAttackIdx)
				{
					SoundController.EffectSound_Play(EffectSoundType.ComboAdd);
				}
				yield return new WaitForSeconds(comboDuration);
				yield return null;
				base.transform.localScale = Vector3.one;
				if (comboDuration > 0.1f)
				{
					comboDuration -= 0.02f;
				}
			}
			hunter_Total_Damage = hunterLeaderSkill.CheckLeaderSkillComboSetting(_combo, hunter_Total_Damage);
			hunter_Total_Damage = hunterLeaderSkill.CheckLeaderSkillColorSetting(_color, hunter_Total_Damage);
			InGamePlayManager.AddHunterAttack(hunter_Total_Damage, hunter_Info.Hunter.color, base.transform.position, hunter_Info.Hunter.hunterIdx);
		}
		OnCallback_Combo_Eff(hunter_Arr_Idx);
		OnCallback_Combo_Eff = null;
	}

	private void SetHunterFace()
	{
		for (int i = 0; i < hunterFace.childCount; i++)
		{
			hunterFace.GetChild(i).gameObject.SetActive(value: false);
		}
		switch (hunter_Info.Stat.hunterTier)
		{
		case 1:
			hunterFace.GetChild(int.Parse(hunter_Info.Hunter.hunterImg1) - 1).gameObject.SetActive(value: true);
			break;
		case 2:
			hunterFace.GetChild(int.Parse(hunter_Info.Hunter.hunterImg2) - 1).gameObject.SetActive(value: true);
			break;
		case 3:
			hunterFace.GetChild(int.Parse(hunter_Info.Hunter.hunterImg3) - 1).gameObject.SetActive(value: true);
			break;
		case 4:
			hunterFace.GetChild(int.Parse(hunter_Info.Hunter.hunterImg4) - 1).gameObject.SetActive(value: true);
			break;
		case 5:
			hunterFace.GetChild(int.Parse(hunter_Info.Hunter.hunterImg5) - 1).gameObject.SetActive(value: true);
			break;
		}
	}

	private Vector3 SetVector3Scale(string _type, Transform _tr, float _scale)
	{
		Vector3 result = Vector3.one;
		switch (_type)
		{
		case "x":
		{
			Vector3 localScale5 = _tr.localScale;
			float y = localScale5.y;
			Vector3 localScale6 = _tr.localScale;
			result = new Vector3(_scale, y, localScale6.z);
			break;
		}
		case "y":
		{
			Vector3 localScale3 = _tr.localScale;
			float x2 = localScale3.x;
			Vector3 localScale4 = _tr.localScale;
			result = new Vector3(x2, _scale, localScale4.z);
			break;
		}
		case "z":
		{
			Vector3 localScale = _tr.localScale;
			float x = localScale.x;
			Vector3 localScale2 = _tr.localScale;
			result = new Vector3(x, localScale2.y, _scale);
			break;
		}
		}
		return result;
	}

	private IEnumerator SkillEndDelay()
	{
		yield return new WaitForSeconds(1f);
		OnCallback_Skill_End();
		OnCallback_Skill_End = null;
	}

	private void OnDisable()
	{
		Animator[] componentsInChildren = base.transform.GetComponentsInChildren<Animator>(includeInactive: true);
		foreach (Animator animator in componentsInChildren)
		{
			animator.speed = 1f;
		}
	}
}
