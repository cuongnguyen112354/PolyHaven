using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [Header("Item Properties")]
    public string itemName;
    public string description;
    public Sprite icon;

    [Header("Item Stats")]
    public int maxStackSize = 0;

    [Header("How to Interact")]
    public Sprite targetIcon;
    public string textTutorial => $"[E] to {typeTextTutorial}";
    [SerializeField] private TypeTextTutorial typeTextTutorial;

    public abstract void SpawnItem();
    public abstract void DespawnItem();
    public abstract void TakeOut();
    public abstract void PutAway();
}

public enum TypeTextTutorial
{
    Pickup,
    Interact
}