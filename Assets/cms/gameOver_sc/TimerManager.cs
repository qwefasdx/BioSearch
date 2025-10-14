using UnityEngine;
using TMPro;  // TextMeshPro 사용 시 필요

public class TimeManager : MonoBehaviour
{
    public float gameDuration = 60f; // 제한 시간
    private float remainingTime;

    public TextMeshProUGUI timerText; //  UI 연결용
    public bool IsTimeOver => remainingTime <= 0f;

    void Start()
    {
        remainingTime = gameDuration;
        UpdateTimerUI();
    }

    void Update()
    {
        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public void ResetTimer()
    {
        remainingTime = gameDuration;
        UpdateTimerUI();
    }
}
