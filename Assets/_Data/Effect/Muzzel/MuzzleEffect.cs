using UnityEngine;

public class MuzzleEffect : SaiMonoBehaviour
{
    [SerializeField] protected MuzzleCode muzzleCode;

    protected virtual void OnEnable()
    {
        this.SpawnMuzzle();
    }

    protected virtual void SpawnMuzzle()
    {
        if(this.muzzleCode == MuzzleCode.NoMuzzle) return;
        EffectSpawner effectSpawner = EffectSpawnerCtrl.Instance.Spawner;
        EffectCtrl prefab = effectSpawner.PoolPrefabs.GetByName(this.muzzleCode.ToString());
        EffectCtrl newEffect = effectSpawner.Spawn(prefab, transform.position);
        newEffect.gameObject.SetActive(true);
    }
}
