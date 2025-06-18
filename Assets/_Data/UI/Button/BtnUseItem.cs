using UnityEngine;

public class BtnUseItem : ButttonAbstract
{
    [SerializeField] protected InventoryUI inventoryUI;
    public InventoryUI InventoryUI => inventoryUI;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadInventoryUI();
    }

    protected virtual void LoadInventoryUI()
    {
        if (this.inventoryUI != null) return;
        this.inventoryUI = transform.parent.GetComponentInParent<InventoryUI>();
        Debug.Log(transform.name + ": LoadInventoryUI", gameObject);
    }

    protected override void OnClick()
    {
        throw new System.NotImplementedException();
    }
}
