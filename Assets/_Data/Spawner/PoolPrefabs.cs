using System.Collections.Generic;
using UnityEngine;

public class PoolPrefabs<T> : SaiMonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected List<T> prefabs = new();
    public List<T> PrefabsList => prefabs;

    protected override void Awake()
    {
        base.Awake();
        // this.HidePrefabs();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPrefabs();
        this.HidePrefabs();
    }

    protected virtual void HidePrefabs()
    {
        foreach (T prefab in this.prefabs)
        {
            if (prefab != null) // Thêm null check để tránh NullReferenceException
            {
                prefab.gameObject.SetActive(false);
            }
        }
    }

    protected virtual void LoadPrefabs()
    {
        if(this.prefabs.Count > 0) return;
        foreach(Transform child in transform)
        {
            T classPrefab = child.GetComponent<T>();
            if(classPrefab != null) this.prefabs.Add(classPrefab);
        }
        Debug.Log(transform.name + ": LoadPrefabs", gameObject);
    }

    public virtual T GetRandom()
    {
        if (this.prefabs.Count == 0) return null; // Kiểm tra list rỗng
        int rand = Random.Range(0, this.prefabs.Count);
        return this.prefabs[rand];
    }

    public virtual T GetByName(string prefabName)
    {
        foreach (T prefab in this.prefabs)
        {
            if(prefab != null && prefab.name == prefabName) // Thêm null check và sửa logic
            {
                return prefab;
            }
        }
        return null;
    }
}
