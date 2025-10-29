using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PathPanelManager : MonoBehaviour
{
    // ��� ��ư���� ��ġ�� �θ� ������Ʈ (��: HorizontalLayoutGroup�� �ִ� ����)
    public Transform contentArea;

    // ��� ��ư ������ (��: "Root > SubFolder1 > SubFolder2" ��ư �� �ϳ��� �⺻ ����)
    public Button pathButtonPrefab;

    // ���� ���� â ���� (��� �̵� �� Ȱ��)
    private FileWindow fileWindow;

    // ������ ��� ��ư���� �����ϴ� ����Ʈ
    private List<Button> pathButtons = new List<Button>();

    // FileWindow ���� (�ʱ�ȭ �� ȣ��)
    public void Initialize(FileWindow window)
    {
        fileWindow = window;
    }

    // ���� ������ ��� ��ư���� ���� (��: Root > Documents > Images)
    public void UpdatePathButtons()
    {
        // ���� ��ư ����
        foreach (var btn in pathButtons)
            Destroy(btn.gameObject);
        pathButtons.Clear();

        // ���� ��� ���� ����Ʈ ��������
        List<Folder> pathList = fileWindow.GetCurrentPathList();

        // �� ���� ��θ��� ��ư ����
        for (int i = 0; i < pathList.Count; i++)
        {
            int index = i; // Ŭ���� ���� ����
            Button btn = Instantiate(pathButtonPrefab, contentArea);
            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            text.text = pathList[i].name;

            // �ؽ�Ʈ ���̿� ���� ��ư �� �ڵ� ����
            float width = text.preferredWidth + 20f;
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(width, btn.GetComponent<RectTransform>().sizeDelta.y);

            // Ŭ�� �� �ش� ��η� �̵�
            btn.onClick.AddListener(() =>
            {
                fileWindow.NavigateToPathIndex(index);
            });

            // ��� �̺�Ʈ ��� (���� �Ǵ� ������ ��ư ���� ����� ���)
            EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.Drop };
            entry.callback.AddListener((data) =>
            {
                OnPathDrop(index, (PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            // ������ ��ư ����Ʈ�� ����
            pathButtons.Add(btn);
        }
    }

    // ��� ��ư ���� ��ӵ� �� ����Ǵ� ó��
    private void OnPathDrop(int index, PointerEventData eventData)
    {
        Folder draggedFolder = null;
        File draggedFile = null;

        // 1. FolderDragManager���� ���� �巡�� ���� Folder Ȯ��
        if (FolderDragManager.Instance.CurrentDraggedFolderIcon != null)
        {
            draggedFolder = FolderDragManager.Instance.CurrentDraggedFolderIcon.GetFolder();
        }
        // 2. pointerDrag�κ��� ���� ��������
        else if (eventData.pointerDrag != null)
        {
            FolderIcon folderIcon = eventData.pointerDrag.GetComponent<FolderIcon>();
            if (folderIcon != null)
                draggedFolder = folderIcon.GetFolder();
            else
            {
                // pointerDrag�� FileIcon�̸� ��������
                FileIcon fileIcon = eventData.pointerDrag.GetComponent<FileIcon>();
                if (fileIcon != null)
                    draggedFile = fileIcon.GetFile();
            }
        }

        // ��ȿ�� �ε��� ���� Ȯ��
        List<Folder> pathList = fileWindow.GetCurrentPathList();
        if (index < 0 || index >= pathList.Count) return;

        Folder targetFolder = pathList[index];

        // ���� �̵� ó��
        if (draggedFolder != null)
        {
            // �ڱ� �ڽ��̳� ���� ������ ��� ����
            Folder temp = targetFolder;
            while (temp != null)
            {
                if (temp == draggedFolder)
                {
                    LogWindowManager.Instance.Log("�ڽ� �Ǵ� ���� �������� ����� �� �����ϴ�.");
                    return;
                }
                temp = temp.parent;
            }

            // �̵� ���� ���� �˻� (���� ���� ��)
            string warning;
            if (!FolderDepthUtility.CanMove(draggedFolder, targetFolder, out warning))
            {
                LogWindowManager.Instance.Log(warning);
                return;
            }

            // ���� �θ𿡼� ����
            if (draggedFolder.parent != null)
                draggedFolder.parent.children.Remove(draggedFolder);

            // �� �θ� �߰�
            targetFolder.children.Add(draggedFolder);
            draggedFolder.parent = targetFolder;

            // �巡�� ���� �� �α� ���
            FolderDragManager.Instance.EndDrag();
            LogWindowManager.Instance.Log($"���� '{draggedFolder.name}' �� '{targetFolder.name}' �̵���");
        }
        // ���� �̵� ó��
        else if (draggedFile != null)
        {
            // ���� �θ𿡼� ����
            if (draggedFile.parent != null)
                draggedFile.parent.files.Remove(draggedFile);

            // �� �θ� �߰�
            draggedFile.parent = targetFolder;
            targetFolder.files.Add(draggedFile);

            // �巡�� ���� �� �α� ���
            FolderDragManager.Instance.EndDrag();
            LogWindowManager.Instance.Log($"���� '{draggedFile.name}' �� '{targetFolder.name}' �̵���");
        }

        // �� ������ �� UI ���� (��� ȣ�� �� UI �浹 ����)
        fileWindow.StartCoroutine(OpenFolderNextFrame(targetFolder));
    }

    // �� ������ �� �ش� ���� ���� (UI ������ Ȯ����)
    private IEnumerator OpenFolderNextFrame(Folder folder)
    {
        yield return null; // �� ������ ���
        fileWindow.OpenFolder(folder, false);
    }
}
