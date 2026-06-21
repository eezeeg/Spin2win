using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("Fade")]
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionTime = 1f;

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Fade in when the game first starts
        PlayFadeIn();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Fade in after loading a new scene
        PlayFadeIn();
    }

    private void PlayFadeIn()
    {
        animator.ResetTrigger("Start");
        animator.ResetTrigger("End");

        animator.Play("Crossfade_End", 0, 0f);
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (isTransitioning) return;

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isTransitioning = true;

        animator.ResetTrigger("End");
        animator.ResetTrigger("Start");

        animator.Play("Crossfade_Start", 0, 0f);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);

        isTransitioning = false;
    }
}