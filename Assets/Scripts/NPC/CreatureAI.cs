using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    public Transform PathfindingTarget;
    public float MovementSpeed;
    Vector3[] path;
    int pathfindingTargetIndex;
    AStarAlgorithm AStarAlgorithm;

    private void Start()
    {
        AStarAlgorithm.StartFindPath(transform.position, PathfindingTarget.position);
    }
}
