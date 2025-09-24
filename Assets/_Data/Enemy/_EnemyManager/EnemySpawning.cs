using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : EnemyManagerAbstract
{
    [SerializeField] protected float spawnSpeed = 1f;
    [SerializeField] protected int maxSpawn = 10;
    [SerializeField] protected EnemyCtrl specificEnemyPrefab; // Prefab cụ thể để spawn
    protected List<EnemyCtrl> spawnedEnemies = new();

    protected override void Start()
    {
      base.Start();
      Invoke(nameof(this.Spawning), this.spawnSpeed);
    }

    protected virtual void FixedUpdate()
    {
      this.RemoveDeadOne();
    }

    protected virtual void Spawning()
    {
      Invoke(nameof(this.Spawning), this.spawnSpeed);

      if(this.spawnedEnemies.Count >= this.maxSpawn) return;

      EnemyCtrl prefab = this.GetEnemyPrefab();

      EnemyCtrl newEnemy = this.enemyManagerCtrl.EnemySpawner.Spawn(prefab, transform.position);
      newEnemy.gameObject.SetActive(true);

      if (newEnemy != null && newEnemy.EnemyBTree != null)
            newEnemy.EnemyBTree.BuildBehaviorTree();

        // 2. Reset trạng thái cây
        if (newEnemy != null && newEnemy.EnemyBTree != null)
            newEnemy.EnemyBTree.ResetBTree();

        // 3. Khởi động lại cây hành vi
        if (newEnemy != null && newEnemy.EnemyBTree != null)
            newEnemy.EnemyBTree.StartBTree();

        // 4. Khởi tạo EnemyScream
        if (newEnemy != null && newEnemy.EnemyScream != null)
            newEnemy.EnemyScream.SetCanScream(true);

        

      this.spawnedEnemies.Add(newEnemy);
      Debug.Log("Spawning");
    }

    protected virtual EnemyCtrl GetEnemyPrefab()
    {
      // Ưu tiên sử dụng specificEnemyPrefab nếu có
      if (specificEnemyPrefab != null)
      {
          return specificEnemyPrefab;
      }
      
      // Fallback về random prefab nếu không có specific prefab
      return this.enemyManagerCtrl.EnemyPrefabs.GetRandom();
    }

    protected virtual void RemoveDeadOne()
    {
      foreach(EnemyCtrl enemyCtrl in this.spawnedEnemies)
      {
        if(enemyCtrl.EnemyDamageReceiver.IsDead())
        {
          this.spawnedEnemies.Remove(enemyCtrl);
          return;
        }
      }
    }
    
    // Public properties để truy cập từ bên ngoài
    public virtual int MaxSpawn => maxSpawn;
    public virtual float SpawnSpeed => spawnSpeed;
    public virtual List<EnemyCtrl> SpawnedEnemies => spawnedEnemies;
    public virtual EnemyManagerCtrl EnemyManagerCtrl => enemyManagerCtrl;
    
    // Public property để gán specific enemy prefab
    public virtual EnemyCtrl SpecificEnemyPrefab 
    { 
        get => specificEnemyPrefab; 
        set => specificEnemyPrefab = value; 
    }
}
