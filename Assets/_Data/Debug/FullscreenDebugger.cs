using UnityEngine;

/// <summary>
/// Script để debug fullscreen settings khi vào scene
/// </summary>
public class FullscreenDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] protected bool showDebugInfo = true;
    [SerializeField] protected bool forceFullscreenOnStart = false;
    
    protected virtual void Start()
    {
        this.ShowFullscreenInfo();
        
        if (forceFullscreenOnStart)
        {
            this.ForceFullscreen();
        }
    }
    
    protected virtual void ShowFullscreenInfo()
    {
        if (!showDebugInfo) return;
        
        Debug.Log("=== FULLSCREEN DEBUG INFO ===");
        Debug.Log($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"Screen.width: {Screen.width}");
        Debug.Log($"Screen.height: {Screen.height}");
        Debug.Log($"Screen.fullScreen: {Screen.fullScreen}");
        Debug.Log($"Screen.fullScreenMode: {Screen.fullScreenMode}");
        
        // Kiểm tra PlayerPrefs
        int fullscreenPref = PlayerPrefs.GetInt("FullScreen", -1);
        int widthPref = PlayerPrefs.GetInt("ResolutionWidth", -1);
        int heightPref = PlayerPrefs.GetInt("ResolutionHeight", -1);
        
        Debug.Log($"PlayerPrefs FullScreen: {fullscreenPref} (1=fullscreen, 0=windowed, -1=not set)");
        Debug.Log($"PlayerPrefs Resolution: {widthPref}x{heightPref}");
        
        // Kiểm tra Project Settings
        Debug.Log($"Project Settings fullscreenMode: {UnityEngine.QualitySettings.GetQualityLevel()}");
        
        Debug.Log("=============================");
    }
    
    protected virtual void ForceFullscreen()
    {
        Debug.Log("🔧 FORCING FULLSCREEN...");
        
        // Lấy resolution từ PlayerPrefs hoặc dùng mặc định
        int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        
        // Force fullscreen
        Screen.SetResolution(width, height, true);
        
        Debug.Log($"✅ Forced fullscreen: {width}x{height}");
        
        // Cập nhật PlayerPrefs
        PlayerPrefs.SetInt("FullScreen", 1);
        PlayerPrefs.Save();
        
        Debug.Log("✅ PlayerPrefs updated to fullscreen");
    }
    
    [ContextMenu("Show Fullscreen Info")]
    protected virtual void ShowFullscreenInfoContext()
    {
        this.ShowFullscreenInfo();
    }
    
    [ContextMenu("Force Fullscreen")]
    protected virtual void ForceFullscreenContext()
    {
        this.ForceFullscreen();
    }
    
    protected virtual void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 300, 300, 150));
        GUILayout.Label("=== FULLSCREEN DEBUG ===");
        GUILayout.Label($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        GUILayout.Label($"Resolution: {Screen.width}x{Screen.height}");
        GUILayout.Label($"Fullscreen: {Screen.fullScreen}");
        GUILayout.Label($"FullscreenMode: {Screen.fullScreenMode}");
        
        int fullscreenPref = PlayerPrefs.GetInt("FullScreen", -1);
        GUILayout.Label($"PlayerPrefs: {fullscreenPref}");
        
        if (GUILayout.Button("Force Fullscreen"))
        {
            this.ForceFullscreen();
        }
        
        GUILayout.EndArea();
    }
}
