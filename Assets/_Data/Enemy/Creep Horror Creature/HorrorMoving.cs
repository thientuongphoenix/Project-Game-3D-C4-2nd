using UnityEngine;

public class HorrorMoving : EnemyMoving
{
    protected override void ResetValue()
    {
        base.ResetValue();
        this.pathName = "Path_0";
    }
}
