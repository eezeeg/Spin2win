using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    // Anything can set this to true to disable pausing
    public static bool PauseLocked { get; set; }

    [Header("References")]
    [SerializeField] private GameObject pauseMenuUI;

    private void Start()
    {
        IsPaused = false;
        PauseLocked = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Update()
    {
        if (PauseLocked)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (PauseLocked)
            return;

        IsPaused = true;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        AudioListener.pause = true;

    }

    public void ResumeGame()
    {
        IsPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

    }

    public void OpenSettings()
    {
        Debug.Log("Open Settings");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        IsPaused = false;
        PauseLocked = false;

        SceneManager.LoadScene("MainMenu");
    }
}