using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class LogWindowManager : MonoBehaviour
{
    public static LogWindowManager Instance;

    [Header("UI References")]
    public TMP_Text logText;
    public TMP_InputField inputField;
    public ScrollRect scrollRect;

    [Header("Settings")]
    public int maxLines = 50;

    private string[] lines;
    private int currentLine = 0;
    private StringBuilder sb;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lines = new string[maxLines];
        sb = new StringBuilder();

        logText.text = "";
        inputField.onSubmit.AddListener(OnCommandEntered);
        inputField.ActivateInputField();
    }

    public void Log(string message)
    {
        lines[currentLine % maxLines] = "> " + message;
        currentLine++;

        sb.Clear();
        int start = Mathf.Max(0, currentLine - maxLines);
        for (int i = start; i < currentLine; i++)
        {
            sb.AppendLine(lines[i % maxLines]); // 한 줄씩 아래→위 쌓기
        }

        logText.text = sb.ToString();

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // 항상 맨 아래 표시
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
    }
}
