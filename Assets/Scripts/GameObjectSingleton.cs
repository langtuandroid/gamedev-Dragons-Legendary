using UnityEngine;

public class GameObjectSingleton<T> : MonoBehaviour where T : GameObjectSingleton<T>
{
	private static T instance;

	public static bool Loaded => (Object)instance != (Object)null && instance.Valid;

	protected static T Inst => instance;

	protected virtual bool Valid => (Object)instance != (Object)null;

	protected virtual void Awake()
	{
		if (!((Object)instance != (Object)null))
		{
			instance = (this as T);
			OnAttached();
		}
	}

	protected virtual void OnDestroy()
	{
		if (object.ReferenceEquals(this, instance))
		{
			instance = (T)null;
			OnDetached();
		}
	}

	protected virtual void OnAttached()
	{
	}

	protected virtual void OnDetached()
	{
	}

	public static T Create(string strObjectName)
	{
		if (Loaded)
		{
			return instance;
		}
		GameObject gameObject = new GameObject(strObjectName);
		return gameObject.AddComponent<T>();
	}
}
