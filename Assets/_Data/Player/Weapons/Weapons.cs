using System.Collections.Generic;
using UnityEngine;

public class Weapons : SaiMonoBehaviour
{
    [SerializeField] protected List<WeaponAbstract> weapons;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadWeapons();
    }

    protected virtual void LoadWeapons()
    {
        if(this.weapons.Count > 0) return;
        foreach(Transform child in transform)
        {
            WeaponAbstract weaponAbstract = child.GetComponent<WeaponAbstract>();
            if(weaponAbstract == null) continue;
            this.weapons.Add(weaponAbstract);
        }
        Debug.Log(transform.name + ": LoadWeapons", gameObject);
    }

    public virtual WeaponAbstract GetCurrentWeapon()
    {
        return this.weapons[0];
    }
}
