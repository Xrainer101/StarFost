using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int lives = 3;
    public static GameManager gameManager;
    DialogueManager dM;
    [SerializeField] GameObject player;

    [Header("UI References")]
    public GameObject winScreenPanel;

    [Header("Audio References")]
    public MusicManager musicManager;

    void Awake()
    {
        //single instance
        if (gameManager == null) gameManager = this;
    }
    void Start()
    {
        dM = DialogueManager.dialogueManager;
    }

    public void PlayerDie()
    {
        if (lives > 0) StartCoroutine(Revive());
        else StartCoroutine(ReloadScene());
    }
    IEnumerator Revive()
    {
        lives--;
        dM.setLives(lives);
        yield return new WaitForSeconds(0.5f);
        player.SetActive(true);
    }
    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(0.5f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ShowWinScreen()
    {
        // Turn on UI
        if(winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
        }

        // Pause everything
        Time.timeScale = 0f;

        if(musicManager != null)
        {
            musicManager.PlayVictoryMusic();
        }
        Debug.Log("I WON!");
    }

    public void LoadMainMenu(string menuSceneName)
    {
        // Unpause game
        Time.timeScale = 1f;

        // Load menu scene
        SceneManager.LoadScene(menuSceneName);
    }
}
