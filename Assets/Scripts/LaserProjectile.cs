using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LaserProjectile : MonoBehaviour
{
    [Header("Laser Settings")]
    public float speed = 100f;
    public float lifeTime = 2f;
    public int damage = 10;

    [Header("Impact Settings")]
    public GameObject impactPrefab;
    [Range(0.1f, 2f)]
    public float impactScale = 0.3f;

    private float lifeTimer;

    // Reference to the pool that owns this laser
    private IObjectPool<GameObject> myPool;

    // Method to assign the pool when the laser is created
    public void SetPool(IObjectPool<GameObject> pool)
    {
        myPool = pool;
    }

    // OnEnable is called the exact moment the object is turned on by the pool
    void OnEnable()
    {
        lifeTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        lifeTimer += Time.deltaTime;
        if(lifeTimer >= lifeTime)
        {
            Deactivate();
        }
    }

    // Call this when it hits an enemy or when the lifetime runs out
    public void Deactivate()
    {
        // Release the laser for the pool to retrieve
        if(myPool != null)
        {
            myPool.Release(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // The laser moves through the player
        if (other.CompareTag("Player"))
        {
            return;
        }

        // Impact explosion for visual and audio feedback
        if(impactPrefab != null)
        {
            // Get the closest point on the surface of whatever the laser collided with
            Vector3 surfacePoint = other.ClosestPoint(transform.position);

            // Get a point slightly outside the collider
            Vector3 impactPoint = surfacePoint - (transform.forward * 0.5f);


            GameObject impact = Instantiate(impactPrefab, impactPoint, transform.rotation);
            impact.GetComponent<AudioSource>().volume = 0.3f;
            impact.transform.localScale *= impactScale;
        }

        // Hitting an enemy
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        // Hitting a target in the tutorial
        if(other.CompareTag("Target"))
        {
            Destroy(other.gameObject);
        }

        Deactivate(); // Return to pool on impact
    }
}
