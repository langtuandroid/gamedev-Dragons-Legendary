using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SoundSubItem
{
	[FormerlySerializedAs("SubItemType")] public SoundSubItemType _soundSubItemType;

	[FormerlySerializedAs("Probability")] public float _probability = 1f;

	[FormerlySerializedAs("DisableOtherSubitems")] public bool _disableOtherSubitems;

	[FormerlySerializedAs("ItemModeAudioID")] public string _itemModeAudioID;

	[FormerlySerializedAs("Clip")] public AudioClip _audioClip;

	[FormerlySerializedAs("Volume")] public float _volume = 1f;

	[FormerlySerializedAs("PitchShift")] public float _pitchShift;

	[FormerlySerializedAs("Pan2D")] public float _pan2D;

	[FormerlySerializedAs("Delay")] public float _delay;

	[FormerlySerializedAs("RandomPitch")] public float _randomPitch;

	[FormerlySerializedAs("RandomVolume")] public float _randomVolume;

	[FormerlySerializedAs("RandomDelay")] public float _randomDelay;

	[FormerlySerializedAs("ClipStopTime")] public float _clipStopTime;

	[FormerlySerializedAs("ClipStartTime")] public float _clipStartTime;

	[FormerlySerializedAs("FadeIn")] public float _fadeIn;

	[FormerlySerializedAs("FadeOut")] public float _fadeOut;

	[FormerlySerializedAs("RandomStartPosition")] public bool _randomStartPos;

	[FormerlySerializedAs("individualSettings")] public List<string> _individualSettings = new List<string>();

	private float _summedProbability = -1f;

	internal int _subItemID;

	[NonSerialized]
	private SoundItem _item;

	internal float SummedProbability
	{
		get => _summedProbability;
		set => _summedProbability = value;
	}

	public SoundItem Item
	{
		get => _item;
		internal set => _item = value;
	}

	public SoundSubItem()
	{
	}

	public SoundSubItem(SoundSubItem orig, SoundItem item)
	{
		_soundSubItemType = orig._soundSubItemType;
		if (_soundSubItemType == SoundSubItemType.Clip)
		{
			_audioClip = orig._audioClip;
		}
		else if (_soundSubItemType == SoundSubItemType.Item)
		{
			_itemModeAudioID = orig._itemModeAudioID;
		}
		_probability = orig._probability;
		_disableOtherSubitems = orig._disableOtherSubitems;
		_audioClip = orig._audioClip;
		_volume = orig._volume;
		_pitchShift = orig._pitchShift;
		_pan2D = orig._pan2D;
		_delay = orig._delay;
		_randomPitch = orig._randomPitch;
		_randomVolume = orig._randomVolume;
		_randomDelay = orig._randomDelay;
		_clipStopTime = orig._clipStopTime;
		_clipStartTime = orig._clipStartTime;
		_fadeIn = orig._fadeIn;
		_fadeOut = orig._fadeOut;
		_randomStartPos = orig._randomStartPos;
		for (int i = 0; i < orig._individualSettings.Count; i++)
		{
			_individualSettings.Add(orig._individualSettings[i]);
		}
		this.Item = item;
	}

	public override string ToString()
	{
		if (_soundSubItemType == SoundSubItemType.Clip)
		{
			return "CLIP: " + _audioClip.name;
		}
		return "ITEM: " + _itemModeAudioID;
	}
}
