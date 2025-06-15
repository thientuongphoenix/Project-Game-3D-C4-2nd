using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemsDropManager : SaiSingleton<ItemsDropManager>
{
    [SerializeField] protected ItemsDropSpawner spawner;
    public ItemsDropSpawner Spawner => spawner;

    protected float spawnHeight = 1.0f;
    protected float forceAmount = 5.0f;

    protected override void Start()
    {
        base.Start();
        //this.Drop(ItemCode.Gold, 10);
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadSpawner();
    }

    protected virtual void LoadSpawner()
    {
        if (this.spawner != null) return;
        this.spawner = GetComponent<ItemsDropSpawner>();
        Debug.Log(transform.name + ": LoadSpawner", gameObject);
    }

    public virtual void DropMany(ItemCode itemCode, int dropCount, Vector3 dropPosition)
    {
        for (int i = 0; i < dropCount; i++)
        {
            this.Drop(itemCode, 1, dropPosition);
        }
    }

    public virtual void Drop(ItemCode itemCode, int dropCount, Vector3 dropPosition)
    {
        Vector3 spawnPosition = dropPosition + new Vector3(Random.Range(-0.5f, 0.5f), spawnHeight, Random.Range(-0.5f, 0.5f));
        ItemDropCtrl itemPrefab = this.spawner.PoolPrefabs.GetByName(itemCode.ToString());
        if (itemPrefab == null) itemPrefab = this.spawner.PoolPrefabs.GetByName("DefaultDrop");

        ItemDropCtrl newItem = this.spawner.Spawn(itemPrefab, spawnPosition);
        //newItem.SetValue(itemCode, dropCount, InvCodeName.Currency); //Chỗ này cần xử lý định hướng túi nào khi nhặt item chứ không thể nào để InvCodeName.Monies được
        ItemProfileSO itemProfile = InventoryManager.Instance.GetProfileByCode(itemCode);
        newItem.SetValue(itemCode, dropCount, itemProfile.invCodeName);

        newItem.gameObject.SetActive(true);

        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Abs(randomDirection.y);
        newItem.Rigidbody.AddForce(randomDirection * forceAmount, ForceMode.Impulse);
    }

    public void DropItemWithAutoPickupCheck(ItemCode itemCode, int amount, Vector3 position, InventoryCtrl targetInventory = null)
    {
        ItemProfileSO profile = InventoryManager.Instance.GetProfileByCode(itemCode);
        if (profile == null) return;

        if (profile.isAutoPickup)
        {
            if (targetInventory == null) targetInventory = InventoryManager.Instance.GetByCodeName(profile.invCodeName);
            ItemInventory item = new ItemInventory(profile, amount);
            targetInventory.AddItem(item);
        }
        else
        {
            // Nếu không phải auto-pickup thì spawn item drop ra ngoài thế giới
            this.DropMany(itemCode, amount, position);
        }
    }
}
