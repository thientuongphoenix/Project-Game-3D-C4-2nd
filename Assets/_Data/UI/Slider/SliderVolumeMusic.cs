using UnityEngine;

public class SliderVolumeMusic : SliderAbstract
{
    protected override void OnSliderValueChanged(float value)
    {
        //Debug.Log("OnSliderValueChanged: " + value);
        SoundManager.Instance.VolumeMusicUpdating(value);
    }
}
