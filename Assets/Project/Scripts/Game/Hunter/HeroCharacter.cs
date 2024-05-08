using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class HeroCharacter : MonoBehaviour
{
	[FormerlySerializedAs("hunterAnim")] [SerializeField]
	private Animator _hunterAnimator;
	
	[FormerlySerializedAs("attackAnchor")] [SerializeField]
	private Transform _attackAnchor;
	
	private bool _isTweening;
	private Hero _hero;
	private Vector3 _originPos;
	private int _originDepth;
    public SkeletonAnimation HunterAnim => _hunterAnimator.GetComponent<SkeletonAnimation>();

    public Vector3 AttackAnchor => _attackAnchor.position;

	public Hero Hero => _hero;

	public bool IsTweening => _isTweening;

    private void Awake()
    {
        if (_hunterAnimator == null) _hunterAnimator = GetComponentInChildren<Animator>();
    }

    public void ConstructSkill(Hero _hunter)
	{
		if (this._hero != null)
		{
			this._hero = null;
		}
		SetTier(_hunter);
	}

	public void Construct(Hero _hunter)
	{
		if (this._hero != null)
		{
			this._hero = null;
		}
		this._hero = _hunter;
		SetTier(this._hero);
	}

	public void SetEffect()
	{
		Transform transform = null;
		transform = MWPoolManager.Spawn("Effect", "FX_Summon02", null, 1f);
		transform.position = base.transform.position;
	}

	private void SetTier(Hero _hunter)
	{
		switch (_hunter.HeroInfo.Stat.hunterTier)
		{
		case 1:
			HunterAnim.initialSkinName = _hunter.HeroInfo.Hunter.hunterImg1;
			break;
		case 2:
			HunterAnim.initialSkinName = _hunter.HeroInfo.Hunter.hunterImg2;
			break;
		case 3:
			HunterAnim.initialSkinName = _hunter.HeroInfo.Hunter.hunterImg3;
			break;
		case 4:
			HunterAnim.initialSkinName = _hunter.HeroInfo.Hunter.hunterImg4;
			break;
		case 5:
			HunterAnim.initialSkinName = _hunter.HeroInfo.Hunter.hunterImg5;
			break;
		}
		HunterAnim.Initialize(overwrite: true);
		_hunterAnimator.GetComponent<MeshRenderer>().sortingLayerName = "Default";
		_hunterAnimator.GetComponent<MeshRenderer>().sortingOrder = _hunter.HeroArrIdx;
	}

	public void SetTweeenMonster(Monster _monster)
	{
		_isTweening = true;
		_originPos = base.transform.position;
		_originDepth = _hunterAnimator.GetComponent<MeshRenderer>().sortingOrder;
		StartCoroutine(SetTweenMonster_Coroutine(_monster));
	}

	public void End_Attack_Anim()
	{
		if (!(_hero == null))
		{
			ChangeAnim(Anim_Type.IDLE);
			_hero.ChangeState(HeroState.idle);
			_hero.AttackEnd();
			StartCoroutine(EndAttackAnimCoroutine());
		}
	}

	public void SetTweenSkil(Monster _monster)
	{
		_originPos = base.transform.position;
		_originDepth = _hunterAnimator.GetComponent<MeshRenderer>().sortingOrder;
		if (_hero.HeroInfo.Skill.motionType == 1)
		{
			ChangeAnim(Anim_Type.MOVE);
			StartCoroutine(SetSkillCoroutine(_monster));
		}
	}

	public void SetMonsterHP_Gauge()
	{
		if (!(_hero == null))
		{
			_hero.SetMonsterHP_Gauge();
		}
	}

	public void ChangeAnim(Anim_Type _type)
	{
		switch (_type)
		{
		case Anim_Type.IDLE:
			_hunterAnimator.ResetTrigger("Idle");
			_hunterAnimator.SetTrigger("Idle");
			break;
		case Anim_Type.ATTACK_HUNTER:
			_hunterAnimator.ResetTrigger("Attack_Hunter");
			_hunterAnimator.SetTrigger("Attack_Hunter");
			break;
		case Anim_Type.ATTACK_MONSTER:
			_hunterAnimator.ResetTrigger("Attack_Monster");
			_hunterAnimator.SetTrigger("Attack_Monster");
			break;
		case Anim_Type.DAMAGE:
			_hunterAnimator.ResetTrigger("Damage");
			_hunterAnimator.SetTrigger("Damage");
			break;
		case Anim_Type.SKILL:
			_hunterAnimator.ResetTrigger("Skill");
			_hunterAnimator.SetTrigger("Skill");
			break;
		case Anim_Type.STUN:
			_hunterAnimator.ResetTrigger("Stun");
			_hunterAnimator.SetTrigger("Stun");
			break;
		case Anim_Type.SKILLEFFECT:
			_hunterAnimator.ResetTrigger("SkillEffect");
			_hunterAnimator.SetTrigger("SkillEffect");
			break;
		case Anim_Type.MOVE:
			_hunterAnimator.ResetTrigger("Move");
			_hunterAnimator.SetTrigger("Move");
			break;
		case Anim_Type.DEATH:
			_hunterAnimator.ResetTrigger("Death");
			_hunterAnimator.SetTrigger("Death");
			break;
		}
	}

	private IEnumerator SetTweenMonster_Coroutine(Monster _monster)
	{
		float addY = (GameInfo.inGamePlayData.isDragon != 1) ? 0f : 0.5f;
		if (_hero.HeroInfo.Skill.motionType == 1)
		{
			UnityEngine.Debug.Log("SHORT !!!! = " + _hero.HeroInfo.Hunter.hunterIdx);
			ChangeAnim(Anim_Type.MOVE);
			GameObject gameObject = base.transform.gameObject;
			Vector3 position = _monster.transform.position;
			float x = position.x;
			Vector3 vector = _monster.AttackAnchor;
			float x2 = vector.x;
			Vector3 position2 = _monster.transform.position;
			float num = x2 - position2.x;
			Vector3 position3 = base.transform.position;
			float x3 = position3.x;
			Vector3 vector2 = AttackAnchor;
			LeanTween.moveX(gameObject, x + (num + (x3 - vector2.x)), 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
			GameObject gameObject2 = base.transform.gameObject;
			Vector3 position4 = _monster.transform.position;
			LeanTween.moveY(gameObject2, position4.y + addY, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			_hunterAnimator.GetComponent<MeshRenderer>().sortingOrder = _monster.MonsterAnim.GetComponent<MeshRenderer>().sortingOrder;
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			Debug.Log("!ATTACK");
			ChangeAnim(Anim_Type.ATTACK_HUNTER);
		}
		else
		{
			UnityEngine.Debug.Log("LONG !!!!");
			ChangeAnim(Anim_Type.ATTACK_HUNTER);
		}
		yield return null;
	}

	private IEnumerator EndAttackAnimCoroutine()
	{
		LeanTween.moveX(base.transform.gameObject, _originPos.x, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseOutQuint();
		LeanTween.moveY(base.transform.gameObject, _originPos.y, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseOutQuint();
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		_hunterAnimator.GetComponent<MeshRenderer>().sortingOrder = _originDepth;
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		OnCompleteTween();
	}

	private IEnumerator SetSkillCoroutine(Monster _monster)
	{
		GameObject gameObject = base.transform.gameObject;
		Vector3 position = _monster.transform.position;
		float x = position.x;
		Vector3 vector = _monster.AttackAnchor;
		float x2 = vector.x;
		Vector3 position2 = _monster.transform.position;
		float num = x2 - position2.x;
		Vector3 position3 = base.transform.position;
		float x3 = position3.x;
		Vector3 vector2 = AttackAnchor;
		LeanTween.moveX(gameObject, x + (num + (x3 - vector2.x)), 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
		GameObject gameObject2 = base.transform.gameObject;
		Vector3 position4 = _monster.transform.position;
		LeanTween.moveY(gameObject2, position4.y, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseInQuint();
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		_hunterAnimator.GetComponent<MeshRenderer>().sortingOrder = _monster.MonsterAnim.GetComponent<MeshRenderer>().sortingOrder;
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		SetTweenSkillBack();
	}

	private void SetTweenSkillBack()
	{
		ChangeAnim(Anim_Type.IDLE);
		StartCoroutine(SetTweenSkillBack_Coroutine());
	}

	private IEnumerator SetTweenSkillBack_Coroutine()
	{
		LeanTween.moveX(base.transform.gameObject, _originPos.x, 0.2f / GameInfo.inGameBattleSpeedRate).setDelay(1f).setEaseOutQuint();
		LeanTween.moveY(base.transform.gameObject, _originPos.y, 0.2f / GameInfo.inGameBattleSpeedRate).setDelay(1f).setEaseOutQuint();
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		_hunterAnimator.GetComponent<MeshRenderer>().sortingOrder = _originDepth;
	}

	private void OnCompleteTween()
	{
		_isTweening = false;
	}

	private void OnDisable()
	{
		ChangeAnim(Anim_Type.IDLE);
	}
}
