using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 신체 부위별 버튼, 정상/이상 스프라이트를 관리.
/// </summary>
[System.Serializable]
public class BodyPart
{
    public string partName;       // 부위 이름 (예: "Head")
    public Button button;         // 해당 부위 버튼
    public Sprite normalSprite;   // 정상 상태 스프라이트
    public Sprite abnormalSprite; // 이상 상태 스프라이트
}

/// <summary>
/// 부위별 버튼 클릭 시 FileWindow의 폴더 정보를 기반으로
/// 정상/이상 스프라이트를 전환하는 관리 클래스.
/// </summary>
public class BodyPartViewer : MonoBehaviour
{
    [Header("왼쪽 카메라창")]
    [SerializeField] private Image cameraPanel;  // 스프라이트 표시용 Image

    [Header("부위 & 버튼 매핑")]
    [SerializeField] private BodyPart[] bodyParts;  // Inspector 등록용 배열

    [Header("File Window 연결")]
    [SerializeField] private FileWindow fileWindow; // 폴더 구조 참조용

    // 빠른 조회를 위한 캐시
    private Dictionary<string, BodyPart> bodyPartLookup = new Dictionary<string, BodyPart>();

    void Awake()
    {
        // 캐시 초기화
        foreach (var part in bodyParts)
        {
            if (string.IsNullOrEmpty(part.partName))
            {
                Debug.LogWarning("BodyPart 이름이 비어 있습니다.", this);
                continue;
            }

            if (bodyPartLookup.ContainsKey(part.partName))
            {
                Debug.LogWarning($"중복된 BodyPart 이름: {part.partName}", this);
                continue;
            }

            bodyPartLookup[part.partName] = part;
        }
    }

    void Start()
    {
        // 각 버튼 클릭 이벤트 등록
        foreach (var part in bodyParts)
        {
            if (part.button == null)
            {
                Debug.LogWarning($"{part.partName} 버튼이 설정되지 않았습니다.", this);
                continue;
            }

            string partName = part.partName; // 로컬 복사 (람다 캡처 방지)
            part.button.onClick.AddListener(() => OnBodyPartClicked(partName));
        }
    }

    /// <summary>
    /// 버튼 클릭 시 해당 부위의 정상/이상 스프라이트 표시
    /// </summary>
    private void OnBodyPartClicked(string partName)
    {
        if (cameraPanel == null)
        {
            Debug.LogWarning("cameraPanel이 설정되지 않았습니다.", this);
            return;
        }

        if (fileWindow == null)
        {
            Debug.LogWarning("fileWindow가 연결되지 않았습니다.", this);
            return;
        }

        Folder root = fileWindow.GetRootFolder();
        if (root == null)
        {
            Debug.LogWarning("FileWindow에 루트 폴더가 없습니다.", this);
            return;
        }

        Folder targetFolder = FindFolderByName(root, partName);
        if (targetFolder == null)
        {
            Debug.LogWarning($"'{partName}' 폴더를 찾을 수 없습니다.", this);
            return;
        }

        if (!bodyPartLookup.TryGetValue(partName, out BodyPart part))
        {
            Debug.LogWarning($"'{partName}'에 대응되는 BodyPart가 없습니다.", this);
            return;
        }

        // 이상 상태에 따라 스프라이트 교체
        cameraPanel.sprite = targetFolder.isAbnormal && part.abnormalSprite != null
            ? part.abnormalSprite
            : part.normalSprite;
    }

    /// <summary>
    /// 폴더 이름으로 Folder를 재귀 탐색
    /// </summary>
    private Folder FindFolderByName(Folder folder, string name)
    {
        if (folder == null)
            return null;

        if (folder.name == name)
            return folder;

        foreach (var child in folder.children)
        {
            Folder found = FindFolderByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
}
