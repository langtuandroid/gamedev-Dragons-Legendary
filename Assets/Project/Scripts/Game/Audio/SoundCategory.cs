using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[Serializable]
public class SoundCategory
{
	[FormerlySerializedAs("Name")] public string _name;

	private SoundCategory _parentCategory;

	private SoundFader _audioFader;

	[SerializeField]
	private string _parentCategoryName;

	public GameObject AudioObjectPrefab;

	public SoundItem[] AudioItems;

	[SerializeField]
	private float _volume = 1f;

	[FormerlySerializedAs("audioMixerGroup")] public AudioMixerGroup _audioMixerGroup;

	public float SoundVolume
	{
		get => _volume;
		set
		{
			_volume = value;
			SaveVolumeChange();
		}
	}

	public float SoundVolumeTotal
	{
		get
		{
			FadeUpdate();
			float num = audioFade.Get();
			if (ParentType != null)
			{
				return ParentType.SoundVolumeTotal * _volume * num;
			}
			return _volume * num;
		}
	}

	public SoundCategory ParentType
	{
		get
		{
			if (string.IsNullOrEmpty(_parentCategoryName))
			{
				return null;
			}
			if (_parentCategory == null)
			{
				if (AudioController != null)
				{
					_parentCategory = AudioController.ChangeCategory(_parentCategoryName);
				}
				else
				{
					UnityEngine.Debug.LogWarning("_audioController == null");
				}
			}
			return _parentCategory;
		}
		set
		{
			_parentCategory = value;
			if (value != null)
			{
				_parentCategoryName = _parentCategory._name;
			}
			else
			{
				_parentCategoryName = null;
			}
		}
	}

	private SoundFader audioFade
	{
		get
		{
			if (_audioFader == null)
			{
				_audioFader = new SoundFader();
			}
			return _audioFader;
		}
	}

	public AudioController AudioController
	{
		get;
		set;
	}
	
	public SoundCategory(AudioController audioController)
	{
		this.AudioController = audioController;
	}

	public GameObject AudioPrefab()
	{
		if (AudioObjectPrefab != null)
		{
			return AudioObjectPrefab;
		}
		if (ParentType != null)
		{
			return ParentType.AudioPrefab();
		}
		return AudioController.AudioPrefab;
	}

	public AudioMixerGroup MixerGroup()
	{
		if (_audioMixerGroup != null)
		{
			return _audioMixerGroup;
		}
		if (ParentType != null)
		{
			return ParentType.MixerGroup();
		}
		return null;
	}

	internal void SetAudioSystem(Dictionary<string, SoundItem> audioItemsDict)
	{
		if (AudioItems == null)
		{
			return;
		}
		SoundItem[] audioItems = AudioItems;
		foreach (SoundItem audioItem in audioItems)
		{
			if (audioItem != null)
			{
				audioItem.Configure(this);
				if (audioItemsDict != null)
				{
					try
					{
						audioItemsDict.Add(audioItem._name, audioItem);
					}
					catch (ArgumentException)
					{
						UnityEngine.Debug.LogWarning("Multiple audio items with name '" + audioItem._name + "'", AudioController);
					}
				}
			}
		}
	}

	internal int AudioIndex(SoundItem audioItem)
	{
		if (AudioItems == null)
		{
			return -1;
		}
		for (int i = 0; i < AudioItems.Length; i++)
		{
			if (audioItem == AudioItems[i])
			{
				return i;
			}
		}
		return -1;
	}

	private void SaveVolumeChange()
	{
		List<SoundObject> playingAudioObjects = AudioController.GetObjectByIndex();
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (ParentOfCategory(audioObject.Category, this))
			{
				audioObject.VolumeApply();
			}
		}
	}

	private bool ParentOfCategory(SoundCategory toTest, SoundCategory parent)
	{
		for (SoundCategory audioCategory = toTest; audioCategory != null; audioCategory = audioCategory.ParentType)
		{
			if (audioCategory == parent)
			{
				return true;
			}
		}
		return false;
	}

	public void LoadAllClips()
	{
		for (int i = 0; i < AudioItems.Length; i++)
		{
			AudioItems[i].DeloadClips();
		}
	}

	public void FadeInSound(float fadeInTime, bool stopCurrentFadeOut = true)
	{
		FadeUpdate();
		audioFade.FadeSoundIn(fadeInTime, stopCurrentFadeOut);
	}

	public void FadeOutSound(float fadeOutLength, float startToFadeTime = 0f)
	{
		FadeUpdate();
		audioFade.FadeSoundOut(fadeOutLength, startToFadeTime);
	}

	private void FadeUpdate()
	{
		audioFade.time = AudioController.InfoSystemTime;
	}
}
