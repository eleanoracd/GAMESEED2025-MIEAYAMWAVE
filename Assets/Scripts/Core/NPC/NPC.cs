using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private bool isConverted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isConverted) return;

        if (other.CompareTag("Zombie"))
        {
            // ConvertToZombie();
        }
    }

    // public void ConvertToZombie()
    // {
    //     isConverted = true;

    //     // Option A: Disable NPC and spawn a Zombie
    //     gameObject.SetActive(false);
    //     ZombieManager.Instance.SpawnZombie(transform.position); // You’ll make this in step 4

    //     // Optionally: return to NPC pool after a short delay
    //     StartCoroutine(ReturnToPoolDelayed(1f));
    // }

    // private IEnumerator ReturnToPoolDelayed(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     isConverted = false;
    //     NPCManager npcManager = FindObjectOfType<NPCManager>();
    //     npcManager.ReturnToPool(this.gameObject);
    // }
}

