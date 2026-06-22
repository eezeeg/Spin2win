using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public static bool isLevelSelect;
    public static int selectedPage = 0;

    [Header("Second Camera")]
    [SerializeField] private Camera SecondCam;
    [SerializeField] private float camMoveDur = 1f;

    private Coroutine viewportCoroutine;

    [Header("Level select pages")]
    [SerializeField] private List<GameObject> levelPageList;

    [Header("Panels")]
    [SerializeField] private GameObject panelMain;

    [Header("Buttons")]
    [SerializeField] private List<GameObject> levelList;

    [Header("Camera")]
    [SerializeField] private GameObject CameraToHide;

    [Header("Star Sprites")]
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    private void Awake()
    {
        if (isLevelSelect)
        {
            PlayGame();
        }
    }

    private void Start()
    {
        RefreshMenu();
    }

    public void pageUp(bool isNext)
    {
        levelPageList[selectedPage].SetActive(false);
        selectedPage += isNext ? 1 : -1;
        levelPageList[selectedPage].SetActive(true);
    }

    public void PlayGame()
    {
        panelMain.SetActive(false);
        levelPageList[selectedPage].SetActive(true);
        CameraToHide.SetActive(false);
        isLevelSelect = true;
    }

    public void GoAccesories()
    {
        if (viewportCoroutine != null)
        {
            StopCoroutine(viewportCoroutine);
        }

        viewportCoroutine = StartCoroutine(LerpCameraViewport(SecondCam, 0f));
    }

    private IEnumerator LerpCameraViewport(Camera cam, float targetx)
    {
        Rect startRect = cam.rect;
        Rect targetRect = cam.rect;
        targetRect.x = targetx;
        float elapsed = 0f;

        while (elapsed < camMoveDur)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / camMoveDur;
            t = Mathf.SmoothStep(0f, 1f, t);

            Rect newRect = cam.rect;
            newRect.x = Mathf.Lerp(startRect.x, targetRect.x, t);
            cam.rect = newRect;

            yield return null;
        }

        cam.rect = targetRect;
        viewportCoroutine = null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadLevel(int levelId)
    {
        string level = "Level_" + levelId;
        StartCoroutine(LoadLevel(level));
    }

    private IEnumerator LoadLevel(string levelName)
    {
        TransitionManager.Instance.LoadSceneWithFade(levelName);
        yield break;
    }

    public void ReturnMain()
    {
        panelMain.SetActive(true);
        levelPageList[selectedPage].SetActive(false);
        CameraToHide.SetActive(true);
        isLevelSelect = false;
    }

    private void RefreshMenu()
    {
        int totalComplete = 0;
        int levelId = 1;

        foreach (GameObject level in levelList)
        {
            TMP_Text timer = level.transform.Find("CompleteTime").GetComponent<TMP_Text>();

            Transform starsParent = level.transform.Find("LvlImage1/Stars");

            GameObject buttonObject = level.transform.Find("BtnLvl").gameObject;
            Button button = buttonObject.GetComponent<Button>();
            TMP_Text btnText = buttonObject.GetComponentInChildren<TMP_Text>();

            timer.text = "--:--.--";
            button.interactable = true;
            btnText.text = "Level " + levelId;

            SetLevelStars(starsParent, 0);

            if (LevelsBeatSave.IsLevelComplete(levelId))
            {
                totalComplete++;

                float time = LevelsBeatSave.GetBestTime(levelId);
                int stars = LevelsBeatSave.GetStars(levelId);

                timer.text = FormatTime(time);
                SetLevelStars(starsParent, stars);
            }

            if (levelId > 3)
            {
                if (totalComplete + 3 < levelId)
                {
                    button.interactable = false;
                    btnText.text = "locked";
                    SetLevelStars(starsParent, 0);
                }
            }

            levelId++;
        }
    }

    private void SetLevelStars(Transform starsParent, int starCount)
    {
        if (starsParent == null)
        {
            return;
        }

        starCount = Mathf.Clamp(starCount, 0, 3);

        for (int i = 0; i < starsParent.childCount; i++)
        {
            Image starImage = starsParent.GetChild(i).GetComponent<Image>();

            if (starImage == null)
            {
                continue;
            }

            starImage.sprite = i < starCount ? filledStar : emptyStar;
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100f);

        return minutes.ToString("00") + ":" +
               seconds.ToString("00") + "." +
               milliseconds.ToString("00");
    }

    public void DeleteData()
    {
        LevelsBeatSave.DeleteEVERYTHING();
        RefreshMenu();
    }
}