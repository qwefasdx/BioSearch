using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PathPanelManager : MonoBehaviour
{
    public Transform contentArea;
    public Button pathButtonPrefab;

    private FileWindow fileWindow;
    private List<Button> pathButtons = new List<Button>();

    public void Initialize(FileWindow window)
    {
        fileWindow = window;
    }

    public void UpdatePathButtons()
    {
        // 기존 버튼 제거
        foreach (var btn in pathButtons)
            Destroy(btn.gameObject);
        pathButtons.Clear();

        List<Folder> pathList = fileWindow.GetCurrentPathList();

        for (int i = 0; i < pathList.Count; i++)
        {
            int index = i;
            Button btn = Instantiate(pathButtonPrefab, contentArea);
            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            text.text = pathList[i].name;

            float width = text.preferredWidth + 20f;
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(width, btn.GetComponent<RectTransform>().sizeDelta.y);

            // 클릭 시 이동
            btn.onClick.AddListener(() =>
            {
                fileWindow.NavigateToPathIndex(index);
            });

            // Drop 이벤트 등록
            EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.Drop };
            entry.callback.AddListener((data) =>
            {
                OnPathDrop(index, (PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            pathButtons.Add(btn);
        }
    }

    private void OnPathDrop(int index, PointerEventData eventData)
    {
        Folder draggedFolder = null;

        // 1. FileDragManager에서 현재 드래그 중인 Folder 확인
        if (FileDragManager.Instance.CurrentDraggedFolder != null)
        {
            draggedFolder = FileDragManager.Instance.CurrentDraggedFolder;
        }
        // 2. pointerDrag가 FileIcon이면 가져오기
        else if (eventData.pointerDrag != null)
        {
            FileIcon icon = eventData.pointerDrag.GetComponent<FileIcon>();
            if (icon != null)
                draggedFolder = icon.GetFolder();
        }

        if (draggedFolder == null)
        {
            Debug.LogWarning("드래그 중인 폴더를 찾을 수 없습니다.");
            return;
        }

        List<Folder> pathList = fileWindow.GetCurrentPathList();
        if (index < 0 || index >= pathList.Count) return;

        Folder targetFolder = pathList[index];

        // 자기 자신이나 하위 폴더로 드롭 방지
        Folder temp = targetFolder;
        while (temp != null)
        {
            if (temp == draggedFolder)
            {
                Debug.LogWarning("자신 또는 하위 폴더에는 드롭할 수 없습니다.");
                return;
            }
            temp = temp.parent;
        }

        // 기존 부모에서 제거
        if (draggedFolder.parent != null)
            draggedFolder.parent.children.Remove(draggedFolder);

        // 새 부모에 추가
        targetFolder.children.Add(draggedFolder);
        draggedFolder.parent = targetFolder;

        // Ghost 제거
        FileDragManager.Instance.ForceEndDrag();

        // UI 갱신
        fileWindow.StartCoroutine(OpenFolderNextFrame(targetFolder));
    }

    private IEnumerator OpenFolderNextFrame(Folder folder)
    {
        yield return null; // 한 프레임 대기
        fileWindow.OpenFolder(folder, false);
    }
}
