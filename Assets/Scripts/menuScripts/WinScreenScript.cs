using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenScript : MonoBehaviour
{
    [Header("Text Boxes")]
    [SerializeField] private TMP_Text CompletionBox;
    [SerializeField] private TMP_Text BestBox;

    [Header("Current Stars")]
    [SerializeField] private Image[] stars;

    [Header("Best Stars Optional")]
    [SerializeField] private Image[] bestStars;

    [Header("Star Sprites")]
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    public void ReturnLevelSelect()
    {
        TransitionManager.Instance.LoadSceneWithFade("MainMenu");
    }

    public void UpdateBoxes(float completion, float best, int starsEarned, int bestStarsEarned)
    {
        if (CompletionBox != null)
        {
            CompletionBox.text = FormatTime(completion);
        }

        if (BestBox != null)
        {
            BestBox.text = FormatTime(best);
        }

        SetStars(stars, starsEarned);
        SetStars(bestStars, bestStarsEarned);
    }

    private void SetStars(Image[] starImages, int starCount)
    {
        if (starImages == null)
            return;

        starCount = Mathf.Clamp(starCount, 0, 3);

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null)
                continue;

            starImages[i].sprite = i < starCount ? filledStar : emptyStar;
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
}
