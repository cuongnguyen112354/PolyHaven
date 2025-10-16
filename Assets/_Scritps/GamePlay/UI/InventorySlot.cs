using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public int slotIndex;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private TMP_Text quantityText;

    public void UpdateUI(int quantity)
    {
        if (quantity == 0)
            quantityText.gameObject.SetActive(false);
        else
            quantityText.gameObject.SetActive(true);

        UpdateQuantityUI(quantity);
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

        int firstSlot = draggableItem.parentAfterDrag.GetComponent<InventorySlot>().slotIndex;
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

        InventoryManager.Instance.SwapSlotItem(firstSlot, slotIndex, basic);
    }
}
