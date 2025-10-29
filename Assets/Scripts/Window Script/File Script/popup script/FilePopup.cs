using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// ���� �˾� ������ �����ϴ� Ŭ����
/// - �巡�� �̵�
/// - �ݱ� ��ư ó��
/// - �ı� �� PopupManager�� ����ȭ
/// </summary>
public class FilePopup : MonoBehaviour
{
    [Header("UI References")]
    public Button closeButton;          // ������ X ��ư
    public RectTransform topBar;        // �巡�׿� TopBar
    public TMP_Text topBarText;         // TopBar �ؽ�Ʈ

    private RectTransform popupRect;
    private Canvas parentCanvas;
    private Vector2 offset;

    // PopupManager���� ������ Ű (���� �̸�)
    private string fileKey;

    /// <summary>
    /// �˾� ���� �� ���� ������ ����
    /// </summary>
    public void Initialize(string fileName, string fileExtension, Canvas canvas)
    {
        parentCanvas = canvas;
        popupRect = GetComponent<RectTransform>();

        // TopBar �ؽ�Ʈ ����
        if (topBarText != null)
            topBarText.text = $"{fileName}.{fileExtension}";

        // X ��ư Ŭ�� �̺�Ʈ
        if (closeButton != null)
            closeButton.onClick.AddListener(() => Destroy(gameObject));
    }

    /// <summary>
    /// PopupManager���� ������ Ű ����
    /// </summary>
    public void SetFileKey(string key)
    {
        fileKey = key;
    }

    private void OnDestroy()
    {
        // �ı� �� PopupManager�� �˸�
        if (FilePopupManager.Instance != null && !string.IsNullOrEmpty(fileKey))
            FilePopupManager.Instance.OnPopupDestroyed(fileKey);
    }

    #region �巡�� ����
    public void OnTopBarPointerDown(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        if (popupRect == null) return;

        // Ŭ�� ��ġ�� �˾� ��ġ ���� ����
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popupRect, pointerData.position, pointerData.pressEventCamera, out offset);

        // �巡�� ���� �� �ֻ������
        if (popupRect != null)
            popupRect.SetAsLastSibling();
    }

    public void OnTopBarDrag(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        if (popupRect == null || parentCanvas == null) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform, pointerData.position, pointerData.pressEventCamera, out localPoint))
        {
            popupRect.localPosition = localPoint - offset;
        }
    }
    #endregion
}
