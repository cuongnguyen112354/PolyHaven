using UnityEngine;

public class Recoverable : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite pickableTargetIcon;
    [SerializeField] private string textTutorial = "Press [E]";

    public string GetItemName()
    {
        return gameObject.name;
    }

    public (Sprite, string) HowInteract()
    {
        return (pickableTargetIcon, textTutorial);
    }

    public void Interact()
    {
        ItemSO itemData = Resources.Load<ItemSO>($"_ItemSO/{gameObject.name}");

        if (InventoryManager.Instance.IsAddItem(itemData) & itemData != null)
        {
            GameObject floatingTextPrefab = Resources.Load<GameObject>("_Prefabs/FloatingText");
            Vector3 spawnPosition = new(transform.position.x, transform.position.y + .5f, transform.position.z);

            UIManager.Instance.ShowPickupNotify(1, itemData.itemName);

            ConstructionManager.Instance.RemovePlacedObject(gameObject.name, gameObject);

            AudioManager.Instance.PlayAudioClip("pick_up");
            Destroy(gameObject);
        }
    }
}
