using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texasstarPlate : MonoBehaviour
{

    public AudioSource soundSource;
    public AudioClip[] hitsounds;
    private Vector3 soundDestination = new Vector3(783.840027f, 1.12f, 300.640015f);
    public texasStar wholeTarget;

    // Start is called before the first frame update
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(DelayedHitSound(Vector3.Distance(transform.position, soundDestination) / 350f));
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        wholeTarget.plateUpdate();
    }

    private IEnumerator DelayedHitSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayHitSound();
    }

    private void PlayHitSound()
    {
        soundSource.clip = hitsounds[Random.Range(0, 4)];
        soundSource.Play();
   
    }



}
