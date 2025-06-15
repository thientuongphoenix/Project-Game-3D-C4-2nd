using System.Collections.Generic;
using UnityEngine;

public class EnemyBTree : BTAgent
{
    [SerializeField] protected EnemyCtrl enemyCtrl;
    public EnemyCtrl EnemyCtrl => enemyCtrl;

    private float attackCooldown = 1.0f; // thời gian giữa các đòn đánh
    private float lastAttackTime = -999f;

    public override void Start()
    {
        base.Start();

        this.BuildBehaviorTree();
    }

    public virtual void BuildBehaviorTree()
    {
        if (tree == null)
        {
            tree = new BehaviorTree();
        }
        
        // Xóa các node con cũ nếu có
        if(tree != null)
        {
            tree.children.Clear();
            tree.currentChild = 0;
        }
        

        // Lấy danh sách các Point từ EnemyMoving
        var path = enemyCtrl.EnemyMoving.EnemyPath;
        List<Point> points = new List<Point>();
        Point p = path.GetPoint(0);
        while (p != null)
        {
            points.Add(p);
            p = p.NextPoint;
        }

        //RandomSelector theo tỉ lệ 50/25/25
        Leaf goToNearestTower = new Leaf("Go To Nearest Tower", GoToNearestTower);
        Leaf goToPlayer = new Leaf("Go To Player", GoToPlayer);
        Leaf goToNextPoint = new Leaf("Go To Next Point", GoToNextPoint);
        Leaf attackTower = new Leaf("Attack Tower", AttackTower);
        Leaf attackPlayer = new Leaf("Attack Player", AttackPlayer);

        // Sequence: Đi đến gần player rồi mới attack
        Sequence attackPlayerSequence = new Sequence("Attack Player Sequence");
        attackPlayerSequence.AddChild(goToPlayer);
        attackPlayerSequence.AddChild(attackPlayer);

        // Sequence: Đi đến gần tower rồi mới attack
        Sequence attackTowerSequence = new Sequence("Attack Tower Sequence");
        attackTowerSequence.AddChild(goToNearestTower);
        attackTowerSequence.AddChild(attackTower);

        var children = new List<Node> { goToNextPoint, attackPlayerSequence, attackTowerSequence };
        var weights = new List<float> { 0.5f, 0.25f, 0.25f };
        RandomSelectorEnemy randomSelector = new RandomSelectorEnemy(children, weights);
        tree.AddChild(randomSelector);
        tree.PrintTree();
    }

    protected virtual void Reset()
    {
        this.LoadComponents();
        //this.ResetValue();
    }

    protected virtual void LoadComponents()
    {
        this.LoadEnemyCtrl();
        this.LoadAgent();
    }

    protected virtual void LoadEnemyCtrl()
    {
        if (this.enemyCtrl != null) return;
        this.enemyCtrl = transform.parent.GetComponent<EnemyCtrl>();
        Debug.Log(transform.name + ": LoadEnemyCtrl", gameObject);
    }

    protected virtual void LoadAgent()
    {
        if (this.agent != null) return;
        this.agent = this.enemyCtrl.Agent;
        Debug.Log(transform.name + ": LoadAgent", gameObject);
    }

