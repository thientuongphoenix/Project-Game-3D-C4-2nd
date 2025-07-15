using UnityEngine;

public class CoreCtrl : TowerCtrl
{
    public override string GetName()
    {
        return TowerCode.Core.ToString();
    }

    protected override void SetActiveEnemyTargetable()
    {
        if (this.transform.parent == null)
        {
            if (this.enemyTargetable != null) this.enemyTargetable.gameObject.SetActive(true);
            if (this.towerDamageReceiver != null) this.towerDamageReceiver.gameObject.SetActive(true);
        }
        else
        {
            if (this.enemyTargetable != null) this.enemyTargetable.gameObject.SetActive(false);
            if (this.towerDamageReceiver != null) this.towerDamageReceiver.gameObject.SetActive(false);
        }
    }
} 