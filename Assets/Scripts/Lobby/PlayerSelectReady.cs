using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectReady : NetworkBehaviour
{ 
    private Dictionary<ulong, bool> playerReadyDictionary;
    private void Awake()
    {
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allActivesClientsReady = true;
        foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!playerReadyDictionary.ContainsKey(clientID) || !playerReadyDictionary[clientID])
            {
                allActivesClientsReady = false;
                break;
            } 
        }
        if(allActivesClientsReady)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("TesteM", LoadSceneMode.Single);
        }
    }
}
