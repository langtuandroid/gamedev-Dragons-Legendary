using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : EnemyBase
{
	[FormerlySerializedAs("monster_HP_Gauge_Sprite")] [SerializeField]
	private Transform _monsterHpGaugeSprite;

	[FormerlySerializedAs("monster_HP_Gauge_Value")] [SerializeField]
	private int _monsterHpGaugeValue;

	[FormerlySerializedAs("monster_HP_Gauge_Sprite_Value")] [SerializeField]
	private int _monsterHpGaugeSpriteValue;

	[FormerlySerializedAs("monster_Attack_Turn_Sprite")] [SerializeField]
	private GameObject[] _monsterAttackTurnSprite;
	
	[FormerlySerializedAs("monster_Anim")] [SerializeField]
	private Animator _monsterAnim;
	
	[FormerlySerializedAs("monsterAttributeUI")] [SerializeField]
	private Transform _monsterAttributeUI;

	[FormerlySerializedAs("monsterGaugeUI")] [SerializeField]
	private Transform _monsterGaugeUI;

	[FormerlySerializedAs("monsterTurnUI")] [SerializeField]
	private Transform _monsterTurnUI;

	[FormerlySerializedAs("attackAnchor")] [SerializeField]
	private Transform _attackAnchor;

	[FormerlySerializedAs("hitAnchor")] [SerializeField]
	private Transform _hitAnchor;

	[FormerlySerializedAs("skillHitAnchor")] [SerializeField]
	private Transform _skillHitAnchor;

	[FormerlySerializedAs("dragonSkillAnchor")] [SerializeField]
	private Transform _dragonSkillAnchor;

	private EnemyState _monsterState;
	private Vector3 _originPos;
	private int _originDepth;
	private Hero _attackHunter;
	private Action onCharacterAttackEnd;
	private EnemyDbData _monsterInfo;
	private EnemyStatDbData _monsterStat;
	private EnemySkillDbData _monsterSkillDbData;
	private int _monsterAttackTurnValue;
	private int _killMonster;
	private bool _isTarget;
	private bool _isStun;
	private bool _isDying;
	private bool _isItemGet;
	private int _skillInterval;
	private Transform _stunEff;
	private int _currentDamage;

	public EnemyDbData MonsterInfo => _monsterInfo;

	public EnemyStatDbData MonsterStat => _monsterStat;

	public EnemySkillDbData MonsterSkillDbData => _monsterSkillDbData;

	public Transform HP_GaugeBar => _monsterGaugeUI;

	public int SkillInterval => _skillInterval;

	public int MonsterHP => _monsterHpGaugeValue;

	public int MonsterGaugeHP => _monsterHpGaugeSpriteValue;

	public int MonsterDamage => _monsterStat.mDamageAttack;

	public int MonsterCurrentDamage => _currentDamage;

	public bool MonsterTarget => _isTarget;

	public int MonsterTurn => _monsterAttackTurnValue;
	
    public SkeletonAnimation MonsterAnim => _monsterAnim.GetComponent<SkeletonAnimation>();

    public Vector3 AttackAnchor => _attackAnchor.position;

	public Vector3 HitAnchor => _hitAnchor.position;

	public Vector3 SkillHitAnchor => _skillHitAnchor.position;

	public bool IsStun => _isStun;

    private void Awake()
    {
        if (_monsterAnim == null) _monsterAnim = GetComponentInChildren<Animator>();
    }
    public void Construct(int _monsterSettingTurn, int _monster_DropItem, EnemyStatDbData _monster_Data, int _sortIdx)
	{
		if (GameInfo.inGamePlayData.inGameType == PuzzleInGameType.Arena)
		{
			DisableMonsterUI(_onoff: true);
		}
		_monsterStat = _monster_Data;
		if (_stunEff != null)
		{
			MasterPoolManager.ReturnToPool("Effect", _stunEff);
			_stunEff = null;
		}
		_monsterInfo = GameDataManager.GetMonsterData(_monsterStat.mIdx);
		_monsterSkillDbData = GameDataManager.GetMonsterSkillData(_monsterStat.mSkillIdx);
		_isTarget = false;
		_isDying = false;
		_isItemGet = false;
		_skillInterval = 0;
		_monsterAttackTurnValue = _monsterSettingTurn;
		_killMonster = _monster_DropItem;
		_monsterHpGaugeValue = _monsterStat.mHp;
		_monsterHpGaugeSpriteValue = _monsterStat.mHp;
		SetImageMonster(_sortIdx);
		AttackTurn();
		SetMonsterHealth();
		SetMonsterHP_Gauge();
		ChangeAnim(Anim_Type.IDLE);
	}

	private void SetImageMonster(int _sortIdx)
	{
		MonsterAnim.initialSkinName = _monsterStat.mMonsterImg;
		MonsterAnim.Initialize(overwrite: true);
		_monsterAnim.GetComponent<MeshRenderer>().sortingOrder = _sortIdx;
	}

	public void SetDamage(int _damage)
	{
		_currentDamage = _damage;
	}

	public void SetMonsterAttack(int _minus = 0)
	{
		if (!_isStun)
		{
			_monsterAttackTurnValue -= _minus;
		}
	}

	public void AttackTurn(int _minus = 0)
	{
		if (_isStun)
		{
			if (_stunEff != null)
			{
				MasterPoolManager.ReturnToPool("Effect", _stunEff);
				_stunEff = null;
			}
			_isStun = false;
			ChangeAnim(Anim_Type.IDLE);
			return;
		}
		_monsterAttackTurnValue -= _minus;
		if (_monsterAttackTurnValue <= 0)
		{
			return;
		}
		for (int i = 0; i < _monsterAttackTurnSprite.Length; i++)
		{
			if (_monsterAttackTurnValue == i + 1)
			{
				_monsterAttackTurnSprite[i].SetActive(value: true);
			}
			else
			{
				_monsterAttackTurnSprite[i].SetActive(value: false);
			}
		}
	}

	public Transform GetMonsterTurn()
	{
		return _monsterAttackTurnSprite[0].transform.parent;
	}

	public void RefreshTurnAttack()
	{
		_monsterAttackTurnValue = _monsterStat.mTurnsAttack;
		AttackTurn();
	}

	public void HealMonster(int _healHP)
	{
		if (_monsterHpGaugeValue + _healHP > _monsterStat.mHp)
		{
			_monsterHpGaugeValue = _monsterStat.mHp;
			_monsterHpGaugeSpriteValue = _monsterStat.mHp;
		}
		else
		{
			_monsterHpGaugeValue += _healHP;
			_monsterHpGaugeSpriteValue += _healHP;
		}
		_monsterHpGaugeSprite.localScale = VecrorScale("x", _monsterHpGaugeSprite, (float)_monsterHpGaugeSpriteValue * (1f / (float)_monsterStat.mHp));
	}

	public void ForceMonsterHp(int _hp)
	{
		_monsterHpGaugeValue = _hp;
	}

	public void SetMonsterHealth(int _damage = 0)
	{
		UnityEngine.Debug.Log("SET_MONSTER_HP");
		_monsterHpGaugeValue -= _damage;
		if (_monsterHpGaugeValue < 0)
		{
			_monsterHpGaugeValue = 0;
		}
	}

	public void SetMonsterHP_Gauge(int _damage = 0)
	{
		UnityEngine.Debug.Log("SET_MONSTER_HP_GAUGE");
		DieEffect();
		_monsterHpGaugeSpriteValue -= _damage;
		if (_monsterHpGaugeValue != _monsterHpGaugeSpriteValue)
		{
			SetHP();
		}
		if (_monsterHpGaugeSpriteValue <= 0)
		{
			_monsterHpGaugeSpriteValue = 0;
			if (_damage > 0)
			{
				ChangeAnim(Anim_Type.DEATH);
				SoundController.EffectSound_Play(EffectSoundType.MonsterHit);
				if (!_isDying)
				{
					_isDying = true;
					PuzzlePlayManager.MoreMonster(_killMonster);
				}
			}
		}
		else if (_damage > 0)
		{
			ChangeAnim(Anim_Type.DAMAGE);
			SoundController.EffectSound_Play(EffectSoundType.MonsterHit);
		}
		_monsterHpGaugeSprite.localScale = VecrorScale("x", _monsterHpGaugeSprite, (float)_monsterHpGaugeSpriteValue * (1f / (float)_monsterStat.mHp));
	}

	public void SetHP()
	{
		_monsterHpGaugeSpriteValue = _monsterHpGaugeValue;
		_monsterHpGaugeSprite.localScale = VecrorScale("x", _monsterHpGaugeSprite, (float)_monsterHpGaugeSpriteValue * (1f / (float)_monsterStat.mHp));
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
		if (_isTarget)
		{
			PuzzlePlayManager.MonsterUnTargeting();
		}
	}

	public void SetMonsterTargeting(bool _istarget)
	{
		if (_isTarget)
		{
		}
		if (_istarget)
		{
			UnityEngine.Debug.Log("MONSTER TARGETING true =  " + base.gameObject.name);
			_isTarget = true;
			SoundController.EffectSound_Play(EffectSoundType.MonsterTargeting);
		}
		else
		{
			UnityEngine.Debug.Log("MONSTER TARGETING false =  " + base.gameObject.name);
			_isTarget = false;
		}
	}

	public void SkillIntervalChange()
	{
		_skillInterval = _monsterSkillDbData.mSkillInterval;
	}

	public void DecreaseSkill()
	{
		_skillInterval--;
	}

	public void StunMonster()
	{
		if (_stunEff != null)
		{
			MasterPoolManager.ReturnToPool("Effect", _stunEff);
			_stunEff = null;
		}
		UnityEngine.Debug.Log("***********STUN 22");
		SoundController.EffectSound_Play(EffectSoundType.MonsterStun);
		_stunEff = MasterPoolManager.SpawnObject("Effect", "FX_stun", base.transform);
		if (GameInfo.inGamePlayData.isDragon == 0)
		{
			Transform transform = _stunEff;
			Vector3 localPosition = _monsterAttackTurnSprite[0].transform.parent.localPosition;
			transform.localPosition = new Vector3(0f, localPosition.y, 0f);
		}
		else
		{
			_stunEff.localPosition = new Vector3(-1.8f, 3.5f, 0f);
		}
		ChangeAnim(Anim_Type.STUN);
		_isStun = true;
	}

	private void RemoveStun()
	{
		if (_stunEff != null)
		{
			MasterPoolManager.ReturnToPool("Effect", _stunEff);
			_stunEff = null;
			_isStun = false;
		}
	}

	public void ChangeAnim(Anim_Type _type)
	{
		switch (_type)
		{
		case Anim_Type.SKILL:
		case Anim_Type.SKILLEFFECT:
		case Anim_Type.MOVE:
			break;
		case Anim_Type.IDLE:
			_monsterAnim.ResetTrigger("Idle");
			_monsterAnim.SetTrigger("Idle");
			break;
		case Anim_Type.ATTACK_HUNTER:
			_monsterAnim.ResetTrigger("Attack_Hunter");
			_monsterAnim.SetTrigger("Attack_Hunter");
			break;
		case Anim_Type.ATTACK_MONSTER:
			_monsterAnim.ResetTrigger("Attack_Monster");
			_monsterAnim.SetTrigger("Attack_Monster");
			break;
		case Anim_Type.DAMAGE:
			_monsterAnim.ResetTrigger("Damage");
			_monsterAnim.SetTrigger("Damage");
			break;
		case Anim_Type.STUN:
			_monsterAnim.ResetTrigger("Stun");
			_monsterAnim.SetTrigger("Stun");
			break;
		case Anim_Type.DEATH:
			_monsterAnim.ResetTrigger("Death");
			_monsterAnim.SetTrigger("Death");
			break;
		}
	}

	public void SetAnimation(Hero _hunter, Action _onCallBack)
	{
		if (!(_hunter == null))
		{
			onCharacterAttackEnd = _onCallBack;
			_monsterState = EnemyState.Attack;
			_originPos = base.transform.position;
			_originDepth = _monsterAnim.GetComponent<MeshRenderer>().sortingOrder;
			_attackHunter = _hunter;
			StartCoroutine(CharacterAnimRoutine(_hunter));
			if (GameInfo.inGamePlayData.isDragon == 1)
			{
				StartCoroutine(DragonFireBlast());
			}
		}
	}

	public void End_Attack_Anim()
	{
		ChangeAnim(Anim_Type.IDLE);
		_monsterState = EnemyState.Idle;
		onCharacterAttackEnd();
		StartCoroutine(EndAttackAnimCoroutine());
	}

	public void DamageAnim()
	{
		_attackHunter.HunterCharacter.ChangeAnim(Anim_Type.DAMAGE);
	}

	public void DisableMonsterUI(bool _onoff)
	{
		if (_onoff)
		{
			_monsterAttributeUI.gameObject.SetActive(value: true);
			_monsterGaugeUI.gameObject.SetActive(value: true);
			_monsterTurnUI.gameObject.SetActive(value: true);
		}
		else
		{
			_monsterAttributeUI.gameObject.SetActive(value: false);
			_monsterGaugeUI.gameObject.SetActive(value: false);
			_monsterTurnUI.gameObject.SetActive(value: false);
		}
	}

	public void ChangeMonsterState(EnemyState _state)
	{
		switch (_state)
		{
		case EnemyState.Attack:
			_monsterState = EnemyState.Attack;
			break;
		case EnemyState.Idle:
			_monsterState = EnemyState.Idle;
			break;
		}
	}

	private void DieEffect()
	{
		Transform transform = null;
		Transform transform2 = null;
		int num = 0;
		if (_monsterHpGaugeValue != 0 || _isItemGet)
		{
			return;
		}
		_isItemGet = true;
		PuzzlePlayManager.LastCoinEffect();
		RemoveStun();
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

	private IEnumerator CharacterAnimRoutine(Hero _hunter)
	{
		if (_monsterInfo.motionType == 1)
		{
			if (_isTarget)
			{
				PuzzlePlayManager.MonsterUnTargeting();
			}
			ChangeAnim(Anim_Type.MOVE);
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
			_monsterAnim.GetComponent<MeshRenderer>().sortingOrder = _hunter.HunterCharacter.HunterAnim.GetComponent<MeshRenderer>().sortingOrder;
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			ChangeAnim(Anim_Type.ATTACK_MONSTER);
		}
		else
		{
			ChangeAnim(Anim_Type.ATTACK_MONSTER);
		}
		yield return null;
	}

	private IEnumerator EndAttackAnimCoroutine()
	{
		if (_monsterInfo.motionType == 1)
		{
			LeanTween.moveX(base.transform.gameObject, _originPos.x, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseOutQuint();
			LeanTween.moveY(base.transform.gameObject, _originPos.y, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseOutQuint();
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			_monsterAnim.GetComponent<MeshRenderer>().sortingOrder = _originDepth;
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			if (_isTarget)
			{
				PuzzlePlayManager.MonsterTargeting(this);
			}
		}
		yield return null;
	}

	private IEnumerator DragonFireBlast()
	{
		yield return new WaitForSeconds(0.7f / GameInfo.inGameBattleSpeedRate);
		Transform _eff_ball = MasterPoolManager.SpawnObject("Effect", "Skill_Dragon_fireball", null, 0.2f / GameInfo.inGameBattleSpeedRate);
		_eff_ball.position = _dragonSkillAnchor.position;
		LeanTween.moveX(_eff_ball.gameObject, -2.5f, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
		LeanTween.moveY(_eff_ball.gameObject, 1.5f, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate);
		Transform _eff_hit = MasterPoolManager.SpawnObject("Effect", "Skill_Dragon", null, 1f);
		_eff_hit.position = Vector3.zero;
	}

	private Vector3 VecrorScale(string _type, Transform _tr, float _scale)
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
