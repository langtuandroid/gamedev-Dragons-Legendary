using UnityEngine;

public class PlayAudio : SoundTriggerBase
{
	public enum PlayPosition
	{
		Global,
		ChildObject,
		ObjectPosition
	}

	public enum SoundType
	{
		SFX,
		Music,
		AmbienceSound
	}

	public string audioID;

	public SoundType soundType;

	public PlayPosition position;

	public float volume = 1f;

	public float delay;

	public float startTime;

	protected override void Awake()
	{
		if (_eventType == Type.OnDestroy && position == PlayPosition.ChildObject)
		{
			position = PlayPosition.ObjectPosition;
			UnityEngine.Debug.LogWarning("OnDestroy event can not be used with ChildObject");
		}
		base.Awake();
	}

	private void _Play()
	{
		switch (position)
		{
		case PlayPosition.Global:
			AudioController.PlayAudio(audioID, volume, delay, startTime);
			break;
		case PlayPosition.ChildObject:
			AudioController.PlayAudio(audioID, base.transform, volume, delay, startTime);
			break;
		case PlayPosition.ObjectPosition:
			AudioController.PlayAudio(audioID, base.transform.position, null, volume, delay, startTime);
			break;
		}
	}

	protected override void _OnEventTriggered()
	{
		if (!string.IsNullOrEmpty(audioID))
		{
			switch (soundType)
			{
			case SoundType.SFX:
				_Play();
				break;
			case SoundType.Music:
				_PlayMusic();
				break;
			case SoundType.AmbienceSound:
				_PlayAmbienceSound();
				break;
			}
		}
	}

	private void _PlayMusic()
	{
		switch (position)
		{
		case PlayPosition.Global:
			AudioController.PlaySound(audioID, volume, delay, startTime);
			break;
		case PlayPosition.ChildObject:
			AudioController.PlaySound(audioID, base.transform, volume, delay, startTime);
			break;
		case PlayPosition.ObjectPosition:
			AudioController.PlaySound(audioID, base.transform.position, null, volume, delay, startTime);
			break;
		}
	}

	private void _PlayAmbienceSound()
	{
		switch (position)
		{
		case PlayPosition.Global:
			AudioController.PlayAmbienceMusic(audioID, volume, delay, startTime);
			break;
		case PlayPosition.ChildObject:
			AudioController.PlayAmbienceMusic(audioID, base.transform, volume, delay, startTime);
			break;
		case PlayPosition.ObjectPosition:
			AudioController.PlayAmbienceMusic(audioID, base.transform.position, null, volume, delay, startTime);
			break;
		}
	}
}
