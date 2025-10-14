using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public float gameDuration = 60f; // 제한 시간
    private float remainingTime;

    public TextMeshProUGUI timerText; // UI 연결용
    public bool IsTimeOver => remainingTime <= 0f;

    private bool isGameOverTriggered = false; // 중복 호출 방지

    void Start()
    {
        remainingTime = gameDuration;
        UpdateTimerUI();
    }

    void Update()
    {
        if (isGameOverTriggered) return; // 이미 끝났으면 중단

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);
        UpdateTimerUI();

        if (remainingTime <= 0f)
        {
            isGameOverTriggered = true;
            OnTimeOver(); //  시간 다 됐을 때 호출
        }
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

    void OnTimeOver()
    {
        Debug.Log("[TimeManager] 제한 시간 종료! 게임 오버 조건 발동 가능");
        // TODO: 나중에 GameOverManager로 연결 예정
        // ex) FindObjectOfType<GameOverManager>()?.TriggerGameOver();
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public void ResetTimer()
    {
        remainingTime = gameDuration;
        isGameOverTriggered = false;
        UpdateTimerUI();
    }
}
