using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resDropDown;
    public TMP_Dropdown qualityDropDown; // Thêm quality dropdown
    public Toggle fullScreenToggle;

    Resolution[] allResolutions;
    bool isFullScreen;
    int selectedResolution;
    List<Resolution> selectedResolutionList = new List<Resolution>();
    
    // Thêm biến để lưu trạng thái pause trước khi mở setting
    private float previousTimeScale = 1f;

    void Start()
    {
        Debug.Log("=== SETTINGS MENU START ===");
        Debug.Log($"Current quality before LoadSettings: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        
        allResolutions = Screen.resolutions;

        // Khởi tạo resolution dropdown
        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution resolution in allResolutions)
        {
            newRes = resolution.width.ToString() +" x "+ resolution.height.ToString();
            if(!resolutionStringList.Contains(newRes) )
            {
                resolutionStringList.Add(newRes);
                selectedResolutionList.Add(resolution);
            }
        }
        resDropDown.AddOptions(resolutionStringList);
        
        // Load settings đã lưu hoặc dùng mặc định
        LoadSettings();
        Debug.Log($"Current quality after LoadSettings: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        
        // Khởi tạo quality dropdown
        InitializeQualityDropdown();
        Debug.Log($"Current quality after InitializeQualityDropdown: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        
        // Đảm bảo fullscreen mặc định được set
        this.EnsureDefaultFullscreen();
        
        // Lưu timeScale ban đầu
        previousTimeScale = Time.timeScale;
        
        Debug.Log("=== SETTINGS MENU START COMPLETE ===");
    }
    
    // Method để set resolution hiện tại trong dropdown
    void SetCurrentResolutionInDropdown()
    {
        if (resDropDown == null) return;
        
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;
        
        // Tìm index của resolution hiện tại trong danh sách
        for (int i = 0; i < selectedResolutionList.Count; i++)
        {
            if (selectedResolutionList[i].width == currentWidth && selectedResolutionList[i].height == currentHeight)
            {
                selectedResolution = i;
                resDropDown.value = i;
                resDropDown.RefreshShownValue();
                Debug.Log($"Current resolution set to: {currentWidth}x{currentHeight} (index: {i})");
                return;
            }
        }
        
        // Nếu không tìm thấy, set về 0 (resolution đầu tiên)
        selectedResolution = 0;
        resDropDown.value = 0;
        resDropDown.RefreshShownValue();
        Debug.LogWarning("Current resolution not found in list, set to first option");
    }
    
    // Thêm method khởi tạo quality dropdown
    void InitializeQualityDropdown()
    {
        if (qualityDropDown == null) return;
        
        // Xóa options cũ
        qualityDropDown.ClearOptions();
        
        // Lấy danh sách quality levels từ Unity
        string[] qualityNames = QualitySettings.names;
        List<string> qualityOptions = new List<string>();
        
        foreach (string qualityName in qualityNames)
        {
            qualityOptions.Add(qualityName);
        }
        
        // Thêm options vào dropdown
        qualityDropDown.AddOptions(qualityOptions);
        
        // Set giá trị từ PlayerPrefs (đã được load trong LoadSettings)
        int savedQuality = PlayerPrefs.GetInt("QualityLevel", 3);
        
        // Kiểm tra quality level có hợp lệ không
        if (savedQuality < 0 || savedQuality >= QualitySettings.names.Length)
        {
            savedQuality = 3; // Reset về High nếu không hợp lệ
            Debug.LogWarning($"Invalid quality level in InitializeQualityDropdown, reset to High (3)");
        }
        
        // Set giá trị và áp dụng quality
        qualityDropDown.value = savedQuality;
        qualityDropDown.RefreshShownValue();
        
        // Đảm bảo quality được áp dụng
        QualitySettings.SetQualityLevel(savedQuality);
        
        Debug.Log($"InitializeQualityDropdown: Set quality to {QualitySettings.names[savedQuality]} (index: {savedQuality})");
    }

    public void ChangeResolution()
    {
        selectedResolution = resDropDown.value;
        
        // Kiểm tra selectedResolution có hợp lệ không
        if (selectedResolution >= 0 && selectedResolution < selectedResolutionList.Count)
        {
            Screen.SetResolution(selectedResolutionList[selectedResolution].width, selectedResolutionList[selectedResolution].height, isFullScreen);
        }
        else
        {
            Debug.LogWarning($"Invalid resolution index: {selectedResolution}, using first available resolution");
            selectedResolution = 0;
            if (selectedResolutionList.Count > 0)
            {
                Screen.SetResolution(selectedResolutionList[0].width, selectedResolutionList[0].height, isFullScreen);
            }
        }
        
        // Save settings khi thay đổi resolution
        SaveSettings();
    }

    public void SetQuality(int qualityIndex)
    {
        // Kiểm tra index hợp lệ
        if (qualityIndex >= 0 && qualityIndex < QualitySettings.names.Length)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            Debug.Log($"Quality changed to: {QualitySettings.names[qualityIndex]}");
            // Save settings khi thay đổi quality
            SaveSettings();
        }
        else
        {
            Debug.LogWarning($"Invalid quality index: {qualityIndex}, available range: 0-{QualitySettings.names.Length - 1}");
        }
    }
    
    // Thêm method để set quality từ dropdown value
    public void OnQualityDropdownChanged()
    {
        if (qualityDropDown != null)
        {
            SetQuality(qualityDropDown.value);
        }
    }

    public void SetFullScreen()
    {
        Debug.Log("=== SET FULLSCREEN START ===");
        Debug.Log($"Fullscreen toggle value: {fullScreenToggle.isOn}");
        
        isFullScreen = fullScreenToggle.isOn;
        
        // Kiểm tra selectedResolution có hợp lệ không
        if (selectedResolution >= 0 && selectedResolution < selectedResolutionList.Count)
        {
            // Sử dụng FullScreenMode để tương thích với Player Settings
            if (isFullScreen)
            {
                Screen.SetResolution(selectedResolutionList[selectedResolution].width, selectedResolutionList[selectedResolution].height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.SetResolution(selectedResolutionList[selectedResolution].width, selectedResolutionList[selectedResolution].height, FullScreenMode.Windowed);
            }
            Debug.Log($"Applied resolution: {selectedResolutionList[selectedResolution].width}x{selectedResolutionList[selectedResolution].height}, Fullscreen: {isFullScreen} (Mode: {(isFullScreen ? "FullScreenWindow" : "Windowed")})");
        }
        else
        {
            Debug.LogWarning($"Invalid resolution index in SetFullScreen: {selectedResolution}, using current screen resolution");
            if (isFullScreen)
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
            }
            Debug.Log($"Applied current resolution: {Screen.width}x{Screen.height}, Fullscreen: {isFullScreen} (Mode: {(isFullScreen ? "FullScreenWindow" : "Windowed")})");
        }
        
        // Save settings khi thay đổi fullscreen
        Debug.Log("Saving settings...");
        SaveSettings();
        Debug.Log("=== SET FULLSCREEN COMPLETE ===");
    }
    
    // Method để pause game khi mở setting
    public void PauseGame()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Debug.Log("Game paused - Settings opened");
    }
    
    // Method để resume game khi đóng setting
    public void ResumeGame()
    {
        Time.timeScale = previousTimeScale;
        Debug.Log("Game resumed - Settings closed");
    }
    
    // Method để toggle pause state
    public void TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    // Method để save settings vào PlayerPrefs
    public void SaveSettings()
    {
        Debug.Log("=== SAVE SETTINGS START ===");
        Debug.Log($"Current settings: Resolution={selectedResolution}, Fullscreen={isFullScreen}, Quality={QualitySettings.GetQualityLevel()}");
        
        // Kiểm tra selectedResolution có hợp lệ không
        int width, height;
        if (selectedResolution >= 0 && selectedResolution < selectedResolutionList.Count)
        {
            width = selectedResolutionList[selectedResolution].width;
            height = selectedResolutionList[selectedResolution].height;
            PlayerPrefs.SetInt("ResolutionIndex", selectedResolution);
            Debug.Log($"Using selected resolution: {width}x{height}");
        }
        else
        {
            // Nếu không hợp lệ, dùng resolution hiện tại của screen
            width = Screen.width;
            height = Screen.height;
            PlayerPrefs.SetInt("ResolutionIndex", 0);
            Debug.LogWarning($"Invalid selectedResolution ({selectedResolution}), using current screen resolution: {width}x{height}");
        }
        
        // Save resolution
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        Debug.Log($"Saved resolution: {width}x{height}");
        
        // Save fullscreen
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        Debug.Log($"Saved fullscreen: {isFullScreen}");
        
        // Save quality
        int quality = QualitySettings.GetQualityLevel();
        PlayerPrefs.SetInt("QualityLevel", quality);
        Debug.Log($"Saved quality: {quality} ({QualitySettings.names[quality]})");
        
        // Lưu settings
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs.Save() called");
        
        // Sử dụng GlobalSettingsManager để đồng bộ settings
        if (GlobalSettingsManager.Instance != null)
        {
            Debug.Log("Calling GlobalSettingsManager.SaveAndApplySettings...");
            GlobalSettingsManager.Instance.SaveAndApplySettings(width, height, isFullScreen, quality);
        }
        else
        {
            Debug.LogWarning("GlobalSettingsManager.Instance is null!");
        }
        
        Debug.Log("=== SAVE SETTINGS COMPLETE ===");
    }
    
    // Method để load settings từ PlayerPrefs
    public void LoadSettings()
    {
        Debug.Log("=== LOAD SETTINGS START ===");
        Debug.Log($"Quality before GlobalSettingsManager: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        
        // Sử dụng GlobalSettingsManager để đồng bộ settings
        if (GlobalSettingsManager.Instance != null)
        {
            GlobalSettingsManager.Instance.ApplyGlobalSettings();
            Debug.Log($"Quality after GlobalSettingsManager: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        }
        else
        {
            Debug.LogWarning("GlobalSettingsManager.Instance is null!");
        }
        
        // Load resolution (mặc định 1920x1080 nếu chưa có)
        int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        selectedResolution = PlayerPrefs.GetInt("ResolutionIndex", 0);
        
        // Load fullscreen (mặc định true nếu chưa có)
        isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        
        // Force set fullscreen mặc định là true
        if (!isFullScreen)
        {
            Debug.Log("Force setting fullscreen to true (default)");
            isFullScreen = true;
            PlayerPrefs.SetInt("FullScreen", 1);
            PlayerPrefs.Save();
        }
        
        // Load quality (mặc định 3 - High nếu chưa có)
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", 3);
        Debug.Log($"QualityLevel from PlayerPrefs: {qualityLevel}");
        
        // Kiểm tra quality level có hợp lệ không
        if (qualityLevel < 0 || qualityLevel >= QualitySettings.names.Length)
        {
            qualityLevel = 3; // Reset về High nếu không hợp lệ
            Debug.LogWarning($"Invalid quality level loaded, reset to High (3)");
        }
        
        // Set UI elements
        if (resDropDown != null)
        {
            // Tìm resolution trong danh sách
            for (int i = 0; i < selectedResolutionList.Count; i++)
            {
                if (selectedResolutionList[i].width == width && selectedResolutionList[i].height == height)
                {
                    selectedResolution = i;
                    resDropDown.value = i;
                    resDropDown.RefreshShownValue();
                    break;
                }
            }
        }
        
        if (fullScreenToggle != null)
        {
            // Tạm thời disable OnValueChanged để tránh trigger SetFullScreen khi load
            var onValueChanged = fullScreenToggle.onValueChanged;
            fullScreenToggle.onValueChanged = new Toggle.ToggleEvent();
            
            // Set giá trị
            fullScreenToggle.isOn = isFullScreen;
            
            // Khôi phục OnValueChanged
            fullScreenToggle.onValueChanged = onValueChanged;
            
            Debug.Log($"Fullscreen toggle set to: {isFullScreen}");
        }
        else
        {
            Debug.LogWarning("fullScreenToggle is null!");
        }
        
        if (qualityDropDown != null)
        {
            qualityDropDown.value = qualityLevel;
            qualityDropDown.RefreshShownValue();
        }
        
        Debug.Log($"Settings loaded: {width}x{height}, Fullscreen: {isFullScreen}, Quality: {QualitySettings.names[qualityLevel]}");
        Debug.Log("=== LOAD SETTINGS COMPLETE ===");
    }
    
    // Method để save settings khi thoát game
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveSettings();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveSettings();
        }
    }
    
    void OnDestroy()
    {
        SaveSettings();
    }
    
    /// <summary>
    /// Force load settings và update UI
    /// </summary>
    [ContextMenu("Force Load Settings")]
    public virtual void ForceLoadSettings()
    {
        Debug.Log("🔧 FORCING SETTINGS LOAD...");
        this.LoadSettings();
    }
    
    /// <summary>
    /// Force update fullscreen toggle
    /// </summary>
    [ContextMenu("Force Update Fullscreen Toggle")]
    public virtual void ForceUpdateFullscreenToggle()
    {
        Debug.Log("🔧 FORCING FULLSCREEN TOGGLE UPDATE...");
        
        if (fullScreenToggle != null)
        {
            bool savedFullscreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
            
            // Tạm thời disable OnValueChanged
            var onValueChanged = fullScreenToggle.onValueChanged;
            fullScreenToggle.onValueChanged = new Toggle.ToggleEvent();
            
            // Set giá trị
            fullScreenToggle.isOn = savedFullscreen;
            
            // Khôi phục OnValueChanged
            fullScreenToggle.onValueChanged = onValueChanged;
            
            Debug.Log($"Fullscreen toggle force updated to: {savedFullscreen}");
        }
        else
        {
            Debug.LogWarning("fullScreenToggle is null!");
        }
    }
    
    /// <summary>
    /// Force set fullscreen mặc định là true
    /// </summary>
    [ContextMenu("Force Set Default Fullscreen")]
    public virtual void ForceSetDefaultFullscreen()
    {
        Debug.Log("🔧 FORCING DEFAULT FULLSCREEN...");
        
        // Set fullscreen mặc định là true
        isFullScreen = true;
        PlayerPrefs.SetInt("FullScreen", 1);
        PlayerPrefs.Save();
        
        // Update toggle UI
        if (fullScreenToggle != null)
        {
            // Tạm thời disable OnValueChanged
            var onValueChanged = fullScreenToggle.onValueChanged;
            fullScreenToggle.onValueChanged = new Toggle.ToggleEvent();
            
            // Set giá trị
            fullScreenToggle.isOn = true;
            
            // Khôi phục OnValueChanged
            fullScreenToggle.onValueChanged = onValueChanged;
            
            Debug.Log("Fullscreen toggle set to true (default)");
        }
        
        // Apply fullscreen settings
        this.SetFullScreen();
        
        Debug.Log("✅ Default fullscreen applied!");
    }
    
    /// <summary>
    /// Đảm bảo fullscreen mặc định được set
    /// </summary>
    protected virtual void EnsureDefaultFullscreen()
    {
        try
        {
            Debug.Log("=== ENSURING DEFAULT FULLSCREEN ===");
            
            // Kiểm tra PlayerPrefs
            int savedFullscreen = PlayerPrefs.GetInt("FullScreen", -1);
            Debug.Log($"Saved fullscreen from PlayerPrefs: {savedFullscreen}");
            
            // Nếu chưa có setting hoặc là false, set mặc định là true
            if (savedFullscreen != 1)
            {
                Debug.Log("Setting default fullscreen to true...");
                
                // Set fullscreen mặc định
                isFullScreen = true;
                PlayerPrefs.SetInt("FullScreen", 1);
                PlayerPrefs.Save();
                
                // Update toggle UI
                if (fullScreenToggle != null)
                {
                    // Tạm thời disable OnValueChanged
                    var onValueChanged = fullScreenToggle.onValueChanged;
                    fullScreenToggle.onValueChanged = new Toggle.ToggleEvent();
                    
                    // Set giá trị
                    fullScreenToggle.isOn = true;
                    
                    // Khôi phục OnValueChanged
                    fullScreenToggle.onValueChanged = onValueChanged;
                    
                    Debug.Log("Fullscreen toggle set to true (default)");
                }
                
                // Apply fullscreen settings
                this.SetFullScreen();
                
                Debug.Log("✅ Default fullscreen ensured!");
            }
            else
            {
                Debug.Log("✅ Fullscreen already set to true");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong EnsureDefaultFullscreen: {e.Message}");
        }
    }
}
