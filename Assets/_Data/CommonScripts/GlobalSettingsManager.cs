using UnityEngine;

/// <summary>
/// Global Settings Manager để đảm bảo settings được đồng bộ giữa tất cả scene
/// </summary>
public class GlobalSettingsManager : SaiSingleton<GlobalSettingsManager>
{
    [Header("Settings Synchronization")]
    [SerializeField] protected bool autoApplySettingsOnStart = true;
    [SerializeField] protected bool debugSettingsSync = true;
    
    [Header("Default Settings")]
    [SerializeField] protected int defaultWidth = 1920;
    [SerializeField] protected int defaultHeight = 1080;
    [SerializeField] protected bool defaultFullscreen = true;
    [SerializeField] protected int defaultQuality = 3;
    
    protected override void Start()
    {
        base.Start();
        
        if (autoApplySettingsOnStart)
        {
            this.ApplyGlobalSettings();
        }
    }
    
    protected override void Awake()
    {
        base.Awake();
        
        // Đảm bảo GlobalSettingsManager tồn tại xuyên suốt các scene
        DontDestroyOnLoad(gameObject);
        
        if (debugSettingsSync)
        {
            Debug.Log("GlobalSettingsManager: Set to DontDestroyOnLoad");
        }
    }
    
    /// <summary>
    /// Áp dụng settings từ PlayerPrefs cho tất cả scene
    /// </summary>
    public virtual void ApplyGlobalSettings()
    {
        if (debugSettingsSync)
        {
            Debug.Log($"=== GLOBAL SETTINGS MANAGER - {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} ===");
        }
        
        // Load settings từ PlayerPrefs
        int width = PlayerPrefs.GetInt("ResolutionWidth", defaultWidth);
        int height = PlayerPrefs.GetInt("ResolutionHeight", defaultHeight);
        bool isFullscreen = PlayerPrefs.GetInt("FullScreen", defaultFullscreen ? 1 : 0) == 1;
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", defaultQuality);
        
        if (debugSettingsSync)
        {
            Debug.Log($"Loaded settings: {width}x{height}, Fullscreen: {isFullscreen}, Quality: {qualityLevel}");
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
                // Sử dụng FullScreenWindowMode để tương thích với Player Settings
                if (isFullscreen)
                {
                    Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
                }
                else
                {
                    Screen.SetResolution(width, height, FullScreenMode.Windowed);
                }
                
                if (debugSettingsSync)
                {
                    Debug.Log($"✅ Applied resolution: {width}x{height}, Fullscreen: {isFullscreen} (Mode: {(isFullscreen ? "FullScreenWindow" : "Windowed")})");
                }
            }
            
            // Áp dụng quality
            if (needsQualityUpdate)
            {
                QualitySettings.SetQualityLevel(qualityLevel);
                
                if (debugSettingsSync)
                {
                    Debug.Log($"✅ Applied quality: {QualitySettings.names[qualityLevel]}");
                }
            }
        }
        else
        {
            if (debugSettingsSync)
            {
                Debug.Log("✅ Settings already synchronized");
            }
        }
        
        if (debugSettingsSync)
        {
            Debug.Log("================================================");
        }
    }
    
    /// <summary>
    /// Force sync settings ngay lập tức
    /// </summary>
    public virtual void ForceSyncSettings()
    {
        Debug.Log("🔧 FORCING SETTINGS SYNC...");
        this.ApplyGlobalSettings();
    }
    
    /// <summary>
    /// Force apply settings với delay để đảm bảo scene đã load xong
    /// </summary>
    public virtual void ForceApplySettingsWithDelay(float delay = 0.1f)
    {
        StartCoroutine(ForceApplySettingsCoroutine(delay));
    }
    
    /// <summary>
    /// Coroutine để force apply settings với delay
    /// </summary>
    protected virtual System.Collections.IEnumerator ForceApplySettingsCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (debugSettingsSync)
        {
            Debug.Log($"🔧 FORCING SETTINGS SYNC WITH DELAY ({delay}s)...");
        }
        
        this.ApplyGlobalSettings();
    }
    
    /// <summary>
    /// Save settings và áp dụng ngay lập tức
    /// </summary>
    public virtual void SaveAndApplySettings(int width, int height, bool fullscreen, int quality)
    {
        // Save to PlayerPrefs
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        PlayerPrefs.SetInt("FullScreen", fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("QualityLevel", quality);
        PlayerPrefs.Save();
        
        // Apply immediately
        Screen.SetResolution(width, height, fullscreen);
        QualitySettings.SetQualityLevel(quality);
        
        Debug.Log($"💾 Settings saved and applied: {width}x{height}, Fullscreen: {fullscreen}, Quality: {quality}");
    }
    
    /// <summary>
    /// Get current settings từ PlayerPrefs
    /// </summary>
    public virtual (int width, int height, bool fullscreen, int quality) GetCurrentSettings()
    {
        int width = PlayerPrefs.GetInt("ResolutionWidth", defaultWidth);
        int height = PlayerPrefs.GetInt("ResolutionHeight", defaultHeight);
        bool fullscreen = PlayerPrefs.GetInt("FullScreen", defaultFullscreen ? 1 : 0) == 1;
        int quality = PlayerPrefs.GetInt("QualityLevel", defaultQuality);
        
        return (width, height, fullscreen, quality);
    }
    
    [ContextMenu("Apply Global Settings")]
    protected virtual void ApplyGlobalSettingsContext()
    {
        this.ApplyGlobalSettings();
    }
    
    [ContextMenu("Force Sync Settings")]
    protected virtual void ForceSyncSettingsContext()
    {
        this.ForceSyncSettings();
    }
    
    [ContextMenu("Show Current Settings")]
    protected virtual void ShowCurrentSettingsContext()
    {
        var (width, height, fullscreen, quality) = this.GetCurrentSettings();
        
        Debug.Log($"=== CURRENT SETTINGS ===");
        Debug.Log($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"PlayerPrefs: {width}x{height}, Fullscreen: {fullscreen}, Quality: {quality}");
        Debug.Log($"Screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        Debug.Log($"Quality: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        Debug.Log("========================");
    }
}
