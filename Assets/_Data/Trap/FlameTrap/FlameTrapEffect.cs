using UnityEngine;

public class FlameTrapEffect : DamageSender
{
    [SerializeField] protected BoxCollider boxCollider;
    //[SerializeField] protected ParticleSystem flameParticle;
    [SerializeField] protected float flameDuration = 2f;
    [SerializeField] protected FlameTrapCtrl flameTrapCtrl;
    [SerializeField] protected SoundName flameSFXName = SoundName.Flame;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadBoxCollider();
        this.LoadFlameTrapCtrl();
        //this.LoadParticleSystem();
    }

    protected virtual void LoadBoxCollider()
    {
        if (this.boxCollider != null) return;
        this.boxCollider = GetComponent<BoxCollider>();
        this.boxCollider.isTrigger = true;
        this.boxCollider.enabled = false;
    }

    protected virtual void LoadFlameTrapCtrl()
    {
        if (this.flameTrapCtrl != null) return;
        this.flameTrapCtrl = GetComponentInParent<FlameTrapCtrl>();
    }

    // protected virtual void LoadParticleSystem()
    // {
    //     if (this.flameParticle != null) return;
    //     this.flameParticle = GetComponentInChildren<ParticleSystem>();
    // }

    protected override void Start()
    {
        Invoke(nameof(ActivateFlame), 0f);
    }

    protected virtual void ActivateFlame()
    {
        if (this.flameTrapCtrl.FlameParticle != null) this.flameTrapCtrl.FlameParticle.Play();
        this.boxCollider.enabled = true;  
        
        this.SpawnSound(this.transform.position);
        
        Invoke(nameof(DeactivateFlame), flameDuration);
    }

    protected virtual void SpawnSound(Vector3 position)
    {
        SFXCtrl newSfx = SoundManager.Instance.CreateSfx(this.flameSFXName);
        newSfx.transform.position = position;
        newSfx.gameObject.SetActive(true);
    }

    protected virtual void DeactivateFlame()
    {
        this.boxCollider.enabled = false;
        Invoke(nameof(ActivateFlame), flameDuration);
    }

    protected override void Send(DamageReceiver damageReceiver, Collider collider)
    {
        if (damageReceiver is PlayerDamageReceiver || damageReceiver is TowerDamageReceiver)
        {
            return;
        }
        damageReceiver.Deduct(this.damage);
    }
}
