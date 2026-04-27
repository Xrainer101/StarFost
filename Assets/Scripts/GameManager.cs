using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int lives = 3;
    public static GameManager gameManager;
    DialogueManager dM;
    [SerializeField] GameObject player;

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
}
