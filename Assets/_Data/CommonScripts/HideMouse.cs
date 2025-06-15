using UnityEngine;

public class HideMouse : SaiSingleton<HideMouse>
{
    public bool isCursorVisible = false;

    protected override void Start()
    {
        SetCursorVisible(this.isCursorVisible);
    }

    protected virtual void Update()
    {
        SetCursorVisible(this.isCursorVisible);

        // if(Input.GetKeyDown(KeyCode.Escape))
        // {
        //     Cursor.visible = true;
        //     Cursor.lockState = CursorLockMode.None;
        // }
    }

    protected void SetCursorVisible(bool visible)
    {
        if(this.isCursorVisible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
