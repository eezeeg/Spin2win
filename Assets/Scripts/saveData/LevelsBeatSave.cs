using UnityEngine;

public class LevelsBeatSave : MonoBehaviour
{
    
    //bit thingy
    private const string CompletedLevelsInt = "CompletedLevels";
    private static string GetLevelTime(int id)
    {
        return "Level" + id + "_BestTime";
    }

    public static void SaveLevelComplete(int levelId, float completeTime)
    {

        //with the bits thing set complete to true
        int completedLevels = PlayerPrefs.GetInt(CompletedLevelsInt, 0);
        int bitIndex = levelId - 1;
        completedLevels = SetBit(completedLevels, bitIndex);
        PlayerPrefs.SetInt(CompletedLevelsInt, completedLevels);


        //names of playerPrefs
        string bestTimeKey = GetLevelTime(levelId);


        //get old best time to compare
        float oldBestTIme = PlayerPrefs.GetFloat(bestTimeKey);

        //if first time/better then last time: update time
        if (oldBestTIme <= 0f || completeTime < oldBestTIme)
        {
            PlayerPrefs.SetFloat(bestTimeKey, completeTime);
        }

        //actually save update
        PlayerPrefs.Save();
    }

    public static bool IsLevelComplete(int levelId)
    {
        int completedLevels = PlayerPrefs.GetInt(CompletedLevelsInt, 0);
        int bitIndx = levelId - 1;

        return GetBit(completedLevels, bitIndx);
    }

    public static float GetBestTime(int levelId)
    {
        return PlayerPrefs.GetFloat(GetLevelTime(levelId));
    }


    //bit stuff
    private static int SetBit(int value, int bitidx)
    { 
        return value | (1 << bitidx);
    }

    private static bool GetBit(int value, int bitIdx)
    {
        return (value & (1 << bitIdx)) != 0;
    }

    ///FOR TESTING, DELETE ALL LEVEL COMPLETION DATA
    public static void DeleteEVERYTHING()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
