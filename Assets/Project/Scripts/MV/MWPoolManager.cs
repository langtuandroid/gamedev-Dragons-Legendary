using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MWPoolManager : GameObjectSingleton<MWPoolManager>
{
    public const string POOL_NAME_HUNTER = "Hunter";

    public const string POOL_NAME_MONSTER = "Monster";

    public const string POOL_NAME_PUZZLE = "Puzzle";

    public const string POOL_NAME_EFFECT = "Effect";

    public const string POOL_NAME_SKILL = "Skill";

    public const string POOL_NAME_LOBBY = "Lobby";

    public const string POOL_NAME_Stage = "Stage";

    public const string POOL_NAME_ITEM = "Item";

    public const string POOL_NAME_Grow = "Grow";

    public const string POOL_NAME_INFO = "Info";

    public const string POOL_NAME_SCENARIO = "Scenario";

    public const string POOL_NAME_TUTORIAL = "Tutorial";

    private static MWPoolManager instance;

    [SerializeField]
    private Dictionary<string, Transform> dicPoolParent = new Dictionary<string, Transform>();

    [SerializeField]
    private Dictionary<string, bool> dicPoolAllDespawn = new Dictionary<string, bool>();

    private Transform trPoolManager;

    private Dictionary<string, List<Transform>> dicSpawnPool = new Dictionary<string, List<Transform>>();

    public static Transform Spawn(string poolName, string spawnName, Transform parent = null, float removeTime = -1f, bool isSpeedProcess = true, bool isScaleChange = true)
    {
        Transform transform = PoolManager.Pools[poolName].Spawn(spawnName, Vector3.zero, Quaternion.Euler(Vector3.zero), parent);
        transform.localPosition = Vector3.zero;
        if (isScaleChange)
        {
            transform.localScale = Vector3.one;
        }
        if (!GameObjectSingleton<MWPoolManager>.Inst.dicSpawnPool.ContainsKey(poolName))
        {
            GameObjectSingleton<MWPoolManager>.Inst.dicSpawnPool.Add(poolName, new List<Transform>());
        }
        GameObjectSingleton<MWPoolManager>.Inst.dicSpawnPool[poolName].Add(transform);
        if (removeTime > -1f)
        {
            GameObjectSingleton<MWPoolManager>.Inst.StartCoroutine(GameObjectSingleton<MWPoolManager>.Inst.CheckRemovePoolDuration(poolName, transform, removeTime));
        }
        if (isSpeedProcess)
        {
            GameObjectSingleton<MWPoolManager>.Inst.SetPoolObjectSpeed(poolName, transform);
        }
        return transform;
    }

    public static void DeSpawn(string poolName, Transform trObj)
    {
        if (!(GameObjectSingleton<MWPoolManager>.Inst == null) && CheckSpawned(poolName, trObj) && trObj != null && !GameObjectSingleton<MWPoolManager>.Inst.dicPoolAllDespawn[poolName])
        {
            trObj.SetParent(GameObjectSingleton<MWPoolManager>.Inst.dicPoolParent[poolName]);
            GameObjectSingleton<MWPoolManager>.Inst.dicSpawnPool[poolName].Remove(trObj);
            PoolManager.Pools[poolName].Despawn(trObj);
        }
    }

    public static void DeSpawnPoolAll(string poolName)
    {
        if (GameObjectSingleton<MWPoolManager>.Inst.dicSpawnPool.ContainsKey(poolName) && !GameObjectSingleton<MWPoolManager>.Inst.dicPoolAllDespawn[poolName])
        {
            GameObjectSingleton<MWPoolManager>.Inst.dicPoolAllDespawn[poolName] = true;
            foreach (Transform item in GameObjectSingleton<MWPoolManager>.Inst.dicSpawnPool[poolName])
            {
                if (item != null)
                {
                    item.SetParent(GameObjectSingleton<MWPoolManager>.Inst.dicPoolParent[poolName]);
                    PoolManager.Pools[poolName].Despawn(item);
                }
            }
            GameObjectSingleton<MWPoolManager>.Inst.dicPoolAllDespawn[poolName] = false;
            GameObjectSingleton<MWPoolManager>.Inst.dicSpawnPool[poolName].Clear();
        }
    }

    public static bool CheckSpawned(string poolName, Transform trSpawn)
    {
        return PoolManager.Pools[poolName].IsSpawned(trSpawn);
    }

    private IEnumerator CheckRemovePoolDuration(string poolName, Transform trSpawn, float removeTime)
    {
        yield return new WaitForSeconds(removeTime);
        DeSpawn(poolName, trSpawn);
    }

    private void SetPoolObjectSpeed(string poolName, Transform trSpawn)
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
        trPoolManager = base.gameObject.transform;
        SpawnPool[] componentsInChildren = trPoolManager.GetComponentsInChildren<SpawnPool>();
        foreach (SpawnPool spawnPool in componentsInChildren)
        {
            UnityEngine.Debug.Log("pool name :: " + spawnPool.poolName);
            dicPoolParent.Add(spawnPool.poolName, spawnPool.transform);
            dicPoolAllDespawn.Add(spawnPool.poolName, value: false);
        }
    }
}