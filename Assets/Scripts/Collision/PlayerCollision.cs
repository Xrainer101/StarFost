using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerCollision : MonoBehaviour
{
    PlayerStateMachine ctx;
    float hitCooldown = 0.5f;
    float lastHitTime;

    void Awake()
    {
        ctx = GetComponent<PlayerStateMachine>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastHitTime < hitCooldown) return;

        if (other.CompareTag("Obstacle"))
        {
            lastHitTime = Time.time;
            Debug.Log("Obstacle Collision");

            ctx.ApplyKnockback(other.transform.position);
        }
    }
}
