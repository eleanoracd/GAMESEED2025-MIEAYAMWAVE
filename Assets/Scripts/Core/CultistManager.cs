using System.Collections.Generic;
using UnityEngine;

public class CultistManager : MonoBehaviour
{
    public static CultistManager Instance;

    [Header("Settings")]
    [SerializeField] private CultistLogic.CultistPrefab[] cultistPrefabs;
    [SerializeField] private int initialPoolSize = 10;

    private List<CultistLogic> activeCultists = new List<CultistLogic>();
    private Dictionary<CultistType, Queue<GameObject>> cultistPools = new Dictionary<CultistType, Queue<GameObject>>();
    private CultistLogic leader;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var cp in cultistPrefabs)
        {
            var pool = new Queue<GameObject>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = Instantiate(cp.prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            cultistPools.Add(cp.type, pool);
        }
    }

    public void SpawnCultist(CultistType type, Vector3 position, Quaternion rotation)
    {
        if (cultistPools.TryGetValue(type, out var pool))
        {
            GameObject cultistObj = pool.Count > 0 ? pool.Dequeue() : Instantiate(GetPrefab(type));
            cultistObj.transform.SetPositionAndRotation(position, rotation);
            cultistObj.SetActive(true);

            CultistLogic cultistComp = cultistObj.GetComponent<CultistLogic>();
            activeCultists.Add(cultistComp);

            if (leader != null)
            {
                cultistComp.SetAsFollower(leader.transform);
            }
            else
            {
                SetLeader(cultistComp);
            }
        }
    }

    public void SetLeader(CultistLogic newLeader)
    {
        if (leader != null)
        {
            leader.SetAsFollower(newLeader.transform);
        }
        leader = newLeader;
        leader.SetAsLeader();
    }

    public void ReturnCultist(CultistLogic cultist)
    {
        if (activeCultists.Remove(cultist))
        {
            CultistType type = cultist.MyType;
            cultist.gameObject.SetActive(false);
            cultistPools[type].Enqueue(cultist.gameObject);

            if (cultist == leader)
            {
                leader = activeCultists.Count > 0 ? activeCultists[0] : null;
                if (leader != null) leader.SetAsLeader();
            }
        }
    }

    private GameObject GetPrefab(CultistType type)
    {
        foreach (var cp in cultistPrefabs)
        {
            if (cp.type == type) return cp.prefab;
        }
        return null;
    }
}