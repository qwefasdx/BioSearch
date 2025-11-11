using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // 드래그 시작 시 부모를 Canvas로 변경 (시각적으로 따라오게 하기)
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out movePos
        );
        rectTransform.anchoredPosition = movePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        //  드래그 종료 시 다시 원래 부모로 복귀
        transform.SetParent(originalParent);

        // 휴지통 위가 아니면 원래 자리로 돌아가기
        if (!eventData.pointerEnter || eventData.pointerEnter.GetComponent<UITrashBin>() == null)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void DeleteSelf()
    {
        Destroy(gameObject);
    }
}

