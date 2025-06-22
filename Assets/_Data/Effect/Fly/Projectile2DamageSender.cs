using UnityEngine;

public class Projectile2DamageSender : EffectDamageSender
{
    protected override string GetHitName()
    {
        return "Hit2";
    }

    protected override void Send(DamageReceiver damageReceiver, Collider collider)
    {
        if (damageReceiver is PlayerDamageReceiver || damageReceiver is TowerDamageReceiver)
        {
            this.effectCtrl.Despawn.DoDespawn();
            return;
        }

        base.Send(damageReceiver, collider);
    }
}
