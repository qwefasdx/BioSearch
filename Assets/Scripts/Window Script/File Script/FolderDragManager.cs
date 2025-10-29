using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// FolderDragManager
/// - ���� �� ���� �巡�� ������ �Ѱ� �����ϴ� �Ŵ���
/// - �巡�� ����/�̵�/���Ḧ �����ϰ�, ȭ�鿡 "���� ������(Ghost)"�� ǥ����
/// - �̱������� �����Ͽ� �������� ���� ����
/// </summary>
public class FolderDragManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static FolderDragManager Instance;

    // �巡�� �� ����ٴϴ� ���� ������ ������Ʈ
    private GameObject ghostIcon;

    // UI�� ��Ʈ ĵ���� (UI ��ǥ ��ȯ��)
    private Canvas mainCanvas;

    // ���� �巡�� ���� ���� ������
    public FolderIcon CurrentDraggedFolderIcon { get; private set; }

    // ���� �巡�� ���� ���� ������
    public FileIcon CurrentDraggedFileIcon { get; private set; }

    void Awake()
    {
        // �̱��� �ʱ�ȭ
        Instance = this;

        // ���� ĵ���� �ڵ� Ž��
        mainCanvas = FindObjectOfType<Canvas>();

        // ĵ������ �������� ������ ��� ���
        if (mainCanvas == null)
            Debug.LogError("���� Canvas �ʿ�!");
    }

    #region �巡�� ����

    /// <summary>
    /// ���� �巡�� ���� �� ȣ���
    /// </summary>
    public void BeginDrag(FolderIcon folderIcon, PointerEventData eventData)
    {
        CurrentDraggedFolderIcon = folderIcon;
        CurrentDraggedFileIcon = null;
        CreateGhost(folderIcon.GetFolder().name, eventData);
    }

    /// <summary>
    /// ���� �巡�� ���� �� ȣ���
    /// </summary>
    public void BeginDrag(FileIcon fileIcon, PointerEventData eventData)
    {
        CurrentDraggedFolderIcon = null;
        CurrentDraggedFileIcon = fileIcon;
        CreateGhost($"{fileIcon.GetFile().name}.{fileIcon.GetFile().extension}", eventData);
    }

    /// <summary>
    /// �巡�� �� ���콺 ��ġ�� �̵��� �� ȣ���
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIcon != null)
            UpdateGhostPosition(eventData);
    }

    /// <summary>
    /// �巡�� ���� �� ȣ���
    /// </summary>
    public void EndDrag()
    {
        DestroyGhost();
    }

    /// <summary>
    /// �ܺο��� �巡�׸� ���� �����ؾ� �� �� ȣ���
    /// </summary>
    public void ForceEndDrag()
    {
        DestroyGhost();
    }

    /// <summary>
    /// ���� ������ ���� �� ���� �ʱ�ȭ
    /// </summary>
    private void DestroyGhost()
    {
        if (ghostIcon != null)
            Destroy(ghostIcon);

        ghostIcon = null;
        CurrentDraggedFolderIcon = null;
        CurrentDraggedFileIcon = null;
    }

    #endregion

    #region Ghost ����/��ġ

    /// <summary>
    /// �巡�� �� ���콺�� ����ٴϴ� ���� ������ ����
    /// </summary>
    /// <param name="name">ǥ���� ����/���� �̸�</param>
    /// <param name="eventData">���콺 �̺�Ʈ ����</param>
    private void CreateGhost(string name, PointerEventData eventData)
    {
        // Ghost ������ ������Ʈ ���� (Image ����)
        ghostIcon = new GameObject("GhostIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        ghostIcon.transform.SetParent(mainCanvas.transform, false);
        ghostIcon.transform.SetAsLastSibling(); // �׻� �� ���� ǥ��

        // ������ ��� �̹��� ����
        Image img = ghostIcon.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0.5f); // ��� ������
        img.raycastTarget = false; // Ŭ�� �Ұ��� ����

        // RectTransform ũ�� ����
        RectTransform rt = ghostIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 40);
        rt.pivot = new Vector2(0.5f, 0.5f);

        // �ؽ�Ʈ ������Ʈ ���� (����/���� �̸� ǥ��)
        TextMeshProUGUI txt = new GameObject("GhostText", typeof(RectTransform), typeof(TextMeshProUGUI))
            .GetComponent<TextMeshProUGUI>();
        txt.text = name;
        txt.fontSize = 24;
        txt.color = Color.yellow; // ����� �۾�
        txt.alignment = TextAlignmentOptions.Center;
        txt.raycastTarget = false;
        txt.transform.SetParent(ghostIcon.transform, false);

        // �ؽ�Ʈ�� Ghost ������ ��ü�� �°� Ȯ��
        txt.rectTransform.anchorMin = Vector2.zero;
        txt.rectTransform.anchorMax = Vector2.one;
        txt.rectTransform.offsetMin = Vector2.zero;
        txt.rectTransform.offsetMax = Vector2.zero;

        // �ʱ� ��ġ ������Ʈ
        UpdateGhostPosition(eventData);
    }

    /// <summary>
    /// ���� �������� ���콺 ��ġ�� ���� �̵�
    /// </summary>
    private void UpdateGhostPosition(PointerEventData eventData)
    {
        Camera cam = mainCanvas.worldCamera; // ĵ������ ������ ī�޶� ����
        Vector3 worldPos;

        // ��ũ�� ��ǥ �� ���� ��ǥ ��ȯ
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            mainCanvas.transform as RectTransform,
            eventData.position,
            cam,
            out worldPos
        );

        // Ghost �������� ��ġ ����
        ghostIcon.GetComponent<RectTransform>().position = worldPos;
    }

    #endregion
}
