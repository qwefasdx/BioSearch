using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 로그 출력 및 명령어 입력을 관리하는 클래스
/// - 입력 필드에서 명령어 처리
/// - 메시지 큐 기반 출력 (한 글자씩 타이핑 효과)
/// - 스크롤 자동 관리
/// </summary>
public class LogWindowManager : MonoBehaviour
{
    public static LogWindowManager Instance;

    [Header("UI References")]
    public TMP_Text logText;                  // 로그가 표시될 텍스트
    public TMP_InputField inputField;         // 명령어 입력 필드
    public ScrollRect scrollRect;             // 스크롤 제어용

    [Header("Settings")]
    public int maxLines = 50;                 // 최대 저장 로그 라인 수
    public float charDelay = 0.01f;           // 글자 출력 간격 (타이핑 효과)

    private string[] lines;                   // 로그 라인 저장 버퍼
    private int currentLine = 0;              // 현재 라인 인덱스
    private StringBuilder sb;                 // 로그 문자열 조합기

    private bool autoScroll = false;          // 새 로그 출력 시 자동 스크롤 여부
    private bool userScrolling = false;       // 사용자가 스크롤을 움직였는지 여부

    // scan 명령어 이벤트
    public delegate void ScanCommandHandler(string fileName);
    public event ScanCommandHandler OnScanCommandEntered;

    // 메시지 큐 (타이핑 효과를 위해 사용)
    private readonly Queue<string> messageQueue = new Queue<string>();
    private bool isTyping = false;

    private void Awake()
    {
        // 싱글톤 인스턴스 보장
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lines = new string[maxLines];
        sb = new StringBuilder();

        // 초기 세팅
        logText.text = "";
        inputField.onSubmit.AddListener(OnInputSubmitted);
        inputField.ActivateInputField();

        if (scrollRect == null)
            scrollRect = GetComponentInChildren<ScrollRect>();

        // 스크롤 세팅
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.inertia = true;
        scrollRect.content = logText.rectTransform;
        scrollRect.onValueChanged.AddListener(_ => userScrolling = true);

        // 텍스트 위치 설정 (아래쪽 고정)
        logText.rectTransform.pivot = new Vector2(0, 0);
        logText.rectTransform.anchorMin = new Vector2(0, 0);
        logText.rectTransform.anchorMax = new Vector2(1, 0);
    }

    private void LateUpdate()
    {
        // 텍스트 높이를 콘텐츠 크기에 맞춤
        float contentHeight = logText.preferredHeight;
        Vector2 size = logText.rectTransform.sizeDelta;
        logText.rectTransform.sizeDelta = new Vector2(size.x, contentHeight);

        // 자동 스크롤
        if (autoScroll && !userScrolling)
        {
            scrollRect.verticalNormalizedPosition = 0f;
            autoScroll = false;
        }

        userScrolling = false;
    }

    /// <summary>
    /// 외부에서 로그를 추가할 때 호출
    /// </summary>
    public void Log(string message)
    {
        messageQueue.Enqueue(message);
        if (!isTyping)
            StartCoroutine(ProcessQueue());
    }

    /// <summary>
    /// 메시지 큐를 순차적으로 출력 (타이핑 효과 포함)
    /// </summary>
    private IEnumerator ProcessQueue()
    {
        isTyping = true;

        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            lines[currentLine % maxLines] = "> " + message;
            currentLine++;

            sb.Clear();
            int start = Mathf.Max(0, currentLine - maxLines);

            // 이전 로그 출력
            for (int i = start; i < currentLine - 1; i++)
                sb.AppendLine(lines[i % maxLines]);

            logText.text = sb.ToString();

            // 새 메시지를 한 글자씩 출력
            string newLine = lines[(currentLine - 1) % maxLines];
            for (int i = 0; i < newLine.Length; i++)
            {
                logText.text = sb.ToString() + newLine.Substring(0, i + 1);
                yield return new WaitForSeconds(charDelay);
            }

            autoScroll = true;
        }

        isTyping = false;
    }

    /// <summary>
    /// 명령어 입력 처리
    /// </summary>
    private void OnInputSubmitted(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        Log("명령어 입력: " + command);

        command = command.Trim().ToLower();

        // 명령어 처리
        if (command.StartsWith("scan "))
        {
            string fileName = command.Substring(5).Trim();
            OnScanCommandEntered?.Invoke(fileName);
        }
        else if (command == "help")
        {
            Log("사용 가능한 명령어: scan [파일명], help, clear");
        }
        else if (command == "clear")
        {
            ClearLog();
        }
        else
        {
            Log("잘못된 명령어 입력됨.");
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }

    /// <summary>
    /// 로그 창 전체 초기화
    /// </summary>
    public void ClearLog()
    {
        currentLine = 0;
        sb.Clear();
        logText.text = "";
        autoScroll = true;
    }
}
