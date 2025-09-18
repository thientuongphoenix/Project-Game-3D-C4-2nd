using UnityEngine;
using UnityEngine.UI;

public class TimeText : MonoBehaviour
{
    public Text timerText;   // UI Text để hiển thị thời gian
    private float time = 0f; // thời gian tính bằng giây

    void Update()
    {
        time += Time.deltaTime; // tăng thời gian theo frame
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
