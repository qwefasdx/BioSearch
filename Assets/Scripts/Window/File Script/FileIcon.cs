using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FileIcon : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TMP_Text fileNameText;

    private FileWindow fileWindow;
    private Folder folder;

    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;
    private float lastClickTime;
    private float doubleClickThreshold = 0.3f;

    public void Setup(Folder folder, FileWindow window)
    {
        this.folder = folder;
        this.fileWindow = window;
        fileNameText.text = folder.name;
        SetSelected(false);
    }

    public Folder GetFolder() => folder;

    public void SetSelected(bool selected)
    {
        iconImage.color = selected ? selectedColor : normalColor;
        fileNameText.color = selected ? selectedColor : normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        fileWindow.SetSelectedIcon(this);

        if (Time.time - lastClickTime < doubleClickThreshold)
            fileWindow.OpenSelected();

        lastClickTime = Time.time;
    }
}
