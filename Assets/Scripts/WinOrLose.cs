using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLose : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool loadNextLevelOnWin = true;
    [SerializeField] private bool restartLevelOnDeath = true;

    [Header("Delays")]
    [SerializeField] private float winDelay = 0.5f;
    [SerializeField] private float deathDelay = 0.5f;

    private bool gameEnded;

    private void OnTriggerEnter(Collider other)
    {
        if (gameEnded)
            return;

        if (other.CompareTag("Goal"))
        {
            Win();
        }
        else if (other.CompareTag("Hazard"))
        {
            Lose();
        }
    }

    private void Win()
    {
        gameEnded = true;

        Debug.Log("You Win!");

        if (loadNextLevelOnWin)
        {
            Invoke(nameof(LoadNextLevel), winDelay);
        }
    }

    private void Lose()
    {
        gameEnded = true;

        Debug.Log("You Died!");

        if (restartLevelOnDeath)
        {
            Invoke(nameof(RestartLevel), deathDelay);
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No next level found.");
        }
    }
}