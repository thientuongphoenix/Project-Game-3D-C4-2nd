using UnityEngine;

public class TowerSpawnerCtrl : SaiSingleton<TowerSpawnerCtrl>
{
    [SerializeField] protected TowerSpawner spawner;
    public TowerSpawner Spawner => spawner;

    [SerializeField] protected TowerPrefabs prefabs;
    public TowerPrefabs Prefabs => prefabs;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadTowerSpawner();
        this.LoadTowerPrefabs();
    }

    protected virtual void LoadTowerSpawner()
    {
        if (this.spawner != null) return;
        this.spawner = GetComponent<TowerSpawner>();
        Debug.Log(transform.name + ": LoadTowerSpawner", gameObject);
    }

    protected virtual void LoadTowerPrefabs()
    {
        if (this.prefabs != null) return;
        this.prefabs = GetComponentInChildren<TowerPrefabs>();
        Debug.Log(transform.name + ": LoadTowerPrefabs", gameObject);
    }
}
