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

    [Header("Path Panel")]
    public PathPanelManager pathPanelManager;

    [Header("Back Button")]
    public Button backButton;

    private FileIcon selectedIcon;
    private Folder rootFolder;
    private Folder currentFolder;
    private Stack<Folder> folderHistory = new Stack<Folder>();

    void Awake()
    {
        // PathPanelManager 먼저 초기화
        if (pathPanelManager != null)
            pathPanelManager.Initialize(this);
    }

    void Start()
    {
        // 폴더 구조
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

        // 이상 폴더 랜덤 지정 + 하위 폴더 상속
        PickAbnormalFolderRecursive(rootFolder);

        // Back 버튼 항상 활성화
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
        if (backButton != null)
            backButton.gameObject.SetActive(true);

        OpenFolder(rootFolder, false);
    }

    void PickAbnormalFolderRecursive(Folder folder)
    {
        if (folder.children.Count == 0)
        {
            folder.isAbnormal = Random.value < 0.1f; // 10% 확률
            return;
        }
        int index = Random.Range(0, folder.children.Count);
        folder.children[index].isAbnormal = true;
        foreach (var child in folder.children)
            PickAbnormalFolderRecursive(child);
    }

    public void OpenFolder(Folder folder, bool recordPrevious = true)
    {
        if (recordPrevious && currentFolder != null && folder != currentFolder)
            folderHistory.Push(currentFolder);

        currentFolder = folder;

        foreach (Transform child in contentArea)
            Destroy(child.gameObject);

        selectedIcon = null;
        emptyText.gameObject.SetActive(folder.children.Count == 0);

        foreach (Folder child in folder.children)
        {
            GameObject iconObj = Instantiate(fileIconPrefab, contentArea);
            FileIcon icon = iconObj.GetComponent<FileIcon>();
            icon.Setup(child, this, folder.isAbnormal);
        }

        if (backButton != null)
            backButton.gameObject.SetActive(true);

        if (pathPanelManager != null)
            pathPanelManager.UpdatePathButtons();
    }

    public void SetSelectedIcon(FileIcon icon)
    {
        if (selectedIcon != null)
            selectedIcon.SetSelected(false);

        selectedIcon = icon;
        selectedIcon.SetSelected(true);
    }

    public List<Folder> GetCurrentPathList()
    {
        List<Folder> pathList = new List<Folder>();
        Folder temp = currentFolder;
        while (temp != null)
        {
            pathList.Insert(0, temp);
            temp = temp.parent;
        }
        return pathList;
    }

    public void NavigateToPathIndex(int index)
    {
        var pathList = GetCurrentPathList();
        if (index < 0 || index >= pathList.Count) return;
        OpenFolder(pathList[index], false);
    }

    private void OnBackButtonClicked()
    {
        if (folderHistory.Count == 0) return;
        Folder previous = folderHistory.Pop();
        OpenFolder(previous, false);
    }
}
