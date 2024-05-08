using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("ClockStone/Audio/AudioObject")]
public class SoundObject : RegisteredComponent
{
	public delegate void AudioEventDelegate(SoundObject audioObject);

	[NonSerialized]
	private SoundCategory _soundCategory;

	private SoundSubItem _sumItemMain;

	private SoundSubItem _subItemNotMain;

	private AudioEventDelegate _completelyPlayedDelegate;

	private int _pauseCoroutine;

	private bool _isSoursesSwaped;

	internal float _volumeCategory = 1f;

	private float _volumeMainFade = 1f;

	private float _volumeSecondFade = 1f;

	internal float _volumeScripCall = 1f;

	private bool _isPaused;

	private bool _isApplicationPaused;

	private SoundFader _mainFader;

	private SoundFader _scondFader;

	private double _timePlay = -1.0;

	private double _startTimeLocal = -1.0;

	private double _playStartTimeSystem = -1.0;

	private double _playScheduledTimeDsp = -1.0;

	private double _audioObjectTime;

	private bool _isInactive = true;

	private bool _isStopRequested;

	private bool _stopSequence;

	private int _loopSequenceCount;

	private bool _stopAfterFadeoutUserSetting;

	private bool _fadeRequest;

	private double _paiseRamainig;

	private AudioController _audioController;

	internal bool _isPlaulistTrack;

	internal float _sourseDistance = 1f;

	internal float _souseDistanceSaved = 500f;

	internal float _savedBlend;

	private AudioMixerGroup _audioMixerGroup;

	internal int _lastSubIndex = -1;

	private AudioSource _audioSourceMain;

	private AudioSource _audioSourceSecond;

	private bool _primaryAudioSourcePaused;

	private bool _secondaryAudioSourcePaused;

	public string IDAudio
	{
		get;
		internal set;
	}

	public SoundCategory Category
	{
		get => _soundCategory;
		internal set => _soundCategory = value;
	}

	public SoundSubItem SumItem
	{
		get => _sumItemMain;
		internal set => _sumItemMain = value;
	}

	public AudioChannelType Chanel
	{
		get;
		internal set;
	}

	public SoundItem AudioItem
	{
		get
		{
			if (SumItem != null)
			{
				return SumItem.Item;
			}
			return null;
		}
	}

	public AudioEventDelegate completelyPlayedDelegate
	{
		get => _completelyPlayedDelegate;
		set
		{
			_completelyPlayedDelegate = value;
		}
	}

	private float volumeTotalWithoutFade
	{
		get
		{
			float num = VolumeWithCategory;
			AudioController audioController = null;
			audioController = ((Category == null) ? _audioController : Category.AudioController);
			if (audioController != null)
			{
				num *= audioController.VolumeValue;
				if (audioController.IsMuted && Chanel == AudioChannelType.Default)
				{
					num = 0f;
				}
			}
			return num;
		}
	}
	
	public double startedPlayingAtTime => _playStartTimeSystem;

	public float timeUntilEnd => LengthClip - audioTime;

	public double scheduledPlayingAtDspTime
	{
		get => _playScheduledTimeDsp;
		set
		{
			_playScheduledTimeDsp = value;
			PrimaryAudioSource.SetScheduledStartTime(_playScheduledTimeDsp);
		}
	}

	public float LengthClip
	{
		get
		{
			if (StopClipAtTime > 0f)
			{
				return StopClipAtTime - StartClipAtTime;
			}
			if (PrimaryAudioSource.clip != null)
			{
				return PrimaryAudioSource.clip.length - StartClipAtTime;
			}
			return 0f;
		}
	}

	public float audioTime
	{
		get
		{
			return PrimaryAudioSource.time - StartClipAtTime;
		}
		set
		{
			PrimaryAudioSource.time = value + StartClipAtTime;
		}
	}

	public bool IsFadingOut => _mainFader.IsFading;

	public bool IsFadingOutOrScheduled => _mainFader.IsScheduled;

	public double AudioObjectTime => _audioObjectTime;

	public AudioSource PrimaryAudioSource => _audioSourceMain;

	public AudioSource SecondaryAudioSource => _audioSourceSecond;

	internal float VolumeFromCategory
	{
		get
		{
			if (Category != null)
			{
				return Category.SoundVolumeTotal;
			}
			return 1f;
		}
	}

	internal float VolumeWithCategory => VolumeFromCategory * _volumeCategory;

	private float StopClipAtTime => (SumItem == null) ? 0f : SumItem._clipStopTime;

