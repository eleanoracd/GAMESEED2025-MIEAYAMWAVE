using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private bool isConverted = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isConverted) return;

        Cultist cultist = collision.gameObject.GetComponent<Cultist>();
        if (cultist != null)
        {
            ConvertToCultist();
        }
    }

    public void ConvertToCultist()
    {
        isConverted = true;

        NPCManager npcManager = FindObjectOfType<NPCManager>();
        if (npcManager != null)
        {
            npcManager.ReturnToPoolDelayed(this.gameObject, .1f);
        }

        gameObject.SetActive(false);


        CultistManager cultistManager = FindObjectOfType<CultistManager>();
        if (cultistManager != null)
        {
            GameObject newCultist = cultistManager.SpawnCultistBehindLast();
            if (newCultist != null)
            {
                Cultist cultistComponent = newCultist.GetComponent<Cultist>();
                if (cultistComponent != null && !cultistComponent.IsLeader())
                {
                }
            }
        }
    }

    public void ResetConversion()
    {
        isConverted = false;
    }
}

