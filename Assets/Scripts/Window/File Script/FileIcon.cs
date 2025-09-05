using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FileIcon : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public TMP_Text fileNameText;

    private FileWindow fileWindow;
    private Folder folder;

    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    public void Setup(Folder folder, FileWindow window, bool parentAbnormal = false)
    {
        this.folder = folder;
        this.fileWindow = window;

        bool isAbnormal = parentAbnormal || folder.isAbnormal;

        if (fileNameText != null)
            fileNameText.text = folder.name;

        SetSelected(false);

        if (fileNameText != null && isAbnormal)
            fileNameText.color = Color.red;
    }

    public Folder GetFolder() => folder;

    public void SetSelected(bool selected)
    {
        if (fileNameText == null) return;
        if (folder != null && folder.isAbnormal) return;
        fileNameText.color = selected ? selectedColor : normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        fileWindow.SetSelectedIcon(this);
        if (eventData.clickCount == 2)
            fileWindow.OpenFolder(folder);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("드랍됨");
        FileIcon dragged = eventData.pointerDrag?.GetComponent<FileIcon>();
        if (dragged == null) return;

        Folder source = dragged.GetFolder();
        Folder target = folder;

        if (source.parent != null)
            source.parent.children.Remove(source);

        target.children.Add(source);
        source.parent = target;

        fileWindow.OpenFolder(target, false);

        // 드롭 직후 Ghost 제거 (EndDrag를 LateUpdate로 호출)
        
        FileDragManager.Instance.ForceEndDrag();
    }

}
