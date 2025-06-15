using UnityEngine;

public abstract class SaiSingleton<T> : SaiMonoBehaviour where T : SaiMonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError(typeof(T).Name + ": Instance chưa được khởi tạo thì phải :D");
            }

            return _instance;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.LoadInstance();
    }

    protected virtual void LoadInstance()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if(transform.parent == null) DontDestroyOnLoad(gameObject);
            return;
        }
        else if (_instance != this)
        {
            Debug.LogError("Có 2 Instance trong 1 scene, chỉ được có 1 cái thôi!");
        }
    }
}
