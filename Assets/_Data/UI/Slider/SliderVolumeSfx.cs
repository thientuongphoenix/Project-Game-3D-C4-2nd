using UnityEngine;

public class SliderVolumeSfx : SliderAbstract
{
    protected override void Start()
    {
        base.Start();
        this.SyncWithSavedSettings();
    }
    
    protected override void OnSliderValueChanged(float value)
    {
        //Debug.Log("OnSliderValueChanged: " + value);
        SoundManager.Instance.VolumeSfxUpdating(value);
    }
    
    /// <summary>
    /// Đồng bộ slider với settings đã lưu
    /// </summary>
    protected virtual void SyncWithSavedSettings()
    {
        if (SoundManager.Instance != null)
        {
            float savedVolume = SoundManager.Instance.GetSFXVolume();
            this.slider.value = savedVolume;
            Debug.Log($"SliderVolumeSfx: Synced with saved volume - {savedVolume}");
        }
        else
        {
            Debug.LogWarning("SoundManager.Instance is null! Cannot sync slider with saved settings.");
        }
    }
}
