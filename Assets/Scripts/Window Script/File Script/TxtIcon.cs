using UnityEngine;

/// <summary>
/// TXT 파일 아이콘 (텍스트 파일 열기 등 처리)
/// </summary>
public class TxtIcon : FileIcon
{
    protected override void OnDoubleClick()
    {
        LogWindowManager.Instance.Log($"TXT 파일 열기: {file.name}.{file.extension}");
        // TODO: 텍스트 뷰어 열기 기능 연결
    }
}
