using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    private bool isGameOver = false;

    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log($"[GameOver] 이유: {reason}");
        // TODO: 게임오버 UI / 씬 전환 등
    }
}
