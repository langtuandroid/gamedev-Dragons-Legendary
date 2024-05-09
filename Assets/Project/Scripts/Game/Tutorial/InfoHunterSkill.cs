using UnityEngine;
using UnityEngine.Serialization;

public class InfoHunterSkill : MonoBehaviour
{
	[FormerlySerializedAs("userSkillBT")] [SerializeField]
	private Transform _userSkill;

	[FormerlySerializedAs("userSkillHunter")] [SerializeField]
	private Transform _hunterSkill;

	public void Open(int _seq)
	{
		switch (_seq)
		{
		case 1:
			UnityEngine.Debug.Log("USER SKILL TUTORIAL 5555");
			InfoManager.ContinueStep();
			break;
		case 2:
			UnityEngine.Debug.Log("TutorialHunterSkill - 2");
			_hunterSkill = PuzzlePlayManager.CheckIsUseHunterSkill();
			InfoManager.SortLightSprite(_hunterSkill);
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 3:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 4:
			InfoManager.SetClickDimmed(isClick: true);
			break;
		case 5:
			PuzzlePlayManager.OnHunterSkill = null;
			InfoManager.SetClickDimmed(isClick: false);
			_userSkill.gameObject.SetActive(value: true);
			_userSkill.position = _hunterSkill.position;
			PuzzlePlayManager.OnHunterSkillEventComplete = OnHunterEventComplete;
			break;
		}
	}

	private void OnHunterEventComplete()
	{
		InfoManager.ReturnBackAll();
		_userSkill.gameObject.SetActive(value: false);
		InfoManager.CloseTutorial();
		PuzzlePlayManager.OnHunterSkillEventComplete = null;
		InfoManager.SaveAllData();
		InfoManager.EventTutorialEbd();
	}

	public void UseHunterSkillForTutorial()
	{
		PuzzlePlayManager.TutorialHunterSkill(_hunterSkill.GetComponent<Hero>());
	}
}
