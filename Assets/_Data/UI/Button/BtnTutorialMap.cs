using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnTutorialMap : BtnMapLock
{
    [Header("Tutorial Map Settings")]
    [SerializeField] protected string tutorialRequiredMap = ""; // Tutorial map luôn unlock
    
    protected override void Start()
    {
        base.Start();
        this.SetRequiredMap(tutorialRequiredMap); // Không cần map nào để unlock tutorial
    }
    
    protected override void ExecuteMapAction()
    {
        Debug.Log("Loading Tutorial Map...");
        
        // Reset quests khi chọn tutorial map
        if (TowerQuestSystem.Instance != null)
        {
            TowerQuestSystem.Instance.ResetAndReinitializeQuests();
            Debug.Log("Quests reset for tutorial map!");
        }
        else
        {
            Debug.LogWarning("TowerQuestSystem.Instance is null! Tutorial may not be reset properly.");
        }
        
        // Đảm bảo sound system được khởi tạo trước khi chuyển scene
        this.EnsureSoundSystemBeforeSceneChange();
        
        SceneManager.LoadScene("Hai_SampleScene");
    }
    
    /// <summary>
    /// Đảm bảo sound system được khởi tạo trước khi chuyển scene
    /// </summary>
    protected virtual void EnsureSoundSystemBeforeSceneChange()
    {
        try
        {
            Debug.Log("=== ENSURING SOUND SYSTEM BEFORE TUTORIAL SCENE CHANGE ===");
            
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
                
                // Tắt background music trước khi vào tutorial (tutorial có hệ thống sound riêng)
                if (SoundManager.Instance.GetBackgroundMusic() != null)
                {
                    SoundManager.Instance.GetBackgroundMusic().gameObject.SetActive(false);
                    Debug.Log("Background music stopped for tutorial (tutorial has its own sound system)");
                }
                else
                {
                    Debug.Log("Background music is already stopped or null");
                }
                
                // Áp dụng lại settings để đảm bảo volume đúng
                SoundManager.Instance.LoadSettingsPublic();
            }
            else
            {
                Debug.LogWarning("SoundManager.Instance is null! Sound system may not work in tutorial scene.");
            }
            
            Debug.Log("Sound system check before tutorial scene change completed!");
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in EnsureSoundSystemBeforeSceneChange: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    protected override void ShowLockMessage()
    {
        string message = "Tutorial Map is locked!";
        Debug.Log(message);
        
        if (this.lockText != null)
        {
            this.lockText.text = message;
            this.lockText.gameObject.SetActive(true);
            Invoke(nameof(HideLockMessage), 3f);
        }
    }
} 