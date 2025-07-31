using UnityEngine;

public class HulkMoving : EnemyMoving
{
    protected override void ResetValue()
    {
        base.ResetValue();
        this.pathName = "Path_0";
    }
}
