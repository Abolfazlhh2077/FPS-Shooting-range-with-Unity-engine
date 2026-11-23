using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{

    public GameObject[] holePrefabs;
    public GameObject smokeEffect;
    private Rigidbody physicalBody;
    public wind wind;

    void Start()
    {
        physicalBody = GetComponent<Rigidbody>();
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector3 windForce = wind.getWind();
        physicalBody.AddForce(windForce, ForceMode.Force);

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Target"))
        {
            Destroy(gameObject);
            return;
        }


        int result = UnityEngine.Random.Range(0, 2);
        ContactPoint contact = collision.contacts[0];

        Quaternion rot = Quaternion.LookRotation(-contact.normal); // flip normal
        Vector3 pos = contact.point + contact.normal * 0.01f;      // push out more

        Quaternion smokeRot = Quaternion.LookRotation(contact.normal);

        GameObject hole = Instantiate(holePrefabs[result], pos, rot);
        GameObject smoke = Instantiate(smokeEffect, pos, smokeRot);

        // can do setparent to make the hole object stick to the target, it is needed for moving targets
        hole.transform.SetParent(collision.gameObject.transform, true);

        hole.transform.Rotate(0, 0, Random.Range(0f, 360f));

        Destroy(smoke, 2f);
        Destroy(gameObject);
    }

}
