using UnityEngine;

public class SkillEffect_Anim : MonoBehaviour
{
	public Hero hunter;

	public Monster[] monster;

	public int damage;

	public void SetMonsterHP_Gauge()
	{
		for (int i = 0; i < monster.Length; i++)
		{
			monster[i].SetMonsterHP_Gauge(damage);
			GameUtil.Check_Property_Damage_UI(hunter, monster[i], damage);
		}
		PuzzlePlayManager.ShakeEffect();
	}

	public void SetMonster(Hero _hunter, Monster[] _monster, int _damage)
	{
		hunter = _hunter;
		monster = _monster;
		damage = _damage;
	}
}
