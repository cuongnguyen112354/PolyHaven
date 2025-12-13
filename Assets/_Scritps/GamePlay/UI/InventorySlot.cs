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

        if (!basic)
        {
            bool isMerge = false;

            Storage pickStorage = StorageCodeMap.GetComponentByCode(pickSlot.storageCode);
            Storage dropStorage = StorageCodeMap.GetComponentByCode(storageCode);

            (ItemSO, int) slotPickInfor = pickStorage.GetSlotIndexInfor(pickSlot.slotIndex);
            (ItemSO, int) slotDropInfor = dropStorage.GetSlotIndexInfor(slotIndex);

            if (slotPickInfor.Item1.itemName == slotDropInfor.Item1.itemName)
                isMerge = true;

            if (storageCode == pickSlot.storageCode)
            {             
                if (isMerge)
                    StorageHandle.Instance.MergeSlotItemSameStorage(storageCode, pickSlot.slotIndex, slotIndex);
                else
                    StorageHandle.Instance.SwapSlotItemSameStorage(storageCode, pickSlot.slotIndex, slotIndex, basic);
            }
            else
                if (isMerge)
                    StorageHandle.Instance.MergeSlotItemDifferentStorage(pickSlot.storageCode, storageCode, pickSlot.slotIndex, slotIndex);
                else
                    StorageHandle.Instance.SwapSlotItemDifferentStorageNoneBasic(pickSlot.storageCode, storageCode, pickSlot.slotIndex, slotIndex);
        }
        else
            StorageHandle.Instance.SwapSlotItemDifferentStorageBasic(pickSlot.storageCode, storageCode, pickSlot.slotIndex, slotIndex);
    }
}
