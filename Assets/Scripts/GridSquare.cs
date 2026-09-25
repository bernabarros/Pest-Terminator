using UnityEngine;

/// <summary>
/// Controls the state and visual appearance of a grid square.
/// Also handles player presence and jump interactions.
/// </summary>
public class GridSquare : MonoBehaviour
{
    private bool playerPresent;

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

    [Header("Danger Visuals")]
    [SerializeField] private GameObject dangerSprite;

    [Header("Changing To Danger Visuals")]
    [SerializeField] private GameObject changingToDangerSprite;

    private void Awake()
    {
        squareRenderer = GetComponent<Renderer>();
    }

    public void SetPlayerPresent(bool present)
    {
        playerPresent = present;
    }

    public void SetMaterials(
        Material empty,
        Material target,
        Material danger)
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
            if (targetSprites == null ||
                targetSprites.Length == 0)
            {
                Debug.LogWarning(
                    $"{gameObject.name}: No target sprites assigned."
                );

                return;
            }

            GameObject prefab = targetSprites[
                Random.Range(0, targetSprites.Length)
            ];

            if (prefab == null)
            {
                Debug.LogError(
                    $"{gameObject.name}: One of the target sprite prefabs is null."
                );

                continue;
            }

            GameObject visual = Instantiate(
                prefab,
                targetVisualParent
            );

            visual.transform.localPosition = new Vector3(
                Random.Range(-0.4f, 0.4f),
                1.2f,
                Random.Range(-0.4f, 0.4f)
            );
        }
    }

    private void ShowDangerVisual()
    {
        ClearTargetVisual();

        if (dangerSprite == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: No danger sprite assigned."
            );

            return;
        }

        GameObject visual = Instantiate(
            dangerSprite,
            targetVisualParent
        );

        visual.transform.localPosition = new Vector3(
            0f,
            1.2f,
            0f
        );
    }

    private void ShowChangingToDangerVisual()
    {
        ClearTargetVisual();

        if (changingToDangerSprite == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: No changing-to-danger sprite assigned."
            );

            return;
        }

        GameObject visual = Instantiate(
            changingToDangerSprite,
            targetVisualParent
        );

        visual.transform.localPosition = new Vector3(
            0f,
            1.2f,
            0f
        );
    }

    private void ClearTargetVisual()
    {
        if (targetVisualParent == null)
        {
            return;
        }

        for (int i = targetVisualParent.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                targetVisualParent.GetChild(i).gameObject
            );
        }
    }

    private void UpdateVisual()
    {
        if (squareRenderer == null)
        {
            return;
        }

        switch (squareState)
        {
            case SquareState.Empty:

                //squareRenderer.material = emptyMaterial;
                ClearTargetVisual();

                break;

            case SquareState.Target:

                //squareRenderer.material = emptyMaterial;
                ShowTargetVisual();

                break;

            case SquareState.ChangingToDanger:

                //squareRenderer.material = emptyMaterial;
                ShowChangingToDangerVisual();

                break;

            case SquareState.Danger:

                //squareRenderer.material = emptyMaterial;
                ShowDangerVisual();

                break;
        }
    }

    /// <summary>
    /// Called when both feet land on this square
    /// after the player has jumped.
    /// </summary>
    public void PlayerJumpedOn()
    {
        Debug.Log(
            $"Player jumped onto {gameObject.name}"
        );

        if (squareState == SquareState.Target)
        {
            if (gridManager != null)
            {
                gridManager.TargetInteracted(this);
            }
        }
    }

    public void PlayerEntered()
    {
        playerPresent = true;
    }

    public void PlayerExited()
    {
        playerPresent = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        MocapPlayer player =
            other.GetComponentInParent<MocapPlayer>();

        if (player == null)
        {
            return;
        }

        PlayerEntered();

        /*
         * Notice that we deliberately DON'T interact
         * with the target here.
         *
         * Walking onto a target does nothing.
         */
    }

    private void OnTriggerExit(Collider other)
    {
        MocapPlayer player =
            other.GetComponentInParent<MocapPlayer>();

        if (player == null)
        {
            return;
        }

        PlayerExited();
    }
}
