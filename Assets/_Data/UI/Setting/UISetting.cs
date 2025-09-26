using UnityEngine;

public class UISetting : SaiSingleton<UISetting>
{
    protected bool isShow = true;
    protected bool IsShow => isShow;
    
    // Public property để truy cập từ bên ngoài
    public bool IsSettingOpen => isShow;

    [SerializeField] protected Transform showHide;

    protected override void Start()
    {
        base.Start();
        this.Hide();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadShowHide();
    }

    protected virtual void LoadShowHide()
    {
        if (this.showHide != null) return;
        this.showHide = transform.Find("ShowHide");
        Debug.Log(transform.name + ": LoadShowHide", gameObject);
    }

    public virtual void Show()
    {
        this.isShow = true;
        this.showHide.gameObject.SetActive(this.isShow);
        HideMouse.Instance.isCursorVisible = this.isShow;
        // Pause game khi mở setting
        Time.timeScale = 0f; // Pause game
        
        // Đảm bảo fullscreen luôn on khi mở setting
        this.EnsureFullscreenOnShow();
        
        Debug.Log("Game paused - Settings opened");
    }

    public virtual void Hide()
    {
        this.isShow = false;
        this.showHide.gameObject.SetActive(false);
        HideMouse.Instance.isCursorVisible = this.isShow;
        // Unpause game khi đóng setting
        Time.timeScale = 1f; // Resume game
        Debug.Log("Game resumed - Settings closed");
    }

    public virtual void ToggleSetting()
    {
        if (this.isShow) this.Hide();
        else this.Show();
    }
    
    /// <summary>
    /// Set fullscreen luôn on
    /// </summary>
    public virtual void SetFullscreenAlwaysOn()
    {
        try
        {
            Debug.Log("=== SETTING FULLSCREEN ALWAYS ON ===");
            
            // Force fullscreen ngay lập tức
            if (FullscreenManager.Instance != null)
            {
                Debug.Log("FullscreenManager found, forcing fullscreen...");
                FullscreenManager.Instance.ForceFullscreen();
                Debug.Log("Fullscreen forced successfully!");
            }
            else
            {
                // Fallback: Set trực tiếp qua Unity API
                Debug.Log("FullscreenManager not found, using Unity API directly...");
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                PlayerPrefs.SetInt("FullScreen", 1);
                PlayerPrefs.Save();
                Debug.Log($"Fullscreen set directly: {Screen.currentResolution.width}x{Screen.currentResolution.height}");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in SetFullscreenAlwaysOn: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Đảm bảo fullscreen luôn on khi mở setting
    /// </summary>
    public virtual void EnsureFullscreenOnShow()
    {
        try
        {
            Debug.Log("=== ENSURING FULLSCREEN ON SHOW ===");
            
            // Kiểm tra xem có đang fullscreen không
            if (!Screen.fullScreen)
            {
                Debug.Log("Not in fullscreen, forcing fullscreen...");
                this.SetFullscreenAlwaysOn();
            }
            else
            {
                Debug.Log("Already in fullscreen mode");
            }
            
            Debug.Log("================================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in EnsureFullscreenOnShow: {e.Message}");
        }
    }
}
