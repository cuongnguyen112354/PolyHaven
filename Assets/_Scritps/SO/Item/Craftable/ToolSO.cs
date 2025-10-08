using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTool", menuName = "ScriptableObjects/Items/Tool")]
public class ToolSO : CraftableSO
{
    [Header("Tool Stats")]
    public int damage;
    public int durability;

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
        GameObject model = Resources.Load<GameObject>($"_Models/Tools/{itemName}");
        
        itemModel = Instantiate(model, parentTransform.position, model.transform.rotation);
        itemModel.transform.SetParent(parentTransform);
        itemModel.name = itemName;

        AudioManager.Instance.PlayAudioClip("equip");
        TutorialManager.Instance.ShowTutorial(tutorialSteps);
    }

    public override void DespawnItem()
    {
        itemModel.GetComponent<IEquipment>().Despawn();
    }

    public override void TakeOut()
    {
        itemModel.SetActive(true);
        AudioManager.Instance.PlayAudioClip("equip");

        TutorialManager.Instance.ShowTutorial(tutorialSteps);
    }

    public override void PutAway()
    {
        if (itemModel)
            itemModel.SetActive(false);

        TutorialManager.Instance.HideAllTutorials();
    }
}
