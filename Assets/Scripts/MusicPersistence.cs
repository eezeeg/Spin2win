using UnityEngine;

public class MusicPersistence : MonoBehaviour
{
    private void Awake()
    {
        // Find existing music objects to prevent duplicates
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("GameMusic");

        if (musicObj.Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // Keeps the audio playing when loading a new scene
            DontDestroyOnLoad(this.gameObject);
        }

        //foreach (GameObject obj in musicObj)
        //{
        //    if (obj != this.gameObject)
        //    {
        //        DontDestroyOnLoad(obj);
        //    }
        //}
    }
}
