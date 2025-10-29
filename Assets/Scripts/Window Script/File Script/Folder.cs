using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ���� ������ ����.
/// �ڽ� ���� ����, �̻� ���� ǥ��.
/// </summary>
[System.Serializable]
public class Folder
{
    public string name;
    public List<Folder> children = new List<Folder>();
    public List<File> files = new List<File>();   // ���� ����Ʈ �߰�
    public Folder parent;

    // �̻� ���� ����
    public bool isAbnormal = false;

    // �̻� ���� Ž���� �Ķ���� (0~1 ����, �ܺο��� ���� ����)
    public float abnormalParameter = 0f;

    // ����� ��ü ��ư
    public Button linkedBodyButton;

    public Folder(string name, Folder parent = null, Button linkedButton = null)
    {
        this.name = name;
        this.parent = parent;
        this.linkedBodyButton = linkedButton;
    }

    // Ȯ���� ���� �̻� ���� ���� (�ڽ� ���� ����)
    public void AssignAbnormalByParameter()
    {
        isAbnormal = Random.value < abnormalParameter;

        // �ڽ� ������ ��������� �̻� ���� ����
        foreach (var child in children)
        {
            child.AssignAbnormalByParameter();
        }

        // ��ư ���� ���� (�ڽŰ� �θ����)
        UpdateLinkedButtonColor();
        UpdateParentButtonColor();
    }
    // ����� ��ư ���� ������Ʈ (�ڽ� �������� ����)
    public void UpdateLinkedButtonColor()
    {
        if (linkedBodyButton != null)
        {
            var colors = linkedBodyButton.colors;

            // �ڽ� �Ǵ� �ڽ� �� �ϳ��� �̻��̸� ������
            bool shouldBeRed = isAbnormal || HasAbnormalInChildren();
            Color targetColor = shouldBeRed ? Color.red : Color.white;

            colors.normalColor = targetColor;
            colors.highlightedColor = targetColor;
            colors.pressedColor = targetColor;
            colors.selectedColor = targetColor;

            linkedBodyButton.colors = colors;
        }
    }

    // �θ� ��ư ���� ����
    private void UpdateParentButtonColor()
    {
        parent?.UpdateLinkedButtonColor();
        parent?.UpdateParentButtonColor();
    }

    // ��������� �ڽ� ���� �̻� ���� Ȯ��
    private bool HasAbnormalInChildren()
    {
        foreach (var child in children)
        {
            if (child.isAbnormal || child.HasAbnormalInChildren())
                return true;
        }
        return false;
    }


    // �ڽ� �߰�
    public void AddChild(Folder child)
    {
        if (!children.Contains(child))
        {
            children.Add(child);
            child.parent = this;
        }
    }

    // �ڽ� ����
    public void RemoveChild(Folder child)
    {
        if (children.Contains(child))
        {
            children.Remove(child);
            child.parent = null;
        }
    }

    // ���� �߰�
    public void AddFile(File file)
    {
        if (!files.Contains(file))
        {
            files.Add(file);
            file.parent = this;
        }
    }

    // ���� ����
    public void RemoveFile(File file)
    {
        if (files.Contains(file))
        {
            files.Remove(file);
            file.parent = null;
        }
    }
}
