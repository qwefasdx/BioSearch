using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 파일 아이콘의 공통 동작 (이 클래스는 직접 사용하지 않고 상속해서 TxtIcon, PngIcon을 만듦)
/// </summary>
public abstract class FileIcon : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text fileNameText;

    protected FileWindow fileWindow;
    protected File file;

    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    public virtual void Setup(File file, FileWindow window)
    {
        this.file = file;
        this.fileWindow = window;

        if (fileNameText != null)
            fileNameText.text = $"{file.name}.{file.extension}";

        SetSelected(false);
    }

    public File GetFile() => file;

    public void SetSelected(bool selected)
    {
        if (fileNameText == null) return;
        fileNameText.color = selected ? selectedColor : normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        fileWindow.SetSelectedFileIcon(this);

        if (eventData.clickCount == 2)
            OnDoubleClick();
    }

    // 더블 클릭 시 동작 (확장자별로 구현)
    protected abstract void OnDoubleClick();
}
