using UnityEngine;

/// <summary>
/// Tự động tạo SoundSpawnerCtrl nếu không có trong scene
/// Gắn script này vào SoundManager
/// </summary>
public class SoundSpawnerAutoSetup : SaiMonoBehaviour
{
    [Header("=== AUTO SETUP SOUND SPAWNER ===")]
    [SerializeField] protected bool autoCreateIfMissing = true;
    [SerializeField] protected GameObject soundSpawnerPrefab;
    
    protected override void Start()
    {
        base.Start();
        this.EnsureSoundSpawnerExists();
    }

    /// <summary>
    /// Đảm bảo SoundSpawnerCtrl tồn tại trong scene
    /// </summary>
    protected virtual void EnsureSoundSpawnerExists()
    {
        // Kiểm tra xem đã có SoundSpawnerCtrl chưa
        SoundSpawnerCtrl existingCtrl = FindAnyObjectByType<SoundSpawnerCtrl>();
        if (existingCtrl != null)
        {
            Debug.Log("SoundSpawnerAutoSetup: SoundSpawnerCtrl đã tồn tại trong scene");
            return;
        }

        if (!autoCreateIfMissing)
        {
            Debug.LogWarning("SoundSpawnerAutoSetup: SoundSpawnerCtrl không tồn tại và autoCreateIfMissing = false");
            return;
        }

        // Tạo SoundSpawnerCtrl mới
        this.CreateSoundSpawnerCtrl();
    }

    /// <summary>
    /// Tạo SoundSpawnerCtrl mới
    /// </summary>
    protected virtual void CreateSoundSpawnerCtrl()
    {
        GameObject soundSpawnerObj;
        
        if (soundSpawnerPrefab != null)
        {
            // Sử dụng prefab nếu có
            soundSpawnerObj = Instantiate(soundSpawnerPrefab);
            Debug.Log("SoundSpawnerAutoSetup: Tạo SoundSpawnerCtrl từ prefab");
        }
        else
        {
            // Tạo mới từ code
            soundSpawnerObj = this.CreateSoundSpawnerFromCode();
            Debug.Log("SoundSpawnerAutoSetup: Tạo SoundSpawnerCtrl từ code");
        }

        soundSpawnerObj.name = "SoundSpawnerCtrl";
        DontDestroyOnLoad(soundSpawnerObj);
        
        Debug.Log("SoundSpawnerAutoSetup: SoundSpawnerCtrl đã được tạo thành công");
    }

    /// <summary>
    /// Tạo SoundSpawnerCtrl từ code
    /// </summary>
    protected virtual GameObject CreateSoundSpawnerFromCode()
    {
        // Tạo GameObject chính
        GameObject soundSpawnerObj = new GameObject("SoundSpawnerCtrl");
        
        // Thêm SoundSpawnerCtrl component
        SoundSpawnerCtrl ctrl = soundSpawnerObj.AddComponent<SoundSpawnerCtrl>();
        
        // Thêm SoundSpawner component
        SoundSpawner spawner = soundSpawnerObj.AddComponent<SoundSpawner>();
        
        // Tạo SoundPrefabs GameObject
        GameObject soundPrefabsObj = new GameObject("SoundPrefabs");
        soundPrefabsObj.transform.SetParent(soundSpawnerObj.transform);
        
        // Thêm SoundPrefabs component
        SoundPrefabs soundPrefabs = soundPrefabsObj.AddComponent<SoundPrefabs>();
        
        // Tạo PoolHolder
        GameObject poolHolderObj = new GameObject("PoolHolder");
        poolHolderObj.transform.SetParent(soundSpawnerObj.transform);
        
        // Cấu hình SoundSpawner (poolHolder sẽ được set tự động trong LoadPoolHolder)
        // spawner.poolHolder = poolHolderObj.transform;
        
        Debug.Log("SoundSpawnerAutoSetup: SoundSpawnerCtrl structure đã được tạo");
        
        return soundSpawnerObj;
    }

