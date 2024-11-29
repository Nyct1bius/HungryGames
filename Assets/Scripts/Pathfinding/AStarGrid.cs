using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AStarGrid : NetworkBehaviour
{
    public LayerMask ObstacleMask;
    public Vector3 GridWorldSize;
    public float UnitRadius;
    GridUnit[,,] grid;

    float unitDiameter;
    int gridSizeX, gridSizeY, gridSizeZ;

    private void Awake()
    {
        if (!IsServer) return;
        unitDiameter = UnitRadius * 2;

        gridSizeX = Mathf.RoundToInt(GridWorldSize.x / unitDiameter);
        gridSizeY = Mathf.RoundToInt(GridWorldSize.y / unitDiameter);
        gridSizeZ = Mathf.RoundToInt(GridWorldSize.z / unitDiameter);

        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new GridUnit[gridSizeX, gridSizeY, gridSizeZ];

        Vector3 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.up * GridWorldSize.y / 2 - Vector3.forward * GridWorldSize.z / 2;
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = worldBottomLeft 
                                         + Vector3.right * (x * unitDiameter + UnitRadius) 
                                         + Vector3.up * (y * unitDiameter + UnitRadius) 
                                         + Vector3.forward * (z * unitDiameter + UnitRadius);

                    bool walkable = !(Physics.CheckSphere(worldPoint, UnitRadius, ObstacleMask));

                    grid[x, y, z] = new GridUnit(walkable, worldPoint, x, y, z);
                }
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
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }

                    int checkX = unit.GridX + x;
                    int checkY = unit.GridY + y;
                    int checkZ = unit.GridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbous.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }   
        }

        return neighbous;
    }

    public GridUnit UnitFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float percentY = (worldPos.y + GridWorldSize.y / 2) / GridWorldSize.y;
        float percentZ = (worldPos.z + GridWorldSize.z / 2) / GridWorldSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        return grid[x, y, z];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.y, GridWorldSize.z));

        if (grid != null)
        {
            foreach (GridUnit u in grid)
            {
                Gizmos.color = (u.IsWalkable) ? Color.clear : Color.red;
                Gizmos.DrawCube(u.WorldPos, Vector3.one * (unitDiameter - 0.1f));
            }
        }
    }
}
