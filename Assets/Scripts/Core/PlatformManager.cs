using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private GameObject[] platformPrefabs;
    [SerializeField] private int numberOfTiles;
    [SerializeField] private float initialScrollSpeed = 5f;
    [SerializeField] private float tilesGap = 2f;
    [SerializeField] private float gapVariation = 0.5f;
    [SerializeField] private float minYOffset = -1f;
    [SerializeField] private float maxYOffset = 1f;

    [Header("Tiles Spawn Position")]
    [SerializeField] private float initialSpawnX = -10f;
    [SerializeField] private float startAreaYPosition = -6f;
    [SerializeField] private int guaranteedStartTiles = 3;
    [SerializeField] private GameObject player;

    [Header("Difficulty Progression")]
    [SerializeField] private float speedIncreaseInterval = 30f;
    [SerializeField] private float speedIncreaseAmount = 1f;
    [SerializeField] private float maxScrollSpeed = 15f;
    private float currentScrollSpeed;
    private float nextSpeedIncreaseTime;
    private List<PlatformTile> platformTiles = new List<PlatformTile>();

    private class PlatformTile
    {
        public GameObject tileObject;
        public float width;
        public bool isSafeStartTile;

        public PlatformTile(GameObject obj, float w, bool safe = false)
        {
            tileObject = obj;
            width = w;
            isSafeStartTile = safe;
        }
    }

    private void Start()
    {
        currentScrollSpeed = initialScrollSpeed;
        nextSpeedIncreaseTime = Time.time + speedIncreaseInterval;

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

    private void SpawnTile(int index, ref float currentX, bool isSafeTile = false)
    {
        GameObject prefab = GetRandomPlatformPrefab();
        GameObject tile = Instantiate(prefab);

        float width = tile.GetComponent<SpriteRenderer>().bounds.size.x;
        float yOffset = isSafeTile ? startAreaYPosition : Random.Range(minYOffset, maxYOffset);

        if (index > 0)
        {
            float previousWidth = platformTiles[index - 1].width;
            float randomGapVariation = isSafeTile ? startAreaYPosition : Random.Range(-gapVariation, gapVariation);
            float horizontalGap = previousWidth / 2f + width / 2f + tilesGap + randomGapVariation;
            currentX += horizontalGap;
        }

        tile.transform.position = new Vector3(currentX, yOffset, 0f);
        platformTiles.Add(new PlatformTile(tile, width, isSafeTile));
    }

    private void ReplaceOffscreenTile()
    {
        PlatformTile firstTile = platformTiles[0];
        PlatformTile lastTile = platformTiles[platformTiles.Count - 1];

        Destroy(firstTile.tileObject);

        GameObject newPrefab = GetRandomPlatformPrefab();
        GameObject newTile = Instantiate(newPrefab);
        float newWidth = newTile.GetComponent<SpriteRenderer>().bounds.size.x;
        float yOffset = Random.Range(minYOffset, maxYOffset);

        float lastTileRightEdge = lastTile.tileObject.transform.position.x + lastTile.width / 2f;
        float randomGapVariation = Random.Range(-gapVariation, gapVariation);
        float newTileLeftEdge = lastTileRightEdge + tilesGap + randomGapVariation;
        float newTileX = newTileLeftEdge + newWidth / 2f;

        newTile.transform.position = new Vector3(newTileX, yOffset, 0f);

        platformTiles.RemoveAt(0);
        platformTiles.Add(new PlatformTile(newTile, newWidth));
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