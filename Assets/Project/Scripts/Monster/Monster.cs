using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

public class Monster : Monster_Base
{
	[SerializeField]
	private MonsterDbData monster_Info;

	[SerializeField]
	private MonsterStatDbData monster_Stat;

	[SerializeField]
	private MonsterSkillDbData monster_Skill;

	[SerializeField]
	private Transform monster_HP_Gauge_Sprite;

	[SerializeField]
	private int monster_HP_Gauge_Value;

	[SerializeField]
	private int monster_HP_Gauge_Sprite_Value;

	[SerializeField]
	private GameObject[] monster_Attack_Turn_Sprite;

	[SerializeField]
	private int monster_Attack_Turn_Value;

	[SerializeField]
	private int killMonster;

	[SerializeField]
	private bool isTarget;

	[SerializeField]
	private bool isStun;

	[SerializeField]
	private bool isDying;

	[SerializeField]
	private bool isItemGet;

	[SerializeField]
	private int skillInterval;

	[SerializeField]
	private Animator monster_Anim;

	[SerializeField]
	private Transform stunEff;

	[SerializeField]
	private int currentDamage;

	[SerializeField]
	private Transform monsterAttributeUI;

	[SerializeField]
	private Transform monsterGaugeUI;

	[SerializeField]
	private Transform monsterTurnUI;

	[SerializeField]
	private Transform attackAnchor;

	[SerializeField]
	private Transform hitAnchor;

	[SerializeField]
	private Transform skillHitAnchor;

	[SerializeField]
	private Transform dragonSkillAnchor;

	private MONSTER_STATE monsterState;

	private Vector3 origin_Pos;

	private int origin_Depth;

	private Hero attack_Hunter;

	private Action OnCallback_CharacterAttack_End;

	public MonsterDbData MonsterInfo => monster_Info;

	public MonsterStatDbData MonsterStat => monster_Stat;

	public MONSTER_STATE MonsterState => monsterState;

	public MonsterSkillDbData MonsterSkill => monster_Skill;

	public Transform HP_GaugeBar => monsterGaugeUI;

	public int SkillInterval => skillInterval;

	public int MonsterHP => monster_HP_Gauge_Value;

	public int MonsterGaugeHP => monster_HP_Gauge_Sprite_Value;

	public int MonsterDamage => monster_Stat.mDamageAttack;

	public int MonsterCurrentDamage => currentDamage;

	public bool MonsterTarget => isTarget;

	public int MonsterTurn => monster_Attack_Turn_Value;

	//public SkeletonAnimator MonsterAnim => monster_Anim.GetComponent<SkeletonAnimator>();
    public SkeletonAnimation MonsterAnim => monster_Anim.GetComponent<SkeletonAnimation>();


    public Vector3 AttackAnchor => attackAnchor.position;

	public Vector3 HitAnchor => hitAnchor.position;

	public Vector3 SkillHitAnchor => skillHitAnchor.position;

	public bool IsStun => isStun;

