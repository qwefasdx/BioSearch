using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BodyPart
{
    public string partName;   // 부위 이름 (예: "head")
    public Button button;     // 해당 버튼
    public Sprite sprite;     // 보여줄 스프라이트
}

public class BodyPartViewer : MonoBehaviour
{
    [Header("왼쪽 카메라창")]
    public Image cameraPanel;  // Image 컴포넌트

    [Header("부위 & 버튼 매핑")]
    public BodyPart[] bodyParts;  // Inspector에서 버튼과 스프라이트를 한 쌍으로 등록

    void Start()
    {
        // 각 버튼에 클릭 이벤트 연결
        foreach (var part in bodyParts)
        {
            string partName = part.partName;  // 지역 변수로 복사 (람다에서 안전하게 사용)
            if (part.button != null)
            {
                part.button.onClick.AddListener(() => ShowPart(partName));
            }
        }
    }

    // 버튼 클릭 시 호출
    public void ShowPart(string partName)
    {
        foreach (var part in bodyParts)
        {
            if (part.partName == partName)
            {
                if (cameraPanel != null)
                    cameraPanel.sprite = part.sprite;
                return;
            }
        }
        Debug.LogWarning("Unknown part: " + partName);
    }
}
