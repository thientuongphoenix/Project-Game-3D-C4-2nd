using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class EnemyTargeting : SaiMonoBehaviour
{
    [SerializeField] protected SphereCollider sphereCollider;
    [SerializeField] protected Rigidbody rigid;

    [SerializeField] protected List<TowerCtrl> towers = new();
    public List<TowerCtrl> Towers => towers;

    [SerializeField] protected PlayerCtrl player;
    public PlayerCtrl Player => player;

    [SerializeField] protected LayerMask obstacleLayerMask;

    [SerializeField] protected TowerCtrl nearestTower;
    public TowerCtrl NearestTower => nearestTower;

    protected virtual void OnEnable()
    {
        this.OnReborn();
    }

    protected virtual void FixedUpdate()
    {
        if(this.towers.Count == 0)
        {
            this.nearestTower = null;
            return;
        }
        this.RemoveDeadTower();
        this.FindNearestTower();
        //this.FindPlayer();
        //this.RemoveDeadTower();
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        this.AddTower(collider);
        this.AddPlayer(collider);
    }

    protected virtual void OnTriggerExit(Collider collider)
    {
        this.RemoveTower(collider);
        this.RemovePlayer(collider);
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadSphereCollider();
        this.LoadRigidbody();
    }

    protected virtual void LoadSphereCollider()
    {
        if (this.sphereCollider != null) return;
        this.sphereCollider = GetComponent<SphereCollider>();
        this.sphereCollider.radius = 5f;
        this.sphereCollider.isTrigger = true;
        Debug.Log(transform.name + ": LoadSphereCollider", gameObject);
    }

    protected virtual void LoadRigidbody()
    {
        if(this.rigid != null) return;
        this.rigid = GetComponent<Rigidbody>();
        this.rigid.useGravity = false;
        Debug.Log(transform.name + ": LoadRigidbody", gameObject);
    }

    protected virtual void AddTower(Collider collider)
    {
        if (collider.name != Const.ENEMY_TARGETABLE) return;
        TowerCtrl towerCtrl = collider.transform.parent.GetComponent<TowerCtrl>();
        if(towerCtrl == null) return;
        if(towerCtrl.TowerDamageReceiver != null && towerCtrl.TowerDamageReceiver.IsDead()) return;
        if(this.towers.Contains(towerCtrl)) return;
        this.towers.Add(towerCtrl);
    }

    protected virtual void RemoveTower(Collider collider)
    {
        //if(this.towers.Count == 0) return;
        // foreach(TowerCtrl towerCtrl in this.towers)
        // {
        //     if(collider.transform.parent == towerCtrl.transform)
        //     {
        //         this.towers.Remove(towerCtrl);
        //         // Nếu không còn tower nào trong vùng, reset NearestTower
        //         if (this.towers.Count == 0)
        //         {
        //             this.nearestTower = null;
        //         }
        //         return;
        //     }
        // }
        // Dùng for thay vì foreach để tránh lỗi khi remove trong vòng lặp
        for (int i = towers.Count - 1; i >= 0; i--)
        {
            TowerCtrl towerCtrl = towers[i];
            // Kiểm tra null hoặc đã bị destroy
            if (towerCtrl == null) 
            {
                towers.RemoveAt(i);
                continue;
            }
            // Kiểm tra transform
            if (collider.transform.parent == towerCtrl.transform)
            {
                towers.RemoveAt(i);
                // Nếu không còn tower nào thì reset luôn
                if (towers.Count == 0)
                {
                    this.nearestTower = null;
                }
                return;
            }
        }
    }

    protected virtual void AddPlayer(Collider collider)
    {
        if (collider.name != Const.ENEMY_TARGETABLE) return;
        PlayerCtrl playerCtrl = collider.transform.parent.GetComponent<PlayerCtrl>();
        if(playerCtrl == null) return;
        //if(playerCtrl.PlayerDamageReceiver != null && playerCtrl.PlayerDamageReceiver.IsDead()) return;
        this.player = playerCtrl;
    }

    protected virtual void RemovePlayer(Collider collider)
    {
        if (this.player == null) return;
        if (collider.transform.parent == this.player.transform)
        {
            this.player = null;
        }
    }

    protected virtual void FindPlayer()
    {
        // Có thể mở rộng logic tìm player gần nhất nếu có nhiều player
        // Hiện tại chỉ lấy player đầu tiên trong vùng
    }

    protected virtual void RemoveDeadTower()
    {
        for (int i = towers.Count - 1; i >= 0; i--)
        {
            TowerCtrl towerCtrl = towers[i];
            if (towerCtrl == null) 
            {
                towers.RemoveAt(i);
                continue;
            }
            if (towerCtrl.TowerDamageReceiver != null && towerCtrl.TowerDamageReceiver.IsDead())
            {
                towers.RemoveAt(i);
            }
        }
    }

    protected virtual void FindNearestTower()
    {
        float nearestDistance = Mathf.Infinity;
        float towerDistance;
        TowerCtrl foundTower = null;
        foreach(TowerCtrl towerCtrl in this.towers)
        {
            if(towerCtrl == null) continue;
            if(towerCtrl.TowerDamageReceiver != null && towerCtrl.TowerDamageReceiver.IsDead()) continue;
            towerDistance = Vector3.Distance(transform.position, towerCtrl.transform.position);
            if(towerDistance < nearestDistance)
            {
                nearestDistance = towerDistance;
                foundTower = towerCtrl;
            }
        }
        this.nearestTower = foundTower;
    }

    protected virtual void OnReborn()
    {
        if(this.towers != null) this.towers.Clear();
        if(this.nearestTower != null) this.nearestTower = null;
        if(this.player != null) this.player = null;
    }
}
