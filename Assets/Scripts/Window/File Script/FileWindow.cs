using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FileWindow : MonoBehaviour
{
    [Header("Scroll Area")]
    public GameObject fileIconPrefab;   // 아이콘 프리팹
    public Transform contentArea;       // ScrollView Content
    public TMP_Text emptyText;          // Empty Folder 표시용 텍스트

    [Header("Top Bar")]
    public Button backButton;           // TopBar Back 버튼
    public TMP_Text pathText;           // 현재 경로 표시

    private FileIcon selectedIcon;
    private Folder rootFolder;
    private Folder currentFolder;

    private Stack<Folder> folderHistory = new Stack<Folder>();

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

        // 루트 폴더 열기
        OpenFolder(rootFolder, false);
    }

    // 폴더 열기
    public void OpenFolder(Folder folder, bool recordPrevious = true)
    {
        if (recordPrevious && currentFolder != null && folder != currentFolder)
            folderHistory.Push(currentFolder);

        currentFolder = folder;

        // ScrollView Content 초기화
        foreach (Transform child in contentArea)
            Destroy(child.gameObject);

        selectedIcon = null;

        // Back 버튼 항상 활성화
        if (backButton != null)
            backButton.gameObject.SetActive(true);

        // 현재 경로 표시
        if (pathText != null)
            pathText.text = GetFullPath(currentFolder);

        // 비어있는 폴더 처리
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

        Folder folder = selectedIcon.GetFolder();
        OpenFolder(folder);
    }

    private void OnBackButtonClicked()
    {
        // 루트 폴더에서는 이동하지 않음
        if (currentFolder == rootFolder)
            return;

        if (folderHistory.Count > 0)
        {
            if (selectedIcon != null)
                selectedIcon.SetSelected(false);

            Folder previous = folderHistory.Pop();
            OpenFolder(previous, false);
        }
    }

    // 전체 경로 문자열 반환
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

// Folder 클래스
[System.Serializable]
public class Folder
{
    public string name;
    public List<Folder> children = new List<Folder>();
    public Folder parent;

    public Folder(string name, Folder parent = null)
    {
        this.name = name;
        this.parent = parent;
    }
}
