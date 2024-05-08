using UnityEngine;
using UnityEngine.Serialization;

public abstract class SoundTriggerBase : MonoBehaviour
{
	public enum Type
	{
		Start,
		Awake,
		OnDestroy,
		OnCollisionEnter,
		OnCollisionExit,
		OnEnable,
		OnDisable
	}

	[FormerlySerializedAs("triggerEvent")] public Type _eventType;

	protected virtual void Awake()
	{
		ConfigureEvent(Type.Awake);
	}

	protected virtual void Start()
	{
		ConfigureEvent(Type.Start);
	}

	protected virtual void OnDestroy()
	{
		if (_eventType == Type.OnDestroy && (bool)SingletonMonoBehaviour<AudioController>.DoesInstanceExist())
		{
			ConfigureEvent(Type.OnDestroy);
		}
	}

	protected virtual void OnCollisionEnter()
	{
		ConfigureEvent(Type.OnCollisionEnter);
	}

	protected virtual void OnCollisionExit()
	{
		ConfigureEvent(Type.OnCollisionExit);
	}

	protected virtual void OnEnable()
	{
		ConfigureEvent(Type.OnEnable);
	}

	protected virtual void OnDisable()
	{
		ConfigureEvent(Type.OnDisable);
	}

	protected abstract void _OnEventTriggered();

	protected virtual void ConfigureEvent(Type eventType)
	{
		if (_eventType == eventType)
		{
			_OnEventTriggered();
		}
	}
}
