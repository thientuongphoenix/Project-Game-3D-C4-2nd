using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string mapSelectionSceneName = "MapSelect_Hai";
    
    [Header("New Game Button")]
    [SerializeField] protected GameObject newGameButton;
    [SerializeField] protected string tutorialMapName = "Hai_SampleScene";
    
    [Header("Layout Control")]
    [SerializeField] protected RectTransform menuContainer; // Container chứa tất cả button
    [SerializeField] protected float newGameButtonHeight = 60f; // Chiều cao của New Game button
    [SerializeField] protected float buttonSpacing = 20f; // Khoảng cách giữa các button
    
    [Header("Button Text Control")]
    [SerializeField] protected GameObject playButton; // Button PLAY/CONTINUE
    [SerializeField] protected string playText = "PLAY";
    [SerializeField] protected string continueText = "CONTINUE";
    
    [Header("New Game Confirmation Dialog")]
    [SerializeField] protected GameObject confirmationDialog; // Dialog cảnh báo
    [SerializeField] protected TextMeshProUGUI dialogTitleText;
    [SerializeField] protected TextMeshProUGUI dialogMessageText;
    [SerializeField] protected Button confirmButton; // Button "Có"
    [SerializeField] protected Button cancelButton; // Button "Không"
    [SerializeField] protected string dialogTitle = "WARNING";
    [SerializeField] protected string dialogMessage = "If you choose this button, all data will be reset.\nAre you sure you want to do this?";
    [SerializeField] protected string confirmText = "YES";
    [SerializeField] protected string cancelText = "NO";
    
    [Header("Cutscene Video")]
    [SerializeField] protected GameObject cutsceneCanvas; // Canvas chứa video player
    [SerializeField] protected VideoPlayer videoPlayer; // VideoPlayer component
    [SerializeField] protected RawImage videoDisplay; // RawImage để hiển thị video
    [SerializeField] protected Button skipButton; // Nút skip video
    [SerializeField] protected VideoClip cutsceneVideoClip; // Video clip cần phát
    [SerializeField] protected string skipButtonText = "SKIP";
    [SerializeField] protected bool enableCutscene = true; // Bật/tắt cutscene
    
    [Header("Video Settings")]
    [SerializeField] protected int videoWidth = 1216; // Chiều rộng video
    [SerializeField] protected int videoHeight = 1080; // Chiều cao video
    [SerializeField] protected bool maintainAspectRatio = true; // Giữ tỷ lệ khung hình
    [SerializeField] protected float videoScale = 1.0f; // Tỷ lệ phóng to/thu nhỏ video (1.0 = kích thước gốc)
    [SerializeField] protected bool centerVideo = true; // Căn giữa video trên màn hình
    
    protected virtual void Start()
    {
        this.LoadNewGameButton();
        this.LoadMenuContainer();
        this.LoadPlayButton();
        this.LoadConfirmationDialog();
        this.LoadCutsceneComponents();
        // Sử dụng Invoke để đảm bảo MapProgressManager đã được khởi tạo
        Invoke(nameof(CheckNewGameButtonVisibility), 0.1f);
    }
    
    protected virtual void LoadNewGameButton()
    {
        if (this.newGameButton != null) return;
        this.newGameButton = GameObject.Find("NewGameButton");
        Debug.Log(transform.name + ": LoadNewGameButton", gameObject);
    }
    
    protected virtual void LoadMenuContainer()
    {
        if (this.menuContainer != null) return;
        // Tìm container chứa các button menu (có thể là Canvas hoặc Panel)
        this.menuContainer = transform.Find("MenuContainer")?.GetComponent<RectTransform>();
        if (this.menuContainer == null)
        {
            // Nếu không tìm thấy MenuContainer, sử dụng transform của MainMenu
            this.menuContainer = GetComponent<RectTransform>();
        }
        Debug.Log(transform.name + ": LoadMenuContainer", gameObject);
    }
    
    protected virtual void LoadPlayButton()
    {
        if (this.playButton != null) return;
        // Tìm button PLAY trong scene
        this.playButton = GameObject.Find("PlayButton");
        if (this.playButton == null)
        {
            // Nếu không tìm thấy PlayButton, tìm button có text "PLAY"
            Button[] allButtons = GetComponentsInChildren<Button>();
            foreach (Button button in allButtons)
            {
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null && buttonText.text.ToUpper().Contains("PLAY"))
                {
                    this.playButton = button.gameObject;
                    break;
                }
            }
        }
        Debug.Log(transform.name + ": LoadPlayButton", gameObject);
    }
    
    protected virtual void LoadConfirmationDialog()
    {
        if (this.confirmationDialog != null) return;
        
        // Tìm dialog trong scene
        this.confirmationDialog = GameObject.Find("ConfirmationDialog");
        if (this.confirmationDialog != null)
        {
            // Load các component của dialog
            this.LoadDialogComponents();
            
            // Ẩn dialog ban đầu
            this.confirmationDialog.SetActive(false);
            
            Debug.Log(transform.name + ": LoadConfirmationDialog - Dialog found and loaded", gameObject);
        }
        else
        {
            Debug.LogWarning("ConfirmationDialog not found in scene. Please create it manually following the guide.");
        }
    }
    
    protected virtual void LoadDialogComponents()
    {
        // Load các component từ dialog có sẵn
        this.dialogTitleText = this.confirmationDialog.transform.Find("DialogPanel/Title")?.GetComponent<TextMeshProUGUI>();
        this.dialogMessageText = this.confirmationDialog.transform.Find("DialogPanel/Message")?.GetComponent<TextMeshProUGUI>();
        this.confirmButton = this.confirmationDialog.transform.Find("DialogPanel/ButtonContainer/ConfirmButton")?.GetComponent<Button>();
        this.cancelButton = this.confirmationDialog.transform.Find("DialogPanel/ButtonContainer/CancelButton")?.GetComponent<Button>();
        
        Debug.Log($"Dialog components loaded - Title: {dialogTitleText != null}, Message: {dialogMessageText != null}, Confirm: {confirmButton != null}, Cancel: {cancelButton != null}");
        
        // Setup button listeners
        if (this.confirmButton != null)
        {
            this.confirmButton.onClick.AddListener(this.ConfirmNewGame);
            Debug.Log("Confirm button listener added");
        }
        else
        {
            Debug.LogError("ConfirmButton not found! Check hierarchy structure.");
        }
        
        if (this.cancelButton != null)
        {
            this.cancelButton.onClick.AddListener(this.CancelNewGame);
            Debug.Log("Cancel button listener added");
        }
        else
        {
            Debug.LogError("CancelButton not found! Check hierarchy structure.");
        }
    }
    
    protected virtual void LoadCutsceneComponents()
    {
        if (this.cutsceneCanvas != null) return;
        
        // Tìm cutscene canvas trong scene (có thể là root canvas)
        this.cutsceneCanvas = GameObject.Find("CutsceneCanvas");
        if (this.cutsceneCanvas == null)
        {
            // Tìm canvas có tag "CutsceneCanvas" hoặc tên chứa "Cutscene"
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.name.Contains("Cutscene") || canvas.CompareTag("CutsceneCanvas"))
                {
                    this.cutsceneCanvas = canvas.gameObject;
                    break;
                }
            }
        }
        
        if (this.cutsceneCanvas != null)
        {
            // Load các component của cutscene
            this.LoadCutsceneUIComponents();
            
            // Ẩn cutscene canvas ban đầu
            this.cutsceneCanvas.SetActive(false);
            
            Debug.Log(transform.name + ": LoadCutsceneComponents - Cutscene canvas found and loaded", gameObject);
        }
        else
        {
            Debug.LogWarning("CutsceneCanvas not found in scene. Please create it manually following the guide.");
        }
    }
    
    protected virtual void LoadCutsceneUIComponents()
    {
        Debug.Log("LoadCutsceneUIComponents() called!");
        
        // Load VideoPlayer component
        this.videoPlayer = this.cutsceneCanvas.GetComponent<VideoPlayer>();
        if (this.videoPlayer == null)
        {
            Debug.Log("VideoPlayer component not found, creating new one...");
            this.videoPlayer = this.cutsceneCanvas.AddComponent<VideoPlayer>();
            if (this.videoPlayer != null)
            {
                Debug.Log("VideoPlayer component created successfully!");
            }
            else
            {
                Debug.LogError("Failed to create VideoPlayer component!");
                return;
            }
        }
        else
        {
            Debug.Log("VideoPlayer component found!");
        }
        
        // Load RawImage để hiển thị video
        this.videoDisplay = this.cutsceneCanvas.transform.Find("VideoDisplay")?.GetComponent<RawImage>();
        if (this.videoDisplay == null)
        {
            Debug.LogError("VideoDisplay RawImage not found! Check hierarchy structure.");
        }
        else
        {
            Debug.Log("VideoDisplay found!");
        }
        
        // Load Skip button
        this.skipButton = this.cutsceneCanvas.transform.Find("SkipButton")?.GetComponent<Button>();
        if (this.skipButton != null)
        {
            this.skipButton.onClick.AddListener(this.SkipCutscene);
            
            // Set text cho skip button
            TextMeshProUGUI skipButtonText = this.skipButton.GetComponentInChildren<TextMeshProUGUI>();
            if (skipButtonText != null)
            {
                skipButtonText.text = this.skipButtonText;
            }
            
            Debug.Log("Skip button listener added");
        }
        else
        {
            Debug.LogError("SkipButton not found! Check hierarchy structure.");
        }
        
        // Setup VideoPlayer
        this.SetupVideoPlayer();
        
        Debug.Log($"Cutscene components loaded - VideoPlayer: {videoPlayer != null}, VideoDisplay: {videoDisplay != null}, SkipButton: {skipButton != null}");
    }
    
    protected virtual void SetupVideoPlayer()
    {
        Debug.Log("SetupVideoPlayer() called!");
        
        if (this.videoPlayer == null) 
        {
            Debug.LogError("VideoPlayer is null! Cannot setup.");
            return;
        }
        
        Debug.Log("Configuring VideoPlayer...");
        // Cấu hình VideoPlayer
        this.videoPlayer.playOnAwake = false;
        this.videoPlayer.isLooping = false;
        this.videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        
        // Tạo RenderTexture cho video với resolution tùy chỉnh
        Debug.Log($"Creating RenderTexture with resolution {this.videoWidth}x{this.videoHeight}...");
        RenderTexture renderTexture = new RenderTexture(this.videoWidth, this.videoHeight, 0);
        this.videoPlayer.targetTexture = renderTexture;
        
        // Gán RenderTexture cho RawImage
        if (this.videoDisplay != null)
        {
            this.videoDisplay.texture = renderTexture;
            
            // Điều chỉnh RawImage để hiển thị video đúng tỷ lệ
            this.AdjustVideoDisplayAspectRatio();
            
            Debug.Log("RenderTexture assigned to VideoDisplay");
        }
        else
        {
            Debug.LogWarning("VideoDisplay is null! Cannot assign RenderTexture.");
        }
        
        // Gán video clip nếu có
        if (this.cutsceneVideoClip != null)
        {
            this.videoPlayer.clip = this.cutsceneVideoClip;
            Debug.Log($"Video clip assigned: {this.cutsceneVideoClip.name}");
        }
        else
        {
            Debug.LogWarning("No video clip assigned! Please assign cutsceneVideoClip in Inspector.");
        }
        
        // Thêm event khi video kết thúc
        this.videoPlayer.loopPointReached += OnVideoFinished;
        
        Debug.Log("VideoPlayer setup completed successfully!");
    }
    
    protected virtual void AdjustVideoDisplayAspectRatio()
    {
        if (this.videoDisplay == null) return;
        
        Debug.Log("Adjusting video display aspect ratio...");
        
        // Tính tỷ lệ khung hình của video
        float videoAspectRatio = (float)this.videoWidth / this.videoHeight;
        Debug.Log($"Video aspect ratio: {videoAspectRatio} ({this.videoWidth}:{this.videoHeight})");
        
        // Lấy kích thước màn hình
        Canvas canvas = this.cutsceneCanvas.GetComponent<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            float screenWidth = canvasRect.rect.width;
            float screenHeight = canvasRect.rect.height;
            float screenAspectRatio = screenWidth / screenHeight;
            
            Debug.Log($"Screen aspect ratio: {screenAspectRatio} ({screenWidth}:{screenHeight})");
            
            RectTransform videoRect = this.videoDisplay.GetComponent<RectTransform>();
            
            if (this.maintainAspectRatio)
            {
                // Giữ tỷ lệ khung hình video và fit vào màn hình
                float scaleWidth = screenWidth / this.videoWidth;
                float scaleHeight = screenHeight / this.videoHeight;
                float autoScale = Mathf.Min(scaleWidth, scaleHeight); // Chọn scale nhỏ hơn để fit hoàn toàn
                
                // Áp dụng videoScale từ Inspector
                float finalScale = autoScale * this.videoScale;
                
                float newWidth = this.videoWidth * finalScale;
                float newHeight = this.videoHeight * finalScale;
                
                videoRect.sizeDelta = new Vector2(newWidth, newHeight);
                
                if (this.centerVideo)
                {
                    videoRect.anchorMin = new Vector2(0.5f, 0.5f);
                    videoRect.anchorMax = new Vector2(0.5f, 0.5f);
                    videoRect.anchoredPosition = Vector2.zero;
                }
                else
                {
                    videoRect.anchorMin = new Vector2(0, 0);
                    videoRect.anchorMax = new Vector2(0, 0);
                    videoRect.anchoredPosition = new Vector2(newWidth / 2, newHeight / 2);
                }
                
                Debug.Log($"Video scaled to fit: {newWidth}x{newHeight} (auto scale: {autoScale}, final scale: {finalScale})");
            }
            else
            {
                // Stretch toàn màn hình
                videoRect.sizeDelta = new Vector2(screenWidth, screenHeight);
                videoRect.anchorMin = new Vector2(0, 0);
                videoRect.anchorMax = new Vector2(1, 1);
                videoRect.anchoredPosition = Vector2.zero;
                Debug.Log($"Video stretched to full screen: {screenWidth}x{screenHeight}");
            }
        }
        else
        {
            Debug.LogWarning("Canvas not found! Cannot adjust aspect ratio.");
        }
    }
    
    protected virtual void CheckNewGameButtonVisibility()
    {
        if (this.newGameButton == null) return;
        
        // Kiểm tra MapProgressManager có tồn tại không
        if (MapProgressManager.Instance == null)
        {
            Debug.LogWarning("MapProgressManager.Instance is null, retrying in 0.1 seconds...");
            Invoke(nameof(CheckNewGameButtonVisibility), 0.1f);
            return;
        }
        
        // Chỉ hiện button New Game khi player đã hoàn thành tutorial
        bool hasCompletedTutorial = MapProgressManager.Instance.IsMapCompleted(tutorialMapName);
        
        if (hasCompletedTutorial)
        {
            // Hiện button và đẩy các button khác xuống
            this.ShowNewGameButton();
        }
        else
        {
            // Ẩn button và thu hẹp layout
            this.HideNewGameButton();
        }
        
        Debug.Log($"New Game Button visibility: {hasCompletedTutorial} (Tutorial completed: {hasCompletedTutorial})");
    }
    
    protected virtual void ShowNewGameButton()
    {
        if (this.newGameButton == null) return;
        
        // Hiện button
        this.newGameButton.SetActive(true);
        
        // Đặt New Game button lên đầu tiên
        this.newGameButton.transform.SetAsFirstSibling();
        
        // Điều chỉnh layout để đẩy các button khác xuống
        if (this.menuContainer != null)
        {
            // Tăng chiều cao của container để chứa thêm button
            Vector2 sizeDelta = this.menuContainer.sizeDelta;
            sizeDelta.y += this.newGameButtonHeight + this.buttonSpacing;
            this.menuContainer.sizeDelta = sizeDelta;
            
            // Điều chỉnh vị trí của container để giữ menu ở giữa
            Vector3 position = this.menuContainer.localPosition;
            position.y += (this.newGameButtonHeight + this.buttonSpacing) / 2f;
            this.menuContainer.localPosition = position;
        }
        
        // Điều chỉnh vị trí của các button khác
        this.AdjustOtherButtonsPosition(true);
        
        // Đổi tên button PLAY thành CONTINUE
        this.ChangePlayButtonText(true);
        
        Debug.Log("New Game Button shown at top and other buttons pushed down");
    }
    
    protected virtual void HideNewGameButton()
    {
        if (this.newGameButton == null) return;
        
        // Kiểm tra MapProgressManager có tồn tại không
        if (MapProgressManager.Instance == null)
        {
            Debug.LogWarning("MapProgressManager.Instance is null, cannot check tutorial completion.");
            return;
        }
        
        // Kiểm tra xem player đã hoàn thành tutorial chưa
        bool hasCompletedTutorial = MapProgressManager.Instance.IsMapCompleted(tutorialMapName);
        
        // Ẩn button
        this.newGameButton.SetActive(false);
        
        // Chỉ di chuyển button và thay đổi layout nếu player đã hoàn thành tutorial
        if (hasCompletedTutorial)
        {
            // Thu hẹp layout về ban đầu
            if (this.menuContainer != null)
            {
                // Giảm chiều cao của container
                Vector2 sizeDelta = this.menuContainer.sizeDelta;
                sizeDelta.y -= this.newGameButtonHeight + this.buttonSpacing;
                this.menuContainer.sizeDelta = sizeDelta;
                
                // Điều chỉnh vị trí của container về ban đầu
                Vector3 position = this.menuContainer.localPosition;
                position.y -= (this.newGameButtonHeight + this.buttonSpacing) / 2f;
                this.menuContainer.localPosition = position;
            }
            
            // Đưa các button khác về vị trí ban đầu
            this.AdjustOtherButtonsPosition(false);
            
            // Đổi tên button CONTINUE về PLAY
            this.ChangePlayButtonText(false);
            
            Debug.Log("New Game Button hidden and layout restored (tutorial completed)");
        }
        else
        {
            // Nếu chưa hoàn thành tutorial, chỉ ẩn button mà không di chuyển layout
            Debug.Log("New Game Button hidden without layout changes (tutorial not completed)");
        }
    }
    
    protected virtual void ChangePlayButtonText(bool toContinue)
    {
        if (this.playButton == null) return;
        
        TextMeshProUGUI buttonText = this.playButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            if (toContinue)
            {
                buttonText.text = this.continueText;
                Debug.Log($"Play button text changed to: {this.continueText}");
            }
            else
            {
                buttonText.text = this.playText;
                Debug.Log($"Play button text changed to: {this.playText}");
            }
        }
    }
    
    protected virtual void AdjustOtherButtonsPosition(bool pushDown)
    {
        // Tìm tất cả button khác (không phải New Game button)
        Button[] allButtons = GetComponentsInChildren<Button>();
        
        foreach (Button button in allButtons)
        {
            if (button.gameObject == this.newGameButton) continue; // Bỏ qua New Game button
            
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                Vector3 position = buttonRect.localPosition;
                
                if (pushDown)
                {
                    // Đẩy button xuống
                    position.y -= this.newGameButtonHeight + this.buttonSpacing;
                }
                else
                {
                    // Đưa button về vị trí ban đầu
                    position.y += this.newGameButtonHeight + this.buttonSpacing;
                }
                
                buttonRect.localPosition = position;
            }
        }
    }
    
    public void PlayGame()
    {
        Debug.Log("PlayGame() called!");
        Debug.Log($"enableCutscene: {this.enableCutscene}");
        Debug.Log($"cutsceneCanvas: {(this.cutsceneCanvas != null ? "Found" : "NULL")}");
        Debug.Log($"videoPlayer: {(this.videoPlayer != null ? "Found" : "NULL")}");
        
        // Nếu cutscene được bật nhưng thiếu components, thử tạo lại
        if (this.enableCutscene && this.cutsceneCanvas != null && this.videoPlayer == null)
        {
            Debug.Log("VideoPlayer is null, attempting to recreate...");
            this.RecreateVideoPlayer();
        }
        
        // Kiểm tra xem có bật cutscene không
        if (this.enableCutscene && this.cutsceneCanvas != null && this.videoPlayer != null)
        {
            Debug.Log("All cutscene components found! Showing cutscene...");
            // Hiển thị cutscene trước khi load map selection
            this.ShowCutscene();
        }
        else
        {
            Debug.Log("Cutscene components missing or disabled! Loading map selection directly...");
            // Load map selection scene trực tiếp nếu không có cutscene
            SceneManager.LoadScene(mapSelectionSceneName);
        }
    }
    
    protected virtual void RecreateVideoPlayer()
    {
        if (this.cutsceneCanvas == null) return;
        
        Debug.Log("Recreating VideoPlayer component...");
        
        // Xóa VideoPlayer cũ nếu có
        VideoPlayer oldVideoPlayer = this.cutsceneCanvas.GetComponent<VideoPlayer>();
        if (oldVideoPlayer != null)
        {
            DestroyImmediate(oldVideoPlayer);
        }
        
        // Tạo VideoPlayer mới
        this.videoPlayer = this.cutsceneCanvas.AddComponent<VideoPlayer>();
        if (this.videoPlayer != null)
        {
            Debug.Log("VideoPlayer recreated successfully!");
            this.SetupVideoPlayer();
        }
        else
        {
            Debug.LogError("Failed to recreate VideoPlayer!");
        }
    }

    public void NewGame()
    {
        // Hiện dialog cảnh báo thay vì reset ngay lập tức
        this.ShowConfirmationDialog();
    }
    
    protected virtual void ShowConfirmationDialog()
    {
        if (this.confirmationDialog != null)
        {
            this.confirmationDialog.SetActive(true);
            Debug.Log("Confirmation dialog shown");
        }
    }
    
    public void ConfirmNewGame()
    {
        Debug.Log("ConfirmNewGame method called!");
        
        // Kiểm tra MapProgressManager có tồn tại không
        if (MapProgressManager.Instance == null)
        {
            Debug.LogError("MapProgressManager.Instance is null! Cannot reset progress.");
            return;
        }
        
        // Reset toàn bộ tiến trình chơi về ban đầu
        MapProgressManager.Instance.ResetAllProgress();
        
        // Có thể thêm reset các dữ liệu khác ở đây nếu cần
        // Ví dụ: PlayerPrefs.DeleteAll(); // Reset tất cả PlayerPrefs
        
        Debug.Log("New Game confirmed - All progress has been reset!");
        
        // Ẩn dialog
        this.HideConfirmationDialog();
        
        // Load lại scene map selection
        SceneManager.LoadScene(mapSelectionSceneName);
    }
    
    public void CancelNewGame()
    {
        Debug.Log("CancelNewGame method called!");
        
        // Ẩn dialog và không làm gì cả
        this.HideConfirmationDialog();
        Debug.Log("New Game cancelled");
    }
    
    protected virtual void HideConfirmationDialog()
    {
        if (this.confirmationDialog != null)
        {
            this.confirmationDialog.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    // ========== CUTSCENE METHODS ==========
    
    protected virtual void ShowCutscene()
    {
        Debug.Log("ShowCutscene() called!");
        Debug.Log($"cutsceneCanvas: {(this.cutsceneCanvas != null ? "Found" : "NULL")}");
        Debug.Log($"videoPlayer: {(this.videoPlayer != null ? "Found" : "NULL")}");
        
        if (this.cutsceneCanvas == null || this.videoPlayer == null)
        {
            Debug.LogError("Cutscene components not found! Loading map selection directly.");
            SceneManager.LoadScene(mapSelectionSceneName);
            return;
        }
        
        Debug.Log("Activating cutscene canvas...");
        // Hiển thị cutscene canvas
        this.cutsceneCanvas.SetActive(true);
        
        // Ẩn menu chính
        if (this.menuContainer != null)
        {
            Debug.Log("Hiding main menu...");
            this.menuContainer.gameObject.SetActive(false);
        }
        
        // Phát video
        this.PlayCutsceneVideo();
        
        Debug.Log("Cutscene started successfully!");
    }
    
    protected virtual void PlayCutsceneVideo()
    {
        if (this.videoPlayer == null) return;
        
        // Kiểm tra xem có video clip không
        if (this.videoPlayer.clip == null)
        {
            Debug.LogWarning("No video clip assigned! Loading map selection directly.");
            this.LoadMapSelectionScene();
            return;
        }
        
        // Phát video
        this.videoPlayer.Play();
        Debug.Log("Cutscene video started playing");
    }
    
    public void SkipCutscene()
    {
        Debug.Log("Cutscene skipped by user");
        
        // Dừng video
        if (this.videoPlayer != null && this.videoPlayer.isPlaying)
        {
            this.videoPlayer.Stop();
        }
        
        // Load map selection scene
        this.LoadMapSelectionScene();
    }
    
    protected virtual void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Cutscene video finished playing");
        
        // Load map selection scene khi video kết thúc
        this.LoadMapSelectionScene();
    }
    
    protected virtual void LoadMapSelectionScene()
    {
        // Ẩn cutscene canvas
        if (this.cutsceneCanvas != null)
        {
            this.cutsceneCanvas.SetActive(false);
        }
        
        // Hiện lại menu chính (để tránh lỗi khi quay lại)
        if (this.menuContainer != null)
        {
            this.menuContainer.gameObject.SetActive(true);
        }
        
        // Load map selection scene
        SceneManager.LoadScene(mapSelectionSceneName);
        
        Debug.Log("Loading map selection scene: " + mapSelectionSceneName);
    }
}
