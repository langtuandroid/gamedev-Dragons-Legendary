using UnityEngine;

public class UnitySingleton<T> where T : MonoBehaviour
{
	private static T _instance;

	internal static GameObject _autoCreatePrefab;

	private static int _GlobalInstanceCount;

	private static bool _awakeSingletonCalled;

	private UnitySingleton()
	{
	}

	public static T GetSingleton(bool throwErrorIfNotFound, bool autoCreate, bool searchInObjectHierarchy = true)
	{
		if (!(Object)_instance)
		{
			T val = (T)null;
			if (searchInObjectHierarchy)
			{
				T[] array = UnityEngine.Object.FindObjectsOfType<T>();
				for (int i = 0; i < array.Length; i++)
				{
					ISingletonMonoBehaviour singletonMonoBehaviour = array[i] as ISingletonMonoBehaviour;
					if (singletonMonoBehaviour != null && singletonMonoBehaviour.isSingletonObject)
					{
						val = array[i];
						break;
					}
				}
			}
			if (!(Object)val)
			{
				if (!autoCreate || !(_autoCreatePrefab != null))
				{
					if (throwErrorIfNotFound)
					{
						UnityEngine.Debug.LogError("No singleton component " + typeof(T).Name + " found in the scene.");
					}
					return (T)null;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(_autoCreatePrefab);
				gameObject.name = _autoCreatePrefab.name;
				T exists = UnityEngine.Object.FindObjectOfType<T>();
				if (!(Object)exists)
				{
					UnityEngine.Debug.LogError("Auto created object does not have component " + typeof(T).Name);
					return (T)null;
				}
			}
			else
			{
				_AwakeSingleton(val);
			}
			_instance = val;
		}
		return _instance;
	}

	internal static void _Awake(T instance)
	{
		_GlobalInstanceCount++;
		if (_GlobalInstanceCount > 1)
		{
			UnityEngine.Debug.LogError("More than one instance of SingletonMonoBehaviour " + typeof(T).Name);
		}
		else
		{
			_instance = instance;
		}
		_AwakeSingleton(instance);
	}

	internal static void _Destroy()
	{
		if (_GlobalInstanceCount > 0)
		{
			_GlobalInstanceCount--;
			if (_GlobalInstanceCount == 0)
			{
				_awakeSingletonCalled = false;
				_instance = (T)null;
			}
		}
	}

	private static void _AwakeSingleton(T instance)
	{
		if (!_awakeSingletonCalled)
		{
			_awakeSingletonCalled = true;
			instance.SendMessage("AwakeSingleton", SendMessageOptions.DontRequireReceiver);
		}
	}
}
