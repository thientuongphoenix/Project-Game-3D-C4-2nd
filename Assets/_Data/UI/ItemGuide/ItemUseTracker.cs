using UnityEngine;

public class ItemUseTracker : SaiSingleton<ItemUseTracker>
{
    protected bool itemWasUsed = false;
    
    protected override void Start()
    {
        base.Start();
        this.ResetTracker();
    }
    
    public virtual void OnItemUsed()
    {
        this.itemWasUsed = true;
        Debug.Log("Item was used! Hiding guide UI...");
        
        // Hide guide UI when player uses item
        if (ItemGuideUI.Instance != null)
        {
            ItemGuideUI.Instance.HideGuide();
        }
    }
    
    public virtual void ResetTracker()
    {
        this.itemWasUsed = false;
    }
    
    public virtual bool WasItemUsed()
    {
        return this.itemWasUsed;
    }
}
