using System.Collections.Generic;

/// <summary>
/// FileWindow�� ��� �̵� �� �׺���̼� ���� �޼��� ����.
/// </summary>
public partial class FileWindow
{
    /// <summary>
    /// ���� ���������� ��θ� ����Ʈ�� ��ȯ
    /// </summary>
    public List<Folder> GetCurrentPathList()
    {
        List<Folder> pathList = new List<Folder>();
        Folder temp = currentFolder;
        while (temp != null)
        {
            pathList.Insert(0, temp);
            temp = temp.parent;
        }
        return pathList;
    }

    /// <summary>
    /// ��� �гο��� Ư�� �ε����� �̵�
    /// </summary>
    public void NavigateToPathIndex(int index)
    {
        var pathList = GetCurrentPathList();
        if (index < 0 || index >= pathList.Count) return;
        OpenFolder(pathList[index], false);
    }

    /// <summary>
    /// �ڷΰ��� ��ư Ŭ�� �� ����
    /// </summary>
    private void OnBackButtonClicked()
    {
        if (folderHistory.Count == 0) return;
        Folder previous = folderHistory.Pop();
        OpenFolder(previous, false);
    }

    /// <summary>
    /// Ư�� ���� ����
    /// </summary>
    public void RefreshFolder(Folder folder)
    {
        OpenFolder(folder, false);
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void RefreshWindow()
    {
        if (currentFolder != null)
            OpenFolder(currentFolder, false);
    }

    /// <summary>
    /// ���� �̸����� ���� �˻�
    /// </summary>
    private Folder FindFolderByName(Folder folder, string name)
    {
        if (folder.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            return folder;

        foreach (var child in folder.children)
        {
            var found = FindFolderByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
}
