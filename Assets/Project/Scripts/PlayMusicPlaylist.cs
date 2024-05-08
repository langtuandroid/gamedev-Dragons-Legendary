public class PlayMusicPlaylist : SoundTriggerBase
{
	public string playListName = "Default";

	protected override void _OnEventTriggered()
	{
		AudioController.PlayMusic(playListName);
	}
}
