using UnityEngine;

public class Inverter : Node
{
    public Inverter(string n)
    {
        name = n;
    }

    //Inverter phải có đúng 1 đứa con duy nhất để đảo ngược đúng kết quả hành động.
    public override Status Process()
    {
        Status childstatus = children[0].Process(); //Chỉ đảo 1 node con duy nhất
        Debug.Log("Inverter: childStatus = " + childstatus);
        //Đảo ngược trạng thái node
        if(childstatus == Status.RUNNING) return Status.RUNNING;
        if(childstatus == Status.FAILURE) return Status.SUCCESS;
        else return Status.FAILURE;
    }
}
