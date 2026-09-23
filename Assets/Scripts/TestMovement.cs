using UnityEngine;
using UnityEngine.InputSystem;

public class TestMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.5f;

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    private CharacterController controller;
    private Vector3 velocity;

    private bool wasGrounded;

    private GridSquare currentSquare;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool wasGrounded = controller.isGrounded;

        Move();

        HandleGravityAndJump();

        //CheckForLanding();

        bool isGrounded = controller.isGrounded;

        if (!wasGrounded && isGrounded)
        {
            DetectLandingSquare();
        }

        if(wasGrounded && !isGrounded)
        {
            LeaveCurrentSquare();
        }
    }

    private void Move()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 movement = new Vector3(input.x, 0f, input.y);
        movement = movement.normalized;

        controller.Move(movement * moveSpeed * Time.deltaTime);
    }

    private void HandleGravityAndJump()
    {
        bool grounded = controller.isGrounded;

        if (grounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (/*grounded &&*/ jumpAction.action.WasPressedThisFrame())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        wasGrounded = grounded;
    }

    private void CheckForLanding()
    {
        bool grounded = controller.isGrounded;

        if(!wasGrounded && grounded)
        {
            DetectLandingSquare();
        }
    }

    private void DetectLandingSquare()
    {
        RaycastHit hit;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        if(Physics.Raycast(rayOrigin, Vector3.down, out hit, 2f))
        {
            GridSquare square = hit.collider.GetComponent<GridSquare>();

            if(square != null)
            {
                currentSquare = square;
                currentSquare.PlayerLanded(this);
            }
        }
    }

    private void LeaveCurrentSquare()
    {
        if(currentSquare != null)
        {
            currentSquare.PlayerLeft();
            currentSquare = null;
        }
    }
}
