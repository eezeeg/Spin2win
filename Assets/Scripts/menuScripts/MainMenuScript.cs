using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelSelect;
    [SerializeField] private List<GameObject> levelList;
    public void PlayGame()
    {
        panelMain.SetActive(false);
        panelSelect.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadLevel(int levelId)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level_" + levelId);
    }
    public void ReturnMain()
    {
        panelMain.SetActive(true);
        panelSelect.SetActive(false);
    }

    private void Start()
    {
        int totalComplete = 0;
        int levelId = 1;
        foreach (GameObject level in levelList)
        {
            if (LevelsBeatSave.IsLevelComplete(levelId))
            {
                totalComplete++;
                TMP_Text timer = level.transform.Find("CompleteTime").GetComponent<TMP_Text>();
                float time = LevelsBeatSave.GetBestTime(levelId);
                timer.text = FormatTime(time);
            }
            if (levelId > 3)
            {
                
                //disable if not enough levels complete
                if (totalComplete + 3 < levelId)
                {
                    GameObject buttonObject = level.transform.Find("BtnLvl").gameObject;
                    Button button = buttonObject.GetComponent<Button>();
                    button.interactable = false;

                    TMP_Text btnText = button.transform.Find("text").GetComponent<TMP_Text>();
                    btnText.text = "locked";

                }
            }

            levelId++;
        }


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
}
