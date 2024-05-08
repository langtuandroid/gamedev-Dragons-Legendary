using UnityEngine;
using UnityEngine.Serialization;

public class PuzzleResultStarAnimEvent : MonoBehaviour
{
	[FormerlySerializedAs("starIndex")] [SerializeField]
	private int _starsIndex;

	public void OnShowStar()
	{
		switch (_starsIndex)
		{
		case 0:
			SoundController.EffectSound_Play(EffectSoundType.GetStar1);
			break;
		case 1:
			SoundController.EffectSound_Play(EffectSoundType.GetStar2);
			break;
		case 2:
			SoundController.EffectSound_Play(EffectSoundType.GetStar3);
			break;
		}
	}
}
