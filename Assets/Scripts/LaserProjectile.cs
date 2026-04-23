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

    [Header("Homing Settings")]
    public Transform homingTarget;
    public float turnSpeed = 10f;

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
        homingTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Homing stuff
        if (homingTarget != null)
        {
            // If the enemy is destroyed, homingTarget becomes null automatically.
            // If the enemy is disabled, set to null manually
            if (!homingTarget.gameObject.activeInHierarchy)
            {
                homingTarget = null;
            }
            else
            {
                // Calculate the direction to the target
                Vector3 direction = (homingTarget.position - transform.position).normalized;
                
                // Calculate the rotation needed to look at that direction
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                // Smoothly rotate toward that angle
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        // All projectiles programming
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
        if (other.CompareTag("Player") || other.CompareTag("Collectable"))
        {
            return;
        }

        // Impact explosion for visual and audio feedback
        if(impactPrefab != null)
        {
            // Variables for spawning explosion
            Vector3 pushDir = -transform.forward;
            Vector3 surfacePoint;

            // Shoot a ray from 2 meters behind the laser to hit the surface of the building
            Vector3 rayOrigin = transform.position - (transform.forward * 2f);

            // If we hit something (ignoring the laser's own trigger collider)
            if(Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, 3f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                // Save the spot on the wall
                surfacePoint = hit.point;
                // Get the normal of the wall
                pushDir = hit.normal;
            } else
            {
                // If not, just get the closest point on the wall the laser collided with
                surfacePoint = other.ClosestPoint(transform.position);
            }

            // Get a point pushed slightly outside the collider
            Vector3 impactPoint = surfacePoint + (pushDir * 0.2f);

            // Spawnt he explosion
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
