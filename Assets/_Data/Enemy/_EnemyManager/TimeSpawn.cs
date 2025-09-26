using UnityEngine;
using System.Collections.Generic;

public class TimeSpawn : MonoBehaviour
{
    [Header("Enemy Prefabs để spawn (tương thích với EnemyWaveSetup)")]
    public EnemyCtrl enemyType1; // Quái loại 1 - EnemyCtrl thay vì GameObject
    public EnemyCtrl enemyType2; // Quái loại 2
    public EnemyCtrl enemyType3; // Quái loại 3
    public EnemyCtrl enemyType4; // Quái loại 4
    public EnemyCtrl enemyType5; // Boss

    [Header("EnemySpawning References (tương thích với EnemyWaveSetup)")]
    public EnemySpawning enemySpawning1; // Reference đến EnemySpawning cho loại 1
    public EnemySpawning enemySpawning2; // Reference đến EnemySpawning cho loại 2
    public EnemySpawning enemySpawning3; // Reference đến EnemySpawning cho loại 3
    public EnemySpawning enemySpawning4; // Reference đến EnemySpawning cho loại 4
    public EnemySpawning enemySpawning5; // Reference đến EnemySpawning cho loại 5

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // Các điểm spawn quái

    [Header("Thời gian spawn từng loại quái")]
    public float time1; // Thời gian spawn quái loại 1
    public float time2; // Thời gian spawn quái loại 2
    public float time3; // Thời gian spawn quái loại 3
    public float time4; // Thời gian spawn quái loại 4
    public float time5; // Thời gian spawn Boss

    [Header("Wave 5 - Spawn từng loại quái theo thời gian")]
    public bool useWave5SequentialSpawn = true; // Sử dụng spawn tuần tự cho Wave 5
    public float wave5StartTime = 0f; // Thời gian bắt đầu Wave 5 (ngay lập tức)
    public float[] wave5SpawnTimes = { 0f, 5f, 10f, 15f, 20f }; // Thời gian spawn từng loại trong Wave 5
    public int[] wave5SpawnCounts = { 3, 4, 5, 6, 1 }; // Số lượng spawn từng loại (cuối cùng là Boss)
    public bool wave5BossIsFinalBoss = true; // Boss cuối là Final Boss (Player thắng khi chết)

    [Header("Spawn Settings")]
    public bool autoStart = false; // Tắt tự động bắt đầu - chỉ dùng cho Wave 5
    public bool useEnemySpawning = true; // Sử dụng EnemySpawning để spawn thay vì spawn trực tiếp
    public bool disableWave1to4Spawn = true; // Tắt spawn Wave 1-4 để tránh conflict với EnemyWaveSetup

    [Header("Debug")]
    public bool showDebugLogs = true;

    private float currentTime = 0f;
    private bool hasStarted = false;
    private List<bool> spawnedTypes = new List<bool>(); // Theo dõi loại quái đã spawn
    
    // Wave 5 tracking
    private bool wave5Started = false;
    private List<bool> wave5SpawnedTypes = new List<bool>(); // Theo dõi loại quái đã spawn trong Wave 5
    private GameObject finalBoss = null; // Reference đến Final Boss
    
    // Public property để truy cập finalBoss từ bên ngoài
    public GameObject FinalBoss => finalBoss;

    void Start()
    {
        // Khởi tạo danh sách theo dõi spawn
        spawnedTypes = new List<bool> { false, false, false, false, false };
        
        // Khởi tạo Wave 5 tracking
        wave5SpawnedTypes = new List<bool> { false, false, false, false, false };
        
        // Tự động bắt đầu nếu được cấu hình
        if (autoStart)
        {
            StartSpawning();
        }
        
        // Log cấu hình để debug
        if (showDebugLogs)
        {
            Debug.Log($"TimeSpawn Configuration:");
            Debug.Log($"- autoStart: {autoStart}");
            Debug.Log($"- disableWave1to4Spawn: {disableWave1to4Spawn}");
            Debug.Log($"- useWave5SequentialSpawn: {useWave5SequentialSpawn}");
        }
    }

