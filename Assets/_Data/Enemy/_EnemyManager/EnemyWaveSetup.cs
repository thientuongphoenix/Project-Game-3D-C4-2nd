using UnityEngine;
using System.Collections.Generic;

public class EnemyWaveSetup : SaiMonoBehaviour
{
    [Header("Auto Setup from TimeSpawn")]
    [SerializeField] protected TimeSpawn timeSpawn;
    [SerializeField] protected bool autoSetupWaves = true;
    
    [Header("Enemy Prefabs (assign in inspector)")]
    [SerializeField] protected EnemyCtrl enemyType1;
    [SerializeField] protected EnemyCtrl enemyType2;
    [SerializeField] protected EnemyCtrl enemyType3;
    [SerializeField] protected EnemyCtrl enemyType4;
    [SerializeField] protected EnemyCtrl enemyType5;
    
    [Header("EnemySpawning References (assign in inspector)")]
    [SerializeField] protected EnemySpawning enemySpawning1;
    [SerializeField] protected EnemySpawning enemySpawning2;
    [SerializeField] protected EnemySpawning enemySpawning3;
    [SerializeField] protected EnemySpawning enemySpawning4;
    [SerializeField] protected EnemySpawning enemySpawning5;
    
    [Header("Wave Settings")]
    [SerializeField] protected float waveDelay = 2f;
    
    protected override void Start()
    {
        base.Start();
        
        // Tự động tìm TimeSpawn nếu chưa được gán
        this.LoadTimeSpawn();
        
        if (autoSetupWaves)
        {
            this.SetupWavesFromTimeSpawn();
        }
    }
    
    protected virtual void SetupWavesFromTimeSpawn()
    {
        if (timeSpawn == null)
        {
            Debug.LogWarning("TimeSpawn reference is null! Setting up waves without TimeSpawn configuration.");
            this.SetupWavesWithoutTimeSpawn();
            return;
        }
        
        if (EnemyWaveManager.Instance == null)
        {
            Debug.LogError("EnemyWaveManager not found in scene!");
            return;
        }
        
        // Tạo 5 waves dựa trên cấu hình có sẵn
        List<EnemyWaveData> waves = new List<EnemyWaveData>();
        
        EnemyCtrl[] enemyPrefabs = { enemyType1, enemyType2, enemyType3, enemyType4, enemyType5 };
        EnemySpawning[] enemySpawningRefs = { enemySpawning1, enemySpawning2, enemySpawning3, enemySpawning4, enemySpawning5 };
        
        for (int i = 0; i < 5; i++)
        {
            if (enemyPrefabs[i] != null)
            {
                EnemyWaveData wave = new EnemyWaveData();
                wave.waveNumber = i + 1;
                wave.waveName = $"Wave {i + 1}";
                wave.enemyPrefab = enemyPrefabs[i];
                wave.enemySpawning = enemySpawningRefs[i]; // Gán reference đến EnemySpawning
                wave.waveDelay = waveDelay;
                
                // Gán specific enemy prefab cho EnemySpawning
                if (enemySpawningRefs[i] != null)
                {
                    enemySpawningRefs[i].SpecificEnemyPrefab = enemyPrefabs[i];
                    Debug.Log($"Assigned specific prefab {enemyPrefabs[i].name} to EnemySpawning {i + 1}");
                }
                
                waves.Add(wave);
                Debug.Log($"Created Wave {i + 1}: {enemyPrefabs[i].name} (maxSpawn: {enemySpawningRefs[i]?.MaxSpawn ?? 5}, spawnSpeed: {enemySpawningRefs[i]?.SpawnSpeed ?? 1f})");
            }
        }
        
        // Gán waves vào EnemyWaveManager
        EnemyWaveManager.Instance.Waves = waves;
        Debug.Log($"Successfully setup {waves.Count} waves using EnemySpawning configurations!");
    }
    
