using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DebuffSender : DamageSender
{
    [SerializeField] protected BoxCollider boxCollider;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadBoxCollider();
    }

    protected virtual void LoadBoxCollider()
    {
        if (this.boxCollider != null) return;
        this.boxCollider = GetComponent<BoxCollider>();
        this.boxCollider.isTrigger = true;
        Debug.Log(transform.name + " LoadBoxCollider", gameObject);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        EnemyDamageReceiver receiver = collider.GetComponent<EnemyDamageReceiver>();
        if (receiver == null) return;
        this.ApplyDebuff(collider);
    }

    public virtual void OnTriggerExit(Collider collider)
    {
        EnemyDamageReceiver receiver = collider.GetComponent<EnemyDamageReceiver>();
        if (receiver == null) return;
        this.RemoveDebuff(collider);
    }

    protected virtual void ApplyDebuff(Collider collider) { }
    protected virtual void RemoveDebuff(Collider collider) { }

    protected override void Send(DamageReceiver damageReceiver, Collider collider)
    {
        // Không gây damage, không gọi Deduct
    }
}
