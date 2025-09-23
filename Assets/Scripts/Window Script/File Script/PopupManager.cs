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
        GameObject popupInstance = null;
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("씬에 Canvas가 없습니다!");
            return;
        }

        switch (file.extension.ToLower())
        {
            case "txt":
                popupInstance = Instantiate(txtPopupPrefab, canvas.transform, false);
                break;

            case "png":
                popupInstance = Instantiate(pngPopupPrefab, canvas.transform, false);
                break;

            default:
                Debug.LogWarning($"지원하지 않는 확장자: {file.extension}");
                break;
        }

        if (popupInstance != null)
        {
            popupInstance.transform.SetAsLastSibling(); // 최상위에 표시
        }
    }

}
