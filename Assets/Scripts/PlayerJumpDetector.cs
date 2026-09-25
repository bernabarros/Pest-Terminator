using UnityEngine;

public class PlayerJumpDetector : MonoBehaviour
{
    [Header("Foot References")]
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeightThreshold = 0.15f;
    [SerializeField] private float landingHeightTolerance = 0.15f;

    [Header("Foot Detection")]
    [SerializeField] private LayerMask gridLayerMask;
    [SerializeField] private float footRaycastDistance = 1f;

    private float standingLeftFootY;
    private float standingRightFootY;

    private bool isAirborne;
    private bool jumpDetected;

    private bool initialised;

    public bool IsAirborne => isAirborne;
    public bool JumpDetected => jumpDetected;

    private bool leftFootHasRaised;
    private bool rightFootHasRaised;

    public void Initialise(
        Transform leftFootTransform,
        Transform rightFootTransform)
    {
        if (leftFoot == null)
        {
            leftFoot = leftFootTransform;
        }

        if (rightFoot == null)
        {
            rightFoot = rightFootTransform;
        }

        if (leftFoot == null || rightFoot == null)
        {
            Debug.LogError(
                $"{gameObject.name}: PlayerJumpDetector requires both feet."
            );

            return;
        }

        CalibrateStandingHeight();

        initialised = true;
    }

    private void Update()
    {
        if (!initialised)
        {
            return;
        }

        DetectJump();
    }

    //For MocapPlayer script
    
    private void DetectJump()
    {
        bool leftFootRaised =
            leftFoot.position.y >
            standingLeftFootY + jumpHeightThreshold;

        bool rightFootRaised =
            rightFoot.position.y >
            standingRightFootY + jumpHeightThreshold;

        
         // Both feet must rise above the standing height.
         // This prevents normal walking from being considered
         // a jump.
        
        if (!isAirborne)
        {
            if (leftFootRaised && rightFootRaised)
            {
                StartJump();
            }

            return;
        }

        
         // Once airborne, look for both feet returning
         // to the floor.
        
        TryDetectLanding();
    }
    

    //For KeyboardPlayer Script
    /*
    private void DetectJump()
    {
        bool leftFootRaised =
            leftFoot.position.y >
            standingLeftFootY + jumpHeightThreshold;

        bool rightFootRaised =
            rightFoot.position.y >
            standingRightFootY + jumpHeightThreshold;

        if (!isAirborne)
        {
            if (leftFootRaised)
            {
                leftFootHasRaised = true;
            }

            if (rightFootRaised)
            {
                rightFootHasRaised = true;
            }

            
            // Both feet have participated in the jump.
            
            if (leftFootHasRaised && rightFootHasRaised)
            {
                StartJump();
            }

            return;
        }

        TryDetectLanding();
    }
    */

    private void StartJump()
    {
        isAirborne = true;
        jumpDetected = true;

        Debug.Log("Jump detected.");
    }

    private void TryDetectLanding()
    {
        if (!TryGetSquareUnderFoot(
            leftFoot,
            out GridSquare leftSquare,
            out float leftDistance))
        {
            return;
        }

        if (!TryGetSquareUnderFoot(
            rightFoot,
            out GridSquare rightSquare,
            out float rightDistance))
        {
            return;
        }

        /*
         * Both feet must be close enough to the floor.
         */
        if (leftDistance > landingHeightTolerance ||
            rightDistance > landingHeightTolerance)
        {
            return;
        }

        /*
         * Both feet must be on the same square.
         */
        if (leftSquare != rightSquare)
        {
            return;
        }

        LandedOnSquare(leftSquare);
    }

    private bool TryGetSquareUnderFoot(
        Transform foot,
        out GridSquare square,
        out float distance)
    {
        square = null;
        distance = float.MaxValue;

        Vector3 origin = foot.position + Vector3.up * 0.05f;

        if (!Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hit,
            footRaycastDistance,
            gridLayerMask))
        {
            return false;
        }

        square = hit.collider.GetComponent<GridSquare>();

        if (square == null)
        {
            return false;
        }

        distance = foot.position.y - hit.point.y;

        return true;
    }

    private void LandedOnSquare(GridSquare square)
    {
        Debug.Log(
            $"Jump landed on {square.gameObject.name}"
        );

        isAirborne = false;

        if (jumpDetected)
        {
            square.PlayerJumpedOn();
        }

        jumpDetected = false;

        //For KeyboardPlayerScript
        leftFootHasRaised = false;
        rightFootHasRaised = false;
    }

    public void CalibrateStandingHeight()
    {
        if (leftFoot == null || rightFoot == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Cannot calibrate foot height because both feet are required."
            );

            return;
        }

        standingLeftFootY = leftFoot.position.y;
        standingRightFootY = rightFoot.position.y;

        Debug.Log(
            $"Foot height calibrated. " +
            $"Left: {standingLeftFootY:F3}, " +
            $"Right: {standingRightFootY:F3}"
        );
    }
}
