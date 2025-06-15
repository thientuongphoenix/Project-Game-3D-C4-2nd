using UnityEngine;

public class UISetting : SaiSingleton<UISetting>
{
    protected bool isShow = true;
    protected bool IsShow => isShow;

    [SerializeField] protected Transform showHide;

    protected override void Start()
    {
        base.Start();
        this.Hide();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadShowHide();
    }

    protected virtual void LoadShowHide()
    {
        if (this.showHide != null) return;
        this.showHide = transform.Find("ShowHide");
        Debug.Log(transform.name + ": LoadShowHide", gameObject);
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
        this.showHide.gameObject.SetActive(false);
        HideMouse.Instance.isCursorVisible = this.isShow;
    }

    public virtual void ToggleSetting()
    {
        if (this.isShow) this.Hide();
        else this.Show();
    }
}
