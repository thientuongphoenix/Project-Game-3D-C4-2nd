using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : SaiSingleton<EnemyWaveManager>
{
    [Header("Wave Settings")]
    [SerializeField] protected List<EnemyWaveData> waves = new List<EnemyWaveData>();
    [SerializeField] protected int currentWaveIndex = 0;
    [SerializeField] protected bool isSpawning = false;
    [SerializeField] protected bool allWavesCompleted = false;
    [SerializeField] protected bool isWaitingForButtonPress = false;
    
    [Header("Wood Baluster Control")]
    [SerializeField] protected string woodBalusterName = "Wood_Baluster_01_lod0";
    protected MeshRenderer woodBalusterRenderer;
    
    [Header("Enemy Tracking")]
    protected List<EnemyCtrl> activeEnemies = new List<EnemyCtrl>();
    
    [Header("Enemy Manager")]
    protected EnemyManagerCtrl enemyManagerCtrl;
    
    [Header("Time Spawn for Wave 5")]
    [SerializeField] protected TimeSpawn timeSpawn;
    
    protected override void Start()
    {
        base.Start();
        this.LoadEnemyManagerCtrl();
        this.LoadWoodBaluster();
        this.LoadTimeSpawn();
        this.InitializeWaves();
    }
    
    protected virtual void LoadTimeSpawn()
    {
        if (this.timeSpawn != null) return;
        this.timeSpawn = FindObjectOfType<TimeSpawn>();
        if (this.timeSpawn == null)
        {
            Debug.LogWarning("TimeSpawn not found in scene! Wave 5 will use normal spawning.");
        }
        else
        {
            Debug.Log("TimeSpawn found and loaded for Wave 5");
        }
    }
    
    protected virtual void LoadEnemyManagerCtrl()
    {
        if (this.enemyManagerCtrl != null) return;
        this.enemyManagerCtrl = FindObjectOfType<EnemyManagerCtrl>();
        if (this.enemyManagerCtrl == null)
        {
            Debug.LogError("EnemyManagerCtrl not found in scene! Please add it to the scene.");
            return;
        }
        Debug.Log("EnemyManagerCtrl found and loaded");
    }
    
    protected virtual void LoadWoodBaluster()
    {
        GameObject woodBaluster = GameObject.Find(woodBalusterName);
        if (woodBaluster != null)
        {
            this.woodBalusterRenderer = woodBaluster.GetComponent<MeshRenderer>();
            if (this.woodBalusterRenderer != null)
            {
                this.woodBalusterRenderer.enabled = true; // Bắt đầu với mesh renderer bật
                Debug.Log("Wood Baluster found and ready");
            }
        }
        else
        {
            Debug.LogWarning($"Wood Baluster '{woodBalusterName}' not found in scene!");
        }
    }
    
    protected virtual void InitializeWaves()
    {
        // Reset tất cả waves
        foreach (var wave in waves)
        {
            wave.ResetWave();
        }
        currentWaveIndex = 0;
        allWavesCompleted = false;
        isSpawning = false;
    }
    
    public virtual void StartWaveSystem()
    {
        if (isSpawning || allWavesCompleted)
        {
            Debug.Log("Wave system already running or completed!");
            return;
        }
        
        Debug.Log($"Starting Enemy Wave System... Total waves: {waves.Count}");
        
        if (waves.Count == 0)
        {
            Debug.LogError("No waves configured! Please setup waves first.");
            return;
        }
        
        // Tắt Wood Baluster
        this.SetWoodBalusterVisibility(false);
        
        // Bắt đầu wave đầu tiên
        this.StartNextWave();
    }
    
    public virtual void StartNextWaveManually()
    {
        if (allWavesCompleted)
        {
            Debug.Log("All waves already completed!");
            return;
        }
        
        if (isSpawning)
        {
            Debug.Log("Currently spawning, please wait!");
            return;
        }
        
        if (!isWaitingForButtonPress)
        {
            Debug.Log("Not waiting for button press!");
            return;
        }
        
        Debug.Log($"Starting Wave {currentWaveIndex + 1} manually...");
        isWaitingForButtonPress = false;
        this.StartNextWave();
    }
    
    protected virtual void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            this.CompleteAllWaves();
            return;
        }
        
        var currentWave = waves[currentWaveIndex];
        Debug.Log($"Starting Wave {currentWave.waveNumber}: {currentWave.waveName}");
        
        StartCoroutine(SpawnWave(currentWave));
    }
    
    protected virtual IEnumerator SpawnWave(EnemyWaveData wave)
    {
        isSpawning = true;
        
        // Chờ delay trước khi bắt đầu wave
        yield return new WaitForSeconds(wave.waveDelay);
        
        // Bật EnemySpawning để nó tự spawn enemies
        if (wave.enemySpawning != null)
        {
            Debug.Log($"Found EnemySpawning for wave {wave.waveNumber}: {wave.enemySpawning.name}");
            Debug.Log($"EnemySpawning enabled before: {wave.enemySpawning.enabled}");
            Debug.Log($"GameObject active before: {wave.enemySpawning.gameObject.activeInHierarchy}");
            
            // Bật GameObject trước
            if (!wave.enemySpawning.gameObject.activeInHierarchy)
            {
                wave.enemySpawning.gameObject.SetActive(true);
                Debug.Log($"Activated GameObject: {wave.enemySpawning.name}");
            }
            
            // Bật component
            wave.enemySpawning.enabled = true;
            Debug.Log($"Enabled EnemySpawning for wave {wave.waveNumber}");
            Debug.Log($"MaxSpawn: {wave.enemySpawning.MaxSpawn}, SpawnSpeed: {wave.enemySpawning.SpawnSpeed}");
            
            // Kiểm tra nếu là wave cuối (wave 5) thì sử dụng TimeSpawn
            if (wave.waveNumber == 5)
            {
                Debug.Log("Final wave detected! Using TimeSpawn for sequential spawning...");
                yield return this.SpawnWave5WithTimeSpawn(wave);
            }
            else
            {
                // Chờ EnemySpawning spawn đủ enemies theo cách thông thường
                yield return new WaitUntil(() => this.CheckEnemySpawningComplete(wave));
            }
            
            // Tắt EnemySpawning sau khi spawn xong
            wave.enemySpawning.enabled = false;
            Debug.Log($"Disabled EnemySpawning for wave {wave.waveNumber}");
        }
        else
        {
            // Fallback: spawn trực tiếp nếu không có EnemySpawning
            for (int i = 0; i < wave.GetEnemyCount(); i++)
            {
                if (wave.CanSpawnMore())
                {
                    this.SpawnEnemyDirectly(wave);
                    wave.enemiesSpawned++;
                    
                    // Chờ interval giữa các lần spawn
                    yield return new WaitForSeconds(wave.GetSpawnInterval());
                }
            }
        }
        
        // Chờ tất cả enemies trong wave này bị tiêu diệt
        Debug.Log($"Waiting for all enemies in wave {wave.waveNumber} to be defeated...");
        yield return new WaitUntil(() => this.CheckWaveComplete(wave));
        
        // Hoàn thành wave
        wave.isCompleted = true;
        isSpawning = false;
        Debug.Log($"Wave {wave.waveNumber} completed! isSpawning = false");
        
        // Chuyển sang wave tiếp theo
        currentWaveIndex++;
        
        if (currentWaveIndex < waves.Count)
        {
            // Chờ người chơi bấm button để spawn đợt tiếp theo
            this.WaitForNextWaveButton();
        }
        else
        {
            Debug.Log("All waves completed! Calling CompleteAllWaves()...");
            this.CompleteAllWaves();
        }
    }
    
    protected virtual bool CheckEnemySpawningComplete(EnemyWaveData wave)
    {
        if (wave.enemySpawning == null) 
        {
            Debug.LogWarning($"No EnemySpawning reference for wave {wave.waveNumber}!");
            return true;
        }
        
        // Kiểm tra xem EnemySpawning đã spawn đủ enemies chưa
        // Dựa trên maxSpawn của EnemySpawning
        int spawnedCount = wave.enemySpawning.SpawnedEnemies.Count;
        int maxSpawn = wave.enemySpawning.MaxSpawn;
        bool isComplete = spawnedCount >= maxSpawn;
        
        Debug.Log($"EnemySpawning progress: {spawnedCount}/{maxSpawn} enemies spawned. Complete: {isComplete}");
        
        return isComplete;
    }
    
    protected virtual IEnumerator SpawnWave5WithTimeSpawn(EnemyWaveData wave)
    {
        if (timeSpawn == null)
        {
            Debug.LogError("TimeSpawn not found! Cannot spawn Wave 5 with timing.");
            yield break;
        }
        
        Debug.Log("Starting Wave 5 with sequential spawning...");
        
        // TẮT TimeSpawn component để tránh xung đột
        timeSpawn.enabled = false;
        Debug.Log("TimeSpawn component disabled to prevent conflicts");
        
        // Tắt tất cả objects trước khi bắt đầu
        this.DeactivateAllTimeSpawnObjects();
        
        // Spawn lần lượt theo thời gian cố định
        yield return this.SpawnWave5Sequentially();
        
        Debug.Log("Wave 5 sequential spawn completed!");
    }
    
    protected virtual IEnumerator SpawnWave5Sequentially()
    {
        Debug.Log("Starting Wave 5 sequential spawn with increased intervals (except Object A)");
        
        // Spawn Object A (3s) - KHÔNG THAY ĐỔI
        yield return new WaitForSeconds(3f);
        this.ActivateTimeSpawnObject(timeSpawn.objectA, "Object A");
        yield return new WaitForSeconds(3f); // Chờ spawn đủ số lượng
        this.DeactivateTimeSpawnObject(timeSpawn.objectA, "Object A"); // Tắt sau khi spawn xong
        
        // Spawn Object B - Chờ 20s (tăng gấp đôi từ 10s)
        yield return new WaitForSeconds(20f);
        this.ActivateTimeSpawnObject(timeSpawn.objectB, "Object B");
        yield return new WaitForSeconds(3f); // Chờ spawn đủ số lượng
        this.DeactivateTimeSpawnObject(timeSpawn.objectB, "Object B"); // Tắt sau khi spawn xong
        
        // Spawn Object C - Chờ 30s (tăng gấp đôi từ 15s)
        yield return new WaitForSeconds(30f);
        this.ActivateTimeSpawnObject(timeSpawn.objectC, "Object C");
        yield return new WaitForSeconds(3f); // Chờ spawn đủ số lượng
        this.DeactivateTimeSpawnObject(timeSpawn.objectC, "Object C"); // Tắt sau khi spawn xong
        
        // Spawn Object D - Chờ 40s (tăng gấp đôi từ 20s)
        yield return new WaitForSeconds(40f);
        this.ActivateTimeSpawnObject(timeSpawn.objectD, "Object D");
        yield return new WaitForSeconds(3f); // Chờ spawn đủ số lượng
        this.DeactivateTimeSpawnObject(timeSpawn.objectD, "Object D"); // Tắt sau khi spawn xong
        
        // Spawn Object Boss - Chờ 60s
        yield return new WaitForSeconds(60f);
        this.ActivateTimeSpawnObject(timeSpawn.objectBoss, "Object Boss");
        yield return new WaitForSeconds(3f); // Chờ spawn đủ số lượng
        this.DeactivateTimeSpawnObject(timeSpawn.objectBoss, "Object Boss"); // Tắt sau khi spawn xong
        
        Debug.Log("Wave 5 sequential spawn completed!");
    }
    
    protected virtual void DeactivateAllTimeSpawnObjects()
    {
        if (timeSpawn == null) return;
        
        // Tắt tất cả objects
        if (timeSpawn.objectA != null) timeSpawn.objectA.SetActive(false);
        if (timeSpawn.objectB != null) timeSpawn.objectB.SetActive(false);
        if (timeSpawn.objectC != null) timeSpawn.objectC.SetActive(false);
        if (timeSpawn.objectD != null) timeSpawn.objectD.SetActive(false);
        if (timeSpawn.objectBoss != null) timeSpawn.objectBoss.SetActive(false);
        
        // Tắt tất cả EnemySpawning components
        GameObject[] objects = { timeSpawn.objectA, timeSpawn.objectB, timeSpawn.objectC, timeSpawn.objectD, timeSpawn.objectBoss };
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                EnemySpawning enemySpawning = obj.GetComponent<EnemySpawning>();
                if (enemySpawning != null)
                {
                    enemySpawning.enabled = false;
                }
            }
        }
        
        Debug.Log("All TimeSpawn objects deactivated");
    }
    
    protected virtual void ActivateTimeSpawnObject(GameObject obj, string objectName)
    {
        if (obj != null)
        {
            obj.SetActive(true);
            Debug.Log($"Activated {objectName} for Wave 5");
            
            // Bật EnemySpawning component nếu có
            EnemySpawning enemySpawning = obj.GetComponent<EnemySpawning>();
            if (enemySpawning != null)
            {
                enemySpawning.enabled = true;
                Debug.Log($"Enabled EnemySpawning for {objectName}");
            }
        }
        else
        {
            Debug.LogWarning($"{objectName} is null!");
        }
    }
    
    protected virtual void DeactivateTimeSpawnObject(GameObject obj, string objectName)
    {
        if (obj != null)
        {
            // Tắt EnemySpawning component trước
            EnemySpawning enemySpawning = obj.GetComponent<EnemySpawning>();
            if (enemySpawning != null)
            {
                enemySpawning.enabled = false;
                Debug.Log($"Disabled EnemySpawning for {objectName}");
            }
            
            // Tắt GameObject
            obj.SetActive(false);
            Debug.Log($"Deactivated {objectName} for Wave 5");
        }
        else
        {
            Debug.LogWarning($"{objectName} is null!");
        }
    }
    
    protected virtual bool CheckWaveComplete(EnemyWaveData wave)
    {
        if (wave.enemySpawning != null)
        {
            // Nếu là wave cuối (wave 5), kiểm tra tất cả enemies từ TimeSpawn
            if (wave.waveNumber == 5)
            {
                return this.CheckWave5Complete();
            }
            else
            {
                // Các wave khác sử dụng EnemySpawning.SpawnedEnemies
                int aliveEnemies = 0;
                int totalEnemies = wave.enemySpawning.SpawnedEnemies.Count;
                
                foreach (var enemy in wave.enemySpawning.SpawnedEnemies)
                {
                    if (enemy != null && !enemy.EnemyDamageReceiver.IsDead())
                    {
                        aliveEnemies++;
                    }
                }
                
                bool waveComplete = aliveEnemies == 0 && totalEnemies > 0;
                
                if (waveComplete)
                {
                    Debug.Log($"Wave {wave.waveNumber} complete! All {totalEnemies} enemies defeated.");
                }
                else
                {
                    Debug.Log($"Wave {wave.waveNumber} progress: {totalEnemies - aliveEnemies}/{totalEnemies} enemies defeated.");
                }
                
                return waveComplete;
            }
        }
        else
        {
            // Fallback: sử dụng wave.IsWaveComplete()
            return wave.IsWaveComplete();
        }
    }
    
    protected virtual bool CheckWave5Complete()
    {
        if (timeSpawn == null)
        {
            Debug.LogError("TimeSpawn not found for Wave 5 completion check!");
            return true;
        }
        
        // Kiểm tra tất cả enemies từ TimeSpawn objects
        int aliveEnemies = 0;
        int totalEnemies = 0;
        
        // Kiểm tra enemies từ tất cả TimeSpawn objects (không cần activeInHierarchy)
        GameObject[] timeSpawnObjects = { timeSpawn.objectA, timeSpawn.objectB, timeSpawn.objectC, timeSpawn.objectD, timeSpawn.objectBoss };
        
        foreach (var obj in timeSpawnObjects)
        {
            if (obj != null) // Bỏ điều kiện activeInHierarchy
            {
                // Tìm EnemySpawning component trong object
                EnemySpawning enemySpawning = obj.GetComponent<EnemySpawning>();
                if (enemySpawning != null)
                {
                    foreach (var enemy in enemySpawning.SpawnedEnemies)
                    {
                        totalEnemies++;
                        if (enemy != null && !enemy.EnemyDamageReceiver.IsDead())
                        {
                            aliveEnemies++;
                        }
                    }
                }
            }
        }
        
        // Thêm kiểm tra enemies trong activeEnemies list
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && !enemy.EnemyDamageReceiver.IsDead())
            {
                aliveEnemies++;
                totalEnemies++;
            }
        }
        
        bool waveComplete = aliveEnemies == 0 && totalEnemies > 0;
        
        if (waveComplete)
        {
            Debug.Log($"Final wave 5 complete! All {totalEnemies} enemies defeated.");
        }
        else
        {
            Debug.Log($"Final wave 5 progress: {totalEnemies - aliveEnemies}/{totalEnemies} enemies defeated.");
        }
        
        return waveComplete;
    }
    
    protected virtual void WaitForNextWaveButton()
    {
        isWaitingForButtonPress = true;
        Debug.Log($"Wave {currentWaveIndex} completed! Press button to start Wave {currentWaveIndex + 1}");
    }
    
    protected virtual void SpawnEnemy(EnemyWaveData wave)
    {
        if (wave.enemyPrefab == null)
        {
            Debug.LogError($"Enemy prefab is null for wave {wave.waveNumber}!");
            return;
        }
        
        // Sử dụng EnemySpawning có sẵn thay vì spawn trực tiếp
        if (wave.enemySpawning != null)
        {
            // Bật EnemySpawning để nó tự spawn enemies
            wave.enemySpawning.enabled = true;
            Debug.Log($"Enabled EnemySpawning for wave {wave.waveNumber}");
        }
        else
        {
            Debug.LogWarning($"No EnemySpawning reference for wave {wave.waveNumber}! Using direct spawn...");
            this.SpawnEnemyDirectly(wave);
        }
    }
    
    protected virtual void SpawnEnemyDirectly(EnemyWaveData wave)
    {
        if (this.enemyManagerCtrl == null)
        {
            Debug.LogError("EnemyManagerCtrl is null! Cannot spawn enemy.");
            return;
        }
        
        // Spawn enemy using EnemyManagerCtrl
        EnemyCtrl newEnemy = this.enemyManagerCtrl.EnemySpawner.Spawn(wave.enemyPrefab, transform.position);
        if (newEnemy != null)
        {
            newEnemy.gameObject.SetActive(true);
            activeEnemies.Add(newEnemy);
            
            // Setup enemy behavior tree
            if (newEnemy.EnemyBTree != null)
            {
                newEnemy.EnemyBTree.BuildBehaviorTree();
                newEnemy.EnemyBTree.ResetBTree();
                newEnemy.EnemyBTree.StartBTree();
            }
            
            // Setup enemy scream
            if (newEnemy.EnemyScream != null)
            {
                newEnemy.EnemyScream.SetCanScream(true);
            }
            
            // Subscribe to enemy death event
            this.SubscribeToEnemyDeath(newEnemy, wave);
            
            Debug.Log($"Spawned enemy directly for wave {wave.waveNumber}");
        }
    }
    
    protected virtual void SubscribeToEnemyDeath(EnemyCtrl enemy, EnemyWaveData wave)
    {
        // Tìm EnemyDamageReceiver và subscribe to death event
        EnemyDamageReceiver damageReceiver = enemy.GetComponent<EnemyDamageReceiver>();
        if (damageReceiver != null)
        {
            // Tạo một coroutine để theo dõi enemy death
            StartCoroutine(MonitorEnemyDeath(enemy, wave));
        }
    }
    
    protected virtual IEnumerator MonitorEnemyDeath(EnemyCtrl enemy, EnemyWaveData wave)
    {
        while (enemy != null && !enemy.EnemyDamageReceiver.IsDead())
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Enemy đã chết
        if (enemy != null)
        {
            activeEnemies.Remove(enemy);
            wave.enemiesKilled++;
            Debug.Log($"Enemy killed! Wave {wave.waveNumber}: {wave.enemiesKilled}/{wave.GetEnemyCount()}");
        }
    }
    
    protected virtual void CompleteAllWaves()
    {
        allWavesCompleted = true;
        isSpawning = false;
        isWaitingForButtonPress = false;
        
        Debug.Log("All waves completed! Opening Wood Baluster...");
        
        // Bật lại Wood Baluster
        this.SetWoodBalusterVisibility(true);
        
        // Clear active enemies list
        activeEnemies.Clear();
        
        // Thông báo cho GameResultManager để hiển thị win panel
        this.NotifyGameResultManager();
    }
    
    /// <summary>
    /// Thông báo cho GameResultManager khi hoàn thành tất cả waves
    /// </summary>
    protected virtual void NotifyGameResultManager()
    {
        if (GameResultManager.Instance != null)
        {
            Debug.Log("Notifying GameResultManager: All waves completed!");
            GameResultManager.Instance.OnAllWavesCompleted();
        }
        else
        {
            Debug.LogWarning("GameResultManager.Instance is null! Cannot notify wave completion.");
        }
    }
    
    protected virtual void SetWoodBalusterVisibility(bool visible)
    {
        if (woodBalusterRenderer != null)
        {
            woodBalusterRenderer.enabled = visible;
            Debug.Log($"Wood Baluster visibility set to: {visible}");
        }
    }
    
    public virtual void ResetWaveSystem()
    {
        // Dừng tất cả coroutines
        StopAllCoroutines();
        
        // Despawn tất cả enemies đang hoạt động
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(false);
            }
        }
        activeEnemies.Clear();
        
        // Reset waves
        this.InitializeWaves();
        
        // Bật lại Wood Baluster
        this.SetWoodBalusterVisibility(true);
        
        Debug.Log("Wave system reset!");
    }
    
    public virtual bool IsWaveSystemActive()
    {
        return isSpawning || isWaitingForButtonPress || (!allWavesCompleted && currentWaveIndex < waves.Count);
    }
    
    public virtual bool IsWaitingForButtonPress()
    {
        return isWaitingForButtonPress;
    }
    
    public virtual bool IsSpawning()
    {
        return isSpawning;
    }
    
    public virtual int GetCurrentWaveNumber()
    {
        return currentWaveIndex + 1;
    }
    
    // Public property để truy cập waves từ bên ngoài
    public virtual List<EnemyWaveData> Waves
    {
        get { return waves; }
        set { waves = value; }
    }
    
    public virtual int GetTotalWaves()
    {
        return waves.Count;
    }
    
    public virtual bool AreAllWavesCompleted()
    {
        return allWavesCompleted;
    }
}
