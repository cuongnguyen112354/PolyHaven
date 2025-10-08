using UnityEngine;

public class Food : MonoBehaviour, IEquipment
{
    [SerializeField] private Animator animator;

    private InputSystem_Actions inputActions;

    private bool isUsing, usingBtnDown = false;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        isUsing = false;

        inputActions.Enable();

        inputActions.Equipment.Use.started += _ => { usingBtnDown = true; };
        inputActions.Equipment.Use.canceled += _ => { usingBtnDown = false; };
    }

    void OnDisable()
    {
        inputActions.Disable();

        inputActions.Equipment.Use.started -= _ => { usingBtnDown = true; };
        inputActions.Equipment.Use.canceled -= _ => { usingBtnDown = false; };
    }

    void Update()
    {
        Use();
    }

    private void ConsumeEvent()
    {
        InventoryManager.Instance.UseSelectingItem();
    }

    private void DoneUsingEvent()
    {
        isUsing = false;
    }

    private void Use()
    {
        if (GameManager.Instance.CompareGameState("Playing"))
        {
            if (usingBtnDown && !isUsing && InventoryManager.Instance.CheckSelectingItemQuantity())
            {
                isUsing = true;

                AudioManager.Instance.PlayAudioClip("apple_eat");
                animator.SetTrigger("use");
            }
        }
    }
    
    public void Despawn()
    {
        Destroy(gameObject);
    }
}
