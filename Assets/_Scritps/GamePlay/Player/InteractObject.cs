using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractObject : MonoBehaviour
{
    [HideInInspector] public static GameObject focusingObject;

    [SerializeField] private float maxInteractionDistance = 1.75f;

    [Header("Icons")]
    [SerializeField] private Image targetIcon;
    [SerializeField] private Sprite defalutTargetIcon;

    private int layerMask;
    private TMP_Text interactionInfo_Text;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.MainInteract.performed += _ => MainInteract();
        inputActions.Player.Destroy.performed += _ => DestroyRequest();
    }

    void OnDisable()
    {
        inputActions.Player.MainInteract.performed -= _ => MainInteract();
        inputActions.Player.Destroy.performed -= _ => DestroyRequest();

        inputActions.Disable();
    }

    void Start()
    {
        layerMask = ~LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (!GameManager.Instance.CompareGameState("Playing") ||
            !Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxInteractionDistance, layerMask, QueryTriggerInteraction.Ignore) ||
            (hit.collider.gameObject.GetComponent<IInteractable>() == null & hit.collider.gameObject.GetComponent<ISubInteractable>() == null))
        {
            if (focusingObject)
            {
                if (focusingObject.GetComponent<ISubInteractable>() != null)
                    TutorialManager.Instance.HideAllTutorials();
            }

            focusingObject = null;
            
            interactionInfo_Text.gameObject.SetActive(false);

            targetIcon.sprite = defalutTargetIcon;
            targetIcon.SetNativeSize();

            return;
        }

        focusingObject = hit.collider.gameObject;

        if (focusingObject.TryGetComponent<IInteractable>(out var item))
        {
            (targetIcon.sprite, interactionInfo_Text.text) = item.HowInteract();
            targetIcon.SetNativeSize();
            interactionInfo_Text.gameObject.SetActive(true);
        }

        if (focusingObject.TryGetComponent<DestroyableObject>(out var prop))
        {
            prop.HowInteract();
        }
    }

    private void MainInteract()
    {
        if (focusingObject == null || !GameManager.Instance.CompareGameState("Playing")) return;

        if (focusingObject.TryGetComponent<PickableObject>(out var item))
            item.Affected();
        else if (focusingObject.TryGetComponent<Chest>(out var chest))
            chest.Affected();
    }
    private void DestroyRequest()
    {
        if (focusingObject == null || !GameManager.Instance.CompareGameState("Playing")) return;

        if (focusingObject.TryGetComponent<DestroyableObject>(out var prop))
            prop.DestroyRequest();
    }


    public void SetInteractionText(TMP_Text _TMP_Text)
    {
        interactionInfo_Text = _TMP_Text;
    }
}