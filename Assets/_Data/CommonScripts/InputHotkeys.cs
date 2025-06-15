using UnityEngine;

public class InputHotkeys : SaiSingleton<InputHotkeys>
{
    [SerializeField] protected bool isToggleInventoryUI = false;
    public bool IsToggleInventoryUI => isToggleInventoryUI;

    public bool isToogleMusic = false;
    public bool isToogleSetting = false;

    protected virtual void Update()
    {
        this.OpenInventory();
        this.ToogleMusic();
        this.ToggleSetting();
    }

    protected virtual void OpenInventory()
    {
        this.isToggleInventoryUI = Input.GetKeyUp(KeyCode.I);
    }

    protected virtual void ToogleMusic()
    {
        this.isToogleMusic = Input.GetKeyUp(KeyCode.M);
    }

    protected virtual void ToggleSetting()
    {
        this.isToogleSetting = Input.GetKeyUp(KeyCode.Escape);
    }
}
