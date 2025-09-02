using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class FileIcon : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text fileNameText;

    private FileWindow fileWindow;
    private Folder folder;
    private float lastClickTime;
    private float doubleClickThreshold = 0.3f;

    private GlobalColorManager gcm;

    private bool isSelected = false;

    public void Setup(Folder folder, FileWindow window)
    {
        this.folder = folder;
        this.fileWindow = window;
        this.gcm = FindObjectOfType<GlobalColorManager>();

        fileNameText.text = folder.name;
        ApplyFolderColor();
    }

    public Folder GetFolder() => folder;

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        ApplyFolderColor();
    }

    private void ApplyFolderColor()
    {
        if (folder == null || fileNameText == null || gcm == null) return;

        if (folder.isAbnormal)
        {
            // 이상 폴더는 무조건 빨간색
            fileNameText.color = gcm.abnormalFolderTextColor;
        }
        else if (isSelected)
        {
            // 선택 상태 색상 (원하면 GlobalColorManager에서 별도로 정의 가능)
            fileNameText.color = gcm.fileTextColor; // 선택 색상을 원하면 따로 추가 가능
        }
        else
        {
            // 일반 폴더 색상
            fileNameText.color = gcm.fileTextColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        fileWindow.SetSelectedIcon(this);

        if (Time.time - lastClickTime < doubleClickThreshold)
            fileWindow.OpenSelected();

        lastClickTime = Time.time;
    }
}
