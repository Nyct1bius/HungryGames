using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectReady : NetworkBehaviour
{ 
    private Dictionary<ulong, bool> playerReadyDictionary;
    [SerializeField] TextMeshProUGUI playerReadyCounterUI;
    private NetworkVariable<int> playersReady = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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
        playersReady.Value++;
        AttPlayerReadyCounterClientRpc(playersReady.Value);
        if (allActivesClientsReady)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }
    }
    [ClientRpc]
    private void AttPlayerReadyCounterClientRpc(int playersReady)
    {
       
        playerReadyCounterUI.text = $" {playersReady} / 2";
    }
}
