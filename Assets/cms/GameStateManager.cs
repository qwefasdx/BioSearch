using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        Normal,
        EscapePreparing,
        Escape
    }

    public GameState currentState = GameState.Normal;
    public float escapeDelay = 3f;

    private Coroutine escapeCoroutine;

    [Header("카메라 설정")]
    public Camera mainCamera; // Inspector에서 카메라 지정

    void Update()
    {
        // E키 누르면 탈주 준비 시작
        if (Input.GetKeyDown(KeyCode.E) && currentState == GameState.Normal)
        {
            ChangeState(GameState.EscapePreparing);
            escapeCoroutine = StartCoroutine(EscapeDelayCoroutine());
        }

        // 마우스 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            CheckClickOnObject();
        }
    }

    void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("상태 변경: " + currentState);
    }

    IEnumerator EscapeDelayCoroutine()
    {
        yield return new WaitForSeconds(escapeDelay);

        // 탈주 진행 중이었으면 Escape 상태로 전환
        if (currentState == GameState.EscapePreparing)
        {
            ChangeState(GameState.Escape);
        }
    }

    void CheckClickOnObject()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("클릭한 오브젝트: " + hit.collider.name);

            // EscapeCancelObject 태그면 언제든 탈주 취소
            if (hit.collider.CompareTag("EscapeCancelObject"))
            {
                CancelEscape();
            }
        }
    }

    void CancelEscape()
    {
        // 현재 상태 상관없이 탈주 취소
        if (escapeCoroutine != null)
        {
            StopCoroutine(escapeCoroutine);
            escapeCoroutine = null;
        }

        ChangeState(GameState.Normal);
        Debug.Log("탈주 상태 취소됨!");
    }
}
