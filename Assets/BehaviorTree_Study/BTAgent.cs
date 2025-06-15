using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
    public BehaviorTree tree;
    public NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING};
    public ActionState state = ActionState.IDLE;

    public Node.Status treeStatus = Node.Status.RUNNING;

    WaitForSeconds waitForSeconds;
    Vector3 rememberLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();

        this.tree = new BehaviorTree(); // root Node

        this.waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1f));
        StartCoroutine(this.Behave());
    }

    public Node.Status CanSee(Vector3 target, string tag, float distance, float maxAngle)
    {
        Vector3 directionToTarget = target - this.transform.position;
        float  angle = Vector3.Angle(directionToTarget, this.transform.forward);

        if(angle <= maxAngle || directionToTarget.magnitude <= distance)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(this.transform.position, directionToTarget, out hitInfo))
            {
                if(hitInfo.collider.gameObject.CompareTag(tag))
                {
                    return Node.Status.SUCCESS;
                }
            }
        }
        return Node.Status.FAILURE;
    }

    public Node.Status IsOpen()
    {
        if(Blackboard.Instance.timeOfDay < Blackboard.Instance.openTime || Blackboard.Instance.timeOfDay > Blackboard.Instance.closeTime)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status Flee(Vector3 location, float distance)
    {
        if(state == ActionState.IDLE)
        {
            this.rememberLocation = this.transform.position + (this.transform.position - location).normalized * distance;
        }
        return GotoLocation(this.rememberLocation);
    }

    /// <summary>
    /// Điều khiển NavMeshAgent di chuyển đến vị trí chỉ định.
    /// Trả về trạng thái:
    /// - SUCCESS: Khi đã tới gần đích.
    /// - FAILURE: Khi điểm đích không còn hợp lệ.
    /// - RUNNING: Khi đang di chuyển.
    /// </summary>
    /// <param name="destination">Vị trí mục tiêu cần đến.</param>
    /// <returns>Trạng thái hành động (SUCCESS, FAILURE, RUNNING).</returns>
    public Node.Status GotoLocation(Vector3 destination)
    {
        // Tính khoảng cách hiện tại tới điểm đến
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);

        // Nếu đang ở trạng thái chờ (IDLE), bắt đầu di chuyển
        if (this.state == ActionState.IDLE)
        {
            this.agent.SetDestination(destination); // Đặt điểm đến cho NavMeshAgent
            this.state = ActionState.WORKING;        // Chuyển trạng thái sang đang làm việc
        }
        // Nếu điểm đến đã thay đổi quá xa so với kế hoạch ban đầu
        else if (Vector3.Distance(this.agent.pathEndPosition, destination) >= 2)
        {
            this.state = ActionState.IDLE;           // Reset trạng thái
            return Node.Status.FAILURE;              // Thất bại
        }
        // Nếu đã gần đến điểm đích
        else if (distanceToTarget < 2)
        {
            this.state = ActionState.IDLE;           // Reset trạng thái
            return Node.Status.SUCCESS;              // Thành công
        }

        // Nếu chưa tới đích và không thất bại → tiếp tục di chuyển
        return Node.Status.RUNNING;
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GotoLocation(door.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                //door.SetActive(false);
                door.GetComponent<NavMeshObstacle>().enabled = false;
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else return s; //Thông thường mà chưa tới nơi, nó sẽ trả ra RUNNING
    }

    /// <summary>
    /// Hàm dùng để hạn chế bớt tầng xuất cập nhật BTree
    /// </summary>
    /// <returns></returns>
    IEnumerator Behave()
    {
        while (true)
        {
            this.treeStatus = tree.Process();
            yield return this.waitForSeconds;
        }
    }

    // Hàm này đã được thay thế bởi Behave để gọi ít hơn
    //void Update()
    //{
    //    if(treeStatus != Node.Status.SUCCESS) treeStatus = tree.Process();
    //}

    public void ResetBTree()
    {
        if (tree != null)
        {
            tree.Reset(); // Reset trạng thái toàn bộ node
        }
    }

    public void StartBTree()
    {
        if (tree != null)
        {
            treeStatus = Node.Status.RUNNING;
            tree.currentChild = 0;
        }
        // Đảm bảo coroutine Behave được chạy lại
        StopAllCoroutines();
        StartCoroutine(this.Behave());
    }
}
