using UnityEngine;

public class TowerHp : SliderHp
{
    [SerializeField] protected TowerCtrl towerCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadTowerCtrl();
    }

    protected virtual void LoadTowerCtrl()
    {
        if (this.towerCtrl != null) return;
        this.towerCtrl = GetComponentInParent<TowerCtrl>();
        Debug.Log(transform.name + ": LoadTowerCtrl", gameObject);
    }

    protected override float GetValue()
    {
        return (float)this.towerCtrl.TowerDamageReceiver.CurrentHp / (float)this.towerCtrl.TowerDamageReceiver.MaxHP;
    }
}
