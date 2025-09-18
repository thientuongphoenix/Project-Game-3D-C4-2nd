using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerLockedNotifier : SaiSingleton<TowerLockedNotifier>
{
    [Header("UI Elements")]
    [SerializeField] protected GameObject lockedNotificationPanel;
    [SerializeField] protected TextMeshProUGUI lockedNotificationText;
    [SerializeField] protected float notificationDuration = 3f;
    
    [Header("Quest Info")]
    [SerializeField] protected GameObject questInfoPanel;
    [SerializeField] protected TextMeshProUGUI questInfoText;
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadUIElements();
    }
    
    protected virtual void LoadUIElements()
    {
        if (this.lockedNotificationPanel != null) return;
        
        // Tìm UI elements trong scene
        this.lockedNotificationPanel = GameObject.Find("LockedNotificationPanel");
        if (this.lockedNotificationPanel != null)
        {
            this.lockedNotificationText = this.lockedNotificationPanel.transform.Find("LockedNotificationText")?.GetComponent<TextMeshProUGUI>();
        }
        
        this.questInfoPanel = GameObject.Find("QuestInfoPanel");
        if (this.questInfoPanel != null)
        {
            this.questInfoText = this.questInfoPanel.transform.Find("QuestInfoText")?.GetComponent<TextMeshProUGUI>();
        }
    }
    
    public virtual void ShowTowerLockedNotification(TowerCode towerCode)
    {
        if (this.lockedNotificationPanel == null || this.lockedNotificationText == null) return;
        
        string towerName = this.GetTowerDisplayName(towerCode);
        
        // Get current quest information
        string questInfo = "No quest available";
        if (TowerQuestSystem.Instance != null)
        {
            var activeQuests = TowerQuestSystem.Instance.GetActiveQuests();
            if (activeQuests.Count > 0)
            {
                var currentQuest = activeQuests[0];
                int totalPlaced = TowerQuestSystem.Instance.GetTotalTowersPlaced();
                questInfo = $"QUEST: {currentQuest.questName}\n{currentQuest.description}\nProgress: {totalPlaced}/{currentQuest.requiredTowerCount}";
            }
        }
        
        this.lockedNotificationText.text = $"{towerName} is not unlocked!\n\nComplete the quest to unlock.\n\n{questInfo}";
        
        this.lockedNotificationPanel.SetActive(true);
        
        // Auto hide after a period of time
        Invoke(nameof(this.HideLockedNotification), this.notificationDuration);
    }
    
    protected virtual void ShowQuestInfo()
    {
        if (this.questInfoPanel == null || this.questInfoPanel == null) return;
        
        if (TowerQuestSystem.Instance != null)
        {
            var activeQuests = TowerQuestSystem.Instance.GetActiveQuests();
            if (activeQuests.Count > 0)
            {
                var currentQuest = activeQuests[0];
                int totalPlaced = TowerQuestSystem.Instance.GetTotalTowersPlaced();
                
                this.questInfoText.text = $"CURRENT QUEST:\n{currentQuest.questName}\n\n{currentQuest.description}\n\nProgress: {totalPlaced}/{currentQuest.requiredTowerCount}";
                
                this.questInfoPanel.SetActive(true);
                
                // Auto hide after a period of time
                Invoke(nameof(this.HideQuestInfo), this.notificationDuration + 1f);
            }
        }
    }
    
    protected virtual string GetTowerDisplayName(TowerCode towerCode)
    {
        switch (towerCode)
        {
            case TowerCode.OneGunBarrel: return "One Gun Barrel Tower";
            case TowerCode.MachineGun: return "Machine Gun Tower";
            case TowerCode.IceTrap: return "Ice Trap";
            case TowerCode.FlameTrap: return "Flame Trap";
            case TowerCode.Core: return "Core Tower";
            default: return towerCode.ToString();
        }
    }
    
    protected virtual void HideLockedNotification()
    {
        if (this.lockedNotificationPanel != null)
            this.lockedNotificationPanel.SetActive(false);
    }
    
    protected virtual void HideQuestInfo()
    {
        if (this.questInfoPanel != null)
            this.questInfoPanel.SetActive(false);
    }
    
    public virtual void HideAllNotifications()
    {
        this.HideLockedNotification();
        this.HideQuestInfo();
    }
} 