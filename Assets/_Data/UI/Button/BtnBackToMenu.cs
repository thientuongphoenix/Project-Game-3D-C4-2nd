using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnBackToMenu : ButttonAbstract
{
    [Header("Settings")]
    [SerializeField] protected string menuSceneName = "Hai_Menu";
    [SerializeField] protected float delayBeforeLoad = 0.1f;
    
    protected override void OnClick()
    {
        BackToMenu();
    }
    
    public virtual void BackToMenu()
    {
        try
        {
            Debug.Log("=== BACK TO MENU ===");
            Debug.Log($"Loading scene: {menuSceneName}");
            
            // Resume game trước khi chuyển scene
            Time.timeScale = 1f;
            Debug.Log("Game resumed before loading menu");
            
            // Đóng setting UI nếu đang mở
            if (UISetting.Instance != null)
            {
                UISetting.Instance.Hide();
                Debug.Log("Settings UI closed");
            }
            
            // Hiện cursor
            if (HideMouse.Instance != null)
            {
                HideMouse.Instance.isCursorVisible = true;
                Debug.Log("Cursor made visible");
            }
            
            // Load menu scene với delay nhỏ
            if (delayBeforeLoad > 0f)
            {
                StartCoroutine(LoadMenuAfterDelay());
            }
            else
            {
                SceneManager.LoadScene(menuSceneName);
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong BackToMenu: {e.Message}");
        }
    }
    
    protected virtual System.Collections.IEnumerator LoadMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(menuSceneName);
    }
    
    // Method để set scene name từ bên ngoài
    public virtual void SetMenuSceneName(string sceneName)
    {
        menuSceneName = sceneName;
        Debug.Log($"Menu scene name set to: {menuSceneName}");
    }
    
    // Method để load scene ngay lập tức (không delay)
    public virtual void BackToMenuImmediate()
    {
        delayBeforeLoad = 0f;
        BackToMenu();
    }
}
