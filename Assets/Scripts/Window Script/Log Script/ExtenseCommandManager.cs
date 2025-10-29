using UnityEngine;

/// <summary>
/// extense ��ɾ ó���ϴ� Ŭ����
/// - Ư�� ������ Ȯ���ڸ� ����
/// </summary>
public class ExtenseCommandManager : MonoBehaviour
{
    [Header("References")]
    public FileWindow fileWindow;          // Ȯ���� ���� ����� �Ǵ� ���� ����
    public LogWindowManager logWindow;     // �α� ��� â

    private void OnEnable()
    {
        if (logWindow != null)
            logWindow.OnExtenseCommandEntered += HandleExtenseCommand;
    }

    private void OnDisable()
    {
        if (logWindow != null)
            logWindow.OnExtenseCommandEntered -= HandleExtenseCommand;
    }

    /// <summary>
    /// extense ��ɾ� �Է� �� ȣ��Ǵ� ó�� �Լ�
    /// </summary>
    private void HandleExtenseCommand(string commandArgs)
    {
        // ���� �Ľ�: "���ϸ� Ȯ����"
        string[] parts = commandArgs.Split(' ');
        if (parts.Length < 2)
        {
            logWindow.Log("����: extense [���ϸ�] [�� Ȯ����]");
            return;
        }

        string fileName = parts[0];
        string newExtension = parts[1];

        ChangeFileExtension(fileName, newExtension);
    }

    /// <summary>
    /// ������ Ȯ���ڸ� ����
    /// </summary>
    private void ChangeFileExtension(string fileName, string newExtension)
    {
        if (fileWindow == null)
        {
            logWindow.Log("FileWindow ������ �����ϴ�.");
            return;
        }

        Folder root = fileWindow.GetRootFolder();
        File target = FindFileByName(root, fileName);

        if (target == null)
        {
            logWindow.Log($"���� '{fileName}'��(��) ã�� �� �����ϴ�.");
            return;
        }

        string oldExt = target.extension;
        target.extension = newExtension;

        // ���� �̸��� ������ �˾��� �����ִٸ� ��� �ݱ�
        if (FilePopupManager.Instance != null)
            FilePopupManager.Instance.ClosePopup(fileName);

        logWindow.Log($"'{target.name}' ���� Ȯ���� ����: {oldExt} �� {newExtension}");

        fileWindow.RefreshWindow(); // UI ����
    }

    /// <summary>
    /// ���� �̸����� ��� �˻�
    /// </summary>
    private File FindFileByName(Folder folder, string name)
    {
        // ���� ���� �� ���� �˻�
        foreach (var file in folder.files)
        {
            if (file.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return file;
        }

        // ���� ���� ��� Ž��
        foreach (var child in folder.children)
        {
            var found = FindFileByName(child, name);
            if (found != null) return found;
        }

        return null;
    }
}
