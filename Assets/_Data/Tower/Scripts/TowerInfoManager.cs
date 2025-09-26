using System.Collections.Generic;
using UnityEngine;

public class TowerInfoManager : SaiSingleton<TowerInfoManager>
{
    [Header("Tower Info Database")]
    [SerializeField] protected List<TowerInfoData> allTowerInfos = new List<TowerInfoData>();
    

    
    protected Dictionary<string, TowerInfoData> towerInfoLookup = new Dictionary<string, TowerInfoData>();
    
    protected override void Start()
    {
        base.Start();
        this.InitializeTowerDatabase();
        this.SetupLookupDictionary();
    }
    
    protected virtual void InitializeTowerDatabase()
    {
        // Create all tower infos
        this.CreateAllTowerInfos();
    }
    
    protected virtual void CreateAllTowerInfos()
    {
        // Basic Tower
        TowerInfoData basicTower = new TowerInfoData
        {
            towerName = "Basic Tower",
            description = "A simple defensive tower with balanced stats. Perfect for beginners.",
            basePrice = 100,
            towerType = TowerType.Tower,
            specialAbilities = new string[] { "Auto-Targeting", "Piercing Shot", "Balanced Stats" }
        };
        
        // Archer Tower
        TowerInfoData archerTower = new TowerInfoData
        {
            towerName = "Archer Tower",
            description = "Long-range tower with high accuracy and moderate damage.",
            basePrice = 120,
            towerType = TowerType.Tower,
            specialAbilities = new string[] { "Long Range", "High Accuracy", "Critical Hit Chance" }
        };
        
        // Machine Gun Tower
        TowerInfoData machineGunTower = new TowerInfoData
        {
            towerName = "Machine Gun Tower",
            description = "A fully automatic and rifled firearm designed for sustained direct fire",
            basePrice = 60,
            towerType = TowerType.Tower,
            specialAbilities = new string[] { "Fire Rate: 0.7s per shot", "Range: 15 units", "Level Up: +20% max HP & heal 20%" }
        };
        
        // Cannon Tower
        TowerInfoData cannonTower = new TowerInfoData
        {
            towerName = "Cannon Tower",
            description = "Heavy damage tower with area effect and slow reload.",
            basePrice = 200,
            towerType = TowerType.Tower,
            specialAbilities = new string[] { "Area Damage", "High Damage", "Stun Effect" }
        };
        
        // Spike Trap
        TowerInfoData spikeTrap = new TowerInfoData
        {
            towerName = "Spike Trap",
            description = "Simple trap that deals instant damage to enemies.",
            basePrice = 80,
            towerType = TowerType.Trap,
            specialAbilities = new string[] { "Instant Damage", "Hidden", "Reusable" }
        };
        
        // Pit Trap
        TowerInfoData pitTrap = new TowerInfoData
        {
            towerName = "Pit Trap",
            description = "Trap that slows enemies and deals continuous damage.",
            basePrice = 90,
            towerType = TowerType.Trap,
            specialAbilities = new string[] { "Slow Effect", "Continuous Damage", "Large Area" }
        };
        
        // Flame Trap
        TowerInfoData flameTrap = new TowerInfoData
        {
            towerName = "Flame Trap",
            description = "A burning trap that ignites enemies with continuous fire damage",
            basePrice = 150,
            towerType = TowerType.Trap,
            specialAbilities = new string[] { "Burn Damage: 3 HP per second", "Area Effect", "Continuous Damage" }
        };
        
        // Ice Trap
        TowerInfoData iceTrap = new TowerInfoData
        {
            towerName = "Ice Trap",
            description = "A frozen trap that slows down enemies with ice magic",
            basePrice = 100,
            towerType = TowerType.Trap,
            specialAbilities = new string[] { "Slow Effect: -1 movement speed", "Area Control", "Hidden Trap" }
        };
        
        // Add all to the main list
        this.allTowerInfos.Add(basicTower);
        this.allTowerInfos.Add(archerTower);
        this.allTowerInfos.Add(machineGunTower);
        this.allTowerInfos.Add(cannonTower);
        this.allTowerInfos.Add(spikeTrap);
        this.allTowerInfos.Add(pitTrap);
        this.allTowerInfos.Add(flameTrap);
        this.allTowerInfos.Add(iceTrap);
    }
    
    protected virtual void SetupLookupDictionary()
    {
        this.towerInfoLookup.Clear();
        
        foreach (TowerInfoData towerInfo in this.allTowerInfos)
        {
            if (!string.IsNullOrEmpty(towerInfo.towerName))
            {
                this.towerInfoLookup[towerInfo.towerName] = towerInfo;
            }
        }
    }
    
    public virtual TowerInfoData GetTowerInfo(string towerName)
    {
        if (this.towerInfoLookup.ContainsKey(towerName))
            return this.towerInfoLookup[towerName];
            
        return null;
    }
    
    public virtual List<TowerInfoData> GetTowersByType(TowerType towerType)
    {
        List<TowerInfoData> result = new List<TowerInfoData>();
        
        foreach (TowerInfoData towerInfo in this.allTowerInfos)
        {
            if (towerInfo.towerType == towerType)
                result.Add(towerInfo);
        }
        
        return result;
    }
    

    
    public virtual List<TowerInfoData> GetAllTowerInfos()
    {
        return new List<TowerInfoData>(this.allTowerInfos);
    }
    
    public virtual void AddTowerInfo(TowerInfoData newTowerInfo)
    {
        if (newTowerInfo == null) return;
        
        // Check if tower with same name already exists
        TowerInfoData existingTower = this.GetTowerInfo(newTowerInfo.towerName);
        if (existingTower != null)
        {
            // Update existing tower info
            int index = this.allTowerInfos.IndexOf(existingTower);
            if (index >= 0)
            {
                this.allTowerInfos[index] = newTowerInfo;
            }
        }
        else
        {
            // Add new tower info
            this.allTowerInfos.Add(newTowerInfo);
        }
        
        // Update lookup dictionary
        this.SetupLookupDictionary();
    }
    

    

}
