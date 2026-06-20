using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelSelect;
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
}
