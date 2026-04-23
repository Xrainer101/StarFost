using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelTrigger : MonoBehaviour
{
    [SerializeField] private int sceneNumber;
    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        SceneManager.LoadScene(sceneNumber);
    }
}
