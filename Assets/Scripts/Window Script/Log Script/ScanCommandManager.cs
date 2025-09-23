using UnityEngine;

/// <summary>
/// scan 명령어를 처리하는 클래스
/// - 특정 폴더 탐색
/// - 이상 폴더 개수 확인
/// </summary>
public class ScanCommandManager : MonoBehaviour
{
    [Header("References")]
    public FileWindow fileWindow;          // 스캔할 폴더 구조
    public LogWindowManager logWindow;     // 로그 출력 창

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

    /// <summary>
    /// scan 명령어 입력 시 호출되는 처리 함수
    /// </summary>
    private void HandleScanCommand(string fileName)
    {
        ScanFile(fileName);
    }

    /// <summary>
    /// 특정 폴더를 검색 후 이상 여부 검사
    /// </summary>
    private void ScanFile(string fileName)
    {
        if (fileWindow == null)
        {
            logWindow.Log("FileWindow 참조가 없습니다.");
            return;
        }

        Folder root = fileWindow.GetRootFolder();
        Folder target = FindFolderByName(root, fileName);

        if (target == null)
        {
            logWindow.Log($"폴더 '{fileName}'을(를) 찾을 수 없습니다.");
            return;
        }

        int abnormalCount = CountAbnormalFoldersRecursive(target);

        if (abnormalCount > 0)
            logWindow.Log($"{abnormalCount}개의 이상 감지됨.");
        else
            logWindow.Log("이상 없음.");
    }

    /// <summary>
    /// 폴더 이름으로 재귀 검색
    /// </summary>
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

    /// <summary>
    /// 이상 폴더 개수를 재귀적으로 계산
    /// </summary>
    private int CountAbnormalFoldersRecursive(Folder folder)
    {
        int count = folder.isAbnormal ? 1 : 0;
        foreach (var child in folder.children)
            count += CountAbnormalFoldersRecursive(child);
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