    // Phương thức tạo waves khi không có TimeSpawn
    protected virtual void SetupWavesWithoutTimeSpawn()
    {
        if (EnemyWaveManager.Instance == null)
        {
            Debug.LogError("EnemyWaveManager not found in scene!");
            return;
        }
        
        Debug.Log("=== SETTING UP WAVES WITHOUT TIMESPAWN ===");
        
        // Tạo 5 waves với cấu hình mặc định
        List<EnemyWaveData> waves = new List<EnemyWaveData>();
        
        EnemyCtrl[] enemyPrefabs = { enemyType1, enemyType2, enemyType3, enemyType4, enemyType5 };
        EnemySpawning[] enemySpawningRefs = { enemySpawning1, enemySpawning2, enemySpawning3, enemySpawning4, enemySpawning5 };
        
        Debug.Log($"Enemy Prefabs: Type1={enemyType1 != null}, Type2={enemyType2 != null}, Type3={enemyType3 != null}, Type4={enemyType4 != null}, Type5={enemyType5 != null}");
        Debug.Log($"EnemySpawning Refs: Spawn1={enemySpawning1 != null}, Spawn2={enemySpawning2 != null}, Spawn3={enemySpawning3 != null}, Spawn4={enemySpawning4 != null}, Spawn5={enemySpawning5 != null}");
        
        for (int i = 0; i < 5; i++)
        {
            if (enemyPrefabs[i] != null)
            {
                EnemyWaveData wave = new EnemyWaveData();
                wave.waveNumber = i + 1;
                wave.waveName = $"Wave {i + 1}";
                wave.enemyPrefab = enemyPrefabs[i];
                wave.enemySpawning = enemySpawningRefs[i];
                wave.waveDelay = waveDelay;
                
                waves.Add(wave);
                Debug.Log($"Created Wave {i + 1}: {enemyPrefabs[i].name} (maxSpawn: {enemySpawningRefs[i]?.MaxSpawn ?? 5}, spawnSpeed: {enemySpawningRefs[i]?.SpawnSpeed ?? 1f})");
            }
            else
            {
                Debug.LogWarning($"Enemy Type {i + 1} is NULL! Creating default wave {i + 1}");
                // Tạo wave mặc định ngay cả khi không có enemy prefab
                EnemyWaveData wave = new EnemyWaveData();
                wave.waveNumber = i + 1;
                wave.waveName = $"Wave {i + 1}";
                wave.enemyPrefab = null;
                wave.enemySpawning = enemySpawningRefs[i];
                wave.waveDelay = waveDelay;
                
                waves.Add(wave);
                Debug.Log($"Created default Wave {i + 1} (no enemy prefab)");
            }
        }
        
        // Đảm bảo có ít nhất 1 wave được tạo
        if (waves.Count == 0)
        {
            Debug.LogWarning("No waves created! Creating at least 1 default wave...");
            this.CreateDefaultWaves();
            return;
        }
        
        // Gán waves vào EnemyWaveManager
        EnemyWaveManager.Instance.Waves = waves;
        Debug.Log($"Successfully setup {waves.Count} waves without TimeSpawn configuration!");
        
        // Kiểm tra xem waves có được gán đúng không
        if (EnemyWaveManager.Instance.Waves != null && EnemyWaveManager.Instance.Waves.Count > 0)
        {
            Debug.Log($"EnemyWaveManager now has {EnemyWaveManager.Instance.Waves.Count} waves configured!");
        }
        else
        {
            Debug.LogError("Waves were not properly assigned to EnemyWaveManager!");
        }
        
        Debug.Log("=== END SETUP WAVES WITHOUT TIMESPAWN ===");
    }
    
    // Thêm phương thức tạo waves mặc định nếu không có enemy prefabs
    protected virtual void CreateDefaultWaves()
    {
        if (EnemyWaveManager.Instance == null)
        {
            Debug.LogError("EnemyWaveManager not found in scene!");
            return;
        }
        
        Debug.Log("=== CREATING DEFAULT WAVES ===");
        
        // Tạo 5 waves mặc định với dummy data
        List<EnemyWaveData> waves = new List<EnemyWaveData>();
        
        for (int i = 0; i < 5; i++)
        {
            EnemyWaveData wave = new EnemyWaveData();
            wave.waveNumber = i + 1;
            wave.waveName = $"Wave {i + 1}";
            wave.enemyPrefab = null; // Sẽ được gán sau khi tìm thấy enemy prefabs
            wave.enemySpawning = null; // Sẽ được gán sau khi tìm thấy EnemySpawning
            wave.waveDelay = waveDelay;
            
            waves.Add(wave);
            Debug.Log($"Created default Wave {i + 1}");
        }
        
        // Gán waves vào EnemyWaveManager
        EnemyWaveManager.Instance.Waves = waves;
        Debug.Log($"Successfully created {waves.Count} default waves!");
        
        Debug.Log("=== END CREATING DEFAULT WAVES ===");
    }
    
    [ContextMenu("Setup Waves Manually")]
    public virtual void SetupWavesManually()
    {
        // Tự động tìm TimeSpawn trước khi setup
        this.LoadTimeSpawn();
        this.SetupWavesFromTimeSpawn();
    }
    
    [ContextMenu("Test Wave System")]
    public virtual void TestWaveSystem()
    {
        if (EnemyWaveManager.Instance != null)
        {
            EnemyWaveManager.Instance.StartWaveSystem();
        }
    }
    
    // Thêm phương thức tự động tìm TimeSpawn
    protected virtual void LoadTimeSpawn()
    {
        if (this.timeSpawn != null) return;
        
        this.timeSpawn = FindObjectOfType<TimeSpawn>();
        if (this.timeSpawn == null)
        {
            Debug.LogWarning("TimeSpawn not found in scene! EnemyWaveSetup will skip TimeSpawn setup.");
        }
        else
        {
            Debug.Log("TimeSpawn found and loaded automatically for EnemyWaveSetup");
        }
    }
}