using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FileIcon : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TMP_Text fileNameText;

    private string fileName;
    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    private FileWindow fileWindow; // 상위 FileWindow 참조

    private Color normalColor = Color.white;   // 기본 색
    private Color selectedColor = Color.yellow; // 선택 시 색

    public void Setup(string name, FileWindow window)
    {
        fileName = name;
        fileWindow = window;
        fileNameText.text = name;
        iconImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 더블클릭 판별
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            OnDoubleClick();
        }
        else
        {
            OnSingleClick();
        }
        lastClickTime = Time.time;
    }

    private void OnSingleClick()
    {
        // 이전 선택 해제
        if (fileWindow.selectedIcon != null && fileWindow.selectedIcon != this)
        {
            fileWindow.selectedIcon.iconImage.color = fileWindow.selectedIcon.normalColor;
            fileWindow.selectedIcon.fileNameText.color = fileWindow.selectedIcon.normalColor;
        }

        // 현재 선택 강조
        iconImage.color = selectedColor;
        fileNameText.color = selectedColor;

        // FileWindow에 선택 아이콘 등록
        fileWindow.selectedIcon = this;

        Debug.Log("파일 선택: " + fileName);
    }

    private void OnDoubleClick()
    {
        Debug.Log("파일 열기: " + fileName);
        // 열람 창 열기 기능 추가 예정
    }
}
