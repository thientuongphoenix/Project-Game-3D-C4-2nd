using UnityEngine;

public class TimeSpawn1 : MonoBehaviour
{
    [Header("Objects quản lý")]
    public GameObject objectA;
    public GameObject objectB;


    [Header("Thời gian kích hoạt")]
    public float time1;
    public float time2;
    private float currentTime = 0f;

    void Start()
    {
        // Ban đầu tắt hết
        SetActiveObjects(false, false);
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime < time1) 
        {
            SetActiveObjects(false, false);
        }
        else if (currentTime < time2) 
        {
            SetActiveObjects(true, true);
        }
        else // Sau 7 phút
        {
            SetActiveObjects(false, false);
        }
    }

    void SetActiveObjects(bool a, bool b)
    {
        objectA.SetActive(a);
        objectB.SetActive(b);
    }
}
