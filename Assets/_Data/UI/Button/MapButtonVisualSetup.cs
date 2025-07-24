using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script để tự động setup visual cho Map Button Lock
/// Attach script này vào button và click "Setup Visual" trong Inspector
/// </summary>
public class MapButtonVisualSetup : MonoBehaviour
{
    [Header("Setup Settings")]
    [SerializeField] protected bool autoSetupOnStart = false;
    
    [Header("Lock Icon Settings")]
    [SerializeField] protected Sprite lockIconSprite;
    [SerializeField] protected Color lockIconColor = Color.red;
    [SerializeField] protected Vector2 lockIconSize = new Vector2(32, 32);
    
    [Header("Lock Text Settings")]
    [SerializeField] protected int lockTextSize = 12;
    [SerializeField] protected Color lockTextColor = Color.red;
    [SerializeField] protected Vector2 lockTextOffset = new Vector2(0, -40);
    
    protected virtual void Start()
    {
        if (autoSetupOnStart)
        {
            SetupVisual();
        }
    }
    
    [ContextMenu("Setup Visual")]
    public virtual void SetupVisual()
    {
        // Tạo LockIcon nếu chưa có
        if (transform.Find("LockIcon") == null)
        {
            CreateLockIcon();
        }
        
        // Tạo LockText nếu chưa có
        if (transform.Find("LockText") == null)
        {
            CreateLockText();
        }
        
        // Setup references trong script
        SetupScriptReferences();
        
        Debug.Log($"Visual setup completed for {gameObject.name}");
    }
    
    protected virtual void CreateLockIcon()
    {
        // Tạo GameObject LockIcon
        GameObject lockIcon = new GameObject("LockIcon");
        lockIcon.transform.SetParent(transform, false);
        
        // Thêm Image component
        Image iconImage = lockIcon.AddComponent<Image>();
        
        // Set sprite (nếu có)
        if (lockIconSprite != null)
        {
            iconImage.sprite = lockIconSprite;
        }
        else
        {
            // Tạo sprite đơn giản nếu không có
            iconImage.sprite = CreateDefaultLockSprite();
        }
        
        iconImage.color = lockIconColor;
        
        // Setup RectTransform
        RectTransform rectTransform = lockIcon.GetComponent<RectTransform>();
        rectTransform.sizeDelta = lockIconSize;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        
        // Ẩn mặc định
        lockIcon.SetActive(false);
        
        Debug.Log("Created LockIcon");
    }
    
    protected virtual void CreateLockText()
    {
        // Tạo GameObject LockText
        GameObject lockText = new GameObject("LockText");
        lockText.transform.SetParent(transform, false);
        
        // Thêm TextMeshPro component
        TextMeshProUGUI textComponent = lockText.AddComponent<TextMeshProUGUI>();
        textComponent.text = "LOCKED";
        textComponent.fontSize = lockTextSize;
        textComponent.color = lockTextColor;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        // Setup RectTransform
        RectTransform rectTransform = lockText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = lockTextOffset;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        
        // Ẩn mặc định
        lockText.SetActive(false);
        
        Debug.Log("Created LockText");
    }
    
    protected virtual void SetupScriptReferences()
    {
        // Tìm script BtnMapLock
        BtnMapLock mapLock = GetComponent<BtnMapLock>();
        if (mapLock == null)
        {
            Debug.LogWarning("No BtnMapLock script found on this GameObject!");
            return;
        }
        
        // Tìm LockIcon và LockText
        GameObject lockIcon = transform.Find("LockIcon")?.gameObject;
        TextMeshProUGUI lockText = transform.Find("LockText")?.GetComponent<TextMeshProUGUI>();
        Image buttonImage = GetComponent<Image>();
        
        // Set references bằng reflection
        var lockIconField = typeof(BtnMapLock).GetField("lockIcon", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lockTextField = typeof(BtnMapLock).GetField("lockText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var buttonImageField = typeof(BtnMapLock).GetField("buttonImage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (lockIconField != null && lockIcon != null)
            lockIconField.SetValue(mapLock, lockIcon);
        
        if (lockTextField != null && lockText != null)
            lockTextField.SetValue(mapLock, lockText);
        
        if (buttonImageField != null && buttonImage != null)
            buttonImageField.SetValue(mapLock, buttonImage);
        
        Debug.Log("Script references setup completed");
    }
    
    protected virtual Sprite CreateDefaultLockSprite()
    {
        // Tạo sprite đơn giản cho lock icon
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        // Tạo hình lock đơn giản
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                // Tạo hình lock đơn giản
                bool isLock = (x >= 8 && x <= 23 && y >= 8 && y <= 23) || // Body
                             (x >= 12 && x <= 19 && y >= 4 && y <= 8) ||  // Top
                             (x >= 14 && x <= 17 && y >= 0 && y <= 4);    // Hole
                
                pixels[y * 32 + x] = isLock ? Color.white : Color.clear;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
    
    [ContextMenu("Remove Visual")]
    public virtual void RemoveVisual()
    {
        // Xóa LockIcon
        Transform lockIcon = transform.Find("LockIcon");
        if (lockIcon != null)
        {
            DestroyImmediate(lockIcon.gameObject);
        }
        
        // Xóa LockText
        Transform lockText = transform.Find("LockText");
        if (lockText != null)
        {
            DestroyImmediate(lockText.gameObject);
        }
        
        Debug.Log("Visual elements removed");
    }
} 