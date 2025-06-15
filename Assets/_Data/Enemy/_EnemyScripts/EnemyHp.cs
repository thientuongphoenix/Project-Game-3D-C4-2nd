using UnityEngine;

public class EnemyHp : SliderHp
{
    [SerializeField] protected EnemyCtrl enemyCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadEnemyCtrl();
    }
    protected virtual void LoadEnemyCtrl()
    {
        if (this.enemyCtrl != null) return;
        this.enemyCtrl = GetComponentInParent<EnemyCtrl>();
        Debug.Log(transform.name + ": LoadEnemyCtrl", gameObject);
    }

    protected override float GetValue()
    {
        return (float)this.enemyCtrl.EnemyDamageReceiver.CurrentHp / (float)this.enemyCtrl.EnemyDamageReceiver.MaxHP;
    }
}
