using UnityEngine;
using TMPro; // can cho TextMeshPro

public class ClockUI : MonoBehaviour
{
    public DayAndNightController dayNightController; // tham chieu script ban da co
    public TextMeshProUGUI clockText; // tham chieu den UI Text

    void Update()
    {
        // lay gio trong game tu script DayAndNightController2D
        float timeOfDay = dayNightController.timeOfDay;

        // chuyen doi sang gio phut
        int hour = Mathf.FloorToInt(timeOfDay);
        int minute = Mathf.FloorToInt((timeOfDay - hour) * 60);

        // hien thi len UI
        clockText.text = string.Format("{0:00}:{1:00}", hour, minute);
    }
}