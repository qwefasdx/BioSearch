using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실제 폴더 데이터 구조.
/// 자식 폴더 보관, 이상 여부 표시.
/// </summary>
[System.Serializable]
public class Folder
{
    public string name;
    public List<Folder> children = new List<Folder>();
    public List<File> files = new List<File>();   // 파일 리스트 추가
    public Folder parent;

    // 이상 폴더 여부
    public bool isAbnormal = false;

    // 이상 폴더 탐색용 파라미터 (0~1 범위, 외부에서 설정 가능)
    public float abnormalParameter = 0f;

    public Folder(string name, Folder parent = null)
    {
        this.name = name;
        this.parent = parent;
    }

    // 편의 함수: 이상 폴더 여부를 파라미터 기반으로 랜덤 결정
    public void AssignAbnormalByParameter()
    {
        isAbnormal = Random.value < abnormalParameter;
    }

    // 자식 추가
    public void AddChild(Folder child)
    {
        if (!children.Contains(child))
        {
            children.Add(child);
            child.parent = this;
        }
    }

    // 자식 제거
    public void RemoveChild(Folder child)
    {
        if (children.Contains(child))
        {
            children.Remove(child);
            child.parent = null;
        }
    }

    // 파일 추가
    public void AddFile(File file)
    {
        if (!files.Contains(file))
        {
            files.Add(file);
            file.parent = this;
        }
    }

    // 파일 제거
    public void RemoveFile(File file)
    {
        if (files.Contains(file))
        {
            files.Remove(file);
            file.parent = null;
        }
    }
}
