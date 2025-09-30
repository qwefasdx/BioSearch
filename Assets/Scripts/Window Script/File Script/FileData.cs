using UnityEngine;

[System.Serializable]
public class FileData
{
    public string fileName;
    public string extension;
    [TextArea] public string textContent;
    public Sprite imageContent;

    [Header("부모 폴더 이름 (Root 기준)")]
    public string parentFolderName; // Inspector에서 문자열로 입력
}
