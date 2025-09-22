using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySpawnButton : SaiMonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected Button spawnButton;
    [SerializeField] protected TextMeshProUGUI buttonText;
    [SerializeField] protected TextMeshProUGUI statusText;
    
    [Header("Button Settings")]
    [SerializeField] protected string startText = "Start Enemy Waves (P)";
    [SerializeField] protected string spawningText = "Spawning...";
    [SerializeField] protected string waitingText = "Start Next Wave (P)";
    [SerializeField] protected string completedText = "All Waves Completed! (P)";
    [SerializeField] protected string resetText = "Reset Waves (P)";
    
    [Header("Keyboard Shortcut")]
    [SerializeField] protected KeyCode shortcutKey = KeyCode.P;
    
    protected override void Start()
    {
        base.Start();
        this.LoadUIReferences();
        this.UpdateButtonState();
    }
    
    protected virtual void LoadUIReferences()
    {
        if (this.spawnButton == null)
            this.spawnButton = GetComponent<Button>();
            
        if (this.buttonText == null)
            this.buttonText = transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
            
        if (this.statusText == null)
            this.statusText = transform.parent?.Find("StatusText")?.GetComponent<TextMeshProUGUI>();
    }
    
    protected virtual void Update()
    {
        this.UpdateButtonState();
        this.CheckKeyboardInput();
    }
    
    protected virtual void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(shortcutKey))
        {
            Debug.Log($"Keyboard shortcut '{shortcutKey}' pressed!");
            this.OnSpawnButtonClicked();
        }
    }
    
    protected virtual void UpdateButtonState()
    {
        if (EnemyWaveManager.Instance == null) return;
        
        bool isSpawning = EnemyWaveManager.Instance.IsSpawning();
        bool isWaiting = EnemyWaveManager.Instance.IsWaitingForButtonPress();
        bool allCompleted = EnemyWaveManager.Instance.AreAllWavesCompleted();
        
        if (allCompleted)
        {
            this.SetButtonState(true, completedText, "All waves completed!");
        }
        else if (isSpawning)
        {
            this.SetButtonState(false, spawningText, $"Spawning Wave {EnemyWaveManager.Instance.GetCurrentWaveNumber()}/{EnemyWaveManager.Instance.GetTotalWaves()}");
        }
        else if (isWaiting)
        {
            this.SetButtonState(true, waitingText, $"Wave {EnemyWaveManager.Instance.GetCurrentWaveNumber() - 1} completed! Ready for Wave {EnemyWaveManager.Instance.GetCurrentWaveNumber()}");
        }
        else
        {
            this.SetButtonState(true, startText, "Ready to start waves");
        }
    }
    
    protected virtual void SetButtonState(bool interactable, string buttonText, string statusText)
    {
        if (this.spawnButton != null)
            this.spawnButton.interactable = interactable;
            
        if (this.buttonText != null)
            this.buttonText.text = buttonText;
            
        if (this.statusText != null)
            this.statusText.text = statusText;
    }
    
    public virtual void OnSpawnButtonClicked()
    {
        if (EnemyWaveManager.Instance == null)
        {
            Debug.LogError("EnemyWaveManager not found!");
            return;
        }
        
        bool allCompleted = EnemyWaveManager.Instance.AreAllWavesCompleted();
        bool isSpawning = EnemyWaveManager.Instance.IsSpawning();
        bool isWaiting = EnemyWaveManager.Instance.IsWaitingForButtonPress();
        
        if (allCompleted)
        {
            // Reset waves
            EnemyWaveManager.Instance.ResetWaveSystem();
            Debug.Log("Waves reset!");
        }
        else if (isWaiting)
        {
            // Start next wave manually
            EnemyWaveManager.Instance.StartNextWaveManually();
            Debug.Log("Starting next wave manually...");
        }
        else if (!isSpawning)
        {
            // Start first wave
            EnemyWaveManager.Instance.StartWaveSystem();
            Debug.Log("Starting enemy waves...");
        }
    }
}
