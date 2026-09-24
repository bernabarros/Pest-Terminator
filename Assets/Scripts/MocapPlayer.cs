using UnityEngine;

public class MocapPlayer : MonoBehaviour
{

    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    [SerializeField] private PlayerJumpDetector jumpDetector;

    public Transform LeftFoot => leftFoot;
    public Transform RightFoot => rightFoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (jumpDetector == null)
        {
            jumpDetector = GetComponent<PlayerJumpDetector>();
        }
    }
    void Start()
    {
        if (jumpDetector == null)
        {
            Debug.LogError(
                $"{gameObject.name}: No PlayerJumpDetector assigned."
            );

            return;
        }

        jumpDetector.Initialise(leftFoot, rightFoot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
