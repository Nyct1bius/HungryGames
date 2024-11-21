using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager localInstance;
    [SerializeField] private Transform[] spawnPoints;
    private int index;
    private Transform currentPlayer;
    public override void OnNetworkSpawn()
    {
        localInstance = this;
    }
    public void PlayerToSpawnLocation(Transform player)
    {
        if (!IsOwner) return;
        if (index < spawnPoints.Length)
        {
            player.position = spawnPoints[index].position;
            index++;
        }
        else
        {
            index = 0;
            player.transform.position = spawnPoints[index].position;
            index++;
        }

    }
}
