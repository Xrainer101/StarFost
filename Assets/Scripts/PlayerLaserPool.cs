using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerLaserPool : MonoBehaviour
{
    [Header("References")]
    public GameObject laserPrefab;
    public Transform shootPoint;

    [Header("Shooting Stats")]
    public float fireRate = 0.15f;
    private float fireTimer;

    private ObjectPool<GameObject> laserPool;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the pool
        laserPool = new ObjectPool<GameObject>(
            createFunc: CreateLaser,          // 1. How to make a new laser
            actionOnGet: TakeLaserFromPool,   // 2. What to do when we borrow one
            actionOnRelease: ReturnLaserToPool, // 3. What to do when it's returned
            actionOnDestroy: DestroyLaser,    // 4. What to do if the pool gets too full
            collectionCheck: false,
            defaultCapacity: 30, // Start by making 30 lasers
            maxSize: 100         // Never make more than 100
        );

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
        if(Input.GetKey(KeyCode.Mouse0) && fireTimer >= fireRate)
        {
            Shoot(); // Shoot
            fireTimer = 0f; // Reset the timer
        }
    }

    private void Shoot()
    {
        GameObject activeLaaser = laserPool.Get();

        // Snap it to the gun barrel and point it forward
        activeLaaser.transform.position = shootPoint.position;
        activeLaaser.transform.rotation = shootPoint.rotation;
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

    private void TakeLaserFromPool(GameObject laser)
    {
        // Turn it on when the player shoots
        laser.SetActive(true);
    }

    private void ReturnLaserToPool(GameObject laser)
    {
        // Make sure it's turned off when it comes back
        laser.SetActive(false);
    }

    private void DestroyLaser(GameObject laser)
    {
        // Completely delete it from memory if the pool overflows
        Destroy(laser);
    }
}
