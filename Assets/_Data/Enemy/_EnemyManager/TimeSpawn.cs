using UnityEngine;

public class TimeSpawn : MonoBehaviour
{
    [Header("Objects quản lý")]
    public GameObject objectA;
    public GameObject objectB;
    public GameObject objectC;
    public GameObject objectD;

    [Header("Thời gian kích hoạt")]
    public float timeA;
    public float timeB;
    public float timeC; 
    public float timeD;

    private float currentTime = 0f;

    void Start()
    {
        // Ban đầu tắt hết
        SetActiveObjects(false, false, false, false);
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime < timeA) 
        {
            SetActiveObjects(false, false, false, false);
        }
        else if (currentTime < timeB) 
        {
            SetActiveObjects(true, false, false, false);
        }
        else if (currentTime < timeC) 
        {
            SetActiveObjects(false, true, false, false);
        }
        else if (currentTime < timeD) 
        {
            SetActiveObjects(false, false, true, false);
        }
        else // Sau 7 phút
        {
            SetActiveObjects(false, false, true, true);
        }
    }

    void SetActiveObjects(bool a, bool b, bool c, bool d)
    {
        objectA.SetActive(a);
        objectB.SetActive(b);
        objectC.SetActive(c);
        objectD.SetActive(d);
    }
}
