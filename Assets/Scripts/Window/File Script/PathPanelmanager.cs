using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        if (fileWindow == null)
        {
            Debug.LogWarning("PathPanelManager: fileWindow가 아직 할당되지 않았습니다.");
            return;
        }

        foreach (var btn in pathButtons)
            Destroy(btn.gameObject);
        pathButtons.Clear();

        List<Folder> pathList = fileWindow.GetCurrentPathList();
        for (int i = 0; i < pathList.Count; i++)
        {
            int index = i;
            Button btn = Instantiate(pathButtonPrefab, contentArea);
            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            if (text == null)
            {
                Debug.LogError("PathButton Prefab에 TMP_Text가 없습니다.");
                continue;
            }
            text.text = pathList[i].name;

            // Width 자동 조절
            float width = text.preferredWidth + 20f;
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(width, btn.GetComponent<RectTransform>().sizeDelta.y);

            btn.onClick.AddListener(() =>
            {
                fileWindow.NavigateToPathIndex(index);
            });

            pathButtons.Add(btn);
        }
    }
}
