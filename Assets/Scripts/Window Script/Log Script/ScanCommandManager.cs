using System.Collections;
using UnityEngine;

/// <summary>
/// ScanCommandManager
/// - 로그 창에서 입력된 '스캔 명령'을 감지하고 실행하는 매니저
/// - 폴더 구조를 순회하며 이상 파일/폴더를 탐색하고 진행률을 표시함
/// </summary>
public class ScanCommandManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ScanCommandManager Instance;

    // 폴더 구조와 UI를 관리하는 FileWindow
    public FileWindow fileWindow;

    // 로그 및 명령어 입력을 처리하는 LogWindowManager
    public LogWindowManager logWindow;

    // 스캔 중인지 여부를 나타내는 플래그
    private bool isScanning = false;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // 중복 인스턴스 제거
    }

    private void OnEnable()
    {
        // LogWindowManager가 활성화될 때, 스캔 명령 이벤트 구독
        if (logWindow != null)
            logWindow.OnScanCommandEntered += HandleScanCommand;
    }

    private void OnDisable()
    {
        // 비활성화 시 이벤트 구독 해제 (메모리 누수 방지)
        if (logWindow != null)
            logWindow.OnScanCommandEntered -= HandleScanCommand;
    }

    /// <summary>
    /// 로그창에서 '스캔' 명령이 입력되었을 때 호출되는 메서드
    /// </summary>
    /// <param name="folderName">사용자가 입력한 폴더 이름</param>
    private void HandleScanCommand(string folderName)
    {
        // 이미 스캔 중이면 중복 실행 방지
        if (isScanning)
        {
            logWindow.Log("스캔 중입니다...");
            return;
        }

        // FileWindow에서 루트 폴더 가져오기
        Folder root = fileWindow.GetRootFolder();

        // 이름으로 대상 폴더 탐색
        Folder target = FindFolderByName(root, folderName);

        if (target == null)
        {
            logWindow.Log($"폴더 '{folderName}'을(를) 찾을 수 없습니다.");
            return;
        }

        // 코루틴으로 스캔 시작
        StartCoroutine(ScanFolderCoroutine(target));
    }

    /// <summary>
    /// 지정한 폴더를 비동기로 스캔하며 진행 상황을 로그로 표시
    /// </summary>
    private IEnumerator ScanFolderCoroutine(Folder folder)
    {
        isScanning = true;
        logWindow.DisableInput(); // 사용자 입력 비활성화

        // 스캔 대상 내 폴더 및 파일 개수 계산
        int totalItems = CountAllFilesAndFolders(folder);
        int progressBarLength = 10; // 진행바의 전체 길이 (10칸)

        // 진행 속도 (총 아이템 수에 따라 대기 시간 증가)
        float timePerBar = totalItems * 1f; // 총 아이템 수 × 1초
        int abnormalCount = CountAbnormal(folder); // 이상 항목 개수

        // 초기 로그 출력 (빈 진행 바)
        logWindow.Log($"이상 스캔중 {new string('ㅁ', progressBarLength)}");

        // 진행바를 1칸씩 채워가며 표시
        for (int i = 0; i < progressBarLength; i++)
        {
            yield return new WaitForSeconds(timePerBar); // 한 칸마다 대기
            string progress = new string('■', i + 1) + new string('ㅁ', progressBarLength - i - 1);
            logWindow.ReplaceLastScanLog($"이상 스캔중 {progress}");
        }

        // 최종 결과 출력
        logWindow.ReplaceLastScanLog($"스캔 완료: 이상 {abnormalCount}개 발견됨.");

        // 입력 다시 활성화
        logWindow.EnableInput();
        isScanning = false;
    }

    /// <summary>
    /// 폴더 이름으로 Folder 객체 찾기 (재귀 탐색)
    /// </summary>
    private Folder FindFolderByName(Folder folder, string name)
    {
        // 현재 폴더 이름이 일치하면 반환
        if (folder.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            return folder;

        // 자식 폴더 재귀 탐색
        foreach (var child in folder.children)
        {
            var found = FindFolderByName(child, name);
            if (found != null) return found;
        }
        return null;
    }

    /// <summary>
    /// 폴더 및 모든 하위 파일/폴더 수를 재귀적으로 계산
    /// </summary>
    private int CountAllFilesAndFolders(Folder folder)
    {
        int count = 1 + folder.files.Count; // 현재 폴더 1개 + 포함된 파일 수
        foreach (var child in folder.children)
            count += CountAllFilesAndFolders(child); // 하위 폴더 포함
        return count;
    }

    /// <summary>
    /// 폴더 내부의 이상(abnormal) 파일 및 폴더 개수 계산
    /// </summary>
    private int CountAbnormal(Folder folder)
    {
        int count = folder.isAbnormal ? 1 : 0; // 현재 폴더가 이상일 경우
        foreach (var child in folder.children)
            count += CountAbnormal(child); // 자식 폴더 재귀 탐색
        foreach (var file in folder.files)
            if (file.isAbnormal) count++; // 이상 파일 카운트
        return count;
    }
}

/// <summary>
/// FileWindow 클래스의 비공개 rootFolder 필드에 접근하기 위한 확장 메서드
/// - 리플렉션을 이용해 비공개 필드 "rootFolder" 값을 가져옴
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

