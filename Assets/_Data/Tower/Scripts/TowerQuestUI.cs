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
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadQuestUI();
    }
    
    protected virtual void LoadQuestUI()
    {
        if (this.questPanel != null) return;
        
        Debug.Log("LoadQuestUI: Bắt đầu tìm UI elements...");
        
        // Tìm UI elements trong scene
        this.questPanel = GameObject.Find("QuestPanel");
        if (this.questPanel != null)
        {
            Debug.Log("LoadQuestUI: Đã tìm thấy QuestPanel");
            
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
            Debug.LogError("LoadQuestUI: KHÔNG TÌM THẤY QuestPanel trong scene!");
        }
        
        // Tìm notification panel
        this.notificationPanel = GameObject.Find("NotificationPanel");
        if (this.notificationPanel != null)
        {
            this.notificationText = this.notificationPanel.transform.Find("NotificationText")?.GetComponent<TextMeshProUGUI>();
        }
    }
    
    protected virtual void Update()
    {
        this.UpdateQuestDisplay();
    }
    
    public virtual void UpdateQuestDisplay()
    {
        if (TowerQuestSystem.Instance == null)
        {
            Debug.LogWarning("TowerQuestSystem.Instance is null!");
            return;
        }
        
        if (this.questPanel == null)
        {
            Debug.LogWarning("QuestPanel is null! Không thể cập nhật UI");
            return;
        }
        
        // Lấy thông tin nhiệm vụ (kể cả đã hoàn thành)
        var allQuests = TowerQuestSystem.Instance.GetAllQuests();
        Debug.Log($"UpdateQuestDisplay: Số quest hiện có: {allQuests.Count}");
        
        if (allQuests.Count > 0)
        {
            // Tìm nhiệm vụ chưa hoàn thành đầu tiên, nếu không có thì lấy nhiệm vụ cuối cùng
            var currentQuest = allQuests.Find(q => !q.isCompleted) ?? allQuests[allQuests.Count - 1];
            
            // QUAN TRỌNG: Lấy tiến độ phù hợp với từng quest
            int currentProgress = this.GetProgressForQuest(currentQuest);
            
            // Debug: Kiểm tra tiến độ real-time
            Debug.Log($"UI Update: Tiến độ {currentProgress}/{currentQuest.requiredTowerCount} - Quest: {currentQuest.questName}");
            Debug.Log($"UI Update: QuestTitleText null? {this.questTitleText == null}");
            Debug.Log($"UI Update: ProgressText null? {this.progressText == null}");
            Debug.Log($"UI Update: ProgressBar null? {this.progressBar == null}");
            
            if (this.questTitleText != null)
            {
                if (currentQuest.isCompleted)
                    this.questTitleText.text = $"✅ {currentQuest.questName} - HOÀN THÀNH!";
                else
                    this.questTitleText.text = $"🔒 {currentQuest.questName}";
                Debug.Log($"Đã cập nhật QuestTitleText: {this.questTitleText.text}");
            }
                
            if (this.questDescriptionText != null)
            {
                if (currentQuest.isCompleted)
                    this.questDescriptionText.text = $"Nhiệm vụ đã hoàn thành!\n\nTower đã mở khóa: {this.GetTowerDisplayName(currentQuest.unlockedTower)}";
                else
                    this.questDescriptionText.text = $"{currentQuest.description}\n\nTower bị khóa: {this.GetTowerDisplayName(currentQuest.unlockedTower)}";
                Debug.Log($"Đã cập nhật QuestDescriptionText: {this.questDescriptionText.text}");
            }
                
            if (this.progressText != null)
            {
                this.progressText.text = $"Tiến độ: {currentProgress}/{currentQuest.requiredTowerCount}";
                Debug.Log($"Đã cập nhật ProgressText: {this.progressText.text}");
            }
                
            if (this.progressBar != null)
            {
                this.progressBar.maxValue = currentQuest.requiredTowerCount;
                this.progressBar.value = currentProgress;
                Debug.Log($"Đã cập nhật ProgressBar: maxValue={this.progressBar.maxValue}, value={this.progressBar.value}");
            }
            
            // Tự động ẩn panel sau 2 giây khi hoàn thành nhiệm vụ (chỉ gọi 1 lần)
            if (currentQuest.isCompleted && this.questPanel.activeSelf && !this.isAutoHideScheduled)
            {
                this.isAutoHideScheduled = true;
                Invoke(nameof(this.HideQuestPanel), 2f);
            }
        }
        else
        {
            Debug.LogWarning("Không có quest nào để hiển thị!");
        }
    }
    
    public virtual void ShowQuestCompletedNotification(string questName, string unlockedTower)
    {
        if (this.notificationPanel == null || this.notificationText == null) return;
        
        this.notificationText.text = $"Hoàn thành: {questName}\nMở khóa: {unlockedTower}";
        this.notificationPanel.SetActive(true);
        
        // Tự động ẩn sau một thời gian
        Invoke(nameof(this.HideNotification), this.notificationDuration);
    }
    
    public virtual void ShowNewQuestNotification(string questName, string description)
    {
        if (this.notificationPanel == null || this.notificationText == null) return;
        
        this.notificationText.text = $"🎯 Nhiệm vụ mới: {questName}\n{description}";
        this.notificationPanel.SetActive(true);
        
        // Tự động ẩn sau một thời gian
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
    
    protected virtual void HideQuestPanel()
    {
        if (this.questPanel != null)
        {
            this.questPanel.SetActive(false);
            this.isAutoHideScheduled = false; // Reset biến để có thể gọi lại
            Debug.Log("Quest Panel đã tự động ẩn sau khi hoàn thành nhiệm vụ!");
        }
    }
    
    public virtual void ShowQuestPanel()
    {
        if (this.questPanel != null)
        {
            this.questPanel.SetActive(true);
            this.isAutoHideScheduled = false; // Reset biến khi hiện panel
        }
    }
    
    protected virtual string GetTowerDisplayName(TowerCode towerCode)
    {
        switch (towerCode)
        {
            case TowerCode.MachineGun: return "Machine Gun (Phím 1)";
            case TowerCode.OneGunBarrel: return "One Gun Barrel (Phím 2)";
            case TowerCode.IceTrap: return "Ice Trap (Phím 3)";
            case TowerCode.FlameTrap: return "Flame Trap (Phím 4)";
            case TowerCode.Core: return "Core Tower (Phím 5)";
            default: return towerCode.ToString();
        }
    }
    
    /// <summary>
    /// Lấy tiến độ phù hợp cho từng quest
    /// </summary>
    protected virtual int GetProgressForQuest(TowerQuest quest)
    {
        if (quest == null || TowerQuestSystem.Instance == null) return 0;
        
        // Quest tutorial di chuyển: kiểm tra movement tutorial
        if (quest.questName == "Movement Tutorial")
        {
            int movementProgress = TowerQuestSystem.Instance.GetMovementTutorialStatus();
            Debug.Log($"🎮 Movement Tutorial Progress: {movementProgress}/1");
            return movementProgress;
        }
        // Quest 2: Đếm OneGunBarrel towers
        else if (quest.questName == "Tower Builder II")
        {
            int oneGunBarrelProgress = TowerQuestSystem.Instance.GetOneGunBarrelTowersPlaced();
            Debug.Log($"🎯 Quest II Progress: {oneGunBarrelProgress} OneGunBarrel towers");
            return oneGunBarrelProgress;
        }
        // Quest 3: Đếm Ice Trap towers
        else if (quest.questName == "Tower Builder III")
        {
            int iceTrapProgress = TowerQuestSystem.Instance.GetIceTrapTowersPlaced();
            Debug.Log($"🎯 Quest III Progress: {iceTrapProgress} Ice Trap towers");
            return iceTrapProgress;
        }
        // Quest 1 và các quest khác: Đếm tất cả towers (trừ OneGunBarrel, IceTrap)
        else
        {
            int totalProgress = TowerQuestSystem.Instance.GetTotalTowersPlaced();
            Debug.Log($"🎯 Quest {quest.questName} Progress: {totalProgress} towers");
            return totalProgress;
        }
    }
} 