using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PathPanelManager : MonoBehaviour
{
    public Transform contentArea;
    public Button pathButtonPrefab;

    private FileWindow fileWindow;
    private List<Button> pathButtons = new List<Button>();

    public void Initialize(FileWindow window)
    {
        fileWindow = window;
    }

    public void UpdatePathButtons()
    {
        foreach (var btn in pathButtons)
            Destroy(btn.gameObject);
        pathButtons.Clear();

        List<Folder> pathList = fileWindow.GetCurrentPathList();
        for (int i = 0; i < pathList.Count; i++)
        {
            int index = i;
            Button btn = Instantiate(pathButtonPrefab, contentArea);
            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            text.text = pathList[i].name;

            float width = text.preferredWidth + 20f;
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(width, btn.GetComponent<RectTransform>().sizeDelta.y);

            btn.onClick.AddListener(() =>
            {
                fileWindow.NavigateToPathIndex(index);
            });

            EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.Drop };
            entry.callback.AddListener((data) =>
            {
                OnPathDrop(index);
            });
            trigger.triggers.Add(entry);

            pathButtons.Add(btn);
        }
    }

    private void OnPathDrop(int index)
    {
        FileIcon dragged = FileDragManager.Instance.GetDraggingIcon();
        if (dragged == null) return;

        Folder target = fileWindow.GetCurrentPathList()[index];
        Folder source = dragged.GetFolder();
        if (source == null || target == null) return;

        if (source.parent != null)
            source.parent.children.Remove(source);

        target.children.Add(source);
        source.parent = target;

        fileWindow.OpenFolder(fileWindow.GetCurrentPathList()[^1], false);
    }
}
