using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Subpart of the boss: Weakpoint and chunk pair
[System.Serializable]
public class BossPart
{
    public string partName; // Organization
    public GameObject weakPoint; // BasicEnemy
    public GameObject armorChunk; // Cosmetic

    [HideInInspector]
    public bool isDetached = false; // Only detach once
}

public class BossManager : MonoBehaviour
{
    [Header("Boss Parts")]
    // Add as many parts as you want
    public BossPart[] bossParts;
    public GameObject rootPart;

    [Header("Effects")]
    public GameObject finalExplosionPrefab;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        int destroyedPartsCount = 0;

        // Check status of each boss part
        foreach(BossPart part in bossParts)
        {
            if(part.weakPoint == null) {
                // If part is not detached and weak point has been destroyed
                if(!part.isDetached && part.weakPoint == null)
                {
                    part.isDetached = true;
                    DetachChunk(part.armorChunk);
                }

                // Add to broken parts
                destroyedPartsCount++;
            }
        }

        // If broken parts >= total parts, die
        if(destroyedPartsCount >= bossParts.Length)
        {
            Die();
        }
    }

    private void DetachChunk(GameObject chunk)
    {
        if(chunk == null) return;

        chunk.transform.SetParent(null);

        // Add rigidbody so the part falls
        Rigidbody rb = chunk.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        // Push part away
        rb.AddExplosionForce(500f, chunk.transform.position + transform.forward, 10f);
        rb.AddTorque(new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f)));

        Destroy(chunk, 5f);
    }

    private void Die()
    {
        // Set boss to dead
        isDead = true;

        DetachChunk(rootPart);

        // Start death routine
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        // Big boom
        if(finalExplosionPrefab != null)
        {
            GameObject bigBoom = Instantiate(finalExplosionPrefab, transform.position, transform.rotation);

            bigBoom.transform.localScale = new Vector3(50f, 50f, 50f);
        }

        // Turn off boss colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(Collider col in colliders)
        {
            col.enabled = false;
        }

        // Wait for epic death to stop
        yield return new WaitForSeconds(3f);

        // Show win screen
        GameManager.gameManager.ShowWinScreen();

        // Destroy the boss
        Destroy(gameObject);
    }
}
