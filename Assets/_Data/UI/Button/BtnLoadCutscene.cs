using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnLoadCutscene : ButttonAbstract
{
    [Header("Cutscene Settings")]
    [SerializeField] protected string cutsceneSceneName = "Hai_Cutscene";
    [SerializeField] protected float delayBeforeLoad = 0.1f;
    
    protected override void OnClick()
    {
        LoadCutscene();
    }
    
    public virtual void LoadCutscene()
    {
        try
        {
            Debug.Log("=== LOADING CUTSCENE ===");
            Debug.Log($"Cutscene scene name: {cutsceneSceneName}");
            
            // Resume game trước khi chuyển scene
            Time.timeScale = 1f;
            Debug.Log("Game resumed before loading cutscene");
            
            // Tắt nhạc nền hiện tại trước khi chuyển scene
            this.StopCurrentBackgroundMusic();
            
            // Đóng setting UI nếu đang mở
            if (UISetting.Instance != null)
            {
                UISetting.Instance.Hide();
                Debug.Log("Settings UI closed");
            }
            
            // Ẩn cursor trong cutscene
            if (HideMouse.Instance != null)
            {
                HideMouse.Instance.isCursorVisible = false;
                Debug.Log("Cursor hidden for cutscene");
            }
            
            // Load cutscene scene với delay nhỏ
            if (delayBeforeLoad > 0f)
            {
                StartCoroutine(LoadCutsceneAfterDelay());
            }
            else
            {
                SceneManager.LoadScene(cutsceneSceneName);
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong LoadCutscene: {e.Message}");
        }
    }
    
    protected virtual System.Collections.IEnumerator LoadCutsceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(cutsceneSceneName);
    }
    
    // Method để set scene name từ bên ngoài
    public virtual void SetCutsceneSceneName(string sceneName)
    {
        cutsceneSceneName = sceneName;
        Debug.Log($"Cutscene scene name set to: {cutsceneSceneName}");
    }
    
    // Method để load scene ngay lập tức (không delay)
    public virtual void LoadCutsceneImmediate()
    {
        delayBeforeLoad = 0f;
        LoadCutscene();
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
