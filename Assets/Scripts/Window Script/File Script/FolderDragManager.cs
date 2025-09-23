using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FolderDragManager : MonoBehaviour
{
    public static FolderDragManager Instance;

    private GameObject ghostIcon;
    private Canvas mainCanvas;

    public FolderIcon CurrentDraggedFolderIcon { get; private set; }
    public FileIcon CurrentDraggedFileIcon { get; private set; }

    void Awake()
    {
        Instance = this;
        mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
            Debug.LogError("씬에 Canvas 필요!");
    }

    #region 드래그 통제

    public void BeginDrag(FolderIcon folderIcon, PointerEventData eventData)
    {
        CurrentDraggedFolderIcon = folderIcon;
        CurrentDraggedFileIcon = null;
        CreateGhost(folderIcon.GetFolder().name, eventData);
    }

    public void BeginDrag(FileIcon fileIcon, PointerEventData eventData)
    {
        CurrentDraggedFolderIcon = null;
        CurrentDraggedFileIcon = fileIcon;
        CreateGhost($"{fileIcon.GetFile().name}.{fileIcon.GetFile().extension}", eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIcon != null)
            UpdateGhostPosition(eventData);
    }

    public void EndDrag()
    {
        DestroyGhost();
    }

    public void ForceEndDrag()
    {
        DestroyGhost();
    }

    private void DestroyGhost()
    {
        if (ghostIcon != null)
            Destroy(ghostIcon);

        ghostIcon = null;
        CurrentDraggedFolderIcon = null;
        CurrentDraggedFileIcon = null;
    }

    #endregion

    #region Ghost 생성/위치

    private void CreateGhost(string name, PointerEventData eventData)
    {
        ghostIcon = new GameObject("GhostIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        ghostIcon.transform.SetParent(mainCanvas.transform, false);
        ghostIcon.transform.SetAsLastSibling();

        Image img = ghostIcon.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0.5f);
        img.raycastTarget = false;

        RectTransform rt = ghostIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 40);
        rt.pivot = new Vector2(0.5f, 0.5f);

        TextMeshProUGUI txt = new GameObject("GhostText", typeof(RectTransform), typeof(TextMeshProUGUI))
            .GetComponent<TextMeshProUGUI>();
        txt.text = name;
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

    #endregion
}
