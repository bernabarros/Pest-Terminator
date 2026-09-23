using UnityEngine;
using UnityEngine.InputSystem;

public class CircleScript : MonoBehaviour
{
    [SerializeField] private bool useMouse;

    private Mouse mouse;

    private void Awake()
    {
        mouse = Mouse.current;
    }

    private void Update()
    {
        if (!useMouse || mouse == null || !mouse.leftButton.isPressed)
        {
            return;
        }

        Vector2 screenPosition = mouse.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 worldPosition = ray.GetPoint(enter);
            Vector3 position = transform.position;
            position.x = worldPosition.x;
            position.z = worldPosition.z;
            transform.position = position;
        }
    }
}
