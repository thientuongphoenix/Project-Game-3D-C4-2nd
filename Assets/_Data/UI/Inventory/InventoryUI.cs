using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : SaiSingleton<InventoryUI>
{
    protected bool isShow = false;
    public bool IsShow => isShow;

    [SerializeField] protected Transform showHide;

    [SerializeField] protected BtnItemInventory defaultItemInventoryUI;
    protected List<BtnItemInventory> btnItems = new();

    [Header("Selected Item")]
    [SerializeField] protected BtnItemInventory selectedItem;
    public BtnItemInventory SelectedItem => selectedItem;

    [Header("Action Buttons")]
    [SerializeField] protected BtnUseItem btnUseItem;

    protected override void Start()
    {
        base.Start();
        this.Hide();
        this.HideDefaultItemInventory();
    }

    // protected virtual void Update()
    // {
    //     this.UpdateActionButtons();
    // }

    protected virtual void FixedUpdate()
    {
        this.ItemsUpdating();
        this.UpdateActionButtons();
    }

    protected virtual void LateUpdate()
    {
        this.HotkeyToggleInventory();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadBtnItemInventory();
        this.LoadShowHide();
        this.LoadBtnUseItem();
    }

    protected virtual void LoadBtnUseItem()
    {
        if (this.btnUseItem != null) return;
        this.btnUseItem = GetComponentInChildren<BtnUseItem>();
        Debug.Log(transform.name + ": LoadBtnUseItem", gameObject);
    }

    protected virtual void LoadShowHide()
    {
        if (this.showHide != null) return;
        this.showHide = transform.Find("ShowHide");
        Debug.Log(transform.name + ": LoadShowHide", gameObject);
    }

    protected virtual void LoadBtnItemInventory()
    {
        if (this.defaultItemInventoryUI != null) return;
        this.defaultItemInventoryUI = GetComponentInChildren<BtnItemInventory>();
        Debug.Log(transform.name + ": LoadBtnItemInventory", gameObject);
    }

    protected virtual void UpdateActionButtons()
    {
        if (this.btnUseItem != null)
        {
            bool canUseItem = this.selectedItem != null;
            this.btnUseItem.gameObject.SetActive(canUseItem);
            Button btnComponent = this.btnUseItem.GetComponent<Button>();
            if (btnComponent != null)
                btnComponent.interactable = canUseItem;
        }
    }

    public virtual void Show()
    {
        this.isShow = true;
        this.showHide.gameObject.SetActive(this.isShow);
        HideMouse.Instance.isCursorVisible = this.isShow;
    }

    public virtual void Hide()
    {
        this.isShow = false;
        this.showHide.gameObject.SetActive(this.isShow);
        HideMouse.Instance.isCursorVisible = this.isShow;
        // Khi đóng inventory, bỏ chọn item đang chọn
        if (this.selectedItem != null)
        {
            this.selectedItem.Deselect();
            this.selectedItem = null;
        }
    }

    public virtual void Toggle()
    {
        if(this.isShow) this.Hide();
        else this.Show();
    }

    protected virtual void HideDefaultItemInventory()
    {
        this.defaultItemInventoryUI.gameObject.SetActive(false);
    }

    protected virtual void ItemsUpdating()
    {
        if(!this.isShow) return;
        //Debug.Log("UI Updating");

        InventoryCtrl itemInvCtrl = InventoryManager.Instance.Items();

        //if(itemInvCtrl.Items.Count > 20) return;
        foreach(ItemInventory itemInventory in itemInvCtrl.Items)
        {
            BtnItemInventory newBtnItem = this.GetExistItem(itemInventory);
            if(newBtnItem == null)
            {
                newBtnItem = Instantiate(this.defaultItemInventoryUI);
                newBtnItem.transform.SetParent(this.defaultItemInventoryUI.transform.parent);
                newBtnItem.SetItem(itemInventory);
                newBtnItem.transform.localScale = new Vector3(1, 1, 1);
                newBtnItem.gameObject.SetActive(true);
                newBtnItem.name = itemInventory.GetItemName() + "-" + itemInventory.ItemID;
                this.btnItems.Add(newBtnItem);
            }
        }
    }

    protected virtual BtnItemInventory GetExistItem(ItemInventory itemInventory)
    {
        foreach(BtnItemInventory itemInvUI in this.btnItems)
        {
            if(itemInvUI.ItemInventory.ItemID == itemInventory.ItemID) return itemInvUI;
        }
        return null;
    }

    protected virtual void HotkeyToggleInventory()
    {
        if(InputHotkeys.Instance.IsToggleInventoryUI) this.Toggle();
    }

    public virtual void OnItemSelected(BtnItemInventory selectedButton)
    {
        // Nếu click vào item đang được chọn, bỏ chọn nó
        if (this.selectedItem == selectedButton)
        {
            this.DeselectCurrentItem();
            return;
        }

        // Nếu có item khác đang được chọn, bỏ chọn nó trước
        if (this.selectedItem != null)
        {
            this.selectedItem.Deselect();
        }

        // Chọn item mới
        this.selectedItem = selectedButton;
        Debug.Log($"Selected item in inventory: {selectedButton.ItemInventory.GetItemName()}");
    }

    public virtual void DeselectCurrentItem()
    {
        if (this.selectedItem != null)
        {
            this.selectedItem.Deselect();
            this.selectedItem = null;
            Debug.Log("Deselected current item");
        }
    }
}
