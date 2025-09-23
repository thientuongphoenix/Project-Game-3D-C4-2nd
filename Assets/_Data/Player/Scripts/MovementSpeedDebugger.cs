using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// Script để debug và hiển thị movement speed gốc của player
/// </summary>
public class MovementSpeedDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] protected bool showDebugInfo = true;
    [SerializeField] protected float updateInterval = 1f;
    
    protected PlayerCtrl playerCtrl;
    protected vThirdPersonMotor motor;
    protected float lastUpdateTime;
    
    protected virtual void Start()
    {
        this.LoadPlayerReferences();
    }
    
    protected virtual void Update()
    {
        if (!showDebugInfo) return;
        
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            this.ShowMovementSpeedInfo();
            lastUpdateTime = Time.time;
        }
    }
    
    protected virtual void LoadPlayerReferences()
    {
        if (playerCtrl != null) return;
        playerCtrl = PlayerCtrl.Instance;
        
        if (playerCtrl != null && playerCtrl.ThirdPersonController != null)
        {
            motor = playerCtrl.ThirdPersonController.GetComponent<vThirdPersonMotor>();
        }
        
        Debug.Log("MovementSpeedDebugger: Loaded player references", gameObject);
    }
    
    protected virtual void ShowMovementSpeedInfo()
    {
        if (motor == null)
        {
            Debug.LogWarning("❌ vThirdPersonMotor not found!");
            return;
        }
        
        Debug.Log("=== ORIGINAL MOVEMENT SPEED STATS ===");
        Debug.Log($"Free Speed - Walk: {motor.freeSpeed.walkSpeed}");
        Debug.Log($"Free Speed - Running: {motor.freeSpeed.runningSpeed}");
        Debug.Log($"Free Speed - Sprint: {motor.freeSpeed.sprintSpeed}");
        Debug.Log($"Strafe Speed - Walk: {motor.strafeSpeed.walkSpeed}");
        Debug.Log($"Strafe Speed - Running: {motor.strafeSpeed.runningSpeed}");
        Debug.Log($"Strafe Speed - Sprint: {motor.strafeSpeed.sprintSpeed}");
        Debug.Log($"Air Speed: {motor.airSpeed}");
        Debug.Log($"Current Move Speed: {motor.moveSpeed}");
        Debug.Log("=====================================");
    }
    
    [ContextMenu("Show Current Movement Speed")]
    protected virtual void ShowCurrentMovementSpeed()
    {
        this.ShowMovementSpeedInfo();
    }
    
    protected virtual void OnGUI()
    {
        if (!showDebugInfo || motor == null) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== MOVEMENT SPEED DEBUG ===");
        GUILayout.Label($"Free Walk: {motor.freeSpeed.walkSpeed}");
        GUILayout.Label($"Free Running: {motor.freeSpeed.runningSpeed}");
        GUILayout.Label($"Free Sprint: {motor.freeSpeed.sprintSpeed}");
        GUILayout.Label($"Strafe Walk: {motor.strafeSpeed.walkSpeed}");
        GUILayout.Label($"Strafe Running: {motor.strafeSpeed.runningSpeed}");
        GUILayout.Label($"Strafe Sprint: {motor.strafeSpeed.sprintSpeed}");
        GUILayout.Label($"Air Speed: {motor.airSpeed}");
        GUILayout.Label($"Current Move Speed: {motor.moveSpeed:F2}");
        GUILayout.EndArea();
    }
}
