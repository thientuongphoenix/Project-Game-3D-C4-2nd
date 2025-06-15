using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyDamageReceiver : DamageReceiver
{
    [SerializeField] protected CapsuleCollider capsuleCollider;
    [SerializeField] protected EnemyCtrl enemyCtrl;
    
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCapsuleCollider();
        this.LoadEnemyCtrl();
    }

    protected virtual void LoadCapsuleCollider()
    {
        if(this.capsuleCollider != null) return;
        this.capsuleCollider = GetComponent<CapsuleCollider>();
        this.capsuleCollider.center = new Vector3(0, 1, 0);
        this.capsuleCollider.radius = 0.3f;
        this.capsuleCollider.height = 1.5f;
        this.capsuleCollider.isTrigger = true;
        Debug.Log(transform.name + " LoadCapsuleCollider", gameObject);
    }

    protected virtual void LoadEnemyCtrl()
    {
        if(this.enemyCtrl != null) return;
        this.enemyCtrl = GetComponentInParent<EnemyCtrl>();
        Debug.Log(transform.name + " LoadAnimator", gameObject);
    }

    protected override void OnDead()
    {
        base.OnDead();
        //this.enemyCtrl.Agent.isStopped = true;
        this.enemyCtrl.Animator.SetBool("isDead", this.isDead);
        this.capsuleCollider.enabled = false;
        this.RewardOnDead();
        Invoke(nameof(this.Disappear), 3f);
    }

    protected override void OnHurt()
    {
        base.OnHurt();
        this.enemyCtrl.Animator.SetTrigger("isHurt");
    }

    protected virtual void Disappear()
    {
        this.enemyCtrl.Despawn.DoDespawn();
    }

    protected override void OnReborn()
    {
        base.OnReborn();
        //this.enemyCtrl.Agent.isStopped = false;
        this.capsuleCollider.enabled = true;
    }

    protected virtual void RewardOnDead()
    {
        // ItemInventory item = new();
        // item.itemProfile = InventoryManager.Instance.GetProfileByCode(ItemCode.Gold);
        // item.itemCount = 1;
        // InventoryManager.Instance.Monies().AddItem(item);
        //ItemsDropManager.Instance.DropMany(ItemCode.Gold, 10, transform.position);
        //ItemsDropManager.Instance.DropMany(ItemCode.Wand, 1, transform.position);
        //ItemsDropManager.Instance.DropMany(ItemCode.PlayerExp, 10, transform.position);
        ItemsDropManager.Instance.DropItemWithAutoPickupCheck(ItemCode.Gold, 10, transform.position);
        ItemsDropManager.Instance.DropItemWithAutoPickupCheck(ItemCode.Wand, 1, transform.position);
        ItemsDropManager.Instance.DropItemWithAutoPickupCheck(ItemCode.PlayerExp, 10, transform.position);
    }
}
