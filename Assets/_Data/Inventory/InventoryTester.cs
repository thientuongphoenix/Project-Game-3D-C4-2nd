using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class InventoryTester : SaiMonoBehaviour
{
    protected override void Start()
    {
        base.Start();
        // Tắt tự động thêm vàng - sử dụng MapGoldManager thay thế
        // this.AddTestItems(ItemCode.Gold, 120);
    }

    [ProButton]
    public virtual void AddTestItems(ItemCode itemCode, int count)
    {
        for (int i = 0; i < count; i++)
        {
            InventoryManager.Instance.AddItem(itemCode, 1);
        }
    }

    [ProButton]
    public virtual void RemoveTestItems(ItemCode itemCode, int count)
    {
        for (int i = 0; i < count; i++)
        {
            InventoryManager.Instance.RemoveItem(itemCode, 1);
        }
    }
}
