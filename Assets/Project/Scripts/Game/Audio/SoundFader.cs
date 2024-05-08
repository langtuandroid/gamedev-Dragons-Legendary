using UnityEngine;

public class SoundFader
{
	private float _fadeOutTime = -1f;

	private double _startFadeOut = -1.0;

	private float _fadeInTime = -1f;

	private double _fadeInStart = -1.0;

	public double time { get; set; }

	public bool IsFading
	{
		get
		{
			if (_startFadeOut > 0.0)
			{
				return _fadeOutTime >= 0f && time >= _startFadeOut && time < _startFadeOut + (double)_fadeOutTime;
			}
			return _fadeOutTime >= 0f && time < (double)_fadeOutTime;
		}
	}

	public bool IsScheduled => _fadeOutTime >= 0f;

	private bool IsFadingIn
	{
		get
		{
			if (_fadeInStart > 0.0)
			{
				return _fadeInTime > 0f && time >= _fadeInStart && time - _fadeInStart < (double)_fadeInTime;
			}
			return _fadeInTime > 0f && time < (double)_fadeInTime;
		}
	}

	public void Reset()
	{
		time = 0.0;
		_fadeOutTime = -1f;
		_startFadeOut = -1.0;
		_fadeInTime = -1f;
		_fadeInStart = -1.0;
	}

	public void FadeSoundIn(float fadeInTime, bool stopCurrentFadeOut = false)
	{
		FadeSoundIn(fadeInTime, time, stopCurrentFadeOut);
	}

	public void FadeSoundIn(float fadeInTime, double startToFadeTime, bool stopCurrentFadeOut = false)
	{
		if (IsScheduled && stopCurrentFadeOut)
		{
			float num = GetSoundOutValue();
			_fadeOutTime = -1f;
			_startFadeOut = -1.0;
			_fadeInTime = fadeInTime;
			_fadeInStart = startToFadeTime - (double)(fadeInTime * num);
		}
		else
		{
			_fadeInTime = fadeInTime;
			_fadeInStart = startToFadeTime;
		}
	}

	public void FadeSoundOut(float fadeOutLength, float startToFadeTime)
	{
		if (IsScheduled)
		{
			double num = time + (double)startToFadeTime + (double)fadeOutLength;
			double num2 = _startFadeOut + (double)_fadeOutTime;
			if (!(num2 < num))
			{
				double num3 = time - _startFadeOut;
				double num4 = startToFadeTime + fadeOutLength;
				double num5 = num2 - time;
				if (num5 != 0.0)
				{
					double num6 = num3 * num4 / num5;
					_startFadeOut = time - num6;
					_fadeOutTime = (float)(num4 + num6);
				}
			}
		}
		else
		{
			_fadeOutTime = fadeOutLength;
			_startFadeOut = time + (double)startToFadeTime;
		}
	}

	public float Get()
	{
		bool finishedFadeOut;
		return Get(out finishedFadeOut);
	}

	public float Get(out bool finishedFadeOut)
	{
		float num = 1f;
		finishedFadeOut = false;
		if (IsScheduled)
		{
			num *= GetSoundOutValue();
			if (num == 0f)
			{
				finishedFadeOut = true;
				return 0f;
			}
		}
		if (IsFadingIn)
		{
			num *= GetFadeSoundInValue();
		}
		return num;
	}

	private float GetSoundOutValue()
	{
		return 1f - FadeValue((float)(time - _startFadeOut), _fadeOutTime);
	}

	private float GetFadeSoundInValue()
	{
		return FadeValue((float)(time - _fadeInStart), _fadeInTime);
	}

	private float FadeValue(float t, float dt)
	{
		if (dt <= 0f)
		{
			return (!(t > 0f)) ? 0f : 1f;
		}
		return Mathf.Clamp(t / dt, 0f, 1f);
	}
}
