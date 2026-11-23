using System.Collections;
using UnityEngine;

public class ParticleWarmup : MonoBehaviour
{
    public GameObject[] prefabs;

    IEnumerator Start()
    {
        foreach (GameObject prefab in prefabs)
        {
            GameObject instance = Instantiate(prefab);

            
            yield return new WaitForSeconds(0.02f);

            Destroy(instance);

            yield return null;
        }

        Debug.Log("Particle warm-up complete.");
    }
}