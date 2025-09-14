using UnityEngine;
using TMPro;

public class CountdownTimerUI : SaiSingleton<CountdownTimerUI>
{
    [Header("UI References")]
    [SerializeField] protected GameObject countdownPanel;
    [SerializeField] protected TextMeshProUGUI countdownText;
    [SerializeField] protected TextMeshProUGUI missionText;
    
    [Header("Timer Settings")]
    [SerializeField] protected float totalTime = 60f;
    [SerializeField] protected float currentTime = 0f;
    [SerializeField] protected bool isRunning = false;
    [SerializeField] protected bool isFinalMission = false;
    
    [Header("Visual Effects")]
    [SerializeField] protected Color normalColor = Color.white;
    [SerializeField] protected Color warningColor = Color.yellow;
    [SerializeField] protected Color dangerColor = Color.red;
    [SerializeField] protected float warningThreshold = 30f; // Cảnh báo khi còn 30 giây
    [SerializeField] protected float dangerThreshold = 10f; // Nguy hiểm khi còn 10 giây
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCountdownUI();
    }
    
    protected virtual void LoadCountdownUI()
    {
        if (this.countdownPanel != null) return;
        
        Debug.Log("LoadCountdownUI: Starting to find UI elements...");
        
        // Find countdown panel in scene
        this.countdownPanel = GameObject.Find("CountdownPanel");
        if (this.countdownPanel != null)
        {
            Debug.Log("LoadCountdownUI: Found CountdownPanel");
            
            this.countdownText = this.countdownPanel.transform.Find("CountdownText")?.GetComponent<TextMeshProUGUI>();
            this.missionText = this.countdownPanel.transform.Find("MissionText")?.GetComponent<TextMeshProUGUI>();
            
            Debug.Log($"LoadCountdownUI: CountdownText null? {this.countdownText == null}");
            Debug.Log($"LoadCountdownUI: MissionText null? {this.missionText == null}");
        }
        else
        {
            Debug.LogError("LoadCountdownUI: CountdownPanel NOT FOUND in scene!");
        }
    }
    
    protected virtual void Update()
    {
        if (this.isRunning && this.isFinalMission)
        {
            this.UpdateCountdown();
        }
    }
    
    protected virtual void UpdateCountdown()
    {
        if (this.currentTime > 0f)
        {
            this.currentTime -= Time.deltaTime;
            this.UpdateDisplay();
        }
        else
        {
            // Time's up!
            this.currentTime = 0f;
            this.isRunning = false;
            this.OnTimeUp();
        }
    }
    
    protected virtual void UpdateDisplay()
    {
        if (this.countdownText == null) return;
        
        // Format time as MM:SS
        int minutes = Mathf.FloorToInt(this.currentTime / 60f);
        int seconds = Mathf.FloorToInt(this.currentTime % 60f);
        
        this.countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        // Change color based on remaining time
        if (this.currentTime <= this.dangerThreshold)
        {
            this.countdownText.color = this.dangerColor;
        }
        else if (this.currentTime <= this.warningThreshold)
        {
            this.countdownText.color = this.warningColor;
        }
        else
        {
            this.countdownText.color = this.normalColor;
        }
    }
    
    public virtual void StartCountdown(float time = 60f)
    {
        this.totalTime = time;
        this.currentTime = time;
        this.isRunning = true;
        this.isFinalMission = true;
        
        Debug.Log($"CountdownTimerUI: Started countdown for {time} seconds");
        
        // Show countdown panel
        if (this.countdownPanel != null)
        {
            this.countdownPanel.SetActive(true);
        }
        
        // Update mission text
        if (this.missionText != null)
        {
            this.missionText.text = "DEFEND THE CORE!";
        }
        
        this.UpdateDisplay();
    }
    
    public virtual void StopCountdown()
    {
        this.isRunning = false;
        this.isFinalMission = false;
        
        Debug.Log("CountdownTimerUI: Countdown stopped");
        
        // Hide countdown panel
        if (this.countdownPanel != null)
        {
            this.countdownPanel.SetActive(false);
        }
    }
    
    public virtual void PauseCountdown()
    {
        this.isRunning = false;
        Debug.Log("CountdownTimerUI: Countdown paused");
    }
    
    public virtual void ResumeCountdown()
    {
        if (this.isFinalMission)
        {
            this.isRunning = true;
            Debug.Log("CountdownTimerUI: Countdown resumed");
        }
    }
    
    protected virtual void OnTimeUp()
    {
        Debug.Log("CountdownTimerUI: Time's up! Final mission completed!");
        
        // Notify TowerQuestSystem that final mission is completed
        if (TowerQuestSystem.Instance != null)
        {
            TowerQuestSystem.Instance.CompleteFinalMission();
        }
        
        // Notify GameResultManager that player won
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.OnFinalMissionCompleted();
        }
        
        // Hide countdown panel
        this.StopCountdown();
    }
    
    public virtual float GetRemainingTime()
    {
        return this.currentTime;
    }
    
    public virtual bool IsRunning()
    {
        return this.isRunning;
    }
    
    public virtual bool IsFinalMission()
    {
        return this.isFinalMission;
    }
    
    /// <summary>
    /// Test method để kiểm tra countdown timer
    /// </summary>
    [ContextMenu("Test Countdown Timer")]
    public virtual void TestCountdownTimer()
    {
        Debug.Log("=== TEST COUNTDOWN TIMER ===");
        Debug.Log($"Total time: {this.totalTime}");
        Debug.Log($"Current time: {this.currentTime}");
        Debug.Log($"Is running: {this.isRunning}");
        Debug.Log($"Is final mission: {this.isFinalMission}");
        Debug.Log("Starting 10-second test countdown...");
        
        this.StartCountdown(10f);
    }
}
