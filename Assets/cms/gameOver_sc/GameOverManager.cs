using UnityEngine;
using TMPro; // UI 표시용 (선택)

public class GameOverManager : MonoBehaviour
{
    private bool isGameOver = false;

    [Header("UI 설정 (선택 사항)")]
    public TextMeshProUGUI gameOverText; //  게임오버 메시지 TMP 연결용

    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log($"[GameOverManager] 게임 오버 발생! 이유: {reason}");

        // UI 표시 (임시 텍스트)
        if (gameOverText != null)
        {
            gameOverText.text = $"GAME OVER\n({reason})";
            gameOverText.gameObject.SetActive(true);
        }

       
    }

    public void ResetGameOver()
    {
        isGameOver = false;
        Time.timeScale = 1f;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
