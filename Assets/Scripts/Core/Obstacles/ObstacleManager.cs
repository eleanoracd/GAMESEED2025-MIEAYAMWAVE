using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> obstaclePool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obstacle = Instantiate(obstaclePrefab);
            obstacle.SetActive(false);
            obstaclePool.Enqueue(obstacle);
        }
    }

    public GameObject Spawnobstacle(Vector3 spawnPosition)
    {
        if (obstaclePool.Count == 0) return null;

        GameObject obstacle = obstaclePool.Dequeue();
        obstacle.transform.position = spawnPosition;
        obstacle.SetActive(true);
        return obstacle;
    }

    public void ReturnToPool(GameObject obstacle)
    {
        obstacle.SetActive(false);
        obstaclePool.Enqueue(obstacle);
    }
}
