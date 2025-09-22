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
    
    [Header("Tower Placement")]
    [SerializeField] protected float minTowerDistance = 3f; // Khoảng cách tối thiểu giữa các tower
    [SerializeField] protected float maxPlayerDistance = 10f; // Khoảng cách tối đa từ người chơi
    [SerializeField] protected bool canPlaceTower = true; // Có thể đặt tower không
    [SerializeField] protected LayerMask groundLayerMask = 1; // Layer mask cho mặt đất
    [SerializeField] protected float raycastDistance = 10f; // Khoảng cách raycast
    [SerializeField] protected bool showPlayerRange = true; // Hiển thị vòng tròn giới hạn khoảng cách
    [SerializeField] protected bool isOutOfRange = false; // Tower prefab có vượt quá tầm không

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
    
    // --- Hàm kiểm tra vị trí có phải là mặt đất không ---
    protected virtual bool IsOnGround(Vector3 position)
    {
        // Raycast từ vị trí xuống dưới để kiểm tra mặt đất
        Ray ray = new Ray(position + Vector3.up * 0.1f, Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, this.raycastDistance, this.groundLayerMask))
        {
            Debug.Log($"Tìm thấy mặt đất tại: {hit.point}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            return true;
        }
        
        Debug.Log($"Không tìm thấy mặt đất tại vị trí: {position}");
        return false;
    }
    
    // --- Hàm kiểm tra khoảng cách từ người chơi ---
    protected virtual bool IsWithinPlayerRange(Vector3 position)
    {
        if (PlayerCtrl.Instance == null)
        {
            Debug.LogWarning("PlayerCtrl.Instance không tồn tại!");
            return false;
        }
        
        float distance = Vector3.Distance(position, PlayerCtrl.Instance.transform.position);
        
        if (distance > this.maxPlayerDistance)
        {
            Debug.Log($"Vị trí quá xa người chơi! Khoảng cách: {distance:F2}m (tối đa: {this.maxPlayerDistance}m)");
            return false;
        }
        
        return true;
    }
    
    // --- Hàm lấy vị trí chính xác trên mặt đất ---
    protected virtual Vector3 GetGroundPosition(Vector3 position)
    {
        // Raycast từ vị trí xuống dưới để tìm mặt đất
        Ray ray = new Ray(position + Vector3.up * 0.1f, Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, this.raycastDistance, this.groundLayerMask))
        {
            Debug.Log($"Đặt tower tại vị trí mặt đất: {hit.point}");
            return hit.point;
        }
        
        // Nếu không tìm thấy mặt đất, trả về vị trí gốc
        Debug.LogWarning("Không tìm thấy mặt đất, sử dụng vị trí gốc");
        return position;
    }
    
    // --- Hàm kiểm tra vị trí đặt tower có hợp lệ không ---
    protected virtual bool IsValidTowerPosition(Vector3 newPosition)
    {
        // Kiểm tra xem vị trí có phải là mặt đất không
        if (!this.IsOnGround(newPosition))
        {
            Debug.Log("Vị trí không hợp lệ! Chỉ được đặt tower trên mặt đất!");
            return false;
        }
        
        // Kiểm tra khoảng cách từ người chơi
        if (!this.IsWithinPlayerRange(newPosition))
        {
            Debug.Log("Vị trí không hợp lệ! Quá xa người chơi!");
            return false;
        }
        
        // Tìm tất cả tower đã đặt trong scene
        TowerCtrl[] existingTowers = FindObjectsOfType<TowerCtrl>();
        
        foreach (TowerCtrl tower in existingTowers)
        {
            if (tower == null || tower == this.towerPrefab) continue; // Bỏ qua tower prefab đang di chuyển
            
            float distance = Vector3.Distance(newPosition, tower.transform.position);
            
            if (distance < this.minTowerDistance)
            {
                Debug.Log($"Vị trí không hợp lệ! Khoảng cách đến {tower.name}: {distance:F2}m (cần tối thiểu {this.minTowerDistance}m)");
                return false;
            }
        }
        
        return true;
    }
    
    // --- Hàm cập nhật trạng thái có thể đặt tower ---
    protected virtual void UpdateTowerPlacementStatus()
    {
        if (this.towerPrefab == null) 
        {
            this.canPlaceTower = true;
            this.isOutOfRange = false;
            return;
        }
        
        // Nếu vượt quá tầm, không thể đặt tower
        if (this.isOutOfRange)
        {
            this.canPlaceTower = false;
        }
        else
        {
            Vector3 currentPosition = this.towerPrefab.transform.position;
            this.canPlaceTower = this.IsValidTowerPosition(currentPosition);
        }
        
        // Cập nhật màu sắc tower prefab để phản hồi trực quan
        this.UpdateTowerPrefabVisual();
    }
    
    // --- Hàm cập nhật màu sắc tower prefab ---
    protected virtual void UpdateTowerPrefabVisual()
    {
        if (this.towerPrefab == null) return;
        
        // Tìm tất cả renderer trong tower prefab
        Renderer[] renderers = this.towerPrefab.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
            {
                // Xác định màu sắc dựa trên trạng thái
                Color targetColor;
                if (this.isOutOfRange)
                {
                    targetColor = Color.yellow; // Màu vàng khi vượt quá tầm
                }
                else if (this.canPlaceTower)
                {
                    targetColor = Color.green; // Màu xanh khi có thể đặt
                }
                else
                {
                    targetColor = Color.red; // Màu đỏ khi không thể đặt
                }
                
                // Kiểm tra xem material có hỗ trợ thay đổi màu sắc không
                if (renderer.material.HasProperty("_Color"))
                {
                    // Chỉ thay đổi màu nếu khác với màu hiện tại
                    if (renderer.material.color != targetColor)
                    {
                        renderer.material.color = targetColor;
                    }
                }
                // Nếu không có _Color property, thử sử dụng _TintColor (cho một số shader khác)
                else if (renderer.material.HasProperty("_TintColor"))
                {
                    renderer.material.SetColor("_TintColor", targetColor);
                }
                // Nếu không có cả hai, bỏ qua material này
                else
                {
                    // Debug.Log($"Material {renderer.material.name} không hỗ trợ thay đổi màu sắc");
                }
            }
        }
    }
    
    // --- Hàm reset màu sắc tower prefab về màu gốc ---
    protected virtual void ResetTowerPrefabVisual()
    {
        if (this.towerPrefab == null) return;
        
        // Tìm tất cả renderer trong tower prefab
        Renderer[] renderers = this.towerPrefab.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
            {
                // Kiểm tra xem material có hỗ trợ thay đổi màu sắc không
                if (renderer.material.HasProperty("_Color"))
                {
                    // Reset về màu trắng (màu gốc)
                    renderer.material.color = Color.white;
                }
                // Nếu không có _Color property, thử sử dụng _TintColor
                else if (renderer.material.HasProperty("_TintColor"))
                {
                    renderer.material.SetColor("_TintColor", Color.white);
                }
                // Nếu không có cả hai, bỏ qua material này
            }
        }
    }

    protected virtual void Update()
    {
      this.ShowTowerToPlace();
    }
    
    // --- Hàm vẽ vòng tròn giới hạn khoảng cách từ người chơi ---
    protected virtual void OnDrawGizmos()
    {
        if (!this.showPlayerRange || PlayerCtrl.Instance == null) return;
        
        // Vẽ vòng tròn giới hạn khoảng cách (sử dụng DrawWireSphere)
        Gizmos.color = Color.yellow;
        Vector3 playerPos = PlayerCtrl.Instance.transform.position;
        Gizmos.DrawWireSphere(playerPos, this.maxPlayerDistance);
        
        // Vẽ vòng tròn khoảng cách tối thiểu giữa các tower (nếu có tower prefab)
        if (this.towerPrefab != null)
        {
            Gizmos.color = Color.red;
            Vector3 towerPos = this.towerPrefab.transform.position;
            Gizmos.DrawWireSphere(towerPos, this.minTowerDistance);
        }
    }

    protected virtual void ShowTowerToPlace()
    {
      if(this.towerPlaced) return;

      TowerCode newTowerId = this.MapKeyCodeToTowerCode(InputHotkeys.Instance.KeyCode);

      if(newTowerId == TowerCode.NoTower) 
      {
        if(this.towerPrefab != null) 
        {
            // Reset màu sắc trước khi ẩn
            this.ResetTowerPrefabVisual();
            this.towerPrefab.SetActive(false);
        }
        this.towerPrefab = null;
        this.newTowerId = TowerCode.NoTower;
        this.canPlaceTower = true;
        this.isOutOfRange = false;
        
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
          // Reset màu sắc tower cũ trước khi thay đổi
          if (this.towerPrefab != null)
          {
              this.ResetTowerPrefabVisual();
              this.towerPrefab.SetActive(false);
              this.towerPrefab = null;
          }
          
          // Hiển thị thông tin tower mới (không ẩn tower cũ)
          this.ShowTowerInfo(newTowerId);
          this.newTowerId = newTowerId;
      }

      if(this.towerPrefab == null) 
      {
        this.towerPrefab = this.GetTowerPrefab(this.newTowerId);
        if(this.towerPrefab == null) return;

        if (this.towerPrefab.TowerType == TowerType.Tower && this.towerPrefab.TowerShooting != null)
            this.towerPrefab.TowerShooting.Disable();
        this.towerPrefab.SetActive(true);
      }

      // Lấy vị trí từ crosshair và kiểm tra phạm vi
      Vector3 crosshairPosition = PlayerCtrl.Instance.CrosshairPointer.transform.position;
      Vector3 playerPosition = PlayerCtrl.Instance.transform.position;
      
      // Kiểm tra khoảng cách từ người chơi đến crosshair
      float distanceToCrosshair = Vector3.Distance(crosshairPosition, playerPosition);
      
      if (distanceToCrosshair > this.maxPlayerDistance)
      {
        // Nếu quá xa, đặt tower prefab ở vị trí giới hạn
        Vector3 direction = (crosshairPosition - playerPosition).normalized;
        Vector3 limitedPosition = playerPosition + direction * this.maxPlayerDistance;
        Vector3 groundPosition = this.GetGroundPosition(limitedPosition);
        this.towerPrefab.transform.position = groundPosition;
        this.isOutOfRange = true; // Đánh dấu vượt quá tầm
      }
      else
      {
        // Nếu trong phạm vi, đặt bình thường
        Vector3 groundPosition = this.GetGroundPosition(crosshairPosition);
        this.towerPrefab.transform.position = groundPosition;
        this.isOutOfRange = false; // Đánh dấu trong tầm
      }
      
      // Cập nhật trạng thái có thể đặt tower dựa trên vị trí hiện tại
      this.UpdateTowerPlacementStatus();

      if(InputHotkeys.Instance.IsPlaceTower)
      {
        this.PlaceTower();
      }
    }

    protected virtual void PlaceTower()
    {
        // --- Kiểm tra vị trí đặt tower ---
        if (!this.canPlaceTower)
        {
            // Kiểm tra cụ thể lý do không thể đặt
            if (this.isOutOfRange)
            {
                Debug.LogWarning($"Không thể đặt tower! Vượt quá tầm cho phép (tối đa {this.maxPlayerDistance}m)!");
            }
            else
            {
                Vector3 currentPosition = this.towerPrefab.transform.position;
                
                if (!this.IsOnGround(currentPosition))
                {
                    Debug.LogWarning("Không thể đặt tower! Chỉ được đặt trên mặt đất!");
                }
                else if (!this.IsWithinPlayerRange(currentPosition))
                {
                    Debug.LogWarning($"Không thể đặt tower! Quá xa người chơi (tối đa {this.maxPlayerDistance}m)!");
                }
                else
                {
                    Debug.LogWarning($"Không thể đặt tower! Vị trí quá gần với tower khác (cần tối thiểu {this.minTowerDistance}m)");
                }
            }
            
            // Hiện UI thông báo không thể đặt
            if (this.checkMoneyUI != null) 
            {
                if (this.isOutOfRange)
                {
                    // Hiển thị thông báo quá tầm thay vì không đủ tiền
                    this.checkMoneyUI.ShowOutOfRangeMessage();
                }
                else
                {
                    // Hiển thị thông báo vị trí không hợp lệ
                    this.checkMoneyUI.ShowInvalidPositionMessage();
                }
            }
            return;
        }
        
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

        // Tìm vị trí chính xác trên mặt đất
        Vector3 groundPosition = this.GetGroundPosition(this.towerPrefab.transform.position);
        
        TowerCtrl newTower = this.Spawn(this.towerPrefab);
        newTower.transform.position = groundPosition;
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
        
        // --- Reset màu sắc tower prefab sau khi đặt thành công ---
        this.ResetTowerPrefabVisual();
        
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
            case TowerCode.MachineGun:     // Cần quest "Movement Tutorial"
            case TowerCode.OneGunBarrel:  // Cần quest "Tower Builder I"
            case TowerCode.IceTrap:        // Cần quest "Tower Builder II"
            case TowerCode.FlameTrap:      // Có thể cần quest trong tương lai
                return true;
            
            case TowerCode.Core:           // Luôn mở khóa từ đầu
            default:
                return false;
        }
    }
}
