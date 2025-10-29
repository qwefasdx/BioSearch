using System.Collections;
using UnityEngine;

/// <summary>
/// ScanCommandManager
/// - �α� â���� �Էµ� '��ĵ ���'�� �����ϰ� �����ϴ� �Ŵ���
/// - ���� ������ ��ȸ�ϸ� �̻� ����/������ Ž���ϰ� ������� ǥ����
/// </summary>
public class ScanCommandManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static ScanCommandManager Instance;

    // ���� ������ UI�� �����ϴ� FileWindow
    public FileWindow fileWindow;

    // �α� �� ��ɾ� �Է��� ó���ϴ� LogWindowManager
    public LogWindowManager logWindow;

    // ��ĵ ������ ���θ� ��Ÿ���� �÷���
    private bool isScanning = false;

    private void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
    }

    private void OnEnable()
    {
        // LogWindowManager�� Ȱ��ȭ�� ��, ��ĵ ��� �̺�Ʈ ����
        if (logWindow != null)
            logWindow.OnScanCommandEntered += HandleScanCommand;
    }

    private void OnDisable()
    {
        // ��Ȱ��ȭ �� �̺�Ʈ ���� ���� (�޸� ���� ����)
        if (logWindow != null)
            logWindow.OnScanCommandEntered -= HandleScanCommand;
    }

    /// <summary>
    /// �α�â���� '��ĵ' ����� �ԷµǾ��� �� ȣ��Ǵ� �޼���
    /// </summary>
    /// <param name="folderName">����ڰ� �Է��� ���� �̸�</param>
    private void HandleScanCommand(string folderName)
    {
        // �̹� ��ĵ ���̸� �ߺ� ���� ����
        if (isScanning)
        {
            logWindow.Log("��ĵ ���Դϴ�...");
            return;
        }

        // FileWindow���� ��Ʈ ���� ��������
        Folder root = fileWindow.GetRootFolder();

        // �̸����� ��� ���� Ž��
        Folder target = FindFolderByName(root, folderName);

        if (target == null)
        {
            logWindow.Log($"���� '{folderName}'��(��) ã�� �� �����ϴ�.");
            return;
        }

        // �ڷ�ƾ���� ��ĵ ����
        StartCoroutine(ScanFolderCoroutine(target));
    }

    /// <summary>
    /// ������ ������ �񵿱�� ��ĵ�ϸ� ���� ��Ȳ�� �α׷� ǥ��
    /// </summary>
    private IEnumerator ScanFolderCoroutine(Folder folder)
    {
        isScanning = true;
        logWindow.DisableInput(); // ����� �Է� ��Ȱ��ȭ

        // ��ĵ ��� �� ���� �� ���� ���� ���
        int totalItems = CountAllFilesAndFolders(folder);
        int progressBarLength = 10; // ������� ��ü ���� (10ĭ)

        // ���� �ӵ� (�� ������ ���� ���� ��� �ð� ����)
        float timePerBar = totalItems * 1f; // �� ������ �� �� 1��
        int abnormalCount = CountAbnormal(folder); // �̻� �׸� ����

        // �ʱ� �α� ��� (�� ���� ��)
        logWindow.Log($"�̻� ��ĵ�� {new string('��', progressBarLength)}");

        // ����ٸ� 1ĭ�� ä������ ǥ��
        for (int i = 0; i < progressBarLength; i++)
        {
            yield return new WaitForSeconds(timePerBar); // �� ĭ���� ���
            string progress = new string('��', i + 1) + new string('��', progressBarLength - i - 1);
            logWindow.ReplaceLastScanLog($"�̻� ��ĵ�� {progress}");
        }

        // ���� ��� ���
        logWindow.ReplaceLastScanLog($"��ĵ �Ϸ�: �̻� {abnormalCount}�� �߰ߵ�.");

        // �Է� �ٽ� Ȱ��ȭ
        logWindow.EnableInput();
        isScanning = false;
    }

    /// <summary>
    /// ���� �̸����� Folder ��ü ã�� (��� Ž��)
    /// </summary>
    private Folder FindFolderByName(Folder folder, string name)
    {
        // ���� ���� �̸��� ��ġ�ϸ� ��ȯ
        if (folder.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            return folder;

        // �ڽ� ���� ��� Ž��
        foreach (var child in folder.children)
        {
            var found = FindFolderByName(child, name);
            if (found != null) return found;
        }
        return null;
    }

    /// <summary>
    /// ���� �� ��� ���� ����/���� ���� ��������� ���
    /// </summary>
    private int CountAllFilesAndFolders(Folder folder)
    {
        int count = 1 + folder.files.Count; // ���� ���� 1�� + ���Ե� ���� ��
        foreach (var child in folder.children)
            count += CountAllFilesAndFolders(child); // ���� ���� ����
        return count;
    }

    /// <summary>
    /// ���� ������ �̻�(abnormal) ���� �� ���� ���� ���
    /// </summary>
    private int CountAbnormal(Folder folder)
    {
        int count = folder.isAbnormal ? 1 : 0; // ���� ������ �̻��� ���
        foreach (var child in folder.children)
            count += CountAbnormal(child); // �ڽ� ���� ��� Ž��
        foreach (var file in folder.files)
            if (file.isAbnormal) count++; // �̻� ���� ī��Ʈ
        return count;
    }
}

/// <summary>
/// FileWindow Ŭ������ ����� rootFolder �ʵ忡 �����ϱ� ���� Ȯ�� �޼���
/// - ���÷����� �̿��� ����� �ʵ� "rootFolder" ���� ������
/// </summary>
public static class FileWindowExtensions
{
    public static Folder GetRootFolder(this FileWindow window)
    {
        var field = typeof(FileWindow).GetField("rootFolder",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field.GetValue(window) as Folder;
    }
}

public static class AbnormalDetector
{
    public static int GetAbnormalCount(Folder folder)
    {
        if (folder == null) return 0;

        int count = folder.isAbnormal ? 1 : 0;

        foreach (var child in folder.children)
            count += GetAbnormalCount(child);

        foreach (var file in folder.files)
            if (file.isAbnormal) count++;

        return count;
    }
}

