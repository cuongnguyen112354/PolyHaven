using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemData;

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
        return (itemData.targetIcon, itemData.textTutorial);
    }

    public void Affected(int damage)
    {
        int itemQty = RandomQty();

        // Logic for picking up the item
        if (InventoryManager.Instance.IsAddItem(itemData, itemQty))
        {
            UIManager.Instance.ShowPickupNotify(itemQty, itemData.itemName);
            AudioManager.Instance.PlayAudioClip("pick_up");

            if (gameObject.GetComponent<Rigidbody>())
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
    }
}
