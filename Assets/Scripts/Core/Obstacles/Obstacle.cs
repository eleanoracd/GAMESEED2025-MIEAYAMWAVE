using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float npcSpawnRadius = 0.5f;
    [SerializeField] private int maxNPCsToSpawn = 3;

    private int targetCultistsToConvert;
    private int currentConvertedCount = 0;
    private bool initialized = false;

    private void OnEnable()
    {
        targetCultistsToConvert = Random.Range(1, maxNPCsToSpawn + 1);
        currentConvertedCount = 0;
        initialized = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized) return;
        if (currentConvertedCount >= targetCultistsToConvert) return;

        Cultist cultist = other.GetComponent<Cultist>();
        if (cultist != null)
        {
            RevertCultistToNPC(cultist);
            currentConvertedCount++;

            if (currentConvertedCount >= targetCultistsToConvert)
            {
                Debug.Log("Obstacle reached conversion limit.");
            }
        }
    }

    private void RevertCultistToNPC(Cultist cultist)
    {
        CultistManager cultistManager = FindObjectOfType<CultistManager>();
        if (cultistManager != null)
        {
            cultistManager.ReturnToPool(cultist.gameObject);
        }

        SpawnNPCsAtObstaclePosition();
    }
    
    private void SpawnNPCsAtObstaclePosition()
    {
        NPCManager npcManager = FindObjectOfType<NPCManager>();
        if (npcManager == null) return;

        float angle = currentConvertedCount * (360f / maxNPCsToSpawn);
        Vector3 offset = Quaternion.Euler(0, 0, angle) * Vector3.right * npcSpawnRadius;

        GameObject npc = npcManager.SpawnNPC(transform.position + offset);
        if (npc != null)
        {
            npc.GetComponent<NPC>().ResetConversion();
            npc.transform.parent = transform.parent;
        }
    }
}
