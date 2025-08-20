using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class LogWindowManager : MonoBehaviour
{
    public static LogWindowManager Instance;

    [Header("UI References")]
    public TMP_Text logText;           // ScrollRect Content로 바로 연결
    public TMP_InputField inputField;
    public ScrollRect scrollRect;

    [Header("Settings")]
    public int maxLines = 50;

    private string[] lines;
    private int currentLine = 0;
    private StringBuilder sb;

    private bool autoScroll = false;
    private bool userScrolling = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lines = new string[maxLines];
        sb = new StringBuilder();

        logText.text = "";
        inputField.onSubmit.AddListener(OnCommandEntered);
        inputField.ActivateInputField();

        if (scrollRect == null)
            scrollRect = GetComponentInChildren<ScrollRect>();

        // Elastic Scroll
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.inertia = true;

        // TMP_Text를 ScrollRect Content로 설정
        scrollRect.content = logText.rectTransform;

        // 스크롤 중 플래그
        scrollRect.onValueChanged.AddListener(_ => userScrolling = true);

        // TMP_Text 세팅: 아래쪽 시작
        logText.rectTransform.pivot = new Vector2(0, 0);          // Pivot 아래쪽
        logText.rectTransform.anchorMin = new Vector2(0, 0);       // Bottom Left
        logText.rectTransform.anchorMax = new Vector2(1, 0);       // Bottom Right Stretch
    }

    private void LateUpdate()
    {
        // TMP_Text 높이 반영
        float contentHeight = logText.preferredHeight;
        Vector2 size = logText.rectTransform.sizeDelta;
        logText.rectTransform.sizeDelta = new Vector2(size.x, contentHeight);

        // 새 로그 입력 시 자동 스크롤
        if (autoScroll && !userScrolling)
        {
            if (contentHeight > scrollRect.viewport.rect.height)
                scrollRect.verticalNormalizedPosition = 0f; // 맨 아래
            else
                scrollRect.verticalNormalizedPosition = 0f; // content가 짧아도 아래쪽 표시

            autoScroll = false;
        }

        userScrolling = false;
    }

    public void Log(string message)
    {
        lines[currentLine % maxLines] = "> " + message;
        currentLine++;

        sb.Clear();
        int start = Mathf.Max(0, currentLine - maxLines);

        // 아래에서 위로 쌓이도록 순서 유지
        for (int i = start; i < currentLine; i++)
        {
            sb.AppendLine(lines[i % maxLines]);
        }

        logText.text = sb.ToString();

        // 새 로그 발생 시 자동 스크롤
        autoScroll = true;
    }

    private void OnCommandEntered(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        Log("명령어 입력: " + command);

        switch (command.ToLower())
        {
            case "help":
                Log("사용 가능한 명령어: help, hello, clear");
                break;
            case "hello":
                Log("Hello, Commander!");
                break;
            case "clear":
                ClearLog();
                break;
            default:
                Log("알 수 없는 명령어: " + command);
                break;
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void ClearLog()
    {
        currentLine = 0;
        sb.Clear();
        logText.text = "";
        autoScroll = true;
    }
}
