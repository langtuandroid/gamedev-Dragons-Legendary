using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[AddComponentMenu("ClockStone/Audio/AudioController")]
public class AudioController : SingletonMonoBehaviour<AudioController>, ISerializationCallbackReceiver
{
	[FormerlySerializedAs("AudioObjectPrefab")] public GameObject AudioPrefab;

	public bool Persistent;

	[FormerlySerializedAs("UnloadAudioClipsOnDestroy")] public bool IsUnloadAudioClipsOnDestroy;

	[FormerlySerializedAs("UsePooledAudioObjects")] public bool IsUsePooledAudioObjects = true;

	[FormerlySerializedAs("PlayWithZeroVolume")] public bool IsPlayWithZeroVolume;

	[FormerlySerializedAs("EqualPowerCrossfade")] public bool IsEqualPowerCrossfade;

	[FormerlySerializedAs("musicCrossFadeTime")] public float crossFade;

	[FormerlySerializedAs("ambienceSoundCrossFadeTime")] public float ambienceCrossFade;

	[FormerlySerializedAs("specifyCrossFadeInAndOutSeperately")] public bool specifyCrossFade;
	
	private float _croudFadeTime;
	private float _crossFadeTimeOut;
	private float _crossFadeTimeIn;
	private float _crossFadeTimeOutSound;

	[FormerlySerializedAs("AudioCategories")] public SoundCategory[] soundCategory;

	[FormerlySerializedAs("musicPlaylists")] public Playlist[] playList = new Playlist[1];

	[Obsolete]
	public string[] musicPlaylist;

	[FormerlySerializedAs("loopPlaylist")] public bool loopList;

	[FormerlySerializedAs("shufflePlaylist")] public bool shuffleList;

	[FormerlySerializedAs("crossfadePlaylist")] public bool crossFadeLits;

	[FormerlySerializedAs("delayBetweenPlaylistTracks")] public float trackDelay = 1f;

	protected static PoolableReference<SoundObject> currentMusicReference = new PoolableReference<SoundObject>();

	protected static PoolableReference<SoundObject> CurrentAmbienceReference = new PoolableReference<SoundObject>();

	private string _playListName;

	protected AudioListener _audioListener;

	private static Transform _soundParent = null;

	private static Transform _ambienceParent = null;

	private bool _isEnabledValue = true;

	private bool _isAmbienceDisabled = true;

	private bool _isMuted;

	private bool _validatingSound;
	
	private bool _isAudioController;
	
	private bool _audioDisabled;

	private Dictionary<string, SoundItem> _itemsDict;

	private static List<int> _playedList;

	private static bool _playingList = false;
	
	private float _volumeValueValue = 1f;

	private static double _infoSystemTime;

	private static double _systemType = -1.0;

	private static double _deltaTime = -1.0;

	private static List<AudioController> _registerController;

	private List<AudioController> _audioController;

	private bool isAudioController => _isAudioController;

	public float VolumeValue
	{
		get => _volumeValueValue;
		private set
		{
			if (value != _volumeValueValue)
			{
				_volumeValueValue = value;
				_ApplyVolumeChange();
			}
		}
	}

	private bool EnabledValue
	{
		get => _isEnabledValue;
		set
		{
			if (_isEnabledValue == value)
			{
				return;
			}
			_isEnabledValue = value;
			if (!thisMusic)
			{
				return;
			}
			if (value)
			{
				if (thisMusic.IsPaused())
				{
					thisMusic.PlaySound();
				}
			}
			else
			{
				thisMusic.PauseSound();
			}
		}
	}

	private bool IsAmbienceDisabled
	{
		get => _isAmbienceDisabled;
		set
		{
			if (_isAmbienceDisabled == value)
			{
				return;
			}
			_isAmbienceDisabled = value;
			if (!AmbienceSound)
			{
				return;
			}
			if (value)
			{
				if (AmbienceSound.IsPaused())
				{
					AmbienceSound.PlaySound();
				}
			}
			else
			{
				AmbienceSound.PauseSound();
			}
		}
	}

	public bool IsMuted
	{
		get => _isMuted;
		private set
		{
			_isMuted = value;
			_ApplyVolumeChange();
		}
	}

	public float CrossFadeIn
	{
		get
		{
			if (specifyCrossFade)
			{
				return _croudFadeTime;
			}
			return crossFade;
		}
	}

	public float CrossFadeOut
	{
		get
		{
			if (specifyCrossFade)
			{
				return _crossFadeTimeOut;
			}
			return crossFade;
		}
	}

	private float FadeTimeIn
	{
		get
		{
			if (specifyCrossFade)
			{
				return _crossFadeTimeIn;
			}
			return ambienceCrossFade;
		}
	}

	private float CrossFadeTimeOut
	{
		get
		{
			if (specifyCrossFade)
			{
				return _crossFadeTimeOutSound;
			}
			return ambienceCrossFade;
		}
	}

	public static double InfoSystemTime => _infoSystemTime;

	public static double DeltaTime => _deltaTime;

	private static SoundObject thisMusic
	{
		get => currentMusicReference.Get();
		set
		{
			currentMusicReference.Set(value, allowNonePoolable: true);
		}
	}

	private static SoundObject AmbienceSound
	{
		get => CurrentAmbienceReference.Get();
		set
		{
			CurrentAmbienceReference.Set(value, allowNonePoolable: true);
		}
	}

	public override bool isSingleton => !_isAudioController;

	public static SoundObject PlaySound(string audioID, float volume = 1f, float delay = 0f, float startTime = 0f)
	{
		_playingList = false;
		return SingletonMonoBehaviour<AudioController>.Instance._PlayMusic(audioID, volume, delay, startTime);
	}

	public static SoundObject PlaySound(string audioID, Vector3 worldPosition, Transform parentObj = null, float volume = 1f, float delay = 0f, float startTime = 0f)
	{
		_playingList = false;
		return SingletonMonoBehaviour<AudioController>.Instance._PlayMusic(audioID, worldPosition, parentObj, volume, delay, startTime);
	}

	public static SoundObject PlaySound(string audioID, Transform parentObj, float volume = 1f, float delay = 0f, float startTime = 0f)
	{
		_playingList = false;
		return SingletonMonoBehaviour<AudioController>.Instance._PlayMusic(audioID, parentObj.position, parentObj, volume, delay, startTime);
	}

