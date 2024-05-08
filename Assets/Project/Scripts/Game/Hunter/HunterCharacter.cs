using Spine.Unity;
using System.Collections;
using UnityEngine;

public class HunterCharacter : MonoBehaviour
{
	[SerializeField]
	private Animator hunterAnim;

	[SerializeField]
	private Hero hunter;

	[SerializeField]
	private Vector3 origin_Pos;

	[SerializeField]
	private int origin_Depth;

	[SerializeField]
	private Transform attackAnchor;

	[SerializeField]
	private bool isTweening;

    //public SkeletonAnimator HunterAnim => hunterAnim.GetComponent<SkeletonAnimator>();
    public SkeletonAnimation HunterAnim => hunterAnim.GetComponent<SkeletonAnimation>();

    public Vector3 AttackAnchor => attackAnchor.position;

	public Hero Hunter => hunter;

	public bool IsTweening => isTweening;

    private void Awake()
    {
        if (hunterAnim == null) hunterAnim = GetComponentInChildren<Animator>();
    }

    public void HunterSkillInit(Hero _hunter)
	{
		if (hunter != null)
		{
			hunter = null;
		}
		SetHunterCharacterTier(_hunter);
	}

	public void Init(Hero _hunter)
	{
		if (hunter != null)
		{
			hunter = null;
		}
		hunter = _hunter;
		SetHunterCharacterTier(hunter);
	}

	public void SetSummonEffect()
	{
		Transform transform = null;
		transform = MWPoolManager.Spawn("Effect", "FX_Summon02", null, 1f);
		transform.position = base.transform.position;
	}

	private void SetHunterCharacterTier(Hero _hunter)
	{
		switch (_hunter.HunterInfo.Stat.hunterTier)
		{
		case 1:
			HunterAnim.initialSkinName = _hunter.HunterInfo.Hunter.hunterImg1;
			break;
		case 2:
			HunterAnim.initialSkinName = _hunter.HunterInfo.Hunter.hunterImg2;
			break;
		case 3:
			HunterAnim.initialSkinName = _hunter.HunterInfo.Hunter.hunterImg3;
			break;
		case 4:
			HunterAnim.initialSkinName = _hunter.HunterInfo.Hunter.hunterImg4;
			break;
		case 5:
			HunterAnim.initialSkinName = _hunter.HunterInfo.Hunter.hunterImg5;
			break;
		}
		HunterAnim.Initialize(overwrite: true);
		hunterAnim.GetComponent<MeshRenderer>().sortingLayerName = "Default";
		hunterAnim.GetComponent<MeshRenderer>().sortingOrder = _hunter.Hunter_Arr_Idx;
	}

	public void SetTweenMonster(Monster _monster)
	{
		isTweening = true;
		origin_Pos = base.transform.position;
		origin_Depth = hunterAnim.GetComponent<MeshRenderer>().sortingOrder;
		StartCoroutine(SetTweenMonster_Coroutine(_monster));
	}

	public void End_Attack_Anim()
	{
		if (!(hunter == null))
		{
			SetAnim(Anim_Type.IDLE);
			hunter.SetHunterState(HUNTER_STATE.IDLE);
			hunter.EndCharacterAttack();
			StartCoroutine(End_Attack_Anim_Coroutine());
		}
	}

	public void SetTweenSkill(Monster _monster)
	{
		origin_Pos = base.transform.position;
		origin_Depth = hunterAnim.GetComponent<MeshRenderer>().sortingOrder;
		if (hunter.HunterInfo.Skill.motionType == 1)
		{
			SetAnim(Anim_Type.MOVE);
			StartCoroutine(SetTweenSkill_Coroutine(_monster));
		}
	}

	public void SetMonsterHP_Gauge()
	{
		if (!(hunter == null))
		{
			hunter.SetMonsterHP_Gauge();
		}
	}

