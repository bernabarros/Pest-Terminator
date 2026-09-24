using UnityEngine;
using UnityEngine.InputSystem;

public class CircleScript : MonoBehaviour
{
    [SerializeField] private bool useMouse;

    [SerializeField] private float testingJumpPointerHeight = 1f;

    [SerializeField] private float jumpThreshold = 0.1f;
    private Mouse mouse;

    private float normalY;

    private float previousY;

    private bool jumpTriggered;

    public bool JumpTriggered => jumpTriggered;

    private void Awake()
    {
        mouse = Mouse.current;
    }

    private void Start()
    {
        normalY = transform.position.y;
        previousY = transform.position.y;
    }

    private void Update()
    {
        jumpTriggered = false;

        if (useMouse && mouse != null)
        {
            HandleMouseMovement();
        }

        DetectJump();
    }

    private void HandleMouseMovement()
    {
        Vector2 screenPosition = mouse.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 worldPosition = ray.GetPoint(enter);

            Vector3 position = transform.position;

            position.x = worldPosition.x;
            position.z = worldPosition.z;

            // Right mouse button raises the pointer.
            if (mouse.rightButton.isPressed)
            {
                position.y = normalY + testingJumpPointerHeight;
            }
            else
            {
                position.y = normalY;
            }

            transform.position = position;
        }
    }

    private void DetectJump()
    {
        float yDifference = transform.position.y - previousY;

        if(yDifference > jumpThreshold)
        {
            jumpTriggered = true;
        }
        previousY = transform.position.y;
    }
}
