using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFood", menuName = "ScriptableObjects/Items/Food")]
public class FoodSO : ItemSO
{
    [Header("Food Stats")]
    public int hungerRestore;

    [Header("Tutorial Info")]
    public List<TutorialStep> tutorialSteps;

    private GameObject itemModel;

    public override void SpawnItem()
    {
        if (itemModel)
        {
            TakeOut();
            return;
        }

        Transform parentTransform = PlayerController.Instance.handPosition.transform;
        GameObject model = Resources.Load<GameObject>($"_Models/Foods/{itemName}");
        
        itemModel = Instantiate(model, parentTransform.position, model.transform.rotation);
        itemModel.transform.SetParent(parentTransform);

        TutorialManager.Instance.ShowTutorial(tutorialSteps);
    }

    public void UpdatePlayerIndexs()
    {
        PlayerHealth.Instance.UpdateHunger(hungerRestore);
    }

    public override void DespawnItem()
    {
        itemModel.GetComponent<IEquipment>().Despawn();
    }

    public override void TakeOut()
    {
        itemModel.SetActive(true);

        TutorialManager.Instance.ShowTutorial(tutorialSteps);
    }

    public override void PutAway()
    {
        if (itemModel)
            itemModel.SetActive(false);

        TutorialManager.Instance.HideAllTutorials();
    }
}