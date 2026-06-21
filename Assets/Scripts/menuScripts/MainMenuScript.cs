using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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
    [Header("buttons")]
    [SerializeField] private List<GameObject> levelList;
    [Header("camera")]
    [SerializeField] private GameObject CameraToHide;



    private void Awake()
    {
        if (isLevelSelect)
        {
            PlayGame();
        }
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
        //panelSelect.SetActive(true);
        CameraToHide.SetActive(false);
        isLevelSelect = true;
    }

    public void GoAccesories()
    {
        if(viewportCoroutine != null)
            StopCoroutine(viewportCoroutine);

        viewportCoroutine = StartCoroutine(LerpCameraViewport(SecondCam, 0f));
    }

    IEnumerator LerpCameraViewport(Camera cam, float targetx)
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

    IEnumerator LoadLevel(string levelName)
    {
        TransitionManager.Instance.LoadSceneWithFade(levelName);
        yield break;
    }
    public void ReturnMain()
    {
        panelMain.SetActive(true);
        levelPageList[selectedPage].SetActive(false);
        //panelSelect.SetActive(false);
        CameraToHide.SetActive(true);
        isLevelSelect = false;
    }

    private void Start()
    {
        RefreshMenu();
    }
    


    //helper for timer format
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100f);

        return minutes.ToString("00") + ":" + 
               seconds.ToString("00") + "." + 
               milliseconds.ToString("00");
    }


    private void RefreshMenu()
    {
        int totalComplete = 0;
        int levelId = 1;
        foreach (GameObject level in levelList)
        {
            TMP_Text timer = level.transform.Find("CompleteTime").GetComponent<TMP_Text>();
            GameObject buttonObject = level.transform.Find("BtnLvl").gameObject;
            Button button = buttonObject.GetComponent<Button>();
            TMP_Text btnText = buttonObject.GetComponentInChildren<TMP_Text>();


            //def values
            timer.text = "--:--.--";
            button.interactable = true;
            btnText.text = "Level " + levelId;

            if (LevelsBeatSave.IsLevelComplete(levelId))
            {
                totalComplete++;
                
                float time = LevelsBeatSave.GetBestTime(levelId);
                timer.text = FormatTime(time);
            }
            if (levelId > 3)
            {
                //disable if not enough levels complete
                if (totalComplete + 3 < levelId)
                {
                    button.interactable = false;
                    btnText.text = "locked";
                }
            }
            levelId++;
        }
    }
    ///TESTING SCRIPT TO DELETE PLAYER DATA
    public void DeleteData()
    {
        LevelsBeatSave.DeleteEVERYTHING();
        RefreshMenu();
    }

}
