using UnityEngine;

public class ItemDropDespawn : Despawn<ItemDropCtrl>
{
    protected override void Awake()
    {
        base.Awake();
        // Override settings để vật phẩm không tự động biến mất
        this.isDespawnByTime = false;
        this.timeLife = 999f;
        this.currentTime = this.timeLife;
    }
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
    }
    
    public override void DoDespawn()
    {
        //ItemDropCtrl itemDropCtrl = (ItemDropCtrl) this.parent;

        // ItemInventory item = new();
        // item.itemProfile = InventoryManager.Instance.GetProfileByCode(itemDropCtrl.ItemCode);
        // item.itemCount = itemDropCtrl.ItemCount;
        // InventoryManager.Instance.GetByCodeName(itemDropCtrl.InvCodeName).AddItem(item);

        //InventoryManager.Instance.AddItem(itemDropCtrl.ItemCode, itemDropCtrl.ItemCount);

        base.DoDespawn();
    }
    
    /// <summary>
    /// Override DespawnByTime để tắt tự động despawn
    /// </summary>
    protected override void DespawnByTime()
    {
        // Không làm gì cả - vật phẩm sẽ không tự động biến mất
        // Chỉ despawn khi player nhặt hoặc gọi DoDespawn() thủ công
        return;
    }
}
