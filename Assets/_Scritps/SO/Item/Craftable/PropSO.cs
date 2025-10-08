using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProp", menuName = "ScriptableObjects/Items/Prop")]
public class PropSO : CraftableSO
{
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

        Transform parentTransform = ConstructionManager.Instance.transform;

        itemModel = Instantiate(Resources.Load<GameObject>($"_Models/Props/{itemName}"),
            parentTransform.position, Quaternion.Euler(parentTransform.eulerAngles));

        itemModel.transform.SetParent(parentTransform);
        itemModel.name = itemName;

        AudioManager.Instance.PlayAudioClip("equip");
        TutorialManager.Instance.ShowTutorial(tutorialSteps);
    }

    public override void DespawnItem()
    { 
        Destroy(itemModel);
    }

    public override void TakeOut()
    {
        itemModel.SetActive(true);
    }

    public override void PutAway()
    {
        if (itemModel)
            itemModel.SetActive(false);
    }
}