    // Hàm di chuyển tới một Point, dùng lại logic của EnemyMoving
    public Node.Status GoToPoint(Point point)
    {
        // Luôn cập nhật point mới trước khi di chuyển
        if (!enemyCtrl.EnemyMoving.CanMove)
        {
            enemyCtrl.Agent.isStopped = true;
            return Node.Status.FAILURE;
        }

        // Kiểm tra đã chết chưa
        if (enemyCtrl.EnemyDamageReceiver.IsDead())
        {
            enemyCtrl.Agent.isStopped = true;
            return Node.Status.FAILURE;
        }

        //enemyCtrl.Agent.isStopped = false;

        // Luôn cập nhật point mới nếu đã đến nơi
        enemyCtrl.EnemyMoving.FindNextPoint();
        point = enemyCtrl.EnemyMoving.CurrentPoint;

        if (point == null || enemyCtrl.EnemyMoving.IsFinish)
        {
            enemyCtrl.Agent.isStopped = true;
            return Node.Status.SUCCESS;
        }

        enemyCtrl.Agent.isStopped = false;
        enemyCtrl.Agent.SetDestination(point.transform.position);

        float distance = Vector3.Distance(enemyCtrl.transform.position, point.transform.position);
        if (distance < enemyCtrl.EnemyMoving.StopDistance)
        {
            // Đã đến nơi, lần tick sau sẽ FindNextPoint tiếp
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    // Hàm di chuyển tới Tower gần nhất
    public Node.Status GoToNearestTower()
    {
        if (enemyCtrl.EnemyDamageReceiver.IsDead())
        {
            enemyCtrl.Agent.isStopped = true;
            return Node.Status.FAILURE;
        }

        var targeting = enemyCtrl.EnemyTargeting;
        if (targeting == null || targeting.NearestTower == null) return Node.Status.FAILURE;
        var tower = targeting.NearestTower;
        if (tower.TowerDamageReceiver != null && tower.TowerDamageReceiver.IsDead()) return Node.Status.FAILURE;
        enemyCtrl.Agent.isStopped = false;
        enemyCtrl.Agent.SetDestination(tower.transform.position);
        float distance = Vector3.Distance(enemyCtrl.transform.position, tower.transform.position);
        if (distance < enemyCtrl.EnemyMoving.StopDistance)
        {
            // Logic tấn công tower
            //enemyCtrl.Agent.isStopped = true;
            //enemyCtrl.Animator.SetBool("isAttack", true);
            Debug.Log("Enemy đã đến gần Tower: " + tower.name);
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    // Hàm di chuyển tới Player
    public Node.Status GoToPlayer()
    {
        if (enemyCtrl.EnemyDamageReceiver.IsDead())
        {
            enemyCtrl.Agent.isStopped = true;
            return Node.Status.FAILURE;
        }

        var targeting = enemyCtrl.EnemyTargeting;
        if (targeting == null || targeting.Player == null) return Node.Status.FAILURE;
        var player = targeting.Player;
        // Có thể kiểm tra player chết không nếu cần
        enemyCtrl.Agent.isStopped = false;
        enemyCtrl.Agent.SetDestination(player.transform.position);
        float distance = Vector3.Distance(enemyCtrl.transform.position, player.transform.position);
        if (distance < enemyCtrl.EnemyMoving.StopDistance)
        {
            // Có thể bổ sung logic tấn công player ở đây
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    // Hàm di chuyển tới point tiếp theo (giữ nguyên logic cũ)
    public Node.Status GoToNextPoint()
    {
        if (enemyCtrl.EnemyDamageReceiver.IsDead())
        {
            enemyCtrl.Agent.isStopped = true;
            return Node.Status.FAILURE;
        }
        
        return GoToPoint(enemyCtrl.EnemyMoving.CurrentPoint);
    }

    // Node tấn công tower
    public Node.Status AttackTower()
    {
        var targeting = enemyCtrl.EnemyTargeting;
        if (targeting == null || targeting.NearestTower == null) return Node.Status.FAILURE;
        var tower = targeting.NearestTower;
        if (tower.TowerDamageReceiver != null && tower.TowerDamageReceiver.IsDead())
        {
            enemyCtrl.Animator.SetBool("isAttack", false);
            return Node.Status.SUCCESS;
        }
        // Đứng lại tại chỗ để đánh
        enemyCtrl.Agent.isStopped = true;
        // Đánh liên tục theo cooldown
        if (Time.time - lastAttackTime > attackCooldown)
        {
            enemyCtrl.Animator.SetTrigger("isAttack");
            lastAttackTime = Time.time;
        }
        return Node.Status.RUNNING;
    }

    // Node tấn công player
    public Node.Status AttackPlayer()
    {
        var targeting = enemyCtrl.EnemyTargeting;
        if (targeting == null || targeting.Player == null)
        {
            // Player đã rời khỏi phạm vi phát hiện
            enemyCtrl.Animator.SetBool("isAttack", false);
            return Node.Status.SUCCESS;
        }
        // Kiểm tra khoảng cách tấn công
        float distance = Vector3.Distance(enemyCtrl.transform.position, targeting.Player.transform.position);
        float attackRange = 1.5f; // hoặc enemyCtrl.EnemyAttackRange nếu có
        if (distance > attackRange)
        {
            // Player đã rời khỏi phạm vi tấn công, quay lại chase
            enemyCtrl.Animator.SetBool("isAttack", false);
            return Node.Status.FAILURE;
        }
        // Đứng lại tại chỗ để đánh
        enemyCtrl.Agent.isStopped = true;
        // Đánh liên tục theo cooldown
        if (Time.time - lastAttackTime > attackCooldown)
        {
            enemyCtrl.Animator.SetTrigger("isAttack");
            lastAttackTime = Time.time;
        }
        return Node.Status.RUNNING;
    }

    /*
        Root
└── Selector
    ├── Combat Selector (Nếu phát hiện Tower/Player ở gần)
    │   ├── Attack Tower Sequence (25%)
    │   │   ├── Check Nearest Tower
    │   │   ├── Go To Nearest Tower
    │   │   └── Attack Tower
    │   ├── Attack Player Sequence (25%)
    │   │   ├── Check Player
    │   │   ├── Go To Player
    │   │   └── Attack Player
    │   └── Continue Moving (50%)
    │       └── Go To Next Point
    └── Patrol Sequence (Nếu không có combat)
        └── Go To Next Point (lặp qua các point)
        */
}
