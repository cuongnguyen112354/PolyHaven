using Cysharp.Threading.Tasks;
using JoaoMilone.Pooler.Controller;
using UnityEngine;

public class DestroyableObject : MonoBehaviour, ISubInteractable
{
    [SerializeField] private PropSO itemData;

    [SerializeField] private string destroyDescription =
        "- Are you sure you want to dismantle this chest?\n" +
        "- 50% of the materials will be refunded.";

    private async UniTaskVoid ConfirmDestroy()
    {
        gameObject.SetActive(false);

        foreach (Ingredient ingredient in itemData.ingredients)
        {
            await UniTask.Delay(100);

            // Hoàn lại 50% số nguyên liệu
            GameObject obj = ObjectPooler.ME.RequestObject(ingredient.item.itemName, transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
            obj.GetComponent<PickableObject>().quantity = Mathf.RoundToInt(ingredient.quantity * 0.5f);
            
            if (!obj.GetComponent<Rigidbody>())
                obj.AddComponent<Rigidbody>().linearDamping = 5f;

            AudioManager.Instance.PlayAudioClip("pick_up");
        }

        ConstructionManager.Instance.RemovePlacedObject(itemData.itemName, gameObject);

        if (gameObject.TryGetComponent<Chest>(out var chest))
            StorageCodeMap.codeMap.Remove(chest.storageCode);
        
        await UniTask.WaitForEndOfFrame();
        TutorialManager.Instance.HideAllTutorials();
    }

    public void HowInteract()
    {
        TutorialManager.Instance.ShowTutorial(itemData.subInteractSteps);
    }

    public void DestroyRequest()
    {
        UIManager.Instance.ShowConfirmPopup(destroyDescription, ConfirmDestroy);
    }
}
