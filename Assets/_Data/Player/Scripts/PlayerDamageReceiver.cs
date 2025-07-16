using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerDamageReceiver : DamageReceiver
{
    [SerializeField] protected CapsuleCollider capsuleCollider;
    [SerializeField] protected PlayerCtrl playerCtrl;
    public PlayerCtrl PlayerCtrl => playerCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCapsuleCollider();
        this.LoadPlayerCtrl();
    }

    protected virtual void LoadPlayerCtrl()
    {
        if (this.playerCtrl != null) return;
        this.playerCtrl = transform.parent.GetComponent<PlayerCtrl>();
        Debug.Log(transform.name + ": LoadPlayerCtrl", gameObject);
    }

    protected virtual void LoadCapsuleCollider()
    {
        if (this.capsuleCollider != null) return;
        this.capsuleCollider = GetComponent<CapsuleCollider>();
        this.capsuleCollider.center = new Vector3(0, 0.9f, 0);
        this.capsuleCollider.radius = 0.1f;
        this.capsuleCollider.height = 0.6f;
        this.capsuleCollider.isTrigger = true;
        Debug.Log(transform.name + ": LoadCapsuleCollider", gameObject);
    }

    protected override void OnHurt()
    {
        base.OnHurt();
        ShowDamageText();
    }

    // Hàm mới để hiển thị DamageText
    protected void ShowDamageText()
    {
        var canvas = GameObject.Find("CanvasDamageText");
        if (canvas == null) return;
        var prefab = Resources.Load<GameObject>("DamageText");
        if (prefab == null) return;
        Vector3 worldPos = transform.position + Vector3.up * 1.5f;
        var go = GameObject.Instantiate(prefab, worldPos, Quaternion.identity, canvas.transform);
        var effect = go.GetComponent<_Data.UI.DamageText.DamageTextEffect>();
        if (effect != null)
        {
            effect.Play(this.lastDamage.ToString(), worldPos);
        }
    }
}
