using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    [SerializeField]GameObject menuPanel;
    [SerializeField] GameObject levelsPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] AudioSource audioSource;

    public void ShowMenu()
    {
        audioSource.Play();
        menuPanel.SetActive(true);
        levelsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    public void ShowLevels()
    {
        audioSource.Play();
        menuPanel.SetActive(false);
        levelsPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
    public void ShowSettings()
    {
        audioSource.Play();
        menuPanel.SetActive(false);
        levelsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void LoadScene(int level)
    {
        audioSource.Play();
        StartCoroutine(LoadSceneDelayed(level));
    }
    IEnumerator LoadSceneDelayed(int level)
    {
        yield return new WaitForSeconds(0.2f);
        if(level < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level);
        else Debug.LogError("Level " + level + " not found in build settings.");
    }
}