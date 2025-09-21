using UnityEngine;
using UnityEngine.UI;

public class TimeTest : MonoBehaviour
{
    public Text timerText;      // Kéo UI Text vào đây
    public float startTime = 0; // có thể set sẵn thời gian bắt đầu (tính bằng giây)

    private float timer;

    void Start()
    {
        timer = startTime;
    }

    void Update()
    {
        // Cộng dồn thời gian (nếu muốn đếm lên)
        timer += Time.deltaTime;

        // Tính phút và giây
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        // Format mm:ss
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
