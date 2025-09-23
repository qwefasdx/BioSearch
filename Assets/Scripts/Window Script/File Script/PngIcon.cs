using UnityEngine;

/// <summary>
/// PNG 파일 아이콘 (이미지 파일 열기 등 처리)
/// </summary>
public class PngIcon : FileIcon
{
    protected override void OnDoubleClick()
    {
        LogWindowManager.Instance.Log($"PNG 파일 열기: {file.name}.{file.extension}");
        // TODO: 이미지 뷰어 열기 기능 연결
    }
}
