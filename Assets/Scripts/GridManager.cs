using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Material emptyAreaColor;
    [SerializeField] private Material targetAreaColor;
    [SerializeField] private Material dangerAreaColor;

    [SerializeField] private float dangerChangeTime = 10f;
    [SerializeField] private float dangerWarningTime = 3f;
    [SerializeField] private float flashSpeed = 0.15f;
    [SerializeField] private float laserChangeTime = 20f;

    private GridSquare currentTarget;
    private GridSquare currentDanger;
    private GridSquare nextDanger;
    private TestMovement player;
    private bool isReloading;

    private int score = 0;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject[] gameGrid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        InitialiseGrid();
        ChooseInitialTarget();
        ChooseInitialDanger();

        player = FindFirstObjectByType<TestMovement>();

        if(scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        StartCoroutine(DangerCycle());
        StartCoroutine(LaserCycle());
    }

    // Update is called once per frame
    private void Update()
    {
        if (player == null)
        {
            return;
        }

        GridSquare square = GetClosestSquareToPlayer();

        if (square != null && square.State == SquareState.Danger)
        {
            PlayerHitDanger();
        }
    }

    private GridSquare GetClosestSquareToPlayer()
    {
        GridSquare closestSquare = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject gridObject in gameGrid)
        {
            GridSquare square = gridObject.GetComponent<GridSquare>();

            if (square == null)
            {
                continue;
            }

            Vector3 offset = square.transform.position - player.transform.position;
            offset.y = 0f;
            float distance = offset.sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSquare = square;
            }
        }

        return closestSquare;
    }

    private void InitialiseGrid()
    {
        foreach(GameObject gridObject in gameGrid)
        {
            GridSquare square = gridObject.GetComponent<GridSquare>();

            square.SetMaterials(emptyAreaColor, targetAreaColor, dangerAreaColor);

            square.SetState(SquareState.Empty);
        }
    }

    private void ChooseInitialTarget()
    {
        GridSquare square = GetRandomAvailableSquare();

        if (square == null)
        {
            return;
        }

        currentTarget = square;
        currentTarget.SetState(SquareState.Target);
    }

    private void ChooseInitialDanger()
    {
        GridSquare square = GetRandomAvailableSquare();

        if(square == null)
        {
            return;
        }

        currentDanger = square;
        currentDanger.SetState(SquareState.Danger);
    }

    private void Laser()
    {
        List<GridSquare> squares = new List<GridSquare>();
        List<Vector2Int> coordinates = new List<Vector2Int>();
        List<GridSquare> laserSquares = new List<GridSquare>();

        foreach (GameObject gridObject in gameGrid)
        {
            string[] coordinateParts = gridObject.name.Split('x');

            if (coordinateParts.Length != 2 ||
                !int.TryParse(coordinateParts[0], out int x) ||
                !int.TryParse(coordinateParts[1], out int y))
            {
                continue;
            }

            GridSquare square = gridObject.GetComponent<GridSquare>();

            if (square == null)
            {
                continue;
            }

            squares.Add(square);
            coordinates.Add(new Vector2Int(x, y));
        }

        bool horizontal = Random.value < 0.5f;
        int line = Random.Range(1, 4);

        for (int index = 0; index < squares.Count; index++)
        {
            Vector2Int coordinate = coordinates[index];
            bool belongsToLine = horizontal
                ? coordinate.y == line
                : coordinate.x == line;

            if (belongsToLine)
            {
                laserSquares.Add(squares[index]);
            }
        }

        StartCoroutine(ActivateLaser(laserSquares));
    }

    private IEnumerator ActivateLaser(List<GridSquare> laserSquares)
    {
        float laserWarningTime = dangerWarningTime * 0.5f;
        float laserHoldTime = (dangerChangeTime - dangerWarningTime) * 0.5f;
        float elapsed = 0f;
        bool flashing = false;

        while (elapsed < laserWarningTime)
        {
            flashing = !flashing;

            foreach (GridSquare square in laserSquares)
            {
                square.SetState(flashing
                    ? SquareState.ChangingToDanger
                    : SquareState.Empty);
            }

            yield return new WaitForSeconds(flashSpeed);
            elapsed += flashSpeed;
        }

        foreach (GridSquare square in laserSquares)
        {
            square.SetState(SquareState.Danger);
        }

        yield return new WaitForSeconds(laserHoldTime);

        foreach (GridSquare square in laserSquares)
        {
            if (square.State == SquareState.Danger)
            {
                square.SetState(SquareState.Empty);
            }
        }
    }

    private IEnumerator LaserCycle()
    {
        while(true)
        {
            yield return new WaitForSeconds(laserChangeTime);
            Laser();
        }
    }

    private GridSquare GetRandomAvailableSquare()
    {
        List<GridSquare> availableSquares = new List<GridSquare>();

        foreach (GameObject gridObject in gameGrid)
        {
            GridSquare square = gridObject.GetComponent<GridSquare>();

            if(square == null)
            {
                continue;
            }

            if(square.PlayerPresent)
            {
                continue;
            }

            if(square == currentTarget)
            {
                continue;
            }

            if(square == currentDanger)
            {
                continue;
            }

            if(square == nextDanger)
            {
                continue;
            }

            availableSquares.Add(square);
        }

        if(availableSquares.Count == 0)
        {
            return null;
        }

        return availableSquares[Random.Range(0, availableSquares.Count)];
    }

    public void TargetInteracted(GridSquare square)
    {
        if(square != currentTarget)
        {
            return;
        }

        score += 100;

        if(scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        currentTarget.SetState(SquareState.Empty);

        GridSquare newTarget = GetRandomAvailableSquare();

        if(newTarget == null)
        {
            Debug.LogWarning("Could not find a new target.");
            return;
        }

        currentTarget = newTarget;
        currentTarget.SetState(SquareState.Target);
    }

    public void PlayerHitDanger()
    {
        if (isReloading)
        {
            return;
        }

        isReloading = true;
        StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator DangerCycle()
    {
        while(true)
        {
            yield return new WaitForSeconds(dangerChangeTime - dangerWarningTime);

            nextDanger = GetRandomAvailableSquare();

            if(nextDanger == null)
            {
                Debug.LogWarning("Could not find next danger square.");
                continue;
            }

            yield return StartCoroutine(FlashSquare(nextDanger));

            if(currentDanger != null)
            {
                currentDanger.SetState(SquareState.Empty);
            }

            currentDanger = nextDanger;
            currentDanger.SetState(SquareState.Danger);

            nextDanger = null;
        }
    }

    private IEnumerator FlashSquare(GridSquare square)
    {
        float elapsed = 0f;
        bool flashing = false;

        while (elapsed < dangerWarningTime)
        {
            flashing = !flashing;

            if(flashing)
            {
                square.SetState(SquareState.ChangingToDanger);
            }
            else
            {
                square.SetState(SquareState.Empty);
            }

            yield return new WaitForSeconds(flashSpeed);

            elapsed += flashSpeed;
        }

        square.SetState(SquareState.Danger);
    }
}