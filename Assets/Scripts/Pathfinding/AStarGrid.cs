using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public LayerMask ObstacleMask;
    public Vector2 GridWorldSize;
    public float UnitRadius;
    GridUnit[,] grid;

    float unitDiameter;
    int gridSizeX, gridSizeY;

    private void Start()
    {
        unitDiameter = UnitRadius * 2;

        gridSizeX = Mathf.RoundToInt(GridWorldSize.x / unitDiameter);
        gridSizeY = Mathf.RoundToInt(GridWorldSize.y / unitDiameter);

        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new GridUnit[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.forward * GridWorldSize.y / 2;
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * unitDiameter + UnitRadius) + Vector3.forward * (y * unitDiameter + UnitRadius);

                bool walkable = !(Physics.CheckSphere(worldPoint, UnitRadius, ObstacleMask));

                grid[x, y] = new GridUnit(walkable, worldPoint, x, y);
            }
        }
    }

    public List<GridUnit> GetNeighbours(GridUnit unit)
    {
        List<GridUnit> neighbous = new List<GridUnit>();    

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = unit.GridX + x;
                int checkY = unit.GridY + y;

                if (checkX >= 0 && checkY < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbous.Add(grid[checkX, checkY]);
                }
            }   
        }

        return neighbous;
    }

    public GridUnit UnitFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float percentY = (worldPos.z + GridWorldSize.y / 2) / GridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentX = Mathf.Clamp01(percentX);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, 1, GridWorldSize.y));

        if (grid != null)
        {
            foreach (GridUnit u in grid)
            {
                Gizmos.color = (u.IsWalkable) ? Color.white : Color.red;
                Gizmos.DrawCube(u.WorldPos, Vector3.one * (unitDiameter - 0.1f));
            }
        }
    }
}
