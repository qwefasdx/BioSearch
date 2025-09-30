using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FileWindow : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject folderIconPrefab;
    public GameObject fileIconPrefab;

    [Header("Scroll Area")]
    public Transform contentArea;
    public TMP_Text emptyText;

    [Header("Path Panel")]
    public PathPanelManager pathPanelManager;

    [Header("Back Button")]
    public Button backButton;

    [Header("Inspector File List")]
    public List<FileData> fileDatas = new List<FileData>();

    private FolderIcon selectedFolderIcon;
    private FileIcon selectedFileIcon;

    private Folder rootFolder;
    private Folder currentFolder;
    private Stack<Folder> folderHistory = new Stack<Folder>();

    private List<File> currentFolderFiles = new List<File>();

    void Awake()
    {
        if (pathPanelManager != null)
            pathPanelManager.Initialize(this);
    }

    void Start()
    {
        // 기본 폴더 구조 생성
        rootFolder = new Folder("Root");
        CreateDefaultFolders();

        // Inspector에 입력한 FileData 기반으로 파일 생성
        foreach (var data in fileDatas)
        {
            Folder targetParent = FindFolderByName(rootFolder, data.parentFolderName);
            if (targetParent == null)
            {
                Debug.LogWarning($"부모 폴더 '{data.parentFolderName}'을(를) 찾을 수 없습니다. Root에 추가합니다.");
                targetParent = rootFolder;
            }

            File file = new File(
                data.fileName,
                data.extension,
                targetParent,
                data.textContent,
                data.imageContent
            );

            currentFolderFiles.Add(file);
            targetParent.files.Add(file);
        }

        // 이상 폴더 확률 설정
        AssignAbnormalParameters(rootFolder);

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
            backButton.gameObject.SetActive(true);
        }

        OpenFolder(rootFolder, false);
    }

    void CreateDefaultFolders()
    {
        Folder Head = new Folder("Head", rootFolder);
        Head.children.Add(new Folder("Mouse", Head));
        Head.children.Add(new Folder("LeftEye", Head));
        Head.children.Add(new Folder("RightEye", Head));
        Head.children.Add(new Folder("Nose", Head));

        Folder Body = new Folder("Body", rootFolder);
        Folder Organ = new Folder("Organ", rootFolder);
        Organ.children.Add(new Folder("Heart", Organ));

        Folder LeftArm = new Folder("LeftArm", rootFolder);
        LeftArm.children.Add(new Folder("LeftHand", LeftArm));
        Folder RightArm = new Folder("RightArm", rootFolder);
        RightArm.children.Add(new Folder("RightHand", RightArm));

        Folder LeftLeg = new Folder("LeftLeg", rootFolder);
        LeftLeg.children.Add(new Folder("LeftFoot", LeftLeg));
        Folder RightLeg = new Folder("RightLeg", rootFolder);
        RightLeg.children.Add(new Folder("RightFoot", RightLeg));

        rootFolder.children.Add(Head);
        rootFolder.children.Add(Body);
        rootFolder.children.Add(Organ);
        rootFolder.children.Add(LeftArm);
        rootFolder.children.Add(RightArm);
        rootFolder.children.Add(LeftLeg);
        rootFolder.children.Add(RightLeg);
    }

    void AssignAbnormalParameters(Folder folder)
    {
        foreach (var child in folder.children)
        {
            child.abnormalParameter = 0.1f; // 10% 확률
            child.AssignAbnormalByParameter();
            AssignAbnormalParameters(child);
        }
    }

    public void OpenFolder(Folder folder, bool recordPrevious = true)
    {
        if (recordPrevious && currentFolder != null && folder != currentFolder)
            folderHistory.Push(currentFolder);

        currentFolder = folder;

        foreach (Transform child in contentArea)
            Destroy(child.gameObject);

        selectedFolderIcon = null;
        selectedFileIcon = null;

        bool hasContent = (folder.children.Count > 0) || HasFilesInFolder(folder);
        emptyText.gameObject.SetActive(!hasContent);

        // 폴더 아이콘 생성
        foreach (Folder child in folder.children)
        {
            GameObject iconObj = Instantiate(folderIconPrefab, contentArea);
            FolderIcon icon = iconObj.GetComponent<FolderIcon>();
            icon.Setup(child, this, child.isAbnormal);
        }

        // 파일 아이콘 생성
        foreach (File file in currentFolderFiles)
        {
            if (file.parent != folder) continue;
            GameObject iconObj = Instantiate(fileIconPrefab, contentArea);
            FileIcon icon = iconObj.GetComponent<FileIcon>();
            icon.Setup(file, this);
        }

        if (backButton != null)
            backButton.gameObject.SetActive(true);

        if (pathPanelManager != null)
            pathPanelManager.UpdatePathButtons();
    }

    private bool HasFilesInFolder(Folder folder)
    {
        foreach (var f in currentFolderFiles)
        {
            if (f.parent == folder) return true;
        }
        return false;
    }

    public void SetSelectedIcon(FolderIcon icon)
    {
        if (selectedFolderIcon != null)
            selectedFolderIcon.SetSelected(false);

        selectedFolderIcon = icon;
        selectedFolderIcon.SetSelected(true);
    }

    public void SetSelectedFileIcon(FileIcon icon)
    {
        if (selectedFileIcon != null)
            selectedFileIcon.SetSelected(false);

        selectedFileIcon = icon;
        selectedFileIcon.SetSelected(true);
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

    public void RefreshFolder(Folder folder)
    {
        OpenFolder(folder, false);
    }

    public void RefreshWindow()
    {
        if (currentFolder != null)
            OpenFolder(currentFolder, false);
    }
    private Folder FindFolderByName(Folder folder, string name)
    {
        if (folder.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            return folder;

        foreach (var child in folder.children)
        {
            var found = FindFolderByName(child, name);
            if (found != null)
                return found;
        }

        return null;
    }

}
