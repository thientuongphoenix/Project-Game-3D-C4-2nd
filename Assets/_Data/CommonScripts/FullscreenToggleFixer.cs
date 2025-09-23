using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script để sửa lỗi fullscreen toggle bị untick khi chuyển scene
/// </summary>
public class FullscreenToggleFixer : MonoBehaviour
{
    [Header("Fullscreen Toggle Fix")]
    [SerializeField] protected bool fixOnStart = true;
    [SerializeField] protected bool fixOnEnable = true;
    [SerializeField] protected bool debugToggleFix = true;
    [SerializeField] protected float fixDelay = 0.1f;
    
    protected virtual void Start()
    {
        if (fixOnStart)
        {
            this.FixFullscreenToggle();
        }
    }
    
    protected virtual void OnEnable()
    {
        if (fixOnEnable)
        {
            this.FixFullscreenToggle();
        }
    }
    
    /// <summary>
    /// Sửa lỗi fullscreen toggle
    /// </summary>
    protected virtual void FixFullscreenToggle()
    {
        if (debugToggleFix)
        {
            Debug.Log($"=== FULLSCREEN TOGGLE FIXER - {SceneManager.GetActiveScene().name} ===");
        }
        
        // Tìm SettingsMenu trong scene
        SettingsMenu settingsMenu = FindObjectOfType<SettingsMenu>();
        if (settingsMenu == null)
        {
            if (debugToggleFix)
            {
                Debug.LogWarning("❌ SettingsMenu not found in scene!");
            }
            return;
        }
        
        if (debugToggleFix)
        {
            Debug.Log("✅ SettingsMenu found");
        }
        
        // Tìm fullScreenToggle
        Toggle fullScreenToggle = settingsMenu.fullScreenToggle;
        if (fullScreenToggle == null)
        {
            if (debugToggleFix)
            {
                Debug.LogWarning("❌ fullScreenToggle is null!");
            }
            return;
        }
        
        if (debugToggleFix)
        {
            Debug.Log("✅ fullScreenToggle found");
        }
        
        // Load settings từ PlayerPrefs
        bool savedFullscreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        
        if (debugToggleFix)
        {
            Debug.Log($"Saved fullscreen from PlayerPrefs: {savedFullscreen}");
            Debug.Log($"Current toggle value: {fullScreenToggle.isOn}");
        }
        
        // Kiểm tra xem toggle có đúng giá trị không
        if (fullScreenToggle.isOn != savedFullscreen)
        {
            if (debugToggleFix)
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
            
            if (debugToggleFix)
            {
                Debug.Log($"✅ Toggle fixed to: {fullScreenToggle.isOn}");
            }
        }
        else
        {
            if (debugToggleFix)
            {
                Debug.Log("✅ Toggle already has correct value");
            }
        }
        
        // Đảm bảo settings được load đúng cách
        if (settingsMenu != null)
        {
            if (debugToggleFix)
            {
                Debug.Log("Calling SettingsMenu.LoadSettings()...");
            }
            settingsMenu.LoadSettings();
        }
        
        if (debugToggleFix)
        {
            Debug.Log("=== FULLSCREEN TOGGLE FIXER COMPLETE ===");
        }
    }
    
    /// <summary>
    /// Fix với delay để đảm bảo UI đã load xong
    /// </summary>
    protected virtual void FixFullscreenToggleWithDelay()
    {
        StartCoroutine(FixFullscreenToggleCoroutine());
    }
    
    /// <summary>
    /// Coroutine để fix với delay
    /// </summary>
    protected virtual System.Collections.IEnumerator FixFullscreenToggleCoroutine()
    {
        yield return new WaitForSeconds(fixDelay);
        this.FixFullscreenToggle();
    }
    
    /// <summary>
    /// Context menu để force fix toggle
    /// </summary>
    [ContextMenu("Force Fix Fullscreen Toggle")]
    protected virtual void ForceFixFullscreenToggleContext()
    {
        this.FixFullscreenToggle();
    }
    
    /// <summary>
    /// Context menu để fix với delay
    /// </summary>
    [ContextMenu("Fix Fullscreen Toggle With Delay")]
    protected virtual void FixFullscreenToggleWithDelayContext()
    {
        this.FixFullscreenToggleWithDelay();
    }
    
    /// <summary>
    /// Context menu để show current toggle state
    /// </summary>
    [ContextMenu("Show Current Toggle State")]
    protected virtual void ShowCurrentToggleStateContext()
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
