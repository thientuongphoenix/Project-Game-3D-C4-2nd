using UnityEngine;

public class AttackPoint : SaiMonoBehaviour
{
    protected override void Reset()
    {
        base.Reset();
        transform.localPosition = new Vector3(0.076f, 0.671f, 0.005f);
    }
}
