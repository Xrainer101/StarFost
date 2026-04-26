using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public Slider healthSlider;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        healthSlider.value = currentHealth;
        Debug.Log("Player took damage! Health: " + currentHealth);

        // (Optional: You could trigger a camera shake or red UI flash here!)

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("GAME OVER!");
        // We can add actual Game Over UI and explosions later. 
        // For now, we will just disable the ship.
        gameObject.SetActive(false); 
    }
}
