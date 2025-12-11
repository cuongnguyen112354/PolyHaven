using UnityEngine;
using UnityEngine.EventSystems;

public class Trash : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        InventorySlot pickSlot = draggableItem.parentAfterDrag.GetComponent<InventorySlot>();
        Storage dropStorage = StorageCodeMap.GetComponentByCode(pickSlot.storageCode);
        dropStorage.TrashSlotItem(pickSlot.slotIndex);

        Destroy(dropped);
        PlayerController.Instance.onDragging = false;
    }
}
