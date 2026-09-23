using UnityEngine;

/// <summary>
/// Controls Grid Squares State, checks if player is on it and which material it uses
/// </summary>
public class GridSquare : MonoBehaviour
{
    /// <summary>
    /// Returns a boolean depending on whether the player is standing on the square
    /// </summary>
    private bool playerPresent;
    /// <summary>
    /// Enum that returns the square's state, it can be Empty, Target or Danger
    /// </summary>
    private SquareState squareState = SquareState.Empty;

    public bool PlayerPresent => playerPresent;
    public SquareState State => squareState;

    private Renderer squareRenderer;

    private Material emptyMaterial;
    private Material targetMaterial;
    private Material dangerMaterial;

    [SerializeField] private GridManager gridManager;

    private void Awake()
    {
        squareRenderer = GetComponent<Renderer>();
    }

    public void SetPlayerPresent(bool present)
    {
        playerPresent = present;
    }

    public void SetMaterials(Material empty, Material target, Material danger)
    {
        emptyMaterial = empty;
        targetMaterial = target;
        dangerMaterial = danger;

        UpdateVisual();
    }

    public void SetState(SquareState newState)
    {
        squareState = newState;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if(squareRenderer == null)
        {
            return;
        }

        switch(squareState)
        {
            case SquareState.Empty:
                squareRenderer.material = emptyMaterial;
                break;

            case SquareState.Target:
                squareRenderer.material = targetMaterial;
                break;

            case SquareState.Danger:
                squareRenderer.material = dangerMaterial;
                break;
        }
    }

    public void PlayerLanded(TestMovement player)
    {
        playerPresent = true;

        Debug.Log($"Player landed on {gameObject.name}");

        if(squareState == SquareState.Target)
        {
            if(gridManager != null)
            {
                gridManager.TargetInteracted(this);
            }
        }
    }

    public void PlayerLeft()
    {
        playerPresent = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        TestMovement player = other.GetComponent<TestMovement>();

        if(player == null)
        {
            return;
        }

        playerPresent = true;

        if(squareState == SquareState.Danger)
        {
            if(gridManager != null)
            {
                gridManager.PlayerHitDanger();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TestMovement player = other.GetComponent<TestMovement>();

        if(player == null)
        {
            return;
        }

        playerPresent = false;
    }
}