	private float StartClipAtTime => (SumItem == null) ? 0f : SumItem._clipStartTime;

	private bool ShouldStopIfPrimaryFadedOut => _stopAfterFadeoutUserSetting && !_fadeRequest;

	public void SoundFadeIn(float fadeInTime)
	{
		if (_startTimeLocal > 0.0)
		{
			double num = _startTimeLocal - AudioObjectTime;
			if (num > 0.0)
			{
				_mainFader.FadeSoundIn(fadeInTime, _startTimeLocal);
				UpdateFadeVolume();
				return;
			}
		}
		_mainFader.FadeSoundIn(fadeInTime, AudioObjectTime, !ShouldStopIfPrimaryFadedOut);
		UpdateFadeVolume();
	}

	public void PlayTabeled(double dspTime)
	{
		PlaySceduled(dspTime);
	}

	public void PlaySound(float delay = 0f)
	{
		DelayPlay(delay);
	}

	public void StopSound()
	{
		StopSound(-1f);
	}

	public void StopSound(float fadeOutLength)
	{
		StopSound(fadeOutLength, 0f);
	}

	public void StopSound(float fadeOutLength, float startToFadeTime)
	{
		if (IsPaused(returnTrueIfStillFadingOut: false))
		{
			fadeOutLength = 0f;
			startToFadeTime = 0f;
		}
		if (startToFadeTime > 0f)
		{
			StartCoroutine(WaitTillStop(startToFadeTime, fadeOutLength));
			return;
		}
		_isStopRequested = true;
		if (fadeOutLength < 0f)
		{
			fadeOutLength = ((SumItem == null) ? 0f : SumItem._fadeOut);
		}
		if (fadeOutLength == 0f && startToFadeTime == 0f)
		{
			_Stop();
			return;
		}
		FadeSoundOut(fadeOutLength, startToFadeTime);
		if (IsSecondPlaying())
		{
			SwapAudioSources();
			FadeSoundOut(fadeOutLength, startToFadeTime);
			SwapAudioSources();
		}
	}

	private IEnumerator WaitTillStop(float startToFadeTime, float fadeOutLength)
	{
		yield return new WaitForSeconds(startToFadeTime);
		if (!_isInactive)
		{
			StopSound(fadeOutLength);
		}
	}

	public void FadeSoundOut(float fadeOutLength)
	{
		FadeSoundOut(fadeOutLength, 0f);
	}

	public void FadeSoundOut(float fadeOutLength, float startToFadeTime)
	{
		if (fadeOutLength < 0f)
		{
			fadeOutLength = ((SumItem == null) ? 0f : SumItem._fadeOut);
		}
		if (fadeOutLength > 0f || startToFadeTime > 0f)
		{
			_mainFader.FadeSoundOut(fadeOutLength, startToFadeTime);
		}
		else if (fadeOutLength == 0f)
		{
			if (ShouldStopIfPrimaryFadedOut)
			{
				_Stop();
			}
			else
			{
				_mainFader.FadeSoundOut(0f, startToFadeTime);
			}
		}
	}

	public void PauseSound()
	{
		PauseSound(0f);
	}

	public void PauseSound(float fadeOutTime)
	{
		if (!_isPaused)
		{
			_isPaused = true;
			if (fadeOutTime > 0f)
			{
				_fadeRequest = true;
				FadeSoundOut(fadeOutTime);
				StartCoroutine(PauseDelay(fadeOutTime, ++_pauseCoroutine));
			}
			else
			{
				PauseExtraNow();
			}
		}
	}

	private void PauseExtraNow()
	{
		if (_playScheduledTimeDsp > 0.0)
		{
			_paiseRamainig = _playScheduledTimeDsp - AudioSettings.dspTime;
			scheduledPlayingAtDspTime = 9000000000.0;
		}
		PauseAll();
		if (_fadeRequest)
		{
			_fadeRequest = false;
			_mainFader.Reset();
		}
	}

	public void ContinueSound()
	{
		ContinueSound(0f);
	}

	public void ContinueSound(float fadeInTime)
	{
		if (_isPaused)
		{
			ContinueNow();
			if (fadeInTime > 0f)
			{
				SoundFadeIn(fadeInTime);
			}
			_fadeRequest = false;
		}
	}

