public class StopAudio : SoundTriggerBase
{
	public string audioID;

	public float fadeOut;

	protected override void _OnEventTriggered()
	{
		AudioController.StopSound(audioID, fadeOut);
	}
}
