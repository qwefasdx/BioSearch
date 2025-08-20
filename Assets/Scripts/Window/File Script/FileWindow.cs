using UnityEngine;
using System.Collections.Generic;

public class FileWindow : MonoBehaviour
{
    public Transform contentArea;       // ScrollView 안 Content
    public GameObject fileIconPrefab;   // 아이콘 프리팹

    [HideInInspector] public FileIcon selectedIcon = null; // 현재 선택된 아이콘

    private List<FileIcon> fileIcons = new List<FileIcon>();

    void Start()
    {
        // 임시 테스트 파일들 생성
        CreateFile("head");
        CreateFile("body");
        CreateFile("leg");
        CreateFile("arm");
        CreateFile("hand");
        CreateFile("organ");
    }

    void CreateFile(string fileName)
    {
        GameObject obj = Instantiate(fileIconPrefab, contentArea);
        FileIcon icon = obj.GetComponent<FileIcon>();
        icon.Setup(fileName, this); // FileWindow 참조 전달
        fileIcons.Add(icon);
    }
}
