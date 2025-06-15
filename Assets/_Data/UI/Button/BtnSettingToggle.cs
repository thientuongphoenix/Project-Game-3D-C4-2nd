using UnityEngine;

public class BtnSettingToggle : ButttonAbstract
{
    protected virtual void LateUpdate()
    {
        this.HotkeyToogleSetting();
    }

    protected override void OnClick()
    {
        UISetting.Instance.ToggleSetting();
    }

    protected virtual void HotkeyToogleSetting()
    {
        if (InputHotkeys.Instance.isToogleSetting) UISetting.Instance.ToggleSetting();
    }
}
