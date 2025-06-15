using UnityEngine;

public class TowerManager : SaiSingleton<TowerManager>
{
    [SerializeField] protected TowerCode newTowerId = TowerCode.NoTower;
    [SerializeField] protected TowerCtrl towerPrefab;

    protected virtual void LateUpdate()
    {
      this.ShowTowerToPlace();
    }

    protected virtual void ShowTowerToPlace()
    {
      this.newTowerId = this.MapKeyCodeToTowerCode(InputHotkeys.Instance.KeyCode);

      if(this.newTowerId == TowerCode.NoTower) return;

      if(this.towerPrefab == null) 
      {
        this.towerPrefab = this.GetTowerPrefab(this.newTowerId);
        if(this.towerPrefab == null) return;

        // Vector3 prefabPos = PlayerCtrl.Instance.transform.position;
        // prefabPos.x += 2f;
        // this.towerPrefab.transform.position = prefabPos;
        this.towerPrefab.SetActive(true);
      }

      this.towerPrefab.transform.position = PlayerCtrl.Instance.CrosshairPointer.transform.position;

      Debug.Log("Placing tower: " + this.newTowerId.ToString());
    }

    protected virtual TowerCtrl GetTowerPrefab(TowerCode towerCode)
    {
      return TowerSpawnerCtrl.Instance.Prefabs.GetByName(towerCode.ToString());
    }

    protected virtual TowerCode MapKeyCodeToTowerCode(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Alpha1: return TowerCode.MachineGun;
            case KeyCode.Alpha2: return TowerCode.LaserGun;
            default: return TowerCode.NoTower;
        }
    }
}
