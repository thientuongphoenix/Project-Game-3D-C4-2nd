using UnityEngine;

public class AttackLight : AttackAbstract
{
    protected string effectName = "Projectile2";
    protected SoundName shootSFXName = SoundName.MagicSpell;

    protected override void Attacking()
    {
        if(!InputManager.Instance.IsAttackLight()) return;

        AttackPoint attackPoint = this.GetAttackPoint();

        EffectCtrl effect = this.spawner.Spawn(this.GetEffect(), attackPoint.transform.position);

        EffectFlyAbstract effectFly = (EffectFlyAbstract)effect;
        effectFly.FlyToTarget.SetTarget(this.playerCtrl.CrosshairPointer.transform);

        effect.gameObject.SetActive(true);
        //Debug.Log("Attack Light");

        this.SpawnSound(effectFly.transform.position);
    }

    protected virtual EffectCtrl GetEffect()
    {
        return this.prefabs.GetByName(this.effectName);
    }

    protected virtual void SpawnSound(Vector3 position)
    {
        // SFXCtrl sfxPrefab = (SFXCtrl)SoundSpawnerCtrl.Instance.Prefabs.GetByName(this.shootSFXName.ToString());
        // SFXCtrl newSfx = (SFXCtrl)SoundSpawnerCtrl.Instance.Spawner.Spawn(sfxPrefab, position);
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("SoundManager.Instance is null, cannot spawn SFX");
            return;
        }
        
        SFXCtrl newSfx = SoundManager.Instance.CreateSfx(this.shootSFXName);
        if (newSfx != null)
        {
            newSfx.transform.position = position;
            newSfx.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Failed to create SFX: {this.shootSFXName}");
        }
    }
}
