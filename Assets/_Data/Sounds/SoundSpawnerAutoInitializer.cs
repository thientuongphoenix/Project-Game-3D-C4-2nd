using UnityEngine;

/// <summary>
/// Script tự động đảm bảo SoundSpawnerCtrl tồn tại trong mọi scene
/// Chỉ cần thêm vào scene đầu tiên (ví dụ: Hai_Menu)
/// </summary>
public class SoundSpawnerAutoInitializer : MonoBehaviour
{
    [Header("Auto Setup Settings")]
    [SerializeField] protected bool autoSetupOnAwake = true;
    [SerializeField] protected bool debugAutoSetup = true;
    
    protected virtual void Awake()
    {
        if (autoSetupOnAwake)
        {
            this.EnsureSoundSpawnerExists();
        }
    }
    
    /// <summary>
    /// Đảm bảo SoundSpawnerCtrl tồn tại trong scene
    /// </summary>
    protected virtual void EnsureSoundSpawnerExists()
    {
        if (debugAutoSetup)
        {
            Debug.Log("=== SOUND SPAWNER AUTO INITIALIZER ===");
        }
        
        // Kiểm tra xem đã có SoundSpawnerCtrl chưa
        SoundSpawnerCtrl existingCtrl = FindAnyObjectByType<SoundSpawnerCtrl>();
        if (existingCtrl != null)
        {
            if (debugAutoSetup)
            {
                Debug.Log("✅ SoundSpawnerCtrl đã tồn tại trong scene");
            }
            return;
        }
        
        if (debugAutoSetup)
        {
            Debug.Log("⚠️ SoundSpawnerCtrl không tồn tại! Đang tạo mới...");
        }
        
        // Tạo SoundSpawnerCtrl mới
        this.CreateSoundSpawnerCtrl();
        
        if (debugAutoSetup)
        {
            Debug.Log("=== AUTO INITIALIZER HOÀN TẤT ===");
        }
    }
    
    /// <summary>
    /// Tạo SoundSpawnerCtrl mới với cấu trúc đầy đủ
    /// </summary>
    protected virtual void CreateSoundSpawnerCtrl()
    {
        try
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
            
            // Đảm bảo không bị destroy khi load scene mới (chỉ trong Play mode)
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(soundSpawnerObj);
            }
            
            if (debugAutoSetup)
            {
                Debug.Log("✅ SoundSpawnerCtrl đã được tạo tự động với cấu trúc đầy đủ");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Lỗi khi tạo SoundSpawnerCtrl: {e.Message}");
        }
    }
    
    /// <summary>
    /// Kiểm tra và sửa lỗi SoundSpawnerCtrl
    /// </summary>
    [ContextMenu("Check and Fix SoundSpawnerCtrl")]
    public virtual void CheckAndFixSoundSpawnerCtrl()
    {
        Debug.Log("=== KIỂM TRA SOUND SPAWNER CTRL ===");
        
        // Kiểm tra SoundSpawnerCtrl
        SoundSpawnerCtrl ctrl = FindAnyObjectByType<SoundSpawnerCtrl>();
        if (ctrl == null)
        {
            Debug.LogWarning("❌ SoundSpawnerCtrl không tồn tại! Đang tạo mới...");
            this.CreateSoundSpawnerCtrl();
            return;
        }
        
        Debug.Log("✅ SoundSpawnerCtrl: OK");
        
        // Kiểm tra SoundSpawner
        if (ctrl.Spawner == null)
        {
            Debug.LogWarning("⚠️ SoundSpawner is null! Đang thêm component...");
            ctrl.gameObject.AddComponent<SoundSpawner>();
        }
        else
        {
            Debug.Log("✅ SoundSpawner: OK");
        }
        
        // Kiểm tra SoundPrefabs
        if (ctrl.Prefabs == null)
        {
            Debug.LogWarning("⚠️ SoundPrefabs is null! Đang tạo...");
            GameObject soundPrefabsObj = new GameObject("SoundPrefabs");
            soundPrefabsObj.transform.SetParent(ctrl.transform);
            soundPrefabsObj.AddComponent<SoundPrefabs>();
        }
        else
        {
            Debug.Log("✅ SoundPrefabs: OK");
        }
        
        Debug.Log("=== KIỂM TRA HOÀN TẤT ===");
    }
    
    /// <summary>
    /// Force tạo lại SoundSpawnerCtrl
    /// </summary>
    [ContextMenu("Force Create SoundSpawnerCtrl")]
    public virtual void ForceCreateSoundSpawnerCtrl()
    {
        // Xóa SoundSpawnerCtrl cũ nếu có
        SoundSpawnerCtrl oldCtrl = FindAnyObjectByType<SoundSpawnerCtrl>();
        if (oldCtrl != null)
        {
            Debug.Log("🗑️ Xóa SoundSpawnerCtrl cũ...");
            DestroyImmediate(oldCtrl.gameObject);
        }
        
        // Tạo mới
        Debug.Log("🔄 Tạo SoundSpawnerCtrl mới...");
        this.CreateSoundSpawnerCtrl();
    }
}
