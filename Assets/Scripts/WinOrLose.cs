using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLose : MonoBehaviour
{
    [Header("Level ID")]
    [SerializeField] private int levelId = 1;

    [Header("WinScreen & script")] 
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private WinScreenScript winScript;

    [Header("Completion timer")]
    [SerializeField] private float timer;

    [Header("Settings")]
    [SerializeField] private bool loadNextLevelOnWin = true;
    [SerializeField] private bool restartLevelOnDeath = true;

    [Header("Delays")]
    [SerializeField] private float winDelay = 0.5f;
    [SerializeField] private float deathDelay = 0.5f;

    private bool gameEnded;

    private float startTime;
    private void Start()
    {
        startTime = Time.time;
    }

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

        timer = Time.time - startTime;

        Debug.Log("You Win!");
        Debug.Log("completion time:" + timer.ToString("00.00"));



        if (loadNextLevelOnWin)
        {
            //Invoke(nameof(LoadNextLevel), winDelay);

            //saves level completion info and gets best time
            LevelsBeatSave.SaveLevelComplete(levelId, timer);
            float best = LevelsBeatSave.GetBestTime(levelId);
            //update boxes on canvas before activating it
            winScript.UpdateBoxes(timer, best);
            winCanvas.gameObject.SetActive(true);
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

}