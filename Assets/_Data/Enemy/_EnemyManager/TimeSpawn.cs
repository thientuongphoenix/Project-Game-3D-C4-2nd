using UnityEngine;

public class TimeSpawn : MonoBehaviour
{
    [Header("Objects quản lý")]
    public GameObject objectA;
    public GameObject objectB;
    public GameObject objectC;
    public GameObject objectD;
    public GameObject objectBoss;

    [Header("Thời gian kích hoạt")]
    public float time1;
    public float time2;
    public float time3; 
    public float time4;
    public float time5;
    public float timeBoss;

    private float currentTime = 0f;

    void Start()
    {
        // Ban đầu tắt hết
        SetActiveObjects(false, false, false, false, false);
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime < time1) 
        {
            SetActiveObjects(false, false, false, false, false);
        }
        else if (currentTime < time2) //bat a b 
        {
            SetActiveObjects(false, true, true, false, false);
        }
        else if (currentTime < time3) 
        {
            SetActiveObjects(false, false, false, false, false);
        }
        else if (currentTime < time4) 
        {
            SetActiveObjects(false, false, false, true, true);
        }
        else if (currentTime < time5)
        {
            SetActiveObjects(false, false, false, false, false);
        }
        else if (currentTime < timeBoss)
        {
            SetActiveObjects(true, false, true, false, false);
        }
        else // Sau 7 phút
        {
            SetActiveObjects(false, false, false, false, false);
        }
    }

    void SetActiveObjects(bool a, bool b, bool c, bool d, bool e)
    {
        objectA.SetActive(a);
        objectB.SetActive(b);
        objectC.SetActive(c);
        objectD.SetActive(d);
        objectBoss.SetActive(e);
    }
}
