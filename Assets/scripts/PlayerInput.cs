using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{


    
    // movement variables
    Vector3 movementVector;
    public float movementSpeed = 3f;
    public CharacterController controller;

    // reload variables
    public int totalAmmo = 20;
    private int neededAmmo;
    public int clipCapacity = 7;
    public int clipRounds = 7;
    private bool isReloading = false;
    private bool reloadLerp = false;
    private float reloadingTime = 4f;
    private float reloadTimer = 0;

    // gun, camera and shell setup
    public shellgenerator shellgenerator;
    public projectilegenerator projectilegenerator;
    public Transform myCamera;
    public Camera myCameraComponent;
    public Transform myGun;
    public Camera scopeCamera;
    public Material scopeMaterial;

    // mouse look variables
    public float mouseSensitivity = 175f;
    float xRotation = 0f;

    // aiming variables
    // iron sight aims: Y=0.135f | Y=0.115f , scope sight aim: Y=0.065f.
    public float aimSpeed = 5f;
    private Vector3 startGunLocalPos;
    private Vector3 aimGunLocalPos;
    private Vector3 aimSightLocalPos;
    private Vector3 startToTargetTransition = new Vector3(-0.154f, 0.062f, -0.2f);
    private bool isAiming;
    private bool aimBySight = false;
    private float normalFov = 55;
    private float aimingFov = 27;
    private float currentFov;
    // scope zoom variables
    public float scrollZoomSpeed = 0.1f;
    public float minScopeFov = 2f;
    public float maxScopeFov = 9f;
    public float zoomInput = 0;
    private float targetScopeFov;


    // shooting variables
    private bool isShooting = false;
    public float fireRate = 0.2f;
    private float shootingTimer = 0;
    private int shootingAudioIndex = 0;
    public AudioSource gunSound;
    public AudioClip[] singleShotClips;
    public AudioClip[] reloadClip;
    public AudioClip emptyClick;
    public Animator akAnimator;

    // all recoil variables
    private float recoilProgress;
    private bool applyRecoil;
    public float recoilSpeed = 12f;
    // kick back recoil variables
    public float kickBackAmount = 0.1f;
    private Vector3 kickBackStart;
    private Vector3 kickBackTarget;
    // x y recoil variables
    public float kickUpAmount = 0.05f;
    private float currentRecoilX = 0f;
    private float targetRecoilX = 0f;
    private float currentRecoilY = 0f;
    private float targetRecoilY = 0f;
    public float minYrecoilRange = -0.3f;
    public float maxYrecoilRange = 0.3f;
    private float recoilVelocityX = 0f;
    private float recoilVelocityY = 0f;
    public float recoilSmoothness = 0.005f;

    // shoot sway variables
    public float[] ShootZsway = {70, 120, 100};
    public float[] ShootYsway = { 150, 200 };
    public float[] ShootXsway = { -15, 15 };
    public Vector3 ShootSway = Vector3.zero;
    public Vector3 currentSway = Vector3.zero;

    challengeHandler challengeHandler;




    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        startGunLocalPos = myGun.localPosition;
        aimGunLocalPos = startGunLocalPos + startToTargetTransition;
        aimSightLocalPos = startGunLocalPos + new Vector3(-0.154f, 0.135f, -0.1f);
        challengeHandler = FindObjectOfType<challengeHandler>();
    }

    void Start()
    {
        myCameraComponent.fieldOfView = normalFov;
        currentFov = myCameraComponent.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        isAiming = false;
        akAnimator.enabled = false;
    }

    void Update()
    {

        handleRestart();
        handleReload();
        handleAim();
        handleShoot();
        handleMovement();
        handleMouse();

    }


    void handleRestart()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void handleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementVector = (transform.forward * vertical + transform.right * horizontal).normalized;
        controller.Move(movementVector * movementSpeed * Time.deltaTime);
    }


    void handleMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        myCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }


    void handleAim()
    {

        
        if (Input.GetKeyDown(KeyCode.Mouse1) && !reloadLerp && !isReloading)
        {
            isAiming = !isAiming;
        }


        // Note: makes the gun return to the default position after the shoot knockback which is needed as a feature right now.
        if (isAiming)
        {

            if (aimBySight)
            {
                myGun.localPosition = Vector3.Lerp(myGun.localPosition, aimSightLocalPos, aimSpeed * Time.deltaTime);
            } else {
                
                myGun.localPosition = Vector3.Lerp(myGun.localPosition, aimGunLocalPos, aimSpeed * Time.deltaTime);
            }

            currentFov = Mathf.Lerp(currentFov, aimingFov, aimSpeed * Time.deltaTime);
            myCameraComponent.fieldOfView = currentFov;


            /* scope zoom handling
            if (Input.GetKey(KeyCode.Q))
            {
                if (scopeCamera.fieldOfView < 9) {
                    scopeCamera.fieldOfView += 5 * Time.deltaTime;
                }
            } else if (Input.GetKey(KeyCode.E)) {
                if (scopeCamera.fieldOfView > 2) {
                    scopeCamera.fieldOfView -= 5 * Time.deltaTime;
                }
            } */

            //
            targetScopeFov -= Input.mouseScrollDelta.y * scrollZoomSpeed;
            targetScopeFov = Mathf.Clamp(targetScopeFov, minScopeFov, maxScopeFov);
            scopeCamera.fieldOfView = Mathf.Lerp(scopeCamera.fieldOfView, targetScopeFov, 8f * Time.deltaTime);

            //

            float reticleZoom = 9f / scopeCamera.fieldOfView;
            scopeMaterial.SetFloat("_reticleZoom", reticleZoom);

            if (Input.GetKeyDown(KeyCode.X))
            {
                aimBySight = !aimBySight;
            }


        } else {
            currentFov = Mathf.Lerp(currentFov, normalFov, aimSpeed * Time.deltaTime);
            myCameraComponent.fieldOfView = currentFov;
            myGun.localPosition = Vector3.Lerp(myGun.localPosition, startGunLocalPos, aimSpeed * Time.deltaTime);
        }


    }


    void handleShoot()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isShooting && !isReloading)
        {

            if (clipRounds == 0)
            {
                gunSound.pitch = UnityEngine.Random.Range(0.92f, 1.08f);
                gunSound.volume = UnityEngine.Random.Range(0.02f, 0.04f);
                gunSound.PlayOneShot(emptyClick);

            } else if (clipRounds > 0)
            {

                // initialize shooting variables
                isShooting = true;
                clipRounds--;
                if (clipRounds == 0 && totalAmmo == 0) {
                    StartCoroutine(challengeHandler.check_lose());
                }

                // apply sound, shell and projectile ejection
                gunSound.pitch = 1f;
                gunSound.volume = 1f;
                gunSound.clip = singleShotClips[shootingAudioIndex];
                shootingAudioIndex++;
                if (shootingAudioIndex >= singleShotClips.Length)
                {
                    shootingAudioIndex = 0;
                }
                gunSound.Play();
                shellgenerator.generate_shell();
                projectilegenerator.generate_projectile();

                // setting up kickback recoil variables
                kickBackStart = myGun.localPosition;
                kickBackTarget = kickBackStart + new Vector3(0, 0, -kickBackAmount);
                recoilProgress = 0f;
                applyRecoil = true;

                // setting up x y recoil variables
                targetRecoilX = kickUpAmount;
                targetRecoilY = UnityEngine.Random.Range(minYrecoilRange, maxYrecoilRange);

                // setting up the ShootSway variable
                ShootSway.z = ShootZsway[UnityEngine.Random.Range(0, ShootZsway.Length)];
                ShootSway.y = ShootYsway[UnityEngine.Random.Range(0, ShootYsway.Length)];
                ShootSway.x = ShootXsway[UnityEngine.Random.Range(0, ShootXsway.Length)];

            }



        }


        if (isShooting)
        {
            // applying recoil here
            if (applyRecoil)
            {
                // applying kickback recoil
                myGun.localPosition = Vector3.Lerp(kickBackStart, kickBackTarget, recoilProgress);

                // applying the x y recoil
                currentRecoilX = Mathf.SmoothDamp(currentRecoilX, targetRecoilX, ref recoilVelocityX, recoilSmoothness);
                currentRecoilY = Mathf.SmoothDamp(currentRecoilY, targetRecoilY, ref recoilVelocityY, recoilSmoothness);
                xRotation -= currentRecoilX;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                myCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * currentRecoilY * Time.deltaTime * 60);

                // applying the shoot sway
                currentSway = Vector3.Lerp(currentSway, ShootSway, recoilSpeed);
                myGun.Rotate(currentSway * Time.deltaTime);


                // timing the entire recoil
                recoilProgress += recoilSpeed * Time.deltaTime;
                if (recoilProgress >= 1f)
                {
                    currentSway = Vector3.zero;
                    currentRecoilX = 0;
                    currentRecoilY = 0;
                    applyRecoil = false;
                }
            }

            // timing the fires
            shootingTimer += Time.deltaTime;
            if (shootingTimer > fireRate)
            {
                isShooting = false;
                shootingTimer = 0f;
            }

        }

        if (!applyRecoil && !isReloading) {
            // aligning the gun after shoot sway
            Quaternion targetRotation = myCamera.rotation * Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, -90f, 0);
            myGun.rotation = Quaternion.Lerp(myGun.rotation, targetRotation, 5f * Time.deltaTime);
        }

        

    }


    void handleReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && totalAmmo != 0) {
            if (Vector3.Distance(myGun.localPosition, startGunLocalPos) > 0.01f)
            {
                reloadLerp = true;
                isAiming = false;
                aimBySight = false;
            } else
            {
                isReloading = true;
                gunSound.PlayOneShot(reloadClip[0]);
                akAnimator.enabled = true;
                akAnimator.SetTrigger("reload");
            }
        }

        if (reloadLerp)
        {
            currentFov = Mathf.Lerp(currentFov, normalFov, aimSpeed * Time.deltaTime);
            myCameraComponent.fieldOfView = currentFov;
            myGun.localPosition = Vector3.Lerp(myGun.localPosition, startGunLocalPos, aimSpeed * Time.deltaTime);

            if (Vector3.Distance(myGun.localPosition, startGunLocalPos) < 0.01f)
            {
                myGun.localPosition = startGunLocalPos;
                reloadLerp = false;
                isReloading = true;
                gunSound.PlayOneShot(reloadClip[0]);
                akAnimator.enabled = true;
                akAnimator.SetTrigger("reload");
            }
        }

        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > reloadingTime)
            {
                neededAmmo = clipCapacity - clipRounds;
                if (totalAmmo >= neededAmmo)
                {
                    clipRounds += neededAmmo;
                    totalAmmo -= neededAmmo;
                }
                else
                {
                    clipRounds += totalAmmo;
                    totalAmmo = 0;
                }
                reloadTimer = 0f;
                akAnimator.enabled = false;
                isReloading = false;
            }
        }

    }


    public void give_ammo(int number)
    {
        clipRounds = 0;
        totalAmmo = number;

        // setting to reload
        if (Vector3.Distance(myGun.localPosition, startGunLocalPos) > 0.01f)
        {
            reloadLerp = true;
            isAiming = false;
            aimBySight = false;
        }
        else
        {
            isReloading = true;
            gunSound.PlayOneShot(reloadClip[0]);
            akAnimator.enabled = true;
            akAnimator.SetTrigger("reload");
        }
    }



}




