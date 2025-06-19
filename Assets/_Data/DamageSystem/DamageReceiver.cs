using UnityEngine;

public abstract class DamageReceiver : SaiMonoBehaviour
{
    [SerializeField] protected int maxHP = 10;
    public int MaxHP => maxHP;

    [SerializeField] protected int currentHP = 10;
    public int CurrentHp => currentHP;
    
    protected bool isDead = false;
    [SerializeField] protected bool isImmotal = false;

    protected virtual void OnEnable()
    {
        this.OnReborn();
    }

    public virtual int Deduct(int hp)
    {
        if(!isImmotal) this.currentHP -= hp;
        if(this.IsDead())
        {
            this.OnDead();
        }
        else
        {
            this.OnHurt();
        }

        if(this.currentHP < 0) this.currentHP = 0;
        return this.currentHP;
    }

    public virtual bool IsDead()
    {
        return this.isDead = this.currentHP <= 0;
    }

    protected virtual void OnDead()
    {
        // For Override
    }

    protected virtual void OnHurt()
    {
        // For Override
    }

    protected virtual void OnReborn()
    {
        this.currentHP = this.maxHP;
    }

    public virtual void Heal(int amount)
    {
        if (this.isDead) return;
        this.currentHP += amount;
        if (this.currentHP > this.maxHP) this.currentHP = this.maxHP;
    }
}
