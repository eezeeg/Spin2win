using TMPro;
using UnityEngine;

public class CollectionStatsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text tiesText;
    [SerializeField] private TMP_Text starsText;

    [Header("Settings")]
    [SerializeField] private int totalLevels = 10;
    [SerializeField] private int starsPerLevel = 3;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int completedLevels = 0;
        int collectedStars = 0;

        for (int levelId = 1; levelId <= totalLevels; levelId++)
        {
            if (LevelsBeatSave.IsLevelComplete(levelId))
            {
                completedLevels++;
            }

            collectedStars += LevelsBeatSave.GetStars(levelId);
        }

        if (tiesText != null)
        {
            tiesText.text = completedLevels + "/" + totalLevels;
        }

        if (starsText != null)
        {
            int maxStars = totalLevels * starsPerLevel;
            starsText.text = collectedStars + "/" + maxStars;
        }
    }
}