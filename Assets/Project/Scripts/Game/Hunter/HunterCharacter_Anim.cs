using Spine.Unity;
using UnityEngine;

public class HunterCharacter_Anim : MonoBehaviour
{
	[SerializeField]
	private HunterCharacter hunterCharacter;

    private void Awake()
    {
        if (hunterCharacter == null) hunterCharacter = transform.parent.GetComponent<HunterCharacter>();
        var skeleton = GetComponent<SkeletonAnimation>();
        if (skeleton) skeleton.loop = true;
    }

    public void End_Attack_Anim()
	{
		UnityEngine.Debug.Log("22222222222222 = ");
		hunterCharacter.End_Attack_Anim();
	}

	public void SetMonsterHP_Gauge()
	{
		UnityEngine.Debug.Log("11111111111111 = ");
		SoundController.EffectSound_Play(EffectSoundType.HunterAttack);
		InGamePlayManager.ShakeCamera(isVibration: false);
		hunterCharacter.SetMonsterHP_Gauge();
	}

	public void CheckStun()
	{
		if (hunterCharacter.Hunter.IsHunterStun)
		{
			hunterCharacter.SetAnim(Anim_Type.STUN);
		}
		else
		{
			hunterCharacter.SetAnim(Anim_Type.IDLE);
		}
	}
}
