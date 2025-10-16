using UnityEngine;
using UnityEngine.EventSystems;

public class Trash : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        int slotIndex = draggableItem.parentAfterDrag.GetComponent<InventorySlot>().slotIndex;
        InventoryManager.Instance.TrashSlotItem(slotIndex);

        Destroy(dropped);
        InventoryManager.Instance.onDragging = false;
    }
}
