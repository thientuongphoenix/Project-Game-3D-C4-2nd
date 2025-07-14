using UnityEngine;

public class TowerManager : SaiSingleton<TowerManager>
{
    [SerializeField] protected TowerCode newTowerId = TowerCode.NoTower;
    [SerializeField] protected TowerCtrl towerPrefab;
    [SerializeField] protected bool towerPlaced = false;
    [Header("UI")] public CheckMoney checkMoneyUI;

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

    protected virtual void Update()
    {
      this.ShowTowerToPlace();
    }

    protected virtual void ShowTowerToPlace()
    {
      if(this.towerPlaced) return;

      this.newTowerId = this.MapKeyCodeToTowerCode(InputHotkeys.Instance.KeyCode);

      if(this.newTowerId == TowerCode.NoTower) 
      {
        if(this.towerPrefab != null) this.towerPrefab.SetActive(false);
        this.towerPrefab = null;
        return;
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
        switch (keyCode)
        {
            case KeyCode.Alpha1: return TowerCode.MachineGun;
            case KeyCode.Alpha2: return TowerCode.OneGunBarrel;
            case KeyCode.Alpha3: return TowerCode.IceTrap;
            case KeyCode.Alpha4: return TowerCode.FlameTrap;
            default: return TowerCode.NoTower;
        }
    }
}
