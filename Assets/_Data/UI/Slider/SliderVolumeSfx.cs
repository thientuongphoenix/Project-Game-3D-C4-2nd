using UnityEngine;

public class SliderVolumeSfx : SliderAbstract
{
    protected override void OnSliderValueChanged(float value)
    {
        //Debug.Log("OnSliderValueChanged: " + value);
        SoundManager.Instance.VolumeSfxUpdating(value);
    }
}
