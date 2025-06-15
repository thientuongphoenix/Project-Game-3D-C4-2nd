using UnityEngine;

public class FlyToTarget : SaiMonoBehaviour
{
    protected Transform target;
    protected float speed = 27f;

    protected void Update()
    {
        this.Flying();
    }

    public virtual void SetTarget(Transform target)
    {
        this.target = target;
        transform.parent.LookAt(target);
    }

    protected virtual void Flying()
    {
        if(this.target == null) return;
        transform.parent.Translate(this.speed * Time.deltaTime * Vector3.forward);
    }
}
