using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemGuideUI : SaiSingleton<ItemGuideUI>
{
    [Header("UI Components")]
    [SerializeField] protected GameObject guidePanel;
    [SerializeField] protected TextMeshProUGUI guideText;
    [SerializeField] protected Button closeButton;
    
    [Header("Guide Settings")]
    [SerializeField] protected float autoHideDelay = 10f; // Auto hide after 10 seconds
    [SerializeField] protected string guideMessage = "You received items! Press I key to open inventory and use items.";
    
    protected bool isShowing = false;
    protected float showTime = 0f;
    protected static bool hasShownOnce = false; // Track if guide has been shown once
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadGuidePanel();
        this.LoadGuideText();
        this.LoadCloseButton();
    }
    
    protected virtual void LoadGuidePanel()
    {
        if (this.guidePanel != null) return;
        this.guidePanel = transform.Find("GuidePanel")?.gameObject;
        if (this.guidePanel == null)
        {
            Debug.LogWarning("GuidePanel not found! Please create a UI panel named 'GuidePanel'");
        }
        Debug.Log(transform.name + ": LoadGuidePanel", gameObject);
    }
    
    protected virtual void LoadGuideText()
    {
        if (this.guideText != null) return;
        this.guideText = transform.Find("GuidePanel/GuideText")?.GetComponent<TextMeshProUGUI>();
        if (this.guideText == null)
        {
            Debug.LogWarning("GuideText not found! Please create a TextMeshPro - Text (UI) component named 'GuideText'");
        }
        Debug.Log(transform.name + ": LoadGuideText", gameObject);
    }
    
    protected virtual void LoadCloseButton()
    {
        if (this.closeButton != null) return;
        this.closeButton = transform.Find("GuidePanel/CloseButton")?.GetComponent<Button>();
        if (this.closeButton != null)
        {
            this.closeButton.onClick.AddListener(this.HideGuide);
        }
        else
        {
            Debug.LogWarning("CloseButton not found! Please create a Button component named 'CloseButton'");
        }
        Debug.Log(transform.name + ": LoadCloseButton", gameObject);
    }
    
    protected virtual void Start()
    {
        base.Start();
        this.HideGuide();
    }
    
    protected virtual void Update()
    {
        if (this.isShowing)
        {
            this.showTime += Time.deltaTime;
            
            // Auto hide after a period of time
            if (this.showTime >= this.autoHideDelay)
            {
                this.HideGuide();
            }
            
            // Hide when player presses I key (open inventory)
            if (InputHotkeys.Instance != null && InputHotkeys.Instance.IsToggleInventoryUI)
            {
                this.HideGuide();
            }
        }
    }
    
    public virtual void ShowGuide()
    {
        // Check if guide has already been shown once
        if (hasShownOnce)
        {
            Debug.Log("Item Guide has already been shown once. Skipping...");
            return;
        }
        
        if (this.guidePanel == null)
        {
            Debug.LogError("GuidePanel is null! Cannot show guide.");
            return;
        }
        
        this.isShowing = true;
        this.showTime = 0f;
        this.guidePanel.SetActive(true);
        
        if (this.guideText != null)
        {
            this.guideText.text = this.guideMessage;
        }
        
        // Mark as shown once
        hasShownOnce = true;
        
        Debug.Log("Item Guide UI shown for the first time!");
    }
    
    public virtual void HideGuide()
    {
        if (this.guidePanel == null) return;
        
        this.isShowing = false;
        this.showTime = 0f;
        this.guidePanel.SetActive(false);
        
        Debug.Log("Item Guide UI hidden!");
    }
    
    public virtual void SetGuideMessage(string message)
    {
        this.guideMessage = message;
        if (this.guideText != null && this.isShowing)
        {
            this.guideText.text = this.guideMessage;
        }
    }
    
    public virtual bool IsShowing()
    {
        return this.isShowing;
    }
    
    /// <summary>
    /// Reset the guide state to allow showing again (useful for new game)
    /// </summary>
    public virtual void ResetGuideState()
    {
        hasShownOnce = false;
        this.isShowing = false;
        this.showTime = 0f;
        Debug.Log("Item Guide state reset! Can be shown again.");
    }
    
    /// <summary>
    /// Check if guide has been shown once
    /// </summary>
    public virtual bool HasShownOnce()
    {
        return hasShownOnce;
    }
}
