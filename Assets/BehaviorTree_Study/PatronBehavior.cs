using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatronBehavior : BTAgent
{
    public GameObject[] art;
    public GameObject frontdoor;
    public GameObject home;

    [Range(0, 1000)]
    public int boredom = 0;

    public bool ticket = false;
    public bool isWaiting = false;

    public override void Start()
    {
        base.Start();
        RSelector selectObject = new RSelector("Select Art To View");
        for(int i = 0; i < art.Length; i++)
        {
            Leaf gta = new Leaf("Go To " + art[i].name, i, GoToArt);
            selectObject.AddChild(gta);
        }

        Leaf goToFrontDoor = new Leaf("Go To Frontdoor", GoToFrontDoor);
        Leaf goToHome = new Leaf("Go To Home", GoToHome);
        Leaf isBored = new Leaf("Is Bored?", IsBored);
        Leaf isOpen = new Leaf("Is Open?", IsOpen);

        Sequence viewArt = new Sequence("View Art");
        viewArt.AddChild(isOpen);
        viewArt.AddChild(isBored);
        viewArt.AddChild(goToFrontDoor);

        Leaf noTicket = new Leaf("Wait For Ticket", NoTicket);
        Leaf isWaiting = new Leaf("Waiting For Worker", IsWaiting);

        BehaviorTree waitForTicket = new BehaviorTree();
        waitForTicket.AddChild(noTicket);

        Loop getTicket = new Loop("Ticket", waitForTicket);
        getTicket.AddChild(isWaiting);

        viewArt.AddChild(getTicket);

        BehaviorTree whileBored = new BehaviorTree();
        whileBored.AddChild(isBored);

        Loop lookAtPainting = new Loop("Look", whileBored);
        lookAtPainting.AddChild(selectObject);

        viewArt.AddChild(lookAtPainting);

        viewArt.AddChild(goToHome);

        BehaviorTree galleryOpenCondition = new BehaviorTree();
        galleryOpenCondition.AddChild(isOpen);
        DepSequence bePatron = new DepSequence("Be A Patron", galleryOpenCondition, agent);
        bePatron.AddChild(viewArt);

        Selector viewArtWithFallback = new Selector("View Art With Fallback");
        viewArtWithFallback.AddChild(bePatron);
        viewArtWithFallback.AddChild(goToHome);

        tree.AddChild(viewArtWithFallback);

        StartCoroutine("IncreaseBoredom");

        tree.PrintTree();

        // Cây hành vi của khách tham quan:
        // Root
        // └── View Art With Fallback (Selector)
        //     ├── Be A Patron (DepSequence)
        //     │   ├── Gallery Open Condition (BehaviorTree)
        //     │   │   └── Is Open? (Leaf)
        //     │   └── View Art (Sequence)
        //     │       ├── Is Open? (Leaf)
        //     │       ├── Is Bored? (Leaf)
        //     │       ├── Go To Frontdoor (Leaf)
        //     │       ├── Ticket (Loop)
        //     │       │   ├── Wait For Ticket (BehaviorTree)
        //     │       │   │   └── Wait For Ticket (Leaf)
        //     │       │   └── Waiting For Worker (Leaf)
        //     │       ├── Look (Loop)
        //     │       │   ├── While Bored (BehaviorTree)
        //     │       │   │   └── Is Bored? (Leaf)
        //     │       │   └── Select Object (RSelector)
        //     │       │       ├── Go To Art 1 (Leaf)
        //     │       │       ├── Go To Art 2 (Leaf)
        //     │       │       └── Go To Art N (Leaf)
        //     │       └── Go To Home (Leaf)
        //     └── Go To Home (Leaf)
    }

    IEnumerator IncreaseBoredom()
    {
        while(true)
        {
            this.boredom = Mathf.Clamp(this.boredom + 20, 0, 1000);
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    public Node.Status GoToArt(int i)
    {
        if(!this.art[i].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.art[i].transform.position);
        if(s == Node.Status.SUCCESS)
        {
            this.boredom = Mathf.Clamp(this.boredom - 150, 0, 1000);
        }
        return s;
    }

    public Node.Status GoToFrontDoor()
    {
        Node.Status s = base.GoToDoor(frontdoor);
        
        return s;
    }

    public Node.Status GoToHome()
    {
        Node.Status s = GotoLocation(this.home.transform.position);
        isWaiting = false;
        return s;
    }

    public Node.Status IsBored()
    {
        if(this.boredom < 100)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status NoTicket()
    {
        if(this.ticket || IsOpen() == Node.Status.FAILURE)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status IsWaiting()
    {
        if(Blackboard.Instance.RegisterPatrol(this.gameObject))
        {
            isWaiting = true;
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }
}
