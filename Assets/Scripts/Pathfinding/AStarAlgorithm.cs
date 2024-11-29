 using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class AStarAlgorithm : NetworkBehaviour
{
    public CreatureAI Creature;
    
    public Transform Seeker, Target;
    
    AStarGrid grid;

    private void Awake()
    {
        grid = GetComponent<AStarGrid>();
    }

    public List<GridUnit> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        GridUnit startUnit = grid.UnitFromWorldPoint(startPos);
        GridUnit targetUnit = grid.UnitFromWorldPoint(targetPos);

        List<GridUnit> path = new List<GridUnit>();

        if (startUnit.IsWalkable && targetUnit.IsWalkable)
        {
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
                    RetracePath(startUnit, targetUnit, ref path);
                    return path;
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

        return path;
    }

    void RetracePath(GridUnit startUnit, GridUnit endUnit, ref List<GridUnit> path)
    {
        GridUnit currentUnit = endUnit;

        while (currentUnit != startUnit)
        {
            path.Add(currentUnit);
            currentUnit = currentUnit.Parent;
        }

        path.Reverse();
    }
    int GetDistance (GridUnit unitA, GridUnit unitB)
    {
        int distX = Mathf.Abs(unitA.GridX - unitB.GridX);
        int distY = Mathf.Abs(unitA.GridY - unitB.GridY);
        int distZ = Mathf.Abs(unitA.GridZ - unitB.GridZ);

        return 10 * (distX + distY + distZ);
    }
}
