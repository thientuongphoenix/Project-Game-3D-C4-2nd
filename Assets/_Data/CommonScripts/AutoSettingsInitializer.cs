using UnityEngine;

/// <summary>
/// Script để tự động khởi tạo settings khi game bắt đầu
/// Chỉ cần thêm vào scene đầu tiên (Hai_Menu)
/// </summary>
public class AutoSettingsInitializer : MonoBehaviour
{
    [Header("Auto Initialization")]
    [SerializeField] protected bool initializeOnStart = true;
    [SerializeField] protected bool debugInitialization = true;
    
    protected virtual void Start()
    {
        if (initializeOnStart)
        {
            this.InitializeSettings();
        }
    }
    
    /// <summary>
    /// Khởi tạo settings cho toàn bộ game
    /// </summary>
    protected virtual void InitializeSettings()
    {
        if (debugInitialization)
        {
            Debug.Log("=== AUTO SETTINGS INITIALIZER ===");
        }
        
        // Kiểm tra xem đã có GlobalSettingsManager chưa
        if (GlobalSettingsManager.Instance == null)
        {
            // Tạo GlobalSettingsManager nếu chưa có
            GameObject globalSettingsObj = new GameObject("GlobalSettingsManager");
            globalSettingsObj.AddComponent<GlobalSettingsManager>();
            
            if (debugInitialization)
            {
                Debug.Log("✅ Created GlobalSettingsManager");
            }
        }
        
        // Kiểm tra xem đã có BuildSettingsManager chưa
        BuildSettingsManager buildManager = FindObjectOfType<BuildSettingsManager>();
        if (buildManager == null)
        {
            // Tạo BuildSettingsManager nếu chưa có
            GameObject buildSettingsObj = new GameObject("BuildSettingsManager");
            buildSettingsObj.AddComponent<BuildSettingsManager>();
            
            if (debugInitialization)
            {
                Debug.Log("✅ Created BuildSettingsManager");
            }
        }
        
        // Áp dụng settings ngay lập tức
        if (GlobalSettingsManager.Instance != null)
        {
            GlobalSettingsManager.Instance.ApplyGlobalSettings();
        }
        
        if (debugInitialization)
        {
            Debug.Log("✅ Settings initialized successfully");
            Debug.Log("=====================================");
        }
    }
    
    /// <summary>
    /// Force initialize settings
    /// </summary>
    public virtual void ForceInitialize()
    {
        Debug.Log("🔧 FORCING SETTINGS INITIALIZATION...");
        this.InitializeSettings();
    }
    
    [ContextMenu("Initialize Settings")]
    protected virtual void InitializeSettingsContext()
    {
        this.InitializeSettings();
    }
    
    [ContextMenu("Force Initialize")]
    protected virtual void ForceInitializeContext()
    {
        this.ForceInitialize();
    }
}
