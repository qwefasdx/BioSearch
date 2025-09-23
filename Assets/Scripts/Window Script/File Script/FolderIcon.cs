using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class FolderIcon : MonoBehaviour, IPointerClickHandler, IDropHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
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

    public void OnDrop(PointerEventData eventData)
    {
        // 1. 폴더 드롭
        FolderIcon draggedFolderIcon = eventData.pointerDrag?.GetComponent<FolderIcon>();
        if (draggedFolderIcon != null)
        {
            Folder source = draggedFolderIcon.GetFolder();
            Folder target = folder;

            string warning;
            if (!FolderDepthUtility.CanMove(source, target, out warning))
            {
                LogWindowManager.Instance.Log(warning);
                return;
            }

            if (source.parent != null)
                source.parent.children.Remove(source);

            target.children.Add(source);
            source.parent = target;

            LogWindowManager.Instance.Log($"폴더 '{source.name}' → '{target.name}' 이동됨");

            FolderDragManager.Instance.ForceEndDrag();
            fileWindow.StartCoroutine(OpenFolderNextFrame(target));
            return;
        }

        // 2. 파일 드롭
        FileIcon draggedFileIcon = eventData.pointerDrag?.GetComponent<FileIcon>();
        if (draggedFileIcon != null)
        {
            File file = draggedFileIcon.GetFile();
            Folder target = folder;

            if (file.parent != null)
                file.parent.files.Remove(file);

            target.files.Add(file);
            file.parent = target;

            LogWindowManager.Instance.Log($"파일 '{file.name}.{file.extension}' → '{target.name}' 이동됨");

            FolderDragManager.Instance.ForceEndDrag();
            fileWindow.RefreshWindow(); // 파일 이동 후 UI 갱신
        }
    }

    private IEnumerator OpenFolderNextFrame(Folder target)
    {
        yield return null;
        fileWindow.OpenFolder(target, false);
    }
}