	public void SetAnim(Anim_Type _type)
	{
		switch (_type)
		{
		case Anim_Type.IDLE:
			hunterAnim.ResetTrigger("Idle");
			hunterAnim.SetTrigger("Idle");
			break;
		case Anim_Type.ATTACK_HUNTER:
			hunterAnim.ResetTrigger("Attack_Hunter");
			hunterAnim.SetTrigger("Attack_Hunter");
			break;
		case Anim_Type.ATTACK_MONSTER:
			hunterAnim.ResetTrigger("Attack_Monster");
			hunterAnim.SetTrigger("Attack_Monster");
			break;
		case Anim_Type.DAMAGE:
			hunterAnim.ResetTrigger("Damage");
			hunterAnim.SetTrigger("Damage");
			break;
		case Anim_Type.SKILL:
			hunterAnim.ResetTrigger("Skill");
			hunterAnim.SetTrigger("Skill");
			break;
		case Anim_Type.STUN:
			hunterAnim.ResetTrigger("Stun");
			hunterAnim.SetTrigger("Stun");
			break;
		case Anim_Type.SKILLEFFECT:
			hunterAnim.ResetTrigger("SkillEffect");
			hunterAnim.SetTrigger("SkillEffect");
			break;
		case Anim_Type.MOVE:
			hunterAnim.ResetTrigger("Move");
			hunterAnim.SetTrigger("Move");
			break;
		case Anim_Type.DEATH:
			hunterAnim.ResetTrigger("Death");
			hunterAnim.SetTrigger("Death");
			break;
		}
	}

	private IEnumerator SetTweenMonster_Coroutine(Monster _monster)
	{
		float addY = (GameInfo.inGamePlayData.isDragon != 1) ? 0f : 0.5f;
		if (hunter.HunterInfo.Skill.motionType == 1)
		{
			UnityEngine.Debug.Log("SHORT !!!! = " + hunter.HunterInfo.Hunter.hunterIdx);
			SetAnim(Anim_Type.MOVE);
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
			hunterAnim.GetComponent<MeshRenderer>().sortingOrder = _monster.MonsterAnim.GetComponent<MeshRenderer>().sortingOrder;
			yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
			Debug.Log("!ATTACK");
			SetAnim(Anim_Type.ATTACK_HUNTER);
		}
		else
		{
			UnityEngine.Debug.Log("LONG !!!!");
			SetAnim(Anim_Type.ATTACK_HUNTER);
		}
		yield return null;
	}

	private IEnumerator End_Attack_Anim_Coroutine()
	{
		LeanTween.moveX(base.transform.gameObject, origin_Pos.x, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseOutQuint();
		LeanTween.moveY(base.transform.gameObject, origin_Pos.y, 0.2f / GameInfo.inGameBattleSpeedRate).setEaseOutQuint();
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		hunterAnim.GetComponent<MeshRenderer>().sortingOrder = origin_Depth;
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		TweenComplete();
	}

	private IEnumerator SetTweenSkill_Coroutine(Monster _monster)
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
		hunterAnim.GetComponent<MeshRenderer>().sortingOrder = _monster.MonsterAnim.GetComponent<MeshRenderer>().sortingOrder;
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		SetTweenSkillBack();
	}

	private void SetTweenSkillBack()
	{
		SetAnim(Anim_Type.IDLE);
		StartCoroutine(SetTweenSkillBack_Coroutine());
	}

	private IEnumerator SetTweenSkillBack_Coroutine()
	{
		LeanTween.moveX(base.transform.gameObject, origin_Pos.x, 0.2f / GameInfo.inGameBattleSpeedRate).setDelay(1f).setEaseOutQuint();
		LeanTween.moveY(base.transform.gameObject, origin_Pos.y, 0.2f / GameInfo.inGameBattleSpeedRate).setDelay(1f).setEaseOutQuint();
		yield return new WaitForSeconds(0.2f / GameInfo.inGameBattleSpeedRate / 2f);
		hunterAnim.GetComponent<MeshRenderer>().sortingOrder = origin_Depth;
	}

	private void TweenComplete()
	{
		isTweening = false;
	}

	private void OnDisable()
	{
		SetAnim(Anim_Type.IDLE);
	}
}
