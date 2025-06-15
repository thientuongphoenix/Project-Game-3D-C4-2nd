using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree()
    {
        // name này là name trong node
        // gọi qua đây để khai báo tên của cây, vd như NPC Behavior hoặc Boss chẳng hạn
        this.name = "Tree";
    }

    public BehaviorTree(string n)
    {
        // Lấy chính tên cây để đặt tên cho node đầu tiên, đây chính là root node
        this.name = n;
    }

    public override Status Process() //Ghi đè lại Process của Node
    {
        if (children.Count == 0) return Status.SUCCESS;
        //Khi Behave chạy là chưa có Code nào trong cây hết,
        //dòng này trả ra SUCCESS để cây thêm con vô rồi chạy lại, chứ không nó lỗi OutOfRange
        return children[currentChild].Process(); //Chạy từ node con
    }

    struct NodeLevel
    {
        public int level;
        public Node node;
    }

    public void PrintTree()
    {
        string treePrintout = "";
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        Node currentNode = this;
        nodeStack.Push(new NodeLevel { level = 0, node = currentNode });

        while (nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            treePrintout += new string('-', nextNode.level) + nextNode.node.name + "\n";
            for(int i = nextNode.node.children.Count - 1; i >= 0; i--)
            {
                nodeStack.Push( new NodeLevel { level = nextNode.level + 1, node = nextNode.node.children[i] });
            }
        }

        Debug.Log(treePrintout);
    }
}
