using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
	public sealed class SpawnPool : MonoBehaviour, IList<Transform>, ICollection<Transform>, IEnumerable<Transform>, IEnumerable
	{
		public delegate GameObject InstantiateDelegate(GameObject prefab, Vector3 pos, Quaternion rot);

		public delegate void DestroyDelegate(GameObject instance);

		public string poolName = string.Empty;

		public bool matchPoolScale;

		public bool matchPoolLayer;

		public bool dontReparent;

		public bool _dontDestroyOnLoad;

		public bool logMessages;

		public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

		public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

		public float maxParticleDespawnTime = 300f;

		public PrefabsDict prefabs = new PrefabsDict();

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private List<PrefabPool> _prefabPools = new List<PrefabPool>();

		internal List<Transform> _spawned = new List<Transform>();

		public InstantiateDelegate instantiateDelegates;

		public DestroyDelegate destroyDelegates;

		public bool dontDestroyOnLoad
		{
			get
			{
				return _dontDestroyOnLoad;
			}
			set
			{
				_dontDestroyOnLoad = value;
				if (group != null)
				{
					UnityEngine.Object.DontDestroyOnLoad(group.gameObject);
				}
			}
		}

		public Transform group
		{
			get;
			private set;
		}

		public Dictionary<string, PrefabPool> prefabPools
		{
			get
			{
				Dictionary<string, PrefabPool> dictionary = new Dictionary<string, PrefabPool>();
				for (int i = 0; i < _prefabPools.Count; i++)
				{
					dictionary[_prefabPools[i].prefabGO.name] = _prefabPools[i];
				}
				return dictionary;
			}
		}

		public Transform this[int index]
		{
			get
			{
				return _spawned[index];
			}
			set
			{
				throw new NotImplementedException("Read-only.");
			}
		}

		public int Count => _spawned.Count;

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private void Awake()
		{
			if (_dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			group = base.transform;
			if (poolName == string.Empty)
			{
				poolName = group.name.Replace("Pool", string.Empty);
				poolName = poolName.Replace("(Clone)", string.Empty);
			}
			if (logMessages)
			{
				UnityEngine.Debug.Log($"SpawnPool {poolName}: Initializing..");
			}
			for (int i = 0; i < _perPrefabPoolOptions.Count; i++)
			{
				if (_perPrefabPoolOptions[i].prefab == null)
				{
					UnityEngine.Debug.LogWarning($"Initialization Warning: Pool '{poolName}' contains a PrefabPool with no prefab reference. Skipping.");
					continue;
				}
				_perPrefabPoolOptions[i].inspectorInstanceConstructor();
				CreatePrefabPool(_perPrefabPoolOptions[i]);
			}
			PoolManager.Pools.Add(this);
		}

		internal GameObject InstantiatePrefab(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			if (instantiateDelegates != null)
			{
				return instantiateDelegates(prefab, pos, rot);
			}
			return InstanceHandler.InstantiatePrefab(prefab, pos, rot);
		}

		internal void DestroyInstance(GameObject instance)
		{
			if (destroyDelegates != null)
			{
				destroyDelegates(instance);
			}
			else
			{
				InstanceHandler.DestroyInstance(instance);
			}
		}

		private void OnDestroy()
		{
			if (logMessages)
			{
				UnityEngine.Debug.Log($"SpawnPool {poolName}: Destroying...");
			}
			if (PoolManager.Pools.ContainsValue(this))
			{
				PoolManager.Pools.Remove(this);
			}
			StopAllCoroutines();
			_spawned.Clear();
			foreach (PrefabPool prefabPool in _prefabPools)
			{
				prefabPool.SelfDestruct();
			}
			_prefabPools.Clear();
			prefabs._Clear();
		}

		public void CreatePrefabPool(PrefabPool prefabPool)
		{
			if ((GetPrefabPool(prefabPool.prefab) != null) ? true : false)
			{
				throw new Exception($"Prefab '{prefabPool.prefab}' is already in  SpawnPool '{poolName}'. Prefabs can be in more than 1 SpawnPool but cannot be in the same SpawnPool twice.");
			}
			prefabPool.spawnPool = this;
			_prefabPools.Add(prefabPool);
			prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
			if (!prefabPool.preloaded)
			{
				if (logMessages)
				{
					UnityEngine.Debug.Log($"SpawnPool {poolName}: Preloading {prefabPool.preloadAmount} {prefabPool.prefab.name}");
				}
				prefabPool.PreloadInstances();
			}
		}

		public void Add(Transform instance, string prefabName, bool despawn, bool parent)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == null)
				{
					UnityEngine.Debug.LogError("Unexpected Error: PrefabPool.prefabGO is null");
					return;
				}
				if (_prefabPools[i].prefabGO.name == prefabName)
				{
					_prefabPools[i].AddUnpooled(instance, despawn);
					if (logMessages)
					{
						UnityEngine.Debug.Log($"SpawnPool {poolName}: Adding previously unpooled instance {instance.name}");
					}
					if (parent)
					{
						bool worldPositionStays = !(instance is RectTransform);
						instance.SetParent(group, worldPositionStays);
					}
					if (!despawn)
					{
						_spawned.Add(instance);
					}
					return;
				}
			}
			UnityEngine.Debug.LogError($"SpawnPool {poolName}: PrefabPool {prefabName} not found.");
		}

		public void Add(Transform item)
		{
			string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
			throw new NotImplementedException(message);
		}

		public void Remove(Transform item)
		{
			string message = "Use Despawn() to properly manage items that should remain in the pool but be deactivated.";
			throw new NotImplementedException(message);
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform;
			bool worldPositionStays;
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == prefab.gameObject)
				{
					transform = _prefabPools[i].SpawnInstance(pos, rot);
					if (transform == null)
					{
						return null;
					}
					worldPositionStays = !(transform is RectTransform);
					if (parent != null)
					{
						transform.SetParent(parent, worldPositionStays);
					}
					else if (!dontReparent && transform.parent != group)
					{
						transform.SetParent(group, worldPositionStays);
					}
					_spawned.Add(transform);
					transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
					return transform;
				}
			}
			PrefabPool prefabPool = new PrefabPool(prefab);
			CreatePrefabPool(prefabPool);
			transform = prefabPool.SpawnInstance(pos, rot);
			worldPositionStays = !(transform is RectTransform);
			if (parent != null)
			{
				transform.SetParent(parent, worldPositionStays);
			}
			else if (!dontReparent && transform.parent != group)
			{
				transform.SetParent(group, worldPositionStays);
			}
			_spawned.Add(transform);
			transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
			return transform;
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform = Spawn(prefab, pos, rot, null);
			if (transform == null)
			{
				return null;
			}
			return transform;
		}

		public Transform Spawn(Transform prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity);
		}

		public Transform Spawn(Transform prefab, Transform parent)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
		}

		public Transform Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			return Spawn(prefab.transform, pos, rot, parent);
		}

		public Transform Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab.transform, pos, rot);
		}

		public Transform Spawn(GameObject prefab)
		{
			return Spawn(prefab.transform);
		}

		public Transform Spawn(GameObject prefab, Transform parent)
		{
			return Spawn(prefab.transform, parent);
		}

		public Transform Spawn(string prefabName)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab);
		}

		public Transform Spawn(string prefabName, Transform parent)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab, parent);
		}

		public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab, pos, rot);
		}

		public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab, pos, rot, parent);
		}

		public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab, pos, rot, null);
		}

		public AudioSource Spawn(AudioSource prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, null);
		}

		public AudioSource Spawn(AudioSource prefab, Transform parent)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
		}

		public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform = Spawn(prefab.transform, pos, rot, parent);
			if (transform == null)
			{
				return null;
			}
			AudioSource component = transform.GetComponent<AudioSource>();
			component.Play();
			StartCoroutine(ListForAudioStop(component));
			return component;
		}

		public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab, pos, rot, null);
		}

		public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform = Spawn(prefab.transform, pos, rot, parent);
			if (transform == null)
			{
				return null;
			}
			ParticleSystem component = transform.GetComponent<ParticleSystem>();
			StartCoroutine(ListenForEmitDespawn(component));
			return component;
		}

		public void Despawn(Transform instance)
		{
			bool flag = false;
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i]._spawned.Contains(instance))
				{
					flag = _prefabPools[i].DespawnInstance(instance);
					break;
				}
				if (_prefabPools[i]._despawned.Contains(instance))
				{
					UnityEngine.Debug.LogError($"SpawnPool {poolName}: {instance.name} has already been despawned. You cannot despawn something more than once!");
					return;
				}
			}
			if (!flag)
			{
				UnityEngine.Debug.LogError($"SpawnPool {poolName}: {instance.name} not found in SpawnPool");
			}
			else
			{
				_spawned.Remove(instance);
			}
		}

		public void Despawn(Transform instance, Transform parent)
		{
			bool worldPositionStays = !(instance is RectTransform);
			instance.SetParent(parent, worldPositionStays);
			Despawn(instance);
		}

		public void Despawn(Transform instance, float seconds)
		{
			StartCoroutine(DoDespawnAfterSeconds(instance, seconds, useParent: false, null));
		}

		public void Despawn(Transform instance, float seconds, Transform parent)
		{
			StartCoroutine(DoDespawnAfterSeconds(instance, seconds, useParent: true, parent));
		}

		private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds, bool useParent, Transform parent)
		{
			GameObject go = instance.gameObject;
			while (seconds > 0f)
			{
				yield return null;
				if (!go.activeInHierarchy)
				{
					yield break;
				}
				seconds -= Time.deltaTime;
			}
			if (useParent)
			{
				Despawn(instance, parent);
			}
			else
			{
				Despawn(instance);
			}
		}

		public void DespawnAll()
		{
			List<Transform> list = new List<Transform>(_spawned);
			for (int i = 0; i < list.Count; i++)
			{
				Despawn(list[i]);
			}
		}

		public bool IsSpawned(Transform instance)
		{
			return _spawned.Contains(instance);
		}

		public PrefabPool GetPrefabPool(Transform prefab)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == null)
				{
					UnityEngine.Debug.LogError($"SpawnPool {poolName}: PrefabPool.prefabGO is null");
				}
				if (_prefabPools[i].prefabGO == prefab.gameObject)
				{
					return _prefabPools[i];
				}
			}
			return null;
		}

		public PrefabPool GetPrefabPool(GameObject prefab)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == null)
				{
					UnityEngine.Debug.LogError($"SpawnPool {poolName}: PrefabPool.prefabGO is null");
				}
				if (_prefabPools[i].prefabGO == prefab)
				{
					return _prefabPools[i];
				}
			}
			return null;
		}

		public Transform GetPrefab(Transform instance)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].Contains(instance))
				{
					return _prefabPools[i].prefab;
				}
			}
			return null;
		}

		public GameObject GetPrefab(GameObject instance)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].Contains(instance.transform))
				{
					return _prefabPools[i].prefabGO;
				}
			}
			return null;
		}

		private IEnumerator ListForAudioStop(AudioSource src)
		{
			yield return null;
			GameObject srcGameObject = src.gameObject;
			while (src.isPlaying)
			{
				yield return null;
			}
			if (!srcGameObject.activeInHierarchy)
			{
				src.Stop();
			}
			else
			{
				Despawn(src.transform);
			}
		}

		private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
		{
			yield return new WaitForSeconds(emitter.startDelay + 0.25f);
			float safetimer = 0f;
			GameObject emitterGO = emitter.gameObject;
			while (emitter.IsAlive(withChildren: true) && emitterGO.activeInHierarchy)
			{
				safetimer += Time.deltaTime;
				if (safetimer > maxParticleDespawnTime)
				{
					UnityEngine.Debug.LogWarning($"SpawnPool {poolName}: Timed out while listening for all particles to die. Waited for {maxParticleDespawnTime}sec.");
				}
				yield return null;
			}
			if (emitterGO.activeInHierarchy)
			{
				Despawn(emitter.transform);
				emitter.Clear(withChildren: true);
			}
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (Transform item in _spawned)
			{
				list.Add(item.name);
			}
			return string.Join(", ", list.ToArray());
		}

		public bool Contains(Transform item)
		{
			string message = "Use IsSpawned(Transform instance) instead.";
			throw new NotImplementedException(message);
		}

		public void CopyTo(Transform[] array, int arrayIndex)
		{
			_spawned.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Transform> GetEnumerator()
		{
			for (int i = 0; i < _spawned.Count; i++)
			{
				yield return _spawned[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _spawned.Count; i++)
			{
				yield return _spawned[i];
			}
		}

		public int IndexOf(Transform item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, Transform item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		bool ICollection<Transform>.Remove(Transform item)
		{
			throw new NotImplementedException();
		}
	}
}
