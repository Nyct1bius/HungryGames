using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    public Transform Seeker, Target;
    
    AStarGrid grid;

    private void Awake()
    {
        grid = GetComponent<AStarGrid>();   
    }

    private void Update()
    {
        FindPath(Seeker.position, Target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        GridUnit startUnit = grid.UnitFromWorldPoint(startPos);
        GridUnit targetUnit = grid.UnitFromWorldPoint(targetPos);

        List<GridUnit> openUnits = new List<GridUnit>();
        HashSet<GridUnit> closedUnits = new HashSet<GridUnit>();    

        openUnits.Add(startUnit);

        while (openUnits.Count > 0)
        {
            GridUnit currentUnit = openUnits[0];

            for (int i = 1; i < openUnits.Count; i++)
            {
                if (openUnits[i].FCost < currentUnit.FCost || openUnits[i].FCost == currentUnit.FCost && openUnits[i].HCost < currentUnit.HCost)
                {
                    currentUnit = openUnits[i];
                }
            }

            openUnits.Remove(currentUnit);
            closedUnits.Add(currentUnit);

            if (currentUnit == targetUnit)
            {            
                RetracePath(startUnit, targetUnit);
                return;
            }

            foreach (GridUnit neighbour in grid.GetNeighbours(currentUnit))
            {
                if (!neighbour.IsWalkable || closedUnits.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentUnit.GCost + GetDistance(currentUnit, neighbour); 

                if (newMovementCostToNeighbour < neighbour.GCost || !openUnits.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetUnit);
                    neighbour.Parent = currentUnit;

                    if (!openUnits.Contains(neighbour))
                    {
                        openUnits.Add(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(GridUnit startUnit, GridUnit endUnit)
    {
        List<GridUnit> path = new List<GridUnit>();
        GridUnit currentUnit = endUnit;

        while (currentUnit != startUnit)
        {
            path.Add(currentUnit);
            currentUnit = currentUnit.Parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance (GridUnit unitA, GridUnit unitB)
    {
        int distX = Mathf.Abs(unitA.GridX - unitB.GridX);
        int distY = Mathf.Abs(unitA.GridY - unitB.GridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
