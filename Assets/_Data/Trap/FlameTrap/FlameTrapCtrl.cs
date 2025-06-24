using UnityEngine;

public class FlameTrapCtrl : TowerCtrl
{
    [SerializeField] protected ParticleSystem flameParticle;
    public ParticleSystem FlameParticle => flameParticle;

    public override string GetName()
    {
        return TowerCode.FlameTrap.ToString();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadParticleSystem();
    }

    protected virtual void LoadParticleSystem()
    {
        if (this.flameParticle != null) return;
        this.flameParticle = GetComponentInChildren<ParticleSystem>();
    }
}
