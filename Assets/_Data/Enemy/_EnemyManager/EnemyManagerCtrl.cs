using UnityEngine;

public class EnemyManagerCtrl : SaiMonoBehaviour
{
    [SerializeField] protected EnemySpawner enemySpawner;
    public EnemySpawner EnemySpawner => enemySpawner;

    [SerializeField] protected EnemyPrefabs enemyPrefabs;
    public EnemyPrefabs EnemyPrefabs => enemyPrefabs;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadEnemySpawner();
        this.LoadEnemyPrefabs();
    }

    protected virtual void LoadEnemySpawner()
    {
        if(this.enemySpawner != null) return;
        this.enemySpawner = transform.GetComponentInChildren<EnemySpawner>();
        Debug.Log(transform.name + " LoadEnemySpawner", gameObject);
    }

    protected virtual void LoadEnemyPrefabs()
    {
        if(this.enemyPrefabs != null) return;
        this.enemyPrefabs = transform.GetComponentInChildren<EnemyPrefabs>();
        Debug.Log(transform.name + " LoadEnemyPrefabs", gameObject);
    }
}
