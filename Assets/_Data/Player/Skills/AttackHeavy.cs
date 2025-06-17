using UnityEngine;

public class AttackHeavy : AttackAbstract
{
    protected string effectName = "Projectile3";
    protected SoundName shootSFXName = SoundName.MagicSpell;

    [SerializeField] protected float cooldown = 3f;
    protected float currentCooldown;
    protected bool isOnCooldown;

    [SerializeField] protected float manaCost = 2f;

    protected override void Attacking()
    {
        if(!InputManager.Instance.IsAttackHeavy()) return;
        if(isOnCooldown) return;
        
        if (!this.playerCtrl.PlayerMana.UseMana(this.manaCost)) return;

        AttackPoint attackPoint = this.GetAttackPoint();
        EffectCtrl effect = this.spawner.Spawn(this.GetEffect(), attackPoint.transform.position);
        EffectFlyAbstract effectFly = (EffectFlyAbstract)effect;
        effectFly.FlyToTarget.SetTarget(this.playerCtrl.CrosshairPointer.transform);
        effect.gameObject.SetActive(true);
        this.SpawnSound(effectFly.transform.position);
        StartCooldown();
    }

    protected override void Update()
    {
        base.Update();
        if(isOnCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if(currentCooldown <= 0)
            {
                isOnCooldown = false;
                currentCooldown = 0;
            }
        }
    }

    protected virtual void StartCooldown()
    {
        isOnCooldown = true;
        currentCooldown = cooldown;
    }

    protected virtual EffectCtrl GetEffect()
    {
        return this.prefabs.GetByName(this.effectName);
    }

    protected virtual void SpawnSound(Vector3 position)
    {
        // SFXCtrl sfxPrefab = (SFXCtrl)SoundSpawnerCtrl.Instance.Prefabs.GetByName(this.shootSFXName.ToString());
        // SFXCtrl newSfx = (SFXCtrl)SoundSpawnerCtrl.Instance.Spawner.Spawn(sfxPrefab, position);
        SFXCtrl newSfx = SoundManager.Instance.CreateSfx(this.shootSFXName);
        newSfx.transform.position = position;
        newSfx.gameObject.SetActive(true);
    }
}
