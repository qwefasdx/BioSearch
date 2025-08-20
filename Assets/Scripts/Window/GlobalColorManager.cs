using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalColorManager : MonoBehaviour
{
    [Header("Panels (배경 등)")]
    public Image[] panels;

    [Header("Texts (로그, 입력 텍스트)")]
    public TMP_Text[] texts;

    [Header("Placeholders (입력 필드 힌트 텍스트)")]
    public TMP_Text[] placeholders;

    [Header("Colors")]
    public Color panelAndPlaceholderColor = Color.black; // Panel + Placeholder 통합
    public Color textColor = Color.green;                // Text는 별도

    private void Update()
    {
        ApplyColors();
    }

    private void ApplyColors()
    {
        // Panels + Placeholder 색상 통합 적용
        foreach (var panel in panels)
        {
            if (panel != null)
                panel.color = panelAndPlaceholderColor;
        }

        foreach (var placeholder in placeholders)
        {
            if (placeholder != null)
                placeholder.color = panelAndPlaceholderColor;
        }

        // Texts 색상
        foreach (var text in texts)
        {
            if (text != null)
                text.color = textColor;
        }
    }

    // 런타임에서 색상 변경
    public void SetPanelAndPlaceholderColor(Color color)
    {
        panelAndPlaceholderColor = color;
    }

    public void SetTextColor(Color color)
    {
        textColor = color;
    }
}
