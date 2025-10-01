using System.Collections;
using UnityEngine;

public class ScanCommandManager : MonoBehaviour
{
    public static ScanCommandManager Instance;

    public FileWindow fileWindow;
    public LogWindowManager logWindow;

    private bool isScanning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (logWindow != null)
            logWindow.OnScanCommandEntered += HandleScanCommand;
    }

    private void OnDisable()
    {
        if (logWindow != null)
            logWindow.OnScanCommandEntered -= HandleScanCommand;
    }

    private void HandleScanCommand(string folderName)
    {
        if (isScanning)
        {
            logWindow.Log("스캔 중입니다...");
            return;
        }

        Folder root = fileWindow.GetRootFolder();
        Folder target = FindFolderByName(root, folderName);

        if (target == null)
        {
            logWindow.Log($"폴더 '{folderName}'을(를) 찾을 수 없습니다.");
            return;
        }

        StartCoroutine(ScanFolderCoroutine(target));
    }

    private IEnumerator ScanFolderCoroutine(Folder folder)
    {
        isScanning = true;
        logWindow.DisableInput();

        int totalItems = CountAllFilesAndFolders(folder);
        int totalFiles = CountAllFilesOnly(folder); // 하위 파일만 계산
        int progressBarLength = 10;

        float itemsPerBar = totalItems / (float)progressBarLength;
        float timePerBar = itemsPerBar * totalFiles * 2f; // 2초 * 하위 파일 개수

        logWindow.Log($"이상 스캔중 {new string('ㅁ', progressBarLength)}");

        for (int i = 1; i <= progressBarLength; i++)
        {
            yield return new WaitForSeconds(timePerBar);
            string progress = new string('■', i) + new string('ㅁ', progressBarLength - i);
            logWindow.ReplaceLastLog($"이상 스캔중 {progress}");
        }

        int abnormalCount = CountAbnormal(folder);
        logWindow.ReplaceLastLog($"스캔 완료: 이상 {abnormalCount}개 발견됨.");

        logWindow.EnableInput();
        isScanning = false;
    }

    private int CountAllFilesOnly(Folder folder)
    {
        int count = folder.files.Count;
        foreach (var child in folder.children)
            count += CountAllFilesOnly(child);
        return count;
    }



    private Folder FindFolderByName(Folder folder, string name)
    {
        if (folder.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            return folder;

        foreach (var child in folder.children)
        {
            var found = FindFolderByName(child, name);
            if (found != null) return found;
        }
        return null;
    }

    private int CountAllFilesAndFolders(Folder folder)
    {
        int count = 1 + folder.files.Count;
        foreach (var child in folder.children)
            count += CountAllFilesAndFolders(child);
        return count;
    }

    private int CountAbnormal(Folder folder)
    {
        int count = folder.isAbnormal ? 1 : 0;
        foreach (var child in folder.children)
            count += CountAbnormal(child);
        foreach (var file in folder.files)
            if (file.isAbnormal) count++;
        return count;
    }
}


/// <summary>
/// FileWindow 클래스의 루트 폴더 접근 확장 메서드
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
