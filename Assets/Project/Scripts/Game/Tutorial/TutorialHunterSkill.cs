using UnityEngine;

public class TutorialHunterSkill : MonoBehaviour
{
	[SerializeField]
	private Transform userSkillBT;

	[SerializeField]
	private Transform userSkillHunter;

	public void Show(int _seq)
	{
		switch (_seq)
		{
		case 1:
			UnityEngine.Debug.Log("USER SKILL TUTORIAL 5555");
			TutorialManager.NextSep();
			break;
		case 2:
			UnityEngine.Debug.Log("TutorialHunterSkill - 2");
			userSkillHunter = InGamePlayManager.CheckIsUseHunterSkill();
			TutorialManager.ShowAndSortHighLightSprite(userSkillHunter);
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 3:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 4:
			TutorialManager.SetDimmedClick(isClick: true);
			break;
		case 5:
			InGamePlayManager.HunterSkillEvent = null;
			TutorialManager.SetDimmedClick(isClick: false);
			userSkillBT.gameObject.SetActive(value: true);
			userSkillBT.position = userSkillHunter.position;
			InGamePlayManager.HunterSkillEventComplete = HunterSkillEventComplete;
			break;
		}
	}

	private void HunterSkillEventComplete()
	{
		TutorialManager.ReturnHighLightSpriteList();
		userSkillBT.gameObject.SetActive(value: false);
		TutorialManager.HideTutorial();
		InGamePlayManager.HunterSkillEventComplete = null;
		TutorialManager.SaveTutorial();
		TutorialManager.EndEventTutorial();
	}

	public void UseHunterSkillForTutorial()
	{
		InGamePlayManager.UseHunterSkillForTutorial(userSkillHunter.GetComponent<Hero>());
	}
}
