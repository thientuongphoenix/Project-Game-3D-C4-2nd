using UnityEngine;

public class Worker : BTAgent
{
    public GameObject office;
    GameObject patron;
    
    public override void Start()
    {
        base.Start();
        Leaf patronStillWaiting = new Leaf("Is Patron Waiting", PatronWaiting);
        Leaf allocatePatron = new Leaf("Allocate Patron", AllocatePatron);
        Leaf goToPatron = new Leaf("Go To Patron", GoToPatron);
        Leaf goToOffice = new Leaf("Go To Office", GoToOffice);

        Sequence getPatron = new Sequence("Find a Patron");
        getPatron.AddChild(allocatePatron);

        BehaviorTree waiting = new BehaviorTree();
        waiting.AddChild(patronStillWaiting);

        DepSequence moveToPatron = new DepSequence("Move To Patron", waiting, agent);
        moveToPatron.AddChild(goToPatron);

        getPatron.AddChild(moveToPatron);

        Selector beWorker = new Selector("Be A Worker");
        beWorker.AddChild(getPatron);
        beWorker.AddChild(goToOffice);

        tree.AddChild(beWorker);

        tree.PrintTree();

        // Cây hành vi của nhân viên:
        // Root
        // └── Be A Worker (Selector)
        //     ├── Find a Patron (Sequence)
        //     │   ├── Allocate Patron (Leaf)
        //     │   └── Move To Patron (DepSequence)
        //     │       ├── Is Patron Waiting (BehaviorTree)
        //     │       │   └── Is Patron Waiting (Leaf)
        //     │       └── Go To Patron (Leaf)
        //     └── Go To Office (Leaf)
    }

    public Node.Status PatronWaiting()
    {
        if(patron == null) return Node.Status.FAILURE;
        if(patron.GetComponent<PatronBehavior>().isWaiting) return Node.Status.SUCCESS;
        return Node.Status.FAILURE;
    }

    public Node.Status AllocatePatron()
    {
        if(Blackboard.Instance.patrons.Count == 0) return Node.Status.FAILURE;
        this.patron = Blackboard.Instance.patrons.Pop();
        if(patron == null) return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToPatron()
    {
        if(patron == null) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(patron.transform.position);
        if(s == Node.Status.SUCCESS)
        {
            patron.GetComponent<PatronBehavior>().ticket = true;
            patron = null;
        }
        return s;
    }

    public Node.Status GoToOffice()
    {
        Node.Status s = GotoLocation(office.transform.position);
        patron = null;
        return s;
    }
}
