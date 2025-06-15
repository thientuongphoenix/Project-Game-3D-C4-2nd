using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerDamageReceiver : DamageReceiver
{
    [SerializeField] protected CapsuleCollider capsuleCollider;
    [SerializeField] protected PlayerCtrl playerCtrl;
    public PlayerCtrl PlayerCtrl => playerCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCapsuleCollider();
        this.LoadPlayerCtrl();
    }

    protected virtual void LoadPlayerCtrl()
    {
        if (this.playerCtrl != null) return;
        this.playerCtrl = transform.parent.GetComponent<PlayerCtrl>();
        Debug.Log(transform.name + ": LoadPlayerCtrl", gameObject);
    }

    protected virtual void LoadCapsuleCollider()
    {
        if (this.capsuleCollider != null) return;
        this.capsuleCollider = GetComponent<CapsuleCollider>();
        this.capsuleCollider.center = new Vector3(0, 0.9f, 0);
        this.capsuleCollider.radius = 0.1f;
        this.capsuleCollider.height = 0.6f;
        this.capsuleCollider.isTrigger = true;
        Debug.Log(transform.name + ": LoadCapsuleCollider", gameObject);
    }
}
