using UnityEngine;

public abstract class SFXCtrl : SoundCtrl
{
    protected override void ResetValue()
    {
        base.ResetValue();
        this.audioSource.loop = false;
    }
    
    protected virtual void Update()
    {
        // Tự động despawn khi audio đã phát xong
        if (this.audioSource != null && !this.audioSource.isPlaying && this.gameObject.activeInHierarchy)
        {
            this.Despawn.DoDespawn();
        }
    }
}
