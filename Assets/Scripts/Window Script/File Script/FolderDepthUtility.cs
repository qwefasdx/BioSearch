using System.Collections.Generic;
using UnityEngine;

public static class FolderDepthUtility
{
    // ���� ���� ���� ��� (��Ʈ ����, ��Ʈ�� depth 1)
    public static int GetFolderDepth(Folder folder)
    {
        int depth = 1; // ��Ʈ ����
        Folder temp = folder;
        while (temp.parent != null)
        {
            depth++;
            temp = temp.parent;
        }
        return depth;
    }

    // ���� Ʈ���� �ִ� ���� ��� (�ڱ� �ڽ� ���� �� ��)
    public static int GetSubtreeMaxDepth(Folder folder)
    {
        if (folder.children == null || folder.children.Count == 0)
            return 0;

        int maxDepth = 0;
        foreach (var child in folder.children)
        {
            int childDepth = 1 + GetSubtreeMaxDepth(child);
            if (childDepth > maxDepth)
                maxDepth = childDepth;
        }
        return maxDepth;
    }

    // �̵� �� ���ο� ���̸� ���
    public static bool CanMove(Folder dragged, Folder target, out string warningMessage)
    {
        int targetDepth = GetFolderDepth(target);              // ��� ���� ����
        int subtreeDepth = 1 + GetSubtreeMaxDepth(dragged);    // �巡�׵� ���� ���� ���� ����
        int newDepth = targetDepth + subtreeDepth;

        if (newDepth > 6)
        {
            warningMessage = "Warning. �ִ� ����(6) �ʰ�.";
            return false;
        }

        warningMessage = null;
        return true;
    }
}
