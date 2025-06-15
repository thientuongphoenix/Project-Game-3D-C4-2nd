using UnityEngine;

public class PlayerHp : SliderHp
{
    [SerializeField] protected PlayerCtrl playerCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPlayerCtrl();
    }

    protected virtual void LoadPlayerCtrl()
    {
        if (this.playerCtrl != null) return;
        this.playerCtrl = Object.FindAnyObjectByType<PlayerCtrl>();
        Debug.Log(transform.name + ": LoadPlayerCtrl", gameObject);
    }

    protected override float GetValue()
    {
        return (float)this.playerCtrl.PlayerDamageReceiver.CurrentHp / (float)this.playerCtrl.PlayerDamageReceiver.MaxHP;
    }
}
