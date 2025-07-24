using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BtnMapLock : ButttonAbstract
{
    [Header("Lock Settings")]
    [SerializeField] protected bool isLocked = false;
    [SerializeField] protected string requiredMapCompleted = ""; // Tên map cần hoàn thành để unlock
    
    [Header("Lock Visual")]
    [SerializeField] protected GameObject lockIcon;
    [SerializeField] protected Image buttonImage;
    [SerializeField] protected Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] protected Color unlockedColor = Color.white;
    
    [Header("Lock Text")]
    [SerializeField] protected TextMeshProUGUI lockText;
    
    protected override void Start()
    {
        base.Start();
        this.UpdateLockState();
    }
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadLockIcon();
        this.LoadButtonImage();
        this.LoadLockText();
    }
    
    protected virtual void LoadLockIcon()
    {
        if (this.lockIcon != null) return;
        this.lockIcon = transform.Find("LockIcon")?.gameObject;
        Debug.Log(transform.name + ": LoadLockIcon", gameObject);
    }
    
    protected virtual void LoadButtonImage()
    {
        if (this.buttonImage != null) return;
        this.buttonImage = GetComponent<Image>();
        Debug.Log(transform.name + ": LoadButtonImage", gameObject);
    }
    
    protected virtual void LoadLockText()
    {
        if (this.lockText != null) return;
        this.lockText = transform.Find("LockText")?.GetComponent<TextMeshProUGUI>();
        Debug.Log(transform.name + ": LoadLockText", gameObject);
    }
    
    protected override void OnClick()
    {
        if (this.isLocked)
        {
            this.ShowLockMessage();
            return;
        }
        
        // Nếu không bị lock, thực hiện hành động bình thường
        this.ExecuteMapAction();
    }
    
    protected virtual void ExecuteMapAction()
    {
        // Override trong các class con để thực hiện hành động cụ thể
        Debug.Log("Map button clicked - Execute action");
    }
    
    protected virtual void ShowLockMessage()
    {
        string message = $"Map is locked!\nComplete {requiredMapCompleted} to unlock";
        Debug.Log(message);
        
        if (this.lockText != null)
        {
            this.lockText.text = message;
            this.lockText.gameObject.SetActive(true);
            Invoke(nameof(HideLockMessage), 3f);
        }
    }
    
    protected virtual void HideLockMessage()
    {
        if (this.lockText != null)
        {
            this.lockText.gameObject.SetActive(false);
        }
    }
    
    public virtual void SetLocked(bool locked)
    {
        this.isLocked = locked;
        this.UpdateLockState();
    }
    
    public virtual void SetRequiredMap(string mapName)
    {
        this.requiredMapCompleted = mapName;
        this.CheckLockCondition();
    }
    
    protected virtual void CheckLockCondition()
    {
        // Kiểm tra xem map cần thiết đã được hoàn thành chưa
        if (string.IsNullOrEmpty(requiredMapCompleted))
        {
            this.isLocked = false; // Không có yêu cầu = luôn unlock
        }
        else
        {
            this.isLocked = !MapProgressManager.Instance.IsMapCompleted(requiredMapCompleted);
        }
        
        this.UpdateLockState();
    }
    
    protected virtual void UpdateLockState()
    {
        // Cập nhật trạng thái button
        if (this.button != null)
        {
            this.button.interactable = !this.isLocked;
        }
        
        // Cập nhật màu sắc
        if (this.buttonImage != null)
        {
            this.buttonImage.color = this.isLocked ? this.lockedColor : this.unlockedColor;
        }
        
        // Hiển thị/ẩn lock icon
        if (this.lockIcon != null)
        {
            this.lockIcon.SetActive(this.isLocked);
        }
        
        // Cập nhật lock text
        if (this.lockText != null)
        {
            if (this.isLocked)
            {
                this.lockText.text = $"LOCKED\nComplete {requiredMapCompleted}";
            }
            else
            {
                this.lockText.text = "";
            }
        }
    }
    
    protected virtual void Update()
    {
        // Kiểm tra điều kiện lock mỗi frame
        this.CheckLockCondition();
    }
} 