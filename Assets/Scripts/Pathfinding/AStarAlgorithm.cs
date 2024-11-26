using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    public CreatureAI Creature;
    
    public Transform Seeker, Target;
    
    AStarGrid grid;

    private void Awake()
    {
        grid = GetComponent<AStarGrid>();
    }

    public void StartFindPath (Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool success = false;   
        
        GridUnit startUnit = grid.UnitFromWorldPoint(startPos);
        GridUnit targetUnit = grid.UnitFromWorldPoint(targetPos);

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
                    success = true;
                    break;
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

        yield return null;

        if (success)
        {
            waypoints = RetracePath(startUnit, targetUnit);
        }
    }

    Vector3[] RetracePath(GridUnit startUnit, GridUnit endUnit)
    {
        List<GridUnit> path = new List<GridUnit>();
        GridUnit currentUnit = endUnit;

        while (currentUnit != startUnit)
        {
            path.Add(currentUnit);
            currentUnit = currentUnit.Parent;
        }

        Vector3[] waypoints = SimplifyPath(path);

        Array.Reverse(waypoints);     
        return waypoints;
    }

    Vector3[] SimplifyPath(List<GridUnit> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 0; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridZ - path[i].GridZ);

            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].WorldPos);
            }

            directionOld = directionNew;    
        }

        return waypoints.ToArray();
    }

    int GetDistance (GridUnit unitA, GridUnit unitB)
    {
        int distX = Mathf.Abs(unitA.GridX - unitB.GridX);
        int distY = Mathf.Abs(unitA.GridY - unitB.GridY);
        int distZ = Mathf.Abs(unitA.GridZ - unitB.GridZ);

        return 10 * (distX + distY + distZ);
    }
}
