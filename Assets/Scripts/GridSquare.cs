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

    [SerializeField] private GameObject[] targetSprites;
    [SerializeField] private Transform targetVisualParent;

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

    private void ShowTargetVisual()
    {
        ClearTargetVisual();

        int spriteCount = Random.Range(1, 4);

            for (int i = 0; i < spriteCount; i++)
        {
            GameObject prefab = targetSprites[
                Random.Range(0, targetSprites.Length)
            ];

            if (prefab == null)
            {
                Debug.LogError($"{gameObject.name}: One of the target sprite prefabs is null.");
                continue;
            }

            GameObject visual = Instantiate(
                prefab,
                targetVisualParent
            );

            visual.transform.localPosition = new Vector3(
                Random.Range(-0.4f, 0.4f),
                1f,
                Random.Range(-0.4f, 0.4f)
            );
        }
    }

    private void ClearTargetVisual()
    {
        for (int i = targetVisualParent.childCount - 1; i >= 0; i--)
        {
            Destroy(targetVisualParent.GetChild(i).gameObject);
        }
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
                ClearTargetVisual();
                break;

            case SquareState.Target:
                squareRenderer.material = emptyMaterial;
                ShowTargetVisual();
                break;

            case SquareState.ChangingToDanger:
                squareRenderer.material = dangerMaterial;
                ClearTargetVisual();
                break;

            case SquareState.Danger:
                squareRenderer.material = dangerMaterial;
                ClearTargetVisual();
                break;
        }
    }

    public void PlayerLanded(FollowCircle player)
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
        FollowCircle player = other.GetComponent<FollowCircle>();

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
        FollowCircle player = other.GetComponent<FollowCircle>();

        if(player == null)
        {
            return;
        }

        playerPresent = false;
    }
}
