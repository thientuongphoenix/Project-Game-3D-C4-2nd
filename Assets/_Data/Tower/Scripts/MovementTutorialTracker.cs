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
    
    [Header("Advanced Movement")]
    [SerializeField] protected bool spacePressed = false; // Space để nhảy
    [SerializeField] protected bool leftShiftPressed = false; // Left Shift để chạy nhanh
    [SerializeField] protected bool allMovementCompleted = false; // Tất cả movement đã hoàn thành
    
    [Header("Debug")]
    [SerializeField] protected bool showDebugLogs = true;
    
    protected virtual void Update()
    {
        this.CheckWASDInput();
        this.CheckAdvancedMovementInput();
    }
    
    protected virtual void CheckWASDInput()
    {
        // Kiểm tra từng phím WASD
        if (Input.GetKeyDown(KeyCode.W))
        {
            wPressed = true;
            if (showDebugLogs) Debug.Log("Phím W đã được nhấn!");
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            aPressed = true;
            if (showDebugLogs) Debug.Log("Phím A đã được nhấn!");
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            sPressed = true;
            if (showDebugLogs) Debug.Log("Phím S đã được nhấn!");
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            dPressed = true;
            if (showDebugLogs) Debug.Log("Phím D đã được nhấn!");
        }
        
        // Kiểm tra xem đã nhấn đủ WASD chưa
        if (wPressed && aPressed && sPressed && dPressed && !wasdPressed)
        {
            wasdPressed = true;
            Debug.Log(" Đã nhấn đủ WASD! Tiếp tục với Space và Left Shift...");
        }
        
        // Kiểm tra xem đã hoàn thành tất cả movement chưa (WASD + Space + Left Shift)
        if (wasdPressed && spacePressed && leftShiftPressed && !allMovementCompleted)
        {
            allMovementCompleted = true;
            this.CompleteMovementTutorial();
        }
    }
    
    protected virtual void CheckAdvancedMovementInput()
    {
        // Kiểm tra phím Space để nhảy
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressed = true;
            if (showDebugLogs) Debug.Log("Phím Space đã được nhấn! (Nhảy)");
        }
        
        // Kiểm tra phím Left Shift để chạy nhanh
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            leftShiftPressed = true;
            if (showDebugLogs) Debug.Log("Phím Left Shift đã được nhấn! (Chạy nhanh)");
        }
    }
    
    protected virtual void CompleteMovementTutorial()
    {
        Debug.Log("=== MOVEMENT TUTORIAL COMPLETION ===");
        Debug.Log(" HOÀN THÀNH! Đã nhấn đủ tất cả phím movement!");
        Debug.Log($"WASD: W={wPressed}, A={aPressed}, S={sPressed}, D={dPressed}");
        Debug.Log($"Advanced: Space (Nhảy)={spacePressed}, Left Shift (Chạy nhanh)={leftShiftPressed}");
        Debug.Log(" Movement Tutorial đã hoàn thành đầy đủ!");
        
        // Thông báo cho TowerQuestSystem
        if (TowerQuestSystem.Instance != null)
        {
            Debug.Log("Tìm thấy TowerQuestSystem.Instance, gọi CompleteMovementTutorial...");
            TowerQuestSystem.Instance.CompleteMovementTutorial();
            Debug.Log("Đã gọi CompleteMovementTutorial!");
        }
        else
        {
            Debug.LogError(" TowerQuestSystem.Instance KHÔNG TÌM THẤY!");
            Debug.LogError(" Hãy kiểm tra xem TowerQuestSystem có được khởi tạo trong scene không!");
        }
        
        Debug.Log(" === END MOVEMENT TUTORIAL COMPLETION ===");
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
        spacePressed = false;
        leftShiftPressed = false;
        allMovementCompleted = false;
        
        Debug.Log(" Movement Tutorial đã được reset!");
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
        Debug.Log($"Space pressed (Nhảy): {spacePressed}");
        Debug.Log($"Left Shift pressed (Chạy nhanh): {leftShiftPressed}");
        Debug.Log($"All Movement Completed: {allMovementCompleted}");
        Debug.Log("================================");
    }
}
