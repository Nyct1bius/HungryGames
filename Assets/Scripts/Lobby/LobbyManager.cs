using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private int index;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void OnDisable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
    }
    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().Spawn(true);
            PlayerToSpawnLocation(playerTransform);
        }
    }
    public void PlayerToSpawnLocation(Transform player)
    {

        if (index < spawnPoints.Length)
        {
            player.position = spawnPoints[index].position;
            player.rotation = spawnPoints[index].rotation;
            index++;
        }
        else
        {
            index = 0;
            player.position = spawnPoints[index].position;
            player.rotation = spawnPoints[index].rotation;
            index++;
        }

    }
}
