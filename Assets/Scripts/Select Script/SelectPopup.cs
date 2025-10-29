using UnityEngine;
using UnityEngine.UI;
using System;

public class SelectPopup : MonoBehaviour
{
    [Header("��ư ����")]
    public Button yesButton;
    public Button noButton;
    public Button closeButton;

    // �˾� ���� �� ȣ��Ǵ� �̺�Ʈ
    public event Action onClose;
    public event Action onYes; // Yes Ŭ�� �� �߰� �̺�Ʈ

    void Start()
    {
        if (yesButton != null)
            yesButton.onClick.AddListener(OnYes);

        if (noButton != null)
            noButton.onClick.AddListener(ClosePopup);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);
    }

    private void OnYes()
    {
        onYes?.Invoke();
        ClosePopup();
    }

    private void ClosePopup()
    {
        onClose?.Invoke();
        Destroy(gameObject);
    }
}