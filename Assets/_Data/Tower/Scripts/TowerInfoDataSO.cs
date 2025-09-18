using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Info", menuName = "Tower Defense/Tower Info Data")]
public class TowerInfoDataSO : ScriptableObject
{
    [Header("Basic Information")]
    public string towerName = "Tower Name";
    [TextArea(3, 5)]
    public string description = "Tower description here";
    public Sprite icon;
    
    [Header("Cost")]
    public int basePrice = 100;
    
    [Header("Special Abilities")]
    [TextArea(2, 3)]
    public string[] specialAbilities = new string[3];
    
    [Header("Tower Type")]
    public TowerType towerType = TowerType.Tower;
    
    // Convert to TowerInfoData
    public TowerInfoData ToTowerInfoData()
    {
        TowerInfoData data = new TowerInfoData();
        data.towerName = this.towerName;
        data.description = this.description;
        data.icon = this.icon;
        data.basePrice = this.basePrice;
        data.specialAbilities = this.specialAbilities;
        data.towerType = this.towerType;
        return data;
    }
    
    // Create from TowerInfoData
    public void FromTowerInfoData(TowerInfoData data)
    {
        this.towerName = data.towerName;
        this.description = data.description;
        this.icon = data.icon;
        this.basePrice = data.basePrice;
        this.specialAbilities = data.specialAbilities;
        this.towerType = data.towerType;
    }
}
