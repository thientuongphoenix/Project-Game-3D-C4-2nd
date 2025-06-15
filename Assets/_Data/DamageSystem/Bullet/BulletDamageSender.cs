using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BulletDamageSender : DamageSender
{
    [SerializeField] protected BulletCtrl bulletCtrl;
    [SerializeField] protected SphereCollider sphereCollider;
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadSphereCollider();
        this.LoadBulletCtrl();
    }

    protected virtual void LoadBulletCtrl()
    {
        if(this.bulletCtrl != null) return;
        this.bulletCtrl = transform.GetComponentInParent<BulletCtrl>();
        Debug.Log(transform.name + " LoadBulletCtrl", gameObject);
    }

    protected virtual void LoadSphereCollider()
    {
        if(this.sphereCollider != null) return;
        this.sphereCollider = GetComponent<SphereCollider>();
        this.sphereCollider.radius = 0.05f;
        this.sphereCollider.isTrigger = true;
        Debug.Log(transform.name + " LoadSphereCollider", gameObject);
    }

    protected override void Send(DamageReceiver damageRecever, Collider collider)
    {
        base.Send(damageRecever, collider);
        //Làm gì đó để Despawn Bullet
        this.bulletCtrl.Bullet.Despawn.DoDespawn();
    }   
}
