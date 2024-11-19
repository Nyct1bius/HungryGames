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
    private void Start()
    {
        localInstance = this;
    }
    public void PlayerToSpawnLocation(Transform player)
    {
        if (!IsOwner) return;
            currentPlayer = player;
        if (index < spawnPoints.Length)
        {
            MovePlayerToSpawnServerRpc();
            index++;
        }
        else
        {
            index = 0;
            MovePlayerToSpawnServerRpc();
            index++;
        }

    }
    [ServerRpc]
    private void MovePlayerToSpawnServerRpc()
    {
       
        MovePlayerToSpawnClientRpc();
    }
    [ClientRpc]
    private void MovePlayerToSpawnClientRpc()
    {

        currentPlayer.position = spawnPoints[index].position;
    }
}
