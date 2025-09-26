using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerQuestUI : SaiSingleton<TowerQuestUI>
{
    [Header("Quest Display")]
    [SerializeField] protected GameObject questPanel;
    [SerializeField] protected TextMeshProUGUI questTitleText;
    [SerializeField] protected TextMeshProUGUI questDescriptionText;
    [SerializeField] protected TextMeshProUGUI progressText;
    [SerializeField] protected Slider progressBar;
    
    [Header("Notification")]
    [SerializeField] protected GameObject notificationPanel;
    [SerializeField] protected TextMeshProUGUI notificationText;
    [SerializeField] protected float notificationDuration = 3f;
    
    [Header("Auto Hide")]
    [SerializeField] protected bool isAutoHideScheduled = false;
    
    [Header("Scene Management")]
    [SerializeField] protected bool isDestroyed = false; // Track if this instance has been destroyed
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        
        // Kiểm tra scene hiện tại - chỉ hoạt động ở Hai_SampleScene
        if (!this.IsValidScene())
        {
            Debug.Log("TowerQuestUI: Not in Hai_SampleScene, destroying instance");
            this.DestroyInstance();
            return;
        }
        
        this.LoadQuestUI();
    }
    
    protected override void Start()
    {
        base.Start();
        
        // Kiểm tra nếu instance đã bị destroy
        if (this.isDestroyed)
        {
            return;
        }
        
        // Ẩn QuestPanel nếu đang ở Map1 (Hai_Map)
        if (this.IsMap1())
        {
            this.HideQuestPanelForMap1();
        }
    }
    
    protected virtual void LoadQuestUI()
    {
        if (this.questPanel != null) return;
        
        Debug.Log("LoadQuestUI: Starting to find UI elements...");
        
        // Find UI elements in scene
        this.questPanel = GameObject.Find("QuestPanel");
        if (this.questPanel != null)
        {
            Debug.Log("LoadQuestUI: Found QuestPanel");
            
            this.questTitleText = this.questPanel.transform.Find("QuestTitleText")?.GetComponent<TextMeshProUGUI>();
            this.questDescriptionText = this.questPanel.transform.Find("QuestDescriptionText")?.GetComponent<TextMeshProUGUI>();
            this.progressText = this.questPanel.transform.Find("ProgressText")?.GetComponent<TextMeshProUGUI>();
            this.progressBar = this.questPanel.transform.Find("ProgressBar")?.GetComponent<Slider>();
            
            Debug.Log($"LoadQuestUI: QuestTitleText null? {this.questTitleText == null}");
            Debug.Log($"LoadQuestUI: ProgressText null? {this.progressText == null}");
            Debug.Log($"LoadQuestUI: ProgressBar null? {this.progressBar == null}");
        }
        else
        {
            Debug.LogError("LoadQuestUI: QuestPanel NOT FOUND in scene!");
        }
        
        // Find notification panel
        this.notificationPanel = GameObject.Find("NotificationPanel");
        if (this.notificationPanel != null)
        {
            this.notificationText = this.notificationPanel.transform.Find("NotificationText")?.GetComponent<TextMeshProUGUI>();
        }
    }
    
    protected virtual void Update()
    {
        // Kiểm tra nếu instance đã bị destroy
        if (this.isDestroyed)
        {
            return;
        }
        
        this.UpdateQuestDisplay();
    }
    
    public virtual void UpdateQuestDisplay()
    {
        // Kiểm tra nếu instance đã bị destroy
        if (this.isDestroyed)
        {
            return;
        }
        
        if (TowerQuestSystem.Instance == null)
        {
            Debug.LogWarning("TowerQuestSystem.Instance is null!");
            return;
        }
        
        if (this.questPanel == null)
        {
            Debug.LogWarning("QuestPanel is null! Cannot update UI");
            return;
        }
        
        // Ẩn quest panel nếu đang ở Map 1
        if (IsMap1())
        {
            questPanel.SetActive(false);
            Debug.Log("Map 1 detected - Quest panel hidden!");
            return;
        }
        
        // Get quest information (including completed ones)
        var allQuests = TowerQuestSystem.Instance.GetAllQuests();
        Debug.Log($"UpdateQuestDisplay: Current number of quests: {allQuests.Count}");
        
        if (allQuests.Count > 0)
        {
            // Find first incomplete quest, if none then take the last quest
            var currentQuest = allQuests.Find(q => !q.isCompleted) ?? allQuests[allQuests.Count - 1];
            
            // IMPORTANT: Get appropriate progress for each quest
            int currentProgress = this.GetProgressForQuest(currentQuest);
            
            // Debug: Check real-time progress
            Debug.Log($"UI Update: Progress {currentProgress}/{currentQuest.requiredTowerCount} - Quest: {currentQuest.questName}");
            Debug.Log($"UI Update: QuestTitleText null? {this.questTitleText == null}");
            Debug.Log($"UI Update: ProgressText null? {this.progressText == null}");
            Debug.Log($"UI Update: ProgressBar null? {this.progressBar == null}");
            
            if (this.questTitleText != null)
            {
                if (currentQuest.isCompleted)
                    this.questTitleText.text = $"{currentQuest.questName} - COMPLETED!";
                else
                    this.questTitleText.text = $"{currentQuest.questName}";
                Debug.Log($"Updated QuestTitleText: {this.questTitleText.text}");
            }
                
            if (this.questDescriptionText != null)
            {
                if (currentQuest.isCompleted)
                    this.questDescriptionText.text = $"Quest completed!\n\nTower unlocked: {this.GetTowerDisplayName(currentQuest.unlockedTower)}";
                else
                    this.questDescriptionText.text = $"{currentQuest.description}\n\nTower locked: {this.GetTowerDisplayName(currentQuest.unlockedTower)}";
                Debug.Log($"Updated QuestDescriptionText: {this.questDescriptionText.text}");
            }
                
            if (this.progressText != null)
            {
                this.progressText.text = $"Progress: {currentProgress}/{currentQuest.requiredTowerCount}";
                Debug.Log($"Updated ProgressText: {this.progressText.text}");
            }
                
            if (this.progressBar != null)
            {
                this.progressBar.maxValue = currentQuest.requiredTowerCount;
                this.progressBar.value = currentProgress;
                Debug.Log($"Updated ProgressBar: maxValue={this.progressBar.maxValue}, value={this.progressBar.value}");
            }
            
            // Auto hide panel after 2 seconds when quest is completed (only call once)
            if (currentQuest.isCompleted && this.questPanel.activeSelf && !this.isAutoHideScheduled)
            {
                this.isAutoHideScheduled = true;
                Invoke(nameof(this.HideQuestPanel), 2f);
            }
        }
        else
        {
            Debug.LogWarning("No quests to display!");
        }
    }
    
    public virtual void ShowQuestCompletedNotification(string questName, string unlockedTower)
    {
        if (this.notificationPanel == null || this.notificationText == null) return;
        
        this.notificationText.text = $"Completed: {questName}\nUnlocked: {unlockedTower}";
        this.notificationPanel.SetActive(true);
        
        // Auto hide after a period of time
        Invoke(nameof(this.HideNotification), this.notificationDuration);
    }
    
    public virtual void ShowNewQuestNotification(string questName, string description)
    {
        if (this.notificationPanel == null || this.notificationText == null) return;
        
        this.notificationText.text = $"New Quest: {questName}\n{description}";
        this.notificationPanel.SetActive(true);
        
        // Auto hide after a period of time
        Invoke(nameof(this.HideNotification), this.notificationDuration);
    }
    
    protected virtual void HideNotification()
    {
        if (this.notificationPanel != null)
            this.notificationPanel.SetActive(false);
    }
    
    public virtual void ToggleQuestPanel()
    {
        if (this.questPanel != null)
        {
            this.questPanel.SetActive(!this.questPanel.activeSelf);
        }
    }
    
    public virtual void HideQuestPanel()
    {
        if (this.questPanel != null)
        {
            this.questPanel.SetActive(false);
            this.isAutoHideScheduled = false; // Reset variable to allow calling again
            Debug.Log("Quest Panel automatically hidden after completing quest!");
        }
    }
    
    /// <summary>
    /// Ẩn QuestPanel khi vào Map1 (Hai_Map) để tránh hiện UI nhiệm vụ từ SampleScene
    /// </summary>
    protected virtual void HideQuestPanelForMap1()
    {
        try
        {
            Debug.Log("=== HIDING QUEST PANEL FOR MAP1 ===");
            Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            
            if (this.questPanel != null)
            {
                this.questPanel.SetActive(false);
                Debug.Log("QuestPanel hidden for Map1 - preventing SampleScene quest UI from showing");
            }
            else
            {
                Debug.LogWarning("QuestPanel is null - cannot hide for Map1");
            }
            
            // Ẩn notification panel nếu có
            if (this.notificationPanel != null)
            {
                this.notificationPanel.SetActive(false);
                Debug.Log("NotificationPanel hidden for Map1");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error hiding QuestPanel for Map1: {e.Message}");
        }
    }
    
    public virtual void ShowQuestPanel()
    {
        if (this.questPanel != null)
        {
            this.questPanel.SetActive(true);
            this.isAutoHideScheduled = false; // Reset variable when showing panel
        }
    }
    
    protected virtual string GetTowerDisplayName(TowerCode towerCode)
    {
        switch (towerCode)
        {
            case TowerCode.MachineGun: return "Machine Gun (Key 1)";
            case TowerCode.OneGunBarrel: return "One Gun Barrel (Key 2)";
            case TowerCode.IceTrap: return "Ice Trap (Key 3)";
            case TowerCode.FlameTrap: return "Flame Trap (Key 4)";
            case TowerCode.Core: return "Core Tower (Key 5)";
            default: return towerCode.ToString();
        }
    }
    
    /// <summary>
    /// Get appropriate progress for each quest
    /// </summary>
    protected virtual int GetProgressForQuest(TowerQuest quest)
    {
        if (quest == null || TowerQuestSystem.Instance == null) return 0;
        
        // Quest tutorial di chuyển: kiểm tra movement tutorial
        if (quest.questName == "Movement Tutorial")
        {
            int movementProgress = TowerQuestSystem.Instance.GetMovementTutorialStatus();
            Debug.Log($"Movement Tutorial Progress: {movementProgress}/1");
            return movementProgress;
        }
        // Quest 2: Đếm OneGunBarrel towers
        else if (quest.questName == "Tower Builder II")
        {
            int oneGunBarrelProgress = TowerQuestSystem.Instance.GetOneGunBarrelTowersPlaced();
            Debug.Log($"Quest II Progress: {oneGunBarrelProgress} OneGunBarrel towers");
            return oneGunBarrelProgress;
        }
        // Quest 3: Đếm Ice Trap towers
        else if (quest.questName == "Tower Builder III")
        {
            int iceTrapProgress = TowerQuestSystem.Instance.GetIceTrapTowersPlaced();
            Debug.Log($"Quest III Progress: {iceTrapProgress} Ice Trap towers");
            return iceTrapProgress;
        }
        // Quest 1 và các quest khác: Đếm tất cả towers (trừ OneGunBarrel, IceTrap)
        else
        {
            int totalProgress = TowerQuestSystem.Instance.GetTotalTowersPlaced();
            Debug.Log($"Quest {quest.questName} Progress: {totalProgress} towers");
            return totalProgress;
        }
    }
    
    /// <summary>
    /// Kiểm tra xem có phải Map 1 không
    /// </summary>
    protected virtual bool IsMap1()
    {
        try
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            // Map 1 của Bệ Hạ là "Hai_Map"
            return currentSceneName == "Hai_Map";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong IsMap1: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Kiểm tra xem có phải scene hợp lệ không (chỉ Hai_SampleScene)
    /// </summary>
    protected virtual bool IsValidScene()
    {
        try
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            bool isValid = currentSceneName == "Hai_SampleScene";
            Debug.Log($"TowerQuestUI: Current scene '{currentSceneName}', IsValid: {isValid}");
            return isValid;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong IsValidScene: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Destroy instance khi không ở scene hợp lệ
    /// </summary>
    protected virtual void DestroyInstance()
    {
        if (this.isDestroyed) return;
        
        Debug.Log("TowerQuestUI: Destroying instance - not in Hai_SampleScene");
        this.isDestroyed = true;
        
        // Hide all UI elements
        if (this.questPanel != null)
        {
            this.questPanel.SetActive(false);
        }
        
        if (this.notificationPanel != null)
        {
            this.notificationPanel.SetActive(false);
        }
        
        // Destroy GameObject
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Kiểm tra xem instance có bị destroy không
    /// </summary>
    public virtual bool IsDestroyed()
    {
        return this.isDestroyed;
    }
} 