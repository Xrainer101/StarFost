using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerLaserPool : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
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

        if (Input.GetButton("Fire1"))
        {
            currentChargeTime += Time.deltaTime;

            if(currentChargeTime >= 0.25f)
            {
                shipChargeEffect.Play();
            }
            

            if(currentChargeTime >= chargeTimeRequired && !isFullyCharged)
            {
                isFullyCharged = true;
                shipChargeEffect.transform.localScale = new Vector3(2f, 2f, 2f);

                FindLockOnTarget();
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (isFullyCharged)
            {
                ShootChargeShot();
            }

            currentChargeTime = 0f;
            isFullyCharged = false;
            shipChargeEffect.Stop();
            shipChargeEffect.Clear(); // Wipes any remaining particles
            shipChargeEffect.transform.localScale = new Vector3(1f, 1f, 1f);
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
    }

    private void ShootChargeShot()
    {
        ShootFromPoint(centerShootPoint, 50, chargePool);

        // Todo: pass locked-on target to the projectile so it can curve toward it
    }

    private void ShootFromPoint(Transform laserPoint, int damage, ObjectPool<GameObject> poolToUse)
    {
        GameObject activeProj = poolToUse.Get();

        // Snap it to the gun barrel and point it forward
        activeProj.transform.position = laserPoint.position;
        activeProj.transform.rotation = laserPoint.rotation;

        LaserProjectile projScript = activeProj.GetComponent<LaserProjectile>();
        if(projScript != null)
        {
            projScript.damage = damage;
        }
    }

    private void FindLockOnTarget()
    {
        // Implemented later
        Debug.Log("Charging complete. Locking on...");
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
