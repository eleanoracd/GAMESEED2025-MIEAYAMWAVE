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
    }
}
