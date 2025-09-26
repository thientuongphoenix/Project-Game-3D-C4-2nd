using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionManager : SaiSingleton<MapSelectionManager>
{
    protected override void Start()
    {
        base.Start();
        this.ResetQuestsOnMapSelection();
        this.EnsureBackgroundMusic();
    }
    
    /// <summary>
    /// Reset tất cả quest khi vào scene map selection
    /// </summary>
    protected virtual void ResetQuestsOnMapSelection()
    {
        try
        {
            Debug.Log("=== RESETTING QUESTS ON MAP SELECTION ===");
            
            // Reset TowerQuestSystem nếu có
            if (TowerQuestSystem.Instance != null)
            {
                TowerQuestSystem.Instance.ResetAndReinitializeQuests();
                Debug.Log("TowerQuestSystem reset for map selection!");
            }
            else
            {
                Debug.LogWarning("TowerQuestSystem.Instance is null! Quest progress may not be reset properly.");
            }
            
            // Reset ItemGuideUI nếu có
            if (ItemGuideUI.Instance != null)
            {
                ItemGuideUI.Instance.ResetGuideState();
                Debug.Log("ItemGuideUI reset for map selection!");
            }
            
            // Reset GameResultManager quests nếu có
            if (GameResultManager.Instance != null)
            {
                GameResultManager.Instance.ResetTutorialQuestsPublic();
                Debug.Log("GameResultManager quests reset for map selection!");
            }
            
            Debug.Log("All quests have been reset for map selection!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ResetQuestsOnMapSelection: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Đảm bảo background music được phát trong map selection
    /// </summary>
    protected virtual void EnsureBackgroundMusic()
    {
        try
        {
            Debug.Log("=== ENSURING BACKGROUND MUSIC IN MAP SELECTION ===");
            
            // Kiểm tra SoundManager có tồn tại không
            if (SoundManager.Instance != null)
            {
                // Kiểm tra xem background music đã được khởi động chưa
                if (SoundManager.Instance.IsBackgroundMusicNullOrInactive())
                {
                    Debug.Log("Background music not found or inactive, starting it...");
                    SoundManager.Instance.StartMusicBackground();
                }
                else
                {
                    Debug.Log("Background music is already active - skipping to avoid duplicate");
                }
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Cannot start background music.");
            }
            
            Debug.Log("Background music check completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in EnsureBackgroundMusic: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Public method để reset quest từ bên ngoài
    /// </summary>
    public virtual void ResetAllQuests()
    {
        this.ResetQuestsOnMapSelection();
    }
    
    /// <summary>
    /// Kiểm tra xem có phải scene map selection không
    /// </summary>
    protected virtual bool IsMapSelectionScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName.Contains("MapSelect") || currentSceneName.Contains("Map_Selection");
    }
}



