using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public abstract class FileIcon : MonoBehaviour, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TMP_Text fileNameText;

    protected FileWindow fileWindow;
    protected File file;

    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    public virtual void Setup(File file, FileWindow window)
    {
        this.file = file;
        this.fileWindow = window;

        if (fileNameText != null)
            fileNameText.text = $"{file.name}.{file.extension}";

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
            OnDoubleClick();
    }

    protected abstract void OnDoubleClick();

    #region 드래그 구현

    public void OnBeginDrag(PointerEventData eventData)
    {
        FolderDragManager.Instance.BeginDrag(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        FolderDragManager.Instance.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        FolderDragManager.Instance.EndDrag();
    }

    #endregion
}
