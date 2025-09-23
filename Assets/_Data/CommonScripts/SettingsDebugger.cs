using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script để debug settings khi chuyển scene
/// </summary>
public class SettingsDebugger : MonoBehaviour
{
    [Header("Settings Debug")]
    [SerializeField] protected bool debugOnStart = true;
    [SerializeField] protected bool debugOnSceneChange = true;
    [SerializeField] protected bool debugOnApplicationFocus = true;
    
    protected virtual void Start()
    {
        if (debugOnStart)
        {
            this.DebugCurrentSettings("START");
        }
        
        // Subscribe to scene change events
        if (debugOnSceneChange)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }
    
    protected virtual void OnDestroy()
    {
        // Unsubscribe from events
        if (debugOnSceneChange)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    protected virtual void OnApplicationFocus(bool hasFocus)
    {
        if (debugOnApplicationFocus && hasFocus)
        {
            this.DebugCurrentSettings("APPLICATION FOCUS");
        }
    }
    
    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.DebugCurrentSettings($"SCENE LOADED: {scene.name}");
    }
    
    /// <summary>
    /// Debug current settings
    /// </summary>
    protected virtual void DebugCurrentSettings(string context)
    {
        Debug.Log($"=== SETTINGS DEBUG - {context} ===");
        Debug.Log($"Scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"Screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        Debug.Log($"Quality: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        Debug.Log($"PlayerPrefs Resolution: {PlayerPrefs.GetInt("ResolutionWidth", -1)}x{PlayerPrefs.GetInt("ResolutionHeight", -1)}");
        Debug.Log($"PlayerPrefs Fullscreen: {PlayerPrefs.GetInt("FullScreen", -1)}");
        Debug.Log($"PlayerPrefs Quality: {PlayerPrefs.GetInt("QualityLevel", -1)}");
        
        // Check if GlobalSettingsManager exists
        if (GlobalSettingsManager.Instance != null)
        {
            Debug.Log("✅ GlobalSettingsManager: EXISTS");
        }
        else
        {
            Debug.LogWarning("❌ GlobalSettingsManager: NOT FOUND");
        }
        
        Debug.Log("================================");
    }
    
    /// <summary>
    /// Context menu để debug settings
    /// </summary>
    [ContextMenu("Debug Current Settings")]
    protected virtual void DebugCurrentSettingsContext()
    {
        this.DebugCurrentSettings("MANUAL DEBUG");
    }
    
    /// <summary>
    /// Context menu để force apply settings
    /// </summary>
    [ContextMenu("Force Apply Settings")]
    protected virtual void ForceApplySettingsContext()
    {
        Debug.Log("🔧 FORCING SETTINGS APPLICATION...");
        
        if (GlobalSettingsManager.Instance != null)
        {
            GlobalSettingsManager.Instance.ApplyGlobalSettings();
        }
        else
        {
            Debug.LogWarning("GlobalSettingsManager.Instance is null! Cannot force apply settings.");
        }
    }
    
    /// <summary>
    /// Context menu để clear PlayerPrefs
    /// </summary>
    [ContextMenu("Clear PlayerPrefs")]
    protected virtual void ClearPlayerPrefsContext()
    {
        Debug.Log("🗑️ CLEARING PLAYERPREFS...");
        
        PlayerPrefs.DeleteKey("ResolutionWidth");
        PlayerPrefs.DeleteKey("ResolutionHeight");
        PlayerPrefs.DeleteKey("FullScreen");
        PlayerPrefs.DeleteKey("QualityLevel");
        PlayerPrefs.DeleteKey("ResolutionIndex");
        PlayerPrefs.Save();
        
        Debug.Log("PlayerPrefs cleared!");
    }
    
    /// <summary>
    /// Context menu để set default settings
    /// </summary>
    [ContextMenu("Set Default Settings")]
    protected virtual void SetDefaultSettingsContext()
    {
        Debug.Log("⚙️ SETTING DEFAULT SETTINGS...");
        
        PlayerPrefs.SetInt("ResolutionWidth", 1920);
        PlayerPrefs.SetInt("ResolutionHeight", 1080);
        PlayerPrefs.SetInt("FullScreen", 1);
        PlayerPrefs.SetInt("QualityLevel", 3);
        PlayerPrefs.SetInt("ResolutionIndex", 0);
        PlayerPrefs.Save();
        
        // Apply immediately
        Screen.SetResolution(1920, 1080, true);
        QualitySettings.SetQualityLevel(3);
        
        Debug.Log("Default settings applied!");
    }
}
