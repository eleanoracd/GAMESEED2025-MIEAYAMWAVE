using System.Collections;
using UnityEngine;

public class NonPlayableCharacter : MonoBehaviour, INPC
{
    [SerializeField] private float conversionDelay = 0.3f;
    [SerializeField] private ParticleSystem conversionEffect;

    public void ConvertToCultist(CultistType type)
    {
        StartCoroutine(ConversionRoutine(type));
    }

    private IEnumerator ConversionRoutine(CultistType type)
    {
        GetComponent<Collider2D>().enabled = false;
        if (conversionEffect != null) conversionEffect.Play();
        
        yield return new WaitForSeconds(conversionDelay);

        CultistManager.Instance.SpawnCultist(type, transform.position, transform.rotation);
        CultistManager.Instance.ReturnCultist(GetComponent<CultistLogic>());
    }
}
