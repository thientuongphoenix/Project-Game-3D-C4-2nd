using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Thêm để sử dụng CanvasGroup

public class GameResultManager : MonoBehaviour
{
    
    void Start()
    {
        this.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameEnded) return;
        this.UpdateTimer();
        this.CheckLoseCondition();
        this.CheckWinCondition();
    }

    
    [SerializeField] protected float timer = 0f;
    [SerializeField] protected bool isGameEnded = false;
    [SerializeField] protected bool isLose = false;
    [SerializeField] protected bool isWin = false;

    [SerializeField] protected PlayerCtrl player;
    [SerializeField] protected CoreCtrl core;
    [SerializeField] protected GameObject winPanel;
    [SerializeField] protected GameObject losePanel;
    [SerializeField] protected float defendTime = 60f;
    
    // Thêm biến cho hiệu ứng fade
    [SerializeField] protected float fadeInDuration = 3f;
    protected CanvasGroup winPanelCanvasGroup;
    protected CanvasGroup losePanelCanvasGroup;

    protected virtual void Init()
    {
        // Reset trạng thái game
        this.ResetGameState();
        
        this.player = PlayerCtrl.Instance;
        this.core = FindObjectOfType<CoreCtrl>();
        this.winPanel = GameObject.Find("WinPanel");
        this.losePanel = GameObject.Find("LosePanel");
        
        // Khởi tạo CanvasGroup cho fade effect
        this.InitializeCanvasGroups();
        
        if (this.winPanel != null) this.winPanel.SetActive(false);
        if (this.losePanel != null) this.losePanel.SetActive(false);
        
        Debug.Log("GameResultManager: Initialized for new scene");
    }
    
    protected virtual void ResetGameState()
    {
        this.timer = 0f;
        this.isGameEnded = false;
        this.isLose = false;
        this.isWin = false;
    }
    
    protected virtual void InitializeCanvasGroups()
    {
        // Khởi tạo CanvasGroup cho WinPanel
        if (this.winPanel != null)
        {
            this.winPanelCanvasGroup = this.winPanel.GetComponent<CanvasGroup>();
            if (this.winPanelCanvasGroup == null)
            {
                this.winPanelCanvasGroup = this.winPanel.AddComponent<CanvasGroup>();
            }
            this.winPanelCanvasGroup.alpha = 0f;
        }
        
        // Khởi tạo CanvasGroup cho LosePanel
        if (this.losePanel != null)
        {
            this.losePanelCanvasGroup = this.losePanel.GetComponent<CanvasGroup>();
            if (this.losePanelCanvasGroup == null)
            {
                this.losePanelCanvasGroup = this.losePanel.AddComponent<CanvasGroup>();
            }
            this.losePanelCanvasGroup.alpha = 0f;
        }
    }

    protected virtual void UpdateTimer()
    {
        timer += Time.deltaTime;
    }

    protected virtual void CheckLoseCondition()
    {
        if (isLose || isGameEnded) return;
        if (IsPlayerDead() || IsCoreDead())
        {
            isLose = true;
            // Tắt tất cả SFX ngay lập tức, giữ lại nhạc
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.VolumeSfxUpdating(0f);
                //Debug.Log("Đã tắt tất cả SFX, giữ lại nhạc nền");
            }
            StartCoroutine(ShowLosePanelAfterDelay(2f));
        }
    }

    protected virtual void CheckWinCondition()
    {
        if (isWin || isGameEnded) return;
        if (timer >= defendTime)
        {
            isWin = true;
            // Tắt tất cả SFX ngay lập tức, giữ lại nhạc
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.VolumeSfxUpdating(0f);
                //Debug.Log("Đã tắt tất cả SFX, giữ lại nhạc nền");
            }
            ShowWinPanel();
        }
    }

    protected virtual bool IsPlayerDead()
    {
        if (player == null || player.PlayerDamageReceiver == null) return false;
        return player.PlayerDamageReceiver.IsDead();
    }

    protected virtual bool IsCoreDead()
    {
        if (core == null || core.TowerDamageReceiver == null) return false;
        return core.TowerDamageReceiver.IsDead();
    }

    protected virtual IEnumerator ShowLosePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowLosePanel();
    }

    protected virtual void ShowWinPanel()
    {
        isGameEnded = true;
        if (winPanel != null) 
        {
            winPanel.SetActive(true);
            StartCoroutine(FadeInPanel(winPanelCanvasGroup));
        }
        if (losePanel != null) losePanel.SetActive(false);
        HideMouse.Instance.isCursorVisible = true; // Hiện chuột khi hiện panel
        
        // Hoàn thành map hiện tại
        this.CompleteCurrentMap();
        
        // Có thể bổ sung hiệu ứng, âm thanh,... ở đây
    }
    
    protected virtual void CompleteCurrentMap()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"=== COMPLETING MAP: {currentSceneName} ===");
        
        if (MapProgressManager.Instance != null)
        {
            MapProgressManager.Instance.CompleteMap(currentSceneName);
            Debug.Log($"Map {currentSceneName} completed and saved!");
            
            // Debug: Kiểm tra danh sách completed maps
            var completedMaps = MapProgressManager.Instance.GetCompletedMaps();
            Debug.Log($"Current completed maps: {string.Join(", ", completedMaps)}");
        }
        else
        {
            Debug.LogError("MapProgressManager.Instance is NULL! Cannot save map progress!");
        }
    }

    protected virtual void ShowLosePanel()
    {
        isGameEnded = true;
        if (losePanel != null) 
        {
            losePanel.SetActive(true);
            StartCoroutine(FadeInPanel(losePanelCanvasGroup));
        }
        if (winPanel != null) winPanel.SetActive(false);
        HideMouse.Instance.isCursorVisible = true; // Hiện chuột khi hiện panel
        // Có thể bổ sung hiệu ứng, âm thanh,... ở đây
    }
    
    // Thêm hàm fade in cho panel
    protected virtual IEnumerator FadeInPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) yield break;
        
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        
        
    }
}
