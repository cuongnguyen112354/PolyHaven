using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractObject : MonoBehaviour
{
    [HideInInspector] public static GameObject focusingObject;

    [SerializeField] private Image targetIcon;
    [SerializeField] private Sprite defalutTargetIcon;

    private int layerMask;
    private TMP_Text interactionInfo_Text;

    const float MAX_INTERACTION_DISTANCE = 1.75f;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Interact.performed += _ => { Interact(); };
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        layerMask = ~LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (!GameManager.Instance.CompareGameState("Playing") ||
            !Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, MAX_INTERACTION_DISTANCE, layerMask, QueryTriggerInteraction.Ignore) ||
            hit.collider.gameObject.GetComponent<IInteractable>() == null)
        {
            interactionInfo_Text.gameObject.SetActive(false);
            
            targetIcon.sprite = defalutTargetIcon;
            targetIcon.SetNativeSize();

            return;
        }

        focusingObject = hit.collider.gameObject;

        (targetIcon.sprite, interactionInfo_Text.text) = focusingObject.GetComponent<IInteractable>().HowInteract();
        targetIcon.SetNativeSize();
        interactionInfo_Text.gameObject.SetActive(true);
    }

    private void Interact()
    {
        if (focusingObject.TryGetComponent<IInteractable>(out var obj))
            obj.Interact();
    }

    public void SetInteractionText(TMP_Text _TMP_Text)
    {
        interactionInfo_Text = _TMP_Text;
    }
}