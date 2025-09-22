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
        
        if (autoSetupWaves)
        {
            this.SetupWavesFromTimeSpawn();
        }
    }
    
    protected virtual void SetupWavesFromTimeSpawn()
    {
        if (timeSpawn == null)
        {
            Debug.LogError("TimeSpawn reference is null! Please assign it in inspector.");
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
                
                waves.Add(wave);
                Debug.Log($"Created Wave {i + 1}: {enemyPrefabs[i].name} (maxSpawn: {enemySpawningRefs[i]?.MaxSpawn ?? 5}, spawnSpeed: {enemySpawningRefs[i]?.SpawnSpeed ?? 1f})");
            }
        }
        
        // Gán waves vào EnemyWaveManager
        EnemyWaveManager.Instance.Waves = waves;
        Debug.Log($"Successfully setup {waves.Count} waves using EnemySpawning configurations!");
    }
    
    [ContextMenu("Setup Waves Manually")]
    public virtual void SetupWavesManually()
    {
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
}