using UnityEngine;

public class CultistDespawner : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Cultist cultist = collision.gameObject.GetComponent<Cultist>();
        if (cultist != null)
        {
            CultistManager manager = FindObjectOfType<CultistManager>();
            if (manager != null)
            {
                manager.ReturnToPool(cultist.gameObject);

                if (cultist.IsLeader())
                {
                    manager.HandleLeaderDespawned();
                }
            }
        }

        Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
        {
            if (obstacle != null)
            {
                ObstacleManager manager = FindObjectOfType<ObstacleManager>();
                if (manager != null)
                {
                    manager.ReturnToPool(obstacle.gameObject);
                }
            }
        }

        NPC npc = collision.gameObject.GetComponent<NPC>();
        {
            if (npc != null)
            {
                NPCManager manager = FindObjectOfType<NPCManager>();
                if (manager != null)
                {
                    manager.ReturnToPool(npc.gameObject);
                }
            }
        }
    }
}
