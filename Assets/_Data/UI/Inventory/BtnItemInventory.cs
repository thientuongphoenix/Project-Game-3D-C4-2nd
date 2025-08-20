using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnItemInventory : ButttonAbstract
{
    [SerializeField] protected TextMeshProUGUI txtItemName;
    [SerializeField] protected TextMeshProUGUI txtItemCount;

    [SerializeField] protected ItemInventory itemInventory;
    public ItemInventory ItemInventory => itemInventory;

    [Header("Selected State")]
    [SerializeField] protected bool isSelected = false;
    public bool IsSelected => isSelected;

    [Header("Visual")]
    [SerializeField] protected Image buttonImage;
    protected Color defaultColor;
    protected Color selectedColor = Color.gray;

    protected override void Start()
    {
        base.Start();
        this.LoadDefaultColor();
    }

    protected virtual void LoadDefaultColor()
    {
        if (this.buttonImage != null)
            this.defaultColor = this.buttonImage.color;
    }

    protected virtual void FixedUpdate()
    {
        this.ItemUpdating();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadItemName();
        this.LoadItemCount();
        this.LoadButtonImage();
    }

    protected virtual void LoadButtonImage()
    {
        if (this.buttonImage != null) return;
        this.buttonImage = GetComponent<Image>();
        Debug.Log(transform.name + ": LoadButtonImage", gameObject);
    }

    protected virtual void LoadItemName()
    {
        if(this.txtItemName != null) return;
        this.txtItemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        Debug.Log(transform.name + ": LoadItemName", gameObject);
    }

    protected virtual void LoadItemCount()
    {
        if(this.txtItemCount != null) return;
        this.txtItemCount = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
        Debug.Log(transform.name + ": LoadItemCount", gameObject);
    }

    public virtual void SetItem(ItemInventory itemInventory)
    {
        this.itemInventory = itemInventory;
        // Lấy sprite từ ItemProfileSO và set cho buttonImage
        if (this.buttonImage != null && this.itemInventory != null && this.itemInventory.ItemProfile != null)
        {
            Sprite sprite = this.itemInventory.ItemProfile.itemSprite;
            if (sprite != null)
            {
                this.buttonImage.sprite = sprite;
            }
        }
    }

    protected override void OnClick()
    {
        // Bỏ chọn tất cả các item khác
        BtnItemInventory[] allItems = transform.parent.GetComponentsInChildren<BtnItemInventory>();
        foreach (BtnItemInventory item in allItems)
        {
            if (item != this) item.Deselect();
        }

        // Toggle trạng thái chọn của item hiện tại
        this.ToggleSelect();
        
        Debug.Log($"Item {itemInventory.GetItemName()} is {(isSelected ? "selected" : "deselected")}");
    }

    public virtual void Select()
    {
        isSelected = true;
        if (this.buttonImage != null)
            this.buttonImage.color = this.selectedColor;
        // Thông báo cho InventoryUI biết item nào đang được chọn
        InventoryUI.Instance.OnItemSelected(this);
    }

    public virtual void Deselect()
    {
        isSelected = false;
        if (this.buttonImage != null)
            this.buttonImage.color = this.defaultColor;
    }

    public virtual void ToggleSelect()
    {
        if (isSelected) {
            Deselect();
            InventoryUI.Instance.DeselectCurrentItem();
        }
        else Select();
    }

    protected virtual void ItemUpdating()
    {
        //Debug.Log("BtnItemUpdating");
        this.txtItemName.text = this.itemInventory.GetItemName();
        this.txtItemCount.text = this.itemInventory.itemCount.ToString();

        if(this.itemInventory.itemCount == 0) Destroy(gameObject);
    }
}
