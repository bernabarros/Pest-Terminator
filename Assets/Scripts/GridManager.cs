using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Material emptyAreaColor;
    [SerializeField] private Material targetAreaColor;
    [SerializeField] private Material dangerAreaColor;

    [SerializeField] private float dangerChangeTime = 10f;
    [SerializeField] private float dangerWarningTime = 3f;
    [SerializeField] private float flashSpeed = 0.15f;

    private GridSquare currentTarget;
    private GridSquare currentDanger;
    private GridSquare nextDanger;

    [SerializeField] private GameObject[] gameGrid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void InitialiseGrid()
    {
        foreach(GameObject gridObject in gameGrid)
        {
            GridSquare square = gridObject.GetComponent<GridSquare>();

            if(square == null)
            {
                Debug.LogError($"{gridObject.name} is missing a GridSquare component.");

                continue;
            }

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
                square.SetState(SquareState.Danger);
            }
            else
            {
                square.SetState(SquareState.Empty);
            }

            yield return new WaitForSeconds(flashSpeed);

            elapsed += flashSpeed;
        }

        square.SetState(SquareState.Empty);
    }
}