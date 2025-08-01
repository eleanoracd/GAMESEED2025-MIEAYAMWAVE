using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistManager : MonoBehaviour
{
    [SerializeField] private GameObject cultistPrefab;
    [SerializeField] private GameObject leader;
    [SerializeField] private Vector3 leaderSpawnPosition = Vector3.zero;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float spacingBehind = 1f;

    [Header("Formation Settings")]
    [SerializeField] private Transform formationAnchor;

    private List<GameObject> cultistPool = new List<GameObject>();
    private List<GameObject> activeCultists = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject cultist = Instantiate(cultistPrefab);
            cultist.SetActive(false);
            cultistPool.Add(cultist);
        }
    }

    private void Start() {
        leader = GetPooledCultist();
        if (leader != null)
        {
            leader.GetComponent<Cultist>().SetAsLeader();
            leader.transform.position = leaderSpawnPosition;
            leader.SetActive(true);
            activeCultists.Add(leader);

            PlatformManager platformManager = FindObjectOfType<PlatformManager>();
            if (platformManager != null)
            {
                platformManager.SetPlayer(leader);
            }
        }
    }

    private void LateUpdate()
    {
        if (formationAnchor == null || leader == null) return;

        Vector3 leaderTarget = new Vector3(
            formationAnchor.position.x,
            leader.transform.position.y,
            leader.transform.position.z
        );
        leader.GetComponent<Cultist>().SetTargetXPosition(leaderTarget.x);

        for (int i = 0; i < activeCultists.Count; i++)
        {
            if (activeCultists[i] == leader) continue;

            float targetX;
            if (i == 0)
            {
                targetX = leader.transform.position.x - spacingBehind;
            }
            else
            {
                targetX = activeCultists[i-1].transform.position.x - spacingBehind;
            }

            activeCultists[i].GetComponent<Cultist>().SetTargetXPosition(targetX);
        }
    }

    public GameObject SpawnCultistBehindLast()
    {
        GameObject cultist = GetPooledCultist();
        if (cultist == null) return null;

        Vector3 spawnPosition;
        
        if (activeCultists.Count > 0)
        {
            GameObject lastCultist = activeCultists[activeCultists.Count - 1];
            spawnPosition = lastCultist.transform.position - new Vector3(spacingBehind, 0f, 0f);
        }
        else if (leader != null)
        {
            spawnPosition = leader.transform.position - new Vector3(spacingBehind, 0f, 0f);
        }
        else
        {
            spawnPosition = formationAnchor.position - new Vector3(spacingBehind, 0f, 0f);
        }

        cultist.transform.position = spawnPosition;
        cultist.GetComponent<Cultist>().SetTargetXPosition(spawnPosition.x);
        cultist.SetActive(true);
        activeCultists.Add(cultist);
        
        return cultist;
    }

    private GameObject GetPooledCultist()
    {
        foreach (var c in cultistPool)
        {
            if (!c.activeInHierarchy)
            {
                return c;
            }
        }

        GameObject newCultist = Instantiate(cultistPrefab);
        newCultist.SetActive(false);
        cultistPool.Add(newCultist);
        return newCultist;
    }

    public void ReturnToPool(GameObject cultist)
    {
        if (cultist == null) return;

        Cultist cultistComponent = cultist.GetComponent<Cultist>();
        if (cultistComponent == null) return;

        if (activeCultists.Contains(cultist))
        {
            bool wasLeader = cultistComponent.IsLeader();
            activeCultists.Remove(cultist);

            if (wasLeader)
            {
                HandleLeaderDespawned();
            }
        }

        cultist.SetActive(false);
        cultistComponent.SetAsFollower();
        GameManager.Instance?.CheckGameOver();
    }

    public void HandleLeaderDespawned()
    {
        activeCultists.RemoveAll(item => item == null);

        if (activeCultists.Count > 0)
        {
            GameObject newLeader = activeCultists[0];
            Cultist newLeaderCultist = newLeader.GetComponent<Cultist>();
            
            if (newLeaderCultist != null)
            {
                newLeaderCultist.SetAsLeader();
                leader = newLeader;
            }
        }
        GameManager.Instance?.CheckGameOver();
    }

    public List<GameObject> GetActiveCultists()
    {
        return activeCultists;
    }
}
