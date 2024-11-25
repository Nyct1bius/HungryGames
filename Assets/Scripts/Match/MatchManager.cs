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
    [SerializeField] private GameObject hud;
    private int index;
    private Transform currentPlayer;


    private NetworkVariable<int> hostPoints = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> clientPoints = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    public event Action OnFinishedMatch;
    private void Awake()
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
            clientPoints.Value++;

            if (clientPoints.Value == pointsToWin)
            {
                AtualizePlacarClientServerRpc(clientPoints.Value);
                SpawnVictoryClientRpc(1);
            }
            else
            {            
                AtualizePlacarClientServerRpc(clientPoints.Value);
            }
            
        }
        else
        {
            hostPoints.Value++;
            if (hostPoints.Value == pointsToWin)
            {
                AtualizePlacarHostServerRpc(hostPoints.Value);
                SpawnVictoryClientRpc(0);
            }
            else
            {       
                AtualizePlacarHostServerRpc(hostPoints.Value);
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
    private void SpawnVictoryClientRpc(int winnerPlayer)
    {
        Instantiate(victoryTabs[winnerPlayer], hud.transform);
        OnFinishedMatch?.Invoke();
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