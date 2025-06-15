using UnityEngine;

public abstract class AttackAbstract : SaiMonoBehaviour
{
    [SerializeField] protected PlayerCtrl playerCtrl;
    [SerializeField] protected EffectSpawner spawner;
    [SerializeField] protected EffectPrefabs prefabs;

    protected virtual void Update()
    {
        this.Attacking();
    }
    
    protected abstract void Attacking();

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPlayerCtrl();
        this.LoadEffectSpawner();
        this.LoadEffectPrefabs();
    }

    protected virtual void LoadPlayerCtrl()
    {
        if (this.playerCtrl != null) return;
        this.playerCtrl = GetComponentInParent<PlayerCtrl>();
        Debug.Log(transform.name + ": LoadPlayerCtrl", gameObject);
    }

    protected virtual void LoadEffectSpawner()
    {
        if (this.spawner != null) return;
        this.spawner = GameObject.FindAnyObjectByType<EffectSpawner>();
        Debug.Log(transform.name + ": LoadEffectSpawner", gameObject);
    }

    protected virtual void LoadEffectPrefabs()
    {
        if (this.prefabs != null) return;
        this.prefabs = GameObject.FindAnyObjectByType<EffectPrefabs>();
        Debug.Log(transform.name + ": LoadEffectPrefabs", gameObject);
    }

    protected virtual AttackPoint GetAttackPoint()
    {
        return this.playerCtrl.Weapons.GetCurrentWeapon().AttackPoint;
    }
}
