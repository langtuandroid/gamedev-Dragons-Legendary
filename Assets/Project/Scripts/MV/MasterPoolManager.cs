using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

public class MasterPoolManager : GameObjectSingleton<MasterPoolManager>
{
    private static MasterPoolManager instance;
    
    private Dictionary<string, Transform> _dicPoolParent = new Dictionary<string, Transform>();
    
    private Dictionary<string, bool> _dicPoolAllDespawn = new Dictionary<string, bool>();

    private Transform _poolManagerTransform;

    private Dictionary<string, List<Transform>> _dicSpawnPool = new Dictionary<string, List<Transform>>();

    public static Transform SpawnObject(string poolName, string spawnName, Transform parent = null, float removeTime = -1f, bool isSpeedProcess = true, bool isScaleChange = true)
    {
        Transform transform = PoolManager.Pools[poolName].Spawn(spawnName, Vector3.zero, Quaternion.Euler(Vector3.zero), parent);
        transform.localPosition = Vector3.zero;
        if (isScaleChange)
        {
            transform.localScale = Vector3.one;
        }
        if (!Inst._dicSpawnPool.ContainsKey(poolName))
        {
            Inst._dicSpawnPool.Add(poolName, new List<Transform>());
        }
        Inst._dicSpawnPool[poolName].Add(transform);
        if (removeTime > -1f)
        {
            Inst.StartCoroutine(Inst.ReturnToPoolDuration(poolName, transform, removeTime));
        }
        if (isSpeedProcess)
        {
            Inst.PoolObjectSpeed(poolName, transform);
        }
        return transform;
    }

    public static void ReturnToPool(string poolName, Transform trObj)
    {
        if (!(Inst == null) && IsSpawned(poolName, trObj) && trObj != null && !Inst._dicPoolAllDespawn[poolName])
        {
            trObj.SetParent(Inst._dicPoolParent[poolName]);
            Inst._dicSpawnPool[poolName].Remove(trObj);
            PoolManager.Pools[poolName].Despawn(trObj);
        }
    }

    public static void ReturnToPoolAll(string poolName)
    {
        if (Inst._dicSpawnPool.ContainsKey(poolName) && !Inst._dicPoolAllDespawn[poolName])
        {
            Inst._dicPoolAllDespawn[poolName] = true;
            foreach (Transform item in Inst._dicSpawnPool[poolName])
            {
                if (item != null)
                {
                    item.SetParent(Inst._dicPoolParent[poolName]);
                    PoolManager.Pools[poolName].Despawn(item);
                }
            }
            Inst._dicPoolAllDespawn[poolName] = false;
            Inst._dicSpawnPool[poolName].Clear();
        }
    }

    public static bool IsSpawned(string poolName, Transform trSpawn)
    {
        return PoolManager.Pools[poolName].IsSpawned(trSpawn);
    }

    private IEnumerator ReturnToPoolDuration(string poolName, Transform trSpawn, float removeTime)
    {
        yield return new WaitForSeconds(removeTime);
        ReturnToPool(poolName, trSpawn);
    }

    private void PoolObjectSpeed(string poolName, Transform trSpawn)
    {
        switch (GameInfo.currentSceneType)
        {
            case SceneType.InGame:
                if (poolName == "Effect" || poolName == "Hunter" || poolName == "Monster" || poolName == "Stage")
                {
                    ParticleSystem[] componentsInChildren3 = trSpawn.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
                    foreach (ParticleSystem particleSystem2 in componentsInChildren3)
                    {
                        var main = particleSystem2.main;
                        main.simulationSpeed = GameInfo.inGameBattleSpeedRate;
                    }
                    Animator[] componentsInChildren4 = trSpawn.GetComponentsInChildren<Animator>(includeInactive: true);
                    foreach (Animator animator2 in componentsInChildren4)
                    {
                        animator2.speed = GameInfo.inGameBattleSpeedRate;
                    }
                }
                break;
            case SceneType.Lobby:
                if (poolName == "Hunter" || poolName == "Monster")
                {
                    ParticleSystem[] componentsInChildren = trSpawn.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
                    foreach (ParticleSystem particleSystem in componentsInChildren)
                    {
                        var main = particleSystem.main;
                        main.simulationSpeed = 1f;
                    }
                    Animator[] componentsInChildren2 = trSpawn.GetComponentsInChildren<Animator>(includeInactive: true);
                    foreach (Animator animator in componentsInChildren2)
                    {
                        animator.speed = 1f;
                    }
                }
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _poolManagerTransform = gameObject.transform;
        SpawnPool[] componentsInChildren = _poolManagerTransform.GetComponentsInChildren<SpawnPool>();
        foreach (SpawnPool spawnPool in componentsInChildren)
        {
            Debug.Log("pool name :: " + spawnPool.poolName);
            _dicPoolParent.Add(spawnPool.poolName, spawnPool.transform);
            _dicPoolAllDespawn.Add(spawnPool.poolName, value: false);
        }
    }
}