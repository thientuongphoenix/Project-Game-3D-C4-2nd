using UnityEngine;

/// <summary>
/// Manager để đảm bảo settings được áp dụng ngay khi game khởi động (build)
/// </summary>
public class BuildSettingsManager : MonoBehaviour
{
    [Header("Build Settings")]
    [SerializeField] protected bool applySettingsOnAwake = true;
    [SerializeField] protected bool debugBuildSettings = true;
    
    [Header("Default Build Settings")]
    [SerializeField] protected int buildDefaultWidth = 1920;
    [SerializeField] protected int buildDefaultHeight = 1080;
    [SerializeField] protected bool buildDefaultFullscreen = true;
    [SerializeField] protected int buildDefaultQuality = 3;
    
    protected virtual void Awake()
    {
        if (applySettingsOnAwake)
        {
            this.ApplyBuildSettings();
        }
    }
    
    /// <summary>
    /// Áp dụng settings ngay khi game khởi động
    /// </summary>
    protected virtual void ApplyBuildSettings()
    {
        if (debugBuildSettings)
        {
            Debug.Log("=== BUILD SETTINGS MANAGER - GAME STARTUP ===");
        }
        
        // Kiểm tra xem có PlayerPrefs không
        bool hasPlayerPrefs = PlayerPrefs.HasKey("ResolutionWidth") || 
                             PlayerPrefs.HasKey("ResolutionHeight") || 
                             PlayerPrefs.HasKey("FullScreen");
        
        int width, height;
        bool fullscreen;
        int quality;
        
        if (hasPlayerPrefs)
        {
            // Load từ PlayerPrefs nếu có
            width = PlayerPrefs.GetInt("ResolutionWidth", buildDefaultWidth);
            height = PlayerPrefs.GetInt("ResolutionHeight", buildDefaultHeight);
            fullscreen = PlayerPrefs.GetInt("FullScreen", buildDefaultFullscreen ? 1 : 0) == 1;
            quality = PlayerPrefs.GetInt("QualityLevel", buildDefaultQuality);
            
            if (debugBuildSettings)
            {
                Debug.Log($"✅ Loaded settings from PlayerPrefs: {width}x{height}, Fullscreen: {fullscreen}, Quality: {quality}");
            }
        }
        else
        {
            // Sử dụng default settings và lưu vào PlayerPrefs
            width = buildDefaultWidth;
            height = buildDefaultHeight;
            fullscreen = buildDefaultFullscreen;
            quality = buildDefaultQuality;
            
            // Lưu default settings vào PlayerPrefs
            PlayerPrefs.SetInt("ResolutionWidth", width);
            PlayerPrefs.SetInt("ResolutionHeight", height);
            PlayerPrefs.SetInt("FullScreen", fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("QualityLevel", quality);
            PlayerPrefs.Save();
            
            if (debugBuildSettings)
            {
                Debug.Log($"💾 Created default PlayerPrefs: {width}x{height}, Fullscreen: {fullscreen}, Quality: {quality}");
            }
        }
        
        // Áp dụng settings ngay lập tức
        Screen.SetResolution(width, height, fullscreen);
        QualitySettings.SetQualityLevel(quality);
        
        if (debugBuildSettings)
        {
            Debug.Log($"🔧 Applied settings: {width}x{height}, Fullscreen: {fullscreen}, Quality: {quality}");
            Debug.Log($"📱 Current screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        }
        
        if (debugBuildSettings)
        {
            Debug.Log("================================================");
        }
    }
    
    /// <summary>
    /// Force apply settings ngay lập tức
    /// </summary>
    public virtual void ForceApplySettings()
    {
        Debug.Log("🔧 FORCING BUILD SETTINGS...");
        this.ApplyBuildSettings();
    }
    
    /// <summary>
    /// Reset về default settings
    /// </summary>
    public virtual void ResetToDefaultSettings()
    {
        PlayerPrefs.DeleteKey("ResolutionWidth");
        PlayerPrefs.DeleteKey("ResolutionHeight");
        PlayerPrefs.DeleteKey("FullScreen");
        PlayerPrefs.DeleteKey("QualityLevel");
        PlayerPrefs.Save();
        
        Debug.Log("🗑️ PlayerPrefs reset to default");
        this.ApplyBuildSettings();
    }
    
    [ContextMenu("Apply Build Settings")]
    protected virtual void ApplyBuildSettingsContext()
    {
        this.ApplyBuildSettings();
    }
    
    [ContextMenu("Force Apply Settings")]
    protected virtual void ForceApplySettingsContext()
    {
        this.ForceApplySettings();
    }
    
    [ContextMenu("Reset to Default")]
    protected virtual void ResetToDefaultContext()
    {
        this.ResetToDefaultSettings();
    }
    
    [ContextMenu("Show Current Settings")]
    protected virtual void ShowCurrentSettingsContext()
    {
        Debug.Log($"=== CURRENT BUILD SETTINGS ===");
        Debug.Log($"Screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        Debug.Log($"Quality: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        
        bool hasPlayerPrefs = PlayerPrefs.HasKey("ResolutionWidth");
        Debug.Log($"Has PlayerPrefs: {hasPlayerPrefs}");
        
        if (hasPlayerPrefs)
        {
            Debug.Log($"PlayerPrefs: {PlayerPrefs.GetInt("ResolutionWidth")}x{PlayerPrefs.GetInt("ResolutionHeight")}, Fullscreen: {PlayerPrefs.GetInt("FullScreen")}");
        }
        
        Debug.Log("==============================");
    }
}
