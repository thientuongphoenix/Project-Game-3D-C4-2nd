using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Prefab script cho EnemySpawnButton để dễ setup trong Unity
/// </summary>
public class EnemySpawnButtonPrefab : SaiMonoBehaviour
{
    [Header("Prefab Setup")]
    [SerializeField] protected Button button;
    [SerializeField] protected TextMeshProUGUI buttonText;
    [SerializeField] protected TextMeshProUGUI statusText;
    
    // Thêm Canvas để kiểm soát sorting order
    protected Canvas buttonCanvas;
    
    protected override void Awake()
    {
        base.Awake();
        this.SetupButton();
        this.SetupCanvasSorting();
    }
    
    protected virtual void SetupButton()
    {
        // Tự động tìm và gán references
        if (button == null)
            button = GetComponent<Button>();
            
        if (buttonText == null)
            buttonText = transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
            
        if (statusText == null)
        {
            // Tìm status text trong parent hoặc tạo mới
            Transform statusParent = transform.parent;
            if (statusParent != null)
            {
                Transform statusTransform = statusParent.Find("StatusText");
                if (statusTransform != null)
                {
                    statusText = statusTransform.GetComponent<TextMeshProUGUI>();
                }
                else
                {
                    // Tạo status text nếu không có
                    GameObject statusObj = new GameObject("StatusText");
                    statusObj.transform.SetParent(statusParent);
                    statusText = statusObj.AddComponent<TextMeshProUGUI>();
                    statusText.text = "Ready to start waves";
                    statusText.fontSize = 14;
                    statusText.color = Color.white;
                    
                    // Position status text
                    RectTransform rectTransform = statusObj.AddComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    rectTransform.offsetMin = new Vector2(10, 10);
                    rectTransform.offsetMax = new Vector2(-10, 40);
                }
            }
        }
        
        // Gán EnemySpawnButton component
        EnemySpawnButton spawnButton = GetComponent<EnemySpawnButton>();
        if (spawnButton == null)
        {
            spawnButton = gameObject.AddComponent<EnemySpawnButton>();
        }
        
        // Gán references cho EnemySpawnButton
        var spawnButtonType = typeof(EnemySpawnButton);
        var buttonField = spawnButtonType.GetField("spawnButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var buttonTextField = spawnButtonType.GetField("buttonText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var statusTextField = spawnButtonType.GetField("statusText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (buttonField != null) buttonField.SetValue(spawnButton, button);
        if (buttonTextField != null) buttonTextField.SetValue(spawnButton, buttonText);
        if (statusTextField != null) statusTextField.SetValue(spawnButton, statusText);
        
        // Gán OnClick event
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => spawnButton.OnSpawnButtonClicked());
        }
        
        Debug.Log("EnemySpawnButton prefab setup completed!");
    }
    
    /// <summary>
    /// Thiết lập Canvas sorting order để đảm bảo button không đè lên win/lose panel
    /// </summary>
    protected virtual void SetupCanvasSorting()
    {
        // Tìm Canvas của button
        buttonCanvas = GetComponentInParent<Canvas>();
        if (buttonCanvas == null)
        {
            // Tạo Canvas mới nếu không có
            GameObject canvasObj = new GameObject("EnemySpawnCanvas");
            canvasObj.transform.SetParent(transform.parent);
            buttonCanvas = canvasObj.AddComponent<Canvas>();
            buttonCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            buttonCanvas.sortingOrder = 1; // Thấp hơn win/lose panel
            
            // Thêm CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Thêm GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Di chuyển button vào Canvas mới
            transform.SetParent(canvasObj.transform);
        }
        else
        {
            // Đảm bảo Canvas có sorting order thấp
            if (buttonCanvas.sortingOrder >= 10)
            {
                buttonCanvas.sortingOrder = 1;
            }
        }
    }
    
    /// <summary>
    /// Ẩn button khi game kết thúc (win/lose)
    /// </summary>
    protected virtual void Update()
    {
        if (GameResultManager.Instance != null && GameResultManager.Instance.IsGameEnded())
        {
            // Ẩn button khi game kết thúc
            if (button != null) button.gameObject.SetActive(false);
            if (statusText != null) statusText.gameObject.SetActive(false);
        }
        else
        {
            // Hiện lại button khi game chưa kết thúc
            if (button != null) button.gameObject.SetActive(true);
            if (statusText != null) statusText.gameObject.SetActive(true);
        }
    }
}

