using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemData;

    [SerializeField] private Sprite pickableTargetIcon;
    [SerializeField] private string textTutorial = "Press [E]";

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
        // Logic for picking up the item
        if (InventoryManager.Instance.IsAddItem(itemData))
        {
            GameObject floatingTextPrefab = Resources.Load<GameObject>("_Prefabs/FloatingText");
            Vector3 spawnPosition = new(transform.position.x, transform.position.y + .5f, transform.position.z);

            GameObject ft = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            ft.GetComponent<FloatingTextWorld>().SetText($"+1 {itemData.itemName}");

            AudioManager.Instance.PlayAudioClip("pick_up");
            Destroy(gameObject);
        }
    }
}
