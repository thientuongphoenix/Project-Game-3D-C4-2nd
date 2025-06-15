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
        if (this.towerCtrl != null && !this.towerCtrl.CanLevelUp()) return;
        if (this.currentLevel >= this.maxLevel) return;
        if (this.GetCurrentExp() < this.GetNextLevelExp()) return;
        if (!this.DeductExp(this.GetNextLevelExp())) return;
        this.currentLevel++;
    }
}
