using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TowerDamageReceiver : DamageReceiver
{
    [SerializeField] protected SphereCollider sphereCollider;
    [SerializeField] protected TowerCtrl towerCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadSphereCollider();
        this.LoadTowerCtrl();
    }

    protected virtual void LoadTowerCtrl()
    {
        if(this.towerCtrl != null) return;
        this.towerCtrl = transform.parent.GetComponent<TowerCtrl>();
        Debug.Log(transform.name + ": LoadTowerCtrl", gameObject);
    }

    protected virtual void LoadSphereCollider()
    {
        if(this.sphereCollider != null) return;
        this.sphereCollider = GetComponent<SphereCollider>();
        this.sphereCollider.radius = 0.5f;
        this.sphereCollider.isTrigger = true;
        Debug.Log(transform.name + ": LoadSphereCollider", gameObject);
    }

    protected override void OnDead()
    {
        base.OnDead();
        //this.enemyCtrl.Animator.SetBool("isDead", this.isDead);
        this.sphereCollider.enabled = false;
        //this.RewardOnDead();
        Invoke(nameof(this.Disappear), 1f);
    }

    protected override void OnHurt()
    {
        base.OnHurt();
        //this.enemyCtrl.Animator.SetTrigger("isHurt");
    }

    protected virtual void Disappear()
    {
        this.towerCtrl.TowerDespawn.DoDespawn();
        //this.enemyCtrl.Despawn.DoDespawn();
    }

    protected override void OnReborn()
    {
        base.OnReborn();
        this.sphereCollider.enabled = true;
    }
}
