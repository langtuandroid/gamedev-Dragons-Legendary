public class UnpauseAllAudio : SoundTriggerBase
{
	public enum PauseType
	{
		All,
		MusicOnly,
		AmbienceOnly,
		Category
	}

	public PauseType pauseType;

	public float fadeIn;

	public string categoryName;

	protected override void _OnEventTriggered()
	{
		switch (pauseType)
		{
		case PauseType.All:
			AudioController.UnpauseAllSounds(fadeIn);
			break;
		case PauseType.MusicOnly:
			AudioController.UnpauseSound(fadeIn);
			break;
		case PauseType.AmbienceOnly:
			AudioController.UnpauseAmbienceMusic(fadeIn);
			break;
		case PauseType.Category:
			AudioController.UnpauseByCategory(categoryName, fadeIn);
			break;
		}
	}
}
