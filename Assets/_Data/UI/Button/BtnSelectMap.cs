using UnityEngine;

public class BtnSelectMap : ButttonAbstract
{
    [Header("Settings")]
    [SerializeField] protected string mapSelectionSceneName = "MapSelect_Hai";
    [SerializeField] protected float delayBeforeLoad = 0.1f;
    
    protected override void OnClick()
    {
        SelectMap();
    }
    
    public virtual void SelectMap()
    {
        try
        {
            Debug.Log("=== SELECT MAP FROM WIN PANEL ===");
            Debug.Log($"Loading scene: {mapSelectionSceneName}");
            
            // Khôi phục SFX về bình thường trước khi chuyển scene
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.VolumeSfxUpdating(1f);
                Debug.Log("SFX volume restored to normal");
            }
            
            // Tắt nhạc nền hiện tại trước khi chuyển scene
            this.StopCurrentBackgroundMusic();
            
            // Load map selection scene với delay nhỏ
            if (delayBeforeLoad > 0f)
            {
                StartCoroutine(LoadMapSelectionAfterDelay());
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(mapSelectionSceneName);
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong SelectMap: {e.Message}");
        }
    }
    
    protected virtual System.Collections.IEnumerator LoadMapSelectionAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        UnityEngine.SceneManagement.SceneManager.LoadScene(mapSelectionSceneName);
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
    
    // Method để set scene name từ bên ngoài
    public virtual void SetMapSelectionSceneName(string sceneName)
    {
        mapSelectionSceneName = sceneName;
        Debug.Log($"Map selection scene name set to: {mapSelectionSceneName}");
    }
    
    // Method để load scene ngay lập tức (không delay)
    public virtual void SelectMapImmediate()
    {
        delayBeforeLoad = 0f;
        SelectMap();
    }
}
