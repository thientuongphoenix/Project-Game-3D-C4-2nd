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
            
            // Tắt nhạc nền hiện tại trước khi chuyển scene
            this.StopCurrentBackgroundMusic();
            
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
    
    /// <summary>
    /// Tắt nhạc nền hiện tại trước khi chuyển scene
    /// </summary>
    protected virtual void StopCurrentBackgroundMusic()
    {
        try
        {
            Debug.Log("=== STOPPING CURRENT BACKGROUND MUSIC ===");
            
            // Kiểm tra SoundManager có tồn tại không
            if (SoundManager.Instance != null)
            {
                Debug.Log("SoundManager found, stopping current background music...");
                
                // Tắt background music hiện tại
                if (SoundManager.Instance.GetBackgroundMusic() != null)
                {
                    SoundManager.Instance.GetBackgroundMusic().gameObject.SetActive(false);
                    Debug.Log("Current background music stopped!");
                }
                else
                {
                    Debug.Log("Current background music is already stopped or null");
                }
                
                // Tắt tất cả music trong listMusic
                if (SoundManager.Instance.GetSoundSpawnerCtrl() != null && 
                    SoundManager.Instance.GetSoundSpawnerCtrl().Spawner != null)
                {
                    // Tìm tất cả MusicCtrl trong scene và tắt chúng
                    MusicCtrl[] allMusic = FindObjectsOfType<MusicCtrl>();
                    int stoppedCount = 0;
                    
                    foreach (MusicCtrl music in allMusic)
                    {
                        if (music != null && music.gameObject.activeSelf)
                        {
                            music.gameObject.SetActive(false);
                            stoppedCount++;
                        }
                    }
                    
                    Debug.Log($"Stopped {stoppedCount} music objects in scene");
                }
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Cannot stop background music.");
            }
            
            Debug.Log("Current background music stop completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StopCurrentBackgroundMusic: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
}
