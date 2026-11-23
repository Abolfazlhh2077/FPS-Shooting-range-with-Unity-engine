using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class texasStar : MonoBehaviour
{


    int currentPlateNumber = 5;
    public challengeHandler challengeHandler;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void plateUpdate()
    {
        currentPlateNumber--;
        if (currentPlateNumber <= 0)
        {
            challengeHandler.removeCurrentTarget(this.gameObject);
        }

    }


    public void connect_challenger_handler(challengeHandler handler)
    {

        this.challengeHandler = handler;
        Debug.Log("Handler connected!", challengeHandler);
    }



}
