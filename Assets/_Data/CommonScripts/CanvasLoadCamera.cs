using UnityEngine;

public class CanvasLoadCamera : SaiMonoBehaviour
{
    [SerializeField] protected Canvas canvas;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCanvas();
    }

    protected virtual void LoadCanvas()
    {
        if (this.canvas != null) return;
        this.canvas = GetComponent<Canvas>();
        this.canvas.renderMode = RenderMode.WorldSpace;
        Camera eventCamera = GameObject.Find("ThirdPersonCamera")?.GetComponent<Camera>();
        if (eventCamera != null)
        {
            this.canvas.worldCamera = eventCamera;
        }
        Debug.Log(transform.name + ": LoadCanvas", gameObject);
    }
}
