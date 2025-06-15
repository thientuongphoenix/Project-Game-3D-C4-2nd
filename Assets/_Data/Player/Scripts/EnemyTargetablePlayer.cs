using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyTargetablePlayer : SaiMonoBehaviour
{
    [SerializeField] protected CapsuleCollider capsuleCollider;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCapsuleCollider();
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
