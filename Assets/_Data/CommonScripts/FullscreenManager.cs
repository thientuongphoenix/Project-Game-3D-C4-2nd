using UnityEngine;

/// <summary>
/// Manager để đảm bảo fullscreen settings được áp dụng đúng khi vào scene
/// </summary>
public class FullscreenManager : SaiSingleton<FullscreenManager>
{
    [Header("Fullscreen Settings")]
    [SerializeField] protected bool forceFullscreenOnStart = true;
    [SerializeField] protected bool debugFullscreenInfo = true;
    
    protected virtual void Start()
    {
        this.ApplyFullscreenSettings();
    }
    
    /// <summary>
    /// Áp dụng fullscreen settings từ PlayerPrefs
    /// </summary>
    protected virtual void ApplyFullscreenSettings()
    {
        if (debugFullscreenInfo)
        {
            Debug.Log($"=== FULLSCREEN MANAGER - {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} ===");
        }
        
        // Load settings từ PlayerPrefs
        int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        bool isFullscreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        
        if (debugFullscreenInfo)
        {
            Debug.Log($"Loaded settings: {width}x{height}, Fullscreen: {isFullscreen}");
            Debug.Log($"Current screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        }
        
        // Kiểm tra xem có cần áp dụng settings không
        bool needsUpdate = false;
        
        if (Screen.width != width || Screen.height != height)
        {
            needsUpdate = true;
            if (debugFullscreenInfo)
            {
                Debug.Log($"Resolution mismatch: Current {Screen.width}x{Screen.height} vs Saved {width}x{height}");
            }
        }
        
        if (Screen.fullScreen != isFullscreen)
        {
            needsUpdate = true;
            if (debugFullscreenInfo)
            {
                Debug.Log($"Fullscreen mismatch: Current {Screen.fullScreen} vs Saved {isFullscreen}");
            }
        }
        
        // Áp dụng settings nếu cần
        if (needsUpdate || forceFullscreenOnStart)
        {
            Screen.SetResolution(width, height, isFullscreen);
            
            if (debugFullscreenInfo)
            {
                Debug.Log($"✅ Applied settings: {width}x{height}, Fullscreen: {isFullscreen}");
            }
        }
        else
        {
            if (debugFullscreenInfo)
            {
                Debug.Log("✅ Settings already applied correctly");
            }
        }
        
        if (debugFullscreenInfo)
        {
            Debug.Log("==========================================");
        }
    }
    
    /// <summary>
    /// Force fullscreen ngay lập tức
    /// </summary>
    public virtual void ForceFullscreen()
    {
        int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        
        Screen.SetResolution(width, height, true);
        PlayerPrefs.SetInt("FullScreen", 1);
        PlayerPrefs.Save();
        
        Debug.Log($"🔧 FORCED FULLSCREEN: {width}x{height}");
    }
    
    /// <summary>
    /// Force windowed mode ngay lập tức
    /// </summary>
    public virtual void ForceWindowed()
    {
        int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        
        Screen.SetResolution(width, height, false);
        PlayerPrefs.SetInt("FullScreen", 0);
        PlayerPrefs.Save();
        
        Debug.Log($"🔧 FORCED WINDOWED: {width}x{height}");
    }
    
    [ContextMenu("Force Fullscreen")]
    protected virtual void ForceFullscreenContext()
    {
        this.ForceFullscreen();
    }
    
    [ContextMenu("Force Windowed")]
    protected virtual void ForceWindowedContext()
    {
        this.ForceWindowed();
    }
    
    [ContextMenu("Show Fullscreen Info")]
    protected virtual void ShowFullscreenInfoContext()
    {
        Debug.Log($"=== FULLSCREEN INFO ===");
        Debug.Log($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"Screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        Debug.Log($"PlayerPrefs: {PlayerPrefs.GetInt("ResolutionWidth", -1)}x{PlayerPrefs.GetInt("ResolutionHeight", -1)}, Fullscreen: {PlayerPrefs.GetInt("FullScreen", -1)}");
        Debug.Log("======================");
    }
}
