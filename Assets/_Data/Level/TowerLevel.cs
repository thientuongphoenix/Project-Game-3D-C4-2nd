using UnityEngine;

public class TowerLevel : LevelAbstract
{
    [SerializeField] protected TowerCtrl towerCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadTowerCtrl();
    }

    protected virtual void LoadTowerCtrl()
    {
        if (this.towerCtrl != null) return;
        this.towerCtrl = GetComponentInParent<TowerCtrl>();
        Debug.Log(transform.name + ": LoadTowerCtrl", gameObject);
    }

    protected override bool DeductExp(int exp)
    {
        return this.towerCtrl.TowerShooting.DeductKillCount(exp);
    }

    protected override int GetCurrentExp()
    {
        return this.towerCtrl.TowerShooting.KillCount;
    }
    
    protected override int GetNextLevelExp()
    {
        return this.nextLevelExp = this.currentLevel * 2;
    }

    protected override void Leveling()
    {
        if (this.towerCtrl != null && !this.towerCtrl.CanLevelUp())
        {
            //this.currentLevel = 1;
            return;
        } 
        if (this.currentLevel >= this.maxLevel) return;
        if (this.GetCurrentExp() < this.GetNextLevelExp()) return;
        if (!this.DeductExp(this.GetNextLevelExp())) return;
        
        // Lưu level cũ để kiểm tra
        int oldLevel = this.currentLevel;
        this.currentLevel++;
        
        // Kiểm tra nếu vừa lên cấp
        if (this.currentLevel > oldLevel)
        {
            string towerName = this.towerCtrl.name.ToLower();
            
            // Kiểm tra nếu là MachineGun tower - tăng máu
            if (towerName.Contains("machinegun"))
            {
                // Gọi OnLevelUp để tăng máu
                if (this.towerCtrl.TowerDamageReceiver != null)
                {
                    this.towerCtrl.TowerDamageReceiver.OnLevelUp();
                }
            }
            
            // Kiểm tra nếu là OneGunBarrel tower - tăng tốc độ bắn
            if (towerName.Contains("onegunbarrel"))
            {
                // Gọi OnLevelUp để tăng tốc độ bắn
                if (this.towerCtrl.TowerShooting != null)
                {
                    this.towerCtrl.TowerShooting.OnLevelUp();
                }
            }
        }
    }
}
