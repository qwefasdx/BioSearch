using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FileWindow : MonoBehaviour
{
    [Header("Scroll Area")]
    public GameObject fileIconPrefab;
    public Transform contentArea;
    public TMP_Text emptyText;

    [Header("Top Bar")]
    public Button backButton;
    public TMP_Text pathText;

    private FileIcon selectedIcon;
    private Folder rootFolder;
    private Folder currentFolder;

    private Stack<Folder> folderHistory = new Stack<Folder>();
    private Folder abnormalFolder;

    void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
    }

    void Start()
    {
        // 폴더 구조 생성
        rootFolder = new Folder("Root");

        Folder head = new Folder("head", rootFolder);
        head.children.Add(new Folder("mouse", head));
        head.children.Add(new Folder("left eye", head));
        head.children.Add(new Folder("right eye", head));

        Folder organ = new Folder("organ", rootFolder);
        organ.children.Add(new Folder("heart", organ));

        rootFolder.children.Add(head);
        rootFolder.children.Add(new Folder("body", rootFolder));
        rootFolder.children.Add(new Folder("leg", rootFolder));
        rootFolder.children.Add(new Folder("arm", rootFolder));
        rootFolder.children.Add(new Folder("hand", rootFolder));
        rootFolder.children.Add(organ);

        // 랜덤 이상 폴더 지정 (하위 폴더 포함)
        abnormalFolder = PickRandomAbnormal(rootFolder);

        // 루트 폴더 열기
        OpenFolder(rootFolder, false);
    }

    private Folder PickRandomAbnormal(Folder root)
    {
        List<Folder> allFolders = new List<Folder>();
        CollectFoldersRecursive(root, allFolders);
        allFolders.Remove(root); // 루트 제외

        if (allFolders.Count > 0)
        {
            int idx = Random.Range(0, allFolders.Count);
            Folder abnormal = allFolders[idx];
            MarkAbnormalRecursive(abnormal);
            return abnormal;
        }
        return null;
    }

    private void CollectFoldersRecursive(Folder folder, List<Folder> list)
    {
        list.Add(folder);
        foreach (var child in folder.children)
            CollectFoldersRecursive(child, list);
    }

    private void MarkAbnormalRecursive(Folder folder)
    {
        folder.isAbnormal = true;
        foreach (var child in folder.children)
            MarkAbnormalRecursive(child);
    }

    public void OpenFolder(Folder folder, bool recordPrevious = true)
    {
        if (recordPrevious && currentFolder != null && folder != currentFolder)
            folderHistory.Push(currentFolder);

        currentFolder = folder;

        foreach (Transform child in contentArea)
            Destroy(child.gameObject);

        selectedIcon = null;

        if (backButton != null)
            backButton.gameObject.SetActive(true);

        if (pathText != null)
            pathText.text = GetFullPath(currentFolder);

        emptyText.gameObject.SetActive(folder.children.Count == 0);

        // 아이콘 생성
        foreach (Folder child in folder.children)
        {
            GameObject iconObj = Instantiate(fileIconPrefab, contentArea);
            FileIcon icon = iconObj.GetComponent<FileIcon>();
            icon.Setup(child, this);
        }
    }

    public void SetSelectedIcon(FileIcon icon)
    {
        if (selectedIcon != null)
            selectedIcon.SetSelected(false);

        selectedIcon = icon;
        selectedIcon.SetSelected(true);
    }

    public void OpenSelected()
    {
        if (selectedIcon == null) return;
        OpenFolder(selectedIcon.GetFolder());
    }

    private void OnBackButtonClicked()
    {
        if (currentFolder == rootFolder) return;

        if (folderHistory.Count > 0)
        {
            if (selectedIcon != null)
                selectedIcon.SetSelected(false);

            Folder previous = folderHistory.Pop();
            OpenFolder(previous, false);
        }
    }

    private string GetFullPath(Folder folder)
    {
        List<string> pathList = new List<string>();
        Folder temp = folder;
        while (temp != null)
        {
            pathList.Insert(0, temp.name);
            temp = temp.parent;
        }
        return string.Join(" / ", pathList);
    }
}
