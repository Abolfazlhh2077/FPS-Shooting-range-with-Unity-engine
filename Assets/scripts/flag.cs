using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flag : MonoBehaviour
{


    public Cloth cloth;
    public wind wind;
    private float Amplitude = 5;
    private float Timer = 0.9f;
    

    // Start is called before the first frame update
    void Start()
    {
        cloth = GetComponent<Cloth>();
    }


    private void FixedUpdate()
    {
        Timer += Time.deltaTime;
        if (Timer > 1f) { 
            Timer = 0.9f;
        }

        Vector3 windForce = wind.getWind();
        
        cloth.externalAcceleration = windForce * Amplitude * Timer;
    }


    // Update is called once per frame
    void Update()
    {
       
    }
}
