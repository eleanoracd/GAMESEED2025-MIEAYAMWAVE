using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private GameObject[] platformPrefabs;
    [SerializeField] private int numberOfTiles = 10;
    [SerializeField] private float initialScrollSpeed = 5f;
    [SerializeField] private float tilesGap = 2f;
    [SerializeField] private float gapVariation = 0.5f;
    [SerializeField] private float minYOffset = -7f;
    [SerializeField] private float maxYOffset = -4f;

    [Header("Tiles Spawn Position")]
    [SerializeField] private float initialSpawnX = -10f;
    [SerializeField] private float startAreaYPosition = -6f;
    [SerializeField] private int guaranteedStartTiles = 3;
    [SerializeField] private GameObject player;

    [Header("Difficulty Progression")]
    [SerializeField] private float speedIncreaseInterval = 30f;
    [SerializeField] private float speedIncreaseAmount = 1f;
    [SerializeField] private float maxScrollSpeed = 15f;

    [Header("NPC Settings")]
    [SerializeField] private NPCManager npcManager;
    [SerializeField] private float npcSpawnChance = 0.5f;
    [SerializeField] private float npcYOffset = 1f;
    [SerializeField] private int maxNPCPerPlatform = 3;
    [SerializeField] private float npcHorizontalPadding = 0.5f;

    private float currentScrollSpeed;
    private float nextSpeedIncreaseTime;

    private List<PlatformTile> platformTiles = new List<PlatformTile>();
    private Dictionary<GameObject, Queue<GameObject>> platformPools = new Dictionary<GameObject, Queue<GameObject>>();

    private class PlatformTile
    {
        public GameObject tileObject;
        public float width;
        public bool isSafeStartTile;
        public List<GameObject> npcInstances = new List<GameObject>();

        public PlatformTile(GameObject obj, float w, bool safe = false)
        {
            tileObject = obj;
            width = w;
            isSafeStartTile = safe;
            npcInstances = new List<GameObject>();
        }
    }

    private void Start()
    {
        currentScrollSpeed = initialScrollSpeed;
        nextSpeedIncreaseTime = Time.time + speedIncreaseInterval;

        npcManager = FindObjectOfType<NPCManager>();
        InitializePools();

        float currentX = initialSpawnX;

        for (int i = 0; i < guaranteedStartTiles; i++)
        {
            SpawnTile(i, ref currentX, true);
        }

        for (int i = guaranteedStartTiles; i < numberOfTiles; i++)
        {
            SpawnTile(i, ref currentX);
        }

        if (player != null && platformTiles.Count > 0)
        {
            float playerX = platformTiles[0].tileObject.transform.position.x;
            player.transform.position = new Vector3(playerX, startAreaYPosition + 1f, 0);
        }
    }

    private void Update()
    {
        if (Time.time >= nextSpeedIncreaseTime)
        {
            IncreaseSpeed();
            nextSpeedIncreaseTime = Time.time + speedIncreaseInterval;
        }

        foreach (var tile in platformTiles)
        {
            tile.tileObject.transform.Translate(Vector2.left * currentScrollSpeed * Time.deltaTime);
        }

        PlatformTile firstTile = platformTiles[0];
        float cameraLeftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float rightEdgeOfFirstTile = firstTile.tileObject.transform.position.x + firstTile.width / 2f;

        if (rightEdgeOfFirstTile < cameraLeftEdge)
        {
            ReplaceOffscreenTile();
        }
    }

    private void InitializePools()
    {
        foreach (var prefab in platformPrefabs)
        {
            if (!platformPools.ContainsKey(prefab))
            {
                platformPools[prefab] = new Queue<GameObject>();

                for (int i = 0; i < numberOfTiles; i++)
                {
                    GameObject tile = Instantiate(prefab);
                    tile.SetActive(false);
                    platformPools[prefab].Enqueue(tile);
                }
            }
        }
    }

    private GameObject GetPooledTile(GameObject prefab)
    {
        Queue<GameObject> pool = platformPools[prefab];

        if (pool.Count == 0)
        {
            GameObject tile = Instantiate(prefab);
            tile.SetActive(false);
            return tile;
        }

        GameObject pooledTile = pool.Dequeue();
        pooledTile.SetActive(true);
        return pooledTile;
    }

    private void ReturnToPool(GameObject prefab, GameObject tile)
    {
        tile.SetActive(false);
        platformPools[prefab].Enqueue(tile);
    }

    private void SpawnTile(int index, ref float currentX, bool isSafeTile = false)
    {
        GameObject prefab = GetRandomPlatformPrefab();
        GameObject tile = GetPooledTile(prefab);

        float width = tile.GetComponent<SpriteRenderer>().bounds.size.x;
        float yOffset = isSafeTile ? startAreaYPosition : Random.Range(minYOffset, maxYOffset);

        if (index > 0)
        {
            float previousWidth = platformTiles[index - 1].width;
            float randomGapVariation = isSafeTile ? 0f : Random.Range(-gapVariation, gapVariation);
            float horizontalGap = previousWidth / 2f + width / 2f + tilesGap + randomGapVariation;
            currentX += horizontalGap;
        }

        tile.transform.position = new Vector3(currentX, yOffset, 0f);
        // platformTiles.Add(new PlatformTile(tile, width, isSafeTile));

        // if (!isSafeTile && Random.value < npcSpawnChance && npcManager != null)
        // {
        //     Vector3 npcPos = new Vector3(currentX, yOffset + npcYOffset, 0f);
        //     npcManager.SpawnNPC(npcPos);
        // }

        PlatformTile platformTile = new PlatformTile(tile, width, isSafeTile);

        if (!isSafeTile && Random.value < npcSpawnChance && npcManager != null)
        {
            int npcCount = Random.Range(1, maxNPCPerPlatform + 1);
            float usableWidth = width - npcHorizontalPadding * 2f;

            for (int i = 0; i < npcCount; i++)
            {
                float offsetX = (-usableWidth / 2f) + (usableWidth * (i + 1) / (npcCount + 1));
                Vector3 npcPos = new Vector3(currentX + offsetX, yOffset + npcYOffset, 0f);

                GameObject npc = npcManager.SpawnNPC(npcPos);

                if (npc != null)
                {
                    npc.transform.parent = tile.transform;
                    platformTile.npcInstances.Add(npc);
                }
            }
        }

        platformTiles.Add(platformTile);
    }

    private void ReplaceOffscreenTile()
    {
        PlatformTile firstTile = platformTiles[0];
        PlatformTile lastTile = platformTiles[platformTiles.Count - 1];

        if (firstTile.npcInstances != null && npcManager != null)
        {
            foreach (var npc in firstTile.npcInstances)
            {
                npcManager.ReturnToPool(npc);
            }
            firstTile.npcInstances.Clear();
        }

        GameObject prefab = FindPrefabByInstance(firstTile.tileObject);
        ReturnToPool(prefab, firstTile.tileObject);

        GameObject newPrefab = GetRandomPlatformPrefab();
        GameObject newTile = GetPooledTile(newPrefab);

        float newWidth = newTile.GetComponent<SpriteRenderer>().bounds.size.x;
        float yOffset = Random.Range(minYOffset, maxYOffset);

        float lastTileRightEdge = lastTile.tileObject.transform.position.x + lastTile.width / 2f;
        float randomGapVariation = Random.Range(-gapVariation, gapVariation);
        float newTileLeftEdge = lastTileRightEdge + tilesGap + randomGapVariation;
        float newTileX = newTileLeftEdge + newWidth / 2f;

        newTile.transform.position = new Vector3(newTileX, yOffset, 0f);

        PlatformTile newPlatformTile = new PlatformTile(newTile, newWidth);

        if (Random.value < npcSpawnChance && npcManager != null)
        {
            int npcCount = Random.Range(1, maxNPCPerPlatform + 1);
            float usableWidth = newWidth - npcHorizontalPadding * 2f;

            for (int i = 0; i < npcCount; i++)
            {
                float offsetX = (-usableWidth / 2f) + (usableWidth * (i + 1) / (npcCount + 1));
                Vector3 npcPos = new Vector3(newTileX + offsetX, yOffset + npcYOffset, 0f);

                GameObject npc = npcManager.SpawnNPC(npcPos);

                if (npc != null)
                {
                    npc.transform.parent = newTile.transform;
                    newPlatformTile.npcInstances.Add(npc);
                }
            }
        }

        platformTiles.RemoveAt(0);
        platformTiles.Add(new PlatformTile(newTile, newWidth));
    }

    private GameObject FindPrefabByInstance(GameObject instance)
    {
        foreach (var kvp in platformPools)
        {
            if (instance.name.Contains(kvp.Key.name))
                return kvp.Key;
        }

        Debug.LogWarning("Prefab not matched for " + instance.name);
        return platformPrefabs[0];
    }

    private void IncreaseSpeed()
    {
        currentScrollSpeed = Mathf.Min(currentScrollSpeed + speedIncreaseAmount, maxScrollSpeed);
        Debug.Log($"Speed increased to: {currentScrollSpeed}");
    }

    private GameObject GetRandomPlatformPrefab()
    {
        return platformPrefabs[Random.Range(0, platformPrefabs.Length)];
    }
}
