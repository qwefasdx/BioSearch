using UnityEngine;

public class EscapeCancel : MonoBehaviour
{
    public GameStateManager gameStateManager;

    void OnMouseDown()
    {
        if (gameStateManager == null)
        {
            Debug.LogWarning("[EscapeCancel] GameStateManager�� �Ҵ�Ǿ� ���� �ʽ��ϴ�!");
            return;
        }

        // �������� Ż�� ��Ҹ� ��û (GameStateManager���� ���� ó��)
        gameStateManager.RequestCancelEscape();
    }
}
