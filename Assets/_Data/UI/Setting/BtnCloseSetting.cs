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
        // Unpause game khi đóng setting bằng button X
        Time.timeScale = 1f; // Resume game
        Debug.Log("Game resumed - Settings closed by X button");
    }
}
