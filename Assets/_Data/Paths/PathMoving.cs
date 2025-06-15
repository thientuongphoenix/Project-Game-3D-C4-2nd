using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PathMoving : SaiMonoBehaviour
{
    [SerializeField] protected List<Point> points = new();
    public Point GetPoint(int pointNumber) => points[pointNumber];

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPoints();
    }

    public virtual void LoadPoints()
    {
        if (this.points.Count > 0) return;
        foreach (Transform child in transform)
        {
            Point point = child.GetComponent<Point>();
            point.LoadNextPoint();
            this.points.Add(point);
        }
        Debug.Log(transform.name + ": LoadPoints", gameObject);
    }
}
