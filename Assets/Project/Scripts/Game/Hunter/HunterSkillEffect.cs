using UnityEngine;

public class HunterSkillEffect : MonoBehaviour
{
	[SerializeField]
	private Transform hunter_tr;

	[SerializeField]
	private Transform skillText_tr;

	[SerializeField]
	private Transform hunterCharacter;

	[SerializeField]
	private Transform hunterSkillText;

	private Hunter hunter;

	public void Init(Hunter _hunter)
	{
		hunter = _hunter;
		SetHunter();
		SetSkillText();
	}

	private void SetHunter()
	{
		if (hunter_tr.childCount > 0)
		{
			MWPoolManager.DeSpawn("Hunter", hunter_tr.GetChild(0));
			hunterCharacter = null;
		}
		hunterCharacter = MWPoolManager.Spawn("Hunter", hunter.HunterInfo.Hunter.hunterIdx.ToString(), hunter_tr, -1f, isSpeedProcess: false);
		hunterCharacter.localScale = Vector3.one;
		hunterCharacter.GetComponent<HunterCharacter>().HunterSkillInit(hunter);
		hunterCharacter.GetComponent<HunterCharacter>().SetAnim(Anim_Type.ATTACK_HUNTER);
	}

	private void SetSkillText()
	{
		if (skillText_tr.childCount > 0)
		{
			MWPoolManager.DeSpawn("Effect", skillText_tr.GetChild(0));
			hunterSkillText = null;
		}
		hunterSkillText = MWPoolManager.Spawn("Effect", "Skill_text_" + hunter.HunterInfo.Hunter.skillIdx.ToString(), skillText_tr, -1f, isSpeedProcess: false);
		hunterSkillText.localPosition = Vector3.zero;
		hunterSkillText.localScale = Vector3.one;
	}

	private void OnDisable()
	{
		if (hunter_tr.childCount > 0)
		{
			MWPoolManager.DeSpawn("Hunter", hunter_tr.GetChild(0));
			hunterCharacter = null;
		}
		if (skillText_tr.childCount > 0)
		{
			MWPoolManager.DeSpawn("Effect", skillText_tr.GetChild(0));
			hunterSkillText = null;
		}
	}
}
