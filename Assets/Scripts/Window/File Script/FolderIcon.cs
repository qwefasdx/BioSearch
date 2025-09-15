using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FolderIcon : MonoBehaviour, IPointerClickHandler, IDropHandler
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
        FolderIcon dragged = eventData.pointerDrag?.GetComponent<FolderIcon>();
        if (dragged == null) return;

        // 1) FileDragManager의 OnEndDrag를 EventSystem 흐름으로 직접 호출
        if (FolderDragManager.Instance != null)
        {
            ExecuteEvents.Execute(FolderDragManager.Instance.gameObject, eventData, ExecuteEvents.endDragHandler);
        }

        // 2) 폴더 이동 로직
        Folder source = dragged.GetFolder();
        Folder target = folder;

        // 깊이 제한 확인
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

        // 3) UI 갱신은 다음 프레임으로 지연 이벤트 시스템이 정리될 시간 확보
        fileWindow.StartCoroutine(OpenFolderNextFrame(target));
    }


    private System.Collections.IEnumerator OpenFolderNextFrame(Folder target)
    {
        yield return null; // 한 프레임 대기
        fileWindow.OpenFolder(target, false);
    }

}
