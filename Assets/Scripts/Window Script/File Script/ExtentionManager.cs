using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� Ȯ���ڿ� ���� ������ ��������Ʈ�� �����ϴ� �Ŵ���
/// </summary>
public class ExtensionManager : MonoBehaviour
{
    public static ExtensionManager Instance;

    [Header("������ ��������Ʈ")]
    public Sprite txtIconSprite;
    public Sprite pngIconSprite;
    public Sprite defaultIconSprite; // ���ǵ��� ���� Ȯ���ڿ� �⺻ ������

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Ȯ���ڿ� �´� ������ ��������Ʈ�� ��ȯ
    /// </summary>
    public Sprite GetIconForExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            return defaultIconSprite;

        switch (extension.ToLower())
        {
            case "txt":
                return txtIconSprite;
            case "png":
                return pngIconSprite;
            default:
                return defaultIconSprite;
        }
    }
}
