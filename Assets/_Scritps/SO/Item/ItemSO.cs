using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [Header("Item Properties")]
    public string itemName;
    public string description;
    public Sprite icon;

    [Header("Item Stats")]
    public int maxStackSize = 0;

    public abstract void SpawnItem();
    public abstract void DespawnItem();
    public abstract void TakeOut();
    public abstract void PutAway();
}