using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelGameStage : MonoBehaviour
{
	[FormerlySerializedAs("imageStage")] [SerializeField]
	private Image _imageStage;

	[FormerlySerializedAs("textStageName")] [SerializeField]
	private Text _textStageName;

	public void ChangeSetData(int stageIdx)
	{
		_imageStage.sprite = GameDataManager.GetStageCellSprite(stageIdx - 1);
		_textStageName.text = MasterLocalize.GetData(GameDataManager.GetDicStageDbData()[stageIdx].stageName);
	}
}
