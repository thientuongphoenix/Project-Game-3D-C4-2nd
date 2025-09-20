using UnityEngine;

public class EvilScreamCtrl : SFXCtrl
{
    public override string GetName()
    {
        return "EvilScream";
    }

    protected override void ResetValue()
    {
        base.ResetValue();
        this.audioSource.spatialBlend = 1;
    }
}
