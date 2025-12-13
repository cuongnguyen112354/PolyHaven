using UnityEngine;

public class StorageHandle : MonoBehaviour
{
    public static StorageHandle Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void MergeSlotItemSameStorage(string storageCode, int pickSlotIndex, int dropSlotIndex)
    {
        Storage storage = StorageCodeMap.GetComponentByCode(storageCode);

        // Lấy thông vật phẩm tại 2 ô đồ đã (ItemSO what, int quantity)
        (ItemSO, int) slotPickInfor = storage.GetSlotIndexInfor(pickSlotIndex);

        // Xóa thông tin pick slot
        storage.RemoveSlotItem(pickSlotIndex);

        // Cộng ô pick vào ô drop
        storage.AddItemIntoSlot(dropSlotIndex, slotPickInfor.Item1, slotPickInfor.Item2);
    }

    public void MergeSlotItemDifferentStorage(string pickStorageCode, string dropStorageCode, int pickSlotIndex, int dropSlotIndex)
    {
        Storage pickStorage = StorageCodeMap.GetComponentByCode(pickStorageCode);
        Storage dropStorage = StorageCodeMap.GetComponentByCode(dropStorageCode);
        
        (ItemSO, int) slotPickInfor = pickStorage.GetSlotIndexInfor(pickSlotIndex);

        pickStorage.RemoveSlotItem(pickSlotIndex);

        dropStorage.AddItemIntoSlot(dropSlotIndex, slotPickInfor.Item1, slotPickInfor.Item2);
    }

    public void SwapSlotItemSameStorage(string storageCode, int pickSlotIndex, int dropSlotIndex, bool isBasic)
    {
        Storage storage = StorageCodeMap.GetComponentByCode(storageCode);

        storage.SwapSlotItem(pickSlotIndex, dropSlotIndex, isBasic);
    }

    public void SwapSlotItemDifferentStorageBasic(string pickStorageCode, string dropStorageCode, int pickSlotIndex, int dropSlotIndex)
    {
        Storage pickStorage = StorageCodeMap.GetComponentByCode(pickStorageCode);
        Storage dropStorage = StorageCodeMap.GetComponentByCode(dropStorageCode);

        (ItemSO, int) slotPickInfor = pickStorage.GetSlotIndexInfor(pickSlotIndex);

        pickStorage.TrashSlotItem(pickSlotIndex);
        dropStorage.AddItemIntoSlot(dropSlotIndex, slotPickInfor.Item1, slotPickInfor.Item2);
    }

    public void SwapSlotItemDifferentStorageNoneBasic(string pickStorageCode, string dropStorageCode, int pickSlotIndex, int dropSlotIndex)
    {
        Storage pickStorage = StorageCodeMap.GetComponentByCode(pickStorageCode);
        Storage dropStorage = StorageCodeMap.GetComponentByCode(dropStorageCode);

        (ItemSO, int) slotPickInfor = pickStorage.GetSlotIndexInfor(pickSlotIndex);
        (ItemSO, int) slotDropInfor = dropStorage.GetSlotIndexInfor(dropSlotIndex);

        pickStorage.TrashSlotItem(pickSlotIndex);
        pickStorage.AddItemIntoSlot(pickSlotIndex, slotDropInfor.Item1, slotDropInfor.Item2);

        dropStorage.TrashSlotItem(dropSlotIndex);
        dropStorage.AddItemIntoSlot(dropSlotIndex, slotPickInfor.Item1, slotPickInfor.Item2);
    }
}
