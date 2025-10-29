using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// �˾� UI�� �����ϴ� �Ŵ���
/// - ���� ����Ŭ�� �� �˾� ����
/// - �ߺ� �˾� ����
/// - Ȯ���� ���� �� ���� �˾� �ڵ� ����
/// </summary>
public class FilePopupManager : MonoBehaviour
{
    public static FilePopupManager Instance;

    [Header("Prefab & Canvas")]
    public GameObject popupPrefab;  // Popup ������
    public Canvas canvas;           // Inspector���� ���� ����

    // ���� ���� �˾� ��� (���� �̸� ����)
    private Dictionary<string, GameObject> openPopups = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
    }

    /// <summary>
    /// ���� ���� - �˾� ���� (�ߺ� ����)
    /// </summary>
    public void OpenFile(File file)
    {
        if (file == null) return;

        if (canvas == null)
        {
            Debug.LogError("PopupManager: Canvas�� �������� �ʾҽ��ϴ�!");
            return;
        }

        if (popupPrefab == null)
        {
            Debug.LogError("PopupManager: popupPrefab�� �������� �ʾҽ��ϴ�!");
            return;
        }

        string popupKey = file.name;

        // �̹� ���� �̸��� �˾��� ���� �ִٸ� ���� �������� ����
        if (openPopups.ContainsKey(popupKey))
        {
            Debug.Log($"PopupManager: '{popupKey}' �˾��� �̹� ���� �ֽ��ϴ�.");
            return;
        }

        // �˾� ����
        GameObject popupInstance = Instantiate(popupPrefab, canvas.transform, false);
        if (popupInstance == null)
        {
            Debug.LogError("PopupManager: Popup ���� ����!");
            return;
        }

        // ��Ͽ� ���
        openPopups.Add(popupKey, popupInstance);

        // Popup ��ũ��Ʈ ��������
        FilePopup popupScript = popupInstance.GetComponent<FilePopup>();
        if (popupScript != null)
        {
            popupScript.Initialize(file.name, file.extension, canvas);
            popupScript.SetFileKey(popupKey); // ���� �̸� Ű ����

            // ž�� �巡�� �̺�Ʈ ���� (�� �κ��� ž�� �巡�װ� �� �Ǵ� ���� �ذ�)
            if (popupScript.topBar != null)
            {
                EventTrigger trigger = popupScript.topBar.gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                    trigger = popupScript.topBar.gameObject.AddComponent<EventTrigger>();

                // ���� Ʈ���� �ʱ�ȭ (�ߺ� ���� ����)
                trigger.triggers.Clear();

                // PointerDown
                EventTrigger.Entry entryDown = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerDown
                };
                entryDown.callback.AddListener((data) => popupScript.OnTopBarPointerDown((BaseEventData)data));
                trigger.triggers.Add(entryDown);

                // Drag
                EventTrigger.Entry entryDrag = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.Drag
                };
                entryDrag.callback.AddListener((data) => popupScript.OnTopBarDrag((BaseEventData)data));
                trigger.triggers.Add(entryDrag);
            }
            else
            {
                Debug.LogWarning("PopupManager: �˾� �������� topBar�� �Ҵ���� �ʾҽ��ϴ�. �巡�� �Ұ�.");
            }
        }
        else
        {
            Debug.LogError("PopupManager: Popup �����տ� Popup ��ũ��Ʈ�� �����ϴ�!");
            return;
        }

        // ���� �� �ֻ������
        popupInstance.transform.SetAsLastSibling();

        // Ȯ���ڿ� ���� ���� ǥ��
        Transform popupImage = FindDeepChild(popupInstance.transform, "PopupImage");
        Transform popupText = FindDeepChild(popupInstance.transform, "PopupText");

        if (popupImage == null || popupText == null)
        {
            Debug.LogError("PopupPrefab �ȿ� 'PopupImage' �Ǵ� 'PopupText' ������Ʈ�� �����ϴ�!");
            return;
        }

        // Ȯ���ں� ǥ��
        string ext = file.extension != null ? file.extension.ToLower() : "";
        popupImage.gameObject.SetActive(false);
        popupText.gameObject.SetActive(false);

        switch (ext)
        {
            case "png":
            case "jpg":
            case "jpeg":
                popupImage.gameObject.SetActive(true);
                Image img = popupImage.GetComponent<Image>();
                if (img != null && file.imageContent != null)
                    img.sprite = file.imageContent;
                else
                    Debug.LogWarning($"{file.name}.{file.extension} : �̹��� ���� ����");
                break;

            case "txt":
                popupText.gameObject.SetActive(true);
                TMP_Text textComp = popupText.GetComponent<TMP_Text>();
                if (textComp != null)
                    textComp.text = file.textContent ?? $"{file.name}.{file.extension} (���� ����)";
                else
                    Debug.LogWarning($"{file.name}.{file.extension} : TMP_Text ������Ʈ ����");
                break;

            default:
                Debug.LogWarning($"{file.name}.{file.extension} : �������� �ʴ� ���� ����");
                break;
        }
    }

    /// <summary>
    /// Ư�� �̸��� �˾� �ݱ�
    /// </summary>
    public void ClosePopup(string fileName)
    {
        if (openPopups.TryGetValue(fileName, out GameObject popup))
        {
            if (popup != null)
                Destroy(popup);
            openPopups.Remove(fileName);
        }
    }

    /// <summary>
    /// �˾��� ������ �� �Ŵ��������� ����
    /// </summary>
    public void OnPopupDestroyed(string fileKey)
    {
        if (openPopups.ContainsKey(fileKey))
            openPopups.Remove(fileKey);
    }

    /// <summary>
    /// ��� �˾� �ݱ�
    /// </summary>
    public void CloseAllPopups()
    {
        foreach (var popup in openPopups.Values)
        {
            if (popup != null) Destroy(popup);
        }
        openPopups.Clear();
    }

    /// <summary>
    /// �˾� ���� ���� Ȯ��
    /// </summary>
    public bool IsPopupOpen(string fileName)
    {
        return openPopups.ContainsKey(fileName);
    }

    // ������ ���� ��� Ž��
    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
