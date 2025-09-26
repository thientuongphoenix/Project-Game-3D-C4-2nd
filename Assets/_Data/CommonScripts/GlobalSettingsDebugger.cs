using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script để debug và fix vấn đề GlobalSettingsManager
/// </summary>
public class GlobalSettingsDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] protected bool debugOnStart = true;
    [SerializeField] protected bool debugOnEnable = true;
    [SerializeField] protected bool fixSingletonIssues = true;
    
    protected virtual void Start()
    {
        if (debugOnStart)
        {
            this.DebugGlobalSettingsManager();
        }
    }
    
    protected virtual void OnEnable()
    {
        if (debugOnEnable)
        {
            this.DebugGlobalSettingsManager();
        }
    }
    
    /// <summary>
    /// Debug GlobalSettingsManager
    /// </summary>
    public virtual void DebugGlobalSettingsManager()
    {
        try
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            
            Debug.Log($"=== GLOBAL SETTINGS DEBUGGER - {currentSceneName} ===");
            
            // Kiểm tra GlobalSettingsManager.Instance
            if (GlobalSettingsManager.Instance == null)
            {
                Debug.LogError("❌ GlobalSettingsManager.Instance is NULL!");
                
                // Tìm tất cả GlobalSettingsManager trong scene
                GlobalSettingsManager[] allManagers = FindObjectsOfType<GlobalSettingsManager>();
                Debug.Log($"Found {allManagers.Length} GlobalSettingsManager(s) in scene");
                
                for (int i = 0; i < allManagers.Length; i++)
                {
                    Debug.Log($"Manager {i + 1}: {allManagers[i].name} - Active: {allManagers[i].gameObject.activeSelf}");
                }
                
                // Fix singleton nếu có thể
                if (fixSingletonIssues && allManagers.Length > 0)
                {
                    this.FixSingletonIssue(allManagers);
                }
            }
            else
            {
                Debug.Log("✅ GlobalSettingsManager.Instance exists");
                Debug.Log($"Instance name: {GlobalSettingsManager.Instance.name}");
                Debug.Log($"Instance active: {GlobalSettingsManager.Instance.gameObject.activeSelf}");
                Debug.Log($"Instance scene: {GlobalSettingsManager.Instance.gameObject.scene.name}");
            }
            
            // Kiểm tra PlayerPrefs
            int width = PlayerPrefs.GetInt("ResolutionWidth", -1);
            int height = PlayerPrefs.GetInt("ResolutionHeight", -1);
            int fullscreen = PlayerPrefs.GetInt("FullScreen", -1);
            int quality = PlayerPrefs.GetInt("QualityLevel", -1);
            
            Debug.Log($"PlayerPrefs - Width: {width}, Height: {height}, Fullscreen: {fullscreen}, Quality: {quality}");
            
            // Kiểm tra Screen settings
            Debug.Log($"Screen - Width: {Screen.width}, Height: {Screen.height}, Fullscreen: {Screen.fullScreen}");
            
            // Force apply settings nếu có vấn đề
            if (GlobalSettingsManager.Instance != null)
            {
                Debug.Log("Force calling ApplyGlobalSettings()...");
                GlobalSettingsManager.Instance.ApplyGlobalSettings();
            }
            
            Debug.Log("==========================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong DebugGlobalSettingsManager: {e.Message}");
        }
    }
    
    /// <summary>
    /// Fix singleton issue
    /// </summary>
    protected virtual void FixSingletonIssue(GlobalSettingsManager[] managers)
    {
        try
        {
            Debug.Log("=== FIXING SINGLETON ISSUE ===");
            
            // Tìm manager đầu tiên active
            GlobalSettingsManager activeManager = null;
            for (int i = 0; i < managers.Length; i++)
            {
                if (managers[i].gameObject.activeSelf)
                {
                    activeManager = managers[i];
                    break;
                }
            }
            
            if (activeManager == null && managers.Length > 0)
            {
                activeManager = managers[0];
                activeManager.gameObject.SetActive(true);
                Debug.Log($"Activated manager: {activeManager.name}");
            }
            
            if (activeManager != null)
            {
                // Deactivate các manager khác
                for (int i = 0; i < managers.Length; i++)
                {
                    if (managers[i] != activeManager)
                    {
                        Debug.Log($"Deactivating duplicate manager: {managers[i].name}");
                        managers[i].gameObject.SetActive(false);
                    }
                }
                
                // Force apply settings
                Debug.Log("Force applying settings...");
                activeManager.ApplyGlobalSettings();
                
                Debug.Log("✅ Singleton issue fixed");
            }
            else
            {
                Debug.LogError("❌ No active manager found to fix singleton");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong FixSingletonIssue: {e.Message}");
        }
    }
    
    /// <summary>
    /// Test method
    /// </summary>
    [ContextMenu("Debug Global Settings")]
    public virtual void DebugGlobalSettingsContext()
    {
        this.DebugGlobalSettingsManager();
    }
    
    /// <summary>
    /// Force fix singleton
    /// </summary>
    [ContextMenu("Force Fix Singleton")]
    public virtual void ForceFixSingletonContext()
    {
        GlobalSettingsManager[] allManagers = FindObjectsOfType<GlobalSettingsManager>();
        if (allManagers.Length > 0)
        {
            this.FixSingletonIssue(allManagers);
        }
        else
        {
            Debug.LogWarning("No GlobalSettingsManager found to fix");
        }
    }
}
