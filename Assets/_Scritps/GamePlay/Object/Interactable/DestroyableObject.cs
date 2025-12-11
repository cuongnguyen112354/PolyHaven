// using System.Collections.Generic;
// using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using JoaoMilone.Pooler.Controller;
using UnityEngine;

public class DestroyableObject : MonoBehaviour, ISubInteractable
{
    [SerializeField] private PropSO itemData;

    public void HowInteract()
    {
        TutorialManager.Instance.ShowTutorial(itemData.subInteractSteps);
    }

    public async UniTask Destroy()
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

        // if (gameObject.name == "Chest")
        // {
        //     SpwandItems().Forget();
        // }
        // else
        // {
            ConstructionManager.Instance.RemovePlacedObject(itemData.itemName, gameObject);            
            // Destroy(gameObject);
        // }
    }

    // public UniTaskVoid SpwandItems()
    // {
    //     Chest chestCpnt = gameObject.GetComponent<Chest>();
    //     Dictionary<string, List<int>> DicName = chestCpnt.GetDicName();

    //     foreach (string key in DicName.Keys)
    //     {
    //         chestCpnt.ScanSlotsToGetQuantity(DicName[key]);

    //     }

    //     ConstructionManager.Instance.RemovePlacedObject(itemData.itemName, gameObject);

    //     AudioManager.Instance.PlayAudioClip("pick_up");
    //     Destroy(gameObject);
    // }
}
