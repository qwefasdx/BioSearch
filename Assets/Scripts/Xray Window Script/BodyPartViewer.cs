using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class BodyPart
{
    public string partName;       // ���� �̸� (��: "Head")
    public Button button;         // �ش� ���� ��ư
    public Sprite normalSprite;   // ���� ���� ��������Ʈ
    public Sprite abnormalSprite; // �̻� ���� ��������Ʈ
}

public class BodyPartViewer : MonoBehaviour
{
    [Header("���� ī�޶�â")]
    public Image cameraPanel;  // ��������Ʈ�� ǥ���� Image

    [Header("���� & ��ư ����")]
    public BodyPart[] bodyParts;  // Inspector���� ����, ��ư, ��������Ʈ �� ���

    [Header("File Window ����")]
    public FileWindow fileWindow; // ���� �̻� ���� ������ �������� ���� ����

    void Start()
    {
        // �� ��ư Ŭ�� �̺�Ʈ ���
        foreach (var part in bodyParts)
        {
            string partName = part.partName; // ���� ���� ���� (���� ĸó ����)
            if (part.button != null)
            {
                part.button.onClick.AddListener(() => ShowPart(partName));
            }
        }
    }

    /// <summary>
    /// ��ư Ŭ�� �� �ش� ������ ��������Ʈ ǥ��
    /// </summary>
    public void ShowPart(string partName)
    {
        if (cameraPanel == null)
        {
            Debug.LogWarning("cameraPanel�� �������� �ʾҽ��ϴ�.");
            return;
        }

        if (fileWindow == null)
        {
            Debug.LogWarning("fileWindow�� ������� �ʾҽ��ϴ�.");
            return;
        }

        // FileWindow ������ ���� �������� �ش� ���� ���� ã��
        Folder targetFolder = FindFolderByName(fileWindow.GetRootFolder(), partName);

        if (targetFolder == null)
        {
            Debug.LogWarning("�ش� ���� ������ ã�� �� �����ϴ�: " + partName);
            return;
        }

        // BodyPartViewer�� bodyParts ��Ͽ��� �����Ǵ� BodyPart ã��
        foreach (var part in bodyParts)
        {
            if (part.partName == partName)
            {
                // Folder.cs�� ��ư ���� ���ǰ� ������ ���� ����
                bool shouldShowAbnormal = targetFolder.isAbnormal || HasAbnormalInChildren(targetFolder);

                if (shouldShowAbnormal && part.abnormalSprite != null)
                {
                    cameraPanel.sprite = part.abnormalSprite;
                }
                else
                {
                    cameraPanel.sprite = part.normalSprite;
                }
                return;
            }
        }

        Debug.LogWarning("Unknown part: " + partName);
    }

    /// <summary>
    /// ���� �̸����� Folder �˻� (���)
    /// </summary>
    private Folder FindFolderByName(Folder folder, string name)
    {
        if (folder == null) return null;
        if (folder.name == name) return folder;

        foreach (var child in folder.children)
        {
            Folder found = FindFolderByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    /// <summary>
    /// Folder.cs�� HasAbnormalInChildren�� ������ �������� �ڽ� �̻� ���� ���� ���� Ȯ��
    /// </summary>
    private bool HasAbnormalInChildren(Folder folder)
    {
        foreach (var child in folder.children)
        {
            if (child.isAbnormal || HasAbnormalInChildren(child))
                return true;
        }
        return false;
    }
}
