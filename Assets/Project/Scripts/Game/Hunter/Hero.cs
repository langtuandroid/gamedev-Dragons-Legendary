using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Hero : HeroBase
{
	private Action<int, int> OnHanterAttack;
	private Action<int> OnComboEffect;
	private Action OnSkill;
	private Action OnAttack;

	[FormerlySerializedAs("hunter_Tier_tr")] [SerializeField]
	private Transform _hanterTier;

	[FormerlySerializedAs("hunterCharacter")] [SerializeField]
	private HeroCharacter _heroCharacter;
	
	[FormerlySerializedAs("hunter_type")] [SerializeField]
	private HeroType _heroType;

	[FormerlySerializedAs("hunter_tribe")] [SerializeField]
	private HeroTribe _heroTribe;
	
	[FormerlySerializedAs("hunter_Skill_Ready_Eff")] [SerializeField]
	private GameObject _heroSkillReadyEff;
	
	[FormerlySerializedAs("hunter_Skill_Gauge_Sprite")] [SerializeField]
	private Transform _heroSkillGaugeSprite;
	
	[FormerlySerializedAs("arenaBuffTextAnchor")] [SerializeField]
	private Transform _arenaBuffTextAnchor;

	[FormerlySerializedAs("hunterFace")] [SerializeField]
	private Transform _heroFace;
	
	private HunterInfo _heroInfo;
	private int _heroIndex;
	private int _heroBlockCount;
	private int _heroTotalDamage;
	private int _hunterSkillGaugeValue;
	private bool _isHunterSkillAvailable;
	private int _hunterSkillGaugeFullValue;
	private bool _isHunterStun;
	private int _isHunterStunClearCount;
	private float _userBonusAttack;
	private int _damageDummyConstX = 1;
	private float _damageDummyCombo = 0.1f;
	private Transform _stunEff;
	private HeroLeaderSkill _hunterLeaderSkill;
	private HeroState _hunterState;
	private Monster _attackMonster;
	private int _attackDamage;

	public HunterInfo HeroInfo => _heroInfo;

	public HeroState HunterState => _hunterState;

	public HeroType HeroType => _heroType;

	public int HeroArrIdx => _heroIndex;

	public int HeroTotalDamage => _heroTotalDamage;

	public bool IsHeroSkillAvailable => _isHunterSkillAvailable;

	public bool IsHunterStun => _isHunterStun;

	public HeroCharacter HunterCharacter => _heroCharacter;

	public void Construct(int _hunter_arr_idx, HeroCharacter _hunterChacter, HeroLeaderSkill _hunterLeaderSkill, HunterInfo _hunter_Info = null)
	{
		if (_heroInfo != null)
		{
			_heroInfo = null;
			_heroInfo = new HunterInfo();
		}
		for (int i = 0; i < _hanterTier.childCount; i++)
		{
			_hanterTier.GetChild(i).gameObject.SetActive(value: false);
		}
		if (_stunEff != null)
		{
			UnityEngine.Debug.Log("Return Stun !");
			MWPoolManager.DeSpawn("Effect", _stunEff);
			_stunEff = null;
		}
		OnHanterAttack = null;
		OnComboEffect = null;
		OnSkill = null;
		_heroBlockCount = 0;
		_heroTotalDamage = 0;
		_heroSkillGaugeSprite.localScale = SetScale("x", _heroSkillGaugeSprite, 0f);
		_hunterSkillGaugeValue = 0;
		_isHunterStun = false;
		_isHunterStunClearCount = 0;
		_isHunterSkillAvailable = false;
		_heroIndex = _hunter_arr_idx;
		_heroSkillReadyEff.SetActive(value: false);
		_userBonusAttack = GameDataManager.GetUserLevelData(GameInfo.userData.userInfo.level).attackBonusAll + 1f;
		if (_hunter_Info != null)
		{
			_heroInfo = _hunter_Info;
			_heroType = (HeroType)_hunter_Info.Hunter.color;
			_heroTribe = (HeroTribe)_hunter_Info.Hunter.hunterTribe;
			_hunterSkillGaugeFullValue = _heroInfo.Skill.skillGauge;
		}
		_heroCharacter = _hunterChacter;
		this._hunterLeaderSkill = _hunterLeaderSkill;
		_heroCharacter.Construct(this);
		ChangeHeroFave();
		_hanterTier.GetChild(_heroInfo.Stat.hunterTier - 1).gameObject.SetActive(value: true);
	}

	public void ChangeState(HeroState _state)
	{
		switch (_state)
		{
		case HeroState.attack:
			_hunterState = HeroState.attack;
			break;
		case HeroState.idle:
			_hunterState = HeroState.idle;
			break;
		}
	}

	public void SetAnimation(Anim_Type _type, Monster _monster, int _damage, Action _onCallBack)
	{
		if (!(_monster == null))
		{
			UnityEngine.Debug.Log("** 11");
			ChangeState(HeroState.attack);
			_heroCharacter.SetTweeenMonster(_monster);
			_attackMonster = _monster;
			_attackDamage = _damage;
			OnAttack = _onCallBack;
		}
	}

	public void SetCharacterSkill(HunterSkillRange _range, Monster[] _monster, Action _OnCallback)
	{
		if (_monster != null)
		{
			_heroCharacter.SetTweenSkil(_monster[UnityEngine.Random.Range(0, _monster.Length)]);
		}
		StartCoroutine(UseSkill(_range, _monster, _OnCallback));
	}

	public void SetMonsterHP_Gauge()
	{
		Transform transform = null;
		transform = MWPoolManager.Spawn("Effect", "Fx_hit01", null, 1.5f);
		transform.position = _attackMonster.HitAnchor;
		_attackMonster.SetMonsterHP_Gauge(_attackDamage);
		GameUtil.Check_Property_Damage_UI(this, _attackMonster, _attackDamage);
	}

	public void AttackEnd()
	{
		UnityEngine.Debug.Log("** 22 = " + _hunterState);
		OnAttack();
	}

	public void SetBuff()
	{
		UnityEngine.Debug.Log("SetArenaBuff !!");
		PuzzlePlayManager.UIBuff(_arenaBuffTextAnchor.position, (BlockType)_heroInfo.Hunter.color, CheckArenaBuff(_heroInfo));
	}

	public void SetHunterInfo(HunterInfo _info)
	{
		_heroInfo = _info;
	}

	public void ResetDamage()
	{
		_heroBlockCount = 0;
		_heroTotalDamage = 0;
	}

	public void AddDamage(int _blockCount)
	{
		_heroBlockCount += _blockCount - 2;
		_heroTotalDamage = (int)((GameUtil.GetHunterReinForceAttack(_heroInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_heroInfo.Hunter.hunterIdx)) * (float)CheckArenaBuff(_heroInfo) + (float)_heroInfo.leaderSkillAttack) * _userBonusAttack * (float)_heroBlockCount * (float)_damageDummyConstX);
		PuzzlePlayManager.AddAttack(_heroTotalDamage, _heroInfo.Hunter.color, base.transform.position, _heroInfo.Hunter.hunterIdx);
	}

	public void AddDamageCombo(int _combo, int _color, int _lastAttackIdx, Action<int> _OnCallBack)
	{
		StartCoroutine(AddComboEffect(_combo, _color, _lastAttackIdx, _OnCallBack));
	}

	public void AddGaugeSkill(int _count)
	{
		if (_hunterSkillGaugeFullValue <= _hunterSkillGaugeValue)
		{
			_hunterSkillGaugeValue = _hunterSkillGaugeFullValue;
			if (!_isHunterSkillAvailable)
			{
				_isHunterSkillAvailable = true;
			}
			return;
		}
		_hunterSkillGaugeValue += _count;
		if (_hunterSkillGaugeValue >= _hunterSkillGaugeFullValue)
		{
			_hunterSkillGaugeValue = _hunterSkillGaugeFullValue;
			if (!_isHunterSkillAvailable)
			{
				_isHunterSkillAvailable = true;
			}
		}
		_heroSkillGaugeSprite.localScale = SetScale("x", _heroSkillGaugeSprite, (float)_hunterSkillGaugeValue * (1f / (float)_hunterSkillGaugeFullValue));
	}

	public void SetHeroValue()
	{
		AddGaugeSkill(_hunterSkillGaugeFullValue);
	}
	

	public void StartAttack(Action<int, int> _OnCallback)
	{
		OnHanterAttack = _OnCallback;
		OnHanterAttack(_heroTotalDamage, _heroIndex);
		OnHanterAttack = null;
	}

	public IEnumerator UseSkill(HunterSkillRange _range, Monster[] _monster, Action _OnCallback)
	{
		if (_heroInfo.Skill.motionType == 1)
		{
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate);
		}
		Transform skillEff = null;
		switch (_heroInfo.Hunter.color)
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
		SoundController.HunterSkillCutPlay(_heroInfo.Hunter.hunterIdx);
		skillEff.localScale = Vector3.one;
		skillEff.position = Vector3.zero;
		skillEff.GetComponent<HeroSkillEffect>().Construct(this);
		yield return new WaitForSeconds(1f);
		OnSkill = _OnCallback;
		_hunterSkillGaugeValue = 0;
		_heroSkillGaugeSprite.localScale = SetScale("x", _heroSkillGaugeSprite, (float)_hunterSkillGaugeValue * (1f / (float)_hunterSkillGaugeFullValue));
		_isHunterSkillAvailable = false;
		_heroSkillReadyEff.SetActive(value: false);
		SoundController.HunterSkillSound(HeroInfo.Skill.skillIdx);
		UnityEngine.Debug.Log("************ this.hunter_Info.Skill.skillType = " + _heroInfo.Skill.skillType);
		switch (_heroInfo.Skill.skillType)
		{
		case 1:
			StartCoroutine(UseSkillSettings(_range, _monster));
			break;
		case 2:
			StartCoroutine(UseSkillSettings(_range, _monster));
			PuzzlePlayManager.HealHeroes((int)((float)PuzzlePlayManager.GetTotalHp() * _heroInfo.Skill.recPowers));
			break;
		case 3:
			UnityEngine.Debug.Log("***********STUN 11");
			StartCoroutine(UseSkillSettings(_range, _monster));
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
			PuzzlePlayManager.ChangeBlockType((BlockType)_heroInfo.Skill.beforeBlock, (BlockType)_heroInfo.Skill.afterBlock, _heroInfo.Skill.skillIdx);
			StartCoroutine(SkillEnd());
			break;
		}
	}

	public IEnumerator UseSkillSettings(HunterSkillRange _range, Monster[] _monster)
	{
		int _damage = 0;
		Transform[] _eff = new Transform[_heroInfo.Skill.times];
		float _eff_delay = 0.2f;
		for (int attackcount = 0; attackcount < _heroInfo.Skill.times; attackcount++)
		{
			switch (_heroInfo.Skill.statType)
			{
			case 1:
				_damage = (int)((GameUtil.GetHunterReinForceHP(_heroInfo.Stat.hunterHp, GameDataManager.HasUserHunterEnchant(_heroInfo.Hunter.hunterIdx)) + (float)_heroInfo.leaderSkillHp) * _heroInfo.Skill.multiple);
				break;
			case 2:
				_damage = (int)((GameUtil.GetHunterReinForceAttack(_heroInfo.Stat.hunterAttack, GameDataManager.HasUserHunterEnchant(_heroInfo.Hunter.hunterIdx)) * (float)CheckArenaBuff(_heroInfo) + (float)_heroInfo.leaderSkillAttack) * _heroInfo.Skill.multiple);
				break;
			case 3:
				_damage = (int)((GameUtil.GetHunterReinForceHeal(_heroInfo.Stat.hunterRecovery, GameDataManager.HasUserHunterEnchant(_heroInfo.Hunter.hunterIdx)) + (float)_heroInfo.leaderSkillRecovery) * _heroInfo.Skill.multiple);
				break;
			}
			for (int i = 0; i < _monster.Length; i++)
			{
				GameUtil.Check_Property_Damage(this, _monster[i], _damage);
				_monster[i].SetMonsterHP(_damage);
			}
			_eff[attackcount] = MWPoolManager.Spawn("Effect", "Skill_" + _heroInfo.Skill.skillIdx, null, 1.5f);
			if (_range == HunterSkillRange.SINGLE)
			{
				_eff[attackcount].position = _monster[0].SkillHitAnchor;
			}
			else
			{
				_eff[attackcount].position = Vector3.zero;
			}
			_eff[attackcount].GetChild(0).GetComponent<SkillEffect_Anim>().SetMonster(this, _monster, _damage);
			if (_heroInfo.Skill.times > 1)
			{
				yield return new WaitForSeconds(_eff_delay);
			}
			else
			{
				yield return null;
			}
		}
		yield return new WaitForSeconds(1f);
		OnSkill();
		OnSkill = null;
	}

	public void SkillReadyEffect(bool _isOn)
	{
		if (_isOn)
		{
			if (_isHunterSkillAvailable)
			{
				_heroSkillReadyEff.SetActive(value: true);
				SoundController.EffectSound_Play(EffectSoundType.HunterSkillReady);
			}
			else
			{
				_heroSkillReadyEff.SetActive(value: false);
			}
		}
		else
		{
			_heroSkillReadyEff.SetActive(value: false);
		}
	}

	public void StunHunter()
	{
		if (_stunEff != null)
		{
			UnityEngine.Debug.Log("Return Stun !");
			MWPoolManager.DeSpawn("Effect", _stunEff);
			_stunEff = null;
		}
		_stunEff = MWPoolManager.Spawn("Effect", "FX_stun_hunter", base.transform);
		_stunEff.position = base.transform.position;
		_isHunterStun = true;
		_isHunterStunClearCount = 1;
		_heroCharacter.ChangeAnim(Anim_Type.STUN);
		PuzzlePlayManager.DeBlock((BlockType)_heroInfo.Hunter.color);
	}

	public void RemoveStun()
	{
		if (_isHunterStunClearCount > 0)
		{
			_isHunterStunClearCount--;
			return;
		}
		_isHunterStun = false;
		_heroCharacter.ChangeAnim(Anim_Type.IDLE);
		PuzzlePlayManager.Block((BlockType)_heroInfo.Hunter.color);
		if (_stunEff != null)
		{
			UnityEngine.Debug.Log("Return Stun !");
			MWPoolManager.DeSpawn("Effect", _stunEff);
			_stunEff = null;
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

	private IEnumerator AddComboEffect(int _combo, int _color, int _lastAttackIdx, Action<int> _OnCallBack)
	{
		OnComboEffect = _OnCallBack;
		yield return null;
		if (_combo > 0 && _heroTotalDamage > 0)
		{
			float comboDuration = 0.24f;
			for (int i = 0; i < _combo; i++)
			{
				base.transform.localScale = Vector3.one;
				PuzzlePlayManager.AddCombo(i + 1, _heroInfo.Hunter.color, base.transform.position, _heroInfo.Hunter.hunterIdx);
				if (i > 0)
				{
					_heroTotalDamage += (int)((float)_heroTotalDamage * _damageDummyCombo);
				}
				LeanTween.cancel(base.transform.gameObject);
				LeanTween.scale(base.transform.gameObject, new Vector3(1.2f, 1.2f, 1.2f), comboDuration / 2f).setLoopPingPong(1).setEase(LeanTweenType.linear);
				PuzzlePlayManager.AddAttack(_heroTotalDamage, _heroInfo.Hunter.color, base.transform.position, _heroInfo.Hunter.hunterIdx);
				if (_heroIndex == _lastAttackIdx)
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
			_heroTotalDamage = _hunterLeaderSkill.CkeckSkillCombo(_combo, _heroTotalDamage);
			_heroTotalDamage = _hunterLeaderSkill.CheckLeaderSkillColor(_color, _heroTotalDamage);
			PuzzlePlayManager.AddAttack(_heroTotalDamage, _heroInfo.Hunter.color, base.transform.position, _heroInfo.Hunter.hunterIdx);
		}
		OnComboEffect(_heroIndex);
		OnComboEffect = null;
	}

	private void ChangeHeroFave()
	{
		for (int i = 0; i < _heroFace.childCount; i++)
		{
			_heroFace.GetChild(i).gameObject.SetActive(value: false);
		}
		switch (_heroInfo.Stat.hunterTier)
		{
		case 1:
			_heroFace.GetChild(int.Parse(_heroInfo.Hunter.hunterImg1) - 1).gameObject.SetActive(value: true);
			break;
		case 2:
			_heroFace.GetChild(int.Parse(_heroInfo.Hunter.hunterImg2) - 1).gameObject.SetActive(value: true);
			break;
		case 3:
			_heroFace.GetChild(int.Parse(_heroInfo.Hunter.hunterImg3) - 1).gameObject.SetActive(value: true);
			break;
		case 4:
			_heroFace.GetChild(int.Parse(_heroInfo.Hunter.hunterImg4) - 1).gameObject.SetActive(value: true);
			break;
		case 5:
			_heroFace.GetChild(int.Parse(_heroInfo.Hunter.hunterImg5) - 1).gameObject.SetActive(value: true);
			break;
		}
	}

	private Vector3 SetScale(string _type, Transform _tr, float _scale)
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

	private IEnumerator SkillEnd()
	{
		yield return new WaitForSeconds(1f);
		OnSkill();
		OnSkill = null;
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
