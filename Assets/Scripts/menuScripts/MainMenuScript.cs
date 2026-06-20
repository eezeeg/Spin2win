using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    int test;

    public void PlayGame()
    {
        //eezee is test playlevel
        UnityEngine.SceneManagement.SceneManager.LoadScene("eezee");
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

    }
}
