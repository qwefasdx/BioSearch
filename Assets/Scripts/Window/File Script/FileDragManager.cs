using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FileDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static FileDragManager Instance;

    private FileIcon draggingIcon;
    private GameObject ghostIcon;
    private Canvas mainCanvas;

    void Awake()
    {
        Instance = this;
        mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
            Debug.LogError("씬에 Canvas 필요!");
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 시작");
        draggingIcon = eventData.pointerDrag?.GetComponent<FileIcon>();
        if (draggingIcon == null) return;

        CreateGhost(eventData);
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래그중");
        if (ghostIcon != null)
            UpdateGhostPosition(eventData);
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("종료시작");
        EndDrag();
    }

    // 기존 EndDrag와 동일하지만 안전하게 호출 가능하도록 public
    public void ForceEndDrag()
    {
        if (ghostIcon != null)
        {
            Debug.Log("종료");
            Destroy(ghostIcon);
            ghostIcon = null;
        }
        draggingIcon = null;
    }

    private void CreateGhost(PointerEventData eventData)
    {
        // GhostIcon 생성
        ghostIcon = new GameObject("GhostIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        ghostIcon.transform.SetParent(mainCanvas.transform, false);
        ghostIcon.transform.SetAsLastSibling();

        Image img = ghostIcon.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0.5f);
        img.raycastTarget = false;

        CanvasGroup group = ghostIcon.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        RectTransform rt = ghostIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 40);
        rt.pivot = new Vector2(0.5f, 0.5f);

        // 텍스트 추가
        TextMeshProUGUI txt = new GameObject("GhostText", typeof(RectTransform), typeof(TextMeshProUGUI))
            .GetComponent<TextMeshProUGUI>();
        txt.text = draggingIcon.GetFolder().name;
        txt.fontSize = 24;
        txt.color = Color.yellow;
        txt.alignment = TextAlignmentOptions.Center;
        txt.raycastTarget = false;
        txt.transform.SetParent(ghostIcon.transform, false);

        txt.rectTransform.anchorMin = Vector2.zero;
        txt.rectTransform.anchorMax = Vector2.one;
        txt.rectTransform.offsetMin = Vector2.zero;
        txt.rectTransform.offsetMax = Vector2.zero;

        UpdateGhostPosition(eventData);
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        Camera cam = mainCanvas.worldCamera;
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            mainCanvas.transform as RectTransform,
            eventData.position,
            cam,
            out worldPos
        );
        ghostIcon.GetComponent<RectTransform>().position = worldPos;
    }

    // 드래그 종료 시 즉시 Ghost 제거
    public void EndDrag()
    {
        if (ghostIcon != null)
        {
            Debug.Log("종료");
            Destroy(ghostIcon);
            ghostIcon = null;
        }
        draggingIcon = null;
    }

    void Update()
    {
        // draggingIcon이 null인데 ghostIcon이 남아있으면 안전하게 제거
        if (ghostIcon != null && draggingIcon == null)
        {
            Destroy(ghostIcon);
            ghostIcon = null;
        }
    }

    public FileIcon GetDraggingIcon() => draggingIcon;
}
