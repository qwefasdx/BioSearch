using UnityEngine;
using UnityEngine.UI;

public class SelectPopupManager : MonoBehaviour
{
    [Header("버튼 연결")]
    public Button acceptButton;   // 수용 버튼
    public Button releaseButton;  // 방출 버튼

    [Header("팝업 프리팹")]
    public GameObject acceptPopupPrefab;   // "수용하시겠습니까?" 팝업 프리팹
    public GameObject releasePopupPrefab;  // "방출하시겠습니까?" 팝업 프리팹

    [Header("부모 오브젝트")]
    public Transform popupParent; // 팝업을 띄울 부모 (예: Canvas)

    private GameObject currentPopup; // 현재 떠있는 팝업 (1개만 존재)

    private void Start()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        if (releaseButton != null)
            releaseButton.onClick.AddListener(OnReleaseButtonClicked);
    }

    // 수용 버튼 클릭 시
    private void OnAcceptButtonClicked()
    {
        ShowPopup(acceptPopupPrefab);
    }

    // 방출 버튼 클릭 시
    private void OnReleaseButtonClicked()
    {
        ShowPopup(releasePopupPrefab);
    }

    // 팝업 띄우기 (중복 방지)
    private void ShowPopup(GameObject popupPrefab)
    {
        if (currentPopup != null)
            return; // 이미 떠 있으면 무시

        if (popupPrefab != null && popupParent != null)
        {
            currentPopup = Instantiate(popupPrefab, popupParent);

            // X 버튼 찾기
            Button xButton = currentPopup.transform.Find("XButton")?.GetComponent<Button>();
            if (xButton != null)
                xButton.onClick.AddListener(ClosePopup);

            // Yes / No 버튼 찾기
            Button yesButton = currentPopup.transform.Find("YesButton")?.GetComponent<Button>();
            Button noButton = currentPopup.transform.Find("NoButton")?.GetComponent<Button>();

            if (yesButton != null)
                yesButton.onClick.AddListener(ClosePopup);
            if (noButton != null)
                noButton.onClick.AddListener(ClosePopup);
        }
    }

    // 팝업 닫기
    private void ClosePopup()
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup);
            currentPopup = null;
        }
    }
}
