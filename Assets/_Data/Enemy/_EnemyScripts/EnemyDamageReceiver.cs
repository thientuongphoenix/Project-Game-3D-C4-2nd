using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyDamageReceiver : DamageReceiver
{
    [SerializeField] protected CapsuleCollider capsuleCollider;
    [SerializeField] protected EnemyCtrl enemyCtrl;
    
    [Header("Reward Settings")]
    [SerializeField] protected int goldReward = 10; // Số vàng rơi khi chết
    [SerializeField] protected int expReward = 10; // Số exp rơi khi chết
    
    
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
        // Hiển thị DamageText
        ShowDamageText();
    }

    // Hàm mới để hiển thị DamageText
    protected void ShowDamageText()
    {
        // Tìm Canvas chứa DamageText (giả sử đặt tên là DamageTextCanvas)
        var canvas = GameObject.Find("CanvasDamageText");
        if (canvas == null) return;
        // Load prefab DamageText
        var prefab = Resources.Load<GameObject>("DamageText");
        if (prefab == null) return;
        // Lấy vị trí world
        Vector3 worldPos = transform.position + Vector3.up * 1.5f;
        // Chuyển sang vị trí canvas (nếu canvas là World Space thì dùng luôn worldPos)
        var go = GameObject.Instantiate(prefab, worldPos, Quaternion.identity, canvas.transform);
        var effect = go.GetComponent<_Data.UI.DamageText.DamageTextEffect>();
        if (effect != null)
        {
            effect.Play(this.lastDamage.ToString(), worldPos);
        }
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
        ItemsDropManager.Instance.DropItemWithAutoPickupCheck(ItemCode.Gold, goldReward, transform.position);
        // ItemsDropManager.Instance.DropItemWithAutoPickupCheck(ItemCode.Wand, 1, transform.position);
        ItemsDropManager.Instance.DropItemWithAutoPickupCheck(ItemCode.PlayerExp, expReward, transform.position);
        ItemsDropManager.Instance.DropItemWithAutoPickupCheck(ItemCode.HealthPotion, 1, transform.position);
        ItemsDropManager.Instance.DropItemWithAutoPickupCheck(ItemCode.ManaPotion, 1, transform.position);
        
        // Show item usage guide UI
        this.ShowItemGuide();
    }
    
    protected virtual void ShowItemGuide()
    {
        // Reset tracker before showing new guide
        if (ItemUseTracker.Instance != null)
        {
            ItemUseTracker.Instance.ResetTracker();
        }
        
        // Show guide UI
        if (ItemGuideUI.Instance != null)
        {
            ItemGuideUI.Instance.ShowGuide();
            Debug.Log("Item Guide UI shown after enemy death!");
        }
        else
        {
            Debug.LogWarning("ItemGuideUI.Instance is null! Cannot show item guide.");
        }
    }
}
