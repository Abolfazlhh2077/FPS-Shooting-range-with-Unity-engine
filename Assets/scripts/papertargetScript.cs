using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class papertargetScript : MonoBehaviour
{

    public AudioSource soundSource;
    public AudioClip[] hitsounds;
    private Vector3 soundDestination = new Vector3(783.840027f,1.12f,300.640015f);
    private bool hit = false;
    public challengeHandler challengeHandler;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            transform.Rotate(40 * Time.deltaTime, 0, 0);

            if (transform.eulerAngles.x >= 90f)
            {
                transform.eulerAngles = new Vector3(90f, transform.eulerAngles.y, transform.eulerAngles.z);
                challengeHandler.removeCurrentTarget(this.gameObject);
                hit = false;
            }
        }
    }

    public void connect_challenger_handler(challengeHandler handler)
    {
        
        this.challengeHandler = handler;
        Debug.Log("Handler connected!", challengeHandler);
    }


    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(DelayedHitSound(Vector3.Distance(transform.position, soundDestination)/350f));
        hit = true;
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
