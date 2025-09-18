using UnityEngine;
using System.Collections.Generic;

public class TowerManager : SaiSingleton<TowerManager>
{
    [SerializeField] protected TowerCode newTowerId = TowerCode.NoTower;
    [SerializeField] protected TowerCtrl towerPrefab;
    [SerializeField] protected bool towerPlaced = false;
    [Header("UI")] 
    public CheckMoney checkMoneyUI;
    
    [Header("Tower Info")]
    [SerializeField] protected List<TowerInfoDataSO> towerInfoList = new List<TowerInfoDataSO>();
    [SerializeField] protected bool showTowerInfo = true;
    [SerializeField] protected TowerCode currentShowingTower = TowerCode.NoTower;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCheckMoney();
    }

    protected virtual void LoadCheckMoney()
    {
        if (this.checkMoneyUI != null) return;
        this.checkMoneyUI = GameObject.FindObjectOfType<CheckMoney>(true); // true: tìm cả object đang ẩn
        if (this.checkMoneyUI == null)
        {
            Debug.LogWarning("Không tìm thấy CheckMoney UI trong scene!");
        }
    }
    
    protected virtual void ShowTowerInfo(TowerCode towerCode)
    {
        Debug.Log($"ShowTowerInfo được gọi với TowerCode: {towerCode}");
        
        if (!this.showTowerInfo) 
        {
            Debug.Log("showTowerInfo = false, không hiển thị");
            return;
        }
        
        // Nếu bấm cùng phím số lần nữa thì ẩn UI
        if (this.currentShowingTower == towerCode)
        {
            Debug.Log($"Bấm cùng phím số {towerCode}, ẩn UI");
            this.HideTowerInfo();
            this.currentShowingTower = TowerCode.NoTower;
            return;
        }
        
        // Tìm TowerInfoDataSO tương ứng
        TowerInfoDataSO towerInfo = this.GetTowerInfoByCode(towerCode);
        if (towerInfo == null) 
        {
            Debug.LogWarning($"Không tìm thấy TowerInfoDataSO cho {towerCode}. Hãy kiểm tra towerInfoList!");
            return;
        }
        
        Debug.Log($"Tìm thấy TowerInfoDataSO: {towerInfo.towerName}");
        
        // Convert và hiển thị thông tin
        TowerInfoData data = towerInfo.ToTowerInfoData();
        Vector3 customPosition = new Vector3(142, Screen.height - 800, 0f);
        
        if (TowerInfoUI.Instance != null)
        {
            Debug.Log("Gọi TowerInfoUI.Instance.ShowTowerInfo");
            TowerInfoUI.Instance.ShowTowerInfo(data, customPosition);
            this.currentShowingTower = towerCode; // Lưu tower đang hiển thị
        }
        else
        {
            Debug.LogError("TowerInfoUI.Instance không tồn tại! Hãy đảm bảo đã setup TowerInfoUI.");
        }
    }
    
    protected virtual TowerInfoDataSO GetTowerInfoByCode(TowerCode towerCode)
    {
        Debug.Log($"Tìm kiếm TowerInfoDataSO cho {towerCode}. Số lượng trong list: {this.towerInfoList.Count}");
        
        // Debug: In ra tất cả tower trong list
        for (int i = 0; i < this.towerInfoList.Count; i++)
        {
            var tower = this.towerInfoList[i];
            if (tower != null)
                Debug.Log($"Element {i}: {tower.towerName}");
            else
                Debug.Log($"Element {i}: NULL");
        }
        
        foreach (var towerInfo in this.towerInfoList)
        {
            if (towerInfo == null) continue;
            
            Debug.Log($"Kiểm tra: '{towerInfo.towerName}' vs '{towerCode}'");
            
            // Tìm kiếm chính xác hơn
            string towerNameLower = towerInfo.towerName.ToLower();
            string towerCodeLower = towerCode.ToString().ToLower();
            
            Debug.Log($"So sánh: '{towerNameLower}' có chứa '{towerCodeLower}' không?");
            
            if (towerNameLower.Contains(towerCodeLower))
            {
                Debug.Log($"Tìm thấy match: {towerInfo.towerName}");
                return towerInfo;
            }
            
            // Thử tìm kiếm với từ khóa rút gọn (loại bỏ space)
            string simplifiedCode = towerCodeLower.Replace("tower", "").Replace("trap", "");
            string simplifiedName = towerNameLower.Replace(" ", "").Replace("-", "");
            
            Debug.Log($"Thử tìm kiếm với từ khóa rút gọn: '{simplifiedCode}' vs '{simplifiedName}'");
            
            if (simplifiedName.Contains(simplifiedCode))
            {
                Debug.Log($"Tìm thấy match với từ khóa rút gọn: {towerInfo.towerName}");
                return towerInfo;
            }
            
            // Thử tìm kiếm với từ khóa chính
            if (simplifiedName.Contains("machinegun") && towerCodeLower.Contains("machinegun"))
            {
                Debug.Log($"Tìm thấy match MachineGun: {towerInfo.towerName}");
                return towerInfo;
            }
            
            if (simplifiedName.Contains("onegunbarrel") && towerCodeLower.Contains("onegunbarrel"))
            {
                Debug.Log($"Tìm thấy match OneGunBarrel: {towerInfo.towerName}");
                return towerInfo;
            }
            
            if (simplifiedName.Contains("icetrap") && towerCodeLower.Contains("icetrap"))
            {
                Debug.Log($"Tìm thấy match IceTrap: {towerInfo.towerName}");
                return towerInfo;
            }
            
            if (simplifiedName.Contains("flametrap") && towerCodeLower.Contains("flametrap"))
            {
                Debug.Log($"Tìm thấy match FlameTrap: {towerInfo.towerName}");
                return towerInfo;
            }
            
            if (simplifiedName.Contains("core") && towerCodeLower.Contains("core"))
            {
                Debug.Log($"Tìm thấy match Core: {towerInfo.towerName}");
                return towerInfo;
            }
        }
        
        Debug.LogWarning($"Không tìm thấy TowerInfoDataSO nào cho {towerCode}");
        return null;
    }
    
    protected virtual void HideTowerInfo()
    {
        if (TowerInfoUI.Instance != null)
        {
            TowerInfoUI.Instance.HideTowerInfo();
        }
    }

    protected virtual void Update()
    {
      this.ShowTowerToPlace();
    }

    protected virtual void ShowTowerToPlace()
    {
      if(this.towerPlaced) return;

      TowerCode newTowerId = this.MapKeyCodeToTowerCode(InputHotkeys.Instance.KeyCode);

      if(newTowerId == TowerCode.NoTower) 
      {
        if(this.towerPrefab != null) this.towerPrefab.SetActive(false);
        this.towerPrefab = null;
        this.newTowerId = TowerCode.NoTower;
        
        // Ẩn UI thông tin tower khi không có tower nào được chọn
        if (this.currentShowingTower != TowerCode.NoTower)
        {
            this.HideTowerInfo();
            this.currentShowingTower = TowerCode.NoTower;
        }
        return;
      }
      
      // Chỉ hiển thị thông tin tower khi thay đổi selection
      if (this.newTowerId != newTowerId)
      {
          // Hiển thị thông tin tower mới (không ẩn tower cũ)
          this.ShowTowerInfo(newTowerId);
          this.newTowerId = newTowerId;
          
          // Cập nhật towerPrefab khi thay đổi selection
          if (this.towerPrefab != null)
          {
              this.towerPrefab.SetActive(false);
              this.towerPrefab = null;
          }
      }

      if(this.towerPrefab == null) 
      {
        this.towerPrefab = this.GetTowerPrefab(this.newTowerId);
        if(this.towerPrefab == null) return;

        if (this.towerPrefab.TowerType == TowerType.Tower && this.towerPrefab.TowerShooting != null)
            this.towerPrefab.TowerShooting.Disable();
        this.towerPrefab.SetActive(true);
      }

      this.towerPrefab.transform.position = PlayerCtrl.Instance.CrosshairPointer.transform.position;

      if(InputHotkeys.Instance.IsPlaceTower)
      {
        this.PlaceTower();
      }
    }

    protected virtual void PlaceTower()
    {
        // --- Kiểm tra cooldown ---
        if (!this.towerPrefab.IsCooldownReady())
        {
            Debug.LogWarning($"Tower {this.towerPrefab.name} đang cooldown, vui lòng chờ {Mathf.Ceil(this.towerPrefab.CooldownTime - (Time.time - this.towerPrefab.lastPlacedTime))} giây nữa!");
            // Nếu muốn hiện UI thông báo cooldown thì thêm ở đây
            return;
        }
        // Kiểm tra và trừ tiền trước khi đặt
        int price = this.towerPrefab.price;
        var gold = InventoryManager.Instance.Monies().FindItem(ItemCode.Gold);
        if (gold == null || gold.itemCount < price)
        {
            //Debug.LogWarning($"Không đủ tiền để đặt {this.towerPrefab.name}, cần {price} vàng!");
            // Hiện UI thông báo ở đây
            if (this.checkMoneyUI != null) this.checkMoneyUI.ShowNotEnoughMoney();
            return;
        }
        InventoryManager.Instance.RemoveItem(ItemCode.Gold, price);
        // ---
        this.towerPlaced = true;

        TowerCtrl newTower = this.Spawn(this.towerPrefab);
        if (newTower.TowerType == TowerType.Tower && newTower.TowerShooting != null)
        {
            newTower.TowerShooting.ResetShootingState();
            newTower.TowerShooting.Active();
        }
        newTower.SetActive(true);
        if (newTower.Level != null) newTower.Level.ResetLevel();
        
        // --- Cập nhật thời gian đặt gần nhất để cooldown ---
        this.towerPrefab.SetLastPlacedTime();

        // --- Hiển thị cooldown trên thanh bar ---
        TowerbarUIManager.Instance?.StartCooldownFor(this.newTowerId, this.towerPrefab.CooldownTime);
        
        // --- Thông báo cho hệ thống nhiệm vụ ---
        if (TowerQuestSystem.Instance != null)
        {
            TowerQuestSystem.Instance.OnTowerPlaced(this.newTowerId);
        }
        
        // --- Không ẩn thông tin tower khi đặt (để Bệ Hạ có thể xem thông tin) ---
        // this.HideTowerInfo();
        // this.currentShowingTower = TowerCode.NoTower;
        
        //this.towerPrefab.SetActive(false);
        // this.newTowerId = TowerCode.NoTower;
        // this.towerPrefab = null;

        Invoke(nameof(this.PlaceFinish), 0.5f);
    }

    protected virtual void PlaceFinish()
    {
      this.towerPlaced = false;
    }

    protected virtual TowerCtrl GetTowerPrefab(TowerCode towerCode)
    {
      return TowerSpawnerCtrl.Instance.Prefabs.GetByName(towerCode.ToString());
    }

    protected virtual TowerCtrl Spawn(TowerCtrl prefab)
    {
      return TowerSpawnerCtrl.Instance.Spawner.Spawn(prefab, prefab.transform.position);
    }

    protected virtual TowerCode MapKeyCodeToTowerCode(KeyCode keyCode)
    {
        TowerCode towerCode = TowerCode.NoTower;
        
        switch (keyCode)
        {
            case KeyCode.Alpha1: 
                towerCode = TowerCode.MachineGun;
                break;
            case KeyCode.Alpha2: 
                towerCode = TowerCode.OneGunBarrel;
                break;
            case KeyCode.Alpha3: 
                towerCode = TowerCode.IceTrap;
                break;
            case KeyCode.Alpha4: 
                towerCode = TowerCode.FlameTrap;
                break;
            case KeyCode.Alpha5: 
                towerCode = TowerCode.Core;
                break;
            default: 
                return TowerCode.NoTower;
        }
        
        // Kiểm tra xem tower có được mở khóa chưa (chỉ kiểm tra các tower cần quest)
        if (TowerQuestSystem.Instance != null && this.IsTowerRequiresQuest(towerCode))
        {
            bool isUnlocked = TowerQuestSystem.Instance.IsTowerUnlocked(towerCode);
            int totalPlaced = TowerQuestSystem.Instance.GetTotalTowersPlaced();
            
            Debug.Log($"DEBUG: {towerCode} - IsUnlocked: {isUnlocked}, TotalPlaced: {totalPlaced}");
            
            if (!isUnlocked)
            {
                Debug.Log($"{towerCode} Tower chưa được mở khóa! Hãy hoàn thành nhiệm vụ trước.");
                
                // Hiển thị thông báo UI
                if (TowerLockedNotifier.Instance != null)
                {
                    TowerLockedNotifier.Instance.ShowTowerLockedNotification(towerCode);
                }
                
                return TowerCode.NoTower;
            }
            else
            {
                Debug.Log($"{towerCode} Tower đã được mở khóa! Cho phép sử dụng.");
            }
        }
        
        return towerCode;
    }
    
    protected virtual bool IsTowerRequiresQuest(TowerCode towerCode)
    {
        // Chỉ các tower này cần quest để mở khóa
        switch (towerCode)
        {
            case TowerCode.OneGunBarrel:  // Cần quest "Tower Builder I"
            case TowerCode.IceTrap:        // Cần quest "Tower Builder II"
            case TowerCode.FlameTrap:      // Có thể cần quest trong tương lai
                return true;
            
            case TowerCode.MachineGun:     // Luôn mở khóa từ đầu
            case TowerCode.Core:           // Luôn mở khóa từ đầu
            default:
                return false;
        }
    }
}
