using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    public Transform PathfindingTarget;
    public float MovementSpeed, TimeToMove;
    Vector3[] path;
    int pathfindingTargetIndex;
    public AStarAlgorithm AStarAlgorithm;

    private void Start()
    {
        FindPath();
    }

    private void Update()
    {
        if (TimeToMove > 0)
        {
            TimeToMove -= Time.deltaTime;
        }
        else
        {
            
        }
    }

    void FindPath()
    {
        List<GridUnit> gridPath = AStarAlgorithm.FindPath(transform.position, PathfindingTarget.position);

        path = new Vector3[gridPath.Count];
        for (int i = 0; i < gridPath.Count; i++)
        {
            path[i] = gridPath[i].WorldPos;
        }

        pathfindingTargetIndex = 0;

        if (path.Length > 0)
        {
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentUnit = path[0];

        while (true)
        {
            if (transform.position == currentUnit)
            {
                pathfindingTargetIndex++;

                if (pathfindingTargetIndex >= path.Length)
                {
                    yield break;
                }

                currentUnit = path[pathfindingTargetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentUnit, MovementSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