    void Update()
    {
        if (!hasStarted) return;
        
        currentTime += Time.deltaTime;

        // Kiểm tra và spawn từng loại quái theo thời gian (Wave 1-4)
        CheckAndSpawnEnemies();
        
        // Kiểm tra Wave 5 - Spawn tuần tự
        if (useWave5SequentialSpawn)
        {
            CheckAndSpawnWave5();
        }
        
        // Kiểm tra Final Boss chết
        CheckFinalBossDeath();
    }
    
    /// <summary>
    /// Bắt đầu quá trình spawn
    /// </summary>
    public void StartSpawning()
    {
        hasStarted = true;
        currentTime = 0f;
        spawnedTypes = new List<bool> { false, false, false, false, false };
        
        if (showDebugLogs)
        {
            Debug.Log("TimeSpawn: Bắt đầu spawn quái theo thời gian!");
        }
    }
    
    /// <summary>
    /// Bắt đầu TimeSpawn cho Wave 5 (được gọi từ EnemyWaveManager)
    /// </summary>
    public void StartWave5Spawning()
    {
        if (!hasStarted)
        {
            StartSpawning();
        }
        
        if (showDebugLogs)
        {
            Debug.Log("TimeSpawn: Bắt đầu Wave 5 spawning!");
        }
    }
    
    /// <summary>
    /// Dừng quá trình spawn
    /// </summary>
    public void StopSpawning()
    {
        hasStarted = false;
        
        if (showDebugLogs)
        {
            Debug.Log("TimeSpawn: Dừng spawn quái!");
        }
    }
    
    /// <summary>
    /// Kiểm tra và spawn quái theo thời gian (Wave 1-4)
    /// </summary>
    void CheckAndSpawnEnemies()
    {
        // Tắt spawn Wave 1-4 nếu được cấu hình để tránh conflict với EnemyWaveSetup
        if (disableWave1to4Spawn)
        {
            if (showDebugLogs && currentTime < 1f) // Chỉ log 1 lần
            {
                Debug.Log("TimeSpawn: Wave 1-4 spawn disabled - EnemyWaveSetup will handle these waves");
            }
            return;
        }
        
        // Debug log để kiểm tra
        if (showDebugLogs && currentTime > 0.1f && currentTime < 0.2f)
        {
            Debug.Log($"TimeSpawn: CheckAndSpawnEnemies called - disableWave1to4Spawn: {disableWave1to4Spawn}, hasStarted: {hasStarted}");
        }
        
        // Spawn quái loại 1
        if (currentTime >= time1 && !spawnedTypes[0] && enemyType1 != null)
        {
            SpawnEnemyType(0, enemyType1);
            spawnedTypes[0] = true;
        }
        
        // Spawn quái loại 2
        if (currentTime >= time2 && !spawnedTypes[1] && enemyType2 != null)
        {
            SpawnEnemyType(1, enemyType2);
            spawnedTypes[1] = true;
        }
        
        // Spawn quái loại 3
        if (currentTime >= time3 && !spawnedTypes[2] && enemyType3 != null)
        {
            SpawnEnemyType(2, enemyType3);
            spawnedTypes[2] = true;
        }
        
        // Spawn quái loại 4
        if (currentTime >= time4 && !spawnedTypes[3] && enemyType4 != null)
        {
            SpawnEnemyType(3, enemyType4);
            spawnedTypes[3] = true;
        }
        
        // Không spawn Boss ở đây nữa - sẽ spawn trong Wave 5
    }
    
