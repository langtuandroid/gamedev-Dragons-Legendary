using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SoundItem
{
	[Serializable]
	public enum LoopMode
	{
		DoNotLoop = 0,
		LoopSubitem = 1,
		LoopSequence = 2,
		PlaySequenceAndLoopLast = 4,
		IntroLoopOutroSequence = 5
	}

	[FormerlySerializedAs("Name")] public string _name;

	[FormerlySerializedAs("Loop")] public LoopMode _loopMode;

	[FormerlySerializedAs("loopSequenceCount")] public int _loopSequenceCount;

	[FormerlySerializedAs("loopSequenceOverlap")] public float _loopSequenceOverlap;

	[FormerlySerializedAs("loopSequenceRandomDelay")] public float _loopSequenceRandomDelay;

	[FormerlySerializedAs("loopSequenceRandomPitch")] public float _loopSequenceRandomPitch;

	[FormerlySerializedAs("loopSequenceRandomVolume")] public float _loopSequenceRandomVolume;

	[FormerlySerializedAs("DestroyOnLoad")] public bool IsDestroyOnLoad = true;

	[FormerlySerializedAs("Volume")] public float VolumeValue = 1f;

	[FormerlySerializedAs("SubItemPickMode")] public SoundPickSubItemMode PickMode = SoundPickSubItemMode.RandomNotSameTwice;

	[FormerlySerializedAs("MinTimeBetweenPlayCalls")] public float TimeBetweenCalls = 0.1f;

	[FormerlySerializedAs("MaxInstanceCount")] public int MaxSoundCount;

	[FormerlySerializedAs("Delay")] public float _dalay;

	[FormerlySerializedAs("RandomVolume")] public float _randomVolume;

	[FormerlySerializedAs("RandomPitch")] public float pitch;

	[FormerlySerializedAs("RandomDelay")] public float dalay;

	[FormerlySerializedAs("overrideAudioSourceSettings")] public bool overrideAudioSettings;

	[FormerlySerializedAs("audioSource_MinDistance")] public float _minDistance = 1f;

	[FormerlySerializedAs("audioSource_MaxDistance")] public float _maxDistance = 500f;

	[FormerlySerializedAs("spatialBlend")] public float _spatialBlend;

	[FormerlySerializedAs("subItems")] public SoundSubItem[] _subItemsList;

	internal int _lastChosen = -1;

	internal double _lastPlayedTime = -1.0;

	[NonSerialized]
	private SoundCategory SoundType;

	public SoundCategory category
	{
		get => SoundType;
		private set => SoundType = value;
	}

	public SoundItem()
	{
	}

	public SoundItem(SoundItem orig)
	{
		_name = orig._name;
		_loopMode = orig._loopMode;
		_loopSequenceCount = orig._loopSequenceCount;
		_loopSequenceOverlap = orig._loopSequenceOverlap;
		_loopSequenceRandomDelay = orig._loopSequenceRandomDelay;
		_loopSequenceRandomPitch = orig._loopSequenceRandomPitch;
		_loopSequenceRandomVolume = orig._loopSequenceRandomVolume;
		IsDestroyOnLoad = orig.IsDestroyOnLoad;
		VolumeValue = orig.VolumeValue;
		PickMode = orig.PickMode;
		TimeBetweenCalls = orig.TimeBetweenCalls;
		MaxSoundCount = orig.MaxSoundCount;
		_dalay = orig._dalay;
		_randomVolume = orig._randomVolume;
		pitch = orig.pitch;
		dalay = orig.dalay;
		overrideAudioSettings = orig.overrideAudioSettings;
		_minDistance = orig._minDistance;
		_maxDistance = orig._maxDistance;
		_spatialBlend = orig._spatialBlend;
		for (int i = 0; i < orig._subItemsList.Length; i++)
		{
			ArrayHelper.AddArrayElement(ref _subItemsList, new SoundSubItem(orig._subItemsList[i], this));
		}
	}

	private void Awake()
	{
		if (_loopMode == (LoopMode)3)
		{
			_loopMode = LoopMode.LoopSequence;
		}
		_lastChosen = -1;
	}

	public void ResetTweens()
	{
		_lastChosen = -1;
	}

	internal void Configure(SoundCategory categ)
	{
		category = categ;
		NormalizeItems();
	}

	private void NormalizeItems()
	{
		float num = 0f;
		int num2 = 0;
		bool flag = false;
		SoundSubItem[] array = _subItemsList;
		foreach (SoundSubItem audioSubItem in array)
		{
			if (IsSubItems(audioSubItem) && audioSubItem._disableOtherSubitems)
			{
				flag = true;
				break;
			}
		}
		SoundSubItem[] array2 = _subItemsList;
		foreach (SoundSubItem audioSubItem2 in array2)
		{
			audioSubItem2.Item = this;
			if (IsSubItems(audioSubItem2) && (audioSubItem2._disableOtherSubitems || !flag))
			{
				num += audioSubItem2._probability;
			}
			audioSubItem2._subItemID = num2;
			num2++;
		}
		if (num <= 0f)
		{
			return;
		}
		float num3 = 0f;
		SoundSubItem[] array3 = _subItemsList;
		foreach (SoundSubItem audioSubItem3 in array3)
		{
			if (IsSubItems(audioSubItem3))
			{
				if (audioSubItem3._disableOtherSubitems || !flag)
				{
					num3 += audioSubItem3._probability / num;
				}
				audioSubItem3.SummedProbability = num3;
			}
		}
	}

	private static bool IsSubItems(SoundSubItem item)
	{
		switch (item._soundSubItemType)
		{
		case SoundSubItemType.Clip:
			return item._audioClip != null;
		case SoundSubItemType.Item:
			return item._itemModeAudioID != null && item._itemModeAudioID.Length > 0;
		default:
			return false;
		}
	}

	public void DeloadClips()
	{
		SoundSubItem[] array = _subItemsList;
		foreach (SoundSubItem audioSubItem in array)
		{
			if ((bool)audioSubItem._audioClip)
			{
				if (!audioSubItem._audioClip.preloadAudioData)
				{
					audioSubItem._audioClip.UnloadAudioData();
				}
				else
				{
					Resources.UnloadAsset(audioSubItem._audioClip);
				}
			}
		}
	}
}
