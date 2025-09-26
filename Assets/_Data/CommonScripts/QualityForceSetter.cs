using UnityEngine;

/// <summary>
/// Script để force set quality ngay từ đầu, trước khi các script khác chạy
/// </summary>
public class QualityForceSetter : MonoBehaviour
{
    [Header("Quality Force Settings")]
    [SerializeField] protected bool forceQualityOnAwake = true;
    [SerializeField] protected int forceQualityLevel = 3; // High
    [SerializeField] protected bool debugQualityForce = true;
    
    protected virtual void Awake()
    {
        if (forceQualityOnAwake)
        {
            this.ForceSetQuality();
        }
    }
    
    /// <summary>
    /// Force set quality level
    /// </summary>
    protected virtual void ForceSetQuality()
    {
        if (debugQualityForce)
        {
            Debug.Log("=== QUALITY FORCE SETTER ===");
            Debug.Log($"Current quality before force: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        }
        
        // Kiểm tra quality level có hợp lệ không
        if (forceQualityLevel < 0 || forceQualityLevel >= QualitySettings.names.Length)
        {
            forceQualityLevel = 3; // Reset về High nếu không hợp lệ
            Debug.LogWarning($"Invalid forceQualityLevel, reset to High (3)");
        }
        
        // Force set quality
        QualitySettings.SetQualityLevel(forceQualityLevel);
        
        if (debugQualityForce)
        {
            Debug.Log($"Quality force set to: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
            Debug.Log("=== QUALITY FORCE SETTER COMPLETE ===");
        }
    }
    
    /// <summary>
    /// Force set quality từ PlayerPrefs
    /// </summary>
    protected virtual void ForceSetQualityFromPlayerPrefs()
    {
        int savedQuality = PlayerPrefs.GetInt("QualityLevel", forceQualityLevel);
        
        // Kiểm tra quality level có hợp lệ không
        if (savedQuality < 0 || savedQuality >= QualitySettings.names.Length)
        {
            savedQuality = forceQualityLevel;
            Debug.LogWarning($"Invalid quality from PlayerPrefs, using forceQualityLevel: {forceQualityLevel}");
        }
        
        // Force set quality
        QualitySettings.SetQualityLevel(savedQuality);
        
        if (debugQualityForce)
        {
            Debug.Log($"Quality force set from PlayerPrefs: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");
        }
    }
    
    /// <summary>
    /// Context menu để force set quality
    /// </summary>
    [ContextMenu("Force Set Quality")]
    protected virtual void ForceSetQualityContext()
    {
        this.ForceSetQuality();
    }
    
    /// <summary>
    /// Context menu để force set quality từ PlayerPrefs
    /// </summary>
    [ContextMenu("Force Set Quality From PlayerPrefs")]
    protected virtual void ForceSetQualityFromPlayerPrefsContext()
    {
        this.ForceSetQualityFromPlayerPrefs();
    }
    
    /// <summary>
    /// Context menu để show current quality
    /// </summary>
    [ContextMenu("Show Current Quality")]
    protected virtual void ShowCurrentQualityContext()
    {
        Debug.Log("=== CURRENT QUALITY INFO ===");
        Debug.Log($"Current Quality Level: {QualitySettings.GetQualityLevel()}");
        Debug.Log($"Current Quality Name: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
        Debug.Log($"Available Quality Levels: {string.Join(", ", QualitySettings.names)}");
        Debug.Log($"PlayerPrefs QualityLevel: {PlayerPrefs.GetInt("QualityLevel", -1)}");
        Debug.Log("================================");
    }
}
