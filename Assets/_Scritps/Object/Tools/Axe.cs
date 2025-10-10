using UnityEngine;

public class Axe : MonoBehaviour, IEquipment
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
        inputActions.Equipment.Use.started -= _ => { usingBtnDown = true; };
        inputActions.Equipment.Use.canceled -= _ => { usingBtnDown = false; };

        inputActions.Disable();
    }

    void Update()
    {
        Use();
    }

    void HitEvent()
    {
        if (InteractObject.focusingObject &&
            InteractObject.focusingObject.TryGetComponent<ChoppableObject>(out var choppableObject))
        {
            choppableObject.Interact();
        }
    }

    private void DoneUsingEvent()
    {
        isUsing = false;
    }

    private void Use()
    {
        if (GameManager.Instance.CompareGameState("Playing"))
        {
            if (usingBtnDown && !isUsing)
            {
                isUsing = true;

                animator.SetTrigger("use");
            }
        }
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