    private void Awake()
    {
        if (monster_Anim == null) monster_Anim = GetComponentInChildren<Animator>();
    }
    public void Init(int _monsterSettingTurn, int _monster_DropItem, MonsterStatDbData _monster_Data, int _sortIdx)
	{
		if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Arena)
		{
			MonsterUIOnOff(_onoff: true);
		}
		monster_Stat = _monster_Data;
		if (stunEff != null)
		{
			MasterPoolManager.ReturnToPool("Effect", stunEff);
			stunEff = null;
		}
		monster_Info = GameDataManager.GetMonsterData(monster_Stat.mIdx);
		monster_Skill = GameDataManager.GetMonsterSkillData(monster_Stat.mSkillIdx);
		isTarget = false;
		isDying = false;
		isItemGet = false;
		skillInterval = 0;
		monster_Attack_Turn_Value = _monsterSettingTurn;
		killMonster = _monster_DropItem;
		monster_HP_Gauge_Value = monster_Stat.mHp;
		monster_HP_Gauge_Sprite_Value = monster_Stat.mHp;
		SetMonsterCharacterImg(_sortIdx);
		SetMonsterAttackTurn();
		SetMonsterHP();
		SetMonsterHP_Gauge();
		SetAnim(Anim_Type.IDLE);
	}

	private void SetMonsterCharacterImg(int _sortIdx)
	{
		MonsterAnim.initialSkinName = monster_Stat.mMonsterImg;
		MonsterAnim.Initialize(overwrite: true);
		monster_Anim.GetComponent<MeshRenderer>().sortingOrder = _sortIdx;
	}

	public void SetMonsterCurrentDamage(int _damage)
	{
		currentDamage = _damage;
	}

	public void SetMonsterAttackTurnOnly(int _minus = 0)
	{
		if (!isStun)
		{
			monster_Attack_Turn_Value -= _minus;
		}
	}

	public void SetMonsterAttackTurn(int _minus = 0)
	{
		if (isStun)
		{
			if (stunEff != null)
			{
				MasterPoolManager.ReturnToPool("Effect", stunEff);
				stunEff = null;
			}
			isStun = false;
			SetAnim(Anim_Type.IDLE);
			return;
		}
		monster_Attack_Turn_Value -= _minus;
		if (monster_Attack_Turn_Value <= 0)
		{
			return;
		}
		for (int i = 0; i < monster_Attack_Turn_Sprite.Length; i++)
		{
			if (monster_Attack_Turn_Value == i + 1)
			{
				monster_Attack_Turn_Sprite[i].SetActive(value: true);
			}
			else
			{
				monster_Attack_Turn_Sprite[i].SetActive(value: false);
			}
		}
	}

	public Transform GetMonsterTurn()
	{
		return monster_Attack_Turn_Sprite[0].transform.parent;
	}

	public void SetMonsterAttackTurnRefresh()
	{
		monster_Attack_Turn_Value = monster_Stat.mTurnsAttack;
		SetMonsterAttackTurn();
	}

	public void AddMonsterHP(int _healHP)
	{
		if (monster_HP_Gauge_Value + _healHP > monster_Stat.mHp)
		{
			monster_HP_Gauge_Value = monster_Stat.mHp;
			monster_HP_Gauge_Sprite_Value = monster_Stat.mHp;
		}
		else
		{
			monster_HP_Gauge_Value += _healHP;
			monster_HP_Gauge_Sprite_Value += _healHP;
		}
		monster_HP_Gauge_Sprite.localScale = SetVector3Scale("x", monster_HP_Gauge_Sprite, (float)monster_HP_Gauge_Sprite_Value * (1f / (float)monster_Stat.mHp));
	}

	public void ForceMonsterHp(int _hp)
	{
		monster_HP_Gauge_Value = _hp;
	}

	public void SetMonsterHP(int _damage = 0)
	{
		UnityEngine.Debug.Log("SET_MONSTER_HP");
		monster_HP_Gauge_Value -= _damage;
		if (monster_HP_Gauge_Value < 0)
		{
			monster_HP_Gauge_Value = 0;
		}
	}

	public void SetMonsterHP_Gauge(int _damage = 0)
	{
		UnityEngine.Debug.Log("SET_MONSTER_HP_GAUGE");
		SetDieEffect();
		monster_HP_Gauge_Sprite_Value -= _damage;
		if (monster_HP_Gauge_Value != monster_HP_Gauge_Sprite_Value)
		{
			SetHPAndGauge_Equal();
		}
		if (monster_HP_Gauge_Sprite_Value <= 0)
		{
			monster_HP_Gauge_Sprite_Value = 0;
			if (_damage > 0)
			{
				SetAnim(Anim_Type.DEATH);
				SoundController.EffectSound_Play(EffectSoundType.MonsterHit);
				if (!isDying)
				{
					isDying = true;
					PuzzlePlayManager.MoreMonster(killMonster);
				}
			}
		}
		else if (_damage > 0)
		{
			SetAnim(Anim_Type.DAMAGE);
			SoundController.EffectSound_Play(EffectSoundType.MonsterHit);
		}
		monster_HP_Gauge_Sprite.localScale = SetVector3Scale("x", monster_HP_Gauge_Sprite, (float)monster_HP_Gauge_Sprite_Value * (1f / (float)monster_Stat.mHp));
	}

	public void SetHPAndGauge_Equal()
	{
		monster_HP_Gauge_Sprite_Value = monster_HP_Gauge_Value;
		monster_HP_Gauge_Sprite.localScale = SetVector3Scale("x", monster_HP_Gauge_Sprite, (float)monster_HP_Gauge_Sprite_Value * (1f / (float)monster_Stat.mHp));
	}

	public void End_Die_Anim()
	{
		Transform transform = base.transform;
		Vector3 localPosition = base.transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = base.transform.localPosition;
		float y = localPosition2.y + 2000f;
		Vector3 localPosition3 = base.transform.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition3.z);
		if (isTarget)
		{
			PuzzlePlayManager.MonsterUnTargeting();
		}
	}

	public void SetMonsterTargeting(bool _istarget)
	{
		if (isTarget)
		{
		}
		if (_istarget)
		{
			UnityEngine.Debug.Log("MONSTER TARGETING true =  " + base.gameObject.name);
			isTarget = true;
			SoundController.EffectSound_Play(EffectSoundType.MonsterTargeting);
		}
		else
		{
			UnityEngine.Debug.Log("MONSTER TARGETING false =  " + base.gameObject.name);
			isTarget = false;
		}
	}

	public void ResetSkillInterval()
	{
		skillInterval = monster_Skill.mSkillInterval;
	}

	public void SkillIntervalDecrease()
	{
		skillInterval--;
	}

	public void SetStun()
	{
		if (stunEff != null)
		{
			MasterPoolManager.ReturnToPool("Effect", stunEff);
			stunEff = null;
		}
		UnityEngine.Debug.Log("***********STUN 22");
		SoundController.EffectSound_Play(EffectSoundType.MonsterStun);
		stunEff = MasterPoolManager.SpawnObject("Effect", "FX_stun", base.transform);
		if (GameInfo.inGamePlayData.isDragon == 0)
		{
			Transform transform = stunEff;
			Vector3 localPosition = monster_Attack_Turn_Sprite[0].transform.parent.localPosition;
			transform.localPosition = new Vector3(0f, localPosition.y, 0f);
		}
		else
		{
			stunEff.localPosition = new Vector3(-1.8f, 3.5f, 0f);
		}
		SetAnim(Anim_Type.STUN);
		isStun = true;
	}

	public void ClearStun()
	{
		if (stunEff != null)
		{
			MasterPoolManager.ReturnToPool("Effect", stunEff);
			stunEff = null;
			isStun = false;
		}
	}

	public void SetAnim(Anim_Type _type)
	{
		switch (_type)
		{
		case Anim_Type.SKILL:
		case Anim_Type.SKILLEFFECT:
		case Anim_Type.MOVE:
			break;
		case Anim_Type.IDLE:
			monster_Anim.ResetTrigger("Idle");
			monster_Anim.SetTrigger("Idle");
			break;
		case Anim_Type.ATTACK_HUNTER:
			monster_Anim.ResetTrigger("Attack_Hunter");
			monster_Anim.SetTrigger("Attack_Hunter");
			break;
		case Anim_Type.ATTACK_MONSTER:
			monster_Anim.ResetTrigger("Attack_Monster");
			monster_Anim.SetTrigger("Attack_Monster");
			break;
		case Anim_Type.DAMAGE:
			monster_Anim.ResetTrigger("Damage");
			monster_Anim.SetTrigger("Damage");
			break;
		case Anim_Type.STUN:
			monster_Anim.ResetTrigger("Stun");
			monster_Anim.SetTrigger("Stun");
			break;
		case Anim_Type.DEATH:
			monster_Anim.ResetTrigger("Death");
			monster_Anim.SetTrigger("Death");
			break;
		}
	}

	public void SetCharacterAnim(Anim_Type _type, Hero _hunter, Action _onCallBack)
	{
		if (!(_hunter == null))
		{
			OnCallback_CharacterAttack_End = _onCallBack;
			monsterState = MONSTER_STATE.ATTACK;
			origin_Pos = base.transform.position;
			origin_Depth = monster_Anim.GetComponent<MeshRenderer>().sortingOrder;
			attack_Hunter = _hunter;
			StartCoroutine(SetCharacterAnim_Coroutine(_hunter));
			if (GameInfo.inGamePlayData.isDragon == 1)
			{
				StartCoroutine(DragonFireBall());
			}
		}
	}

	public void End_Attack_Anim()
	{
		SetAnim(Anim_Type.IDLE);
		monsterState = MONSTER_STATE.IDLE;
		OnCallback_CharacterAttack_End();
		StartCoroutine(End_Attack_Anim_Coroutine());
	}

	public void SetAttackHunterDamageAnim()
	{
		attack_Hunter.HunterCharacter.ChangeAnim(Anim_Type.DAMAGE);
	}

	public void MonsterUIOnOff(bool _onoff)
	{
		if (_onoff)
		{
			monsterAttributeUI.gameObject.SetActive(value: true);
			monsterGaugeUI.gameObject.SetActive(value: true);
			monsterTurnUI.gameObject.SetActive(value: true);
		}
		else
		{
			monsterAttributeUI.gameObject.SetActive(value: false);
			monsterGaugeUI.gameObject.SetActive(value: false);
			monsterTurnUI.gameObject.SetActive(value: false);
		}
	}

	public void SetMonsterState(MONSTER_STATE _state)
	{
		switch (_state)
		{
		case MONSTER_STATE.ATTACK:
			monsterState = MONSTER_STATE.ATTACK;
			break;
		case MONSTER_STATE.IDLE:
			monsterState = MONSTER_STATE.IDLE;
			break;
		}
	}

	private void SetDieEffect()
	{
		Transform transform = null;
		Transform transform2 = null;
		int num = 0;
		if (monster_HP_Gauge_Value != 0 || isItemGet)
		{
			return;
		}
		isItemGet = true;
		PuzzlePlayManager.LastCoinEffect();
		ClearStun();
		if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Stage)
		{
			transform = MasterPoolManager.SpawnObject("Effect", "Fx_ItemDrop", null, 2f);
			transform.position = base.transform.position;
			SoundController.EffectSound_Play(EffectSoundType.MonsterItemDrop);
			num = UnityEngine.Random.Range(1, 101);
			if (num <= GameDataManager.GetGameConfigData(ConfigDataType.DropKey) && GameInfo.inGamePlayData.levelIdx > 4)
			{
				transform2 = MasterPoolManager.SpawnObject("Effect", "Fx_KeyDrop", null, 2f);
				transform2.position = base.transform.position;
				GameInfo.userPlayData.AddChestKey();
				SoundController.EffectSound_Play(EffectSoundType.GetKeyIngame);
			}
		}
	}

	private IEnumerator SetCharacterAnim_Coroutine(Hero _hunter)
	{
		if (monster_Info.motionType == 1)
		{
			if (isTarget)
			{
				PuzzlePlayManager.MonsterUnTargeting();
			}
			SetAnim(Anim_Type.MOVE);
			GameObject gameObject = base.transform.gameObject;
			Vector3 position = _hunter.HunterCharacter.transform.position;
			float x = position.x;
			Vector3 vector = _hunter.HunterCharacter.AttackAnchor;
			float x2 = vector.x;
			Vector3 position2 = _hunter.HunterCharacter.transform.position;
			float num = x2 - position2.x;
			Vector3 position3 = base.transform.position;
			float x3 = position3.x;
			Vector3 vector2 = AttackAnchor;
			LeanTween.moveX(gameObject, x + (num + (x3 - vector2.x)), 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
			GameObject gameObject2 = base.transform.gameObject;
			Vector3 position4 = _hunter.HunterCharacter.transform.position;
			LeanTween.moveY(gameObject2, position4.y, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			monster_Anim.GetComponent<MeshRenderer>().sortingOrder = _hunter.HunterCharacter.HunterAnim.GetComponent<MeshRenderer>().sortingOrder;
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			SetAnim(Anim_Type.ATTACK_MONSTER);
		}
		else
		{
			SetAnim(Anim_Type.ATTACK_MONSTER);
		}
		yield return null;
	}

	private IEnumerator End_Attack_Anim_Coroutine()
	{
		if (monster_Info.motionType == 1)
		{
			LeanTween.moveX(base.transform.gameObject, origin_Pos.x, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseOutQuint();
			LeanTween.moveY(base.transform.gameObject, origin_Pos.y, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseOutQuint();
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			monster_Anim.GetComponent<MeshRenderer>().sortingOrder = origin_Depth;
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			if (isTarget)
			{
				PuzzlePlayManager.MonsterTargeting(this);
			}
		}
		yield return null;
	}

	private IEnumerator DragonFireBall()
	{
		yield return new WaitForSeconds(0.7f / GameInfo.inGameBattleSpeedRate);
		Transform _eff_ball = MasterPoolManager.SpawnObject("Effect", "Skill_Dragon_fireball", null, 0.2f / GameInfo.inGameBattleSpeedRate);
		_eff_ball.position = dragonSkillAnchor.position;
		LeanTween.moveX(_eff_ball.gameObject, -2.5f, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
		LeanTween.moveY(_eff_ball.gameObject, 1.5f, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate);
		Transform _eff_hit = MasterPoolManager.SpawnObject("Effect", "Skill_Dragon", null, 1f);
		_eff_hit.position = Vector3.zero;
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

	private void OnDisable()
	{
		Animator[] componentsInChildren = base.transform.GetComponentsInChildren<Animator>(includeInactive: true);
		foreach (Animator animator in componentsInChildren)
		{
			animator.speed = 1f;
		}
	}
}
