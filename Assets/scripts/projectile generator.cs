using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectilegenerator : MonoBehaviour
{

    public GameObject projectile;
    public GameObject shootingPivot;
    public GameObject muzzleFlash;
    private Vector3 shootingDirection;
    private Vector3 shootingForce;
    public float shootingPower = 350f;
    private float projectileSize = 0.03f;


    void Start()
    {
        
    }


    void Update()
    {
        
    }


    public void generate_projectile()
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, transform.rotation);
        newProjectile.transform.localScale = new Vector3(1f,1f,1f) * projectileSize;
        newProjectile.SetActive(true);

        GameObject newMuzzle = Instantiate(muzzleFlash, transform.position, transform.rotation);
        newMuzzle.transform.SetParent(transform);

        shootingDirection = transform.position - shootingPivot.transform.position;
        shootingDirection = shootingDirection.normalized;
        shootingForce = shootingPower * shootingDirection;
        
        Rigidbody body = newProjectile.GetComponent<Rigidbody>();
        body.AddForce(shootingForce, ForceMode.Impulse);

        Destroy(newMuzzle, 2f);
    
    }



}
