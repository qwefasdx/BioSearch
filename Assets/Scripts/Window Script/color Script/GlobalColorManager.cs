using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalColorManager : MonoBehaviour
{
    [Header("1. Scene ��ü")]
    public Image[] globalPanels;
    public Color globalColor = Color.black;

    [Header("2. �α�â Panels")]
    public Image[] logPanels;
    public Color logPanelColor = Color.black;

    [Header("3. �α�â Texts (�α� + �Է��ʵ� �ؽ�Ʈ)")]
    public TMP_Text[] logTexts;
    public TMP_Text[] logPlaceholders;
    public Color logTextColor = Color.green;

    [Header("4. ����â Panels")]
    public Image[] filePanels;
    public Color filePanelColor = Color.black;

    [Header("5. ����â Texts")]
    public TMP_Text[] fileTexts;
    public Color fileTextColor = Color.cyan;

    [Header("6. �̻� ���� Texts")]
    public Color abnormalFolderTextColor = Color.red;

    private void Update()
    {
        ApplyColors();
    }

    private void ApplyColors()
    {
        // 1. Scene ��ü
        foreach (var panel in globalPanels) if (panel != null) panel.color = globalColor;

        // 2. �α�â Panels
        foreach (var panel in logPanels) if (panel != null) panel.color = logPanelColor;

        // 3. �α�â Texts
        foreach (var text in logTexts) if (text != null) text.color = logTextColor;
        foreach (var placeholder in logPlaceholders) if (placeholder != null) placeholder.color = logTextColor;

        // 4. ����â Panels
        foreach (var panel in filePanels) if (panel != null) panel.color = filePanelColor;

        // 5. ����â Texts (���� ����)
        foreach (var text in fileTexts) if (text != null) text.color = fileTextColor;

        // 6. �̻� ���� �ؽ�Ʈ�� FileIcon�� ���� ����
    }

    // ��Ÿ�ӿ��� ���� ����
    public void SetAbnormalFolderTextColor(Color color) => abnormalFolderTextColor = color;
}
