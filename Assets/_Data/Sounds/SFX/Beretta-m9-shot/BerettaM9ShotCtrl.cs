using UnityEngine;

public class BerettaM9ShotCtrl : SFXCtrl
{
    public override string GetName()
    {
        return "BerettaM9Shot";
    }

    protected override void ResetValue()
    {
        base.ResetValue();
        this.audioSource.spatialBlend = 1;
    }
}
