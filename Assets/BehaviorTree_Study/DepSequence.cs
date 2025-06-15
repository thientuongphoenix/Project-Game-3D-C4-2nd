using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Node
{
    BehaviorTree dependancy;
    NavMeshAgent agent;
    public DepSequence(string n, BehaviorTree d, NavMeshAgent a)
    {
        name = n;
        dependancy = d;
        agent = a;
    }

    public override Status Process()
    {
        if(dependancy.Process() == Status.FAILURE)
        {
            agent.ResetPath();
            // Reset trạng thái của BTAgent về IDLE
            if (agent.gameObject.TryGetComponent<BTAgent>(out var btAgent))
            {
                btAgent.state = BTAgent.ActionState.IDLE;
            }
            foreach(Node n in children)
            {
                n.Reset();
            }
            return Status.FAILURE;
        }

        Status childstatus = children[currentChild].Process();
        if(childstatus == Status.RUNNING) return Status.RUNNING;
        if(childstatus == Status.FAILURE) return childstatus; //Đệ quy trở lại Status hiện tại, check tiếp

        currentChild++;
        if(currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS; //Nếu không có lá nào trả về FAILURE thì đồng nghĩa Node Sequence là SUCCESS
        }

        return Status.RUNNING;
    }
}
