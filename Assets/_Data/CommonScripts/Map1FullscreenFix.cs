using UnityEngine;

/// <summary>
/// Script đơn giản để force áp dụng fullscreen khi vào scene Hai_Map
/// </summary>
public class Map1FullscreenFix : MonoBehaviour
{
    [Header("Fullscreen Fix Settings")]
    [SerializeField] protected bool autoFixOnStart = true;
    [SerializeField] protected bool debugFixInfo = true;
    [SerializeField] protected float delayBeforeFix = 0.1f;
    
    protected virtual void Start()
    {
        if (autoFixOnStart)
        {
            // Delay một chút để đảm bảo scene đã load xong
            Invoke(nameof(this.FixFullscreen), delayBeforeFix);
        }
    }
    
    /// <summary>
    /// Fix fullscreen cho Map 1
    /// </summary>
    public virtual void FixFullscreen()
    {
        try
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            if (debugFixInfo)
            {
                Debug.Log($"=== MAP1 FULLSCREEN FIX - {currentSceneName} ===");
            }
            
            // Chỉ áp dụng cho scene Hai_Map
            if (currentSceneName == "Hai_Map")
            {
                // Load settings từ PlayerPrefs
                int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
                int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
                bool isFullscreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
                
                if (debugFixInfo)
                {
                    Debug.Log($"Loaded settings: {width}x{height}, Fullscreen: {isFullscreen}");
                    Debug.Log($"Current screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
                }
                
                // Force áp dụng fullscreen settings
                Screen.SetResolution(width, height, isFullscreen);
                
                if (debugFixInfo)
                {
                    Debug.Log($"✅ FORCE APPLIED FULLSCREEN: {width}x{height}, Fullscreen: {isFullscreen}");
                    Debug.Log($"New screen: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
                }
            }
            else
            {
                if (debugFixInfo)
                {
                    Debug.Log($"Not Hai_Map scene ({currentSceneName}), skipping fullscreen fix");
                }
            }
            
            if (debugFixInfo)
            {
                Debug.Log("==========================================");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong FixFullscreen: {e.Message}");
        }
    }
    
    /// <summary>
    /// Test method để kiểm tra fullscreen
    /// </summary>
    [ContextMenu("Test Fullscreen Fix")]
    public virtual void TestFullscreenFix()
    {
        Debug.Log("=== TESTING FULLSCREEN FIX ===");
        this.FixFullscreen();
        Debug.Log("=== TEST COMPLETE ===");
    }
}
