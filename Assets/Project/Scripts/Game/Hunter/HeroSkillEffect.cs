using UnityEngine;
using UnityEngine.Serialization;

public class HeroSkillEffect : MonoBehaviour
{
	[FormerlySerializedAs("hunter_tr")] [SerializeField]
	private Transform _hunterTR;

	[FormerlySerializedAs("skillText_tr")] [SerializeField]
	private Transform _skillTextTR;
	
	private Transform _hunterCharacter;
	private Transform _hunterSkillText;

	private Hero _hunter;

	public void Construct(Hero _hunter)
	{
		this._hunter = _hunter;
		ConfigureHero();
		SetSkillEffect();
	}

	private void ConfigureHero()
	{
		if (_hunterTR.childCount > 0)
		{
			MasterPoolManager.ReturnToPool("Hunter", _hunterTR.GetChild(0));
			_hunterCharacter = null;
		}
		_hunterCharacter = MasterPoolManager.SpawnObject("Hunter", _hunter.HeroInfo.Hunter.hunterIdx.ToString(), _hunterTR, -1f, isSpeedProcess: false);
		_hunterCharacter.localScale = Vector3.one;
		_hunterCharacter.GetComponent<HeroCharacter>().ConstructSkill(_hunter);
		_hunterCharacter.GetComponent<HeroCharacter>().ChangeAnim(Anim_Type.ATTACK_HUNTER);
	}

	private void SetSkillEffect()
	{
		if (_skillTextTR.childCount > 0)
		{
			MasterPoolManager.ReturnToPool("Effect", _skillTextTR.GetChild(0));
			_hunterSkillText = null;
		}
		_hunterSkillText = MasterPoolManager.SpawnObject("Effect", "Skill_text_" + _hunter.HeroInfo.Hunter.skillIdx.ToString(), _skillTextTR, -1f, isSpeedProcess: false);
		_hunterSkillText.localPosition = Vector3.zero;
		_hunterSkillText.localScale = Vector3.one;
	}

	private void OnDisable()
	{
		if (_hunterTR.childCount > 0)
		{
			MasterPoolManager.ReturnToPool("Hunter", _hunterTR.GetChild(0));
			_hunterCharacter = null;
		}
		if (_skillTextTR.childCount > 0)
		{
			MasterPoolManager.ReturnToPool("Effect", _skillTextTR.GetChild(0));
			_hunterSkillText = null;
		}
	}
}
