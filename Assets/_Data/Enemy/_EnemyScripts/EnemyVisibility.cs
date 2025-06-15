using UnityEngine;
public class EnemyVisibility : SaiMonoBehaviour
{
    [SerializeField] private GameObject renderMesh;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        //this.LoadRenderMesh();
    }

    // protected virtual void LoadRenderMesh()
    // {
    //     if (this.renderMesh != null) return;
    //     this.renderMesh = transform.Find("SkinnedRenderMesh").gameObject;
    //     Debug.Log(transform.name + ": LoadRenderMesh", gameObject);
    // }

    private void OnBecameVisible()
    {
        if (renderMesh != null) renderMesh.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        if (renderMesh != null) renderMesh.SetActive(false);
    }
}
