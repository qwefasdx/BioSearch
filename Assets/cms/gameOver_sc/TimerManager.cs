using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float totalTime = 60f;
    private float currentTime;
    private bool isRunning = false;

    private GameOverManager gameOverManager;

    void Start()
    {
        currentTime = totalTime;
        UpdateTimerText();
        gameOverManager = FindObjectOfType<GameOverManager>();

        //  자동 시작 제거
        // isRunning = true;  ← 이 줄 삭제
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        UpdateTimerText();

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;

            if (gameOverManager != null)
                gameOverManager.TriggerGameOver("시간 초과로 인한 게임 오버");

            
        }
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            Debug.Log("[TimerManager] 타이머 시작!");
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("[TimerManager] 타이머 정지!");
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
    }
}
