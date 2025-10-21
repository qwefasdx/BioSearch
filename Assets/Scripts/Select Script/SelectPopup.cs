using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 수용 / 방출 팝업창 관리 스크립트
/// Yes / No / X 버튼 모두 닫기 가능
/// </summary>
public class SelectPopup : MonoBehaviour
{
    [Header("UI 연결")]
    public Button yesButton;      // YES 버튼
    public Button noButton;       // NO 버튼
    public Button closeButton;    // X 버튼

    // 팝업이 닫힐 때 호출되는 이벤트 (PopupManager가 이걸 받음)
    public event Action onClose;

    void Start()
    {
        // 버튼 이벤트 등록
        if (yesButton != null)
            yesButton.onClick.AddListener(OnYes);

        if (noButton != null)
            noButton.onClick.AddListener(OnNo);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);
    }

    /// <summary>
    /// YES 버튼 클릭 시 동작
    /// </summary>
    private void OnYes()
    {
        Debug.Log("사용자가 YES를 선택했습니다.");
        ClosePopup();
    }

    /// <summary>
    /// NO 버튼 클릭 시 동작
    /// </summary>
    private void OnNo()
    {
        Debug.Log("사용자가 NO를 선택했습니다.");
        ClosePopup();
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    private void ClosePopup()
    {
        onClose?.Invoke();
        Destroy(gameObject);
    }
}
