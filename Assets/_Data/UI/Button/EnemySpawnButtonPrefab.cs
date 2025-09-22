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
    
    protected override void Awake()
    {
        base.Awake();
        this.SetupButton();
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
}
