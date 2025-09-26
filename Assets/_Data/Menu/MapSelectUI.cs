using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectUI : MonoBehaviour
{
    public void LoadTutorialMap()
    {
        // Reset quest trước khi load tutorial map
        this.ResetQuestsBeforeLoad();
        
        // Tắt background music khi vào tutorial (tutorial có hệ thống sound riêng)
        this.StopBackgroundMusicForTutorial();
        
        SceneManager.LoadScene("Hai_SampleScene");
    }

    public void LoadMap1()
    {
        // Reset quest trước khi load map 1
        this.ResetQuestsBeforeLoad();
        
        // Tắt background music khi vào map 1 (map có hệ thống sound riêng)
        this.StopBackgroundMusicForMap();
        
        SceneManager.LoadScene("Hai_Map");
    }

    public void LoadMap2()
    {
        // Reset quest trước khi load map 2
        this.ResetQuestsBeforeLoad();
        
        // Tắt background music khi vào map 2 (map có hệ thống sound riêng)
        this.StopBackgroundMusicForMap();
        
        SceneManager.LoadScene("Map2");
    }

    public void ReturnToMenu()
    {
        // Không cần khởi động background music trước khi chuyển scene
        // MainMenu sẽ tự động khởi động nhạc nền khi vào scene
        Debug.Log("Returning to menu - MainMenu will handle background music");
        
        SceneManager.LoadScene("Hai_Menu");
    }
    
    /// <summary>
    /// Khởi động background music khi quay về menu
    /// </summary>
    protected virtual void StartBackgroundMusicForMenu()
    {
        try
        {
            Debug.Log("=== STARTING BACKGROUND MUSIC FOR MENU ===");
            
            // Kiểm tra SoundManager có tồn tại không
            if (SoundManager.Instance != null)
            {
                Debug.Log("SoundManager found, starting background music for menu...");
                
                // Khởi động background music
                SoundManager.Instance.StartMusicBackground();
                
                // Áp dụng settings để đảm bảo volume đúng
                SoundManager.Instance.LoadSettingsPublic();
                
                Debug.Log("Background music started for menu!");
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Cannot start background music for menu.");
            }
            
            Debug.Log("Background music start for menu completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StartBackgroundMusicForMenu: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Tắt background music trước khi vào tutorial (tutorial có hệ thống sound riêng)
    /// </summary>
    protected virtual void StopBackgroundMusicForTutorial()
    {
        try
        {
            Debug.Log("=== STOPPING BACKGROUND MUSIC FOR TUTORIAL ===");
            
            // Kiểm tra SoundManager có tồn tại không
            if (SoundManager.Instance != null)
            {
                Debug.Log("SoundManager found, stopping background music for tutorial...");
                
                // Tắt background music
                if (SoundManager.Instance.GetBackgroundMusic() != null)
                {
                    SoundManager.Instance.GetBackgroundMusic().gameObject.SetActive(false);
                    Debug.Log("Background music stopped for tutorial (tutorial has its own sound system)");
                }
                else
                {
                    Debug.Log("Background music is already stopped or null");
                }
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Cannot stop background music for tutorial.");
            }
            
            Debug.Log("Background music stop for tutorial completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StopBackgroundMusicForTutorial: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Tắt background music trước khi vào map (map có hệ thống sound riêng)
    /// </summary>
    protected virtual void StopBackgroundMusicForMap()
    {
        try
        {
            Debug.Log("=== STOPPING BACKGROUND MUSIC FOR MAP ===");
            
            // Kiểm tra SoundManager có tồn tại không
            if (SoundManager.Instance != null)
            {
                Debug.Log("SoundManager found, stopping background music for map...");
                
                // Tắt background music
                if (SoundManager.Instance.GetBackgroundMusic() != null)
                {
                    SoundManager.Instance.GetBackgroundMusic().gameObject.SetActive(false);
                    Debug.Log("Background music stopped for map (map has its own sound system)");
                }
                else
                {
                    Debug.Log("Background music is already stopped or null");
                }
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Cannot stop background music for map.");
            }
            
            Debug.Log("Background music stop for map completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StopBackgroundMusicForMap: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Đảm bảo sound system được khởi tạo trước khi chuyển scene
    /// </summary>
    protected virtual void EnsureSoundSystemBeforeSceneChange()
    {
        try
        {
            Debug.Log("=== ENSURING SOUND SYSTEM BEFORE SCENE CHANGE ===");
            
            // Kiểm tra SoundManager có tồn tại không
            if (SoundManager.Instance != null)
            {
                Debug.Log("SoundManager found, ensuring sound system is ready...");
                
                // Đảm bảo SoundSpawnerCtrl tồn tại
                if (SoundManager.Instance.IsSoundSpawnerCtrlNull())
                {
                    Debug.Log("SoundSpawnerCtrl is null, trying to reload...");
                    SoundManager.Instance.LoadSoundSpawnerCtrlPublic();
                    SoundManager.Instance.EnsureSoundSpawnerExistsPublic();
                }
                
                // Đảm bảo background music được khởi động
                if (SoundManager.Instance.IsBackgroundMusicNullOrInactive())
                {
                    Debug.Log("Starting background music before scene change...");
                    SoundManager.Instance.StartMusicBackground();
                }
                else
                {
                    Debug.Log("Background music is already active");
                }
                
                // Áp dụng lại settings để đảm bảo volume đúng
                SoundManager.Instance.LoadSettingsPublic();
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Sound system may not work in next scene.");
            }
            
            Debug.Log("Sound system check before scene change completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in EnsureSoundSystemBeforeSceneChange: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Reset tất cả quest trước khi load map
    /// </summary>
    protected virtual void ResetQuestsBeforeLoad()
    {
        try
        {
            Debug.Log("=== RESETTING QUESTS BEFORE LOADING MAP ===");
            
            // Reset TowerQuestSystem nếu có
            if (TowerQuestSystem.Instance != null)
            {
                TowerQuestSystem.Instance.ResetAndReinitializeQuests();
                Debug.Log("TowerQuestSystem reset before map load!");
            }
            
            // Reset ItemGuideUI nếu có
            if (ItemGuideUI.Instance != null)
            {
                ItemGuideUI.Instance.ResetGuideState();
                Debug.Log("ItemGuideUI reset before map load!");
            }
            
            // Reset GameResultManager quests nếu có
            if (GameResultManager.Instance != null)
            {
                GameResultManager.Instance.ResetTutorialQuestsPublic();
                Debug.Log("GameResultManager quests reset before map load!");
            }
            
            Debug.Log("All quests have been reset before map load!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ResetQuestsBeforeLoad: {e.Message}");
        }
    }
}
