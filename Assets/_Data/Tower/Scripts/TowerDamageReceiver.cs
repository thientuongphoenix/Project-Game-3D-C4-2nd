using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TowerDamageReceiver : DamageReceiver
{
    [SerializeField] protected SphereCollider sphereCollider;
    [SerializeField] protected TowerCtrl towerCtrl;
    
    // Biến lưu máu cơ bản và máu tăng thêm do level
    [SerializeField] protected int baseMaxHP = 10; // Máu cơ bản ban đầu
    [SerializeField] protected int levelBonusHP = 0; // Máu tăng thêm do level

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadSphereCollider();
        this.LoadTowerCtrl();
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        // Khởi tạo baseMaxHP nếu chưa có
        if (this.baseMaxHP <= 0)
        {
            this.baseMaxHP = this.maxHP;
        }
        // Reset máu về cơ bản khi enable reuse
        this.ResetToBaseHP();
    }

    protected virtual void LoadTowerCtrl()
    {
        if(this.towerCtrl != null) return;
        this.towerCtrl = transform.parent.GetComponent<TowerCtrl>();
        Debug.Log(transform.name + ": LoadTowerCtrl", gameObject);
    }

    protected virtual void LoadSphereCollider()
    {
        if(this.sphereCollider != null) return;
        this.sphereCollider = GetComponent<SphereCollider>();
        this.sphereCollider.radius = 0.5f;
        this.sphereCollider.isTrigger = true;
        Debug.Log(transform.name + ": LoadSphereCollider", gameObject);
    }

    protected override void OnDead()
    {
        base.OnDead();
        //this.enemyCtrl.Animator.SetBool("isDead", this.isDead);
        this.sphereCollider.enabled = false;
        //this.RewardOnDead();
        Invoke(nameof(this.Disappear), 1f);
    }

    protected override void OnHurt()
    {
        base.OnHurt();
        ShowDamageText();
    }

    // Hàm mới để hiển thị DamageText
    protected void ShowDamageText()
    {
        var canvas = GameObject.Find("DamageTextCanvas");
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

    protected virtual void Disappear()
    {
        this.towerCtrl.TowerDespawn.DoDespawn();
        //this.enemyCtrl.Despawn.DoDespawn();
    }

    protected override void OnReborn()
    {
        base.OnReborn();
        this.sphereCollider.enabled = true;
    }
    
    // Method để tăng máu khi lên cấp (chỉ cho MachineGun)
    public virtual void OnLevelUp()
    {
        // Kiểm tra nếu là MachineGun tower
        if (this.towerCtrl == null) return;
        
        // Lấy TowerCode từ tên object (vì TowerCtrl không có property TowerCode trực tiếp)
        string towerName = this.towerCtrl.name.ToLower();
        if (!towerName.Contains("machinegun")) return;
        
        // Tính toán máu tăng thêm (20% máu cơ bản)
        int hpIncrease = Mathf.RoundToInt(this.baseMaxHP * 0.2f);
        this.levelBonusHP += hpIncrease;
        
        // Cập nhật maxHP
        this.maxHP = this.baseMaxHP + this.levelBonusHP;
        
        // Hồi 20% máu hiện tại
        int healAmount = Mathf.RoundToInt(this.currentHP * 0.2f);
        this.Heal(healAmount);
        
        Debug.Log($"{transform.name}: Lên cấp! Máu tăng thêm: {hpIncrease}, Máu hiện tại: {this.currentHP}/{this.maxHP}");
    }
    
    // Method để reset máu về cơ bản khi enable reuse
    public virtual void ResetToBaseHP()
    {
        // Chỉ reset bonus máu, giữ nguyên baseMaxHP
        this.levelBonusHP = 0; // Reset bonus máu
        this.maxHP = this.baseMaxHP;
        this.currentHP = this.maxHP;
        
        Debug.Log($"{transform.name}: Reset máu về cơ bản: {this.maxHP}");
    }
}
