using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGoldManager : SaiMonoBehaviour
{
    [Header("Cấu hình vàng cho từng map")]
    [SerializeField] protected int haiMapGold = 120;
    [SerializeField] protected int haiSampleSceneGold = 1000;
    [SerializeField] protected int defaultGold = 100;
    
    protected override void Start()
    {
        base.Start();
        this.SetGoldForCurrentMap();
    }
    
    protected virtual void SetGoldForCurrentMap()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int goldAmount = this.GetGoldForScene(currentSceneName);
        
        Debug.Log($"Map: {currentSceneName} - Thêm {goldAmount} vàng");
        InventoryManager.Instance.AddItem(ItemCode.Gold, goldAmount);
    }
    
    protected virtual int GetGoldForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Hai_Map":
                return this.haiMapGold;
            case "Hai_SampleScene":
                return this.haiSampleSceneGold;
            default:
                Debug.LogWarning($"Không tìm thấy cấu hình vàng cho map: {sceneName}, sử dụng giá trị mặc định: {this.defaultGold}");
                return this.defaultGold;
        }
    }
    
    [ContextMenu("Test Add Gold")]
    protected virtual void TestAddGold()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int goldAmount = this.GetGoldForScene(currentSceneName);
        InventoryManager.Instance.AddItem(ItemCode.Gold, goldAmount);
        Debug.Log($"Đã thêm {goldAmount} vàng cho map {currentSceneName}");
    }
}
