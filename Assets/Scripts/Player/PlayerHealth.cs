using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthSlider;

    [Header("Invincibility")]
    public float invincibilityDuration = 1f;
    public float flickerSpeed = 0.1f; // How fast the ship blinks on and off

    // Array in case ship ever becomes multiple pieces
    public MeshRenderer[] shipRenderers;

    private bool isInvincible = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;

        // Fallback if renderers are not assigned in inspector
        if (shipRenderers.Length == 0)
        {
            shipRenderers = GetComponentsInChildren<MeshRenderer>();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if(isInvincible) return;

        currentHealth -= damageAmount;
        healthSlider.value = currentHealth;
        Debug.Log("Player took damage! Health: " + currentHealth);

        // (Optional: You could trigger a camera shake or red UI flash here!)

        if (currentHealth <= 0)
        {
            Die();
        } else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        float timer = 0f;

        while(timer < invincibilityDuration)
        {   
            // Toggle visual pieces of ship on and off
            foreach(MeshRenderer renderer in shipRenderers)
            {
                renderer.enabled = !renderer.enabled;
            }

            // Wait a bit before looping again
            yield return new WaitForSeconds(flickerSpeed);

            // Add to our timer
            timer += flickerSpeed;
        }

        // Make sure all renderers are on
        foreach(MeshRenderer renderer in shipRenderers)
        {
            renderer.enabled = true;
        }

        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("GAME OVER!");
        // Add actual Game Over UI and explosions later.
        gameObject.SetActive(false); 
    }
}
