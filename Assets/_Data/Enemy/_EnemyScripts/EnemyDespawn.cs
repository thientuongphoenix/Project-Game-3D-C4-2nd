using UnityEngine;

public class EnemyDespawn : Despawn<EnemyCtrl>
{
    protected override void ResetValue()
    {
        base.ResetValue();
        this.isDespawnByTime = false;
    }
}
