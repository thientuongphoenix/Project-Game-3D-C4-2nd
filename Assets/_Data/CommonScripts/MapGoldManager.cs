using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGoldManager : SaiSingleton<MapGoldManager>
{
    [Header("Starting Gold for Each Map")]
    [SerializeField] protected int haiSampleSceneGold = 300; // Vàng cho Hai_SampleScene
    [SerializeField] protected int haiMapGold = 500; // Vàng cho Hai_Map
    [SerializeField] protected int defaultGold = 100; // Vàng mặc định cho map khác
    
    [Header("Settings")]
    [SerializeField] protected bool resetGoldOnNewGame = true; // Reset vàng khi chơi mới
    [SerializeField] protected bool addGoldIfEmpty = true; // Chỉ thêm vàng nếu chưa có
    
    protected override void Start()
    {
        base.Start();
        this.InitializeGoldForCurrentMap();
    }
    
    protected virtual void InitializeGoldForCurrentMap()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int goldToAdd = this.GetGoldAmountForScene(currentSceneName);
        
        // Kiểm tra xem đã có vàng chưa
        if (addGoldIfEmpty)
        {
            var existingGold = InventoryManager.Instance.Monies().FindItem(ItemCode.Gold);
            if (existingGold != null && existingGold.itemCount > 0)
            {
                Debug.Log($"MapGoldManager: Player already has {existingGold.itemCount} gold, no gold added");
                return;
            }
        }
        
        // Thêm vàng cho map hiện tại
        InventoryManager.Instance.AddItem(ItemCode.Gold, goldToAdd);
        Debug.Log($"MapGoldManager: Added {goldToAdd} gold for scene: {currentSceneName}");
    }
    
    protected virtual int GetGoldAmountForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Hai_SampleScene":
                return haiSampleSceneGold;
            case "Hai_Map":
                return haiMapGold;
            default:
                return defaultGold;
        }
    }
    
    // Method để thêm vàng bổ sung (có thể gọi từ UI hoặc events)
    public virtual void AddBonusGold(int amount)
    {
        if (amount > 0)
        {
            InventoryManager.Instance.AddItem(ItemCode.Gold, amount);
            Debug.Log($"MapGoldManager: Added {amount} bonus gold");
        }
    }
    
    // Method để set vàng cho map cụ thể
    public virtual void SetGoldForScene(string sceneName, int goldAmount)
    {
        switch (sceneName)
        {
            case "Hai_SampleScene":
                haiSampleSceneGold = goldAmount;
                break;
            case "Hai_Map":
                haiMapGold = goldAmount;
                break;
            default:
                defaultGold = goldAmount;
                break;
        }
        
        Debug.Log($"MapGoldManager: Set {goldAmount} gold for scene: {sceneName}");
    }
    
    // Method để reset vàng về giá trị ban đầu
    public virtual void ResetGoldToStarting()
    {
        // Xóa tất cả vàng hiện tại
        var existingGold = InventoryManager.Instance.Monies().FindItem(ItemCode.Gold);
        if (existingGold != null)
        {
            InventoryManager.Instance.RemoveItem(ItemCode.Gold, existingGold.itemCount);
        }
        
        // Thêm lại vàng ban đầu cho map hiện tại
        string currentSceneName = SceneManager.GetActiveScene().name;
        int goldToAdd = this.GetGoldAmountForScene(currentSceneName);
        InventoryManager.Instance.AddItem(ItemCode.Gold, goldToAdd);
        
        Debug.Log($"MapGoldManager: Reset to {goldToAdd} starting gold for scene: {currentSceneName}");
    }
    
    // Method để lấy số vàng hiện tại
    public virtual int GetCurrentGold()
    {
        var gold = InventoryManager.Instance.Monies().FindItem(ItemCode.Gold);
        return gold != null ? gold.itemCount : 0;
    }
    
    // Method để kiểm tra đủ vàng không
    public virtual bool HasEnoughGold(int requiredAmount)
    {
        return this.GetCurrentGold() >= requiredAmount;
    }
}
