using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambientSound : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource ambientSource;
    public AudioClip[] ambientClip;
    
    
    void Start()
    {
        int choice = Random.Range(0, 3);
        ambientSource.clip = ambientClip[choice];
        ambientSource.loop = true;
        ambientSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
