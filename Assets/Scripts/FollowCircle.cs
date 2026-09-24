using System.Reflection;
using UnityEngine;

public class FollowCircle : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float speed = 1f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;
    //[SerializeField] private float jumpYThreshold = 0.1f;

    private CharacterController controller;
    private Vector3 velocity;

    private float targetStartingY;

    private GridSquare currentSquare;

    [SerializeField] private CircleScript pointer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (target != null)
        {
            // Store the pointer's starting height.
            // This prevents an immediate jump when the game starts.
            targetStartingY = target.position.y;
        }
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        bool wasGrounded = controller.isGrounded;

        FollowTarget();

        HandleJump();

        HandleGravity();

        bool isGrounded = controller.isGrounded;

        if (!wasGrounded && isGrounded)
        {
            DetectLandingSquare();
        }

        if (wasGrounded && !isGrounded)
        {
            LeaveCurrentSquare();
        }
    }

    private void FollowTarget()
    {
        Vector3 direction = target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            float targetY = targetRotation.eulerAngles.y;

            Vector3 rotation = transform.eulerAngles;

            rotation.y = Mathf.LerpAngle(
                rotation.y,
                targetY,
                rotationSpeed * Time.deltaTime
            );

            transform.eulerAngles = rotation;

            controller.Move(
                direction.normalized * speed * Time.deltaTime
            );
        }
    }

    private void HandleJump()
    {
        if (!controller.isGrounded)
        {
            return;
        }

        if (pointer.JumpTriggered)
        {
            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up * velocity.y * Time.deltaTime
        );
    }

    private void DetectLandingSquare()
    {
        RaycastHit hit;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out hit,
            2f))
        {
            GridSquare square = hit.collider.GetComponent<GridSquare>();

            if (square != null)
            {
                currentSquare = square;
                currentSquare.PlayerLanded(this);
            }
        }
    }

    private void LeaveCurrentSquare()
    {
        if (currentSquare != null)
        {
            currentSquare.PlayerLeft();
            currentSquare = null;
        }
    }
}