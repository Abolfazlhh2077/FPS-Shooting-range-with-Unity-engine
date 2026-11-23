using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundscript : MonoBehaviour
{
    
    // sound settings
    public AudioClip[] Sounds;
    public AudioSource impactSound;
   
    // destruction timer settings
    private float timer = 0;
    public float maxTime = 10f;

    private bool collided = false;

    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > maxTime)
        {
            Destroy(gameObject);
        }

    }


    private void OnCollisionEnter(Collision collision)
    {

        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Grass") && !collided)
        {
            collided = true;
            int randomValue = Random.Range(0, 7);
            impactSound.clip = Sounds[randomValue];
            impactSound.Play();
            collided = true;
        }

        
    }

}
