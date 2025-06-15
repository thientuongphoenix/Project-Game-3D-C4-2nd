using UnityEngine;

public class TowerManager : SaiSingleton<TowerManager>
{
    [SerializeField] protected TowerCode newTowerId = TowerCode.NoTower;

    protected virtual void LateUpdate()
    {
      this.ShowTowerToPlace();
    }

    protected virtual void ShowTowerToPlace()
    {
      this.newTowerId = this.MapKeyCodeToTowerCode(InputHotkeys.Instance.KeyCode);

      if(this.newTowerId == TowerCode.NoTower) return;

      Debug.Log("Placing tower: " + this.newTowerId.ToString());
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
