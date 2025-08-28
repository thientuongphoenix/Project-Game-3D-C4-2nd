using UnityEngine;

[System.Serializable]
public class TowerInfoData
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
    
    // Helper method to create default data
    public static TowerInfoData CreateDefault(string name, TowerType type, int price)
    {
        TowerInfoData data = new TowerInfoData();
        data.towerName = name;
        data.towerType = type;
        data.basePrice = price;
        
        // Set default description based on type
        if (type == TowerType.Trap)
        {
            data.description = "A defensive trap that affects enemies";
            data.specialAbilities[0] = "Area Effect";
            data.specialAbilities[1] = "Continuous Damage";
            data.specialAbilities[2] = "Hidden";
        }
        else
        {
            data.description = "A defensive tower that attacks enemies";
            data.specialAbilities[0] = "Ranged Attack";
            data.specialAbilities[1] = "Auto-Targeting";
            data.specialAbilities[2] = "Balanced Stats";
        }
        
        return data;
    }
}
