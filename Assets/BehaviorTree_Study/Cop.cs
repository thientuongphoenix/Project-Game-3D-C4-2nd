using UnityEngine;

public class Cop : BTAgent
{
    public GameObject[] patrolPoints;
    public GameObject robber;

    public override void Start()
    {
        base.Start();
        
        Sequence selectPatrolPoint = new Sequence("Select Patrol Point");
        for(int i = 0; i < patrolPoints.Length; i++)
        {
            Leaf pp = new Leaf("Go to " + patrolPoints[i].name, i, GoToPoint);
            selectPatrolPoint.AddChild(pp);
        }

        Sequence chaseRobber = new Sequence("Chase");
        Leaf canSee = new Leaf("Can See Robber", CanSeeRobber);
        Leaf chase = new Leaf("Chase Robber", ChaseRobber);

        chaseRobber.AddChild(canSee);
        chaseRobber.AddChild(chase);

        Inverter cantSeeRobber = new Inverter("Cant See Robber");
        cantSeeRobber.AddChild(canSee);

        BehaviorTree patrolConditions = new BehaviorTree();
        Sequence condition = new Sequence("Patrol Conditions");
        condition.AddChild(cantSeeRobber);
        patrolConditions.AddChild(condition);
        DepSequence patrol = new DepSequence("Patrol Until", patrolConditions, agent);
        patrol.AddChild(selectPatrolPoint);

        Selector beCop = new Selector("Be A Cop");
        beCop.AddChild(patrol);
        beCop.AddChild(chaseRobber);

        tree.AddChild(beCop);

        tree.PrintTree();

        // Cây hành vi của cảnh sát:
        // Root
        // └── Be A Cop (Selector)
        //     ├── Patrol Until (DepSequence)
        //     │   ├── Patrol Conditions (Sequence)
        //     │   │   └── Can't See Robber (Inverter)
        //     │   │       └── Can See Robber (Leaf)
        //     │   └── Select Patrol Point (Sequence)
        //     │       ├── Go to Point 1 (Leaf)
        //     │       ├── Go to Point 2 (Leaf)
        //     │       └── Go to Point N (Leaf)
        //     └── Chase (Sequence)
        //         ├── Can See Robber (Leaf)
        //         └── Chase Robber (Leaf)
    }

    public Node.Status GoToPoint(int i)
    {
        Node.Status s = GotoLocation(patrolPoints[i].transform.position);
        return s;
    }

    public Node.Status CanSeeRobber()
    {
        return CanSee(robber.transform.position, "Robber", 5, 60);
    }

    Vector3 rl; //Remember location
    public Node.Status ChaseRobber()
    {
        float chaseDistance = 10;
        if(state == ActionState.IDLE)
        {
            rl = this.transform.position - (transform.position - robber.transform.position).normalized * chaseDistance;
        }
        return GotoLocation(rl);
    }
}
