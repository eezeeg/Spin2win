using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public void PlayGame()
    {
        //eezee is test playlevel
        UnityEngine.SceneManagement.SceneManager.LoadScene("eezee");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
