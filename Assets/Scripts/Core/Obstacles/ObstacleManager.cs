using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float spawnChance = 0.3f;
    [SerializeField] private float minDistanceBetweenObstacles = 3f;
    [SerializeField] private float obstacleHorizontalPadding = 0.5f;
    [SerializeField] private int maxObstaclesPerPlatform = 3;

    private Queue<GameObject> obstaclePool = new Queue<GameObject>();
    private NPCManager npcManager;

    private void Awake()
    {
        npcManager = FindObjectOfType<NPCManager>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obstacle = Instantiate(obstaclePrefab);
            obstacle.SetActive(false);
            obstaclePool.Enqueue(obstacle);
        }
    }

    public void TrySpawnObstacles(Vector3 platformPosition, float platformWidth, PlatformManager.PlatformTile platformTile)
    {
        if (platformTile.isSafeStartTile)
        {
            Debug.Log("Skipping obstacle spawn: Safe start tile.");
            return;
        }
        if (Random.value > spawnChance)
        {
            Debug.Log("Skipping obstacle spawn: Random chance failed.");
            return;
        }
        if (PlatformHasNPCs(platformPosition, platformWidth))
        {
            Debug.Log("Skipping obstacle spawn: NPCs present.");
            return;
        }
        if (IsObstacleTooClose(platformPosition))
        {
            Debug.Log("Skipping obstacle spawn: Obstacle too close.");
            return;
        }

        Debug.Log("Spawning obstacle on tile at position: " + platformPosition);
        SpawnObstacles(platformPosition, platformWidth, platformTile);
    }

    private bool PlatformHasNPCs(Vector3 platformPosition, float platformWidth)
    {
        if (npcManager == null) return false;

        foreach (var npc in FindObjectsOfType<NPC>())
        {
            if (npc.gameObject.activeInHierarchy &&
                Mathf.Abs(npc.transform.position.x - platformPosition.x) < platformWidth / 2f)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsObstacleTooClose(Vector3 position)
    {
        foreach (var obstacle in FindObjectsOfType<Obstacle>())
        {
            if (obstacle.gameObject.activeInHierarchy &&
                Vector3.Distance(obstacle.transform.position, position) < minDistanceBetweenObstacles)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnObstacles(Vector3 platformPosition, float platformWidth, PlatformManager.PlatformTile platformTile)
    {
        if (platformTile.obstacleInstances.Count > 0) return;

        int obstacleCount = Random.Range(1, maxObstaclesPerPlatform + 1);
        float usableWidth = platformWidth - obstacleHorizontalPadding * 2f;

        TopReference topRef = platformTile.tileObject.GetComponentInChildren<TopReference>();
        if (topRef == null)
        {
            Debug.LogWarning("TopReference script not found on platform. Cannot spawn obstacle.");
            return;
        }
        Transform topTransform = topRef.transform;

        float obstacleHeight = obstaclePrefab.GetComponent<BoxCollider2D>().size.y * obstaclePrefab.transform.localScale.y;

        for (int i = 0; i < obstacleCount; i++)
        {
            if (obstaclePool.Count == 0) return;

            float offsetX = (-usableWidth / 2f) + (usableWidth * (i + 1) / (obstacleCount + 1));
            float spawnY = topTransform.position.y + (obstacleHeight / 2f);

            Vector3 worldSpawnPos = new Vector3(
                platformTile.tileObject.transform.position.x + offsetX,
                spawnY,
                0f
            );

            Vector3 localSpawnPos = platformTile.tileObject.transform.InverseTransformPoint(worldSpawnPos);

            GameObject obstacle = obstaclePool.Dequeue();
            obstacle.transform.SetParent(platformTile.tileObject.transform);
            obstacle.transform.localPosition = localSpawnPos;
            obstacle.SetActive(true);

            platformTile.obstacleInstances.Add(obstacle);
        }
    }

    public void ReturnToPoolDelayed(GameObject obstacle, float delay)
    {
        StartCoroutine(DelayedReturn(obstacle, delay));
    }

    private IEnumerator DelayedReturn(GameObject obstacle, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(obstacle);
    }

    public void ReturnToPool(GameObject obstacle)
    {
        obstacle.transform.SetParent(null);
        obstacle.SetActive(false);
        obstaclePool.Enqueue(obstacle);
    }
}