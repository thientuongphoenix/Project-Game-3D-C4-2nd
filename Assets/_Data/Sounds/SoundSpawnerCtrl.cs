using UnityEngine;

public class SoundSpawnerCtrl : SaiSingleton<SoundSpawnerCtrl>
{
    [SerializeField] protected SoundSpawner spawner;
    public SoundSpawner Spawner => spawner;

    [SerializeField] protected SoundPrefabs prefabs;
    public SoundPrefabs Prefabs => prefabs;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadSoundSpawner();
        this.LoadSoundPrefabs();
    }

    protected virtual void LoadSoundPrefabs()
    {
        if(this.prefabs != null) return;
        this.prefabs = GetComponentInChildren<SoundPrefabs>();
        Debug.Log(transform.name + ": LoadSoundPrefabs", gameObject);
    }

    protected virtual void LoadSoundSpawner()
    {
        if(this.spawner != null) return;
        this.spawner = GetComponent<SoundSpawner>();
        Debug.Log(transform.name + ": LoadSoundSpawner", gameObject);
    }
}
