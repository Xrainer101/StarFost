using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class LoadEnemyTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        loadEnemies();
    }
    private void loadEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(true);
        }
    }
}
