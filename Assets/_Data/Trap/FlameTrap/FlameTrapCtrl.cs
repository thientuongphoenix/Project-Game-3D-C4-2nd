using UnityEngine;

public class FlameTrapCtrl : TowerCtrl
{
    [SerializeField] protected ParticleSystem flameParticle;
    public ParticleSystem FlameParticle => flameParticle;

    [SerializeField] protected FlameTrapEffect flameTrapEffect;
    public FlameTrapEffect FlameTrapEffect => flameTrapEffect;

    public override string GetName()
    {
        return TowerCode.FlameTrap.ToString();
    }

    protected override void Start()
    {
        base.Start();
        this.CheckActiveByParent();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadParticleSystem();
        this.LoadFlameTrapEffect();
    }

    protected virtual void LoadParticleSystem()
    {
        if (this.flameParticle != null) return;
        this.flameParticle = GetComponentInChildren<ParticleSystem>();
    }

    protected virtual void LoadFlameTrapEffect()
    {
        if (this.flameTrapEffect != null) return;
        this.flameTrapEffect = GetComponentInChildren<FlameTrapEffect>();
    }

    protected virtual void CheckActiveByParent()
    {
        bool isPoolHolder = (this.transform.parent != null && this.transform.parent.GetComponent<PoolHolder>() != null);
        if (this.flameParticle != null) this.flameParticle.gameObject.SetActive(isPoolHolder);
        if (this.flameTrapEffect != null) this.flameTrapEffect.gameObject.SetActive(isPoolHolder);
    }
}
