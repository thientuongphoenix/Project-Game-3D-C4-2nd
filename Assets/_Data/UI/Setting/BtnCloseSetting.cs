using UnityEngine;

public class BtnCloseSetting : ButttonAbstract
{
    protected override void OnClick()
    {
        CloseSettingUI();
    }

    public virtual void CloseSettingUI()
    {
        UISetting.Instance.Hide();
    }
}
