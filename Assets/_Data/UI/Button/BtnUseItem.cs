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
                
                this.UseHealthPotion(item);
                break;

            case ItemCode.ManaPotion:
                Debug.Log($"Using Mana Potion - Restoring mana...");
                
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

        int healAmount = 10;
        PlayerCtrl.Instance.PlayerDamageReceiver.Heal(healAmount);
        Debug.Log($"Health Potion used! Healing for {healAmount} HP");

        InventoryManager.Instance.RemoveItem(item.ItemProfile.itemCode, 1);
    }

    protected virtual void UseManaPotion(ItemInventory item)
    {
        if (item.itemCount <= 0) return;

        float manaAmount = 10f;
        PlayerCtrl.Instance.PlayerMana.AddMana(manaAmount);
        Debug.Log($"Mana Potion used! Restoring {manaAmount} mana");

        InventoryManager.Instance.RemoveItem(item.ItemProfile.itemCode, 1);
    }
}
