using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script để fix toggle fullscreen trong scene Hai_Map
/// </summary>
public class HaiMapToggleFixer : MonoBehaviour
{
    [Header("Toggle Fix Settings")]
    [SerializeField] protected bool autoFixOnStart = true;
    [SerializeField] protected bool debugFixInfo = true;
    [SerializeField] protected float fixDelay = 0.2f;
    
    protected virtual void Start()
    {
        if (autoFixOnStart)
        {
            // Delay để đảm bảo SettingsMenu đã load xong
            Invoke(nameof(this.FixFullscreenToggle), fixDelay);
        }
    }
    
    /// <summary>
    /// Fix fullscreen toggle trong scene Hai_Map
    /// </summary>
    public virtual void FixFullscreenToggle()
    {
        try
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            
            if (debugFixInfo)
            {
                Debug.Log($"=== HAI MAP TOGGLE FIXER - {currentSceneName} ===");
            }
            
            // Chỉ áp dụng cho scene Hai_Map
            if (currentSceneName == "Hai_Map")
            {
                // Tìm SettingsMenu trong scene
                SettingsMenu settingsMenu = FindObjectOfType<SettingsMenu>();
                if (settingsMenu == null)
                {
                    if (debugFixInfo)
                    {
                        Debug.LogWarning("❌ SettingsMenu not found in Hai_Map!");
                    }
                    return;
                }
                
                if (debugFixInfo)
                {
                    Debug.Log("✅ SettingsMenu found in Hai_Map");
                }
                
                // Tìm fullScreenToggle
                Toggle fullScreenToggle = settingsMenu.fullScreenToggle;
                if (fullScreenToggle == null)
                {
                    if (debugFixInfo)
                    {
                        Debug.LogWarning("❌ fullScreenToggle is null!");
                    }
                    return;
                }
                
                if (debugFixInfo)
                {
                    Debug.Log("✅ fullScreenToggle found");
                }
                
                // Load settings từ PlayerPrefs
                bool savedFullscreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
                
                if (debugFixInfo)
                {
                    Debug.Log($"Saved fullscreen from PlayerPrefs: {savedFullscreen}");
                    Debug.Log($"Current toggle value: {fullScreenToggle.isOn}");
                    Debug.Log($"Current screen fullscreen: {Screen.fullScreen}");
                }
                
                // Kiểm tra xem toggle có đúng giá trị không
                if (fullScreenToggle.isOn != savedFullscreen)
                {
                    if (debugFixInfo)
                    {
                        Debug.Log($"⚠️ Toggle mismatch! Fixing toggle from {fullScreenToggle.isOn} to {savedFullscreen}");
                    }
                    
                    // Tạm thời disable OnValueChanged để tránh trigger SetFullScreen
                    var onValueChanged = fullScreenToggle.onValueChanged;
                    fullScreenToggle.onValueChanged = new Toggle.ToggleEvent();
                    
                    // Set giá trị đúng
                    fullScreenToggle.isOn = savedFullscreen;
                    
                    // Khôi phục OnValueChanged
                    fullScreenToggle.onValueChanged = onValueChanged;
                    
                    if (debugFixInfo)
                    {
                        Debug.Log($"✅ Toggle fixed to: {fullScreenToggle.isOn}");
                    }
                }
                else
                {
                    if (debugFixInfo)
                    {
                        Debug.Log("✅ Toggle already has correct value");
                    }
                }
                
                // Force load settings một lần nữa để đảm bảo đồng bộ
                if (settingsMenu != null)
                {
                    if (debugFixInfo)
                    {
                        Debug.Log("Force calling SettingsMenu.LoadSettings()...");
                    }
                    settingsMenu.LoadSettings();
                }
            }
            else
            {
                if (debugFixInfo)
                {
                    Debug.Log($"Not Hai_Map scene ({currentSceneName}), skipping toggle fix");
                }
            }
            
            if (debugFixInfo)
            {
                Debug.Log("=== HAI MAP TOGGLE FIXER COMPLETE ===");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong FixFullscreenToggle: {e.Message}");
        }
    }
    
    /// <summary>
    /// Test method để kiểm tra toggle
    /// </summary>
    [ContextMenu("Test Toggle Fix")]
    public virtual void TestToggleFix()
    {
        Debug.Log("=== TESTING TOGGLE FIX ===");
        this.FixFullscreenToggle();
        Debug.Log("=== TEST COMPLETE ===");
    }
    
    /// <summary>
    /// Show current toggle state
    /// </summary>
    [ContextMenu("Show Current Toggle State")]
    public virtual void ShowCurrentToggleState()
    {
        Debug.Log("=== CURRENT TOGGLE STATE ===");
        Debug.Log($"Scene: {SceneManager.GetActiveScene().name}");
        
        SettingsMenu settingsMenu = FindObjectOfType<SettingsMenu>();
        if (settingsMenu != null)
        {
            Debug.Log("✅ SettingsMenu: EXISTS");
            
            Toggle fullScreenToggle = settingsMenu.fullScreenToggle;
            if (fullScreenToggle != null)
            {
                Debug.Log($"✅ fullScreenToggle: EXISTS");
                Debug.Log($"Toggle value: {fullScreenToggle.isOn}");
                Debug.Log($"Toggle interactable: {fullScreenToggle.interactable}");
            }
            else
            {
                Debug.LogWarning("❌ fullScreenToggle: NULL");
            }
        }
        else
        {
            Debug.LogWarning("❌ SettingsMenu: NOT FOUND");
        }
        
        Debug.Log($"PlayerPrefs FullScreen: {PlayerPrefs.GetInt("FullScreen", -1)}");
        Debug.Log($"Screen.fullScreen: {Screen.fullScreen}");
        Debug.Log("================================");
    }
}
