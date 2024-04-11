using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	public class SpawnPoolsDict : IDictionary<string, SpawnPool>, ICollection<KeyValuePair<string, SpawnPool>>, IEnumerable<KeyValuePair<string, SpawnPool>>, IEnumerable
	{
		public delegate void OnCreatedDelegate(SpawnPool pool);

		internal Dictionary<string, OnCreatedDelegate> onCreatedDelegates = new Dictionary<string, OnCreatedDelegate>();

		private Dictionary<string, SpawnPool> _pools = new Dictionary<string, SpawnPool>();

		bool ICollection<KeyValuePair<string, SpawnPool>>.IsReadOnly => true;

		public int Count => _pools.Count;

		public SpawnPool this[string key]
		{
			get
			{
				try
				{
					return _pools[key];
				}
				catch (KeyNotFoundException)
				{
					string message = $"A Pool with the name '{key}' not found. \nPools={ToString()}";
					throw new KeyNotFoundException(message);
				}
			}
			set
			{
				string message = "Cannot set PoolManager.Pools[key] directly. SpawnPools add themselves to PoolManager.Pools when created, so there is no need to set them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
				throw new NotImplementedException(message);
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				string message = "If you need this, please request it.";
				throw new NotImplementedException(message);
			}
		}

		public ICollection<SpawnPool> Values
		{
			get
			{
				string message = "If you need this, please request it.";
				throw new NotImplementedException(message);
			}
		}

		private bool IsReadOnly => true;

		public void AddOnCreatedDelegate(string poolName, OnCreatedDelegate createdDelegate)
		{
			if (!onCreatedDelegates.ContainsKey(poolName))
			{
				onCreatedDelegates.Add(poolName, createdDelegate);
				UnityEngine.Debug.Log($"Added onCreatedDelegates for pool '{poolName}': {createdDelegate.Target}");
			}
			else
			{
				Dictionary<string, OnCreatedDelegate> dictionary;
				string key;
				(dictionary = onCreatedDelegates)[key = poolName] = (OnCreatedDelegate)Delegate.Combine(dictionary[key], createdDelegate);
			}
		}

		public void RemoveOnCreatedDelegate(string poolName, OnCreatedDelegate createdDelegate)
		{
			if (!onCreatedDelegates.ContainsKey(poolName))
			{
				throw new KeyNotFoundException("No OnCreatedDelegates found for pool name '" + poolName + "'.");
			}
			Dictionary<string, OnCreatedDelegate> dictionary;
			string key;
			(dictionary = onCreatedDelegates)[key = poolName] = (OnCreatedDelegate)Delegate.Remove(dictionary[key], createdDelegate);
			UnityEngine.Debug.Log($"Removed onCreatedDelegates for pool '{poolName}': {createdDelegate.Target}");
		}

		public SpawnPool Create(string poolName)
		{
			GameObject gameObject = new GameObject(poolName + "Pool");
			return gameObject.AddComponent<SpawnPool>();
		}

		public SpawnPool Create(string poolName, GameObject owner)
		{
			if (!assertValidPoolName(poolName))
			{
				return null;
			}
			string name = owner.gameObject.name;
			try
			{
				owner.gameObject.name = poolName;
				return owner.AddComponent<SpawnPool>();
			}
			finally
			{
				owner.gameObject.name = name;
			}
		}

		private bool assertValidPoolName(string poolName)
		{
			string text = poolName.Replace("Pool", string.Empty);
			if (text != poolName)
			{
				string message = $"'{poolName}' has the word 'Pool' in it. This word is reserved for GameObject defaul naming. The pool name has been changed to '{text}'";
				UnityEngine.Debug.LogWarning(message);
				poolName = text;
			}
			if (ContainsKey(poolName))
			{
				UnityEngine.Debug.Log($"A pool with the name '{poolName}' already exists");
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			string[] array = new string[_pools.Count];
			_pools.Keys.CopyTo(array, 0);
			return string.Format("[{0}]", string.Join(", ", array));
		}

		public bool Destroy(string poolName)
		{
			if (!_pools.TryGetValue(poolName, out SpawnPool value))
			{
				UnityEngine.Debug.LogError($"PoolManager: Unable to destroy '{poolName}'. Not in PoolManager");
				return false;
			}
			UnityEngine.Object.Destroy(value.gameObject);
			_pools.Remove(value.poolName);
			return true;
		}

		public void DestroyAll()
		{
			foreach (KeyValuePair<string, SpawnPool> pool in _pools)
			{
				UnityEngine.Debug.Log("DESTROYING: " + pool.Value.gameObject.name);
				UnityEngine.Object.Destroy(pool.Value.gameObject);
			}
			_pools.Clear();
		}

		internal void Add(SpawnPool spawnPool)
		{
			if (ContainsKey(spawnPool.poolName))
			{
				UnityEngine.Debug.LogError($"A pool with the name '{spawnPool.poolName}' already exists. This should only happen if a SpawnPool with this name is added to a scene twice.");
				return;
			}
			_pools.Add(spawnPool.poolName, spawnPool);
			UnityEngine.Debug.Log($"Added pool '{spawnPool.poolName}'");
			if (onCreatedDelegates.ContainsKey(spawnPool.poolName))
			{
				onCreatedDelegates[spawnPool.poolName](spawnPool);
			}
		}

		public void Add(string key, SpawnPool value)
		{
			string message = "SpawnPools add themselves to PoolManager.Pools when created, so there is no need to Add() them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
			throw new NotImplementedException(message);
		}

		internal bool Remove(SpawnPool spawnPool)
		{
			if (!ContainsValue(spawnPool) & Application.isPlaying)
			{
				UnityEngine.Debug.LogError($"PoolManager: Unable to remove '{spawnPool.poolName}'. Pool not in PoolManager");
				return false;
			}
			_pools.Remove(spawnPool.poolName);
			return true;
		}

		public bool Remove(string poolName)
		{
			string message = "SpawnPools can only be destroyed, not removed and kept alive outside of PoolManager. There are only 2 legal ways to destroy a SpawnPool: Destroy the GameObject directly, if you have a reference, or use PoolManager.Destroy(string poolName).";
			throw new NotImplementedException(message);
		}

		public bool ContainsKey(string poolName)
		{
			return _pools.ContainsKey(poolName);
		}

		public bool ContainsValue(SpawnPool pool)
		{
			return _pools.ContainsValue(pool);
		}

		public bool TryGetValue(string poolName, out SpawnPool spawnPool)
		{
			return _pools.TryGetValue(poolName, out spawnPool);
		}

		public bool Contains(KeyValuePair<string, SpawnPool> item)
		{
			throw new NotImplementedException("Use PoolManager.Pools.ContainsKey(string poolName) or PoolManager.Pools.ContainsValue(SpawnPool pool) instead.");
		}

		public void Add(KeyValuePair<string, SpawnPool> item)
		{
			string message = "SpawnPools add themselves to PoolManager.Pools when created, so there is no need to Add() them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
			throw new NotImplementedException(message);
		}

		public void Clear()
		{
			string message = "Use PoolManager.Pools.DestroyAll() instead.";
			throw new NotImplementedException(message);
		}

		private void CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
		{
			string message = "PoolManager.Pools cannot be copied";
			throw new NotImplementedException(message);
		}

		void ICollection<KeyValuePair<string, SpawnPool>>.CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
		{
			string message = "PoolManager.Pools cannot be copied";
			throw new NotImplementedException(message);
		}

		public bool Remove(KeyValuePair<string, SpawnPool> item)
		{
			string message = "SpawnPools can only be destroyed, not removed and kept alive outside of PoolManager. There are only 2 legal ways to destroy a SpawnPool: Destroy the GameObject directly, if you have a reference, or use PoolManager.Destroy(string poolName).";
			throw new NotImplementedException(message);
		}

		public IEnumerator<KeyValuePair<string, SpawnPool>> GetEnumerator()
		{
			return _pools.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _pools.GetEnumerator();
		}
	}
}
