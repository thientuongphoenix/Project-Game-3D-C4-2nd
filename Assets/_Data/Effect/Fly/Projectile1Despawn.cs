using UnityEngine;

public class Projectile1Despawn : EffectDespawn
{
    //[SerializeField] protected EffectSpawner effectSpawner;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        //this.LoadEffectSpawner();
    }

    // protected virtual void LoadEffectSpawner()
    // {
    //     if (this.effectSpawner != null) return;
    //     this.effectSpawner = GameObject.Find("EffectSpawner").GetComponent<EffectSpawner>();
    //     Debug.Log(transform.name + ": LoadEffectSpawner", gameObject);
    // }


    public override void DoDespawn()
    {
        this.HitVFX();
        base.DoDespawn();
    }

    protected virtual void HitVFX()
    {
        EffectCtrl effect = spawner.PoolPrefabs.GetByName("Hit1");
        EffectCtrl newEffect = spawner.Spawn(effect, gameObject.transform.parent.position);
        newEffect.gameObject.SetActive(true);
    }
}
