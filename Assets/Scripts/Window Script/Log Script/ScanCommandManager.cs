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
        logWindow.DisableInput(); // 입력 비활성화


        int totalItems = CountAllFilesAndFolders(folder); // 폴더 + 파일 총합
        int progressBarLength = 10;

        float timePerBar = totalItems * 1f; // 1초 * totalItems → 한 칸 차는 시간
        int abnormalCount = CountAbnormal(folder);

        // 초기 빈 진행 바 출력 (새 로그)
        logWindow.Log($"이상 스캔중 {new string('ㅁ', progressBarLength)}");

        for (int i = 0; i < progressBarLength; i++)
        {
            yield return new WaitForSeconds(timePerBar); // 한 칸씩 기다림

            // 한 칸 차오른 진행바
            string progress = new string('■', i + 1) + new string('ㅁ', progressBarLength - i - 1);
            logWindow.ReplaceLastScanLog($"이상 스캔중 {progress}");
        }

        logWindow.ReplaceLastScanLog($"스캔 완료: 이상 {abnormalCount}개 발견됨.");
 

        logWindow.EnableInput(); // 입력 재활성화
        isScanning = false;
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
