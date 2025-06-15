using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : SaiSingleton<InventoryUI>
{
    protected bool isShow = false;
    public bool IsShow => isShow;

    [SerializeField] protected Transform showHide;

    [SerializeField] protected BtnItemInventory defaultItemInventoryUI;
    protected List<BtnItemInventory> btnItems = new();

    protected override void Start()
    {
        base.Start();
        this.Hide();
        this.HideDefaultItemInventory();
    }

    protected virtual void FixedUpdate()
    {
        this.ItemsUpdating();
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
}
