using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script tự động thêm SceneSettingsApplier vào scene khi chuyển scene
/// Chỉ cần thêm vào scene đầu tiên (Hai_Menu)
/// </summary>
public class AutoSceneSettingsApplier : MonoBehaviour
{
    [Header("Auto Scene Settings Application")]
    [SerializeField] protected bool autoApplyOnSceneChange = true;
    [SerializeField] protected bool debugAutoApply = true;
    
    protected virtual void Start()
    {
        if (autoApplyOnSceneChange)
        {
            // Subscribe to scene change events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }
    
    protected virtual void OnDestroy()
    {
        // Unsubscribe from events
        if (autoApplyOnSceneChange)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (debugAutoApply)
        {
            Debug.Log($"=== AUTO SCENE SETTINGS APPLIER - {scene.name} ===");
        }
        
        // Kiểm tra xem scene có SceneSettingsApplier chưa
        SceneSettingsApplier existingApplier = FindObjectOfType<SceneSettingsApplier>();
        if (existingApplier != null)
        {
            if (debugAutoApply)
            {
                Debug.Log("✅ SceneSettingsApplier already exists in scene");
            }
            return;
        }
        
        // Kiểm tra xem scene có GlobalSettingsManager chưa
        if (GlobalSettingsManager.Instance != null)
        {
            if (debugAutoApply)
            {
                Debug.Log("✅ GlobalSettingsManager exists, applying settings with delay...");
            }
            // Sử dụng delay để đảm bảo scene đã load xong
            GlobalSettingsManager.Instance.ForceApplySettingsWithDelay(0.1f);
        }
        else
        {
            if (debugAutoApply)
            {
                Debug.LogWarning("⚠️ GlobalSettingsManager not found, creating SceneSettingsApplier...");
            }
            this.CreateSceneSettingsApplier();
        }
        
        if (debugAutoApply)
        {
            Debug.Log("=== AUTO SCENE SETTINGS APPLIER COMPLETE ===");
        }
    }
    
    /// <summary>
    /// Tạo SceneSettingsApplier trong scene hiện tại
    /// </summary>
    protected virtual void CreateSceneSettingsApplier()
    {
        // Tạo GameObject mới
        GameObject settingsApplierObj = new GameObject("SceneSettingsApplier");
        
        // Thêm SceneSettingsApplier component
        SceneSettingsApplier applier = settingsApplierObj.AddComponent<SceneSettingsApplier>();
        
        // Đảm bảo không bị destroy khi load scene mới
        DontDestroyOnLoad(settingsApplierObj);
        
        if (debugAutoApply)
        {
            Debug.Log("✅ SceneSettingsApplier created and set to DontDestroyOnLoad");
        }
    }
    
    /// <summary>
    /// Context menu để force apply settings cho scene hiện tại
    /// </summary>
    [ContextMenu("Force Apply Settings to Current Scene")]
    protected virtual void ForceApplySettingsToCurrentSceneContext()
    {
        Debug.Log("🔧 FORCING SETTINGS APPLICATION TO CURRENT SCENE...");
        
        if (GlobalSettingsManager.Instance != null)
        {
            GlobalSettingsManager.Instance.ApplyGlobalSettings();
        }
        else
        {
            Debug.LogWarning("GlobalSettingsManager.Instance is null! Creating SceneSettingsApplier...");
            this.CreateSceneSettingsApplier();
        }
    }
    
    /// <summary>
    /// Context menu để show current scene settings
    /// </summary>
    [ContextMenu("Show Current Scene Settings")]
    protected virtual void ShowCurrentSceneSettingsContext()
    {
        Debug.Log("=== CURRENT SCENE SETTINGS ===");
        Debug.Log($"Scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"Screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        Debug.Log($"Quality: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        Debug.Log($"PlayerPrefs Resolution: {PlayerPrefs.GetInt("ResolutionWidth", -1)}x{PlayerPrefs.GetInt("ResolutionHeight", -1)}");
        Debug.Log($"PlayerPrefs Fullscreen: {PlayerPrefs.GetInt("FullScreen", -1)}");
        Debug.Log($"PlayerPrefs Quality: {PlayerPrefs.GetInt("QualityLevel", -1)}");
        
        // Check if SceneSettingsApplier exists
        SceneSettingsApplier applier = FindObjectOfType<SceneSettingsApplier>();
        if (applier != null)
        {
            Debug.Log("✅ SceneSettingsApplier: EXISTS");
        }
        else
        {
            Debug.LogWarning("❌ SceneSettingsApplier: NOT FOUND");
        }
        
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
}
