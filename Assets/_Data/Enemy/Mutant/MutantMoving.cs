using UnityEngine;

public class MutantMoving : EnemyMoving
{
    protected override void ResetValue()
    {
        base.ResetValue();
        this.pathName = "Path_0";
        // Có thể set speed riêng cho Mutant ở đây
        // this.moveSpeed = 3f; // Ví dụ: Mutant chạy nhanh hơn
    }
}
