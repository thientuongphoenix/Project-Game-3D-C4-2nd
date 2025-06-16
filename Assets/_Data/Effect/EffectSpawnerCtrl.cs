using UnityEngine;

public class EffectSpawnerCtrl : SaiSingleton<EffectSpawnerCtrl>
{
    [SerializeField] protected EffectSpawner spawner;
    public EffectSpawner Spawner => spawner;

    [SerializeField] protected EffectPrefabs prefabs;
    public EffectPrefabs Prefabs => prefabs;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadEffectSpawner();
        this.LoadEffectPrefabs();
    }

    protected virtual void LoadEffectSpawner()
    {
        if (this.spawner != null) return;
        this.spawner = GetComponent<EffectSpawner>();
        Debug.Log(transform.name + ": LoadEffectSpawner", gameObject);
    }

    protected virtual void LoadEffectPrefabs()
    {
        if (this.prefabs != null) return;
        this.prefabs = GetComponentInChildren<EffectPrefabs>();
        Debug.Log(transform.name + ": LoadEffectPrefabs", gameObject);
    }
}