	private void ContinueNow()
	{
		_isPaused = false;
		if ((bool)SecondaryAudioSource && _secondaryAudioSourcePaused)
		{
			SecondaryAudioSource.Play();
		}
		if (_paiseRamainig > 0.0 && _primaryAudioSourcePaused)
		{
			double num = AudioSettings.dspTime + _paiseRamainig;
			_playStartTimeSystem = AudioController.InfoSystemTime + _paiseRamainig;
			PrimaryAudioSource.PlayScheduled(num);
			scheduledPlayingAtDspTime = num;
			_paiseRamainig = -1.0;
		}
		else if (_primaryAudioSourcePaused)
		{
			PrimaryAudioSource.Play();
		}
	}

	private IEnumerator PauseDelay(float waitTime, int counter)
	{
		yield return new WaitForSeconds(waitTime);
		if (_fadeRequest && counter == _pauseCoroutine)
		{
			PauseExtraNow();
		}
	}

	private void PauseAll()
	{
		if (PrimaryAudioSource.isPlaying)
		{
			_primaryAudioSourcePaused = true;
			PrimaryAudioSource.Pause();
		}
		else
		{
			_primaryAudioSourcePaused = false;
		}
		if ((bool)SecondaryAudioSource && SecondaryAudioSource.isPlaying)
		{
			_secondaryAudioSourcePaused = true;
			SecondaryAudioSource.Pause();
		}
		else
		{
			_secondaryAudioSourcePaused = false;
		}
	}

	public bool IsPaused(bool returnTrueIfStillFadingOut = true)
	{
		if (!returnTrueIfStillFadingOut)
		{
			return !_fadeRequest && _isPaused;
		}
		return _isPaused;
	}

	public bool IsSoundPlaying()
	{
		return IsMainPlaying() || IsSecondPlaying();
	}

	public bool IsMainPlaying()
	{
		return PrimaryAudioSource.isPlaying;
	}

	public bool IsSecondPlaying()
	{
		return SecondaryAudioSource != null && SecondaryAudioSource.isPlaying;
	}

	public void SwapAudioSources()
	{
		if (_audioSourceSecond == null)
		{
			CreateSecondary();
		}
		SwapValues(ref _audioSourceMain, ref _audioSourceSecond);
		SwapValues(ref _mainFader, ref _scondFader);
		SwapValues(ref _sumItemMain, ref _subItemNotMain);
		SwapValues(ref _volumeMainFade, ref _volumeSecondFade);
		_isSoursesSwaped = !_isSoursesSwaped;
	}

	private void SwapValues<T>(ref T v1, ref T v2)
	{
		T val = v1;
		v1 = v2;
		v2 = val;
	}

	protected override void Awake()
	{
		base.Awake();
		if (_mainFader == null)
		{
			_mainFader = new SoundFader();
		}
		else
		{
			_mainFader.Reset();
		}
		if (_scondFader == null)
		{
			_scondFader = new SoundFader();
		}
		else
		{
			_scondFader.Reset();
		}
		if (_audioSourceMain == null)
		{
			AudioSource[] components = GetComponents<AudioSource>();
			if (components.Length <= 0)
			{
				UnityEngine.Debug.LogError("AudioObject does not have an AudioSource component!");
			}
			else
			{
				_audioSourceMain = components[0];
				if (components.Length >= 2)
				{
					_audioSourceSecond = components[1];
				}
			}
		}
		else if ((bool)_audioSourceSecond && _isSoursesSwaped)
		{
			SwapAudioSources();
		}
		_audioMixerGroup = PrimaryAudioSource.outputAudioMixerGroup;
		Reset();
		_audioController = SingletonMonoBehaviour<AudioController>.Instance;
	}

	private void CreateSecondary()
	{
		_audioSourceSecond = base.gameObject.AddComponent<AudioSource>();
		_audioSourceSecond.rolloffMode = _audioSourceMain.rolloffMode;
		_audioSourceSecond.minDistance = _audioSourceMain.minDistance;
		_audioSourceSecond.maxDistance = _audioSourceMain.maxDistance;
		_audioSourceSecond.dopplerLevel = _audioSourceMain.dopplerLevel;
		_audioSourceSecond.spread = _audioSourceMain.spread;
		_audioSourceSecond.spatialBlend = _audioSourceMain.spatialBlend;
		_audioSourceSecond.outputAudioMixerGroup = _audioSourceMain.outputAudioMixerGroup;
		_audioSourceSecond.velocityUpdateMode = _audioSourceMain.velocityUpdateMode;
		_audioSourceSecond.ignoreListenerVolume = _audioSourceMain.ignoreListenerVolume;
		_audioSourceSecond.playOnAwake = false;
		_audioSourceSecond.priority = _audioSourceMain.priority;
		_audioSourceSecond.bypassEffects = _audioSourceMain.bypassEffects;
		_audioSourceSecond.ignoreListenerPause = _audioSourceMain.ignoreListenerPause;
		_audioSourceSecond.bypassListenerEffects = _audioSourceMain.bypassListenerEffects;
		_audioSourceSecond.bypassReverbZones = _audioSourceMain.bypassReverbZones;
		_audioSourceSecond.reverbZoneMix = _audioSourceMain.reverbZoneMix;
	}

