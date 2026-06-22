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

    [Header("Star Times")]
    [SerializeField] private float threeStarTime = 30f;
    [SerializeField] private float twoStarTime = 60f;

    [Header("Settings")]
    [SerializeField] private bool loadNextLevelOnWin = true;
    [SerializeField] private bool restartLevelOnDeath = false;

    [Header("Delays")]
    [SerializeField] private float winDelay = 0.5f;
    [SerializeField] private float deathDelay = 0.5f;

    [Header("Movement Reset")]
    [SerializeField] private BasicMovement basicMovement;

    [Header("Bubble Door Reset")]
    [SerializeField] private bool resetBubbleDoorsOnDeath = true;
    [SerializeField] private BubbleDoor[] bubbleDoors;

    private bool gameEnded;
    private float startTime;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PauseMenu.PauseLocked = false;
        if (playerObject != null)
        {
            player = playerObject.transform;
            basicMovement = playerObject.GetComponent<BasicMovement>();
        }

        if (resetBubbleDoorsOnDeath)
        {
            FindBubbleDoors();
        }

        startTime = Time.time;

        if (winCanvas != null)
        {
            winCanvas.gameObject.SetActive(false);
        }

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

        int starsEarned = CalculateStars(timer);

        Debug.Log("You Win!");
        Debug.Log("Completion time: " + timer.ToString("00.00"));
        Debug.Log("Stars earned: " + starsEarned);

        LevelsBeatSave.SaveLevelComplete(levelId, timer, starsEarned);

        float best = LevelsBeatSave.GetBestTime(levelId);
        int bestStars = LevelsBeatSave.GetStars(levelId);

        PauseMenu.PauseLocked = true;

        if (winScript != null)
        {
            winScript.UpdateBoxes(timer, best, starsEarned, bestStars);
        }

        if (winCanvas != null)
        {
            winCanvas.gameObject.SetActive(true);
        }
    }

    private int CalculateStars(float completionTime)
    {
        if (completionTime <= threeStarTime)
        {
            return 3;
        }

        if (completionTime <= twoStarTime)
        {
            return 2;
        }

        return 1;
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

        ResetBubbleDoors();
        PauseMenu.PauseLocked = false;
        gameEnded = false;
    }

    private void ResetBubbleDoors()
    {
        if (!resetBubbleDoorsOnDeath)
            return;

        if (bubbleDoors == null || bubbleDoors.Length == 0)
        {
            FindBubbleDoors();
        }

        for (int i = 0; i < bubbleDoors.Length; i++)
        {
            if (bubbleDoors[i] != null)
            {
                bubbleDoors[i].ResetDoor();
            }
        }
    }

    private void FindBubbleDoors()
    {
        bubbleDoors = FindObjectsOfType<BubbleDoor>(true);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PauseMenu.PauseLocked = false;
    }
}
