using UnityEngine;

public class TextPlayerLvlCount : TextAbstract
{
    [Header("Display Options")]
    [SerializeField] protected bool showExp = true;
    [SerializeField] protected bool showStats = false;
    
    protected virtual void FixedUpdate()
    {
        this.LoadCount();
    }

    protected virtual void LoadCount()
    {
        if (PlayerCtrl.Instance?.Level == null) return;
        
        string displayText = PlayerCtrl.Instance.Level.CurrentLevel.ToString();
        
        // Thêm exp nếu được bật
        if (showExp && PlayerLevelSystem.Instance != null)
        {
            int currentExp = PlayerLevelSystem.Instance.GetCurrentExp();
            int expToNext = PlayerLevelSystem.Instance.GetExpToNextLevel();
            float progress = PlayerLevelSystem.Instance.GetExpProgress();
            displayText += $" | EXP: {currentExp}/{expToNext} ({progress:P0})";
        }
        
        // Thêm stats nếu được bật
        if (showStats && PlayerLevelSystem.Instance != null)
        {
            var (movement, attack, cooldown) = PlayerLevelSystem.Instance.GetCurrentStats();
            displayText += $"\nSpeed: {movement:P0} | Attack: {attack:P0} | Cooldown: {cooldown:P0}";
        }
        
        this.textPro.text = displayText;
    }
}
