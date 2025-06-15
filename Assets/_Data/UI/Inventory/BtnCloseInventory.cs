using UnityEngine;

public class BtnCloseInventory : ButttonAbstract
{
    protected override void OnClick()
    {
        this.CloseInventoryUI();
    }

    public virtual void CloseInventoryUI()
    {
        InventoryUI.Instance.Hide();
    }
}
