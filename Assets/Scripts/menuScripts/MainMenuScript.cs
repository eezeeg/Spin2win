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
