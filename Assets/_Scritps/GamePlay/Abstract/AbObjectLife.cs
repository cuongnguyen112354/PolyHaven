using UnityEngine;

public abstract class AbObjectLife : MonoBehaviour, IInteractable
{
    [SerializeField] protected Transform[] spawnPoints;

    [SerializeField] protected ObjectSO objectSO;

    protected int currenthealth;
    protected Animator animator;

    void Start()
    {
        currenthealth = objectSO.maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

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
