using UnityEngine;

[System.Serializable]
public class File
{
    // ���� �̸� (Ȯ���ڸ� ������ �̸�)
    public string name;

    // ���� Ȯ���� (��: "txt", "png" ��)
    public string extension;

    // �ؽ�Ʈ ������ ���� (�ؽ�Ʈ ������ ��� ���)
    public string textContent;

    // �̹��� ������ ���� (�̹��� ������ ��� ���)
    public Sprite imageContent;

    // ���� ������ ���� (������ ���Ե� ����)
    public Folder parent;

    //  �ν����Ϳ��� ���� ������ �� �ִ� �̻� ����
    //  true�� ���, ������ ���Ϸ� ǥ�õ�
    public bool isAbnormal = false;

    // ���� ������
    // name : ���� �̸�
    // extension : Ȯ����
    // parent : �θ� ����
    // textContent : �ؽ�Ʈ ���� ����
    // imageContent : �̹��� ���� ����
    // isAbnormal : �̻� ���� (�⺻�� false)
    public File(string name, string extension, Folder parent = null, string textContent = null, Sprite imageContent = null, bool isAbnormal = false)
    {
        this.name = name;
        this.extension = extension;
        this.parent = parent;
        this.textContent = textContent;
        this.imageContent = imageContent;
        this.isAbnormal = isAbnormal;
    }
}
