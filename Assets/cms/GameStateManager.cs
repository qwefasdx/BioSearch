using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        Normal,          // 기본 상태
        EscapePreparing, // 탈주 진행중
        Escape           // 탈주 (완전 발동)
    }

    public GameState currentState = GameState.Normal;

    public float escapeDelay = 3f; // 몇 초 뒤 탈주 확정

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentState == GameState.Normal)
        {
            ChangeState(GameState.EscapePreparing);
            StartCoroutine(EscapeDelayCoroutine());
        }
    }

    void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("상태 변경: " + currentState);

        switch (currentState)
        {
            case GameState.Normal:
                // TODO: 일반 상태 효과
                break;
            case GameState.EscapePreparing:
                // TODO: 탈주 진행중 (경고, UI, 소리 차단 준비 등)
                break;
            case GameState.Escape:
                // TODO: 탈주 발동 (적 등장, 모니터 끄기 등)
                break;
        }
    }

    IEnumerator EscapeDelayCoroutine()
    {
        yield return new WaitForSeconds(escapeDelay);
        if (currentState == GameState.EscapePreparing)
        {
            ChangeState(GameState.Escape);
        }
    }
}
