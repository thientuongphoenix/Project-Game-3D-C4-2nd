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
        
        // Khởi tạo quality dropdown
        InitializeQualityDropdown();
        
        // Lưu timeScale ban đầu
        previousTimeScale = Time.timeScale;
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
        
        // Set giá trị hiện tại
        qualityDropDown.value = QualitySettings.GetQualityLevel();
        qualityDropDown.RefreshShownValue();
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
        isFullScreen = fullScreenToggle.isOn;
        
        // Kiểm tra selectedResolution có hợp lệ không
        if (selectedResolution >= 0 && selectedResolution < selectedResolutionList.Count)
        {
            Screen.SetResolution(selectedResolutionList[selectedResolution].width, selectedResolutionList[selectedResolution].height, isFullScreen);
        }
        else
        {
            Debug.LogWarning($"Invalid resolution index in SetFullScreen: {selectedResolution}, using current screen resolution");
            Screen.SetResolution(Screen.width, Screen.height, isFullScreen);
        }
        
        // Save settings khi thay đổi fullscreen
        SaveSettings();
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
        // Kiểm tra selectedResolution có hợp lệ không
        if (selectedResolution >= 0 && selectedResolution < selectedResolutionList.Count)
        {
            // Save resolution
            PlayerPrefs.SetInt("ResolutionWidth", selectedResolutionList[selectedResolution].width);
            PlayerPrefs.SetInt("ResolutionHeight", selectedResolutionList[selectedResolution].height);
            PlayerPrefs.SetInt("ResolutionIndex", selectedResolution);
        }
        else
        {
            // Nếu không hợp lệ, dùng resolution hiện tại của screen
            PlayerPrefs.SetInt("ResolutionWidth", Screen.width);
            PlayerPrefs.SetInt("ResolutionHeight", Screen.height);
            PlayerPrefs.SetInt("ResolutionIndex", 0);
            Debug.LogWarning($"Invalid selectedResolution ({selectedResolution}), using current screen resolution: {Screen.width}x{Screen.height}");
        }
        
        // Save fullscreen
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        
        // Save quality
        PlayerPrefs.SetInt("QualityLevel", QualitySettings.GetQualityLevel());
        
        // Lưu settings
        PlayerPrefs.Save();
        
        Debug.Log("Settings saved successfully!");
    }
    
    // Method để load settings từ PlayerPrefs
    public void LoadSettings()
    {
        // Load resolution (mặc định 1920x1080 nếu chưa có)
        int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        selectedResolution = PlayerPrefs.GetInt("ResolutionIndex", 0);
        
        // Load fullscreen (mặc định true nếu chưa có)
        isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        
        // Load quality (mặc định 3 - High nếu chưa có)
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", 3);
        
        // Kiểm tra quality level có hợp lệ không
        if (qualityLevel < 0 || qualityLevel >= QualitySettings.names.Length)
        {
            qualityLevel = 3; // Reset về High nếu không hợp lệ
            Debug.LogWarning($"Invalid quality level loaded, reset to High (3)");
        }
        
        // Áp dụng settings
        Screen.SetResolution(width, height, isFullScreen);
        QualitySettings.SetQualityLevel(qualityLevel);
        
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
            fullScreenToggle.isOn = isFullScreen;
        }
        
        if (qualityDropDown != null)
        {
            qualityDropDown.value = qualityLevel;
            qualityDropDown.RefreshShownValue();
        }
        
        Debug.Log($"Settings loaded: {width}x{height}, Fullscreen: {isFullScreen}, Quality: {QualitySettings.names[qualityLevel]}");
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
}