	private void Reset()
	{
		ResetReferences();
		_audioObjectTime = 0.0;
		PrimaryAudioSource.playOnAwake = false;
		if ((bool)SecondaryAudioSource)
		{
			SecondaryAudioSource.playOnAwake = false;
		}
		_lastSubIndex = -1;
		_mainFader.Reset();
		_scondFader.Reset();
		_timePlay = -1.0;
		_startTimeLocal = -1.0;
		_playStartTimeSystem = -1.0;
		_playScheduledTimeDsp = -1.0;
		_volumeMainFade = 1f;
		_volumeSecondFade = 1f;
		_volumeScripCall = 1f;
		_isInactive = true;
		_isStopRequested = false;
		_stopSequence = false;
		_volumeCategory = 1f;
		_isPaused = false;
		_isApplicationPaused = false;
		_isPlaulistTrack = false;
		_loopSequenceCount = 0;
		_stopAfterFadeoutUserSetting = true;
		_fadeRequest = false;
		_paiseRamainig = -1.0;
		_primaryAudioSourcePaused = false;
		_secondaryAudioSourcePaused = false;
	}

	private void ResetReferences()
	{
		_audioController = null;
		PrimaryAudioSource.clip = null;
		if (SecondaryAudioSource != null)
		{
			SecondaryAudioSource.playOnAwake = false;
			SecondaryAudioSource.clip = null;
		}
		SumItem = null;
		Category = null;
		_completelyPlayedDelegate = null;
	}

	private void PlaySceduled(double dspTime)
	{
		if (!PrimaryAudioSource.clip)
		{
			UnityEngine.Debug.LogError("audio.clip == null in " + base.gameObject.name);
			return;
		}
		_playScheduledTimeDsp = dspTime;
		double num = dspTime - AudioSettings.dspTime;
		_startTimeLocal = num + AudioObjectTime;
		_playStartTimeSystem = num + AudioController.InfoSystemTime;
		PrimaryAudioSource.PlayScheduled(dspTime);
		Play();
	}

	private void DelayPlay(float delay)
	{
		if (!PrimaryAudioSource.clip)
		{
			UnityEngine.Debug.LogError("audio.clip == null in " + base.gameObject.name);
			return;
		}
		PrimaryAudioSource.PlayDelayed(delay);
		_playScheduledTimeDsp = -1.0;
		_startTimeLocal = AudioObjectTime + (double)delay;
		_playStartTimeSystem = AudioController.InfoSystemTime + (double)delay;
		Play();
	}

	private void Play()
	{
		_isInactive = false;
		_timePlay = AudioObjectTime;
		_isPaused = false;
		_primaryAudioSourcePaused = false;
		_secondaryAudioSourcePaused = false;
		_mainFader.Reset();
	}

	private void _Stop()
	{
		_mainFader.Reset();
		_scondFader.Reset();
		PrimaryAudioSource.Stop();
		if ((bool)SecondaryAudioSource)
		{
			SecondaryAudioSource.Stop();
		}
		_isPaused = false;
		_primaryAudioSourcePaused = false;
		_secondaryAudioSourcePaused = false;
	}