    /// <summary>
    /// Kiểm tra và spawn Wave 5 - Spawn tuần tự từng loại quái
    /// </summary>
    void CheckAndSpawnWave5()
    {
        // Bắt đầu Wave 5
        if (currentTime >= wave5StartTime && !wave5Started)
        {
            wave5Started = true;
            if (showDebugLogs)
            {
                Debug.Log("🎯 WAVE 5 BẮT ĐẦU - Spawn tuần tự từng loại quái!");
            }
        }
        
        if (!wave5Started) return;
        
        // Spawn từng loại quái theo thời gian đã định
        for (int i = 0; i < wave5SpawnTimes.Length && i < wave5SpawnedTypes.Count; i++)
        {
            if (currentTime >= wave5SpawnTimes[i] && !wave5SpawnedTypes[i])
            {
                SpawnWave5EnemyType(i);
                wave5SpawnedTypes[i] = true;
            }
        }
    }
    
    /// <summary>
    /// Spawn loại quái trong Wave 5
    /// </summary>
    void SpawnWave5EnemyType(int typeIndex)
    {
        EnemyCtrl[] enemyPrefabs = { enemyType1, enemyType2, enemyType3, enemyType4, enemyType5 };
        EnemySpawning[] enemySpawningRefs = { enemySpawning1, enemySpawning2, enemySpawning3, enemySpawning4, enemySpawning5 };
        
        if (typeIndex >= 0 && typeIndex < enemyPrefabs.Length && enemyPrefabs[typeIndex] != null)
        {
            // Spawn với số lượng tùy chỉnh cho Wave 5
            int spawnCount = (typeIndex < wave5SpawnCounts.Length) ? wave5SpawnCounts[typeIndex] : 1;
            
            if (useEnemySpawning && typeIndex < enemySpawningRefs.Length && enemySpawningRefs[typeIndex] != null)
            {
                // Sử dụng EnemySpawning với số lượng tùy chỉnh
                StartCoroutine(SpawnWave5WithCustomCount(enemyPrefabs[typeIndex], enemySpawningRefs[typeIndex], spawnCount));
            }
            else
            {
                // Spawn trực tiếp
                StartCoroutine(SpawnWave5Directly(enemyPrefabs[typeIndex], spawnCount));
            }
            
            // Lưu reference đến Final Boss (sẽ được gán trong SpawnWave5WithCustomCount)
            if (typeIndex == enemyPrefabs.Length - 1 && wave5BossIsFinalBoss)
            {
                if (showDebugLogs)
                {
                    Debug.Log("👑 FINAL BOSS SPAWNING - Player thắng khi Boss chết!");
                }
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"🎯 Wave 5: Spawn {spawnCount} {enemyPrefabs[typeIndex].name} tại {currentTime:F1}s");
            }
        }
    }
    
    /// <summary>
    /// Spawn Wave 5 sử dụng EnemySpawning với số lượng tùy chỉnh
    /// </summary>
    System.Collections.IEnumerator SpawnWave5WithCustomCount(EnemyCtrl enemyPrefab, EnemySpawning enemySpawning, int spawnCount)
    {
        // EnemySpawning kế thừa từ EnemyManagerAbstract và có enemyManagerCtrl property
        if (enemySpawning.EnemyManagerCtrl != null && enemySpawning.EnemyManagerCtrl.EnemySpawner != null)
        {
            // Spawn từng con một
            for (int i = 0; i < spawnCount; i++)
            {
                // Spawn 1 con tại vị trí của EnemySpawning
                EnemyCtrl newEnemy = enemySpawning.EnemyManagerCtrl.EnemySpawner.Spawn(enemyPrefab, enemySpawning.transform.position);
                if (newEnemy != null)
                {
                    newEnemy.gameObject.SetActive(true);
                    
                    // Khởi tạo Behavior Tree
                    if (newEnemy.EnemyBTree != null)
                    {
                        newEnemy.EnemyBTree.BuildBehaviorTree();
                        newEnemy.EnemyBTree.ResetBTree();
                        newEnemy.EnemyBTree.StartBTree();
                    }
                    
                    // Khởi tạo EnemyScream
                    if (newEnemy.EnemyScream != null)
                    {
                        newEnemy.EnemyScream.SetCanScream(true);
                    }
                    
                    // Lưu reference đến Final Boss (chỉ con cuối cùng của loại cuối cùng)
                    if (i == spawnCount - 1 && wave5BossIsFinalBoss)
                    {
                        finalBoss = newEnemy.gameObject;
                        
                        // Set HP cho Final Boss - Sử dụng HP từ prefab
                        if (newEnemy.EnemyDamageReceiver != null)
                        {
                            // Lấy HP từ prefab (enemyPrefab)
                            int bossMaxHP = enemyPrefab.EnemyDamageReceiver.MaxHP;
                            
                            // Sử dụng reflection để set maxHP và currentHP
                            var maxHPField = newEnemy.EnemyDamageReceiver.GetType().GetField("maxHP", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            var currentHPField = newEnemy.EnemyDamageReceiver.GetType().GetField("currentHP", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            
                            if (maxHPField != null && currentHPField != null)
                            {
                                maxHPField.SetValue(newEnemy.EnemyDamageReceiver, bossMaxHP);
                                currentHPField.SetValue(newEnemy.EnemyDamageReceiver, bossMaxHP);
                                
                                if (showDebugLogs)
                                {
                                    Debug.Log($"👑 FINAL BOSS SPAWNED - HP set to {bossMaxHP} (from prefab)! Player thắng khi Boss chết!");
                                }
                            }
                            else
                            {
                                Debug.LogError("❌ Cannot set Final Boss HP - Reflection failed!");
                            }
                        }
                        else
                        {
                            Debug.LogError("❌ Final Boss EnemyDamageReceiver is null!");
                        }
                    }
                }
                
                // Chờ một chút trước khi spawn con tiếp theo
                if (i < spawnCount - 1)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
        else
        {
            Debug.LogError("TimeSpawn: EnemySpawning không có enemyManagerCtrl hoặc EnemySpawner!");
        }
    }
    
    /// <summary>
    /// Spawn Wave 5 trực tiếp với số lượng tùy chỉnh
    /// </summary>
    System.Collections.IEnumerator SpawnWave5Directly(EnemyCtrl enemyPrefab, int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject spawnedEnemy = Instantiate(enemyPrefab.gameObject, spawnPoint.position, spawnPoint.rotation);
                
                // Lưu reference đến Final Boss (chỉ con cuối cùng của loại cuối cùng)
                if (i == spawnCount - 1 && wave5BossIsFinalBoss)
                {
                    finalBoss = spawnedEnemy;
                    if (showDebugLogs)
                    {
                        Debug.Log("👑 FINAL BOSS SPAWNED - Player thắng khi Boss chết!");
                    }
                }
            }
            
            if (i < spawnCount - 1)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    /// <summary>
    /// Kiểm tra Final Boss chết
    /// </summary>
    void CheckFinalBossDeath()
    {
        if (finalBoss == null || !wave5BossIsFinalBoss) 
        {
            if (showDebugLogs && currentTime > 0.1f && currentTime < 0.2f)
            {
                Debug.Log($"TimeSpawn: CheckFinalBossDeath - finalBoss: {finalBoss != null}, wave5BossIsFinalBoss: {wave5BossIsFinalBoss}");
            }
            return;
        }
        
        // Kiểm tra Final Boss còn tồn tại không
        if (finalBoss != null)
        {
            // Kiểm tra health của Final Boss
            var enemyCtrl = finalBoss.GetComponent<EnemyCtrl>();
            if (enemyCtrl != null)
            {
                var damageReceiver = finalBoss.GetComponent<EnemyDamageReceiver>();
                if (damageReceiver != null && damageReceiver.IsDead())
                {
                    Debug.Log("🎉 TimeSpawn: Final Boss is DEAD! Triggering win condition...");
                    // Final Boss chết - Player thắng!
                    OnFinalBossDeath();
                }
                else if (showDebugLogs && currentTime > 0.1f && currentTime < 0.2f)
                {
                    Debug.Log($"TimeSpawn: Final Boss alive - HP: {damageReceiver?.CurrentHp ?? -1}");
                }
            }
            else
            {
                // Final Boss không còn tồn tại - có thể đã bị destroy
                Debug.Log("🎉 TimeSpawn: Final Boss destroyed! Triggering win condition...");
                OnFinalBossDeath();
            }
        }
    }
    
    /// <summary>
    /// Xử lý khi Final Boss chết
    /// </summary>
    void OnFinalBossDeath()
    {
        Debug.Log("🎉 === FINAL BOSS DEFEATED! PLAYER WINS! === 🎉");
        Debug.Log($"TimeSpawn: Final Boss death detected at time {currentTime:F1}s");
        Debug.Log($"GameResultManager.Instance != null: {GameResultManager.Instance != null}");
        Debug.Log($"EnemyWaveManager.Instance != null: {EnemyWaveManager.Instance != null}");
        
        // Gọi EnemyWaveManager để hoàn thành Wave 5
        if (EnemyWaveManager.Instance != null)
        {
            Debug.Log("📢 Notifying EnemyWaveManager: Wave 5 completed!");
            // Gọi CompleteAllWaves() để hoàn thành tất cả waves
            EnemyWaveManager.Instance.CompleteAllWaves();
            Debug.Log("✅ EnemyWaveManager.CompleteAllWaves() called successfully!");
        }
        else
        {
            Debug.LogError("❌ EnemyWaveManager.Instance is null! Cannot notify wave completion!");
        }
        
        // Gọi GameResultManager để thông báo thắng
        // Ưu tiên sử dụng Map1GameResultManager nếu đang ở Map 1
        if (IsMap1() && Map1GameResultManager.Instance != null)
        {
            Debug.Log("📢 Calling Map1GameResultManager.OnAllWavesCompleted()...");
            Map1GameResultManager.Instance.OnAllWavesCompleted();
            Debug.Log("✅ Map1GameResultManager.OnAllWavesCompleted() called successfully!");
        }
        else if (GameResultManager.Instance != null)
        {
            Debug.Log("📢 Calling GameResultManager.OnAllWavesCompleted()...");
            // Sử dụng OnAllWavesCompleted() thay vì Win()
            GameResultManager.Instance.OnAllWavesCompleted();
            Debug.Log("✅ GameResultManager.OnAllWavesCompleted() called successfully!");
        }
        else
        {
            Debug.LogError("❌ No GameResultManager found! Cannot trigger win condition!");
        }
        
        // Reset reference
        finalBoss = null;
        Debug.Log("🎉 === FINAL BOSS DEATH PROCESSING COMPLETED === 🎉");
    }
    
    /// <summary>
    /// Kiểm tra xem có phải Map 1 không
    /// </summary>
    protected virtual bool IsMap1()
    {
        try
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            return currentSceneName == "Hai_Map";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong TimeSpawn.IsMap1: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Spawn một loại quái cụ thể
    /// </summary>
    void SpawnEnemyType(int typeIndex, EnemyCtrl enemyPrefab)
    {
        if (useEnemySpawning)
        {
            // Sử dụng EnemySpawning để spawn
            SpawnUsingEnemySpawning(typeIndex, enemyPrefab);
        }
        else
        {
            // Spawn trực tiếp như cũ
            SpawnDirectly(enemyPrefab);
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"TimeSpawn: Spawn {enemyPrefab.name} tại thời điểm {currentTime:F1}s");
        }
    }
    
    /// <summary>
    /// Spawn sử dụng EnemySpawning (tương thích với EnemyWaveSetup)
    /// </summary>
    void SpawnUsingEnemySpawning(int typeIndex, EnemyCtrl enemyPrefab)
    {
        EnemySpawning[] enemySpawningRefs = { enemySpawning1, enemySpawning2, enemySpawning3, enemySpawning4, enemySpawning5 };
        
        if (typeIndex >= 0 && typeIndex < enemySpawningRefs.Length && enemySpawningRefs[typeIndex] != null)
        {
            // EnemySpawning kế thừa từ EnemyManagerAbstract và có enemyManagerCtrl property
            var enemySpawning = enemySpawningRefs[typeIndex];
            
            // Kiểm tra enemyManagerCtrl từ EnemySpawning
            if (enemySpawning.EnemyManagerCtrl != null && enemySpawning.EnemyManagerCtrl.EnemySpawner != null)
            {
                // Spawn 1 con tại vị trí của EnemySpawning
                EnemyCtrl newEnemy = enemySpawning.EnemyManagerCtrl.EnemySpawner.Spawn(enemyPrefab, enemySpawning.transform.position);
                if (newEnemy != null)
                {
                    newEnemy.gameObject.SetActive(true);
                    
                    // Khởi tạo Behavior Tree
                    if (newEnemy.EnemyBTree != null)
                    {
                        newEnemy.EnemyBTree.BuildBehaviorTree();
                        newEnemy.EnemyBTree.ResetBTree();
                        newEnemy.EnemyBTree.StartBTree();
                    }
                    
                    // Khởi tạo EnemyScream
                    if (newEnemy.EnemyScream != null)
                    {
                        newEnemy.EnemyScream.SetCanScream(true);
                    }
                }
                
                if (showDebugLogs)
                {
                    Debug.Log($"TimeSpawn: Spawned {enemyPrefab.name} sử dụng EnemySpawning {typeIndex + 1} (MaxSpawn: {enemySpawning.MaxSpawn}, SpawnSpeed: {enemySpawning.SpawnSpeed})");
                }
            }
            else
            {
                Debug.LogError($"TimeSpawn: EnemySpawning {typeIndex + 1} không có enemyManagerCtrl hoặc EnemySpawner!");
            }
        }
        else 
        {
            Debug.LogError($"TimeSpawn: EnemySpawning {typeIndex + 1} không được cấu hình!");
        }
    }
    
    /// <summary>
    /// Spawn trực tiếp (phương thức cũ)
    /// </summary>
    void SpawnDirectly(EnemyCtrl enemyPrefab)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("TimeSpawn: Không có spawn points được cấu hình!");
            return;
        }
        
        // Chọn spawn point ngẫu nhiên
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        // Spawn quái
        GameObject spawnedEnemy = Instantiate(enemyPrefab.gameObject, spawnPoint.position, spawnPoint.rotation);
        
        if (showDebugLogs)
        {
            Debug.Log($"TimeSpawn: Spawned {enemyPrefab.name} tại {spawnPoint.name}");
        }
    }
    

    /// <summary>
    /// Reset thời gian và trạng thái spawn
    /// </summary>
    public void ResetTime()
    {
        currentTime = 0f;
        spawnedTypes = new List<bool> { false, false, false, false, false };
        hasStarted = false;
        
        // Reset Wave 5
        wave5Started = false;
        wave5SpawnedTypes = new List<bool> { false, false, false, false, false };
        finalBoss = null;
        
        if (showDebugLogs)
        {
            Debug.Log("TimeSpawn: Reset thời gian và trạng thái spawn (bao gồm Wave 5)!");
        }
    }
    
    /// <summary>
    /// Lấy thời gian hiện tại
    /// </summary>
    public float GetCurrentTime()
    {
        return currentTime;
    }
    
    /// <summary>
    /// Kiểm tra xem đã spawn hết quái chưa
    /// </summary>
    public bool IsAllEnemiesSpawned()
    {
        return spawnedTypes.TrueForAll(x => x);
    }
    
    /// <summary>
    /// Lấy số loại quái đã spawn
    /// </summary>
    public int GetSpawnedTypesCount()
    {
        int count = 0;
        foreach (bool spawned in spawnedTypes)
        {
            if (spawned) count++;
        }
        return count;
    }
    
    /// <summary>
    /// Test method để spawn tất cả quái ngay lập tức
    /// </summary>
    [ContextMenu("Test Spawn All Enemies")]
    public void TestSpawnAllEnemies()
    {
        if (showDebugLogs)
        {
            Debug.Log("TimeSpawn: Test spawn tất cả quái!");
        }
        
        // Spawn tất cả loại quái ngay lập tức
        if (enemyType1 != null) SpawnEnemyType(0, enemyType1);
        if (enemyType2 != null) SpawnEnemyType(1, enemyType2);
        if (enemyType3 != null) SpawnEnemyType(2, enemyType3);
        if (enemyType4 != null) SpawnEnemyType(3, enemyType4);
        if (enemyType5 != null) SpawnEnemyType(4, enemyType5);
    }
    
    /// <summary>
    /// Test method để kiểm tra cấu hình EnemySpawning
    /// </summary>
    [ContextMenu("Test EnemySpawning Configuration")]
    public void TestEnemySpawningConfiguration()
    {
        Debug.Log("=== TESTING ENEMYSPAWNING CONFIGURATION ===");
        
        EnemySpawning[] enemySpawningRefs = { enemySpawning1, enemySpawning2, enemySpawning3, enemySpawning4, enemySpawning5 };
        EnemyCtrl[] enemyPrefabs = { enemyType1, enemyType2, enemyType3, enemyType4, enemyType5 };
        
        for (int i = 0; i < 5; i++)
        {
            Debug.Log($"Type {i + 1}: EnemyPrefab={enemyPrefabs[i] != null}, EnemySpawning={enemySpawningRefs[i] != null}");
            
            if (enemySpawningRefs[i] != null)
            {
                Debug.Log($"  - MaxSpawn: {enemySpawningRefs[i].MaxSpawn}");
                Debug.Log($"  - SpawnSpeed: {enemySpawningRefs[i].SpawnSpeed}");
            }
        }
        
        Debug.Log($"Use EnemySpawning: {useEnemySpawning}");
        Debug.Log("=== END TEST ===");
    }
    
    /// <summary>
    /// Test method để kiểm tra cấu hình Wave 5
    /// </summary>
    [ContextMenu("Test Wave 5 Configuration")]
    public void TestWave5Configuration()
    {
        Debug.Log("=== TESTING WAVE 5 CONFIGURATION ===");
        Debug.Log($"Use Wave 5 Sequential Spawn: {useWave5SequentialSpawn}");
        Debug.Log($"Wave 5 Start Time: {wave5StartTime}s");
        Debug.Log($"Wave 5 Boss Is Final Boss: {wave5BossIsFinalBoss}");
        
        Debug.Log("Wave 5 Spawn Times:");
        for (int i = 0; i < wave5SpawnTimes.Length; i++)
        {
            Debug.Log($"  - Type {i + 1}: {wave5SpawnTimes[i]}s");
        }
        
        Debug.Log("Wave 5 Spawn Counts:");
        for (int i = 0; i < wave5SpawnCounts.Length; i++)
        {
            Debug.Log($"  - Type {i + 1}: {wave5SpawnCounts[i]} enemies");
        }
        
        Debug.Log("=== END TEST ===");
    }
    
    /// <summary>
    /// Test method để spawn Wave 5 ngay lập tức
    /// </summary>
    [ContextMenu("Test Spawn Wave 5")]
    public void TestSpawnWave5()
    {
        if (showDebugLogs)
        {
            Debug.Log("🎯 TimeSpawn: Test spawn Wave 5!");
        }
        
        // Bắt đầu Wave 5
        wave5Started = true;
        
        // Spawn tất cả loại quái trong Wave 5 ngay lập tức
        for (int i = 0; i < wave5SpawnTimes.Length; i++)
        {
            SpawnWave5EnemyType(i);
            wave5SpawnedTypes[i] = true;
        }
    }
}
