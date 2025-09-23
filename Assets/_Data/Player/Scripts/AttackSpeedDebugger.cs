using UnityEngine;

/// <summary>
/// Script để debug và hiển thị attack speed gốc của player
/// </summary>
public class AttackSpeedDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] protected bool showDebugInfo = true;
    [SerializeField] protected float updateInterval = 1f;
    
    protected PlayerCtrl playerCtrl;
    protected AttackLight attackLight;
    protected AttackHeavy attackHeavy;
    protected InputManager inputManager;
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
            this.ShowAttackSpeedInfo();
            lastUpdateTime = Time.time;
        }
    }
    
    protected virtual void LoadPlayerReferences()
    {
        if (playerCtrl != null) return;
        playerCtrl = PlayerCtrl.Instance;
        
        if (playerCtrl != null)
        {
            attackLight = playerCtrl.GetComponentInChildren<AttackLight>();
            attackHeavy = playerCtrl.GetComponentInChildren<AttackHeavy>();
        }
        
        inputManager = InputManager.Instance;
        
        Debug.Log("AttackSpeedDebugger: Loaded player references", gameObject);
    }
    
    protected virtual void ShowAttackSpeedInfo()
    {
        Debug.Log("=== ORIGINAL ATTACK SYSTEM INFO ===");
        Debug.Log("Updated: Heavy Attack cooldown reduced from 3.0s to 1.5s");
        
        // Input Manager Info
        if (inputManager != null)
        {
            Debug.Log($"InputManager - Attack Light Limit: {inputManager.GetType().GetField("attackLightLimit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(inputManager)}");
        }
        
        // Attack Light Info
        if (attackLight != null)
        {
            var cooldownField = attackLight.GetType().GetField("cooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cooldownField != null)
                Debug.Log($"AttackLight - Cooldown: {cooldownField.GetValue(attackLight)}s");
            Debug.Log($"AttackLight - Effect Name: {attackLight.GetType().GetField("effectName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(attackLight)}");
        }
        else
        {
            Debug.LogWarning("❌ AttackLight component not found!");
        }
        
        // Attack Heavy Info
        if (attackHeavy != null)
        {
            var cooldownField = attackHeavy.GetType().GetField("cooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var manaCostField = attackHeavy.GetType().GetField("manaCost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (cooldownField != null)
                Debug.Log($"AttackHeavy - Cooldown: {cooldownField.GetValue(attackHeavy)}s");
            if (manaCostField != null)
                Debug.Log($"AttackHeavy - Mana Cost: {manaCostField.GetValue(attackHeavy)}");
            Debug.Log($"AttackHeavy - Effect Name: {attackHeavy.GetType().GetField("effectName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(attackHeavy)}");
        }
        else
        {
            Debug.LogWarning("❌ AttackHeavy component not found!");
        }
        
        // Weapons Info
        if (playerCtrl != null && playerCtrl.Weapons != null)
        {
            Debug.Log($"Weapons - Current Weapon: {playerCtrl.Weapons.GetCurrentWeapon()?.name}");
            Debug.Log($"Weapons - Total Weapons: {playerCtrl.Weapons.GetType().GetField("weapons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(playerCtrl.Weapons)}");
        }
        
        Debug.Log("=====================================");
    }
    
    [ContextMenu("Show Attack System Info")]
    protected virtual void ShowAttackSystemInfo()
    {
        this.ShowAttackSpeedInfo();
    }
    
    protected virtual void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 220, 300, 150));
        GUILayout.Label("=== ATTACK SYSTEM DEBUG ===");
        
        if (inputManager != null)
        {
            var attackLightLimitField = inputManager.GetType().GetField("attackLightLimit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (attackLightLimitField != null)
                GUILayout.Label($"Light Attack Limit: {attackLightLimitField.GetValue(inputManager)}s");
        }
        
        if (attackHeavy != null)
        {
            var cooldownField = attackHeavy.GetType().GetField("cooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cooldownField != null)
                GUILayout.Label($"Heavy Attack Cooldown: {cooldownField.GetValue(attackHeavy)}s");
                
            var manaCostField = attackHeavy.GetType().GetField("manaCost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (manaCostField != null)
                GUILayout.Label($"Heavy Attack Mana Cost: {manaCostField.GetValue(attackHeavy)}");
        }
        
        if (attackLight != null)
        {
            var cooldownField = attackLight.GetType().GetField("cooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cooldownField != null)
                GUILayout.Label($"Light Attack Cooldown: {cooldownField.GetValue(attackLight)}s");
        }
        
        GUILayout.Label($"Attack Light: {(attackLight != null ? "Found" : "NULL")}");
        GUILayout.Label($"Attack Heavy: {(attackHeavy != null ? "Found" : "NULL")}");
        
        GUILayout.EndArea();
    }
}
