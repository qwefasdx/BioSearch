using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class LogWindowManager : MonoBehaviour
{
    public static LogWindowManager Instance;

    [Header("UI References")]
    public TMP_Text logText;           // Scroll View ¡æ Content ¡æ TMP Text
    public TMP_InputField inputField;  // Input Field
    public ScrollRect scrollRect;      // Scroll View

    [Header("Settings")]
    public int maxLines = 20;

    private string[] lines;
    private int currentLine = 0;
    private StringBuilder sb;

    private void Start()
{
    Log("gamestart");
}
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
        for (int i = currentLine - 1; i >= start; i--)
            sb.AppendLine(lines[i % maxLines]);

        logText.text = sb.ToString();
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnCommandEntered(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        Log("please input key: " + command);

        switch (command.ToLower())
        {
            case "help":
                Log("command: help, hello, clear");
                Debug.Log("got it.");
                break;
            case "hello":
                Log("Hello, Commander!");
                break;
            case "clear":
                ClearLog();
                break;
            default:
                Log("error: " + command);
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
