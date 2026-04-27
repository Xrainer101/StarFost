using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Combat Position")]
    public float forwardOffset = 60f; // How far ahead of the player the boss starts

    public float combatHeight = 0f;
    public float combatX = 0f;

    public float catchUpSpeed = 5f; // How smoothly it adjusts if the player boosts/brakes

    [Header("Hover Animation")]
    public float hoverSpeed = 2f; // How fast it bobs up and down
    public float hoverHeight = 3f; // How high/low it bobs

    private Transform playerTarget;

    // Start is called before the first frame update
    void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) playerTarget = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTarget == null) return;

        // Calculate sine waves for ship bobbing
        float newY = combatHeight + (Mathf.Sin(Time.time * hoverSpeed) * hoverHeight);

        // Determine target ship position
        Vector3 targetPosition = new Vector3(combatX, newY, playerTarget.position.z + forwardOffset);

        // Glide to that position
        transform.position = Vector3.Lerp(transform.position, targetPosition, catchUpSpeed * Time.deltaTime);
    }
}
