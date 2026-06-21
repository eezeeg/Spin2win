using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLose : MonoBehaviour
{
    [Header("Level ID")]
    [SerializeField] private int levelId = 1;

    [Header("Player Respawn")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform spawnPoint;

    [Header("WinScreen & script")]
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private WinScreenScript winScript;

    [Header("Completion timer")]
    [SerializeField] private float timer;

    [Header("Settings")]
    [SerializeField] private bool loadNextLevelOnWin = true;
    [SerializeField] private bool restartLevelOnDeath = false;

    [Header("Delays")]
    [SerializeField] private float winDelay = 0.5f;
    [SerializeField] private float deathDelay = 0.5f;

    [Header("Movement Reset")]
    [SerializeField] private BasicMovement basicMovement;

    private bool gameEnded;
    private float startTime;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            basicMovement = playerObject.GetComponent<BasicMovement>();
        }

        startTime = Time.time;

        if (spawnPoint == null && player != null)
        {
            GameObject autoSpawn = new GameObject("Auto Spawn Point");
            autoSpawn.transform.position = player.position;
            autoSpawn.transform.rotation = player.rotation;
            spawnPoint = autoSpawn.transform;
        }
    }

    private void Update()
    {
        if (!gameEnded)
        {
            timer = Time.time - startTime;
        }
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
        Debug.Log("completion time: " + timer.ToString("00.00"));

        if (loadNextLevelOnWin)
        {
            LevelsBeatSave.SaveLevelComplete(levelId, timer);

            float best = LevelsBeatSave.GetBestTime(levelId);

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
        else
        {
            Invoke(nameof(RespawnPlayer), deathDelay);
        }
    }

    private void RespawnPlayer()
    {
        if (player == null || spawnPoint == null)
            return;

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        player.position = spawnPoint.position;
        player.rotation = spawnPoint.rotation;

        if (basicMovement != null)
        {
            basicMovement.ResetRotation();
        }

        gameEnded = false;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}