using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CreatureAI : NetworkBehaviour
{
    public Transform[] PathfindingTargets;
    private Transform chosenPathfindingTarget;
    public float MovementSpeed;
    private float timeToMove;
    Vector3[] path;
    int pathfindingTargetIndex;
    public AStarAlgorithm AStarAlgorithm;


    private void Awake()
    {
        chosenPathfindingTarget = PathfindingTargets[Random.Range(0, PathfindingTargets.Length)];
    }

    private void Start()
    {
        timeToMove = Random.Range(7, 15);
    }

    private void Update()
    {
        if (!IsServer) return;
        if (timeToMove > 0)
        {
            timeToMove -= Time.deltaTime;
        }
        if (timeToMove <= 0)
        {
            chosenPathfindingTarget = PathfindingTargets[Random.Range(0, PathfindingTargets.Length)];

            FindPath();
        }

        Debug.Log(timeToMove);
    }

    void FindPath()
    {
        Debug.Log("Find Path");
        
        List<GridUnit> gridPath = AStarAlgorithm.FindPath(transform.position, chosenPathfindingTarget.position);

        path = new Vector3[gridPath.Count];
        for (int i = 0; i < gridPath.Count; i++)
        {
            path[i] = gridPath[i].WorldPos;
        }

        pathfindingTargetIndex = 0;

        timeToMove = Random.Range(10, 21);

        if (path.Length > 0)
        {
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        Debug.Log("Follow Path");
        
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
