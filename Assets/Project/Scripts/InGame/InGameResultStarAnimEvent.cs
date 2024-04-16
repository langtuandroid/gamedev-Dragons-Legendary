using UnityEngine;

public class InGameResultStarAnimEvent : MonoBehaviour
{
	[SerializeField]
	private int starIndex;

	public void OnShowStar()
	{
		switch (starIndex)
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
