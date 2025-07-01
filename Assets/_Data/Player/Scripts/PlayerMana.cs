using UnityEngine;

public class PlayerMana : SaiMonoBehaviour
{
    [SerializeField] protected PlayerCtrl playerCtrl;

    [SerializeField] protected float currentMana;
    public float CurrentMana { get => currentMana; set => currentMana = value; }
    [SerializeField] protected float maxMana;
    public float MaxMana => maxMana;

    protected override void Start()
    {
        base.Start();
        this.currentMana = this.maxMana;
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPlayerCtrl();
    }

    protected virtual void LoadPlayerCtrl()
    {
        if (this.playerCtrl != null) return;
        this.playerCtrl = GetComponentInParent<PlayerCtrl>();
        Debug.Log(transform.name + ": LoadPlayerCtrl", gameObject);
    }

    /// <summary>
    /// Tiêu hao mana, trả về true nếu đủ mana và đã trừ, false nếu không đủ.
    /// </summary>
    public bool UseMana(float amount)
    {
        if (this.currentMana < amount) return false;
        this.currentMana -= amount;
        if (this.currentMana < 0) this.currentMana = 0;
        return true;
    }

    /// <summary>
    /// Bơm thêm mana, không vượt quá maxMana.
    /// </summary>
    public void AddMana(float amount)
    {
        this.currentMana += amount;
        if (this.currentMana > this.maxMana) this.currentMana = this.maxMana;
    }
}
