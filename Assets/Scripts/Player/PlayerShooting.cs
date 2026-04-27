using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerShooting : MonoBehaviour
{
    [Header("Upgrade State")]
    [Range(0,2)] // Adds a slider in the Unity editor for the below variable
    public int blasterUpgrade = 0; // 0 = No upgrade, 1 = Twin blasters, 2 = Green lasers

    [Header("References")]
    public Transform centerShootPoint;
    public Transform leftShootPoint;
    public Transform rightShootPoint;

    [Header("Normal Lasers")]
    public GameObject laserPrefab;
    public float fireRate = 0.1f;
    private float fireTimer;
    private ObjectPool<GameObject> laserPool;

    [Header("Charge Shot")]
    public GameObject chargeShotPrefab;
    public ParticleSystem shipChargeEffect;
    public float chargeTimeRequired = 1f;

    private float currentChargeTime = 0f;
    private bool isFullyCharged = false;
    private ObjectPool<GameObject> chargePool;

    [Header("Lock-On Settings")]
    public float lockOnRadius = 15f; // How thick the scanning cylinder is
    public float lockOnDistance = 200f; // How far forward it scans
    public RectTransform lockOnReticle;
    public float lockBreakAngle = 25f; // How off-center the enemy can get before the lock breaks
    private Transform currentTarget;
    private Camera mainCam;

    [Header("Audio")]
    public AudioSource blasterAudioSource;
    public AudioClip laserShootSound;
    public AudioClip chargeShootSound; // Bigger boom

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;

        // Initialize the pool
        laserPool = new ObjectPool<GameObject>(
            createFunc: CreateLaser,          // 1. How to make a new laser
            actionOnGet: TakeFromPool,   // 2. What to do when we borrow one
            actionOnRelease: ReturnToPool, // 3. What to do when it's returned
            actionOnDestroy: DestroyObj,    // 4. What to do if the pool gets too full
            collectionCheck: false,
            defaultCapacity: 30, // Start by making 30 lasers
            maxSize: 100         // Never make more than 100
        );

        chargePool = new ObjectPool<GameObject>(CreateCharge, TakeFromPool, ReturnToPool, DestroyObj, false, 5, 10);

        GameObject[] prewarmArray = new GameObject[30];

        // Spawn 30 lasers immediately
        for(int i = 0; i < 30; i++)
        {
            prewarmArray[i] = laserPool.Get();
        }

        // Retrieve them to put them in the pool
        for(int i = 0; i < 30; i++)
        {
            laserPool.Release(prewarmArray[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;

        // If left mouse button is pressed down and the firerate has refreshed
        if(Input.GetButtonDown("Fire1"))
        {
            if(fireTimer >= fireRate) {
                ShootNormal(); // Shoot
                fireTimer = 0f; // Reset the timer
            }

            // Reset charge whenever you press the button
            currentChargeTime = 0f;
            isFullyCharged = false;
        }

        // While holding the button
        if (Input.GetButton("Fire1"))
        {
            // Increase charge time
            currentChargeTime += Time.deltaTime;

            // If held down long enough, the charging effect plays
            if(currentChargeTime >= 0.25f && !shipChargeEffect.isPlaying)
            {
                shipChargeEffect.Play();
            }
            
            // If held down longer, the shot is fully charged
            if(currentChargeTime >= chargeTimeRequired)
            {
                // If we aren't fully charged already,
                if(!isFullyCharged) {
                    isFullyCharged = true;  // set the boolean
                    shipChargeEffect.transform.localScale = new Vector3(2f, 2f, 2f); // make charge effect bigger
                }

                // If no locked-on target,
                if(currentTarget == null)
                {
                    FindLockOnTarget(); // Get the target
                } else // If we have a target
                {
                    // Get the direction from the ship's noise to the target
                    Vector3 directionToTarget = currentTarget.position - centerShootPoint.position;

                    // Get the angle of where the ship's facing and the direction to the target
                    float angleToTarget = Vector3.Angle(centerShootPoint.forward, directionToTarget);

                    // If our aim is too off-center,
                    if(angleToTarget > lockBreakAngle)
                    {
                        // Set target to null
                        currentTarget = null;

                        // UI will automatically disappear because of code in LateUpdate
                    }
                }
            }
        }

        // Releasing the button
        if (Input.GetButtonUp("Fire1"))
        {
            if (isFullyCharged)
            {
                ShootChargeShot();
            }

            // Cleanup
            currentChargeTime = 0f;
            isFullyCharged = false;
            shipChargeEffect.Stop();
            shipChargeEffect.Clear(); // Wipes any remaining particles
            shipChargeEffect.transform.localScale = new Vector3(1f, 1f, 1f);

            currentTarget = null;
            if(lockOnReticle != null) lockOnReticle.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (currentTarget != null && lockOnReticle != null)
        {
            // If the enemy is destroyed before we shoot, hide the UI
            if (!currentTarget.gameObject.activeInHierarchy)
            {
                lockOnReticle.gameObject.SetActive(false);
                currentTarget = null;
                return;
            }

            // Logic for lock-on reticle position
            Vector3 screenPos = mainCam.WorldToScreenPoint(currentTarget.position);
            if (screenPos.z > 0)
            {
                lockOnReticle.position = screenPos;
                if (!lockOnReticle.gameObject.activeSelf) lockOnReticle.gameObject.SetActive(true);
            }
            else
            {
                lockOnReticle.gameObject.SetActive(false);
            }
        }
        // Turn off reticle if target becomes null
        else if (lockOnReticle != null && lockOnReticle.gameObject.activeSelf)
        {
            lockOnReticle.gameObject.SetActive(false);
        }
    }

    private void ShootNormal()
    {
        int currDamage = (blasterUpgrade == 2) ? 20 : 10;

        if(blasterUpgrade == 0)
        {
            ShootFromPoint(centerShootPoint, currDamage, laserPool);
        } else
        {
            ShootFromPoint(leftShootPoint, currDamage, laserPool);
            ShootFromPoint(rightShootPoint, currDamage, laserPool);
        }

        if(blasterAudioSource != null && laserShootSound != null)
        {
            blasterAudioSource.PlayOneShot(laserShootSound);
        }
    }

    private void ShootChargeShot()
    {
        ShootFromPoint(centerShootPoint, 50, chargePool, currentTarget);

        if(blasterAudioSource != null && chargeShootSound != null)
        {
            blasterAudioSource.PlayOneShot(chargeShootSound);
        }
    }

    private void ShootFromPoint(Transform laserPoint, int damage, ObjectPool<GameObject> poolToUse, Transform target = null)
    {
        GameObject activeProj = poolToUse.Get();

        // Snap it to the gun barrel and point it forward
        activeProj.transform.position = laserPoint.position;
        activeProj.transform.rotation = laserPoint.rotation;

        LaserProjectile projScript = activeProj.GetComponent<LaserProjectile>();
        if(projScript != null)
        {
            projScript.damage = damage;

            if(target != null)
            {
                projScript.homingTarget = currentTarget;
            }
        }
    }

        private void FindLockOnTarget()
    {
        // Shoot a massive invisible cylinder forward
        RaycastHit[] hits = Physics.SphereCastAll(centerShootPoint.position, lockOnRadius, centerShootPoint.forward, lockOnDistance);
        
        float closestDistance = Mathf.Infinity;

        // Loop through everything we hit
        foreach (RaycastHit hit in hits)
        {
            // Did we catch an enemy?
            if (hit.collider.CompareTag("Enemy"))
            {
                // Find out how far away it is
                float distanceToEnemy = Vector3.Distance(centerShootPoint.position, hit.transform.position);
                
                // If this is the closest one we've found so far, save it!
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    currentTarget = hit.transform;
                }
            }
        }
    }

    public void UpgradeBlaster()
    {
        if(blasterUpgrade < 2)
        {
            blasterUpgrade++;
            Debug.Log("Blaster upgraded to level " + blasterUpgrade);
        }
    }

    // -- POOL METHODS --
    private GameObject CreateLaser()
    {
        // Instantiate the laser and assign it to the player's pool, but keep it turned off by default
        GameObject laser = Instantiate(laserPrefab);

        laser.GetComponent<LaserProjectile>().SetPool(laserPool);

        laser.SetActive(false);
        return laser;
    }

    private GameObject CreateCharge()
    {
        GameObject chargeShot = Instantiate(chargeShotPrefab);

        chargeShot.GetComponent<LaserProjectile>().SetPool(chargePool);
        chargeShot.SetActive(false);
        return chargeShot;
    }

    private void TakeFromPool(GameObject laser)
    {
        // Turn it on when the player shoots
        laser.SetActive(true);
    }

    private void ReturnToPool(GameObject laser)
    {
        // Make sure it's turned off when it comes back
        laser.SetActive(false);
    }

    private void DestroyObj(GameObject laser)
    {
        // Completely delete it from memory if the pool overflows
        Destroy(laser);
    }
}
