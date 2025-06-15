using System.Collections.Generic;
using UnityEngine;

public abstract class TowerCtrl : PoolObj
{
    [SerializeField] protected Transform model;
    [SerializeField] protected Transform rotator;
    public Transform Rotator => rotator;

    [SerializeField] protected TowerTargeting towerTargeting;
    public TowerTargeting TowerTargeting => towerTargeting;

    [SerializeField] protected BulletSpawner bulletSpawner;
    public BulletSpawner BulletSpawner => bulletSpawner;

    [SerializeField] protected TowerShooting towerShooting;
    public TowerShooting TowerShooting => towerShooting;

    [SerializeField] protected LevelAbstract level;
    public LevelAbstract Level => level;

    protected string bulletName = "Bullet";
    [SerializeField] protected Bullet bullet;
    public Bullet Bullet => bullet;

    [SerializeField] protected BulletPrefabs bulletPrefabs;
    public BulletPrefabs BulletPrefabs => bulletPrefabs;

    [SerializeField] protected List<FirePoint> firePoints = new();
    public List<FirePoint> FirePoints => firePoints;

    [SerializeField] protected TowerDamageReceiver towerDamageReceiver;
    public TowerDamageReceiver TowerDamageReceiver => towerDamageReceiver;

    [SerializeField] protected TowerDespawn towerDespawn;
    public TowerDespawn TowerDespawn => towerDespawn;

    [SerializeField] protected EnemyTargetable enemyTargetable;
    public EnemyTargetable EnemyTargetable => enemyTargetable;

    protected override void Awake()
    {
        base.Awake();
        this.HidePrefabs();
    }

    protected override void Start()
    {
        this.SetActiveEnemyTargetable();
    }

    protected virtual void OnEnable()
    {
        this.level.ResetLevel();
        this.SetActiveEnemyTargetable();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadModel();
        this.LoadTowerTargeting();
        this.LoadBulletSpawner();
        
        this.LoadBulletPrefabs();
        this.LoadFirePoints();

        this.LoadTowerShootings();
        this.LoadLevel();
        this.LoadTowerDamageReceiver();
        this.LoadTowerDespawn();
        this.LoadEnemyTargetable();
    }

    protected virtual void LoadEnemyTargetable()
    {
        if(this.enemyTargetable != null) return;
        this.enemyTargetable = GetComponentInChildren<EnemyTargetable>();
        
        Debug.Log(transform.name + ": LoadEnemyTargetable", gameObject);
    }

    protected virtual void LoadTowerDespawn()
    {
        if(this.towerDespawn != null) return;
        this.towerDespawn = GetComponentInChildren<TowerDespawn>();
        Debug.Log(transform.name + ": LoadTowerDespawn", gameObject);
    }

    protected virtual void LoadTowerDamageReceiver()
    {
        if(this.towerDamageReceiver != null) return;
        this.towerDamageReceiver = GetComponentInChildren<TowerDamageReceiver>();
        Debug.Log(transform.name + ": LoadTowerDamageReceiver", gameObject);
    }

    protected virtual void LoadLevel()
    {
        if (this.level != null) return;
        this.level = GetComponentInChildren<LevelAbstract>();
        Debug.Log(transform.name + ": LoadLevel", gameObject);
    }

    protected virtual void LoadTowerShootings()
    {
        if (this.towerShooting != null) return;
        this.towerShooting = GetComponentInChildren<TowerShooting>();
        Debug.Log(transform.name + ": LoadTowerShootings", gameObject);
    }

    protected virtual void LoadBulletSpawner()
    {
        if(this.bulletSpawner != null) return;
        this.bulletSpawner = FindFirstObjectByType<BulletSpawner>();
        Debug.Log(transform.name + ": LoadBulletSpawner", gameObject);
    }

    protected virtual void LoadBullet()
    {
        if(this.bullet != null) return;
        this.bullet = this.bulletPrefabs.GetByName(this.bulletName);
        Debug.Log(transform.name + ": LoadBullet", gameObject);
    }

    protected virtual void LoadBulletPrefabs()
    {
        if(this.bulletPrefabs != null) return;
        this.bulletPrefabs = GameObject.FindAnyObjectByType<BulletPrefabs>();
        Debug.Log(transform.name + ": LoadBulletPrefabs", gameObject);

        this.LoadBullet();
    }

    protected virtual void LoadModel()
    {
        if (model != null) return;
        this.model = transform.Find("Model");
        this.rotator = this.model.Find("Rotator");
        Debug.Log(transform.name + ": LoadModel", gameObject);
    }

    protected virtual void LoadTowerTargeting()
    {
        if (towerTargeting != null) return;
        this.towerTargeting = transform.GetComponentInChildren<TowerTargeting>();
        //this.towerTargeting.transform.localPosition = new Vector3(0, 0, 0);
        Debug.Log(transform.name + ": LoadTowerTargeting", gameObject);
    }

    protected virtual void LoadFirePoints()
    {
        if(this.firePoints.Count > 0) return;
        FirePoint[] points = transform.GetComponentsInChildren<FirePoint>();
        this.firePoints = new List<FirePoint>(points);
        Debug.Log(transform.name + ": LoadFirePoints", gameObject);
    }

    protected virtual void HidePrefabs()
    {
        this.bullet.gameObject.SetActive(false);
    }

    public bool CanLevelUp()
    {
        // Chỉ cho phép lên cấp nếu object cha là PoolHolder
        if (this.transform.parent == null) return false;
        if (this.transform.parent.name != "PoolHolder") return false;
        return true;
    }

    protected virtual void SetActiveEnemyTargetable()
    {
        if (this.transform.parent != null && this.transform.parent.name == "PoolHolder")
        {
            this.enemyTargetable.gameObject.SetActive(true);
        }
        else
        {
            this.enemyTargetable.gameObject.SetActive(false);
        }
    }
}
