using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public int slotIndex;
    [HideInInspector] public string storageCode;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private TMP_Text quantityText;

    public void UpdateUI(int quantity)
    {
        if (quantity == 0)
            quantityText.gameObject.SetActive(false);
        else
        {
            quantityText.gameObject.SetActive(true);
            UpdateQuantityUI(quantity);
        }
    }

    public void UpdateQuantityUI(int quantity)
    {
        quantityText.text = $"{quantity}";
    }

    public void RemoveItem()
    {
        quantityText.gameObject.SetActive(false);
        Destroy(transform.GetChild(0).gameObject);
    }

    public void AddItem(ItemSO itemData, int amount = 1)
    {
        GameObject itemObject = Instantiate(itemPrefab);

        itemObject.transform.SetParent(transform);
        itemObject.transform.SetAsFirstSibling();

        itemObject.transform.localScale = Vector3.one;
        itemObject.GetComponent<DraggableItem>().Init(itemData.icon);

        quantityText.gameObject.SetActive(true);
        UpdateQuantityUI(amount);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        InventorySlot pickSlot = draggableItem.parentAfterDrag.GetComponent<InventorySlot>();
        bool basic = true;

        if (transform.childCount != 1)
        {
            basic = false;
            DraggableItem existingItem = transform.GetComponentInChildren<DraggableItem>();
            existingItem.parentAfterDrag = draggableItem.parentAfterDrag;
            existingItem.transform.SetParent(draggableItem.parentAfterDrag);
            existingItem.transform.SetAsFirstSibling();
        }

        draggableItem.parentAfterDrag = transform;

        Storage dropStorage = StorageCodeMap.GetComponentByCode(storageCode);
        // Trường hợp cùng một Storage
        if (storageCode == pickSlot.storageCode)
        {
            dropStorage.SwapSlotItem(pickSlot.slotIndex, slotIndex, basic);
        }
        // Trường hợp khác Storage
        else if (basic)
        {
            Storage pickStorage = StorageCodeMap.GetComponentByCode(pickSlot.storageCode);
            (ItemSO, int) slotDropInfor = pickStorage.GetSlotIndexInfor(pickSlot.slotIndex);

            pickStorage.TrashSlotItem(pickSlot.slotIndex);
            dropStorage.AddItemIntoSlot(slotIndex, slotDropInfor.Item1, slotDropInfor.Item2);
        }
        else
        {
            Storage pickStorage = StorageCodeMap.GetComponentByCode(pickSlot.storageCode);

            (ItemSO, int) slotPickInfor = dropStorage.GetSlotIndexInfor(slotIndex);
            (ItemSO, int) slotDropInfor = pickStorage.GetSlotIndexInfor(pickSlot.slotIndex);

            pickStorage.TrashSlotItem(pickSlot.slotIndex);
            pickStorage.AddItemIntoSlot(pickSlot.slotIndex, slotPickInfor.Item1, slotPickInfor.Item2);

            dropStorage.TrashSlotItem(slotIndex);
            dropStorage.AddItemIntoSlot(slotIndex, slotDropInfor.Item1, slotDropInfor.Item2);
        }
    }
}
