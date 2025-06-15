using System.Collections.Generic;
using UnityEngine;

public class InputHotkeys : SaiSingleton<InputHotkeys>
{
    [SerializeField] protected bool isToggleInventoryUI = false;
    public bool IsToggleInventoryUI => isToggleInventoryUI;

    public bool isToogleMusic = false;
    public bool isToogleSetting = false;

    [SerializeField] protected KeyCode keyCode;
    public KeyCode KeyCode => keyCode;

    [SerializeField] protected bool isPlaceTower;
    public bool IsPlaceTower => isPlaceTower;

    protected virtual void Update()
    {
        this.OpenInventory();
        this.ToogleMusic();
        this.ToggleSetting();
        this.ToggleNumber();
    }

    protected virtual void OpenInventory()
    {
        this.isToggleInventoryUI = Input.GetKeyUp(KeyCode.I);
    }

    protected virtual void ToggleNumber()
    {
        this.isPlaceTower = Input.GetKeyUp(KeyCode.F);

        for (int i = 1; i <= 9; i++) // Duyệt qua các phím số từ 1 đến 9
        {
            KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i);
            if (Input.GetKeyDown(key)) // Dùng GetKeyDown để chỉ kích hoạt khi phím được nhấn
            {
                this.keyCode = this.keyCode == key ? KeyCode.None : key;
                break; // Dừng vòng lặp sau khi xử lý một phím
            }
        }
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
