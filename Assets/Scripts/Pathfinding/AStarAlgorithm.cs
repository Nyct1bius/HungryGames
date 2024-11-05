using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    AStarGrid grid;

    private void Awake()
    {
        grid = GetComponent<AStarGrid>();   
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
                return;
            }

            foreach (GridUnit neighbour in grid.GetNeighbours(currentUnit))
            {
                if (!neighbour.IsWalkable || closedUnits.Contains(neighbour))
                {
                    continue;
                }
            }
        }
    }
}
