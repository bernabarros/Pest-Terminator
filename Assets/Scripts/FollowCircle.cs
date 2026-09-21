using UnityEngine;

public class FollowCircle : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float targetY = targetRotation.eulerAngles.y;

            Vector3 rotation = transform.eulerAngles;
            rotation.y = Mathf.LerpAngle(rotation.y, targetY, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = rotation;

            transform.position += direction.normalized * speed * Time.deltaTime;
        }
    }
}