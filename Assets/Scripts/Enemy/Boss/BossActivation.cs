using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivation : MonoBehaviour
{
    [Header("The Boss")]
    [Tooltip("Drag your disabled BossRoot here")]
    public GameObject bossRoot;

    public AudioSource bossAlarmSound;

    private void OnTriggerEnter(Collider other)
    {
        // If player crosses the line
        if (other.CompareTag("Player"))
        {
            // Activate the boss
            if(bossRoot != null)
            {
                bossRoot.SetActive(true);

                if(bossAlarmSound != null) bossAlarmSound.Play();
            }

            // Destroy this trigger
            Destroy(gameObject);
        }
    }
}
