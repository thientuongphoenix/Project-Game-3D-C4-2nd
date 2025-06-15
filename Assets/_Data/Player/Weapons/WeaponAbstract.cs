using UnityEngine;

public abstract class WeaponAbstract : SaiMonoBehaviour
{
    [SerializeField] protected AttackPoint attackPoint;
    public AttackPoint AttackPoint => attackPoint;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadAttackPoint();
    }

    protected virtual void LoadAttackPoint()
    {
        if(this.attackPoint != null) return;
        this.attackPoint = GetComponentInChildren<AttackPoint>();
        Debug.Log(transform.name + ": LoadAttackPoint", gameObject);
    }
}
