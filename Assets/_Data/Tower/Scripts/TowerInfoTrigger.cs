using UnityEngine;
using UnityEngine.EventSystems;

public class TowerInfoTrigger : MonoBehaviour, IPointerClickHandler
{
    [Header("Tower Info")]
    [SerializeField] protected TowerInfoData towerInfo;
    [SerializeField] protected TowerInfoDataSO towerInfoSO;
    [SerializeField] protected TowerCtrl towerCtrl;
    
    [Header("Auto Setup")]
    [SerializeField] protected bool autoCreateInfo = true;
    [SerializeField] protected string customTowerName = "";
    [SerializeField] protected int customPrice = 100;
    
    [Header("Trigger Settings")]
    [SerializeField] protected bool showOnClick = true;
    
    protected virtual void Start()
    {
        this.LoadTowerCtrl();
        this.LoadTowerInfo();
    }
    

    
    protected virtual void LoadTowerCtrl()
    {
        if (this.towerCtrl != null) return;
        this.towerCtrl = GetComponent<TowerCtrl>();
        Debug.Log(transform.name + ": LoadTowerCtrl", gameObject);
    }
    
    protected virtual void LoadTowerInfo()
    {
        // Priority: TowerInfoDataSO > TowerInfoData > Auto Create
        if (this.towerInfoSO != null)
        {
            this.towerInfo = this.towerInfoSO.ToTowerInfoData();
            return;
        }
        
        if (this.towerInfo != null) return;
        
        // Try to get TowerInfoData from TowerCtrl or create default
        if (this.towerCtrl != null && this.autoCreateInfo)
        {
            this.towerInfo = this.CreateDefaultTowerInfo();
        }
        
        Debug.Log(transform.name + ": LoadTowerInfo", gameObject);
    }
    
    protected virtual TowerInfoData CreateDefaultTowerInfo()
    {
        if (this.towerCtrl != null)
        {
            string towerName = string.IsNullOrEmpty(this.customTowerName) ? this.towerCtrl.name : this.customTowerName;
            int price = this.customPrice > 0 ? this.customPrice : this.towerCtrl.price;
            
            return TowerInfoData.CreateDefault(towerName, this.towerCtrl.TowerType, price);
        }
        
        // Fallback if no TowerCtrl
        return TowerInfoData.CreateDefault("Unknown Tower", TowerType.Tower, 100);
    }
    

    
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!this.showOnClick) return;
        
        this.ShowTowerInfo();
    }
    
    protected virtual void ShowTowerInfo()
    {
        if (this.towerInfo == null) return;
        
        Vector3 worldPosition = this.transform.position;
        TowerInfoUI.Instance.ShowTowerInfo(this.towerInfo, worldPosition);
    }
    
    protected virtual void HideTowerInfo()
    {
        TowerInfoUI.Instance.HideTowerInfo();
    }
    
    public virtual void SetTowerInfo(TowerInfoData newTowerInfo)
    {
        this.towerInfo = newTowerInfo;
    }
}
