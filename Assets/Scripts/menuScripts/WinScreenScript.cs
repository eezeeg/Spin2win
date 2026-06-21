using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenScript : MonoBehaviour
{
    [Header("textBoxes")]
    [SerializeField] TMP_Text CompletionBox;
    [SerializeField] TMP_Text BestBox;
    public void ReturnLevelSelect()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateBoxes(float completion, float best)
    {
        CompletionBox.text = FormatTime(completion);
        BestBox.text = FormatTime(best);
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
