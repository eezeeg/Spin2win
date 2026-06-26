using TMPro;
using UnityEngine;

public class CollectionStatsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text tiesText;
    [SerializeField] private TMP_Text starsText;

    [Header("Settings")]
    [SerializeField] private int totalLevels = 12;
    [SerializeField] private int starsPerLevel = 3;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int collectedTies = 0;
        int collectedStars = 0;

        for (int levelId = 1; levelId <= totalLevels; levelId++)
        {
            if (LevelsBeatSave.IsLevelComplete(levelId))
            {
                collectedTies++;
            }

            collectedStars += LevelsBeatSave.GetStars(levelId);
        }

        int maxTies = totalLevels;

        maxTies++;

        if (LevelsBeatSave.IsMainMenuTieCollected())
        {
            collectedTies++;
        }


        if (tiesText != null)
        {
            tiesText.text = collectedTies + "/" + maxTies;
        }

        if (starsText != null)
        {
            int maxStars = totalLevels * starsPerLevel;
            starsText.text = collectedStars + "/" + maxStars;
        }
    }
}