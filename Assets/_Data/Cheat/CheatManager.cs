using UnityEngine;

/// <summary>
/// Manager để quản lý các phím tắt cheat code cho game
/// </summary>
public class CheatManager : SaiSingleton<CheatManager>
{
    [Header("Cheat Settings")]
    [SerializeField] protected bool enableCheats = true; // Bật/tắt cheat codes
    [SerializeField] protected bool showCheatMessages = true; // Hiển thị thông báo khi dùng cheat
    
    [Header("Cheat Keys")]
    [SerializeField] protected KeyCode toggleCheatsKey = KeyCode.BackQuote; // Phím bật/tắt cheat (`)
    [SerializeField] protected KeyCode godModeKey = KeyCode.F1; // Phím bất tử
    [SerializeField] protected KeyCode addGoldKey = KeyCode.F2; // Phím cộng vàng
    [SerializeField] protected KeyCode healPlayerKey = KeyCode.F3; // Phím hồi máu
    [SerializeField] protected KeyCode killAllEnemiesKey = KeyCode.F4; // Phím giết tất cả enemy
    
    [Header("Cheat Values")]
    [SerializeField] protected int goldAmount = 1000; // Số vàng cộng khi nhấn phím
    [SerializeField] protected int healAmount = 100; // Số máu hồi khi nhấn phím
    
    // Trạng thái cheat
    protected bool isGodModeActive = false;
    protected PlayerDamageReceiver playerDamageReceiver;
    
    protected override void Start()
    {
        base.Start();
        this.LoadPlayerDamageReceiver();
    }
    
    protected virtual void Update()
    {
        this.CheckCheatInputs();
    }
    
    protected virtual void LoadPlayerDamageReceiver()
    {
        if (playerDamageReceiver != null) return;
        playerDamageReceiver = FindObjectOfType<PlayerDamageReceiver>();
        Debug.Log("CheatManager: Loaded PlayerDamageReceiver", gameObject);
    }
    
    /// <summary>
    /// Kiểm tra các phím cheat
    /// </summary>
    protected virtual void CheckCheatInputs()
    {
        // ` - Toggle Cheats (Bật/tắt cheat)
        if (Input.GetKeyDown(toggleCheatsKey))
        {
            this.ToggleCheats();
        }
        
        // Chỉ thực hiện cheat khác khi cheat được bật
        if (!enableCheats) return;
        
        // F1 - God Mode (Bất tử)
        if (Input.GetKeyDown(godModeKey))
        {
            this.ToggleGodMode();
        }
        
        // F2 - Add Gold (Cộng vàng)
        if (Input.GetKeyDown(addGoldKey))
        {
            this.AddGold();
        }
        
        // F3 - Heal Player (Hồi máu)
        if (Input.GetKeyDown(healPlayerKey))
        {
            this.HealPlayer();
        }
        
        // F4 - Kill All Enemies (Giết tất cả enemy)
        if (Input.GetKeyDown(killAllEnemiesKey))
        {
            this.KillAllEnemies();
        }
    }
    
    /// <summary>
    /// Bật/tắt chế độ bất tử
    /// </summary>
    protected virtual void ToggleGodMode()
    {
        if (playerDamageReceiver == null)
        {
            this.ShowCheatMessage("God Mode: PlayerDamageReceiver not found!");
            return;
        }
        
        isGodModeActive = !isGodModeActive;
        
        // Sử dụng reflection để truy cập field isImmotal
        var damageReceiverType = typeof(DamageReceiver);
        var immortalField = damageReceiverType.GetField("isImmotal", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (immortalField != null)
        {
            immortalField.SetValue(playerDamageReceiver, isGodModeActive);
        }
        
        string status = isGodModeActive ? "ACTIVATED" : "DEACTIVATED";
        this.ShowCheatMessage($"God Mode {status}! Player is {(isGodModeActive ? "immortal" : "mortal")}");
    }
    
    /// <summary>
    /// Cộng vàng cho player
    /// </summary>
    protected virtual void AddGold()
    {
        if (InventoryManager.Instance == null)
        {
            this.ShowCheatMessage("Add Gold: InventoryManager not found!");
            return;
        }
        
        InventoryManager.Instance.AddItem(ItemCode.Gold, goldAmount);
        this.ShowCheatMessage($"Added {goldAmount} gold! Total: {this.GetCurrentGold()}");
    }
    
    /// <summary>
    /// Hồi máu cho player
    /// </summary>
    protected virtual void HealPlayer()
    {
        if (playerDamageReceiver == null)
        {
            this.ShowCheatMessage("Heal Player: PlayerDamageReceiver not found!");
            return;
        }
        
        playerDamageReceiver.Heal(healAmount);
        this.ShowCheatMessage($"Healed {healAmount} HP! Current HP: {playerDamageReceiver.CurrentHp}/{playerDamageReceiver.MaxHP}");
    }
    
    /// <summary>
    /// Giết tất cả enemy trên map
    /// </summary>
    protected virtual void KillAllEnemies()
    {
        EnemyCtrl[] enemies = FindObjectsOfType<EnemyCtrl>();
        int killedCount = 0;
        
        foreach (EnemyCtrl enemy in enemies)
        {
            if (enemy.EnemyDamageReceiver != null && !enemy.EnemyDamageReceiver.IsDead())
            {
                enemy.EnemyDamageReceiver.Deduct(enemy.EnemyDamageReceiver.MaxHP);
                killedCount++;
            }
        }
        
        this.ShowCheatMessage($"Killed {killedCount} enemies!");
    }
    
    /// <summary>
    /// Lấy số vàng hiện tại
    /// </summary>
    protected virtual int GetCurrentGold()
    {
        if (InventoryManager.Instance == null) return 0;
        
        var gold = InventoryManager.Instance.Monies().FindItem(ItemCode.Gold);
        return gold != null ? gold.itemCount : 0;
    }
    
    /// <summary>
    /// Hiển thị thông báo cheat
    /// </summary>
    protected virtual void ShowCheatMessage(string message)
    {
        if (!showCheatMessages) return;
        
        Debug.Log($"[CHEAT] {message}");
        
        // Có thể thêm UI notification ở đây nếu cần
        // Ví dụ: NotificationManager.Instance.ShowNotification(message);
    }
    
    /// <summary>
    /// Bật/tắt cheat codes
    /// </summary>
    public virtual void SetCheatsEnabled(bool enabled)
    {
        enableCheats = enabled;
        string status = enabled ? "ENABLED" : "DISABLED";
        this.ShowCheatMessage($"Cheat codes {status}");
    }
    
    /// <summary>
    /// Bật/tắt cheat codes bằng phím tắt
    /// </summary>
    protected virtual void ToggleCheats()
    {
        enableCheats = !enableCheats;
        string status = enableCheats ? "ENABLED" : "DISABLED";
        this.ShowCheatMessage($"Cheat codes {status} (Press ` to toggle)");
        
        // Nếu tắt cheat, cũng tắt God Mode
        if (!enableCheats && isGodModeActive)
        {
            this.ToggleGodMode();
        }
    }
    
    /// <summary>
    /// Kiểm tra trạng thái God Mode
    /// </summary>
    public virtual bool IsGodModeActive()
    {
        return isGodModeActive;
    }
    
    /// <summary>
    /// Thêm vàng với số lượng tùy chỉnh
    /// </summary>
    public virtual void AddCustomGold(int amount)
    {
        if (amount <= 0) return;
        
        InventoryManager.Instance.AddItem(ItemCode.Gold, amount);
        this.ShowCheatMessage($"Added {amount} gold! Total: {this.GetCurrentGold()}");
    }
}
