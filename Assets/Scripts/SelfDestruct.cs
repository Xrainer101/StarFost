using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float lifetime = 1f;

    void Start()
    {
        // Automatically deletes the GameObject from the scene after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }
}
