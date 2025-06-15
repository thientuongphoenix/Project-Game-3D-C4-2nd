using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public delegate Status TickM(int index); //M là multiple, dùng cho cái list tranh
    public TickM ProcessMethodM;

    public int index;

    public Leaf() { }

    public Leaf(string n, Tick pm)
    {
        this.name = n;
        this.ProcessMethod = pm;
    }

    public Leaf(string n, int i, TickM pm)
    {
        this.name = n;
        this.ProcessMethodM = pm;
        this.index = i;
    }

    public Leaf(string n, Tick pm, int order)
    {
        this.name = n;
        this.ProcessMethod = pm;
        this.sortOrder = order;
    }

    public override Status Process()
    {
        Node.Status s;
        if(ProcessMethod != null)
        {
            s = ProcessMethod();
        }
        else if(ProcessMethodM != null)
        {
            s = ProcessMethodM(index);
        }
        else s = Status.FAILURE;
        
        Debug.Log(name + " " + s);
        return s;
    }
}