	private void Update()
	{
		if (_isInactive)
		{
			return;
		}
		if (!IsPaused(returnTrueIfStillFadingOut: false))
		{
			_audioObjectTime += AudioController.DeltaTime;
			_mainFader.time = _audioObjectTime;
			_scondFader.time = _audioObjectTime;
		}
		if (_playScheduledTimeDsp > 0.0 && _audioObjectTime > _startTimeLocal)
		{
			_playScheduledTimeDsp = -1.0;
		}
		if (!_isPaused && !_isApplicationPaused)
		{
			bool flag = IsMainPlaying();
			bool flag2 = IsSecondPlaying();
			if (!flag && !flag2)
			{
				bool flag3 = true;
				if (!_isStopRequested && flag3 && completelyPlayedDelegate != null)
				{
					completelyPlayedDelegate(this);
					flag3 = !IsSoundPlaying();
				}
				if (_isPlaulistTrack && (bool)SingletonMonoBehaviour<AudioController>.DoesInstanceExist())
				{
					SingletonMonoBehaviour<AudioController>.Instance._NotifyPlaylistTrackCompleteleyPlayed(this);
				}
				if (flag3)
				{
					DestroySound();
					return;
				}
			}
			else
			{
				if (!_isStopRequested && IsAudioLooped() && !IsSecondPlaying() && timeUntilEnd < 1f + Mathf.Max(0f, AudioItem._loopSequenceOverlap) && _playScheduledTimeDsp < 0.0)
				{
					SceduleNextInLoop();
				}
				if (!PrimaryAudioSource.loop)
				{
					if (_isPlaulistTrack && (bool)_audioController && _audioController.crossFadeLits && audioTime > LengthClip - _audioController.CrossFadeOut)
					{
						if ((bool)SingletonMonoBehaviour<AudioController>.DoesInstanceExist())
						{
							SingletonMonoBehaviour<AudioController>.Instance._NotifyPlaylistTrackCompleteleyPlayed(this);
						}
					}
					else
					{
						FadeOutIfNeed();
						if (flag2)
						{
							SwapAudioSources();
							FadeOutIfNeed();
							SwapAudioSources();
						}
					}
				}
			}
		}
		UpdateFadeVolume();
	}

	private void FadeOutIfNeed()
	{
		if (SumItem == null)
		{
			UnityEngine.Debug.LogWarning("subItem == null");
			return;
		}
		float audioTime = this.audioTime;
		float num = 0f;
		if (SumItem._fadeOut > 0f)
		{
			num = SumItem._fadeOut;
		}
		else if (StopClipAtTime > 0f)
		{
			num = 0.1f;
		}
		if (!IsFadingOutOrScheduled && num > 0f && audioTime > LengthClip - num)
		{
			FadeSoundOut(SumItem._fadeOut);
		}
	}

	private bool IsAudioLooped()
	{
		SoundItem audioItem = this.AudioItem;
		if (audioItem != null)
		{
			switch (audioItem._loopMode)
			{
			case SoundItem.LoopMode.LoopSequence:
			case (SoundItem.LoopMode)3:
				return true;
			case SoundItem.LoopMode.PlaySequenceAndLoopLast:
			case SoundItem.LoopMode.IntroLoopOutroSequence:
				return !PrimaryAudioSource.loop;
			}
		}
		return false;
	}

	private bool SceduleNextInLoop()
	{
		int num = (this.AudioItem._loopSequenceCount <= 0) ? this.AudioItem._subItemsList.Length : this.AudioItem._loopSequenceCount;
		if (_stopSequence)
		{
			if (this.AudioItem._loopMode != SoundItem.LoopMode.IntroLoopOutroSequence)
			{
				return false;
			}
			if (_loopSequenceCount <= num - 3)
			{
				return false;
			}
			if (_loopSequenceCount >= num - 1)
			{
				return false;
			}
		}
		if (this.AudioItem._loopSequenceCount > 0 && this.AudioItem._loopSequenceCount <= _loopSequenceCount + 1)
		{
			return false;
		}
		double dspTime = AudioSettings.dspTime + (double)timeUntilEnd + (double)SequenceDelay(this.AudioItem);
		SoundItem audioItem = this.AudioItem;
		SwapAudioSources();
		_audioController.PlayAudioItem(audioItem, _volumeScripCall, Vector3.zero, null, 0f, 0f, playWithoutAudioObject: false, this, dspTime);
		_loopSequenceCount++;
		if (this.AudioItem._loopMode == SoundItem.LoopMode.PlaySequenceAndLoopLast || this.AudioItem._loopMode == SoundItem.LoopMode.IntroLoopOutroSequence)
		{
			if (this.AudioItem._loopMode == SoundItem.LoopMode.IntroLoopOutroSequence)
			{
				if (!_stopSequence && num <= _loopSequenceCount + 2)
				{
					PrimaryAudioSource.loop = true;
				}
			}
			else if (num <= _loopSequenceCount + 1)
			{
				PrimaryAudioSource.loop = true;
			}
		}
		return true;
	}

