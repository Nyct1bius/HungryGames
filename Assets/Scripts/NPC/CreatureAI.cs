using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CreatureAI : NetworkBehaviour
{
    public Transform[] PathfindingTargets;
    private Transform chosenPathfindingTarget;
    public float MovementSpeed;
    private float timeToMove, timeToEnableInteractable;
    Vector3[] path;
    int pathfindingTargetIndex;
    public AStarAlgorithm AStarAlgorithm;
    public MatchManager MatchManager;
    private bool foundPlayers = false;
    private Transform chosenPlayer;
    //public GameObject[] Buffs, Debuffs;


    public override void OnNetworkSpawn()
    {
        chosenPathfindingTarget = PathfindingTargets[Random.Range(0, PathfindingTargets.Length)];
        timeToMove = Random.Range(7, 15);
        timeToEnableInteractable = 15;
    }

    private void Update()
    {
        if (!IsHost) return;
        if (timeToMove > 0)
        {
            timeToMove -= Time.deltaTime;
        }
        else
        {
            chosenPathfindingTarget = PathfindingTargets[Random.Range(0, PathfindingTargets.Length)];
            chosenPlayer = MatchManager.connectedPlayers[Random.Range(0, MatchManager.connectedPlayers.Length)];

            foundPlayers = true;

            FindPath();
        }

        if (foundPlayers)
        {
            transform.LookAt(chosenPlayer);
        }

        //EnableInteractables();
    }

    void FindPath()
    {
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

    void EnableInteractables()
    {
        if (timeToEnableInteractable <= 0)
        {
            //Buffs[Random.Range(0, Buffs.Length)].SetActive(true);
            //Debuffs[Random.Range(0, Buffs.Length)].SetActive(true);

            timeToEnableInteractable = 15;
        }
        else
        {
            timeToEnableInteractable -= Time.deltaTime;
        }
    }
}
