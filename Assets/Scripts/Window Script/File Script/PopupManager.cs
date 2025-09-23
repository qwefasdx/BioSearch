using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("Prefabs")]
    public GameObject txtPopupPrefab;
    public GameObject pngPopupPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void OpenFile(File file)
    {
        switch (file.extension.ToLower())
        {
            case "txt":
                Instantiate(txtPopupPrefab, transform);
                break;

            case "png":
                Instantiate(pngPopupPrefab, transform);
                break;

            default:
                Debug.LogWarning($"지원하지 않는 확장자: {file.extension}");
                break;
        }
    }
}
