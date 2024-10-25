using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private PlayerDatabase playersDatabase;
    private PlayerStats player1, player2;

    private void Start()
    {
        player1 = playersDatabase._players[0];
        player2 = playersDatabase._players[1];
    }
    public GameObject GetLosingPlayer()
    {
        if(player1.score > player2.score)
        {
            return player1.playerHolder;
        }
        return player2.playerHolder;
    }
}
