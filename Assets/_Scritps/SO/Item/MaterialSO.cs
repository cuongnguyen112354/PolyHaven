using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterial", menuName = "ScriptableObjects/Items/Material")]
public class MaterialSO : ItemSO
{
    public override void SpawnItem()
    { 
        TutorialManager.Instance.HideAllTutorials();    
    }
    public override void DespawnItem() { }

    public override void PutAway() { }
    public override void TakeOut() { }
}