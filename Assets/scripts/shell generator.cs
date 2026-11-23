using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class shellgenerator : MonoBehaviour
{


    public GameObject shell;
    public float forceCoefficient = 0.9f;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    public void generate_shell()
    {

        GameObject newShell = Instantiate(shell, transform.position, transform.rotation);
        newShell.transform.Rotate(270f, 0, 0);
        newShell.SetActive(true);

        Rigidbody body = newShell.GetComponent<Rigidbody>();
        if (body != null )
        {
            Vector3 force = transform.right * (-1);
            force.Normalize();
            Vector3 offset = new Vector3(UnityEngine.Random.Range(2, 5) * 0.1f, 0, 0);
            body.AddForce(forceCoefficient * force + offset, ForceMode.Impulse);
        }

    }


}
