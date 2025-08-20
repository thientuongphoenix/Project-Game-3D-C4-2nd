using UnityEngine;
using Invector.vCharacterController;
using System;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerDamageReceiver : DamageReceiver
{
    [SerializeField] protected CapsuleCollider capsuleCollider;
    [SerializeField] protected PlayerCtrl playerCtrl;
    public PlayerCtrl PlayerCtrl => playerCtrl;

    // Event để thông báo khi player chết
    public event Action OnPlayerDeath;

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

    protected override void OnDead()
    {
        base.OnDead();
        EnableDeathAnimation();
        
        // Trigger death event
        OnPlayerDeath?.Invoke();
        
        //this.capsuleCollider.enabled = false;
    }

    protected virtual void EnableDeathAnimation()
    {
        if (this.playerCtrl == null) return;
        if (this.playerCtrl.Animator == null) return;
        
        // Kích hoạt animation death
        this.playerCtrl.Animator.SetBool("IsDead", true);
        
        // Disable movement khi chết
        DisablePlayerMovement();
        
        Debug.Log(transform.name + ": Death animation activated", gameObject);
    }

    protected virtual void DisablePlayerMovement()
    {
        if (this.playerCtrl == null) return;
        if (this.playerCtrl.ThirdPersonController == null) return;
        
        // Disable input và movement
        var input = this.playerCtrl.ThirdPersonController.GetComponent<vThirdPersonInput>();
        if (input != null)
            input.enabled = false;
            
        // Disable controller
        this.playerCtrl.ThirdPersonController.enabled = false;
        
        Debug.Log(transform.name + ": Player movement disabled", gameObject);
    }

    // Hàm để hồi sinh player (có thể gọi từ GameManager hoặc UI)
    public virtual void ResurrectPlayer()
    {
        if (!this.IsDead()) return;
        
        // Reset HP
        this.currentHP = this.maxHP;
        this.isDead = false;
        
        // Tắt animation death
        if (this.playerCtrl != null && this.playerCtrl.Animator != null)
            this.playerCtrl.Animator.SetBool("IsDead", false);
        
        // Enable lại movement
        EnablePlayerMovement();
        
        // Enable lại components
        if (this.playerCtrl != null)
            this.playerCtrl.EnablePlayerComponents();
        
        Debug.Log(transform.name + ": Player resurrected", gameObject);
    }

    protected virtual void EnablePlayerMovement()
    {
        if (this.playerCtrl == null) return;
        if (this.playerCtrl.ThirdPersonController == null) return;
        
        // Enable input và movement
        var input = this.playerCtrl.ThirdPersonController.GetComponent<vThirdPersonInput>();
        if (input != null)
            input.enabled = true;
            
        // Enable controller
        this.playerCtrl.ThirdPersonController.enabled = true;
        
        Debug.Log(transform.name + ": Player movement enabled", gameObject);
    }
}
