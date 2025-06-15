using UnityEngine;

public class CrosshairPointer : SaiMonoBehaviour
{
    protected float maxDistance = 100f;
    protected Collider hitObj;
    [SerializeField] LayerMask layerMask = -1;

    protected virtual void Update()
    {
        this.Pointing();
    }

    protected virtual void Pointing()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            transform.position = hit.point;
            this.hitObj = hit.collider;
        }
        // else
        // {
        //     // Nếu không có va chạm, đặt crosshair ở vị trí xa nhất có thể
        //     transform.position = ray.origin + ray.direction * maxDistance;
        // }
    }
}
