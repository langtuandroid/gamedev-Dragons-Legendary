public class StopAllAudio : SoundTriggerBase
{
	public float fadeOut;

	protected override void _OnEventTriggered()
	{
		AudioController.StopAllSounds(fadeOut);
	}
}
