using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string mapSelectionSceneName = "MapSelect_Hai";
    
    public void PlayGame()
    {
        // Load map selection scene instead of next scene
        SceneManager.LoadScene(mapSelectionSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
