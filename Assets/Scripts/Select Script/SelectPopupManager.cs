using UnityEngine;
using UnityEngine.UI;
public class SelectPopupManager : MonoBehaviour
{
    [Header("버튼 연결")]
    public Button acceptButton;
    public Button releaseButton;

    [Header("팝업 프리팹")]
    public GameObject acceptPopupPrefab;
    public GameObject releasePopupPrefab;

    [Header("팝업 부모")]
    public Transform popupParent;

    private GameObject currentPopup;

    // 샌티티/로그 연동
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

        // PopupPanel 안 X 버튼 찾기
        Button xButton = currentPopup.transform.Find("PopupPanel/XButton")?.GetComponent<Button>();
        if (xButton != null)
            xButton.onClick.AddListener(ClosePopup);

        // contentPanel 안 Yes/No 버튼 찾기
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

                // Yes 클릭 시 처리
                popupComp.onYes += () =>
                {
                    HandleYes(isAccept);
                };

                // No 클릭은 그냥 닫기
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
            logWindow.Log("성공!");
        }
        else
        {
            logWindow.Log("실패!");
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