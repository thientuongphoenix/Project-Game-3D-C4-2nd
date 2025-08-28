using UnityEngine;

/// <summary>
/// Script test đơn giản để kiểm tra Movement Tutorial
/// </summary>
public class MovementTutorialTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] protected KeyCode testKey = KeyCode.T;
    
    protected virtual void Update()
    {
        // Nhấn T để test movement tutorial
        if (Input.GetKeyDown(testKey))
        {
            this.TestMovementTutorial();
        }
        
        // Nhấn R để reset movement tutorial
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.ResetMovementTutorial();
        }
    }
    
    [ContextMenu("Test Movement Tutorial")]
    public virtual void TestMovementTutorial()
    {
        Debug.Log("🧪 === TEST MOVEMENT TUTORIAL MANUALLY ===");
        
        if (TowerQuestSystem.Instance != null)
        {
            Debug.Log("🧪 TowerQuestSystem.Instance tìm thấy!");
            
            // Gọi method test
            TowerQuestSystem.Instance.TestMovementTutorial();
            
            // Gọi CompleteMovementTutorial trực tiếp
            Debug.Log("🧪 Gọi CompleteMovementTutorial trực tiếp...");
            TowerQuestSystem.Instance.CompleteMovementTutorial();
            
            // Kiểm tra trạng thái sau khi gọi
            int status = TowerQuestSystem.Instance.GetMovementTutorialStatus();
            Debug.Log($"🧪 Trạng thái sau khi gọi: {status}");
        }
        else
        {
            Debug.LogError("❌ TowerQuestSystem.Instance KHÔNG TÌM THẤY!");
        }
        
        Debug.Log("🧪 === END TEST ===");
    }
    
    [ContextMenu("Reset Movement Tutorial")]
    public virtual void ResetMovementTutorial()
    {
        Debug.Log("🔄 === RESET MOVEMENT TUTORIAL ===");
        
        if (TowerQuestSystem.Instance != null)
        {
            // Reset quest system
            TowerQuestSystem.Instance.ResetQuests();
            Debug.Log("🔄 Đã reset TowerQuestSystem!");
        }
        else
        {
            Debug.LogError("❌ TowerQuestSystem.Instance KHÔNG TÌM THẤY!");
        }
        
        Debug.Log("🔄 === END RESET ===");
    }
}

