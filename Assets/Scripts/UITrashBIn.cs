using UnityEngine;
using UnityEngine.EventSystems;

public class UITrashBin : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged != null)
        {
            var item = dragged.GetComponent<UIDraggableItem>();
            if (item != null)
            {
                Debug.Log($"{dragged.name} → 휴지통에 버려짐 ");
                item.DeleteSelf();
            }
        }
    }
}

