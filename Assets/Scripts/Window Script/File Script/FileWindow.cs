using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// ���� �� ���� UI�� �����ϴ� Ŭ����.
/// ���� ���� ����, ���� ����, ��� �̵�, ���� ���� ���� ���� ���.
/// </summary>
public partial class FileWindow : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject folderIconPrefab;    // ���� ������ ������
    public GameObject fileIconPrefab;      // ���� ������ ������

    [Header("Scroll Area")]
    public Transform contentArea;          // ��ũ�� ������ ����
    public TMP_Text emptyText;             // �������� ���� �� ǥ�õǴ� �ؽ�Ʈ

    [Header("Path Panel")]
    public PathPanelManager pathPanelManager;  // ��� �г� �Ŵ���

    [Header("Back Button")]
    public Button backButton;              // �ڷΰ��� ��ư

    [Header("Inspector File List")]
    public List<FileData> fileDatas = new List<FileData>(); // �ν����Ϳ��� �ԷµǴ� ���� ������

    [Header("Body Buttons")]
    public Button headButton;
    public Button bodyButton;
    public Button leftArmButton;
    public Button leftHandButton;
    public Button rightArmButton;
    public Button rightHandButton;
    public Button leftLegButton;
    public Button leftFootButton;
    public Button rightLegButton;
    public Button rightFootButton;

    [Header("Special Prefabs")]
    public GameObject upButtonPrefab;  // ���� ������ �̵��ϴ� "..." ������ ������


    // ���� ���õ� ������
    private FolderIcon selectedFolderIcon;
    private FileIcon selectedFileIcon;

    // ���� ���� ����
    private Folder rootFolder;                 // ��Ʈ ����
    private Folder currentFolder;              // ���� ���� �ִ� ����
    private Stack<Folder> folderHistory = new Stack<Folder>(); // ���� ���� ��� (�ڷΰ����)

    // ���� ������ ���� ���
    private List<File> currentFolderFiles = new List<File>();

    void Awake()
    {
        // ��� �г� �ʱ�ȭ
        if (pathPanelManager != null)
            pathPanelManager.Initialize(this);
    }

    void Start()
    {
        // �⺻ ���� ���� ����
        rootFolder = new Folder("Root");
        CreateDefaultFolders();

        // Inspector�� FileData ������� ���� ����
        InitializeFilesFromInspector();

        // �̻� ���� Ȯ�� ����
        AssignAbnormalParameters(rootFolder);

        // �ڷΰ��� ��ư �ʱ�ȭ
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
            backButton.gameObject.SetActive(true);
        }

        // ��Ʈ ���� ����
        OpenFolder(rootFolder, false);
    }

    /// <summary>
    /// Inspector���� �Էµ� ���� �����͸� �������� ���� ��ü ����
    /// </summary>
    private void InitializeFilesFromInspector()
    {
        foreach (var data in fileDatas)
        {
            Folder targetParent = FindFolderByName(rootFolder, data.parentFolderName);

            if (targetParent == null)
            {
                Debug.LogWarning($"�θ� ���� '{data.parentFolderName}'��(��) ã�� �� �����ϴ�. Root�� �߰��մϴ�.");
                targetParent = rootFolder;
            }

            File file = new File(
                data.fileName,
                data.extension,
                targetParent,
                data.textContent,
                data.imageContent,
                data.isAbnormal
            );

            currentFolderFiles.Add(file);
            targetParent.files.Add(file);
        }
    }

    /// <summary>
    /// �⺻ ���� ������ �����ϰ� UI ��ư�� ����
    /// </summary>
    void CreateDefaultFolders()
    {
        // ���� ����
        Folder Head = new Folder("Head", rootFolder);
        Head.children.Add(new Folder("Mouse", Head));
        Head.children.Add(new Folder("LeftEye", Head));
        Head.children.Add(new Folder("RightEye", Head));
        Head.children.Add(new Folder("Nose", Head));

        Folder Body = new Folder("Body", rootFolder);

        Folder Organ = new Folder("Organ", rootFolder);
        Organ.children.Add(new Folder("Heart", Organ));

        Folder LeftArm = new Folder("LeftArm", rootFolder);
        Folder LeftHand = new Folder("LeftHand", rootFolder);
        Folder RightArm = new Folder("RightArm", rootFolder);
        Folder RightHand = new Folder("RightHand", rootFolder);

        Folder LeftLeg = new Folder("LeftLeg", rootFolder);
        Folder LeftFoot = new Folder("LeftFoot", rootFolder);
        Folder RightLeg = new Folder("RightLeg", rootFolder);
        Folder RightFoot = new Folder("RightFoot", rootFolder);

        // ��Ʈ�� �߰�
        rootFolder.children.AddRange(new List<Folder>
        {
            Head, Body, Organ, LeftArm, LeftHand, RightArm, RightHand,
            LeftLeg, LeftFoot, RightLeg, RightFoot
        });

        // UI ��ư ����
        if (headButton != null) Head.linkedBodyButton = headButton;
        if (bodyButton != null) Body.linkedBodyButton = bodyButton;
        if (leftArmButton != null) LeftArm.linkedBodyButton = leftArmButton;
        if (leftHandButton != null) LeftHand.linkedBodyButton = leftHandButton;
        if (rightArmButton != null) RightArm.linkedBodyButton = rightArmButton;
        if (rightHandButton != null) RightHand.linkedBodyButton = rightHandButton;
        if (leftLegButton != null) LeftLeg.linkedBodyButton = leftLegButton;
        if (leftFootButton != null) LeftFoot.linkedBodyButton = leftFootButton;
        if (rightLegButton != null) RightLeg.linkedBodyButton = rightLegButton;
        if (rightFootButton != null) RightFoot.linkedBodyButton = rightFootButton;
    }

    /// <summary>
    /// �ڽ� ������ �̻� ���θ� Ȯ�������� ����
    /// </summary>
    void AssignAbnormalParameters(Folder folder)
    {
        foreach (var child in folder.children)
        {
            child.abnormalParameter = 0.1f; // 10% Ȯ��
            child.AssignAbnormalByParameter();
            AssignAbnormalParameters(child);
        }
    }
    public Folder GetRootFolder()
    {
        return rootFolder;
    }

}

