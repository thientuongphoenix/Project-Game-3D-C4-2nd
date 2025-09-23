using UnityEngine;

public class AttackLight : AttackAbstract
{
    protected string effectName = "Projectile2";
    protected SoundName shootSFXName = SoundName.MagicSpell;

    [SerializeField] protected float cooldown = 0.3f; // Cooldown 0.3s cho light attack
    protected float currentCooldown;
    protected bool isOnCooldown;

    protected override void Attacking()
    {
        if(!InputManager.Instance.IsAttackLight()) return;
        if(isOnCooldown) return; // Kiểm tra cooldown

        Debug.Log("🔥 Attack Light triggered!");

        AttackPoint attackPoint = this.GetAttackPoint();

        EffectCtrl effect = this.spawner.Spawn(this.GetEffect(), attackPoint.transform.position);

        EffectFlyAbstract effectFly = (EffectFlyAbstract)effect;
        effectFly.FlyToTarget.SetTarget(this.playerCtrl.CrosshairPointer.transform);

        effect.gameObject.SetActive(true);

        this.SpawnSound(effectFly.transform.position);
        
        // Reset attack light state sau khi bắn
        InputManager.Instance.ResetAttackLight();
        Debug.Log("🔄 Attack Light state reset!");
        
        // Bắt đầu cooldown sau khi bắn
        this.StartCooldown();
    }

    protected override void Update()
    {
        base.Update();
        
        // Cập nhật cooldown
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