    /// <summary>
    /// Tạo SFX prefab mẫu để test
    /// </summary>
    [ContextMenu("Create Sample SFX Prefabs")]
    public virtual void CreateSampleSfxPrefabs()
    {
        SoundSpawnerCtrl ctrl = FindAnyObjectByType<SoundSpawnerCtrl>();
        if (ctrl == null)
        {
            Debug.LogError("SoundSpawnerAutoSetup: Không tìm thấy SoundSpawnerCtrl để tạo SFX prefabs");
            return;
        }

        SoundPrefabs soundPrefabs = ctrl.Prefabs;
        if (soundPrefabs == null)
        {
            Debug.LogError("SoundSpawnerAutoSetup: SoundPrefabs is null");
            return;
        }

        // Tạo EvilScream prefab
        this.CreateSfxPrefab(soundPrefabs.transform, "EvilScream", "EvilScreamCtrl");
        
        
        // Tạo BerettaM9Shot prefab
        this.CreateSfxPrefab(soundPrefabs.transform, "BerettaM9Shot", "BerettaM9ShotCtrl");
        
        // Tạo MagicSpell prefab
        this.CreateSfxPrefab(soundPrefabs.transform, "MagicSpell", "MagicSpellCtrl");
        
        // Tạo Flame prefab
        this.CreateSfxPrefab(soundPrefabs.transform, "Flame", "FlameCtrl");
        
        Debug.Log("SoundSpawnerAutoSetup: Đã tạo sample SFX prefabs");
    }

    /// <summary>
    /// Tạo một SFX prefab
    /// </summary>
    protected virtual void CreateSfxPrefab(Transform parent, string prefabName, string scriptName)
    {
        // Tạo GameObject
        GameObject sfxObj = new GameObject(prefabName);
        sfxObj.transform.SetParent(parent);
        
        // Thêm AudioSource
        AudioSource audioSource = sfxObj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 1f;
        
        // Thêm script tương ứng
        switch (scriptName)
        {
            case "EvilScreamCtrl":
                sfxObj.AddComponent<EvilScreamCtrl>();
                break;
            // case "EnemyPunchCtrl": - REMOVED
            case "BerettaM9ShotCtrl":
                sfxObj.AddComponent<BerettaM9ShotCtrl>();
                break;
            case "MagicSpellCtrl":
                sfxObj.AddComponent<MagicSpellCtrl>();
                break;
            case "FlameCtrl":
                sfxObj.AddComponent<FlameCtrl>();
                break;
        }
        
        // Set inactive để sử dụng như prefab
        sfxObj.SetActive(false);
        
        Debug.Log($"SoundSpawnerAutoSetup: Đã tạo {prefabName} prefab");
    }

    /// <summary>
    /// Kiểm tra và sửa setup hiện tại
    /// </summary>
    [ContextMenu("Check and Fix Setup")]
    public virtual void CheckAndFixSetup()
    {
        Debug.Log("=== KIỂM TRA SOUND SETUP ===");
        
        // Kiểm tra SoundManager
        SoundManager soundManager = FindAnyObjectByType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogError("❌ SoundManager không tồn tại trong scene!");
            return;
        }
        Debug.Log("✅ SoundManager: OK");
        
        // Kiểm tra SoundSpawnerCtrl
        SoundSpawnerCtrl ctrl = FindAnyObjectByType<SoundSpawnerCtrl>();
        if (ctrl == null)
        {
            Debug.LogError("❌ SoundSpawnerCtrl không tồn tại trong scene!");
            this.CreateSoundSpawnerCtrl();
            return;
        }
        Debug.Log("✅ SoundSpawnerCtrl: OK");
        
        // Kiểm tra SoundPrefabs
        SoundPrefabs soundPrefabs = ctrl.Prefabs;
        if (soundPrefabs == null)
        {
            Debug.LogError("❌ SoundPrefabs is null!");
            return;
        }
        Debug.Log("✅ SoundPrefabs: OK");
        
        // Kiểm tra SFX prefabs
        var prefabs = soundPrefabs.PrefabsList;
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning("⚠️ Không có SFX prefab nào! Tạo sample prefabs...");
            this.CreateSampleSfxPrefabs();
        }
        else
        {
            Debug.Log($"✅ SFX Prefabs: {prefabs.Count} prefabs có sẵn");
        }
        
        Debug.Log("=== KIỂM TRA HOÀN TẤT ===");
    }
}