	private static bool StopSound(float fadeOut)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._StopMusic(fadeOut);
	}

	public static bool PauseSound(float fadeOut = 0f)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._PauseMusic(fadeOut);
	}

	public static bool UnpauseSound(float fadeIn = 0f)
	{
		if (!SingletonMonoBehaviour<AudioController>.Instance._isEnabledValue)
		{
			return false;
		}
		if (thisMusic != null && thisMusic.IsPaused())
		{
			thisMusic.ContinueSound(fadeIn);
			return true;
		}
		return false;
	}

	public static SoundObject PlayAmbienceMusic(string audioID, float volume = 1f, float delay = 0f, float startTime = 0f)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._PlayAmbienceSound(audioID, volume, delay, startTime);
	}

	public static SoundObject PlayAmbienceMusic(string audioID, Vector3 worldPosition, Transform parentObj = null, float volume = 1f, float delay = 0f, float startTime = 0f)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._PlayAmbienceSound(audioID, worldPosition, parentObj, volume, delay, startTime);
	}

	public static SoundObject PlayAmbienceMusic(string audioID, Transform parentObj, float volume = 1f, float delay = 0f, float startTime = 0f)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._PlayAmbienceSound(audioID, parentObj.position, parentObj, volume, delay, startTime);
	}

	public static bool StopAmbienceMusic(float fadeOut)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._StopAmbienceSound(fadeOut);
	}

	public static bool PauseAmbienceMusic(float fadeOut = 0f)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._PauseAmbienceSound(fadeOut);
	}

	public static bool UnpauseAmbienceMusic(float fadeIn = 0f)
	{
		if (!SingletonMonoBehaviour<AudioController>.Instance._isAmbienceDisabled)
		{
			return false;
		}
		if (AmbienceSound != null && AmbienceSound.IsPaused())
		{
			AmbienceSound.ContinueSound(fadeIn);
			return true;
		}
		return false;
	}

	private Playlist GetPlayList()
	{
		if (string.IsNullOrEmpty(_playListName))
		{
			return null;
		}
		return GetListByName(_playListName);
	}

	private Playlist GetListByName(string playlistName)
	{
		for (int i = 0; i < playList.Length; i++)
		{
			if (playlistName == playList[i].name)
			{
				return playList[i];
			}
		}
		if (_audioController != null)
		{
			for (int j = 0; j < _audioController.Count; j++)
			{
				AudioController audioController = _audioController[j];
				for (int k = 0; k < audioController.playList.Length; k++)
				{
					if (playlistName == audioController.playList[k].name)
					{
						return audioController.playList[k];
					}
				}
			}
		}
		return null;
	}

	public static bool PlaceCurrentPlaylist(string playlistName)
	{
		if (SingletonMonoBehaviour<AudioController>.Instance.GetListByName(playlistName) == null)
		{
			UnityEngine.Debug.LogError("Playlist with name " + playlistName + " not found");
			return false;
		}
		SingletonMonoBehaviour<AudioController>.Instance._playListName = playlistName;
		return true;
	}

	public static SoundObject PlayMusic(string playlistName = null)
	{
		if (!string.IsNullOrEmpty(playlistName) && !PlaceCurrentPlaylist(playlistName))
		{
			return null;
		}
		return SingletonMonoBehaviour<AudioController>.Instance._PlayMusicPlaylist();
	}

	public static bool IsPlaylistPlaying()
	{
		if (_playingList)
		{
			if (!thisMusic)
			{
				_playingList = false;
				return false;
			}
			return true;
		}
		return false;
	}

	public static void AddPlaylist(string playlistName, string[] audioItemIDs)
	{
		Playlist elToAdd = new Playlist(playlistName, audioItemIDs);
		ArrayHelper.AddArrayElement(ref SingletonMonoBehaviour<AudioController>.Instance.playList, elToAdd);
	}

	public static SoundObject PlayAudio(string audioID)
	{
		AudioListener currentAudioListener = GetCurrentAudioListener();
		if (currentAudioListener == null)
		{
			UnityEngine.Debug.LogWarning("No AudioListener found in the scene");
			return null;
		}
		return PlayAudio(audioID, currentAudioListener.transform.position + currentAudioListener.transform.forward, null, 1f);
	}

	public static SoundObject PlayAudio(string audioID, float volume, float delay = 0f, float startTime = 0f)
	{
		AudioListener currentAudioListener = GetCurrentAudioListener();
		if (currentAudioListener == null)
		{
			UnityEngine.Debug.LogWarning("No AudioListener found in the scene");
			return null;
		}
		return PlayAudio(audioID, currentAudioListener.transform.position + currentAudioListener.transform.forward, null, volume, delay, startTime);
	}

	public static SoundObject PlayAudio(string audioID, Transform parentObj)
	{
		return PlayAudio(audioID, parentObj.position, parentObj, 1f);
	}

	public static SoundObject PlayAudio(string audioID, Transform parentObj, float volume, float delay = 0f, float startTime = 0f)
	{
		return PlayAudio(audioID, parentObj.position, parentObj, volume, delay, startTime);
	}

	public static SoundObject PlayAudio(string audioID, Vector3 worldPosition, Transform parentObj = null)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._PlayEx(audioID, AudioChannelType.Default, 1f, worldPosition, parentObj, 0f, 0f, playWithoutAudioObject: false);
	}

	public static SoundObject PlayAudio(string audioID, Vector3 worldPosition, Transform parentObj, float volume, float delay = 0f, float startTime = 0f)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._PlayEx(audioID, AudioChannelType.Default, volume, worldPosition, parentObj, delay, startTime, playWithoutAudioObject: false);
	}

	public static SoundObject PlayAfterTime(string audioID, double dspTime, Vector3 worldPosition, Transform parentObj = null, float volume = 1f, float startTime = 0f)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._PlayEx(audioID, AudioChannelType.Default, volume, worldPosition, parentObj, 0f, startTime, playWithoutAudioObject: false, dspTime);
	}

	public static SoundObject PlayAfterTime(string audioID, SoundObject playingAudio, double deltaDspTime = 0.0, float volume = 1f, float startTime = 0f)
	{
		double num = AudioSettings.dspTime;
		if (playingAudio.IsSoundPlaying())
		{
			num += (double)playingAudio.timeUntilEnd;
		}
		num += deltaDspTime;
		return PlayAfterTime(audioID, num, playingAudio.transform.position, playingAudio.transform.parent, volume, startTime);
	}

	public static bool StopSound(string audioID, float fadeOutLength)
	{
		SoundItem audioItem = SingletonMonoBehaviour<AudioController>.Instance._GetAudioItem(audioID);
		if (audioItem == null)
		{
			UnityEngine.Debug.LogWarning("Audio item with name '" + audioID + "' does not exist");
			return false;
		}
		List<SoundObject> playingAudioObjects = GetPlayingAudioObjects(audioID);
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (fadeOutLength < 0f)
			{
				audioObject.StopSound();
			}
			else
			{
				audioObject.StopSound(fadeOutLength);
			}
		}
		return playingAudioObjects.Count > 0;
	}

	public static bool StopSound(string audioID)
	{
		return StopSound(audioID, -1f);
	}

	public static void StopAllSounds(float fadeOutLength)
	{
		SingletonMonoBehaviour<AudioController>.Instance._StopMusic(fadeOutLength);
		SingletonMonoBehaviour<AudioController>.Instance._StopAmbienceSound(fadeOutLength);
		List<SoundObject> playingAudioObjects = GetPlayingAudioObjects();
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (audioObject != null)
			{
				audioObject.StopSound(fadeOutLength);
			}
		}
	}

	public static void StopAllSounds()
	{
		StopAllSounds(-1f);
	}

	public static void PauseAllSounds(float fadeOutLength = 0f)
	{
		SingletonMonoBehaviour<AudioController>.Instance._PauseMusic(fadeOutLength);
		SingletonMonoBehaviour<AudioController>.Instance._PauseAmbienceSound(fadeOutLength);
		List<SoundObject> playingAudioObjects = GetPlayingAudioObjects();
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (audioObject != null)
			{
				audioObject.PauseSound(fadeOutLength);
			}
		}
	}

	public static void UnpauseAllSounds(float fadeInLength = 0f)
	{
		UnpauseSound(fadeInLength);
		UnpauseAmbienceMusic(fadeInLength);
		List<SoundObject> playingAudioObjects = GetPlayingAudioObjects(includePausedAudio: true);
		AudioController instance = SingletonMonoBehaviour<AudioController>.Instance;
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (audioObject != null && audioObject.IsPaused() && (instance.EnabledValue || !(thisMusic == audioObject)) && (instance.IsAmbienceDisabled || !(AmbienceSound == audioObject)))
			{
				audioObject.ContinueSound(fadeInLength);
			}
		}
	}

	public static void PauseByCategory(string categoryName, float fadeOutLength = 0f)
	{
		if (thisMusic != null && thisMusic.Category._name == categoryName)
		{
			PauseSound(fadeOutLength);
		}
		if (AmbienceSound != null && AmbienceSound.Category._name == categoryName)
		{
			PauseAmbienceMusic(fadeOutLength);
		}
		List<SoundObject> playingAudioObjectsInCategory = GetPlayingAudioObjectsInCategory(categoryName);
		for (int i = 0; i < playingAudioObjectsInCategory.Count; i++)
		{
			SoundObject audioObject = playingAudioObjectsInCategory[i];
			audioObject.PauseSound(fadeOutLength);
		}
	}

	public static void UnpauseByCategory(string categoryName, float fadeInLength = 0f)
	{
		if (thisMusic != null && thisMusic.Category._name == categoryName)
		{
			UnpauseSound(fadeInLength);
		}
		if (AmbienceSound != null && AmbienceSound.Category._name == categoryName)
		{
			UnpauseAmbienceMusic(fadeInLength);
		}
		List<SoundObject> playingAudioObjectsInCategory = GetPlayingAudioObjectsInCategory(categoryName, includePausedAudio: true);
		for (int i = 0; i < playingAudioObjectsInCategory.Count; i++)
		{
			SoundObject audioObject = playingAudioObjectsInCategory[i];
			if (audioObject.IsPaused())
			{
				audioObject.ContinueSound(fadeInLength);
			}
		}
	}

	public static void StopCategory(string categoryName, float fadeOutLength = 0f)
	{
		if (thisMusic != null && thisMusic.Category._name == categoryName)
		{
			StopSound(fadeOutLength);
		}
		if (AmbienceSound != null && AmbienceSound.Category._name == categoryName)
		{
			StopAmbienceMusic(fadeOutLength);
		}
		List<SoundObject> playingAudioObjectsInCategory = GetPlayingAudioObjectsInCategory(categoryName);
		for (int i = 0; i < playingAudioObjectsInCategory.Count; i++)
		{
			SoundObject audioObject = playingAudioObjectsInCategory[i];
			audioObject.StopSound(fadeOutLength);
		}
	}

	public static bool IsPlaying(string audioID)
	{
		return GetPlayingAudioObjects(audioID).Count > 0;
	}

	public static List<SoundObject> GetPlayingAudioObjects(string audioID, bool includePausedAudio = false)
	{
		List<SoundObject> playingAudioObjects = GetPlayingAudioObjects(includePausedAudio);
		List<SoundObject> list = new List<SoundObject>(playingAudioObjects.Count);
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (audioObject != null && audioObject.IDAudio == audioID)
			{
				list.Add(audioObject);
			}
		}
		return list;
	}

	public static List<SoundObject> GetPlayingAudioObjectsInCategory(string categoryName, bool includePausedAudio = false)
	{
		List<SoundObject> playingAudioObjects = GetPlayingAudioObjects(includePausedAudio);
		List<SoundObject> list = new List<SoundObject>(playingAudioObjects.Count);
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (audioObject != null && audioObject.DoesBelongToCategory(categoryName))
			{
				list.Add(audioObject);
			}
		}
		return list;
	}

	public static List<SoundObject> GetPlayingAudioObjects(bool includePausedAudio = false)
	{
		object[] allOfType = RegisteredComponentController.GetAllOfType(typeof(SoundObject));
		List<SoundObject> list = new List<SoundObject>(allOfType.Length);
		for (int i = 0; i < allOfType.Length; i++)
		{
			SoundObject audioObject = (SoundObject)allOfType[i];
			if (audioObject.IsSoundPlaying() || (includePausedAudio && audioObject.IsPaused()))
			{
				list.Add(audioObject);
			}
		}
		return list;
	}

	public static int GetPlayingAudioObjectsCount(string audioID, bool includePausedAudio = false)
	{
		List<SoundObject> playingAudioObjects = GetPlayingAudioObjects(includePausedAudio);
		int num = 0;
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (audioObject != null && audioObject.IDAudio == audioID)
			{
				num++;
			}
		}
		return num;
	}

	public static void EnableMusic(bool b)
	{
		SingletonMonoBehaviour<AudioController>.Instance.EnabledValue = b;
	}

	public static void EnableAmbienceSound(bool b)
	{
		SingletonMonoBehaviour<AudioController>.Instance.IsAmbienceDisabled = b;
	}

	public static void MuteSound(bool b)
	{
		SingletonMonoBehaviour<AudioController>.Instance.IsMuted = b;
	}

	public static bool IsMusicEnabled()
	{
		return SingletonMonoBehaviour<AudioController>.Instance.EnabledValue;
	}

	public static bool IsAmbienceSoundEnabled()
	{
		return SingletonMonoBehaviour<AudioController>.Instance.IsAmbienceDisabled;
	}

	public static bool IsSoundMuted()
	{
		return SingletonMonoBehaviour<AudioController>.Instance.IsMuted;
	}

	public static AudioListener GetCurrentAudioListener()
	{
		AudioController instance = SingletonMonoBehaviour<AudioController>.Instance;
		if (instance._audioListener != null && instance._audioListener.gameObject == null)
		{
			instance._audioListener = null;
		}
		if (instance._audioListener == null)
		{
			instance._audioListener = (AudioListener)UnityEngine.Object.FindObjectOfType(typeof(AudioListener));
		}
		return instance._audioListener;
	}

	public static SoundObject GetCurrentMusic()
	{
		return thisMusic;
	}

	public static SoundObject GetCurrentAmbienceSound()
	{
		return AmbienceSound;
	}

	public static SoundCategory GetCategory(string name)
	{
		AudioController instance = SingletonMonoBehaviour<AudioController>.Instance;
		SoundCategory audioCategory = instance._GetCategory(name);
		if (audioCategory != null)
		{
			return audioCategory;
		}
		if (instance._audioController != null)
		{
			for (int i = 0; i < instance._audioController.Count; i++)
			{
				AudioController audioController = instance._audioController[i];
				audioCategory = audioController._GetCategory(name);
				if (audioCategory != null)
				{
					return audioCategory;
				}
			}
		}
		return null;
	}

	public static void SetCategoryVolume(string name, float volume)
	{
		List<SoundCategory> list = _GetAllCategories(name);
		if (list.Count == 0)
		{
			UnityEngine.Debug.LogWarning("No audio category with name " + name);
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SoundVolume = volume;
		}
	}

	public static float GetCategoryVolume(string name)
	{
		SoundCategory category = GetCategory(name);
		if (category != null)
		{
			return category.SoundVolume;
		}
		UnityEngine.Debug.LogWarning("No audio category with name " + name);
		return 0f;
	}

	public static void FadeOutCategory(string name, float fadeOutLength, float startToFadeTime = 0f)
	{
		List<SoundCategory> list = _GetAllCategories(name);
		if (list.Count == 0)
		{
			UnityEngine.Debug.LogWarning("No audio category with name " + name);
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].FadeOutSound(fadeOutLength, startToFadeTime);
		}
	}

	public static void FadeInCategory(string name, float fadeInTime, bool stopCurrentFadeOut = true)
	{
		List<SoundCategory> list = _GetAllCategories(name);
		if (list.Count == 0)
		{
			UnityEngine.Debug.LogWarning("No audio category with name " + name);
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].FadeInSound(fadeInTime, stopCurrentFadeOut);
		}
	}

	public static void SetGlobalVolume(float volume)
	{
		AudioController instance = SingletonMonoBehaviour<AudioController>.Instance;
		instance.VolumeValue = volume;
		if (instance._audioController != null)
		{
			for (int i = 0; i < instance._audioController.Count; i++)
			{
				AudioController audioController = instance._audioController[i];
				audioController.VolumeValue = volume;
			}
		}
	}

	public static float GetGlobalVolume()
	{
		return SingletonMonoBehaviour<AudioController>.Instance.VolumeValue;
	}

	public static SoundCategory NewCategory(string categoryName)
	{
		int num = (SingletonMonoBehaviour<AudioController>.Instance.soundCategory != null) ? SingletonMonoBehaviour<AudioController>.Instance.soundCategory.Length : 0;
		SoundCategory[] audioCategories = SingletonMonoBehaviour<AudioController>.Instance.soundCategory;
		SingletonMonoBehaviour<AudioController>.Instance.soundCategory = new SoundCategory[num + 1];
		if (num > 0)
		{
			audioCategories.CopyTo(SingletonMonoBehaviour<AudioController>.Instance.soundCategory, 0);
		}
		SoundCategory audioCategory = new SoundCategory(SingletonMonoBehaviour<AudioController>.Instance);
		audioCategory._name = categoryName;
		SingletonMonoBehaviour<AudioController>.Instance.soundCategory[num] = audioCategory;
		SingletonMonoBehaviour<AudioController>.Instance._InvalidateCategories();
		return audioCategory;
	}

	public static void RemoveCategory(string categoryName)
	{
		int num = -1;
		int num2 = (SingletonMonoBehaviour<AudioController>.Instance.soundCategory != null) ? SingletonMonoBehaviour<AudioController>.Instance.soundCategory.Length : 0;
		for (int i = 0; i < num2; i++)
		{
			if (SingletonMonoBehaviour<AudioController>.Instance.soundCategory[i]._name == categoryName)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			UnityEngine.Debug.LogError("AudioCategory does not exist: " + categoryName);
			return;
		}
		SoundCategory[] array = new SoundCategory[SingletonMonoBehaviour<AudioController>.Instance.soundCategory.Length - 1];
		for (int i = 0; i < num; i++)
		{
			array[i] = SingletonMonoBehaviour<AudioController>.Instance.soundCategory[i];
		}
		for (int i = num + 1; i < SingletonMonoBehaviour<AudioController>.Instance.soundCategory.Length; i++)
		{
			array[i - 1] = SingletonMonoBehaviour<AudioController>.Instance.soundCategory[i];
		}
		SingletonMonoBehaviour<AudioController>.Instance.soundCategory = array;
		SingletonMonoBehaviour<AudioController>.Instance._InvalidateCategories();
	}

	public static void AddToCategory(SoundCategory category, SoundItem audioItem)
	{
		int num = (category.AudioItems != null) ? category.AudioItems.Length : 0;
		SoundItem[] audioItems = category.AudioItems;
		category.AudioItems = new SoundItem[num + 1];
		if (num > 0)
		{
			audioItems.CopyTo(category.AudioItems, 0);
		}
		category.AudioItems[num] = audioItem;
		SingletonMonoBehaviour<AudioController>.Instance._InvalidateCategories();
	}

	public static SoundItem AddToCategory(SoundCategory category, AudioClip audioClip, string audioID)
	{
		SoundItem audioItem = new SoundItem();
		audioItem._name = audioID;
		audioItem._subItemsList = new SoundSubItem[1];
		SoundSubItem audioSubItem = new SoundSubItem();
		audioSubItem._audioClip = audioClip;
		audioItem._subItemsList[0] = audioSubItem;
		AddToCategory(category, audioItem);
		return audioItem;
	}

	public static bool RemoveAudioItem(string audioID)
	{
		SoundItem audioItem = SingletonMonoBehaviour<AudioController>.Instance._GetAudioItem(audioID);
		if (audioItem != null)
		{
			int num = audioItem.category.AudioIndex(audioItem);
			if (num < 0)
			{
				return false;
			}
			SoundItem[] audioItems = audioItem.category.AudioItems;
			SoundItem[] array = new SoundItem[audioItems.Length - 1];
			for (int i = 0; i < num; i++)
			{
				array[i] = audioItems[i];
			}
			for (int i = num + 1; i < audioItems.Length; i++)
			{
				array[i - 1] = audioItems[i];
			}
			audioItem.category.AudioItems = array;
			if (SingletonMonoBehaviour<AudioController>.Instance._validatingSound)
			{
				SingletonMonoBehaviour<AudioController>.Instance._itemsDict.Remove(audioID);
			}
			return true;
		}
		return false;
	}

	public static bool IsValidAudioID(string audioID)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._GetAudioItem(audioID) != null;
	}

	public static SoundItem GetAudioItem(string audioID)
	{
		return SingletonMonoBehaviour<AudioController>.Instance._GetAudioItem(audioID);
	}

	public static void DetachAllAudios(GameObject gameObjectWithAudios)
	{
		SoundObject[] componentsInChildren = gameObjectWithAudios.GetComponentsInChildren<SoundObject>(includeInactive: true);
		foreach (SoundObject audioObject in componentsInChildren)
		{
			audioObject.transform.parent = null;
		}
	}

	public static float GetAudioItemMaxDistance(string audioID)
	{
		SoundItem audioItem = GetAudioItem(audioID);
		if (audioItem.overrideAudioSettings)
		{
			return audioItem._maxDistance;
		}
		return audioItem.category.AudioPrefab().GetComponent<AudioSource>().maxDistance;
	}

	public void UnloadAllAudioClips()
	{
		for (int i = 0; i < soundCategory.Length; i++)
		{
			SoundCategory audioCategory = soundCategory[i];
			audioCategory.LoadAllClips();
		}
	}

	private void _ApplyVolumeChange()
	{
		List<SoundObject> playingAudioObjects = GetPlayingAudioObjects(includePausedAudio: true);
		for (int i = 0; i < playingAudioObjects.Count; i++)
		{
			SoundObject audioObject = playingAudioObjects[i];
			if (audioObject != null)
			{
				audioObject.VolumeApply();
			}
		}
	}

	internal SoundItem _GetAudioItem(string audioID)
	{
		_ValidateCategories();
		if (_itemsDict.TryGetValue(audioID, out SoundItem value))
		{
			return value;
		}
		return null;
	}

	protected SoundObject _PlayMusic(string audioID, float volume, float delay, float startTime)
	{
		if (_soundParent == null)
		{
			AudioListener currentAudioListener = GetCurrentAudioListener();
			if (currentAudioListener == null)
			{
				UnityEngine.Debug.LogWarning("No AudioListener found in the scene");
				return null;
			}
			return _PlayMusic(audioID, currentAudioListener.transform.position + currentAudioListener.transform.forward, null, volume, delay, startTime);
		}
		return _PlayMusic(audioID, _soundParent.position, _soundParent, volume, delay, startTime);
	}

	protected SoundObject _PlayAmbienceSound(string audioID, float volume, float delay, float startTime)
	{
		if (_ambienceParent == null)
		{
			AudioListener currentAudioListener = GetCurrentAudioListener();
			if (currentAudioListener == null)
			{
				UnityEngine.Debug.LogWarning("No AudioListener found in the scene");
				return null;
			}
			return _PlayAmbienceSound(audioID, currentAudioListener.transform.position + currentAudioListener.transform.forward, null, volume, delay, startTime);
		}
		return _PlayAmbienceSound(audioID, _ambienceParent.position, _ambienceParent, volume, delay, startTime);
	}

	protected bool _StopMusic(float fadeOutLength)
	{
		if (thisMusic != null)
		{
			thisMusic.StopSound(fadeOutLength);
			thisMusic = null;
			return true;
		}
		return false;
	}

	protected bool _PauseMusic(float fadeOut)
	{
		if (thisMusic != null)
		{
			thisMusic.PauseSound(fadeOut);
			return true;
		}
		return false;
	}

	protected bool _StopAmbienceSound(float fadeOutLength)
	{
		if (AmbienceSound != null)
		{
			AmbienceSound.StopSound(fadeOutLength);
			AmbienceSound = null;
			return true;
		}
		return false;
	}

	protected bool _PauseAmbienceSound(float fadeOut)
	{
		if (AmbienceSound != null)
		{
			AmbienceSound.PauseSound(fadeOut);
			return true;
		}
		return false;
	}

	protected SoundObject _PlayMusic(string audioID, Vector3 position, Transform parentObj, float volume, float delay, float startTime)
	{
		if (!IsMusicEnabled())
		{
			return null;
		}
		bool flag;
		if (thisMusic != null && thisMusic.IsSoundPlaying())
		{
			flag = true;
			thisMusic.StopSound(CrossFadeOut);
		}
		else
		{
			flag = false;
		}
		if (CrossFadeIn <= 0f)
		{
			flag = false;
		}
		thisMusic = _PlayEx(audioID, AudioChannelType.Music, volume, position, parentObj, delay, startTime, playWithoutAudioObject: false, 0.0, null, (!flag) ? 1 : 0);
		if (flag && (bool)thisMusic)
		{
			thisMusic.SoundFadeIn(CrossFadeIn);
		}
		return thisMusic;
	}

	protected SoundObject _PlayAmbienceSound(string audioID, Vector3 position, Transform parentObj, float volume, float delay, float startTime)
	{
		if (!IsAmbienceSoundEnabled())
		{
			return null;
		}
		bool flag;
		if (AmbienceSound != null && AmbienceSound.IsSoundPlaying())
		{
			flag = true;
			AmbienceSound.StopSound(CrossFadeTimeOut);
		}
		else
		{
			flag = false;
		}
		if (FadeTimeIn <= 0f)
		{
			flag = false;
		}
		AmbienceSound = _PlayEx(audioID, AudioChannelType.Ambience, volume, position, parentObj, delay, startTime, playWithoutAudioObject: false, 0.0, null, (!flag) ? 1 : 0);
		if (flag && (bool)AmbienceSound)
		{
			AmbienceSound.SoundFadeIn(FadeTimeIn);
		}
		return AmbienceSound;
	}

	protected int _EnqueueMusic(string audioID)
	{
		Playlist playlist = GetPlayList();
		int num = (playlist == null) ? 1 : (playList.Length + 1);
		string[] array = new string[num];
		playlist?.playlistItems.CopyTo(array, 0);
		array[num - 1] = audioID;
		playlist.playlistItems = array;
		return num;
	}

	protected SoundObject _PlayMusicPlaylist()
	{
		_ResetLastPlayedList();
		return _PlayNextMusicOnPlaylist(0f);
	}

	private SoundObject _PlayMusicTrackWithID(int nextTrack, float delay, bool addToPlayedList)
	{
		if (nextTrack < 0)
		{
			return null;
		}
		_playedList.Add(nextTrack);
		_playingList = true;
		Playlist playlist = GetPlayList();
		SoundObject audioObject = _PlayMusic(playlist.playlistItems[nextTrack], 1f, delay, 0f);
		if (audioObject != null)
		{
			audioObject._isPlaulistTrack = true;
			audioObject.PrimaryAudioSource.loop = false;
		}
		return audioObject;
	}

	internal SoundObject _PlayNextMusicOnPlaylist(float delay)
	{
		int nextTrack = _GetNextMusicTrack();
		return _PlayMusicTrackWithID(nextTrack, delay, addToPlayedList: true);
	}

	internal SoundObject _PlayPreviousMusicOnPlaylist(float delay)
	{
		int nextTrack = _GetPreviousMusicTrack();
		return _PlayMusicTrackWithID(nextTrack, delay, addToPlayedList: false);
	}

	private void _ResetLastPlayedList()
	{
		_playedList.Clear();
	}

	protected int _GetNextMusicTrack()
	{
		Playlist playlist = GetPlayList();
		if (playlist == null || playlist.playlistItems == null)
		{
			UnityEngine.Debug.LogWarning("There is no current playlist set");
			return -1;
		}
		if (playlist.playlistItems.Length == 1)
		{
			return 0;
		}
		if (shuffleList)
		{
			return _GetNextMusicTrackShuffled();
		}
		return _GetNextMusicTrackInOrder();
	}

	protected int _GetPreviousMusicTrack()
	{
		Playlist playlist = GetPlayList();
		if (playlist.playlistItems.Length == 1)
		{
			return 0;
		}
		if (shuffleList)
		{
			return _GetPreviousMusicTrackShuffled();
		}
		return _GetPreviousMusicTrackInOrder();
	}

	private int _GetPreviousMusicTrackShuffled()
	{
		if (_playedList.Count >= 2)
		{
			int result = _playedList[_playedList.Count - 2];
			_RemoveLastPlayedOnList();
			_RemoveLastPlayedOnList();
			return result;
		}
		return -1;
	}

	private void _RemoveLastPlayedOnList()
	{
		_playedList.RemoveAt(_playedList.Count - 1);
	}

	private int _GetNextMusicTrackShuffled()
	{
		HashSet<int> hashSet = new HashSet<int>();
		int num = _playedList.Count;
		Playlist playlist = GetPlayList();
		if (loopList)
		{
			int num2 = Mathf.Clamp(playlist.playlistItems.Length / 4, 2, 10);
			if (num > playlist.playlistItems.Length - num2)
			{
				num = playlist.playlistItems.Length - num2;
				if (num < 1)
				{
					num = 1;
				}
			}
		}
		else if (num >= playlist.playlistItems.Length)
		{
			return -1;
		}
		for (int i = 0; i < num; i++)
		{
			hashSet.Add(_playedList[_playedList.Count - 1 - i]);
		}
		List<int> list = new List<int>();
		for (int j = 0; j < playlist.playlistItems.Length; j++)
		{
			if (!hashSet.Contains(j))
			{
				list.Add(j);
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private int _GetNextMusicTrackInOrder()
	{
		if (_playedList.Count == 0)
		{
			return 0;
		}
		int num = _playedList[_playedList.Count - 1] + 1;
		Playlist playlist = GetPlayList();
		if (num >= playlist.playlistItems.Length)
		{
			if (!loopList)
			{
				return -1;
			}
			num = 0;
		}
		return num;
	}

	private int _GetPreviousMusicTrackInOrder()
	{
		Playlist playlist = GetPlayList();
		if (_playedList.Count < 2)
		{
			if (loopList)
			{
				return playlist.playlistItems.Length - 1;
			}
			return -1;
		}
		int num = _playedList[_playedList.Count - 1] - 1;
		_RemoveLastPlayedOnList();
		_RemoveLastPlayedOnList();
		if (num < 0)
		{
			if (!loopList)
			{
				return -1;
			}
			num = playlist.playlistItems.Length - 1;
		}
		return num;
	}

	protected SoundObject _PlayEx(string audioID, AudioChannelType channel, float volume, Vector3 worldPosition, Transform parentObj, float delay, float startTime, bool playWithoutAudioObject, double dspTime = 0.0, SoundObject useExistingAudioObject = null, float startVolumeMultiplier = 1f)
	{
		if (_audioDisabled)
		{
			return null;
		}
		SoundItem audioItem = _GetAudioItem(audioID);
		if (audioItem == null)
		{
			UnityEngine.Debug.LogWarning("Audio item with name '" + audioID + "' does not exist");
			return null;
		}
		if (audioItem._lastPlayedTime > 0.0 && dspTime == 0.0)
		{
			double num = InfoSystemTime - audioItem._lastPlayedTime;
			if (num < (double)audioItem.TimeBetweenCalls)
			{
				return null;
			}
		}
		if (audioItem.MaxSoundCount > 0)
		{
			List<SoundObject> playingAudioObjects = GetPlayingAudioObjects(audioID);
			if (playingAudioObjects.Count >= audioItem.MaxSoundCount)
			{
				bool flag = playingAudioObjects.Count > audioItem.MaxSoundCount;
				SoundObject audioObject = null;
				for (int i = 0; i < playingAudioObjects.Count; i++)
				{
					if ((flag || !playingAudioObjects[i].IsFadingOut) && (audioObject == null || playingAudioObjects[i].startedPlayingAtTime < audioObject.startedPlayingAtTime))
					{
						audioObject = playingAudioObjects[i];
					}
				}
				if (audioObject != null)
				{
					audioObject.StopSound((!flag) ? 0.2f : 0f);
				}
			}
		}
		return PlayAudioItem(audioItem, volume, worldPosition, parentObj, delay, startTime, playWithoutAudioObject, useExistingAudioObject, dspTime, channel, startVolumeMultiplier);
	}

	public SoundObject PlayAudioItem(SoundItem sndItem, float volume, Vector3 worldPosition, Transform parentObj = null, float delay = 0f, float startTime = 0f, bool playWithoutAudioObject = false, SoundObject useExistingAudioObj = null, double dspTime = 0.0, AudioChannelType channel = AudioChannelType.Default, float startVolumeMultiplier = 1f)
	{
		SoundObject audioObject = null;
		sndItem._lastPlayedTime = InfoSystemTime;
		SoundSubItem[] array = SoundControllerHelper.ChooseSubSounds(sndItem, useExistingAudioObj);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		foreach (SoundSubItem audioSubItem in array)
		{
			if (audioSubItem == null)
			{
				continue;
			}
			SoundObject audioObject2 = PlayAudioSubItem(audioSubItem, volume, worldPosition, parentObj, delay, startTime, playWithoutAudioObject, useExistingAudioObj, dspTime, channel, startVolumeMultiplier);
			if (!audioObject2)
			{
				continue;
			}
			audioObject = audioObject2;
			audioObject.IDAudio = sndItem._name;
			if (sndItem.overrideAudioSettings)
			{
				audioObject2._sourseDistance = audioObject2.PrimaryAudioSource.minDistance;
				audioObject2._souseDistanceSaved = audioObject2.PrimaryAudioSource.maxDistance;
				audioObject2._savedBlend = audioObject2.PrimaryAudioSource.spatialBlend;
				audioObject2.PrimaryAudioSource.minDistance = sndItem._minDistance;
				audioObject2.PrimaryAudioSource.maxDistance = sndItem._maxDistance;
				audioObject2.PrimaryAudioSource.spatialBlend = sndItem._spatialBlend;
				if (audioObject2.SecondaryAudioSource != null)
				{
					audioObject2.SecondaryAudioSource.minDistance = sndItem._minDistance;
					audioObject2.SecondaryAudioSource.maxDistance = sndItem._maxDistance;
					audioObject2.SecondaryAudioSource.spatialBlend = sndItem._spatialBlend;
				}
			}
		}
		return audioObject;
	}

	internal SoundCategory _GetCategory(string name)
	{
		for (int i = 0; i < soundCategory.Length; i++)
		{
			SoundCategory audioCategory = soundCategory[i];
			if (audioCategory._name == name)
			{
				return audioCategory;
			}
		}
		return null;
	}

	private void Update()
	{
		if (!_isAudioController)
		{
			_UpdateSystemTime();
		}
	}

	private static void _UpdateSystemTime()
	{
		double timeSinceLaunch = SystemTime.timeSinceLaunch;
		if (_systemType >= 0.0)
		{
			_deltaTime = timeSinceLaunch - _systemType;
			if (_deltaTime > (double)(Time.maximumDeltaTime + 0.01f))
			{
				_deltaTime = Time.deltaTime;
			}
			_infoSystemTime += _deltaTime;
		}
		else
		{
			_deltaTime = 0.0;
			_infoSystemTime = 0.0;
		}
		_systemType = timeSinceLaunch;
	}

	protected override void Awake()
	{
		base.Awake();
		if (Persistent)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void OnEnable()
	{
		if (isAudioController)
		{
			if ((bool)SingletonMonoBehaviour<AudioController>.DoesInstanceExist())
			{
				SingletonMonoBehaviour<AudioController>.Instance._RegisterAdditionalAudioController(this);
				return;
			}
			if (_registerController == null)
			{
				_registerController = new List<AudioController>();
			}
			_registerController.Add(this);
		}
		else
		{
			if (_registerController == null)
			{
				return;
			}
			for (int i = 0; i < _registerController.Count; i++)
			{
				AudioController audioController = _registerController[i];
				if ((bool)audioController && audioController.enabled)
				{
					SingletonMonoBehaviour<AudioController>.Instance._RegisterAdditionalAudioController(audioController);
				}
			}
			_registerController = null;
		}
	}

	private void OnDisable()
	{
		if (isAudioController && (bool)SingletonMonoBehaviour<AudioController>.DoesInstanceExist())
		{
			SingletonMonoBehaviour<AudioController>.Instance._UnregisterAdditionalAudioController(this);
		}
	}

	protected override void OnDestroy()
	{
		if (IsUnloadAudioClipsOnDestroy)
		{
			UnloadAllAudioClips();
		}
		base.OnDestroy();
	}

	private void AwakeSingleton()
	{
		_UpdateSystemTime();
		if (AudioPrefab == null)
		{
			UnityEngine.Debug.LogError("No AudioObject prefab specified in AudioController. To make your own AudioObject prefab create an empty game object, add Unity's AudioSource, the AudioObject script, and the PoolableObject script (if pooling is wanted ). Then create a prefab and set it in the AudioController.");
		}
		else
		{
			_ValidateAudioObjectPrefab(AudioPrefab);
		}
		_ValidateCategories();
		if (_playedList == null)
		{
			_playedList = new List<int>();
			_playingList = false;
		}
		_SetDefaultCurrentPlaylist();
	}

	protected void _ValidateCategories()
	{
		if (!_validatingSound)
		{
			InitializeAudioItems();
			_validatingSound = true;
		}
	}

	protected void _InvalidateCategories()
	{
		_validatingSound = false;
	}

	public void InitializeAudioItems()
	{
		if (isAudioController)
		{
			return;
		}
		_itemsDict = new Dictionary<string, SoundItem>();
		_InitializeAudioItems(this);
		if (_audioController == null)
		{
			return;
		}
		for (int i = 0; i < _audioController.Count; i++)
		{
			AudioController audioController = _audioController[i];
			if (audioController != null)
			{
				_InitializeAudioItems(audioController);
			}
		}
	}

	private void _InitializeAudioItems(AudioController audioController)
	{
		for (int i = 0; i < audioController.soundCategory.Length; i++)
		{
			SoundCategory audioCategory = audioController.soundCategory[i];
			audioCategory.AudioController = audioController;
			audioCategory.SetAudioSystem(_itemsDict);
			if ((bool)audioCategory.AudioObjectPrefab)
			{
				_ValidateAudioObjectPrefab(audioCategory.AudioObjectPrefab);
			}
		}
	}

	private void _RegisterAdditionalAudioController(AudioController ac)
	{
		if (_audioController == null)
		{
			_audioController = new List<AudioController>();
		}
		_audioController.Add(ac);
		_InvalidateCategories();
		_SyncCategoryVolumes(ac, this);
	}

	private void _SyncCategoryVolumes(AudioController toSync, AudioController syncWith)
	{
		for (int i = 0; i < toSync.soundCategory.Length; i++)
		{
			SoundCategory audioCategory = toSync.soundCategory[i];
			SoundCategory audioCategory2 = syncWith._GetCategory(audioCategory._name);
			if (audioCategory2 != null)
			{
				audioCategory.SoundVolume = audioCategory2.SoundVolume;
			}
		}
	}

	private void _UnregisterAdditionalAudioController(AudioController ac)
	{
		if (_audioController != null)
		{
			int num = 0;
			while (true)
			{
				if (num < _audioController.Count)
				{
					if (_audioController[num] == ac)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_audioController.RemoveAt(num);
			_InvalidateCategories();
		}
		else
		{
			UnityEngine.Debug.LogWarning("_UnregisterAdditionalAudioController: AudioController " + ac.name + " not found");
		}
	}

	private static List<SoundCategory> _GetAllCategories(string name)
	{
		AudioController instance = SingletonMonoBehaviour<AudioController>.Instance;
		List<SoundCategory> list = new List<SoundCategory>();
		SoundCategory audioCategory = instance._GetCategory(name);
		if (audioCategory != null)
		{
			list.Add(audioCategory);
		}
		if (instance._audioController != null)
		{
			for (int i = 0; i < instance._audioController.Count; i++)
			{
				AudioController audioController = instance._audioController[i];
				audioCategory = audioController._GetCategory(name);
				if (audioCategory != null)
				{
					list.Add(audioCategory);
				}
			}
		}
		return list;
	}

	public SoundObject PlayAudioSubItem(SoundSubItem subItem, float volume, Vector3 worldPosition, Transform parentObj, float delay, float startTime, bool playWithoutAudioObject, SoundObject useExistingAudioObj, double dspTime = 0.0, AudioChannelType channel = AudioChannelType.Default, float startVolumeMultiplier = 1f)
	{
		_ValidateCategories();
		SoundItem item = subItem.Item;
		switch (subItem._soundSubItemType)
		{
		case SoundSubItemType.Item:
			if (subItem._itemModeAudioID.Length == 0)
			{
				UnityEngine.Debug.LogWarning("No item specified in audio sub-item with ITEM mode (audio item: '" + item._name + "')");
				return null;
			}
			return _PlayEx(subItem._itemModeAudioID, channel, volume, worldPosition, parentObj, delay, startTime, playWithoutAudioObject, dspTime, useExistingAudioObj);
		default:
		{
			if (subItem._audioClip == null)
			{
				return null;
			}
			SoundCategory category = item.category;
			float num = subItem._volume * item.VolumeValue * volume;
			if (subItem._randomVolume != 0f || item._loopSequenceRandomVolume != 0f)
			{
				float num2 = subItem._randomVolume + item._loopSequenceRandomVolume;
				num += UnityEngine.Random.Range(0f - num2, num2);
				num = Mathf.Clamp01(num);
			}
			float num3 = num * category.SoundVolumeTotal;
			AudioController audioController = _GetAudioController(subItem);
			if (!audioController.IsPlayWithZeroVolume && (num3 <= 0f || VolumeValue <= 0f))
			{
				return null;
			}
			GameObject gameObject = category.AudioPrefab();
			if (gameObject == null)
			{
				gameObject = ((!(audioController.AudioPrefab != null)) ? AudioPrefab : audioController.AudioPrefab);
			}
			if (playWithoutAudioObject)
			{
				gameObject.GetComponent<AudioSource>().PlayOneShot(subItem._audioClip, SoundObject.VolumeTransform(num3));
				return null;
			}
			GameObject gameObject2;
			SoundObject audioObject;
			if (useExistingAudioObj == null)
			{
				if (item.IsDestroyOnLoad)
				{
					gameObject2 = ((!audioController.IsUsePooledAudioObjects) ? ObjectPoolController.InstantiateWithoutPool(gameObject, worldPosition, Quaternion.identity) : ObjectPoolController.Instantiate(gameObject, worldPosition, Quaternion.identity));
				}
				else
				{
					gameObject2 = ObjectPoolController.InstantiateWithoutPool(gameObject, worldPosition, Quaternion.identity);
					UnityEngine.Object.DontDestroyOnLoad(gameObject2);
				}
				if ((bool)parentObj)
				{
					gameObject2.transform.parent = parentObj;
				}
				audioObject = gameObject2.gameObject.GetComponent<SoundObject>();
			}
			else
			{
				gameObject2 = useExistingAudioObj.gameObject;
				audioObject = useExistingAudioObj;
			}
			audioObject.SumItem = subItem;
			if (object.ReferenceEquals(useExistingAudioObj, null))
			{
				audioObject._lastSubIndex = item._lastChosen;
			}
			audioObject.PrimaryAudioSource.clip = subItem._audioClip;
			gameObject2.name = "AudioObject:" + audioObject.PrimaryAudioSource.clip.name;
			audioObject.PrimaryAudioSource.pitch = SoundObject.TransformPitch(subItem._pitchShift);
			audioObject.PrimaryAudioSource.panStereo = subItem._pan2D;
			if (subItem._randomStartPos)
			{
				startTime = UnityEngine.Random.Range(0f, audioObject.LengthClip);
			}
			audioObject.PrimaryAudioSource.time = startTime + subItem._clipStartTime;
			audioObject.PrimaryAudioSource.loop = (item._loopMode == SoundItem.LoopMode.LoopSubitem || item._loopMode == (SoundItem.LoopMode)3);
			audioObject._volumeCategory = num;
			audioObject._volumeScripCall = volume;
			audioObject.Category = category;
			audioObject.Chanel = channel;
			if (subItem._fadeIn > 0f)
			{
				audioObject.SoundFadeIn(subItem._fadeIn);
			}
			audioObject.PrimaryVolumeSet(startVolumeMultiplier);
			AudioMixerGroup audioMixerGroup = category.MixerGroup();
			if ((bool)audioMixerGroup)
			{
				audioObject.PrimaryAudioSource.outputAudioMixerGroup = category._audioMixerGroup;
			}
			if (subItem._randomPitch != 0f || item._loopSequenceRandomPitch != 0f)
			{
				float num4 = subItem._randomPitch + item._loopSequenceRandomPitch;
				audioObject.PrimaryAudioSource.pitch *= SoundObject.TransformPitch(UnityEngine.Random.Range(0f - num4, num4));
			}
			if (subItem._randomDelay > 0f)
			{
				delay += UnityEngine.Random.Range(0f, subItem._randomDelay);
			}
			if (dspTime > 0.0)
			{
				audioObject.PlayTabeled(dspTime + (double)delay + (double)subItem._delay + (double)item._dalay);
			}
			else
			{
				audioObject.PlaySound(delay + subItem._delay + item._dalay);
			}
			if (subItem._fadeIn > 0f)
			{
				audioObject.SoundFadeIn(subItem._fadeIn);
			}
			return audioObject;
		}
		}
	}

	private AudioController _GetAudioController(SoundSubItem subItem)
	{
		if (subItem.Item != null && subItem.Item.category != null)
		{
			return subItem.Item.category.AudioController;
		}
		return this;
	}

	internal void _NotifyPlaylistTrackCompleteleyPlayed(SoundObject audioObject)
	{
		audioObject._isPlaulistTrack = false;
		if (IsPlaylistPlaying() && thisMusic == audioObject && _PlayNextMusicOnPlaylist(trackDelay) == null)
		{
			_playingList = false;
		}
	}

	private void _ValidateAudioObjectPrefab(GameObject audioPrefab)
	{
		if (IsUsePooledAudioObjects)
		{
			if (audioPrefab.GetComponent<PoolableObject>() == null)
			{
				UnityEngine.Debug.LogWarning("AudioObject prefab does not have the PoolableObject component. Pooling will not work.");
			}
			else
			{
				ObjectPoolController.Preload(audioPrefab);
			}
		}
		if (audioPrefab.GetComponent<SoundObject>() == null)
		{
			UnityEngine.Debug.LogError("AudioObject prefab must have the AudioObject script component!");
		}
	}

	public void OnAfterDeserialize()
	{
		if (musicPlaylist != null && musicPlaylist.Length != 0)
		{
			List<string> list = new List<string>(musicPlaylist);
			playList[0] = new Playlist();
			playList[0].playlistItems = list.ToArray();
			musicPlaylist = null;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	private void _SetDefaultCurrentPlaylist()
	{
		if (playList != null && playList.Length >= 1 && playList[0] != null)
		{
			_playListName = playList[0].name;
		}
	}
}
