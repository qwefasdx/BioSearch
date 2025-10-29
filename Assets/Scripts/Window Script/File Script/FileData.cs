using UnityEngine;

[System.Serializable]
public class FileData
{
    // ���� �̸� (Ȯ���ڸ� ������ ���� �̸�)
    public string fileName;

    // ���� Ȯ���� (��: "txt", "png" ��)
    public string extension;

    // �ؽ�Ʈ ������ ���� (TextArea�� Inspector���� ���� �� �Է� ����)
    [TextArea]
    public string textContent;

    // �̹��� ������ ���� (Sprite�� ����)
    public Sprite imageContent;

    // �̻� ���� (true�� ��� ������ ���Ϸ� ǥ�õ�)
    public bool isAbnormal = false;

    [Header("�θ� ���� �̸� (Root ����)")]
    // �θ� ���� �̸� (���� Folder ��ü ��� ���ڿ��� ����)
    // Inspector���� ���� ���ڿ� �Է� ����
    public string parentFolderName;
}
