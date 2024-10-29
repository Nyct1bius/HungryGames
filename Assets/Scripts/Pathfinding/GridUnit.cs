using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit
{
    public bool IsObstacle;
    public Vector3 WorldPos;

    public GridUnit(bool _isObstacle, Vector3 _worldPos)
    {
        IsObstacle = _isObstacle;
        WorldPos = _worldPos;
    }
}
