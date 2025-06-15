using UnityEngine;

public class BtnInventoryToggle : ButttonAbstract
{
    protected override void OnClick()
    {
        InventoryUI.Instance.Toggle();
    }
}
