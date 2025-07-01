using UnityEngine;

public class SliderPlayerMana : SliderHp
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
        return (float)this.playerCtrl.PlayerMana.CurrentMana / (float)this.playerCtrl.PlayerMana.MaxMana;
    }
}
