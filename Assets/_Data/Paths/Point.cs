using System.Collections.Generic;
using UnityEngine;

public class Point : SaiMonoBehaviour
{
    [SerializeField] protected Point nextPoint;
    public Point NextPoint => nextPoint;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadNextPoint();
    }

    public virtual void LoadNextPoint()
    {
        if (this.nextPoint != null) return;

        Transform currentPoint = transform;
        Transform parent = currentPoint.parent;
        if (parent == null)
        {
            Debug.Log("Không thể tìm Obj anh em"); //Obj Siblings
            return;
        }

        int siblingIndex = currentPoint.GetSiblingIndex(); //Hàm này có sẵn dùng lấy Obj con anh em
        if(siblingIndex + 1 < parent.childCount)
        {
            this.nextPoint = parent.GetChild(siblingIndex + 1).GetComponent<Point>();
        }
        else
        {
            //Debug.Log("Đây là đứa con cuối cùng rồi!");
        }
    }
}
