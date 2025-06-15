using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : BTAgent
{
    //BehaviorTree tree; //Cái này có rồi, nhờ kế thừa BTAgent
    public GameObject diamond;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;
    public GameObject painting;
    public GameObject Cop;

    public GameObject[] art;

    GameObject pickup;
    //NavMeshAgent agent;

    //public enum ActionState { IDLE, WORKING};
    //ActionState state = ActionState.IDLE;

    //Node.Status treeStatus = Node.Status.RUNNING;

    [Range(0, 1000)]
    public int money = 800;

    Leaf goToBackDoor;
    Leaf goToFrontDoor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        //this.agent = GetComponent<NavMeshAgent>();

        //this.tree = new BehaviorTree(); // root Node
        base.Start();
          
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 1);
        Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 2);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);

        // Leaf goToArt1 = new Leaf("Go To Art 1", GoToArt1);
        // Leaf goToArt2 = new Leaf("Go To Art 2", GoToArt2);
        // Leaf goToArt3 = new Leaf("Go To Art 3", GoToArt3);

        RSelector selectObject = new RSelector("Select Object To Steal");
        for(int i = 0; i < art.Length; i++)
        {
            Leaf gta = new Leaf("Go To " + art[i].name, i, GoToArt);
            selectObject.AddChild(gta);
        }

        goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor, 2);
        goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor, 1);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        PSelector opendoor = new PSelector("Open Door");
        
        Sequence runAway = new Sequence("Run Away");
        Leaf canSee = new Leaf("Can See Cop?", CanSeeCop);
        Leaf flee = new Leaf("Flee From Cop", FleeFromCop);

        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);

        opendoor.AddChild(goToFrontDoor);
        opendoor.AddChild(goToBackDoor);

        runAway.AddChild(canSee);
        runAway.AddChild(flee);

        Inverter cantSeeCop = new Inverter("Cant See Cop?");
        cantSeeCop.AddChild(canSee);

        
        
        // steal.AddChild(invertMoney);
        // steal.AddChild(opendoor); // Chọn cửa để vào
        // steal.AddChild(cantSeeCop);

        // steal.AddChild(selectObject); //Chọn vật để trộm

        // //steal.AddChild(goToBackDoor);
        // steal.AddChild(goToVan); // trở về xe

        Selector s1 = new Selector("s1");
        s1.AddChild(invertMoney);
        Sequence s2 = new Sequence("s2");
        s2.AddChild(cantSeeCop);
        s2.AddChild(opendoor);
        Sequence s3 = new Sequence("s3");
        s3.AddChild(cantSeeCop);
        s3.AddChild(selectObject);
        Sequence s4 = new Sequence("s4");
        s4.AddChild(cantSeeCop);
        s4.AddChild(goToVan);

        // steal.AddChild(s1);
        // steal.AddChild(s2);
        // steal.AddChild(s3);
        // steal.AddChild(s4);

        Leaf isOpen = new Leaf("Is Open", IsOpen);
        Inverter isClosed = new Inverter("Is Closed");
        isClosed.AddChild(isOpen);

        BehaviorTree stealConditions = new BehaviorTree();
        Sequence conditions = new Sequence("Stealing Conditions");
        conditions.AddChild(isClosed);
        conditions.AddChild(cantSeeCop);
        conditions.AddChild(invertMoney);
        stealConditions.AddChild(conditions);
        DepSequence steal = new DepSequence("Steal Something", stealConditions, agent);
        //steal.AddChild(invertMoney);
        steal.AddChild(opendoor);
        steal.AddChild(selectObject);
        steal.AddChild(goToVan);

        Selector stealWithFallback = new Selector("Steal With Fallback");
        stealWithFallback.AddChild(steal);
        stealWithFallback.AddChild(goToVan);
        

        Selector beThief = new Selector("Be a Thief");
        beThief.AddChild(stealWithFallback);
        beThief.AddChild(runAway);

        this.tree.AddChild(beThief); // Thêm tất cả vào node gốc

        this.tree.PrintTree();

        StartCoroutine("DecreaseMoney");

        // Cây hành vi của tên trộm:
        // Root
        // └── Be a Thief (Selector)
        //     ├── Steal With Fallback (Selector)
        //     │   ├── Steal Something (DepSequence)
        //     │   │   ├── Stealing Conditions (Sequence)
        //     │   │   │   ├── Is Closed (Inverter)
        //     │   │   │   │   └── Is Open (Leaf)
        //     │   │   │   ├── Can't See Cop (Sequence)
        //     │   │   │   │   └── Can See Cop? (Leaf)
        //     │   │   │   └── Has No Money (Inverter)
        //     │   │   │       └── Has Money? (Leaf)
        //     │   │   ├── Open Door (Leaf)
        //     │   │   ├── Select Object (Selector)
        //     │   │   │   ├── Go To Diamond (Leaf)
        //     │   │   │   └── Go To Painting (Leaf)
        //     │   │   └── Go To Van (Leaf)
        //     │   └── Go To Van (Leaf)
        //     └── Run Away (Sequence)
        //         ├── Can See Cop? (Leaf)
        //         └── Flee From Cop (Leaf)
    }

    IEnumerator DecreaseMoney()
    {
        while(true)
        {
            this.money = Mathf.Clamp(this.money - 50, 0, 1000);
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    public Node.Status CanSeeCop()
    {
        return CanSee(this.Cop.transform.position, "Cop", 10, 90);
    }

    public Node.Status FleeFromCop()
    {
        return Flee(this.Cop.transform.position, 10);
    }

    public Node.Status HasMoney()
    {
        Debug.Log("HasMoney: money = " + this.money);
        if (this.money < 500) return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToDiamond()
    {
        if(!this.diamond.activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            //if (this.diamond != null)
            //{
            //    this.diamond.transform.parent = this.gameObject.transform; // Lôi viên kim cương theo luôn
            //    return Node.Status.SUCCESS;
            //}
            //return Node.Status.FAILURE;

            this.diamond.transform.parent = this.gameObject.transform;
            this.pickup = this.diamond;
        }
        //else return s;
        return s;
    }

    public Node.Status GoToPainting()
    {
        if (!this.painting.activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.painting.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            this.painting.transform.parent = this.gameObject.transform;
            this.pickup = this.painting;
        }
        return s;
    }

    public Node.Status GoToArt(int i)
    {
        if(!this.art[i].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.art[i].transform.position);
        if (s == Node.Status.SUCCESS)
        {
            this.art[i].transform.parent = this.gameObject.transform;
            this.pickup = this.art[i];
        }
        return s;
    }

    public Node.Status GoToArt1()
    {
        if(!this.art[0].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.art[0].transform.position);
        if (s == Node.Status.SUCCESS)
        {
            this.art[0].transform.parent = this.gameObject.transform;
            this.pickup = this.art[0];
        }
        return s;
    }

    public Node.Status GoToArt2()
    {
        if(!this.art[1].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.art[1].transform.position);
        if (s == Node.Status.SUCCESS)
        {
            this.art[1].transform.parent = this.gameObject.transform;
            this.pickup = this.art[1];
        }
        return s;
    }

    public Node.Status GoToArt3()
    {
        if(!this.art[2].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.art[2].transform.position);
        if (s == Node.Status.SUCCESS)
        {
            this.art[2].transform.parent = this.gameObject.transform;
            this.pickup = this.art[2];
        }
        return s;
    }

    //Đầu tiên nhân vật đi theo hướng cửa trước, vì bị khóa nên phải đi theo cửa sau
    //Từ đó cửa trước bị xếp độ ưu tiên là 10, cửa sau là 1
    //Lần thử tiếp theo nhân vật sẽ đi theo cửa sau thay vì cửa trước
    public Node.Status GoToBackDoor()
    {
        Node.Status s = base.GoToDoor(backdoor);
        if (s == Node.Status.FAILURE)
        {
            this.goToBackDoor.sortOrder = 10;
        }
        else
        {
            this.goToBackDoor.sortOrder = 1;
        }
        return s;
        //this.agent.SetDestination(this.diamond.transform.position);
        //return Node.Status.SUCCESS;
    }
    public Node.Status GoToFrontDoor()
    {
        Node.Status s = base.GoToDoor(frontdoor);
        if (s == Node.Status.FAILURE)
        {
            this.goToFrontDoor.sortOrder = 10;
        }
        else
        {
            this.goToFrontDoor.sortOrder = 1;
        }
        return s;
        //this.agent.SetDestination(this.diamond.transform.position);
        //return Node.Status.SUCCESS;
    }

    // public Node.Status GoToDoor(GameObject door)
    // {
    //     Node.Status s = GotoLocation(door.transform.position);
    //     if (s == Node.Status.SUCCESS)
    //     {
    //         if (!door.GetComponent<Lock>().isLocked)
    //         {
    //             //door.SetActive(false);
    //             door.GetComponent<NavMeshObstacle>().enabled = false;
    //             return Node.Status.SUCCESS;
    //         }
    //         return Node.Status.FAILURE;
    //     }
    //     else return s; //Thông thường mà chưa tới nơi, nó sẽ trả ra RUNNING
    // }

    public Node.Status GoToVan()
    {
        //return this.GotoLocation(this.van.transform.position);
        //this.agent.SetDestination(this.van.transform.position);
        //return Node.Status.SUCCESS;
        Node.Status s = GotoLocation(this.van.transform.position);
        if (s == Node.Status.SUCCESS) 
        {
            if (pickup != null) 
            {
                money += 300;
                pickup.SetActive(false);
                pickup = null;
            }
        }
        return s;
    }


    //Đoạn code bên dưới đã chuyển qua cho Class cha giữ rồi nhá
    ///// <summary>
    ///// Điều khiển NavMeshAgent di chuyển đến vị trí chỉ định.
    ///// Trả về trạng thái:
    ///// - SUCCESS: Khi đã tới gần đích.
    ///// - FAILURE: Khi điểm đích không còn hợp lệ.
    ///// - RUNNING: Khi đang di chuyển.
    ///// </summary>
    ///// <param name="destination">Vị trí mục tiêu cần đến.</param>
    ///// <returns>Trạng thái hành động (SUCCESS, FAILURE, RUNNING).</returns>
    //new Node.Status GotoLocation(Vector3 destination)
    //{
    //    // Tính khoảng cách hiện tại tới điểm đến
    //    float distanceToTarget = Vector3.Distance(destination, this.transform.position);

    //    // Nếu đang ở trạng thái chờ (IDLE), bắt đầu di chuyển
    //    if (this.state == ActionState.IDLE)
    //    {
    //        this.agent.SetDestination(destination); // Đặt điểm đến cho NavMeshAgent
    //        this.state = ActionState.WORKING;        // Chuyển trạng thái sang đang làm việc
    //    }
    //    // Nếu điểm đến đã thay đổi quá xa so với kế hoạch ban đầu
    //    else if (Vector3.Distance(this.agent.pathEndPosition, destination) >= 2)
    //    {
    //        this.state = ActionState.IDLE;           // Reset trạng thái
    //        return Node.Status.FAILURE;              // Thất bại
    //    }
    //    // Nếu đã gần đến điểm đích
    //    else if (distanceToTarget < 2)
    //    {
    //        this.state = ActionState.IDLE;           // Reset trạng thái
    //        return Node.Status.SUCCESS;              // Thành công
    //    }

    //    // Nếu chưa tới đích và không thất bại → tiếp tục di chuyển
    //    return Node.Status.RUNNING;
    //}
}
