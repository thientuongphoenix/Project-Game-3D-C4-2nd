using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status { SUCCESS, RUNNING, FAILURE };
    public Status status;
    public List<Node> children = new List<Node>();
    public int currentChild = 0;
    public string name;
    public int sortOrder;

    public Node() { }

    /// <summary>
    /// Dùng để đặt tên cho Node mới
    /// </summary>
    /// <param name="n">Tên đặt cho Node</param>
    public Node(string n)
    {
        this.name = n;
    }

    /// <summary>
    /// Dùng để đặt tên và đặt mức độ ưu tiên cho Node
    /// </summary>
    /// <param name="n">Tên Node</param>
    /// <param name="order">Mức độ ưu tiên</param>
    public Node(string n, int order)
    {
        this.name = n;
        this.sortOrder = order;
    }

    public void Reset()
    {
        foreach(Node n in children)
        {
            n.Reset();
        }
        currentChild = 0;
    }

    public virtual Status Process()
    {
        return children[currentChild].Process();
    }

    /// <summary>
    /// Dùng để thêm Node con vào
    /// </summary>
    /// <param name="n">Node con muốn thêm vào</param>
    public void AddChild(Node n)
    {
        children.Add(n);
    }
}
