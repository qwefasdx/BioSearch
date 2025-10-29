using UnityEngine;
using TMPro;

// FileIcon�� �ؽ�Ʈ ������ �̻� ���� ���ο� ���� �ڵ� ����
public class FileIconColorUpdater : MonoBehaviour
{
    private FolderIcon fileIcon;
    private TMP_Text text;
    private GlobalColorManager gcm;

    void Awake()
    {
        fileIcon = GetComponent<FolderIcon>();
        text = GetComponent<TMP_Text>();
        gcm = FindObjectOfType<GlobalColorManager>();
    }

    void Start()
    {
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (fileIcon == null || text == null || gcm == null) return;

        Folder folder = fileIcon.GetFolder();
        if (folder != null && folder.isAbnormal)
            text.color = gcm.abnormalFolderTextColor;
    }
}
