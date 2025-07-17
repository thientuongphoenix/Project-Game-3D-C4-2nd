using UnityEngine;

public class CoreCtrl : TowerCtrl
{
    public override string GetName()
    {
        return TowerCode.Core.ToString();
    }

    protected override void SetActiveEnemyTargetable()
    {
        if (this.enemyTargetable != null) this.enemyTargetable.gameObject.SetActive(true);
        if (this.towerDamageReceiver != null) this.towerDamageReceiver.gameObject.SetActive(true);
    }
} 