using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resDropDown;
    public Toggle fullScreenToggle;

    Resolution[] allResolutions;
    bool isFullScreen;
    int selectedResolution;
    List<Resolution> selectedResolutionList = new List<Resolution>();

    void Start()
    {
        isFullScreen = true;
        allResolutions = Screen.resolutions;

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
    }

    public void ChangeResolution()
    {
        selectedResolution = resDropDown.value;
        Screen.SetResolution(selectedResolutionList[selectedResolution].width, selectedResolutionList[selectedResolution].height,isFullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen()
    {
        isFullScreen = fullScreenToggle.isOn;
        Screen.SetResolution(selectedResolutionList[selectedResolution].width, selectedResolutionList[selectedResolution].height, isFullScreen);
    }
}
