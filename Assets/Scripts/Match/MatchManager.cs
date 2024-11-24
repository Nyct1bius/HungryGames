using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager localInstance;
    [SerializeField] private TextMeshProUGUI hostPlacar;
    [SerializeField] private TextMeshProUGUI clientPlacar;
    [SerializeField] private GameObject[] victoryTabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int pointsToWin;
    private int index;
    private Transform currentPlayer;


    private NetworkVariable<int> hostPoints = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> clientPoints = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        localInstance = this;
    }
    public void PlayerToSpawnLocation(GameObject player)
    {
        
        if (index < spawnPoints.Length)
        {
            player.transform.position = spawnPoints[index].position;
            index++;
        }
        else
        {
            index = 0;
            player.transform.position = spawnPoints[index].position;
            index++;
        }

    }

    public void PlayerDied(GameObject deadPlayer,GameObject playerMesh,ulong playerID)
    {
        if (clientPoints.Value != pointsToWin)
        {
            CheckWitchPlayerDiedServerRpc(playerID);
            StartCoroutine(RevivePlayer(deadPlayer, playerMesh));
        }

       
    }
    [ServerRpc(RequireOwnership = false)]
    private void CheckWitchPlayerDiedServerRpc(ulong playerID)
    {
        Debug.Log(playerID);
        if (playerID == 0)
        {
            if(clientPoints.Value == pointsToWin)
            {
                Debug.Log("Winner");
                SetActiveVictoryClientRpc(1);
            }
            else
            {
                clientPoints.Value++;
                AtualizePlacarClientServerRpc(clientPoints.Value);
                Debug.Log(clientPoints.Value);
            }
            
        }
        else
        {
            if(hostPoints.Value == pointsToWin)
            {
                SetActiveVictoryClientRpc(0);
            }
            else
            {
                hostPoints.Value++;
                AtualizePlacarHostServerRpc(hostPoints.Value);
                Debug.Log(hostPoints.Value);
            }
        }
    }
    [ServerRpc]
    private void AtualizePlacarHostServerRpc(int value)
    {
        AtualizePlacarHostClientRpc(value);
    }
    [ClientRpc]
    private void AtualizePlacarHostClientRpc(int value)
    {
        hostPlacar.text = value.ToString();
    }
    [ServerRpc]
    private void AtualizePlacarClientServerRpc(int value)
    {
        AtualizePlacarClientRpc(value);
    }
    [ClientRpc]
    private void AtualizePlacarClientRpc(int value)
    {
        clientPlacar.text = value.ToString();
    }
    [ClientRpc]
    private void SetActiveVictoryClientRpc(int winnerPlayer)
    {
        victoryTabs[winnerPlayer].SetActive(true);
    }
    IEnumerator RevivePlayer(GameObject deadPlayer, GameObject playerMesh)
    {
        yield return new WaitForSeconds(5f);
        int randomNunber = UnityEngine.Random.Range(0, spawnPoints.Length + 1);
        deadPlayer.transform.GetComponent<PlayerStatsManager>().Revive();
        deadPlayer.transform.position = spawnPoints[randomNunber].position;
        playerMesh.transform.position = new Vector3(spawnPoints[randomNunber].position.x, spawnPoints[randomNunber].position.y - 1.134f, spawnPoints[randomNunber].position.z);

    }
}