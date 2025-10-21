using UnityEngine;
using UnityEngine.UI;
using System;

public class SelectPopup : MonoBehaviour
{
    [Header("버튼 연결")]
    public Button yesButton;
    public Button noButton;
    public Button closeButton;

    // 팝업 닫힐 때 호출되는 이벤트
    public event Action onClose;
    public event Action onYes; // Yes 클릭 시 추가 이벤트

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