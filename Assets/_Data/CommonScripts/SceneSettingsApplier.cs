using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script để áp dụng settings khi chuyển scene
/// Thêm vào tất cả scene để đảm bảo settings được đồng bộ
/// </summary>
public class SceneSettingsApplier : MonoBehaviour
{
    [Header("Scene Settings Application")]
    [SerializeField] protected bool applySettingsOnStart = true;
    [SerializeField] protected bool applySettingsOnEnable = true;
    [SerializeField] protected bool debugSceneSettings = true;
    
    protected virtual void Start()
    {
        if (applySettingsOnStart)
        {
            this.ApplySceneSettings();
        }
    }
    
    protected virtual void OnEnable()
    {
        if (applySettingsOnEnable)
        {
            this.ApplySceneSettings();
        }
    }
    
    /// <summary>
    /// Áp dụng settings cho scene hiện tại
    /// </summary>
    protected virtual void ApplySceneSettings()
    {
        if (debugSceneSettings)
        {
            Debug.Log($"=== SCENE SETTINGS APPLIER - {SceneManager.GetActiveScene().name} ===");
        }
        
        // Kiểm tra xem có GlobalSettingsManager không
        if (GlobalSettingsManager.Instance != null)
        {
            if (debugSceneSettings)
            {
                Debug.Log("Using GlobalSettingsManager to apply settings...");
            }
            GlobalSettingsManager.Instance.ApplyGlobalSettings();
        }
        else
        {
            if (debugSceneSettings)
            {
                Debug.LogWarning("GlobalSettingsManager.Instance is null! Applying settings manually...");
            }
            this.ApplySettingsManually();
        }
        
        if (debugSceneSettings)
        {
            Debug.Log("=== SCENE SETTINGS APPLIER COMPLETE ===");
        }
    }
    
    /// <summary>
    /// Áp dụng settings thủ công nếu GlobalSettingsManager không có
    /// </summary>
    protected virtual void ApplySettingsManually()
    {
        // Load settings từ PlayerPrefs
        int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        bool isFullscreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", 3);
        
        if (debugSceneSettings)
        {
            Debug.Log($"Manual settings: {width}x{height}, Fullscreen: {isFullscreen}, Quality: {qualityLevel}");
            Debug.Log($"Current screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        }
        
        // Kiểm tra xem có cần áp dụng settings không
        bool needsResolutionUpdate = (Screen.width != width || Screen.height != height);
        bool needsFullscreenUpdate = (Screen.fullScreen != isFullscreen);
        bool needsQualityUpdate = (QualitySettings.GetQualityLevel() != qualityLevel);
        
        if (needsResolutionUpdate || needsFullscreenUpdate || needsQualityUpdate)
        {
            // Áp dụng resolution và fullscreen
            if (needsResolutionUpdate || needsFullscreenUpdate)
            {
                // Sử dụng FullScreenMode để tương thích với Player Settings
                if (isFullscreen)
                {
                    Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
                }
                else
                {
                    Screen.SetResolution(width, height, FullScreenMode.Windowed);
                }
                
                if (debugSceneSettings)
                {
                    Debug.Log($"✅ Applied resolution: {width}x{height}, Fullscreen: {isFullscreen} (Mode: {(isFullscreen ? "FullScreenWindow" : "Windowed")})");
                }
            }
            
            // Áp dụng quality
            if (needsQualityUpdate)
            {
                QualitySettings.SetQualityLevel(qualityLevel);
                
                if (debugSceneSettings)
                {
                    Debug.Log($"✅ Applied quality: {QualitySettings.names[qualityLevel]}");
                }
            }
        }
        else
        {
            if (debugSceneSettings)
            {
                Debug.Log("✅ Settings already synchronized");
            }
        }
    }
    
    /// <summary>
    /// Context menu để force apply settings
    /// </summary>
    [ContextMenu("Force Apply Settings")]
    protected virtual void ForceApplySettingsContext()
    {
        this.ApplySceneSettings();
    }
    
    /// <summary>
    /// Context menu để show current settings
    /// </summary>
    [ContextMenu("Show Current Settings")]
    protected virtual void ShowCurrentSettingsContext()
    {
        Debug.Log("=== CURRENT SCENE SETTINGS ===");
        Debug.Log($"Scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"Screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        Debug.Log($"Quality: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        Debug.Log($"PlayerPrefs Resolution: {PlayerPrefs.GetInt("ResolutionWidth", -1)}x{PlayerPrefs.GetInt("ResolutionHeight", -1)}");
        Debug.Log($"PlayerPrefs Fullscreen: {PlayerPrefs.GetInt("FullScreen", -1)}");
        Debug.Log($"PlayerPrefs Quality: {PlayerPrefs.GetInt("QualityLevel", -1)}");
        Debug.Log("================================");
    }
}
