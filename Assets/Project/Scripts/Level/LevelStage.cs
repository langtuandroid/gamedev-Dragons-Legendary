using UnityEngine;
using UnityEngine.UI;

public class LevelStage : MonoBehaviour
{
	[SerializeField]
	private Image imageStage;

	[SerializeField]
	private Text textStageName;

	public void SetData(int stageIdx)
	{
		imageStage.sprite = GameDataManager.GetStageCellSprite(stageIdx - 1);
		textStageName.text = MWLocalize.GetData(GameDataManager.GetDicStageDbData()[stageIdx].stageName);
	}
}
