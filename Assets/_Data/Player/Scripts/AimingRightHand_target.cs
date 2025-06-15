using UnityEngine;

public class AimingRightHand_target : SaiMonoBehaviour
{
    protected override void ResetValue()
    {
        base.ResetValue();
        transform.localPosition = new Vector3(-20.88097f, 1.391f, -21.46787f);
        transform.localRotation = Quaternion.Euler(46.09f, -92.467f, -111.837f);
    }
}
