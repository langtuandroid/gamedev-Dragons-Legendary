using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyAnimation : MonoBehaviour
{
	[FormerlySerializedAs("monster")] [SerializeField]
	private Enemy _monster;

    private void Awake()
    {
        if (_monster == null) _monster = transform.parent.GetComponent<Enemy>();
        var skeleton = GetComponent<SkeletonAnimation>();
        if (skeleton) skeleton.loop = true;
    }

    public void End_Die_Anim()
	{
		UnityEngine.Debug.Log("End_Die_Anim()");
		_monster.End_Die_Anim();
	}

	public void Damage()
	{
		UnityEngine.Debug.Log("monster.MonsterCurrentDamage = " + _monster.MonsterCurrentDamage);
		SoundController.Monster_Play(_monster.MonsterInfo.mIdx);
		Transform transform = null;
		transform = MasterPoolManager.SpawnObject("Effect", "Fx_Damage_hit", null, 0.5f);
		transform.position = PuzzlePlayManager.DamagePosition;
		_monster.DamageAnim();
		PuzzlePlayManager.Damage(_monster.MonsterCurrentDamage);
	}

	public void End_Attack_Anim()
	{
		_monster.End_Attack_Anim();
	}

	public void CheckStun()
	{
		if (_monster.IsStun)
		{
			_monster.ChangeAnim(Anim_Type.STUN);
		}
		else
		{
			_monster.ChangeAnim(Anim_Type.IDLE);
		}
	}
}
