using UnityEngine;

public class ItemGuideTester : SaiMonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] protected bool testOnStart = false;
    [SerializeField] protected string testMessage = "Test: You received items! Press I key to open inventory and use items.";
    
    protected override void Start()
    {
        base.Start();
        
        if (testOnStart)
        {
            this.TestShowGuide();
        }
    }
    
    protected virtual void Update()
    {
        // Test with T key
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.TestShowGuide();
        }
        
        // Test hide UI with H key
        if (Input.GetKeyDown(KeyCode.H))
        {
            this.TestHideGuide();
        }
    }
    
    [ContextMenu("Test Show Guide")]
    public virtual void TestShowGuide()
    {
        if (ItemGuideUI.Instance != null)
        {
            ItemGuideUI.Instance.SetGuideMessage(testMessage);
            ItemGuideUI.Instance.ShowGuide();
            Debug.Log("Test: Item Guide UI shown!");
        }
        else
        {
            Debug.LogError("ItemGuideUI.Instance is null! Please setup ItemGuideUI in scene.");
        }
    }
    
    [ContextMenu("Test Hide Guide")]
    public virtual void TestHideGuide()
    {
        if (ItemGuideUI.Instance != null)
        {
            ItemGuideUI.Instance.HideGuide();
            Debug.Log("Test: Item Guide UI hidden!");
        }
        else
        {
            Debug.LogError("ItemGuideUI.Instance is null! Please setup ItemGuideUI in scene.");
        }
    }
    
    [ContextMenu("Test Item Use")]
    public virtual void TestItemUse()
    {
        if (ItemUseTracker.Instance != null)
        {
            ItemUseTracker.Instance.OnItemUsed();
            Debug.Log("Test: Item use tracked!");
        }
        else
        {
            Debug.LogError("ItemUseTracker.Instance is null! Please setup ItemUseTracker in scene.");
        }
    }
    
    [ContextMenu("Reset Guide State")]
    public virtual void TestResetGuideState()
    {
        if (ItemGuideUI.Instance != null)
        {
            ItemGuideUI.Instance.ResetGuideState();
            Debug.Log("Test: Guide state reset! Can show again.");
        }
        else
        {
            Debug.LogError("ItemGuideUI.Instance is null! Please setup ItemGuideUI in scene.");
        }
    }
    
    [ContextMenu("Check Guide Status")]
    public virtual void TestCheckGuideStatus()
    {
        if (ItemGuideUI.Instance != null)
        {
            bool hasShown = ItemGuideUI.Instance.HasShownOnce();
            bool isShowing = ItemGuideUI.Instance.IsShowing();
            Debug.Log($"Test: Guide Status - HasShownOnce: {hasShown}, IsShowing: {isShowing}");
        }
        else
        {
            Debug.LogError("ItemGuideUI.Instance is null! Please setup ItemGuideUI in scene.");
        }
    }
}
