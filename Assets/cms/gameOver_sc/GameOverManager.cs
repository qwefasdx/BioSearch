using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    private bool isGameOver = false;
    public float returnDelay = 5f; // 5�� �� ����
    public string startSceneName = "StartScene"; // ������ �� �̸�

    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return; // �ߺ� ȣ�� ����
        isGameOver = true;

        Debug.Log($"[GameOver] �߻�! ����: {reason}");
        StartCoroutine(ReturnToStartScene());
    }

    private IEnumerator ReturnToStartScene()
    {
        Debug.Log("[GameOverManager] 5�� �� ���� ȭ������ �����մϴ�...");
        yield return new WaitForSeconds(returnDelay);
        SceneManager.LoadScene(startSceneName);
    }

    public void ResetGameOver()
    {
        isGameOver = false;
        Debug.Log("[GameOverManager] ���� �ʱ�ȭ��.");
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
