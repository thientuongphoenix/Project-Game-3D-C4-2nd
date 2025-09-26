using UnityEngine;

[System.Serializable]
public class EnemyWaveData
{
    [Header("Wave Information")]
    public int waveNumber = 1;
    public string waveName = "Wave 1";
    
    [Header("Enemy Settings")]
    public EnemyCtrl enemyPrefab;
    public EnemySpawning enemySpawning; // Reference đến EnemySpawning để lấy cấu hình
    public int enemyCount = 5; // Sẽ được lấy từ enemySpawning.maxSpawn
    public float spawnInterval = 1f; // Sẽ được lấy từ enemySpawning.spawnSpeed
    public float waveDelay = 2f; // Thời gian chờ trước khi bắt đầu wave
    
    [Header("Wave Status")]
    public bool isCompleted = false;
    public int enemiesSpawned = 0;
    public int enemiesKilled = 0;
    
    public bool IsWaveComplete()
    {
        return enemiesKilled >= this.GetEnemyCount();
    }
    
    public bool CanSpawnMore()
    {
        return enemiesSpawned < this.GetEnemyCount();
    }
    
    public int GetEnemyCount()
    {
        if (enemySpawning != null)
        {
            return enemySpawning.MaxSpawn;
        }
        return enemyCount; // Fallback nếu không có enemySpawning
    }
    
    public float GetSpawnInterval()
    {
        if (enemySpawning != null)
        {
            return enemySpawning.SpawnSpeed;
        }
        return spawnInterval; // Fallback nếu không có enemySpawning
    }
    
    public void ResetWave()
    {
        isCompleted = false;
        enemiesSpawned = 0;
        enemiesKilled = 0;
    }
}
