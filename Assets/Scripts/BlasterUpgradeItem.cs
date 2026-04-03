using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterUpgradeItem : MonoBehaviour
{
    [Header("Visuals")]
    public float spinSpeed = 90f; // Degrees per second
    public ParticleSystem collectEffectsPrefab;

    // Update is called once per frame
    void Update()
    {
        // Makes item spin
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player touched the item,
        if (other.CompareTag("Player"))
        {
            // Get the player's laser script
            PlayerLaserPool playerLaserPool = other.GetComponentInParent<PlayerLaserPool>();

            // If we do,
            if(playerLaserPool != null)
            {
                // Upgrade
                playerLaserPool.UpgradeBlaster();

                // Particle effect
                ParticleSystem collectEffect = Instantiate(collectEffectsPrefab); // Instantiate so it keeps going after the item is destroyed
                collectEffect.Play();

                // Destroy the item
                Destroy(gameObject);
            }
        }
    }
}
