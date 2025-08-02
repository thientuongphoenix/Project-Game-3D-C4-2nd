using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string mapSelectionSceneName = "MapSelect_Hai";
    
    [Header("New Game Button")]
    [SerializeField] protected GameObject newGameButton;
    [SerializeField] protected string tutorialMapName = "SampleScene";
    
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
    [SerializeField] protected string dialogTitle = "CẢNH BÁO";
    [SerializeField] protected string dialogMessage = "Nếu bạn chọn nút này, tất cả dữ liệu sẽ bị reset.\nBạn có chắc thực hiện việc này không?";
    [SerializeField] protected string confirmText = "CÓ";
    [SerializeField] protected string cancelText = "KHÔNG";
    
    protected virtual void Start()
    {
        this.LoadNewGameButton();
        this.LoadMenuContainer();
        this.LoadPlayButton();
        this.LoadConfirmationDialog();
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
        // Load map selection scene instead of next scene
        SceneManager.LoadScene(mapSelectionSceneName);
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
}
