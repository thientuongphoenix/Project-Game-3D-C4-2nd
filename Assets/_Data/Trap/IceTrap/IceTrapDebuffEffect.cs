using UnityEngine;

// public class IceTrapDebuffEffect : DebuffSender
// {
//     
// }

public class IceTrapDebuffEffect : DebuffSender
{
    // Giả sử enemy có script EnemyMovement với biến speed
    [SerializeField] private float slowAmount = 0.5f;

    protected override void ApplyDebuff(Collider collider)
    {
        EnemyCtrl enemyCtrl = collider.GetComponentInParent<EnemyCtrl>();
        if (enemyCtrl != null && enemyCtrl.Agent != null)
        {
            enemyCtrl.Agent.speed *= slowAmount;
            Debug.Log("Enemy bị làm chậm!");
        }
    }

    protected override void RemoveDebuff(Collider collider)
    {
        EnemyCtrl enemyCtrl = collider.GetComponentInParent<EnemyCtrl>();
        if (enemyCtrl != null && enemyCtrl.Agent != null)
        {
            enemyCtrl.Agent.speed /= slowAmount;
            Debug.Log("Enemy hết bị làm chậm!");
        }
    }
}
