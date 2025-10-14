using UnityEngine;

public class RetrievableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private CraftableSO itemData;

    public string GetItemName()
    {
        return gameObject.name;
    }

    public (Sprite, string) HowInteract()
    {
        return (itemData.targetIcon, itemData.textTutorial);
    }

    public void Affected(int damage)
    {
        if (InventoryManager.Instance.IsAddItem(itemData))
        {
            ConstructionManager.Instance.RemovePlacedObject(itemData.itemName, gameObject);

            AudioManager.Instance.PlayAudioClip("pick_up");
            Destroy(gameObject);
        }
    }
}
