using UnityEngine;

public abstract class ObjectLife : MonoBehaviour, IInteractable
{
    public ObjectSO objectSO;

    public (Sprite, string) HowInteract()
    {
        return (objectSO.targetIcon, objectSO.textTutorial);
    }

    public string GetItemName()
    {
        return objectSO.objectName;
    }

    public abstract void Affected(int damage);
    public abstract void Die();
}
