using UnityEngine;
using UnityEngine.AI;

public class EnemyMoving : SaiMonoBehaviour
{
    //public GameObject target;
    [SerializeField] protected EnemyCtrl enemyCtrl;
    //[SerializeField] protected int pathIndex = 0;
    [SerializeField] protected string pathName = "Path_0";
    [SerializeField] protected PathMoving enemyPath;
    public PathMoving EnemyPath => enemyPath;
    [SerializeField] protected Point currentPoint;
    public Point CurrentPoint { get => currentPoint; set => currentPoint = value; }
    [SerializeField] protected float pointDistance = Mathf.Infinity;
    [SerializeField] protected float stopDistance = 1f;
    public float StopDistance => stopDistance;
    [SerializeField] protected bool canMove = false;
    public bool CanMove => canMove;
    [SerializeField] protected bool isMoving = false;
    public bool IsMoving => isMoving;
    [SerializeField] protected bool isFinish = false;
    public bool IsFinish { get => isFinish; set => isFinish = value; }

    protected virtual void OnEnable()
    {
        this.OnReborn();
    }

    protected override void Start()
    {
        //this.LoadEnemyPath();
    }

    private void FixedUpdate()
    {
        //this.Moving();
        this.CheckMoving();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadEnemyCtrl();
        //this.LoadTarget();
        this.LoadEnemyPath();
    }

    protected virtual void LoadEnemyPath()
    {
        if (this.enemyPath != null) return;
        this.enemyPath = GameObject.FindAnyObjectByType<PathsManager>().GetPath(this.pathName);
        Debug.Log(transform.name + ": LoadEnemyPath", gameObject);
    }

    protected virtual void LoadEnemyCtrl()
    {
        if (this.enemyCtrl != null) return;
        this.enemyCtrl = transform.parent.GetComponent<EnemyCtrl>();
        Debug.Log(transform.name + ": LoadNavMeshAgent", gameObject);
    }

    // protected virtual void LoadTarget()
    // {
    //     if (this.target != null) return;
    //     this.target = GameObject.Find("LoadTarget");
    //     Debug.Log(transform.name + ": LoadTarget", gameObject);
    // }

    protected virtual void Moving()
    {
        if (!this.canMove)
        {
            this.enemyCtrl.Agent.isStopped = true;
            return;
        }

        if (this.enemyCtrl.EnemyDamageReceiver.IsDead())
        {
            this.enemyCtrl.Agent.isStopped = true;
            return;
        }

        this.FindNextPoint();

        if (this.currentPoint == null || this.isFinish == true)
        {
            this.enemyCtrl.Agent.isStopped = true;
            return;
        }

        this.enemyCtrl.Agent.isStopped = false;
        this.enemyCtrl.Agent.SetDestination(this.currentPoint.transform.position);
    }

    public virtual void FindNextPoint()
    {
        if(this.currentPoint == null) this.currentPoint = this.enemyPath.GetPoint(0);

        this.pointDistance = Vector3.Distance(transform.position, this.currentPoint.transform.position);
        if(pointDistance < stopDistance)
        {
            this.currentPoint = this.currentPoint.NextPoint;
            if(this.currentPoint == null) this.isFinish = true;
        }
    }

    // protected virtual void LoadEnemyPath()
    // {
    //     if(this.enemyPath != null) return;
    //     this.enemyPath = PathsManager.Instance.GetPath(this.pathName);
    //     //Debug.Log(transform.name + ": LoadEnemyPath", gameObject);
    // }

    protected virtual void CheckMoving()
    {
        if(this.enemyCtrl.Agent.velocity.magnitude > 0.1f) this.isMoving = true;
        else this.isMoving = false;

        this.enemyCtrl.Animator.SetBool("isMoving", this.isMoving);
    }

    protected virtual void OnReborn()
    {
        this.isFinish = false;
        this.currentPoint = null;
        if(this.enemyCtrl.EnemyTargeting != null) this.enemyCtrl.EnemyTargeting.Towers.Clear();
        //if(this.enemyCtrl.EnemyTargeting != null) this.enemyCtrl.EnemyTargeting.Player = null;
        //this.enemyCtrl.EnemyTargeting.NearestTower = null;
    }
}
