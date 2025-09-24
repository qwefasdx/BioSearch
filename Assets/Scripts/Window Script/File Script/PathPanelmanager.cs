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
            int index = i; // 클로저 문제 방지
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
        File draggedFile = null;

        // 1. FolderDragManager에서 현재 드래그 중인 Folder 확인
        if (FolderDragManager.Instance.CurrentDraggedFolderIcon != null)
        {
            draggedFolder = FolderDragManager.Instance.CurrentDraggedFolderIcon.GetFolder();
        }
        // 2. pointerDrag가 FolderIcon이면 가져오기
        else if (eventData.pointerDrag != null)
        {
            FolderIcon folderIcon = eventData.pointerDrag.GetComponent<FolderIcon>();
            if (folderIcon != null)
                draggedFolder = folderIcon.GetFolder();
            else
            {
                // pointerDrag가 FileIcon이면 가져오기
                FileIcon fileIcon = eventData.pointerDrag.GetComponent<FileIcon>();
                if (fileIcon != null)
                    draggedFile = fileIcon.GetFile();
            }
        }

        List<Folder> pathList = fileWindow.GetCurrentPathList();
        if (index < 0 || index >= pathList.Count) return;

        Folder targetFolder = pathList[index];

        // 폴더 이동 처리
        if (draggedFolder != null)
        {
            // 자기 자신이나 하위 폴더로 드롭 방지
            Folder temp = targetFolder;
            while (temp != null)
            {
                if (temp == draggedFolder)
                {
                    LogWindowManager.Instance.Log("자신 또는 하위 폴더에는 드롭할 수 없습니다.");
                    return;
                }
                temp = temp.parent;
            }

            string warning;
            if (!FolderDepthUtility.CanMove(draggedFolder, targetFolder, out warning))
            {
                LogWindowManager.Instance.Log(warning);
                return;
            }

            // 기존 부모에서 제거
            if (draggedFolder.parent != null)
                draggedFolder.parent.children.Remove(draggedFolder);

            // 새 부모에 추가
            targetFolder.children.Add(draggedFolder);
            draggedFolder.parent = targetFolder;

            FolderDragManager.Instance.EndDrag();
            LogWindowManager.Instance.Log($"폴더 '{draggedFolder.name}' → '{targetFolder.name}' 이동됨");
        }
        // 파일 이동 처리
        else if (draggedFile != null)
        {
            if (draggedFile.parent != null)
                draggedFile.parent.files.Remove(draggedFile); // 기존 부모 폴더에서 제거

            draggedFile.parent = targetFolder;
            targetFolder.files.Add(draggedFile);

            FolderDragManager.Instance.EndDrag();
            LogWindowManager.Instance.Log($"파일 '{draggedFile.name}' → '{targetFolder.name}' 이동됨");
        }

        // UI 갱신
        fileWindow.StartCoroutine(OpenFolderNextFrame(targetFolder));
    }

    private IEnumerator OpenFolderNextFrame(Folder folder)
    {
        yield return null; // 한 프레임 대기
        fileWindow.OpenFolder(folder, false);
    }
}
