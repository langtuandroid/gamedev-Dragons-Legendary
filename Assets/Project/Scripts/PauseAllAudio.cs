public class PauseAllAudio : SoundTriggerBase
{
	public enum PauseType
	{
		All,
		MusicOnly,
		AmbienceOnly,
		Category
	}

	public PauseType pauseType;

	public float fadeOut;

	public string categoryName;

	protected override void _OnEventTriggered()
	{
		switch (pauseType)
		{
		case PauseType.All:
			AudioController.PauseAllSounds(fadeOut);
			break;
		case PauseType.MusicOnly:
			AudioController.PauseSound(fadeOut);
			break;
		case PauseType.AmbienceOnly:
			AudioController.PauseAmbienceMusic(fadeOut);
			break;
		case PauseType.Category:
			AudioController.PauseByCategory(categoryName, fadeOut);
			break;
		}
	}
}
