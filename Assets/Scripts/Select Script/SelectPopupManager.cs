using UnityEngine;
using UnityEngine.UI;
public class SelectPopupManager : MonoBehaviour
{
    [Header("��ư ����")]
    public Button acceptButton;
    public Button releaseButton;

    [Header("�˾� ������")]
    public GameObject acceptPopupPrefab;
    public GameObject releasePopupPrefab;

    [Header("�˾� �θ�")]
    public Transform popupParent;

    private GameObject currentPopup;

    // ��ƼƼ/�α� ����
    public SanityManager sanityManager;
    public LogWindowManager logWindow;
    public FileWindow fileWindow;

    private void Start()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(() => ShowPopup(acceptPopupPrefab, true));

        if (releaseButton != null)
            releaseButton.onClick.AddListener(() => ShowPopup(releasePopupPrefab, false));
    }

    private void ShowPopup(GameObject popupPrefab, bool isAccept)
    {
        if (currentPopup != null) return;

        if (popupPrefab == null || popupParent == null) return;

        currentPopup = Instantiate(popupPrefab, popupParent);

        // PopupPanel �� X ��ư ã��
        Button xButton = currentPopup.transform.Find("PopupPanel/XButton")?.GetComponent<Button>();
        if (xButton != null)
            xButton.onClick.AddListener(ClosePopup);

        // contentPanel �� Yes/No ��ư ã��
        Transform content = currentPopup.transform.Find("PopupPanel/contentPanel");
        if (content != null)
        {
            Button yesButton = content.Find("Yes")?.GetComponent<Button>();
            Button noButton = content.Find("No")?.GetComponent<Button>();

            SelectPopup popupComp = currentPopup.GetComponent<SelectPopup>();
            if (popupComp != null)
            {
                popupComp.yesButton = yesButton;
                popupComp.noButton = noButton;
                popupComp.closeButton = xButton;

                // Yes Ŭ�� �� ó��
                popupComp.onYes += () =>
                {
                    HandleYes(isAccept);
                };

                // No Ŭ���� �׳� �ݱ�
            }
        }
    }

    private void HandleYes(bool isAccept)
    {
        Folder root = fileWindow.GetRootFolder();
        if (root == null) return;

        int abnormalCount = CountAbnormal(root);

        if ((isAccept && abnormalCount == 0) || (!isAccept && abnormalCount > 0))
        {
            logWindow.Log("����!");
        }
        else
        {
            logWindow.Log("����!");
            if (sanityManager != null)
                sanityManager.DecreaseSanity(40f);
        }
    }

    private int CountAbnormal(Folder folder)
    {
        if (folder == null) return 0;

        int count = folder.isAbnormal ? 1 : 0;
        foreach (var child in folder.children)
            count += CountAbnormal(child);
        foreach (var file in folder.files)
            if (file.isAbnormal) count++;
        return count;
    }

    private void ClosePopup()
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup);
            currentPopup = null;
        }
    }
}