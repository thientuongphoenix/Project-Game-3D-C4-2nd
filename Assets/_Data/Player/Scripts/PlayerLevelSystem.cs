using UnityEngine;

/// <summary>
/// Hệ thống level up cho player với 3 stats chính
/// </summary>
public class PlayerLevelSystem : SaiSingleton<PlayerLevelSystem>
{
    [Header("Level Settings")]
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] protected int currentExp = 0;
    [SerializeField] protected int expToNextLevel = 100;
    [SerializeField] protected float expMultiplier = 1.2f; // Tăng exp cần thiết mỗi level
    
    [Header("Base Stats")]
    [SerializeField] protected float baseMovementSpeed = 1.0f;
    [SerializeField] protected float baseAttackSpeed = 1.0f;
    [SerializeField] protected float baseSkillCooldown = 1.0f;
    
    [Header("Original Cooldowns")]
    [SerializeField] protected float originalAttackLightCooldown = 0.3f;
    [SerializeField] protected float originalAttackHeavyCooldown = 1.5f; // Giảm từ 3.0s xuống 1.5s
    [SerializeField] protected float originalAttackLightLimit = 0.1f; // Giá trị gốc của attackLightLimit
    
    [Header("Level Bonuses")]
    [SerializeField] protected float movementSpeedBonus = 0.0f; // Tắt tăng movement speed
    [SerializeField] protected float attackSpeedBonus = 0.03f; // +3% mỗi level (giảm từ 8%)
    [SerializeField] protected float skillCooldownReduction = 0.02f; // -2% mỗi level (giảm từ 3%)
    
    [Header("Current Multipliers")]
    [SerializeField] protected float currentMovementSpeedMultiplier = 1.0f;
    [SerializeField] protected float currentAttackSpeedMultiplier = 1.0f;
    [SerializeField] protected float currentSkillCooldownMultiplier = 1.0f;
    
    // References
    protected PlayerCtrl playerCtrl;
    protected PlayerDamageReceiver playerDamageReceiver;
    protected PlayerLevel playerLevel; // Reference đến hệ thống level cũ
    
    protected override void Start()
    {
        base.Start();
        this.LoadPlayerReferences();
        this.LoadOriginalCooldowns();
        this.LoadExpFromInventory();
        this.UpdatePlayerStats();
    }
    
    protected virtual void Update()
    {
        // Kiểm tra exp trong inventory mỗi frame để cập nhật real-time
        this.CheckExpUpdate();
    }
    
    /// <summary>
    /// Kiểm tra và cập nhật exp từ inventory
    /// </summary>
    protected virtual void CheckExpUpdate()
    {
        if (InventoryManager.Instance == null) return;
        
        var expItem = InventoryManager.Instance.Monies().FindItem(ItemCode.PlayerExp);
        if (expItem != null && expItem.itemCount != currentExp)
        {
            int newExp = expItem.itemCount;
            int expGained = newExp - currentExp;
            
            if (expGained > 0)
            {
                this.AddExp(expGained);
            }
            else
            {
                // Exp bị giảm (có thể do sử dụng item)
                currentExp = newExp;
                Debug.Log($"Exp updated to {currentExp}");
            }
        }
    }
    
    protected virtual void LoadPlayerReferences()
    {
        if (playerCtrl != null) return;
        playerCtrl = PlayerCtrl.Instance;
        
        if (playerDamageReceiver != null) return;
        playerDamageReceiver = FindObjectOfType<PlayerDamageReceiver>();
        
        if (playerLevel != null) return;
        playerLevel = GetComponent<PlayerLevel>();
        
        Debug.Log("PlayerLevelSystem: Loaded player references", gameObject);
    }
    
    /// <summary>
    /// Load original cooldowns từ components
    /// </summary>
    protected virtual void LoadOriginalCooldowns()
    {
        if (playerCtrl == null) return;
        
        // Load Attack Light original cooldown
        var attackLight = playerCtrl.GetComponentInChildren<AttackLight>();
        if (attackLight != null)
        {
            var cooldownField = attackLight.GetType().GetField("cooldown", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cooldownField != null)
            {
                originalAttackLightCooldown = (float)cooldownField.GetValue(attackLight);
                Debug.Log($"Loaded Attack Light original cooldown: {originalAttackLightCooldown}s");
            }
        }
        
        // Load Attack Heavy original cooldown
        var attackHeavy = playerCtrl.GetComponentInChildren<AttackHeavy>();
        if (attackHeavy != null)
        {
            var cooldownField = attackHeavy.GetType().GetField("cooldown", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cooldownField != null)
            {
                originalAttackHeavyCooldown = (float)cooldownField.GetValue(attackHeavy);
                Debug.Log($"Loaded Attack Heavy original cooldown: {originalAttackHeavyCooldown}s");
            }
        }
        
        // Load Attack Light Limit từ InputManager
        if (InputManager.Instance != null)
        {
            var inputManagerType = InputManager.Instance.GetType();
            var attackLightLimitField = inputManagerType.GetField("attackLightLimit", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (attackLightLimitField != null)
            {
                originalAttackLightLimit = (float)attackLightLimitField.GetValue(InputManager.Instance);
                Debug.Log($"Loaded Attack Light Limit: {originalAttackLightLimit}s");
            }
        }
    }
    
    /// <summary>
    /// Load exp hiện tại từ inventory
    /// </summary>
    protected virtual void LoadExpFromInventory()
    {
        if (InventoryManager.Instance == null) return;
        
        // PlayerExp được lưu trong Monies() inventory, không phải Items()
        var expItem = InventoryManager.Instance.Monies().FindItem(ItemCode.PlayerExp);
        if (expItem != null)
        {
            currentExp = expItem.itemCount;
            Debug.Log($"Loaded {currentExp} exp from inventory");
        }
        
        this.CheckLevelUp();
    }
    
    /// <summary>
    /// Thêm exp cho player
    /// </summary>
    public virtual void AddExp(int expAmount)
    {
        currentExp += expAmount;
        Debug.Log($"Added {expAmount} exp! Total: {currentExp}");
        
        this.CheckLevelUp();
    }
    
    /// <summary>
    /// Kiểm tra và thực hiện level up
    /// </summary>
    protected virtual void CheckLevelUp()
    {
        while (currentExp >= expToNextLevel)
        {
            this.LevelUp();
        }
    }
    
    /// <summary>
    /// Thực hiện level up
    /// </summary>
    protected virtual void LevelUp()
    {
        currentExp -= expToNextLevel;
        currentLevel++;
        
        // Tăng exp cần thiết cho level tiếp theo
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expMultiplier);
        
        // Tăng stats
        this.IncreaseStats();
        
        // Cập nhật player stats
        this.UpdatePlayerStats();
        
        // Hiển thị thông báo
        this.ShowLevelUpNotification();
        
        Debug.Log($"Level Up! New Level: {currentLevel}");
    }
    
    /// <summary>
    /// Tăng các stats theo level
    /// </summary>
    protected virtual void IncreaseStats()
    {
        // Movement Speed không tăng theo level (đã tắt)
        // currentMovementSpeedMultiplier += movementSpeedBonus;
        
        // Tăng Attack Speed (+3% mỗi level)
        currentAttackSpeedMultiplier += attackSpeedBonus;
        
        // Giảm Skill Cooldown (-2% mỗi level, tối thiểu 0.1x)
        currentSkillCooldownMultiplier = Mathf.Max(0.1f, currentSkillCooldownMultiplier - skillCooldownReduction);
        
        Debug.Log($"Stats increased - Movement: {currentMovementSpeedMultiplier:F2}x (unchanged), Attack: {currentAttackSpeedMultiplier:F2}x, Cooldown: {currentSkillCooldownMultiplier:F2}x");
    }
    
    /// <summary>
    /// Cập nhật stats của player
    /// </summary>
    protected virtual void UpdatePlayerStats()
    {
        if (playerCtrl == null) return;
        
        // Movement speed không được cập nhật (đã tắt)
        // this.UpdateMovementSpeed();
        
        // Cập nhật attack speed
        this.UpdateAttackSpeed();
        
        // Cập nhật skill cooldown
        this.UpdateSkillCooldown();
    }
    
    /// <summary>
    /// Cập nhật tốc độ di chuyển
    /// </summary>
    protected virtual void UpdateMovementSpeed()
    {
        if (playerCtrl.ThirdPersonController == null) 
        {
            Debug.LogWarning("ThirdPersonController is null!");
            return;
        }
        
        // Lấy vThirdPersonMotor component
        var motor = playerCtrl.ThirdPersonController.GetComponent<Invector.vCharacterController.vThirdPersonMotor>();
        if (motor == null)
        {
            Debug.LogWarning("vThirdPersonMotor component not found!");
            return;
        }
        
        Debug.Log($"=== UPDATING MOVEMENT SPEED (Level {currentLevel}) ===");
        Debug.Log($"Multiplier: {currentMovementSpeedMultiplier:F2}x");
        
        // Cập nhật Free Speed
        Debug.Log($"Free Speed - Walk: {motor.freeSpeed.walkSpeed:F2} → {motor.freeSpeed.walkSpeed * currentMovementSpeedMultiplier:F2}");
        motor.freeSpeed.walkSpeed *= currentMovementSpeedMultiplier;
        
        Debug.Log($"Free Speed - Running: {motor.freeSpeed.runningSpeed:F2} → {motor.freeSpeed.runningSpeed * currentMovementSpeedMultiplier:F2}");
        motor.freeSpeed.runningSpeed *= currentMovementSpeedMultiplier;
        
        Debug.Log($"Free Speed - Sprint: {motor.freeSpeed.sprintSpeed:F2} → {motor.freeSpeed.sprintSpeed * currentMovementSpeedMultiplier:F2}");
        motor.freeSpeed.sprintSpeed *= currentMovementSpeedMultiplier;
        
        // Cập nhật Strafe Speed
        Debug.Log($"Strafe Speed - Walk: {motor.strafeSpeed.walkSpeed:F2} → {motor.strafeSpeed.walkSpeed * currentMovementSpeedMultiplier:F2}");
        motor.strafeSpeed.walkSpeed *= currentMovementSpeedMultiplier;
        
        Debug.Log($"Strafe Speed - Running: {motor.strafeSpeed.runningSpeed:F2} → {motor.strafeSpeed.runningSpeed * currentMovementSpeedMultiplier:F2}");
        motor.strafeSpeed.runningSpeed *= currentMovementSpeedMultiplier;
        
        Debug.Log($"Strafe Speed - Sprint: {motor.strafeSpeed.sprintSpeed:F2} → {motor.strafeSpeed.sprintSpeed * currentMovementSpeedMultiplier:F2}");
        motor.strafeSpeed.sprintSpeed *= currentMovementSpeedMultiplier;
        
        // Cập nhật Air Speed
        Debug.Log($"Air Speed: {motor.airSpeed:F2} → {motor.airSpeed * currentMovementSpeedMultiplier:F2}");
        motor.airSpeed *= currentMovementSpeedMultiplier;
        
        Debug.Log("✅ All movement speeds updated successfully!");
    }
    
    /// <summary>
    /// Cập nhật tốc độ tấn công
    /// </summary>
    protected virtual void UpdateAttackSpeed()
    {
        Debug.Log($"=== UPDATING ATTACK SPEED (Level {currentLevel}) ===");
        Debug.Log($"Multiplier: {currentAttackSpeedMultiplier:F2}x");
        
        // Cập nhật Attack Heavy cooldown (giảm cooldown = tăng attack speed)
        this.UpdateAttackHeavyCooldown();
        
        // Cập nhật Attack Light cooldown (giảm cooldown = tăng attack speed)
        this.UpdateAttackLightCooldown();
        
        // Cập nhật InputManager attack light limit (giảm limit = tăng attack speed)
        this.UpdateAttackLightLimit();
        
        Debug.Log("✅ Attack speed updated successfully!");
    }
    
    /// <summary>
    /// Cập nhật cooldown của Attack Heavy
    /// </summary>
    protected virtual void UpdateAttackHeavyCooldown()
    {
        var attackHeavy = playerCtrl.GetComponentInChildren<AttackHeavy>();
        if (attackHeavy == null)
        {
            Debug.LogWarning("AttackHeavy component not found!");
            return;
        }
        
        var cooldownField = attackHeavy.GetType().GetField("cooldown", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (cooldownField != null)
        {
            // Sử dụng original cooldown thay vì current cooldown
            float newCooldown = originalAttackHeavyCooldown / currentAttackSpeedMultiplier;
            cooldownField.SetValue(attackHeavy, newCooldown);
            Debug.Log($"Attack Heavy Cooldown: {originalAttackHeavyCooldown:F2}s → {newCooldown:F2}s (Level {currentLevel})");
        }
    }
    
    /// <summary>
    /// Cập nhật cooldown của Attack Light
    /// </summary>
    protected virtual void UpdateAttackLightCooldown()
    {
        var attackLight = playerCtrl.GetComponentInChildren<AttackLight>();
        if (attackLight == null)
        {
            Debug.LogWarning("AttackLight component not found!");
            return;
        }
        
        var cooldownField = attackLight.GetType().GetField("cooldown", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (cooldownField != null)
        {
            // Sử dụng original cooldown thay vì current cooldown
            float newCooldown = originalAttackLightCooldown / currentAttackSpeedMultiplier;
            cooldownField.SetValue(attackLight, newCooldown);
            Debug.Log($"Attack Light Cooldown: {originalAttackLightCooldown:F2}s → {newCooldown:F2}s (Level {currentLevel})");
        }
    }
    
    /// <summary>
    /// Cập nhật attack light limit trong InputManager
    /// </summary>
    protected virtual void UpdateAttackLightLimit()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("InputManager.Instance is null!");
            return;
        }
        
        var inputManagerType = InputManager.Instance.GetType();
        var attackLightLimitField = inputManagerType.GetField("attackLightLimit", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (attackLightLimitField != null)
        {
            // Sử dụng giá trị gốc thay vì giá trị hiện tại
            float newLimit = originalAttackLightLimit / currentAttackSpeedMultiplier;
            attackLightLimitField.SetValue(InputManager.Instance, newLimit);
            Debug.Log($"Attack Light Limit: {originalAttackLightLimit:F2}s → {newLimit:F2}s (Level {currentLevel})");
        }
    }
    
    /// <summary>
    /// Cập nhật cooldown của skills
    /// </summary>
    protected virtual void UpdateSkillCooldown()
    {
        // Tìm tất cả skills có cooldown và cập nhật
        var skillComponents = FindObjectsOfType<MonoBehaviour>();
        foreach (var component in skillComponents)
        {
            var componentType = component.GetType();
            var cooldownField = componentType.GetField("cooldown", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (cooldownField != null && cooldownField.FieldType == typeof(float))
            {
                float baseCooldown = (float)cooldownField.GetValue(component);
                float newCooldown = baseCooldown * currentSkillCooldownMultiplier;
                cooldownField.SetValue(component, newCooldown);
            }
        }
        
        Debug.Log($"Skill cooldowns updated with multiplier: {currentSkillCooldownMultiplier:F2}x");
    }
    
    /// <summary>
    /// Hiển thị thông báo level up
    /// </summary>
    protected virtual void ShowLevelUpNotification()
    {
        Debug.Log($"🎉 LEVEL UP! Level {currentLevel} 🎉");
        Debug.Log($"📈 Movement Speed: Unchanged (Total: {currentMovementSpeedMultiplier:F2}x)");
        Debug.Log($"⚔️ Attack Speed: +{attackSpeedBonus * 100:F0}% (Total: {currentAttackSpeedMultiplier:F2}x)");
        Debug.Log($"⚡ Skill Cooldown: -{skillCooldownReduction * 100:F0}% (Total: {currentSkillCooldownMultiplier:F2}x)");
        
        // Có thể thêm UI notification ở đây
        // NotificationManager.Instance.ShowNotification($"Level Up! Level {currentLevel}");
    }
    
    /// <summary>
    /// Lấy thông tin level hiện tại (đồng bộ với hệ thống cũ)
    /// </summary>
    public virtual int GetCurrentLevel()
    {
        // Ưu tiên sử dụng level từ hệ thống cũ nếu có
        if (playerLevel != null)
        {
            return playerLevel.CurrentLevel;
        }
        return currentLevel;
    }
    
    /// <summary>
    /// Lấy exp hiện tại
    /// </summary>
    public virtual int GetCurrentExp()
    {
        return currentExp;
    }
    
    /// <summary>
    /// Lấy exp cần thiết cho level tiếp theo
    /// </summary>
    public virtual int GetExpToNextLevel()
    {
        return expToNextLevel;
    }
    
    /// <summary>
    /// Lấy tỷ lệ exp hiện tại (0-1)
    /// </summary>
    public virtual float GetExpProgress()
    {
        return (float)currentExp / expToNextLevel;
    }
    
    /// <summary>
    /// Lấy thông tin stats hiện tại
    /// </summary>
    public virtual (float movement, float attack, float cooldown) GetCurrentStats()
    {
        return (currentMovementSpeedMultiplier, currentAttackSpeedMultiplier, currentSkillCooldownMultiplier);
    }
    
    /// <summary>
    /// Được gọi khi level up từ hệ thống cũ
    /// </summary>
    public virtual void OnLevelUpFromOldSystem(int levelsGained)
    {
        Debug.Log($"OnLevelUpFromOldSystem called with {levelsGained} levels gained");
        
        // Tăng stats theo số level đã tăng
        for (int i = 0; i < levelsGained; i++)
        {
            this.IncreaseStats();
        }
        
        // Cập nhật player stats
        this.UpdatePlayerStats();
        
        // Hiển thị thông báo
        this.ShowLevelUpNotification();
        
        Debug.Log($"Stats updated for {levelsGained} level(s) gained!");
    }
    
    /// <summary>
    /// Test method để kiểm tra stats hiện tại
    /// </summary>
    [ContextMenu("Test Current Stats")]
    public virtual void TestCurrentStats()
    {
        Debug.Log("=== CURRENT STATS TEST ===");
        Debug.Log($"Level: {currentLevel}");
        Debug.Log($"Movement Speed Multiplier: {currentMovementSpeedMultiplier:F2}x");
        Debug.Log($"Attack Speed Multiplier: {currentAttackSpeedMultiplier:F2}x");
        Debug.Log($"Skill Cooldown Multiplier: {currentSkillCooldownMultiplier:F2}x");
        Debug.Log($"Original Attack Light Cooldown: {originalAttackLightCooldown:F2}s");
        Debug.Log($"Original Attack Heavy Cooldown: {originalAttackHeavyCooldown:F2}s");
        
        if (playerCtrl != null)
        {
            Debug.Log($"PlayerCtrl found: {playerCtrl.name}");
            Debug.Log($"ThirdPersonController: {(playerCtrl.ThirdPersonController != null ? "Found" : "NULL")}");
            Debug.Log($"Weapons: {(playerCtrl.Weapons != null ? "Found" : "NULL")}");
        }
        else
        {
            Debug.LogWarning("PlayerCtrl is NULL!");
        }
        
        Debug.Log("========================");
    }
}
