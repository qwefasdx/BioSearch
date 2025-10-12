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
        // 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Canvas 자동 탐색 (혹시 Inspector에 지정 안 했을 경우 대비)
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
    }

    public void OpenFile(File file)
    {
        if (canvas == null)
        {
            Debug.LogError("PopupManager: Canvas가 지정되지 않았습니다!");
            return;
        }

        if (popupPrefab == null)
        {
            Debug.LogError("PopupManager: popupPrefab이 지정되지 않았습니다!");
            return;
        }

        // 팝업 생성
        GameObject popupInstance = Instantiate(popupPrefab, canvas.transform, false);
        if (popupInstance == null)
        {
            Debug.LogError("PopupManager: Popup 생성 실패!");
            return;
        }

        // Popup 스크립트 가져오기
        Popup popupScript = popupInstance.GetComponent<Popup>();
        if (popupScript != null)
        {
            popupScript.Initialize(file.name, file.extension, canvas);

            // TopBar 드래그 이벤트 연결
            EventTrigger trigger = popupScript.topBar.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = popupScript.topBar.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear(); // 기존 트리거 초기화 (중복 방지)

            // PointerDown
            EventTrigger.Entry entryDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            entryDown.callback.AddListener((data) => popupScript.OnTopBarPointerDown((PointerEventData)data));
            trigger.triggers.Add(entryDown);

            // Drag
            EventTrigger.Entry entryDrag = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Drag
            };
            entryDrag.callback.AddListener((data) => popupScript.OnTopBarDrag((PointerEventData)data));
            trigger.triggers.Add(entryDrag);
        }
        else
        {
            Debug.LogError("PopupManager: Popup 프리팹에 Popup 스크립트가 없습니다!");
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

        // 확장자별 표시 로직
        string ext = file.extension.ToLower();
        popupImage.gameObject.SetActive(false);
        popupText.gameObject.SetActive(false);

        switch (ext)
        {
            case "png":
            case "jpg":
            case "jpeg":
                popupImage.gameObject.SetActive(true);
                Image img = popupImage.GetComponent<Image>();
                if (img != null && file.imageContent != null)
                    img.sprite = file.imageContent;
                else
                    Debug.LogWarning($"{file.name}.{file.extension} : 이미지 내용 없음");
                break;

            case "txt":
                popupText.gameObject.SetActive(true);
                TMP_Text textComp = popupText.GetComponent<TMP_Text>();
                if (textComp != null)
                    textComp.text = file.textContent ?? $"{file.name}.{file.extension} (내용 없음)";
                else
                    Debug.LogWarning($"{file.name}.{file.extension} : TMP_Text 컴포넌트 없음");
                break;

            default:
                Debug.LogWarning($"{file.name}.{file.extension} : 지원되지 않는 파일 형식");
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
