using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardPlayerTest : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string jumpTriggerName = "Jump";

    //[SerializeField] private KeyCode jumpKey = KeyCode.Space;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        if (input.sqrMagnitude > 1f)
            input.Normalize();

        if (input.sqrMagnitude < 0.01f)
            return;

        Vector3 direction = new Vector3(
            input.x,
            0f,
            input.y
        );

        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (animator == null)
        {
            Debug.LogWarning($"{gameObject.name}: No Animator found.");
            return;
        }

        animator.SetTrigger(jumpTriggerName);
    }
}