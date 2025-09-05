using UnityEngine;

public class SliderVolumeMusic : SliderAbstract
{
    protected override void Start()
    {
        base.Start();
        this.SyncWithSavedSettings();
    }
    
    protected override void OnSliderValueChanged(float value)
    {
        //Debug.Log("OnSliderValueChanged: " + value);
        SoundManager.Instance.VolumeMusicUpdating(value);
    }
    
    /// <summary>
    /// Đồng bộ slider với settings đã lưu
    /// </summary>
    protected virtual void SyncWithSavedSettings()
    {
        if (SoundManager.Instance != null)
        {
            float savedVolume = SoundManager.Instance.GetMusicVolume();
            this.slider.value = savedVolume;
            Debug.Log($"SliderVolumeMusic: Synced with saved volume - {savedVolume}");
        }
        else
        {
            Debug.LogWarning("SoundManager.Instance is null! Cannot sync slider with saved settings.");
        }
    }
}
