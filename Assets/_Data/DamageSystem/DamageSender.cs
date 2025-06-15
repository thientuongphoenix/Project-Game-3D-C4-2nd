using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class DamageSender : SaiMonoBehaviour
{
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected int damage = 1;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadRigidbody();
    }

    protected virtual void LoadRigidbody()
    {
        if(this.rigid != null) return;
        this.rigid = GetComponent<Rigidbody>();
        this.rigid.useGravity = false;
        Debug.Log(transform.name + " LoadRigidbody", gameObject);
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        DamageReceiver damageReceiver = collider.GetComponent<DamageReceiver>();
        if (damageReceiver == null) return;
        this.Send(damageReceiver, collider);
        Debug.Log("OnTriggerEnter: " + collider.name);
    }

    protected virtual void Send(DamageReceiver damageReceiver, Collider collider)
    {
        damageReceiver.Deduct(this.damage);
    }    
}
