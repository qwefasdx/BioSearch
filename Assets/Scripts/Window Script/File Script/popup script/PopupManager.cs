using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("Prefab & Canvas")]
    public GameObject popupPrefab;  // Popup 프리팹
    public Canvas canvas;           // Inspector에서 지정 가능

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenFile(File file)
    {
        if (canvas == null)
        {
            Debug.LogError("PopupManager: Canvas가 지정되지 않았습니다!");
            return;
        }

        GameObject popupInstance = Instantiate(popupPrefab, canvas.transform, false);
        Popup popupScript = popupInstance.GetComponent<Popup>();
        if (popupScript != null)
        {
            popupScript.Initialize(file.name, file.extension, canvas);

            // TopBar 드래그 이벤트 연결
            EventTrigger trigger = popupScript.topBar.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = popupScript.topBar.gameObject.AddComponent<EventTrigger>();

            // PointerDown
            EventTrigger.Entry entryDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            entryDown.callback.AddListener((data) => popupScript.OnTopBarPointerDown(data));
            trigger.triggers.Add(entryDown);

            // Drag
            EventTrigger.Entry entryDrag = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Drag
            };
            entryDrag.callback.AddListener((data) => popupScript.OnTopBarDrag(data));
            trigger.triggers.Add(entryDrag);
        }

        // 생성 후 최상단으로
        popupInstance.transform.SetAsLastSibling();

        // 파일 확장자에 따라 내용 표시
        Transform popupImage = FindDeepChild(popupInstance.transform, "PopupImage");
        Transform popupText = FindDeepChild(popupInstance.transform, "PopupText");

        if (popupImage == null || popupText == null)
        {
            Debug.LogError("PopupPrefab 안에 'PopupImage' 또는 'PopupText' 오브젝트가 없습니다!");
            return;
        }

        switch (file.extension.ToLower())
        {
            case "png":
                popupImage.gameObject.SetActive(true);
                popupText.gameObject.SetActive(false);
                Image img = popupImage.GetComponent<Image>();
                if (img != null && file.imageContent != null)
                    img.sprite = file.imageContent;
                break;

            case "txt":
                popupImage.gameObject.SetActive(false);
                popupText.gameObject.SetActive(true);
                TMP_Text textComp = popupText.GetComponent<TMP_Text>();
                if (textComp != null)
                    textComp.text = file.textContent ?? $"{file.name}.{file.extension} (내용 없음)";
                break;

            default:
                popupImage.gameObject.SetActive(false);
                popupText.gameObject.SetActive(false);
                break;
        }
    }

    // 프리팹 내부 재귀 탐색
    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
