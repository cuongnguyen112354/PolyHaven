using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [HideInInspector] public bool onDragging = false;

    [SerializeField] private InventorySlot[] inventorySlots;

    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private Color noneSelectedColor;
    [SerializeField] private Color selectedColor;

    private readonly Dictionary<string, List<int>> DicName = new();
    private readonly Dictionary<int, Slot> DicSlot = new();

    private (int, int) hotbarSlot = (21, 26);

    private int slotSelected;
    private int MAX_SLOT_NUM;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MAX_SLOT_NUM = inventorySlots.Length;

        for (int i = 0; i < MAX_SLOT_NUM; i++)
        {
            inventorySlots[i].slotIndex = i;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (KeyValuePair<string, List<int>> entry in DicName)
            {
                foreach (int index in entry.Value)
                {
                    Debug.Log($"Slot thứ {index}, vật phẩm: {entry.Key}, số lượng: {DicSlot[index].quantity} cái");
                }
            }
        }

        if (onDragging)
        {
            CursorManager.ChangeCursorIcon("drag", true);
            return;
        }

        if (IsPointerOverUIWithComponent<DraggableItem>())
            CursorManager.ChangeCursorIcon("none_drag");
        else
            CursorManager.DefaultCursorIcon();
    }

    bool IsPointerOverUIWithComponent<T>() where T : Component
    {
        PointerEventData pointerData = new(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<T>() != null)
            {
                return true;
            }
        }
        return false;
    }

    private void RemoveItemInSlot(int slotIndex, int quantity)
    {
        DicSlot[slotIndex].quantity -= quantity;
        inventorySlots[slotIndex].UpdateQuantityUI(DicSlot[slotIndex].quantity);
    }

    private void RemoveSlotItem(int slotIndex)
    {
        string itemName = DicSlot[slotIndex].item.itemName;

        if (slotIndex == slotSelected)
        {
            DicSlot[slotSelected].DespawnItem();
            TutorialManager.Instance.HideAllTutorials();
        }
        DicSlot.Remove(slotIndex);

        DicName[itemName].Remove(slotIndex);
        inventorySlots[slotIndex].RemoveItem();
    }

    // Nếu add thành công thì sẽ trả về (tín hiệu, slotIndex, số còn dư)
    // tín hiệu -1: Add thất bại
    // tín hiệu 0: Không còn dư
    // tín hiệu > 0: Số còn dư
    private (int, int) ScanSlotToAdd(ItemSO item, int quantity = 1)
    {
        for (int i = 0; i < MAX_SLOT_NUM && quantity > 0; i++)
        {
            if (!DicSlot.ContainsKey(i))
            {
                if (item is ToolSO equipment)
                {
                    DicSlot[i] = new Slot(equipment, quantity);

                    quantity -= quantity;
                }
                else
                {
                    int toAdd = Mathf.Min(item.maxStackSize, quantity);
                    DicSlot[i] = new Slot(item, toAdd);

                    quantity -= toAdd;
                }

                inventorySlots[i].AddItem(item, DicSlot[i].quantity);

                // Khởi tạo model trong hand nếu trùng với slot mà người chơi đang chọn
                if (i == slotSelected)
                    DicSlot[slotSelected].SpawnItem();

                return (quantity, i);
            }
        }

        return (-1, 0);
    }

    private void RemoveItemFromSlots(List<int> slotIndexs, int quantity)
    {
        int remaining = quantity;

        for (int i = slotIndexs.Count - 1; i >= 0; i--)
        {
            if (DicSlot[slotIndexs[i]].quantity > remaining)
            {
                RemoveItemInSlot(slotIndexs[i], remaining);
                return;
            }

            if (DicSlot[slotIndexs[i]].quantity < remaining)
            {
                remaining -= DicSlot[slotIndexs[i]].quantity;
            }

            RemoveSlotItem(slotIndexs[i]);
        }
    }

    private int ScanSlotsToGetQuantity(List<int> slots)
    {
        int quantity = 0;

        foreach (int slot in slots)
        {
            quantity += DicSlot[slot].quantity;
        }

        return quantity;
    }

    public void TrashSlotItem(int slotIndex)
    {
        string itemName = DicSlot[slotIndex].item.itemName;

        if (slotIndex == slotSelected)
        {
            DicSlot[slotSelected].DespawnItem();
            TutorialManager.Instance.HideAllTutorials();
        }
        DicSlot.Remove(slotIndex);

        DicName[itemName].Remove(slotIndex);
    }

    public bool IsAddItem(ItemSO itemData, int quantity = 1)
    {
        if (DicName.TryGetValue(itemData.itemName, out List<int> indices))
        {
            foreach (int index in indices)
            {
                Slot slot = DicSlot[index];
                int toAdd = Mathf.Min(slot.SpaceLeft, quantity);
                slot.quantity += toAdd;
                quantity -= toAdd;

                inventorySlots[index].UpdateQuantityUI(DicSlot[index].quantity);

                if (quantity <= 0)
                    return true;
            }
        }

        (int result, int slotIndex) = ScanSlotToAdd(itemData, quantity);

        if (result == -1)
        {
            Debug.Log("Inventory is full!");
            return false;
        }
        else
        {
            while (result != -1)
            {
                if (DicName.TryGetValue(itemData.itemName, out List<int> indexs))
                    indexs.Add(slotIndex);
                else
                    DicName[itemData.itemName] = new List<int> { slotIndex };

                if (result == 0)
                    return true;

                (result, slotIndex) = ScanSlotToAdd(itemData, quantity);
            }
        }

        return false;
    }

    public (bool, int) HasItem(Ingredient ingredient)
    {
        if (DicName.TryGetValue(ingredient.item.itemName, out List<int> slots))
        {
            int quantity = ScanSlotsToGetQuantity(slots);
            if (quantity < ingredient.quantity)
                return (false, quantity);
            else
                return (true, 0);
        }

        return (false, 0);
    }

    public void RemoveItem(string itemName, int quantity = 1)
    {
        if (DicName.TryGetValue(itemName, out List<int> slotIndexs))
            RemoveItemFromSlots(slotIndexs, quantity);
    }

    public void RemoveItems(List<Ingredient> ingredients)
    {
        foreach (Ingredient ingredient in ingredients)
        {
            if (DicName.TryGetValue(ingredient.item.itemName, out List<int> slotIndexs))
                RemoveItemFromSlots(slotIndexs, ingredient.quantity);
        }
    }

    public void SwapSlotItem(int firstSlot, int lastSlot, bool basic)
    {
        if (firstSlot == lastSlot) return;

        if (firstSlot == slotSelected)
        {
            if (DicSlot.TryGetValue(firstSlot, out Slot slot))
                slot.PutAway();
            if (DicSlot.TryGetValue(lastSlot, out slot))
                slot.SpawnItem();
        }
        else if (lastSlot == slotSelected)
        {
            if (DicSlot.TryGetValue(firstSlot, out Slot slot))
                slot.SpawnItem();
            if (DicSlot.TryGetValue(lastSlot, out slot))
                slot.PutAway();
        }

        if (basic)
        {
            if (DicName.TryGetValue(DicSlot[firstSlot].item.itemName, out List<int> slots))
                slots[slots.IndexOf(firstSlot)] = lastSlot;

            DicSlot[lastSlot] = DicSlot[firstSlot];
            DicSlot.Remove(firstSlot);

            inventorySlots[firstSlot].UpdateUI(0);
            inventorySlots[lastSlot].UpdateUI(DicSlot[lastSlot].quantity);
        }
        else
        {
            if (DicName.TryGetValue(DicSlot[firstSlot].item.itemName, out List<int> fslots))
                fslots[fslots.IndexOf(firstSlot)] = lastSlot;
            if (DicName.TryGetValue(DicSlot[lastSlot].item.itemName, out List<int> lslots))
                lslots[lslots.IndexOf(lastSlot)] = firstSlot;

            Slot slot = DicSlot[firstSlot];
            DicSlot[firstSlot] = DicSlot[lastSlot];
            DicSlot[lastSlot] = slot;

            inventorySlots[firstSlot].UpdateUI(DicSlot[firstSlot].quantity);
            inventorySlots[lastSlot].UpdateUI(DicSlot[lastSlot].quantity);
        }
    }

    public void SelectingSlot(int slotIndex)
    {
        if (slotIndex == slotSelected) return;

        if (slotIndex < hotbarSlot.Item1)
            slotIndex = hotbarSlot.Item2;
        else if (slotIndex > hotbarSlot.Item2)
            slotIndex = hotbarSlot.Item1;

        inventorySlots[slotSelected].GetComponent<Image>().color = noneSelectedColor;
        inventorySlots[slotIndex].GetComponent<Image>().color = selectedColor;

        if (DicSlot.TryGetValue(slotSelected, out Slot slot))
            slot.PutAway();
        if (DicSlot.TryGetValue(slotIndex, out slot))
            slot.SpawnItem();

        slotSelected = slotIndex;
    }

    public void ChangeOneSlot(bool isIncrease)
    {
        if (isIncrease)
            SelectingSlot(slotSelected + 1);
        else
            SelectingSlot(slotSelected - 1);
    }

    public bool CheckSelectingItemQuantity(int quantity = 1)
    {
        if (DicSlot[slotSelected].quantity >= quantity)
            return true;

        return false;
    }

    public void UseSelectingItem(int quantity = 1)
    {
        DicSlot[slotSelected].UpdatePlayerIndexs();

        if (DicSlot[slotSelected].quantity < quantity)
            return;

        if (DicSlot[slotSelected].quantity > quantity)
            RemoveItemInSlot(slotSelected, quantity);
        else
            RemoveSlotItem(slotSelected);
    }

    #region [[ --- Inventory Data --- ]]
    private void AddItemToSlot(int slotIndex, ItemSO item, int quantity)
    {
        if (DicName.TryGetValue(item.itemName, out List<int> indices))
            indices.Add(slotIndex);
        else
            DicName[item.itemName] = new List<int> { slotIndex };

        DicSlot[slotIndex] = new Slot(item, quantity);
        inventorySlots[slotIndex].AddItem(item, quantity);
    }

    public void Init(List<SlotData> slotDatas)
    {
        List<string> itemExist = new();

        foreach (SlotData slotData in slotDatas)
        {
            if (!itemExist.Contains(slotData.itemName))
                itemExist.Add(slotData.itemName);

            ItemSO item = Resources.Load<ItemSO>($"_ItemSO/{slotData.itemName}");

            AddItemToSlot(slotData.slotIndex, item, slotData.quantity);
        }

        slotSelected = hotbarSlot.Item1 + 1;
        SelectingSlot(hotbarSlot.Item1);
    }

    public List<SlotData> GetInventoryData()
    {
        List<SlotData> slotDatas = new();

        foreach (KeyValuePair<string, List<int>> entry in DicName)
        {
            foreach (int index in entry.Value)
            {
                if (DicSlot.TryGetValue(index, out Slot slot))
                {
                    slotDatas.Add(new SlotData
                    {
                        slotIndex = index,
                        itemName = entry.Key,
                        quantity = slot.quantity
                    });
                }
            }
        }

        return slotDatas;
    }
    #endregion
}

public class Slot
{
    public ItemSO item;
    public int quantity;

    public int SpaceLeft => item != null ? item.maxStackSize - quantity : 0;

    public Slot(ItemSO item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void SpawnItem()
    {
        item.SpawnItem();
    }

    public void UpdatePlayerIndexs()
    {
        if (item is FoodSO foodSO)
            foodSO.UpdatePlayerIndexs();
    }

    public void DespawnItem()
    {
        item.DespawnItem();
    }

    public void PutAway()
    {
        item.PutAway();
    }
}

[System.Serializable]
public class SlotData
{
    public int slotIndex;
    public string itemName;
    public int quantity;
}