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

        // Hitting an enemy
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        Deactivate(); // Return to pool on impact
    }
}
