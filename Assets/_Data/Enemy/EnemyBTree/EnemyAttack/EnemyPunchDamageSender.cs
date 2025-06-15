using UnityEngine;

public class EnemyPunchDamageSender : EnemyDamageSender
{
    protected override string GetHitName()
    {
        return "Hit1";
    }
}
