using UnityEngine;

/// <summary>
/// Script theo dõi input WASD để hoàn thành movement tutorial
/// </summary>
public class MovementTutorialTracker : MonoBehaviour
{
    [Header("Movement Tutorial")]
    [SerializeField] protected bool wasdPressed = false;
    [SerializeField] protected bool wPressed = false;
    [SerializeField] protected bool aPressed = false;
    [SerializeField] protected bool sPressed = false;
    [SerializeField] protected bool dPressed = false;
    
    [Header("Debug")]
    [SerializeField] protected bool showDebugLogs = true;
    
    protected virtual void Update()
    {
        this.CheckWASDInput();
    }
    
    protected virtual void CheckWASDInput()
    {
        // Kiểm tra từng phím WASD
        if (Input.GetKeyDown(KeyCode.W))
        {
            wPressed = true;
            if (showDebugLogs) Debug.Log("🎮 Phím W đã được nhấn!");
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            aPressed = true;
            if (showDebugLogs) Debug.Log("🎮 Phím A đã được nhấn!");
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            sPressed = true;
            if (showDebugLogs) Debug.Log("🎮 Phím S đã được nhấn!");
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            dPressed = true;
            if (showDebugLogs) Debug.Log("🎮 Phím D đã được nhấn!");
        }
        
        // Kiểm tra xem đã nhấn đủ WASD chưa
        if (wPressed && aPressed && sPressed && dPressed && !wasdPressed)
        {
            wasdPressed = true;
            this.CompleteMovementTutorial();
        }
    }
    
    protected virtual void CompleteMovementTutorial()
    {
        Debug.Log("🎮 === MOVEMENT TUTORIAL COMPLETION ===");
        Debug.Log("🎮 Đã nhấn đủ WASD! Movement Tutorial hoàn thành!");
        Debug.Log($"🎮 W: {wPressed}, A: {aPressed}, S: {sPressed}, D: {dPressed}");
        
        // Thông báo cho TowerQuestSystem
        if (TowerQuestSystem.Instance != null)
        {
            Debug.Log("🎮 Tìm thấy TowerQuestSystem.Instance, gọi CompleteMovementTutorial...");
            TowerQuestSystem.Instance.CompleteMovementTutorial();
            Debug.Log("🎮 Đã gọi CompleteMovementTutorial!");
        }
        else
        {
            Debug.LogError("❌ TowerQuestSystem.Instance KHÔNG TÌM THẤY!");
            Debug.LogError("❌ Hãy kiểm tra xem TowerQuestSystem có được khởi tạo trong scene không!");
        }
        
        Debug.Log("🎮 === END MOVEMENT TUTORIAL COMPLETION ===");
    }
    
    /// <summary>
    /// Reset tutorial để test lại
    /// </summary>
    [ContextMenu("Reset Movement Tutorial")]
    public virtual void ResetMovementTutorial()
    {
        wasdPressed = false;
        wPressed = false;
        aPressed = false;
        sPressed = false;
        dPressed = false;
        
        Debug.Log("🔄 Movement Tutorial đã được reset!");
    }
    
    /// <summary>
    /// Kiểm tra trạng thái tutorial
    /// </summary>
    [ContextMenu("Check Tutorial Status")]
    public virtual void CheckTutorialStatus()
    {
        Debug.Log("=== MOVEMENT TUTORIAL STATUS ===");
        Debug.Log($"W pressed: {wPressed}");
        Debug.Log($"A pressed: {aPressed}");
        Debug.Log($"S pressed: {sPressed}");
        Debug.Log($"D pressed: {dPressed}");
        Debug.Log($"All WASD pressed: {wasdPressed}");
        Debug.Log("================================");
    }
}
