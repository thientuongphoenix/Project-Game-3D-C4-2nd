using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TowerInfoUI : SaiSingleton<TowerInfoUI>
{
    [Header("UI References")]
    [SerializeField] protected GameObject infoPanel;
    [SerializeField] protected Image towerIcon;
    [SerializeField] protected TextMeshProUGUI towerNameText;
    [SerializeField] protected TextMeshProUGUI descriptionText;
    [SerializeField] protected TextMeshProUGUI priceText;
    [SerializeField] protected TextMeshProUGUI abilitiesText;
    
    [Header("Animation")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected float fadeInTime = 0.2f;
    [SerializeField] protected float fadeOutTime = 0.1f;
    protected Coroutine fadeCoroutine;
    
    protected TowerInfoData currentTowerInfo;
    protected bool isShowing = false;
    
    protected override void Start()
    {
        base.Start();
        this.Hide();
    }
    
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadUIReferences();
        this.LoadCanvasGroup();
    }
    
    protected virtual void LoadUIReferences()
    {
        if (this.infoPanel != null) return;
        this.infoPanel = transform.Find("InfoPanel")?.gameObject;
        Debug.Log(transform.name + ": LoadUIReferences", gameObject);
    }
    
    protected virtual void LoadCanvasGroup()
    {
        if (this.canvasGroup != null) return;
        this.canvasGroup = GetComponent<CanvasGroup>();
        if (this.canvasGroup == null)
            this.canvasGroup = gameObject.AddComponent<CanvasGroup>();
        Debug.Log(transform.name + ": LoadCanvasGroup", gameObject);
    }
    
    public virtual void ShowTowerInfo(TowerInfoData towerInfo, Vector3 screenPosition)
    {
        Debug.Log($"TowerInfoUI.ShowTowerInfo được gọi với: {towerInfo?.towerName}");
        
        if (towerInfo == null) 
        {
            Debug.LogError("towerInfo là null!");
            return;
        }
        
        this.currentTowerInfo = towerInfo;
        Debug.Log("Cập nhật UI...");
        this.UpdateUI();
        Debug.Log("Đặt vị trí...");
        this.SetScreenPosition(screenPosition);
        Debug.Log("Hiển thị UI...");
        this.Show();
    }
    
    public virtual void HideTowerInfo()
    {
        this.Hide();
    }
    
    protected virtual void UpdateUI()
    {
        Debug.Log("UpdateUI được gọi");
        
        if (this.currentTowerInfo == null) 
        {
            Debug.LogError("currentTowerInfo là null trong UpdateUI!");
            return;
        }
        
        Debug.Log($"Cập nhật UI cho: {this.currentTowerInfo.towerName}");
        
        // Update basic info
        if (this.towerIcon != null)
        {
            this.towerIcon.sprite = this.currentTowerInfo.icon;
            Debug.Log("Đã cập nhật icon");
        }
        else Debug.LogWarning("towerIcon là null!");
            
        if (this.towerNameText != null)
        {
            this.towerNameText.text = this.currentTowerInfo.towerName;
            Debug.Log($"Đã cập nhật tên: {this.currentTowerInfo.towerName}");
        }
        else Debug.LogWarning("towerNameText là null!");
            
        if (this.descriptionText != null)
        {
            this.descriptionText.text = this.currentTowerInfo.description;
            Debug.Log("Đã cập nhật mô tả");
        }
        else Debug.LogWarning("descriptionText là null!");
            
        if (this.priceText != null)
        {
            this.priceText.text = $"Cost: {this.currentTowerInfo.basePrice} Gold";
            Debug.Log($"Đã cập nhật giá: {this.currentTowerInfo.basePrice}");
        }
        else Debug.LogWarning("priceText là null!");
        
        // Update abilities
        if (this.abilitiesText != null)
        {
            string abilities = "";
            if (this.currentTowerInfo.specialAbilities.Length > 0)
            {
                abilities += "Special Abilities:\n";
                foreach (string ability in this.currentTowerInfo.specialAbilities)
                {
                    abilities += $"• {ability}\n";
                }
            }
            
            this.abilitiesText.text = abilities;
            Debug.Log("Đã cập nhật khả năng");
        }
        else Debug.LogWarning("abilitiesText là null!");
        
        Debug.Log("UpdateUI hoàn thành");
    }
    
    protected virtual void SetScreenPosition(Vector3 screenPosition)
    {
        // Adjust position to keep UI on screen
        RectTransform rectTransform = this.infoPanel.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 size = rectTransform.sizeDelta;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            
            // Adjust X position
            if (screenPosition.x + size.x > screenSize.x)
                screenPosition.x = screenSize.x - size.x;
            if (screenPosition.x < 0)
                screenPosition.x = 0;
                
            // Adjust Y position
            if (screenPosition.y + size.y > screenSize.y)
                screenPosition.y = screenSize.y - size.y;
            if (screenPosition.y < 0)
                screenPosition.y = 0;
        }
        
        this.infoPanel.transform.position = screenPosition;
    }
    
    public virtual void Show()
    {
        if (this.isShowing) return;
        
        this.isShowing = true;
        this.infoPanel.SetActive(true);
        
        // Fade in animation
        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 0f;
            if (this.fadeCoroutine != null)
                StopCoroutine(this.fadeCoroutine);
            this.fadeCoroutine = StartCoroutine(this.FadeIn());
        }
    }
    
    public virtual void Hide()
    {
        if (!this.isShowing) return;
        
        this.isShowing = false;
        
        // Fade out animation
        if (this.canvasGroup != null)
        {
            if (this.fadeCoroutine != null)
                StopCoroutine(this.fadeCoroutine);
            this.fadeCoroutine = StartCoroutine(this.FadeOut());
        }
        else
        {
            this.infoPanel.SetActive(false);
        }
    }
    
    protected virtual IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        float startAlpha = 0f;
        float targetAlpha = 1f;
        
        while (elapsedTime < this.fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / this.fadeInTime;
            this.canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            yield return null;
        }
        
        this.canvasGroup.alpha = targetAlpha;
        this.fadeCoroutine = null;
    }
    
    protected virtual IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startAlpha = 1f;
        float targetAlpha = 0f;
        
        while (elapsedTime < this.fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / this.fadeOutTime;
            this.canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            yield return null;
        }
        
        this.canvasGroup.alpha = targetAlpha;
        this.infoPanel.SetActive(false);
        this.fadeCoroutine = null;
    }
}
