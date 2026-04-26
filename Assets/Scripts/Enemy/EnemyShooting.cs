using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject enemyLaserPrefab;
    public Transform shootPoint;
    public GameObject alertSphere; // For visual clarity
    
    [Header("Stats")]
    public float fireRate = 2f; // Shoots every 2 seconds
    public float attackRange = 150f; // Max shooting distance
    public float fieldOfView = 60f; // Angle limit it can turn
    private float fireTimer;

    private Transform playerTarget;
    private Vector3 restingForward;

    void Start()
    {
        restingForward = transform.forward;
        alertSphere.SetActive(false);

        // Automatically find the player when the enemy spawns
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        // If the player is dead or missing, stop running this code
        if (playerTarget == null || !playerTarget.gameObject.activeInHierarchy) return;

        // Find player position
        Vector3 directionToPlayer = playerTarget.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Find player angle relative to original orientation
        float angleToPlayer = Vector3.Angle(restingForward, directionToPlayer);

        // If player is in a valid aiming position,
        if(distanceToPlayer <= attackRange && angleToPlayer <= fieldOfView)
        {
            // Activate alertSphere
            alertSphere.SetActive(true);

            // Aim: Instantly look directly at the player
            transform.LookAt(playerTarget);

            // Shoot: Count up the timer and fire
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                Shoot();
                fireTimer = 0f;
            }
        } else // If not (Player out of range or behind enemy)
        {
            // Deactivate alertSphere
            alertSphere.SetActive(false);

            // Go back to looking straight
            Quaternion restingRotation = Quaternion.LookRotation(restingForward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, restingRotation, 100f * Time.deltaTime);

            // Reset fire timer so it doesn't instantly fire when you enter its range
            fireTimer = 0f;
        }   
    }

    private void Shoot()
    {
        // Spawn the universal laser. Because we set the tags in the prefab, 
        // it knows to ignore other enemies and strictly hunt the player!
        Instantiate(enemyLaserPrefab, shootPoint.position, shootPoint.rotation);
    }
}
