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

    public void NewGame()
    {
        // Reset toàn bộ tiến trình chơi về ban đầu
        MapProgressManager.Instance.ResetAllProgress();
        
        // Có thể thêm reset các dữ liệu khác ở đây nếu cần
        // Ví dụ: PlayerPrefs.DeleteAll(); // Reset tất cả PlayerPrefs
        
        Debug.Log("New Game started - All progress has been reset!");
        
        // Load lại scene map selection
        SceneManager.LoadScene(mapSelectionSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
