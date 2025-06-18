using UnityEngine;

public class BtnUseItem : ButttonAbstract
{
    protected override void LoadComponents()
    {
        base.LoadComponents();
    }

    protected override void OnClick()
    {
        BtnItemInventory selectedItemBtn = InventoryUI.Instance.SelectedItem;
        if (selectedItemBtn == null) return;

        ItemInventory item = selectedItemBtn.ItemInventory;
        if (item == null) return;

        switch (item.ItemProfile.itemCode)
        {
            case ItemCode.HealthPotion:
                Debug.Log($"Using Health Potion - Healing player...");
                // TODO: Implement healing logic
                this.UseHealthPotion(item);
                break;

            case ItemCode.ManaPotion:
                Debug.Log($"Using Mana Potion - Restoring mana...");
                // TODO: Implement mana restore logic
                this.UseManaPotion(item);
                break;

            case ItemCode.Wand:
                Debug.Log($"Cannot use Wand directly - This item should be equipped instead");
                break;

            default:
                Debug.Log($"Item {item.GetItemName()} cannot be used directly");
                break;
        }
    }

    protected virtual void UseHealthPotion(ItemInventory item)
    {
        if (item.itemCount <= 0) return;

        Debug.Log($"Health Potion used! Healing for X amount");
        // TODO: Add healing amount to player's health
        // PlayerManager.Instance.Heal(healAmount);

        // Giảm số lượng item sau khi sử dụng
        InventoryManager.Instance.RemoveItem(item.ItemProfile.itemCode, 1);
    }

    protected virtual void UseManaPotion(ItemInventory item)
    {
        if (item.itemCount <= 0) return;

        Debug.Log($"Mana Potion used! Restoring X mana");
        // TODO: Add mana amount to player's mana
        // PlayerManager.Instance.RestoreMana(manaAmount);

        // Giảm số lượng item sau khi sử dụng
        InventoryManager.Instance.RemoveItem(item.ItemProfile.itemCode, 1);
    }
}
