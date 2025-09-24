using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 통합된 파일 아이콘
/// - 확장자별 아이콘 이미지를 ExtensionManager에서 가져옴
/// - 더블클릭 시 PopupManager 통해 파일 열림
/// - 드래그 앤 드롭 지원
/// </summary>
public class FileIcon : MonoBehaviour, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    public Image iconImage;          // 아이콘 이미지
    public TMP_Text fileNameText;    // 파일 이름 표시

    private FileWindow fileWindow;
    private File file;

    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    /// <summary>
    /// 아이콘 초기화
    /// </summary>
    public void Setup(File fileData, FileWindow window)
    {
        file = fileData;
        fileWindow = window;

        if (fileNameText != null)
            fileNameText.text = $"{file.name}.{file.extension}";

        if (iconImage != null && ExtensionManager.Instance != null)
            iconImage.sprite = ExtensionManager.Instance.GetIconForExtension(file.extension);

        SetSelected(false);
    }

    public File GetFile() => file;

    public void SetSelected(bool selected)
    {
        if (fileNameText == null) return;
        fileNameText.color = selected ? selectedColor : normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        fileWindow.SetSelectedFileIcon(this);

        if (eventData.clickCount == 2)
        {
            // 더블클릭 시 PopupManager 통해 파일 열기
            if (PopupManager.Instance != null && file != null)
                PopupManager.Instance.OpenFile(file);
        }
    }

    #region 드래그 구현

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (FolderDragManager.Instance != null)
            FolderDragManager.Instance.BeginDrag(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (FolderDragManager.Instance != null)
            FolderDragManager.Instance.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (FolderDragManager.Instance != null)
            FolderDragManager.Instance.EndDrag();
    }

    #endregion
}
