using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit
{
    public bool IsWalkable;
    public Vector3 WorldPos;
    public int GridX, GridY, GridZ;

    public int GCost, HCost;

    public GridUnit Parent;

    public GridUnit(bool _isWalkable, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ)
    {
        IsWalkable = _isWalkable;
        WorldPos = _worldPos;

        GridX = _gridX;
        GridY = _gridY;
        GridZ = _gridZ;
    }

    public int FCost
    {
        get
        {
            return GCost + HCost;
        }
    }

    public override string ToString()
    {
        return GridX + "," + GridY + "," + GridZ;
    }
}
