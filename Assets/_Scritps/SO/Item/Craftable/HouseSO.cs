using UnityEngine;

[CreateAssetMenu(fileName = "NewHouse", menuName = "ScriptableObjects/Items/House")]
public class HouseSO : CraftableSO
{
    private GameObject itemModel;

    public override void SpawnItem()
    {
        if (itemModel)
        {
            TakeOut();
            return;
        }

        Transform parentTransform = ConstructionManager.Instance.transform;

        itemModel = Instantiate(Resources.Load<GameObject>($"_Models/Houses/{itemName}"),
            parentTransform.position, Quaternion.Euler(parentTransform.eulerAngles));

        itemModel.transform.SetParent(parentTransform);
        itemModel.name = itemName;
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
