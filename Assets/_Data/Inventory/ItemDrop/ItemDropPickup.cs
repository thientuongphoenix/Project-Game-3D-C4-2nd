using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ItemDropPickup : SaiMonoBehaviour
{
    [SerializeField] protected BoxCollider boxCollider;
    [SerializeField] protected ItemDropCtrl itemDropCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadBoxCollider();
        this.LoadItemDropCtrl();
    }

    protected virtual void LoadBoxCollider()
    {
        if (this.boxCollider != null) return;
        this.boxCollider = GetComponent<BoxCollider>();
        //this.boxCollider.isTrigger = true;
        Debug.Log(transform.name + ": LoadBoxCollider", gameObject);
    }

    protected virtual void LoadItemDropCtrl()
    {
        if (this.itemDropCtrl != null) return;
        this.itemDropCtrl = transform.parent.GetComponent<ItemDropCtrl>();
        Debug.Log(transform.name + ": LoadItemDropCtrl", gameObject);
    }
    
    public virtual void AddItemToInventoryWhenPLayerPickup()
    {
        InventoryManager.Instance.AddItem(itemDropCtrl.ItemCode, itemDropCtrl.ItemCount);
    }
    
    // protected void OnTriggerEnter(Collider other)
    // {
    //     // Kiểm tra nếu collider là Player (có ItemPicker)
    //     if (other.GetComponent<ItemPicker>() == null) return;

    //     // Thêm item vào inventory
    //     this.AddItemToInventoryWhenPLayerPickup();

    //     // Despawn item drop
    //     if (this.itemDropCtrl != null)
    //     {
    //         this.itemDropCtrl.Despawn.DoDespawn();
    //     }
    // }
}
