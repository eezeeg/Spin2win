using UnityEngine;

public class LevelsBeatSave : MonoBehaviour
{
    private const string CompletedLevelsInt = "CompletedLevels";

    private static string GetLevelTime(int id)
    {
        return "Level" + id + "_BestTime";
    }

    private static string GetLevelStars(int id)
    {
        return "Level" + id + "_Stars";
    }

    public static void SaveLevelComplete(int levelId, float completeTime, int stars)
    {
        stars = Mathf.Clamp(stars, 1, 3);

        int completedLevels = PlayerPrefs.GetInt(CompletedLevelsInt, 0);
        int bitIndex = levelId - 1;
        completedLevels = SetBit(completedLevels, bitIndex);
        PlayerPrefs.SetInt(CompletedLevelsInt, completedLevels);

        string bestTimeKey = GetLevelTime(levelId);
        float oldBestTime = PlayerPrefs.GetFloat(bestTimeKey, 0f);

        if (oldBestTime <= 0f || completeTime < oldBestTime)
        {
            PlayerPrefs.SetFloat(bestTimeKey, completeTime);
        }

        string starsKey = GetLevelStars(levelId);
        int oldStars = PlayerPrefs.GetInt(starsKey, 0);

        if (stars > oldStars)
        {
            PlayerPrefs.SetInt(starsKey, stars);
        }

        PlayerPrefs.Save();
    }

    public static void SaveLevelComplete(int levelId, float completeTime)
    {
        SaveLevelComplete(levelId, completeTime, 1);
    }

    public static bool IsLevelComplete(int levelId)
    {
        int completedLevels = PlayerPrefs.GetInt(CompletedLevelsInt, 0);
        int bitIndex = levelId - 1;

        return GetBit(completedLevels, bitIndex);
    }

    public static float GetBestTime(int levelId)
    {
        return PlayerPrefs.GetFloat(GetLevelTime(levelId), 0f);
    }

    public static int GetStars(int levelId)
    {
        return PlayerPrefs.GetInt(GetLevelStars(levelId), 0);
    }

    private static int SetBit(int value, int bitIndex)
    {
        return value | (1 << bitIndex);
    }

    private static bool GetBit(int value, int bitIndex)
    {
        return (value & (1 << bitIndex)) != 0;
    }

    public static void DeleteEVERYTHING()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
