using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager localInstance;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI hostPlacar;
    [SerializeField] private TextMeshProUGUI clientPlacar;
    [SerializeField] private TextMeshProUGUI startMatchCountdownUI;
    [SerializeField] private GameObject counterPanel;
    [SerializeField] private GameObject[] victoryTabs;
    [Header("Match variables")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int pointsToWin;
    [SerializeField] private GameObject hud;
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private int timeToStartMatch;
    [Header("Players Connected")]
    public Transform[] connectedPlayer;
    public GameObject pauseMenuUI;
    private int index = 0;
    private Transform currentPlayer;


    private NetworkVariable<int> hostPoints = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> clientPoints = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);


    public event Action OnFinishedMatch, OnStartMatch;
    
    private void Awake()
    {
        localInstance = this;
    }
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.NetworkConfig.ClientConnectionBufferTimeout = 3000;
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
        StartCoroutine(waitToStartMatch());
    }
    private void OnDisable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
    }
    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
       foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab, spawnPoints[index].position, Quaternion.identity);
            connectedPlayer[index] = playerTransform;
            index++;
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    IEnumerator waitToStartMatch()
    {
        int time = timeToStartMatch;
        startMatchCountdownUI.text = time.ToString();
        while (time > 0)
        {
            startMatchCountdownUI.text = time.ToString();
            yield return new WaitForSeconds(1f);
            time --;
        }
        startMatchCountdownUI.text = "DUEL!";
       yield return new WaitForSeconds(1f);
        counterPanel.SetActive(false);
         OnStartMatch?.Invoke();
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
                AtualizePlacarClientRpc(clientPoints.Value);
                SpawnVictoryClientRpc(1);
                StartCoroutine(WaitToGoBackToLobby());
            }
            else
            {
                AtualizePlacarClientRpc(clientPoints.Value);
            }
            
        }
        else
        {
            hostPoints.Value++;
            if (hostPoints.Value == pointsToWin)
            {
                AtualizePlacarHostClientRpc(hostPoints.Value);
                SpawnVictoryClientRpc(0);
                StartCoroutine(WaitToGoBackToLobby());
            }
            else
            {
                AtualizePlacarHostClientRpc(hostPoints.Value);
            }
        }
    }
    IEnumerator WaitToGoBackToLobby()
    {
        yield return new WaitForSeconds(3);
        BackToLobbyServerRpc();
    }

    [ClientRpc]
    private void AtualizePlacarHostClientRpc(int value)
    {
        hostPlacar.text = $"{value}";
    }
    [ClientRpc]
    private void AtualizePlacarClientRpc(int value)
    {
        clientPlacar.text = $"{value}";
    }
    [ClientRpc]
    private void SpawnVictoryClientRpc(int winnerPlayer)
    {
        Instantiate(victoryTabs[winnerPlayer], hud.transform);
        OnFinishedMatch?.Invoke();
    }
    [ServerRpc]
    private void BackToLobbyServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
    IEnumerator RevivePlayer(GameObject deadPlayer, GameObject playerMesh)
    {
        yield return new WaitForSeconds(5f);
        int randomNunber = UnityEngine.Random.Range(0, spawnPoints.Length);
        Debug.Log(spawnPoints[randomNunber]);
        deadPlayer.transform.GetComponent<PlayerStatsManager>().Revive();
        deadPlayer.transform.position = spawnPoints[randomNunber].position;
        playerMesh.transform.position = new Vector3(spawnPoints[randomNunber].position.x, spawnPoints[randomNunber].position.y - 1.134f, spawnPoints[randomNunber].position.z);

    }
}