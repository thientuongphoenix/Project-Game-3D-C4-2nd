using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyCtrl : PoolObj
{
    [SerializeField] protected Transform model;
    [SerializeField] protected NavMeshAgent agent;
    public NavMeshAgent Agent => agent;

    [SerializeField] protected Animator animator;
    public Animator Animator => animator;

    [SerializeField] protected TowerTargetable towerTargetable;
    public TowerTargetable TowerTargetable => towerTargetable;

    [SerializeField] protected EnemyDamageReceiver enemyDamageReceiver;
    public EnemyDamageReceiver EnemyDamageReceiver => enemyDamageReceiver;

    [SerializeField] protected EnemyBTree enemyBTree;
    public EnemyBTree EnemyBTree => enemyBTree;

    [SerializeField] protected EnemyMoving enemyMoving;
    public EnemyMoving EnemyMoving => enemyMoving;

    [SerializeField] protected EnemyTargeting enemyTargeting;
    public EnemyTargeting EnemyTargeting => enemyTargeting;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadNavMeshAgent();
        this.LoadModel();
        this.LoadAnimator();
        this.LoadTowerTargetable();
        this.LoadEnemyDamageReceiver();
        this.LoadEnemyBTree();
        this.LoadEnemyMoving();
        this.LoadEnemyTargeting();
    }

    protected virtual void LoadEnemyTargeting()
    {
        if (this.enemyTargeting != null) return;
        this.enemyTargeting = transform.GetComponentInChildren<EnemyTargeting>();
        Debug.Log(transform.name + ": LoadEnemyTargeting", gameObject);
    }

    protected virtual void LoadEnemyMoving()
    {
        if (this.enemyMoving != null) return;
        this.enemyMoving = transform.GetComponentInChildren<EnemyMoving>();
        Debug.Log(transform.name + ": LoadEnemyMoving", gameObject);
    }

    protected virtual void LoadEnemyBTree()
    {
        if (this.enemyBTree != null) return;
        this.enemyBTree = transform.GetComponentInChildren<EnemyBTree>();
        Debug.Log(transform.name + ": LoadEnemyBTree", gameObject);
    }

    protected virtual void LoadNavMeshAgent()
    {
        if (this.agent != null) return;
        this.agent = GetComponent<NavMeshAgent>();
        this.agent.speed = 2;
        this.agent.angularSpeed = 200;
        this.agent.acceleration = 150;
        Debug.Log(transform.name + ": LoadNavMeshAgent", gameObject);
    }

    protected virtual void LoadModel()
    {
        if (this.model != null) return;
        this.model = transform.Find("Model");
        this.model.localPosition = new Vector3(0, 0, 0);
        Debug.Log(transform.name + ": LoadModel", gameObject);
    }
    
    protected virtual void LoadAnimator()
    {
        if (this.animator != null) return;
        this.animator = this.model.GetComponent<Animator>();
        Debug.Log(transform.name + ": LoadAnimator", gameObject);
    }

    protected virtual void LoadTowerTargetable()
    {
        if (this.towerTargetable != null) return;
        this.towerTargetable = transform.GetComponentInChildren<TowerTargetable>();
        this.towerTargetable.transform.localPosition = new Vector3(0, 1f, 0);
        Debug.Log(transform.name + ": LoadTowerTargetable", gameObject);
    }

    protected virtual void LoadEnemyDamageReceiver()
    {
        if (this.enemyDamageReceiver != null) return;
        this.enemyDamageReceiver = GetComponentInChildren<EnemyDamageReceiver>();
        Debug.Log(transform.name + ": LoadEnemyDamageReceiver", gameObject);
    }
}
