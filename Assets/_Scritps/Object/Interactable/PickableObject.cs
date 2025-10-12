using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemData;

    [SerializeField] private Sprite pickableTargetIcon;
    [SerializeField] private string textTutorial;


    // Randomly determine the quantity of items to pick up based on defined probabilities
    private int RandomQty()
    {
        int result = Random.Range(0, 100);
        if (result < 75)
            return 1;
        else if (result >= 75 && result < 95)
            return 2;
        else
            return 3;
    }

    public string GetItemName()
    {
        return itemData.itemName;
    }

    public (Sprite, string) HowInteract()
    {
        return (pickableTargetIcon, textTutorial);
    }

    public void Interact()
    {
        int itemQty = RandomQty();

        // Logic for picking up the item
        if (InventoryManager.Instance.IsAddItem(itemData, itemQty))
        {
            UIManager.Instance.ShowPickupNotify(itemQty, itemData.itemName);
            AudioManager.Instance.PlayAudioClip("pick_up");

            if (itemData.itemName == "Wood")
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
    }
}
