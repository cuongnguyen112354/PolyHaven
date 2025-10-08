using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementTest : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Look Settings")]
    [SerializeField] private float maxLookAngle = 80f;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    
    private CharacterController characterController;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    // private void OnDisable()
    // {
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    // }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // InteractObject interactObject = playerCamera.GetComponent<InteractObject>();
        // interactObject.SetInteractionText(GameManager.Instance.interactionInfo_Text);

        Cursor.lockState = CursorLockMode.Locked;
    }

    // protected override void OnSpawned()
    // {
    //     base.OnSpawned();

    //     enabled = isOwner;

    //     if (!isOwner)
    //     {
    //         Destroy(playerCamera.gameObject);
    //         return;
    //     }

    //     characterController = GetComponent<CharacterController>();
    //     InteractObject interactObject = playerCamera.GetComponent<InteractObject>();
    //     interactObject.SetInteractionText(GameManager.Instance.interactionInfo_Text);

    //     if (playerCamera == null)
    //     {
    //         enabled = false;
    //         return;
    //     }
    // }

    // protected override void OnDespawned()
    // {
    //     base.OnDespawned();

    //     if (!isOwner)
    //         return;

    //     OutGame();
    // }

    // void OutGame()
    // {
    //     enabled = false;
    //     Destroy(gameObject);
    // }

    private void Update()
    {
        // if (GameManager.Instance.CompareGameState("Pause") ||
        //     GameManager.Instance.CompareGameState("Loading"))
        //     return;

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        bool isGrounded = IsGrounded();
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // if (!GameManager.Instance.CompareGameState("Playing")) return;

        float mouseX = Input.GetAxis("Mouse X") * 2;
        float mouseY = Input.GetAxis("Mouse Y") * 2;

        // float mouseX = Input.GetAxis("Mouse X") * 2;
        // float mouseY = Input.GetAxis("Mouse Y") * 2; 

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.03f, Vector3.down * groundCheckDistance);
    }
#endif
}