	private void UpdateFadeVolume()
	{
		bool finishedFadeOut;
		float num = CrossFade(_mainFader.Get(out finishedFadeOut));
		if (finishedFadeOut)
		{
			if (_isStopRequested)
			{
				_Stop();
				return;
			}
			if (!IsAudioLooped())
			{
				if (ShouldStopIfPrimaryFadedOut)
				{
					_Stop();
				}
				return;
			}
		}
		if (num != _volumeMainFade)
		{
			_volumeMainFade = num;
		}
		PrimaryVolumeSet();
		if (_audioSourceSecond != null)
		{
			float num2 = CrossFade(_scondFader.Get(out finishedFadeOut));
			if (finishedFadeOut)
			{
				_audioSourceSecond.Stop();
			}
			else if (num2 != _volumeSecondFade)
			{
				_volumeSecondFade = num2;
				SetVolumeSecond();
			}
		}
	}

	private float CrossFade(float v)
	{
		if (!_audioController.IsEqualPowerCrossfade)
		{
			return v;
		}
		return VolumeIncrease(Mathf.Sin(v * (float)Math.PI * 0.5f));
	}

	private void OnApplicationPause(bool b)
	{
		PauseAplication(b);
	}

	private void PauseAplication(bool isPaused)
	{
		_isApplicationPaused = isPaused;
	}

	public void DestroySound()
	{
		if (IsSoundPlaying())
		{
			_Stop();
		}
		ObjectPoolController.Destroy(base.gameObject);
		_isInactive = true;
	}

	public static float VolumeTransform(float volume)
	{
		return Mathf.Pow(volume, 1.6f);
	}

	public static float VolumeIncrease(float volume)
	{
		return Mathf.Pow(volume, 0.625f);
	}

	public static float TransformPitch(float pitchSemiTones)
	{
		return Mathf.Pow(2f, pitchSemiTones / 12f);
	}

	public static float InverseTransformPitch(float pitch)
	{
		return Mathf.Log(pitch) / Mathf.Log(2f) * 12f;
	}

	internal void VolumeApply()
	{
		float volumeTotalWithoutFade = this.volumeTotalWithoutFade;
		float volume = VolumeTransform(volumeTotalWithoutFade * _volumeMainFade);
		PrimaryAudioSource.volume = volume;
		if ((bool)SecondaryAudioSource)
		{
			volume = VolumeTransform(volumeTotalWithoutFade * _volumeSecondFade);
			SecondaryAudioSource.volume = volume;
		}
	}

	internal void PrimaryVolumeSet(float volumeMultiplier = 1f)
	{
		float num = VolumeTransform(volumeTotalWithoutFade * _volumeMainFade * volumeMultiplier);
		if (PrimaryAudioSource.volume != num)
		{
			PrimaryAudioSource.volume = num;
		}
	}

	internal void SetVolumeSecond(float volumeMultiplier = 1f)
	{
		if ((bool)SecondaryAudioSource)
		{
			float num = VolumeTransform(volumeTotalWithoutFade * _volumeSecondFade * volumeMultiplier);
			if (SecondaryAudioSource.volume != num)
			{
				SecondaryAudioSource.volume = num;
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SoundItem audioItem = this.AudioItem;
		if (audioItem != null && audioItem.overrideAudioSettings)
		{
			RestoreSettings();
		}
		ResetReferences();
		PrimaryAudioSource.outputAudioMixerGroup = _audioMixerGroup;
	}

	private void RestoreSettings()
	{
		PrimaryAudioSource.minDistance = _sourseDistance;
		PrimaryAudioSource.maxDistance = _souseDistanceSaved;
		PrimaryAudioSource.spatialBlend = _savedBlend;
		if (SecondaryAudioSource != null)
		{
			SecondaryAudioSource.minDistance = _sourseDistance;
			SecondaryAudioSource.maxDistance = _souseDistanceSaved;
			SecondaryAudioSource.spatialBlend = _savedBlend;
		}
	}

	public bool DoesBelongToCategory(string categoryName)
	{
		for (SoundCategory audioCategory = Category; audioCategory != null; audioCategory = audioCategory.ParentType)
		{
			if (audioCategory._name == categoryName)
			{
				return true;
			}
		}
		return false;
	}

	private float SequenceDelay(SoundItem audioItem)
	{
		float num = 0f - audioItem._loopSequenceOverlap;
		if (audioItem._loopSequenceRandomDelay > 0f)
		{
			num += UnityEngine.Random.Range(0f, audioItem._loopSequenceRandomDelay);
		}
		return num;
	}
}
