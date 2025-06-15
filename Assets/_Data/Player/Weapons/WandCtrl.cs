using UnityEngine;

public class WandCtrl : WeaponAbstract
{
    protected override void ResetValue()
    {
        base.ResetValue();
        transform.localPosition = new Vector3(-0.0104f, 0.0548f, 0.026f);
        transform.localRotation = Quaternion.Euler(14.578f, -43.958f, 62.673f);
        transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
    }
}
