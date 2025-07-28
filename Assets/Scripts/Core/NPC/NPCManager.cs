using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> npcPool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject npc = Instantiate(npcPrefab);
            npc.SetActive(false);
            npcPool.Enqueue(npc);
        }
    }

    public GameObject SpawnNPC(Vector3 spawnPosition)
    {
        if (npcPool.Count == 0) return null;

        GameObject npc = npcPool.Dequeue();
        npc.transform.position = spawnPosition;
        npc.SetActive(true);
        return npc;
    }

    public void ReturnToPool(GameObject npc)
    {
        npc.SetActive(false);
        npcPool.Enqueue(npc);
    }
}