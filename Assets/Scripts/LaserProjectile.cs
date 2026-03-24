using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LaserProjectile : MonoBehaviour
{
    [Header("Laser Settings")]
    public float speed = 100f;
    public float lifeTime = 2f;
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
        if (other.CompareTag("Enemy")) // Need to implement Enemy tag onto enemies later
        {
            // other.GetComponent<Health>().TakeDamage(10)
            Deactivate(); // Return to pool on impact
        }
    }
}
