using UnityEngine;

public class PlayerLevel : LevelByItem
{
    [Header("Level System Integration")]
    [SerializeField] protected PlayerLevelSystem levelSystem;
    
    protected override void Start()
    {
        base.Start();
        this.LoadLevelSystem();
    }
    
    protected virtual void LoadLevelSystem()
    {
        if (levelSystem != null) return;
        levelSystem = GetComponent<PlayerLevelSystem>();
        if (levelSystem == null)
        {
            levelSystem = gameObject.AddComponent<PlayerLevelSystem>();
        }
        Debug.Log("PlayerLevel: Integrated with PlayerLevelSystem", gameObject);
    }
    
    /// <summary>
    /// Override Leveling để tích hợp với PlayerLevelSystem
    /// </summary>
    protected override void Leveling()
    {
        int oldLevel = this.currentLevel;
        base.Leveling();
        
        // Kiểm tra nếu level đã tăng
        if (this.currentLevel > oldLevel && levelSystem != null)
        {
            Debug.Log($"🎉 LEVEL UP DETECTED! Level {oldLevel} → {this.currentLevel}");
            
            // Gọi PlayerLevelSystem để tăng stats
            levelSystem.OnLevelUpFromOldSystem(this.currentLevel - oldLevel);
        }
    }
}
